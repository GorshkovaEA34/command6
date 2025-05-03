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
            plotCurrents.Plot.Title("���� ���");
            plotCurrents.Plot.ShowLegend();
            plotSequence.Plot.Title("������������ ������������");
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
                Title = "�������� ���� COMTRADE"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _analyzer = new FaultCurrentAnalyzer(openFileDialog.FileName);
                    PlotCurrents();
                    PlotSequenceComponents();
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

            double[] timeSeconds = _analyzer.TimeStamps
                .Select(t => (t - _analyzer.TimeStamps[0]).TotalSeconds)
                .ToArray();

            var sigA = plotCurrents.Plot.Add.SignalXY(timeSeconds, _analyzer.CurrentA.ToArray());
            sigA.LegendText = "��� A";
            var sigB = plotCurrents.Plot.Add.SignalXY(timeSeconds, _analyzer.CurrentB.ToArray());
            sigB.LegendText = "��� B";
            var sigC = plotCurrents.Plot.Add.SignalXY(timeSeconds, _analyzer.CurrentC.ToArray());
            sigC.LegendText = "��� C";

            plotCurrents.Refresh();
        }

        private void PlotSequenceComponents()
        {
            plotSequence.Plot.Clear();

            double[] timeSeconds = _analyzer.TimeStamps
                .Select(t => (t - _analyzer.TimeStamps[0]).TotalSeconds)
                .ToArray();

            var sig0 = plotSequence.Plot.Add.SignalXY(timeSeconds, _analyzer.ZeroSequence.ToArray());
            sig0.LegendText = "������� ������������������";
            var sig1 = plotSequence.Plot.Add.SignalXY(timeSeconds, _analyzer.PositiveSequence.ToArray());
            sig1.LegendText = "������ ������������������";
            var sig2 = plotSequence.Plot.Add.SignalXY(timeSeconds, _analyzer.NegativeSequence.ToArray());
            sig2.LegendText = "�������� ������������������";

            plotSequence.Refresh();
        }

        private void ShowFaultInfo()
        {
            string info = $"��� ��: {_analyzer.FaultType}\n" +
                         $"������� ����:\nA = {_analyzer.PeakCurrents[0]:F2} A\n" +
                         $"B = {_analyzer.PeakCurrents[1]:F2} A\n" +
                         $"C = {_analyzer.PeakCurrents[2]:F2} A";

            MessageBox.Show(info, "���������� �������",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_analyzer == null)
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
                    _analyzer.SaveResults(saveDialog.FileName);
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