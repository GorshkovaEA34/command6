using System;
using System.IO;
using System.Windows.Forms;
using CurrentAnalyzer;
using ScottPlot.WinForms;
using System.Linq;

namespace prarktikateam6
{
    public partial class Form1 : Form
    {
        private FaultCurrentAnalyzer _analyzer;

        public Form1()
        {
            InitializeComponent();
            InitializePlots();
            SetupEventHandlers();
        }

        private void InitializePlots()
        {
            plotCurrents.Plot.Title("Токи фаз");
            plotCurrents.Plot.ShowLegend();
            plotSequence.Plot.Title("Симметричные составляющие");
            plotSequence.Plot.ShowLegend();
        }

        private void SetupEventHandlers()
        {
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
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
                    _analyzer = new FaultCurrentAnalyzer(openFileDialog.FileName);
                    PlotCurrents();
                    PlotSequenceComponents();
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
            plotCurrents.Plot.Clear();

            double[] timeSeconds = _analyzer.TimeStamps
                .Select(t => (t - _analyzer.TimeStamps[0]).TotalSeconds)
                .ToArray();

            var sigA = plotCurrents.Plot.Add.SignalXY(timeSeconds, _analyzer.CurrentA.ToArray());
            sigA.LegendText = "Ток A";
            var sigB = plotCurrents.Plot.Add.SignalXY(timeSeconds, _analyzer.CurrentB.ToArray());
            sigB.LegendText = "Ток B";
            var sigC = plotCurrents.Plot.Add.SignalXY(timeSeconds, _analyzer.CurrentC.ToArray());
            sigC.LegendText = "Ток C";

            plotCurrents.Refresh();
        }

        private void PlotSequenceComponents()
        {
            plotSequence.Plot.Clear();

            double[] timeSeconds = _analyzer.TimeStamps
                .Select(t => (t - _analyzer.TimeStamps[0]).TotalSeconds)
                .ToArray();

            var sig0 = plotSequence.Plot.Add.SignalXY(timeSeconds, _analyzer.ZeroSequence.ToArray());
            sig0.LegendText = "Нулевая последовательность";
            var sig1 = plotSequence.Plot.Add.SignalXY(timeSeconds, _analyzer.PositiveSequence.ToArray());
            sig1.LegendText = "Прямая последовательность";
            var sig2 = plotSequence.Plot.Add.SignalXY(timeSeconds, _analyzer.NegativeSequence.ToArray());
            sig2.LegendText = "Обратная последовательность";

            plotSequence.Refresh();
        }

        private void ShowFaultInfo()
        {
            string info = $"Тип КЗ: {_analyzer.FaultType}\n" +
                         $"Пиковые токи:\nA = {_analyzer.PeakCurrents[0]:F2} A\n" +
                         $"B = {_analyzer.PeakCurrents[1]:F2} A\n" +
                         $"C = {_analyzer.PeakCurrents[2]:F2} A";

            MessageBox.Show(info, "Результаты анализа",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_analyzer == null)
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
                    _analyzer.SaveResults(saveDialog.FileName);
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