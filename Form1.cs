using System;
using System.IO;
using System.Windows.Forms;
using CurrentAnalyzer;
using ScottPlot.WinForms;
using System.Linq;
using ScottPlot;
using VoltageAnalyzer;

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
            //SetupEventHandlers();
        }

        private void InitializePlots()
        {
            plotCurrents.Plot.Title("���� ���");
            plotCurrents.Plot.ShowLegend();
            plotCurrentsSequence.Plot.Title("������������ ������������");
            plotCurrentsSequence.Plot.ShowLegend();
            plotVoltage.Plot.Title("���������� ���");
            plotVoltage.Plot.ShowLegend();
            plotVoltageSequence.Plot.Title("������������ ������������");
            plotVoltageSequence.Plot.ShowLegend();
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
                Title = "�������� ���� COMTRADE"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _currentsAnalyzer = new FaultCurrentAnalyzer(openFileDialog.FileName);
                    _voltageAnalyzer = new ThreeVoltageAnalyzer(openFileDialog.FileName);
                    PlotCurrents();
                    PlotCurrentsSequenceComponents();
                    PlotVoltage();
                    PlotVoltageSequenceComponents();
                    ShowFaultInfo();
                    statusLabel.Text = $"�������� ����: {Path.GetFileName(openFileDialog.FileName)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������: {ex.Message}", "������",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusLabel.Text = "������ �������� �����";
                }
            }
        }

        private void PlotCurrents()
        {
            plotCurrents.Plot.Clear();

            double[] timeSeconds = _currentsAnalyzer.TimeStamps
                .Select(t => (t - _currentsAnalyzer.TimeStamps[0]).TotalSeconds)
                .ToArray();

            var sigA = plotCurrents.Plot.Add.SignalXY(timeSeconds, _currentsAnalyzer.CurrentA.ToArray());
            sigA.LegendText = "��� A";
            var sigB = plotCurrents.Plot.Add.SignalXY(timeSeconds, _currentsAnalyzer.CurrentB.ToArray());
            sigB.LegendText = "��� B";
            var sigC = plotCurrents.Plot.Add.SignalXY(timeSeconds, _currentsAnalyzer.CurrentC.ToArray());
            sigC.LegendText = "��� C";

            plotCurrents.Plot.Axes.AutoScale();
            plotCurrents.Plot.ShowLegend(Edge.Bottom);
            plotCurrents.Refresh();
        }

        private void PlotCurrentsSequenceComponents()
        {
            plotCurrentsSequence.Plot.Clear();

            double[] timeSeconds = _currentsAnalyzer.TimeStamps
                .Select(t => (t - _currentsAnalyzer.TimeStamps[0]).TotalSeconds)
                .ToArray();

            var sig0 = plotCurrentsSequence.Plot.Add.SignalXY(timeSeconds, _currentsAnalyzer.ZeroSequence.ToArray());
            sig0.LegendText = "������� ������������������";
            var sig1 = plotCurrentsSequence.Plot.Add.SignalXY(timeSeconds, _currentsAnalyzer.PositiveSequence.ToArray());
            sig1.LegendText = "������ ������������������";
            var sig2 = plotCurrentsSequence.Plot.Add.SignalXY(timeSeconds, _currentsAnalyzer.NegativeSequence.ToArray());
            sig2.LegendText = "�������� ������������������";

            plotCurrentsSequence.Plot.Axes.AutoScale();
            plotCurrentsSequence.Plot.ShowLegend(Edge.Bottom);
            plotCurrentsSequence.Refresh();
        }

        private void PlotVoltage()
        {
            plotVoltage.Plot.Clear();

            double[] timeSeconds = _voltageAnalyzer.TimeStamps
                .Select(t => (t - _voltageAnalyzer.TimeStamps[0]).TotalSeconds)
                .ToArray();

            var sigA = plotVoltage.Plot.Add.SignalXY(timeSeconds, _voltageAnalyzer.VoltageA.ToArray());
            sigA.LegendText = "���������� A";
            var sigB = plotVoltage.Plot.Add.SignalXY(timeSeconds, _voltageAnalyzer.VoltageB.ToArray());
            sigB.LegendText = "���������� B";
            var sigC = plotVoltage.Plot.Add.SignalXY(timeSeconds, _voltageAnalyzer.VoltageC.ToArray());
            sigC.LegendText = "���������� C";

            plotVoltage.Plot.Axes.AutoScale();
            plotVoltage.Plot.ShowLegend(Edge.Bottom);
            plotVoltage.Refresh();
        }

        private void PlotVoltageSequenceComponents()
        {
            plotVoltageSequence.Plot.Clear();

            double[] timeSeconds = _voltageAnalyzer.TimeStamps
                .Select(t => (t - _voltageAnalyzer.TimeStamps[0]).TotalSeconds)
                .ToArray();

            var sig0 = plotVoltageSequence.Plot.Add.SignalXY(timeSeconds, _voltageAnalyzer.ZeroSequence.ToArray());
            sig0.LegendText = "������� ������������������";
            var sig1 = plotVoltageSequence.Plot.Add.SignalXY(timeSeconds, _voltageAnalyzer.PositiveSequence.ToArray());
            sig1.LegendText = "������ ������������������";

            plotVoltageSequence.Plot.Axes.AutoScale();
            plotVoltageSequence.Plot.ShowLegend(Edge.Bottom);
            plotVoltageSequence.Refresh();
        }

        private void ShowFaultInfo()
        {
            string info = $"��� ��: {_currentsAnalyzer.FaultType}\n" +
                         $"������� ����:\nA = {_currentsAnalyzer.PeakCurrents[0]:F2} A\n" +
                         $"B = {_currentsAnalyzer.PeakCurrents[1]:F2} A\n" +
                         $"C = {_currentsAnalyzer.PeakCurrents[2]:F2} A";

            MessageBox.Show(info, "���������� �������",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentsAnalyzer == null)
            {
                MessageBox.Show("��� ������ ��� ����������", "������",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var saveDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "��������� ���������� �������"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _currentsAnalyzer.SaveResults(saveDialog.FileName);
                    statusLabel.Text = $"���������� ��������� � {Path.GetFileName(saveDialog.FileName)}";
                    MessageBox.Show("���������� ������� ���������", "����������",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������ ����������: {ex.Message}", "������",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}