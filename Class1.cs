using Wisp.Comtrade;
using Wisp.Comtrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ScottPlot;
using System.Numerics;
using System.IO;

namespace CurrentAnalyzer
{
    public class FaultCurrentAnalyzer
    {
        private const double TWO_PI = 2 * Math.PI;
        private readonly RecordReader _comtradeData;
        private readonly ConfigurationHandler _configuration;
        private readonly int[] _currentIndices = new int[3];
        private readonly double _nominalFrequency;

        public List<DateTime> TimeStamps { get; }
        public List<double> CurrentA { get; }
        public List<double> CurrentB { get; }
        public List<double> CurrentC { get; }
        public List<double> PeakCurrents { get; private set; }
        public List<double> MaxCurrents { get; private set; }
        public List<double> PositiveSequence { get; private set; }
        public List<double> NegativeSequence { get; private set; }
        public List<double> ZeroSequence { get; private set; }
        public string FaultType { get; private set; }

        public FaultCurrentAnalyzer(string comtradeFilePath)
        {
            if (string.IsNullOrWhiteSpace(comtradeFilePath))
                throw new ArgumentException("Не указан путь к файлу");

            _configuration = new ConfigurationHandler(comtradeFilePath);
            _comtradeData = new RecordReader(comtradeFilePath);
            _nominalFrequency = _configuration.Frequency;

            TimeStamps = new List<DateTime>();
            CurrentA = new List<double>();
            CurrentB = new List<double>();
            CurrentC = new List<double>();

            FindCurrentIndices();
            ReadCurrentData();
            AnalyzeFault();
        }

        private void FindCurrentIndices()
        {
            var currentChannels = new List<int>();
            for (int i = 0; i < _configuration.AnalogChannelsCount; i++)
            {
                var channel = _configuration.AnalogChannelInformationList[i];
                if (channel.Units.ToLower().Contains("a") || channel.Units.ToLower().Contains("а"))
                {
                    currentChannels.Add(i);
                    if (currentChannels.Count == 3) break;
                }
            }

            if (currentChannels.Count != 3)
                throw new InvalidOperationException("Не найдены три токовых канала");

            Array.Copy(currentChannels.ToArray(), _currentIndices, 3);
        }

        private void ReadCurrentData()
        {
            CurrentA.AddRange(_comtradeData.GetAnalogPrimaryChannel(_currentIndices[0]));
            CurrentB.AddRange(_comtradeData.GetAnalogPrimaryChannel(_currentIndices[1]));
            CurrentC.AddRange(_comtradeData.GetAnalogPrimaryChannel(_currentIndices[2]));

            double samplingRate = _configuration.SampleRates[0].SamplingFrequency;
            for (int i = 0; i < CurrentA.Count; i++)
            {
                TimeStamps.Add(_configuration.StartTime.AddSeconds(i / samplingRate));
            }
        }

        private void AnalyzeFault()
        {
            CalculateMaxCurrents();
            CalculatePeakCurrents();
            CalculateSequenceComponents();
            DetermineFaultType();
        }

        private void CalculateMaxCurrents()
        {
            MaxCurrents = new List<double>
            {
                CurrentA.Max(Math.Abs),
                CurrentB.Max(Math.Abs),
                CurrentC.Max(Math.Abs)
            };
        }

        private void CalculatePeakCurrents()
        {
            PeakCurrents = new List<double>{0,0,0,0,0,0};

            bool foundA = false, foundB = false, foundC = false, fA = false, fB = false, fC = false;
            double[] localPeakA = new double[CurrentA.Count];
            double[] localPeakB = new double[CurrentA.Count];
            double[] localPeakC = new double[CurrentA.Count];

            for (int i = 1; i < CurrentA.Count; i++)
            {
                if (!foundA && IsPeak(CurrentA, i))
                {
                    PeakCurrents[3] = Math.Abs(CurrentA[i]);
                    foundA = true;
                }

                if (!foundB && IsPeak(CurrentB, i))
                {
                    PeakCurrents[4] = Math.Abs(CurrentB[i]);
                    foundB = true;
                }

                if (!foundC && IsPeak(CurrentC, i))
                {
                    PeakCurrents[5] = Math.Abs(CurrentC[i]);
                    foundC = true;
                }

                if (!fA && IsPeak(CurrentA, i) && Math.Abs(CurrentA[i]) > PeakCurrents[3] * 1.5)
                {
                    localPeakA[i] = Math.Abs(CurrentA[i]);                   
                }

                if (!fB && IsPeak(CurrentB, i) && Math.Abs(CurrentB[i]) > PeakCurrents[4] * 1.5)
                {
                    localPeakB[i] = Math.Abs(CurrentB[i]);
                }

                if (!fC && IsPeak(CurrentC, i) && Math.Abs(CurrentC[i]) > PeakCurrents[5] * 1.5)
                {
                    localPeakC[i] = Math.Abs(CurrentC[i]);
                }

                if (Array.Exists(localPeakA, x => x != 0.0) && Math.Sign(CurrentA[i]) != Math.Sign(CurrentA[i - 1]))
                {
                    PeakCurrents[0] = localPeakA.Max();
                    fA = true;
                }

                if (Array.Exists(localPeakB, x => x != 0.0) && Math.Sign(CurrentB[i]) != Math.Sign(CurrentB[i - 1]))
                {
                    PeakCurrents[1] = localPeakB.Max();
                    fB = true;
                }

                if (Array.Exists(localPeakC, x => x != 0.0) && Math.Sign(CurrentC[i]) != Math.Sign(CurrentC[i - 1]))
                {
                    PeakCurrents[2] = localPeakC.Max();
                    fC = true;
                }

                if (fA && fB && fC) break;
            }
        }

        private bool IsPeak(List<double> signal, int index)
        {
            return (signal[index] > signal[index - 1] && signal[index] > signal[index + 1]) ||
            (signal[index] < signal[index - 1] && signal[index] < signal[index + 1]);
        }


        private void CalculateSequenceComponents()
        {
            PositiveSequence = new List<double>();
            NegativeSequence = new List<double>();
            ZeroSequence = new List<double>();

            for (int i = 0; i < CurrentA.Count; i++)
            {
                Complex ia = new Complex(CurrentA[i], 0);
                Complex ib = new Complex(CurrentB[i], 0);
                Complex ic = new Complex(CurrentC[i], 0);

                Complex i0 = (ia + ib + ic) / 3;
                Complex i1 = (ia + Complex.FromPolarCoordinates(1, TWO_PI / 3) * ib +
                             Complex.FromPolarCoordinates(1, -TWO_PI / 3) * ic) / 3;
                Complex i2 = (ia + Complex.FromPolarCoordinates(1, -TWO_PI / 3) * ib +
                             Complex.FromPolarCoordinates(1, TWO_PI / 3) * ic) / 3;

                ZeroSequence.Add(i0.Magnitude);
                PositiveSequence.Add(i1.Magnitude);
                NegativeSequence.Add(i2.Magnitude);
            }
        }

        private void DetermineFaultType()
        {
            if (PeakCurrents[0] != 0.0 && PeakCurrents[1] != 0.0 && PeakCurrents[2] != 0.0)
                FaultType = "Трехфазное КЗ";
            else if (PeakCurrents[0] == 0.0 && PeakCurrents[1] != 0.0 && PeakCurrents[2] != 0.0)
                FaultType = "Двухфазное - КЗ фаз B и C";
            else if (PeakCurrents[0] != 0.0 && PeakCurrents[1] == 0.0 && PeakCurrents[2] != 0.0)
                FaultType = "Двухфазное - КЗ фаз A и C";
            else if (PeakCurrents[0] != 0.0 && PeakCurrents[1] != 0.0 && PeakCurrents[2] == 0.0)
                FaultType = "Двухфазное - КЗ фаз A и B";
            else if (PeakCurrents[0] == 0.0 && PeakCurrents[1] == 0.0 && PeakCurrents[2] != 0.0)
                FaultType = "Однофазное - КЗ фазы C";
            else if (PeakCurrents[0] != 0.0 && PeakCurrents[1] == 0.0 && PeakCurrents[2] == 0.0)
                FaultType = "Однофазное - КЗ фазы A";
            else if (PeakCurrents[0] == 0.0 && PeakCurrents[1] != 0.0 && PeakCurrents[2] == 0.0)
                FaultType = "Однофазное - КЗ фазы B";
        }

        public void SaveResults(string outputPath)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Не указан путь для сохранения");

            using var writer = new StreamWriter(outputPath);
            writer.WriteLine("Время,Фаза A,Фаза B,Фаза C,I0,I1,I2");

            for (int i = 0; i < TimeStamps.Count; i++)
            {
                writer.WriteLine($"{TimeStamps[i]:O},{CurrentA[i]},{CurrentB[i]},{CurrentC[i]}," +
                               $"{ZeroSequence[i]},{PositiveSequence[i]},{NegativeSequence[i]}");
            }
        }
    }
}