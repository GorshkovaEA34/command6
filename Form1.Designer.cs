namespace prarktikateam6
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pageCurrents;
        private System.Windows.Forms.TabPage pageVoltage;
        private ScottPlot.WinForms.FormsPlot plotCurrents;
        private ScottPlot.WinForms.FormsPlot plotCurrentsSequence;
        private ScottPlot.WinForms.FormsPlot plotVoltage;
        private ScottPlot.WinForms.FormsPlot plotVoltageSequence;

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
            statusLabel = new Label();
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            openFileDialog = new OpenFileDialog();
            tabControl1 = new TabControl();
            pageCurrents = new TabPage();
            plotCurrents = new ScottPlot.WinForms.FormsPlot();
            plotCurrentsSequence = new ScottPlot.WinForms.FormsPlot();
            pageVoltage = new TabPage();
            plotVoltage = new ScottPlot.WinForms.FormsPlot();
            plotVoltageSequence = new ScottPlot.WinForms.FormsPlot();
            menuStrip.SuspendLayout();
            tabControl1.SuspendLayout();
            pageCurrents.SuspendLayout();
            pageVoltage.SuspendLayout();
            SuspendLayout();
            // 
            // statusLabel
            // 
            statusLabel.Dock = DockStyle.Bottom;
            statusLabel.Location = new Point(0, 369);
            statusLabel.Margin = new Padding(2, 0, 2, 0);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(885, 29);
            statusLabel.TabIndex = 1;
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new Padding(3, 1, 0, 1);
            menuStrip.Size = new Size(885, 24);
            menuStrip.TabIndex = 1;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, saveToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(48, 22);
            fileToolStripMenuItem.Text = "Файл";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(133, 22);
            openToolStripMenuItem.Text = "Открыть";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(133, 22);
            saveToolStripMenuItem.Text = "Сохранить";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "COMTRADE files (*.cfg)|*.cfg|All files (*.*)|*.*";
            openFileDialog.Title = "Открыть файл COMTRADE";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(pageCurrents);
            tabControl1.Controls.Add(pageVoltage);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 24);
            tabControl1.Margin = new Padding(2, 1, 2, 1);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(885, 345);
            tabControl1.TabIndex = 2;
            // 
            // pageCurrents
            // 
            pageCurrents.Controls.Add(plotCurrents);
            pageCurrents.Controls.Add(plotCurrentsSequence);
            pageCurrents.Location = new Point(4, 24);
            pageCurrents.Margin = new Padding(2, 1, 2, 1);
            pageCurrents.Name = "pageCurrents";
            pageCurrents.Padding = new Padding(2, 1, 2, 1);
            pageCurrents.Size = new Size(877, 317);
            pageCurrents.TabIndex = 0;
            pageCurrents.Text = "Токи";
            pageCurrents.UseVisualStyleBackColor = true;
            // 
            // plotCurrents
            // 
            plotCurrents.Anchor = AnchorStyles.Left;
            plotCurrents.DisplayScale = 1F;
            plotCurrents.Location = new Point(2, 3);
            plotCurrents.Margin = new Padding(2, 1, 2, 1);
            plotCurrents.Name = "plotCurrents";
            plotCurrents.Size = new Size(435, 307);
            plotCurrents.TabIndex = 0;
            // 
            // plotCurrentsSequence
            // 
            plotCurrentsSequence.Anchor = AnchorStyles.Right;
            plotCurrentsSequence.DisplayScale = 1F;
            plotCurrentsSequence.Location = new Point(435, 3);
            plotCurrentsSequence.Margin = new Padding(2, 1, 2, 1);
            plotCurrentsSequence.Name = "plotCurrentsSequence";
            plotCurrentsSequence.Size = new Size(435, 307);
            plotCurrentsSequence.TabIndex = 1;
            // 
            // pageVoltage
            // 
            pageVoltage.Controls.Add(plotVoltage);
            pageVoltage.Controls.Add(plotVoltageSequence);
            pageVoltage.Location = new Point(4, 24);
            pageVoltage.Margin = new Padding(2, 1, 2, 1);
            pageVoltage.Name = "pageVoltage";
            pageVoltage.Padding = new Padding(2, 1, 2, 1);
            pageVoltage.Size = new Size(877, 332);
            pageVoltage.TabIndex = 1;
            pageVoltage.Text = "Напряжения";
            pageVoltage.UseVisualStyleBackColor = true;
            // 
            // plotVoltage
            // 
            plotVoltage.Anchor = AnchorStyles.Left;
            plotVoltage.DisplayScale = 1F;
            plotVoltage.Location = new Point(2, 3);
            plotVoltage.Margin = new Padding(2, 1, 2, 1);
            plotVoltage.Name = "plotVoltage";
            plotVoltage.Size = new Size(435, 307);
            plotVoltage.TabIndex = 0;
            // 
            // plotVoltageSequence
            // 
            plotVoltageSequence.Anchor = AnchorStyles.Right;
            plotVoltageSequence.DisplayScale = 1F;
            plotVoltageSequence.Location = new Point(435, 3);
            plotVoltageSequence.Margin = new Padding(2, 1, 2, 1);
            plotVoltageSequence.Name = "plotVoltageSequence";
            plotVoltageSequence.Size = new Size(435, 307);
            plotVoltageSequence.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(885, 398);
            Controls.Add(tabControl1);
            Controls.Add(statusLabel);
            Controls.Add(menuStrip);
            Margin = new Padding(2, 1, 2, 1);
            Name = "Form1";
            Text = "Осциллограф";
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            tabControl1.ResumeLayout(false);
            pageCurrents.ResumeLayout(false);
            pageVoltage.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }


        private ToolStripMenuItem saveToolStripMenuItem;
    }
}