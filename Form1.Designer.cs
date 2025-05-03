namespace prarktikateam6
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private SplitContainer mainSplitContainer;
        private ScottPlot.WinForms.FormsPlot plotCurrents;
        private ScottPlot.WinForms.FormsPlot plotSequence;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // Main Menu
            this.menuStrip = new MenuStrip();
            this.fileToolStripMenuItem = new ToolStripMenuItem();
            this.openToolStripMenuItem = new ToolStripMenuItem();
            this.saveToolStripMenuItem = new ToolStripMenuItem();

            // Plots Area
            this.mainSplitContainer = new SplitContainer();
            this.plotCurrents = new ScottPlot.WinForms.FormsPlot();
            this.plotSequence = new ScottPlot.WinForms.FormsPlot();

            // Status Bar
            this.statusStrip = new StatusStrip();
            this.statusLabel = new ToolStripStatusLabel();

            // MenuStrip
            this.menuStrip.SuspendLayout();
            this.menuStrip.Items.Add(this.fileToolStripMenuItem);

            // File Menu
            this.fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.openToolStripMenuItem,
                this.saveToolStripMenuItem
            });
            this.fileToolStripMenuItem.Text = "Файл";
            this.openToolStripMenuItem.Text = "Открыть";
            this.saveToolStripMenuItem.Text = "Сохранить";

            // SplitContainer
            this.mainSplitContainer.SuspendLayout();
            this.mainSplitContainer.Dock = DockStyle.Fill;
            this.mainSplitContainer.Orientation = Orientation.Vertical;
            this.mainSplitContainer.Panel1.Controls.Add(this.plotCurrents);
            this.mainSplitContainer.Panel2.Controls.Add(this.plotSequence);

            // Plots
            this.plotCurrents.Dock = DockStyle.Fill;
            this.plotCurrents.Name = "currentsPlot";
            this.plotSequence.Dock = DockStyle.Fill;
            this.plotSequence.Name = "sequencesPlot";

            // StatusStrip
            this.statusStrip.Items.Add(this.statusLabel);
            this.statusLabel.Text = "Готово";

            // MainForm
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Text = "Анализатор токов КЗ";

            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.mainSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}