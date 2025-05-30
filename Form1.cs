using System;
using System.IO;
using System.Windows.Forms;
using CurrentAnalyzer;
using ScottPlot.WinForms;
using System.Linq;
using ScottPlot;
using VoltageAnalyzer;
using System.Reflection.Emit;
using System.Numerics;

namespace prarktikateam6
{
    public partial class Form1 : Form
    {
        private FaultCurrentAnalyzer _currentsAnalyzer;
        private ThreeVoltageAnalyzer _voltageAnalyzer;

        public Form1()
        {
            InitializeComponent();           
            InitializePlots();
        }

        private void InitializePlots()
        {
   // Токи фаз
            plotCurrents.Plot.Title("Токи фаз");
            plotCurrents.Plot.XLabel("Время, с");
            plotCurrents.Plot.YLabel("Ток, А");

            // Симметричные составляющие токов
            plotCurrentsSequence.Plot.Title("Симметричные составляющие токов");
            plotCurrentsSequence.Plot.XLabel("Время, с");
            plotCurrentsSequence.Plot.YLabel("Ток, А");

            // Напряжения фаз
            plotVoltage.Plot.Title("Напряжения фаз");
            plotVoltage.Plot.XLabel("Время, с");
            plotVoltage.Plot.YLabel("Напряжение, В");

            // Симметричные составляющие напряжений
            plotVoltageSequence.Plot.Title("Симметричные составляющие напряжений");
            plotVoltageSequence.Plot.XLabel("Время, с");
            plotVoltageSequence.Plot.YLabel("Напряжение, В");
        }
                
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "COMTRADE files (*.cfg)|*.cfg|All files (*.*)|*.*",
                Title = "Выберите файл COMTRADE"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _currentsAnalyzer = new FaultCurrentAnalyzer(openFileDialog.FileName);
                    _voltageAnalyzer = new ThreeVoltageAnalyzer(openFileDialog.FileName);
                    ClearAll();
                    PlotCurrents();
                    PlotCurrentsSequenceComponents();
                    PlotVoltage();
                    PlotVoltageSequenceComponents();                   
                    ShowFaultInfo();
                    statusLabel.Text = $"Загружен файл: {Path.GetFileName(openFileDialog.FileName)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusLabel.Text = "Ошибка загрузки файла";
                }
            }
        }

        private void PlotCurrents()
        {            
            double[] timeSeconds = _currentsAnalyzer.TimeStamps
                .Select(t => (t - _currentsAnalyzer.TimeStamps[0]).TotalSeconds)
                .ToArray();

            var sigA = plotCurrents.Plot.Add.SignalXY(timeSeconds, _currentsAnalyzer.CurrentA.ToArray());
            sigA.LegendText = "Ток A";
            var sigB = plotCurrents.Plot.Add.SignalXY(timeSeconds, _currentsAnalyzer.CurrentB.ToArray());
            sigB.LegendText = "Ток B";
            var sigC = plotCurrents.Plot.Add.SignalXY(timeSeconds, _currentsAnalyzer.CurrentC.ToArray());
            sigC.LegendText = "Ток C";

            plotCurrents.Plot.Axes.AutoScale();
            plotCurrents.Plot.ShowLegend(Edge.Bottom);
            plotCurrents.Refresh();
        }

        private void PlotCurrentsSequenceComponents()
        {
            double[] timeSeconds = _currentsAnalyzer.TimeStamps
                .Select(t => (t - _currentsAnalyzer.TimeStamps[0]).TotalSeconds)
                .ToArray();

            var sig0 = plotCurrentsSequence.Plot.Add.SignalXY(timeSeconds, _currentsAnalyzer.ZeroSequence.ToArray());
            sig0.LegendText = "Нулевая последовательность";
            var sig1 = plotCurrentsSequence.Plot.Add.SignalXY(timeSeconds, _currentsAnalyzer.PositiveSequence.ToArray());
            sig1.LegendText = "Прямая последовательность";
            var sig2 = plotCurrentsSequence.Plot.Add.SignalXY(timeSeconds, _currentsAnalyzer.NegativeSequence.ToArray());
            sig2.LegendText = "Обратная последовательность";

            plotCurrentsSequence.Plot.Axes.AutoScale();
            plotCurrentsSequence.Plot.ShowLegend(Edge.Bottom);
            plotCurrentsSequence.Refresh();
        }

        private void PlotVoltage()
        {
            double[] timeSeconds = _voltageAnalyzer.TimeStamps
                .Select(t => (t - _voltageAnalyzer.TimeStamps[0]).TotalSeconds)
                .ToArray();

            var sigA = plotVoltage.Plot.Add.SignalXY(timeSeconds, _voltageAnalyzer.VoltageA.ToArray());
            sigA.LegendText = "Напряжение A";
            var sigB = plotVoltage.Plot.Add.SignalXY(timeSeconds, _voltageAnalyzer.VoltageB.ToArray());
            sigB.LegendText = "Напряжение B";
            var sigC = plotVoltage.Plot.Add.SignalXY(timeSeconds, _voltageAnalyzer.VoltageC.ToArray());
            sigC.LegendText = "Напряжение C";

            plotVoltage.Plot.Axes.AutoScale();
            plotVoltage.Plot.ShowLegend(Edge.Bottom);
            plotVoltage.Refresh();
        }

        private void PlotVoltageSequenceComponents()
        {
            double[] timeSeconds = _voltageAnalyzer.TimeStamps
                .Select(t => (t - _voltageAnalyzer.TimeStamps[0]).TotalSeconds)
                .ToArray();

            var sig0 = plotVoltageSequence.Plot.Add.SignalXY(timeSeconds, _voltageAnalyzer.ZeroSequence.ToArray());
            sig0.LegendText = "Нулевая последовательность";
            var sig1 = plotVoltageSequence.Plot.Add.SignalXY(timeSeconds, _voltageAnalyzer.PositiveSequence.ToArray());
            sig1.LegendText = "Прямая последовательность";

            plotVoltageSequence.Plot.Axes.AutoScale();
            plotVoltageSequence.Plot.ShowLegend(Edge.Bottom);
            plotVoltageSequence.Refresh();
        }

        private void ClearAll() //Если использовать Plot.Clear, то старая легенда не исчезает.
                                //.Reset решает эту проблему, но с ним есть визуальный баг
        {
            // Полная очистка через пересоздание FormsPlot
            plotCurrents = RecreatePlot(plotCurrents, "Токи фаз");
            plotCurrentsSequence = RecreatePlot(plotCurrentsSequence, "Симметричные составляющие");
            plotVoltage = RecreatePlot(plotVoltage, "Напряжения фаз");
            plotVoltageSequence = RecreatePlot(plotVoltageSequence, "Симметричные составляющие");
        }

        private FormsPlot RecreatePlot(FormsPlot oldPlot, string title)
        {
            // Сохраняем параметры
            var parent = oldPlot.Parent;
            var dock = oldPlot.Dock;
            var size = oldPlot.Size;
            var location = oldPlot.Location;

            // Создаем новый контрол
            var newPlot = new FormsPlot();
            newPlot.Dock = dock;
            newPlot.Size = size;
            newPlot.Location = location;
            newPlot.Plot.Title(title);

            // Заменяем контрол
            parent.Controls.Remove(oldPlot);
            parent.Controls.Add(newPlot);

            return newPlot;
        }

        private void ShowFaultInfo()
        {
            // Рассчитываем средние значения напряжений
            double avgPositiveSeq = _voltageAnalyzer.PositiveSequence.Any() ?
                _voltageAnalyzer.PositiveSequence.Average() : 0;
            double avgZeroSeq = _voltageAnalyzer.ZeroSequence.Any() ?
                _voltageAnalyzer.ZeroSequence.Average() : 0;

            string info = $"Тип КЗ: {_currentsAnalyzer.FaultType}\n" +
                         $"Максимальные токи до КЗ:\n" +
                         $"A = {_currentsAnalyzer.PeakCurrents[3]:F2} A\n" +
                         $"B = {_currentsAnalyzer.PeakCurrents[4]:F2} A\n" +
                         $"C = {_currentsAnalyzer.PeakCurrents[5]:F2} A\n\n" +
                         $"Напряжения:\n" +
                         $"Прямая последовательность: {avgPositiveSeq:F2} В\n" +
                         $"Нулевая последовательность: {avgZeroSeq:F2} В\n\n" +
                         $"Ударные токи:\n";

            string[] phases = { "A", "B", "C" };
            for (int i = 0; i < 3; i++)
            {
                if (_currentsAnalyzer.PeakCurrents[i] == 0.0)
                    info += $"КЗ в фазе {phases[i]} отсутствует\n";
                else
                    info += $"{phases[i]} = {_currentsAnalyzer.PeakCurrents[i]:F2} A\n";
            }

            MessageBox.Show(info, "Результаты анализа",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentsAnalyzer == null)
            {
                MessageBox.Show("Нет данных для сохранения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var saveDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Сохранить результаты анализа"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _currentsAnalyzer.SaveResults(saveDialog.FileName);
                    statusLabel.Text = $"Результаты сохранены в {Path.GetFileName(saveDialog.FileName)}";
                    MessageBox.Show("Результаты успешно сохранены", "Сохранение",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}