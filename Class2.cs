using Wisp.Comtrade;
using Wisp.Comtrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ScottPlot;
using System.Numerics;
using System.IO;

namespace VoltageAnalyzer
{
    class ThreeVoltageAnalyzer
    {
        private const double TWO_PI = 2 * Math.PI;
        private readonly RecordReader _comtradeData;
        private readonly ConfigurationHandler _configuration;
        private readonly int[] _voltageIndices = new int[3];
        private readonly double _nominalFrequency;

        public List<DateTime> TimeStamps { get; }
        public List<double> VoltageA { get; }
        public List<double> VoltageB { get; }
        public List<double> VoltageC { get; }
        public List<double> PositiveSequence { get; private set; }
        public List<double> ZeroSequence { get; private set; }

        public ThreeVoltageAnalyzer(string comtradeFilePath)
        {
            if (string.IsNullOrWhiteSpace(comtradeFilePath))
                throw new ArgumentException("Не указан путь к файлу");

            _configuration = new ConfigurationHandler(comtradeFilePath);
            _comtradeData = new RecordReader(comtradeFilePath);
            _nominalFrequency = _configuration.Frequency;

            TimeStamps = new List<DateTime>();
            VoltageA = new List<double>();
            VoltageB = new List<double>();
            VoltageC = new List<double>();

            FindVoltageIndices();
            ReadVoltageData();
            CalculateVoltageSequenceComponents();
        }

        private void FindVoltageIndices()
        {
            var voltageChannels = new List<int>();
            for (int i = 0; i < _configuration.AnalogChannelsCount; i++)
            {
                var channel = _configuration.AnalogChannelInformationList[i];
                if (channel.Units.ToLower().Contains("в") || channel.Units.ToLower().Contains("v"))
                {
                    voltageChannels.Add(i);
                    if (voltageChannels.Count == 3) break;
                }
            }

            if (voltageChannels.Count != 3)
                throw new InvalidOperationException("Не найдены три канала напряжения");

            Array.Copy(voltageChannels.ToArray(), _voltageIndices, 3);
        }

        private void ReadVoltageData()
        {
            VoltageA.AddRange(_comtradeData.GetAnalogPrimaryChannel(_voltageIndices[0]));
            VoltageB.AddRange(_comtradeData.GetAnalogPrimaryChannel(_voltageIndices[1]));
            VoltageC.AddRange(_comtradeData.GetAnalogPrimaryChannel(_voltageIndices[2]));

            double samplingRate = _configuration.SampleRates[0].SamplingFrequency;
            for (int i = 0; i < VoltageA.Count; i++)
            {
                TimeStamps.Add(_configuration.StartTime.AddSeconds(i / samplingRate));
            }
        }
        private void CalculateVoltageSequenceComponents()
        {
            PositiveSequence = new List<double>();
            ZeroSequence = new List<double>();

            for (int i = 0; i < VoltageA.Count; i++)
            {
                Complex va = new Complex(VoltageA[i], 0);
                Complex vb = new Complex(VoltageB[i], 0);
                Complex vc = new Complex(VoltageC[i], 0);

                Complex v0 = (va + vb + vc) / 3;
                Complex v1 = (va + Complex.FromPolarCoordinates(1, TWO_PI / 3) * vb +
                             Complex.FromPolarCoordinates(1, -TWO_PI / 3) * vc) / 3;

                ZeroSequence.Add(v0.Magnitude);
                PositiveSequence.Add(v1.Magnitude);
            }
        }
    }
}
