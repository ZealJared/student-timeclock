using SecuGen.FDxSDKPro.Windows;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace fingerprint
{
    public class FormMain : Form
    {
        private Button buttonEnroll;
        private Button buttonTimeclock;
        private TableLayoutPanel tableLayoutPanel1;
        private bool reallyClose = true;

        public FormMain()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
        }
        static void Main()
        {
            new FormMain().Show();
            Application.Run();
        }
        private void InitializeComponent()
        {
            this.buttonEnroll = new System.Windows.Forms.Button();
            this.buttonTimeclock = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonEnroll
            // 
            this.buttonEnroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonEnroll.Location = new System.Drawing.Point(3, 3);
            this.buttonEnroll.Name = "buttonEnroll";
            this.buttonEnroll.Size = new System.Drawing.Size(278, 124);
            this.buttonEnroll.TabIndex = 0;
            this.buttonEnroll.Text = "Enroll";
            this.buttonEnroll.UseVisualStyleBackColor = true;
            this.buttonEnroll.Click += new System.EventHandler(this.ButtonEnroll_Click);
            // 
            // buttonTimeclock
            // 
            this.buttonTimeclock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonTimeclock.Location = new System.Drawing.Point(3, 133);
            this.buttonTimeclock.Name = "buttonTimeclock";
            this.buttonTimeclock.Size = new System.Drawing.Size(278, 125);
            this.buttonTimeclock.TabIndex = 1;
            this.buttonTimeclock.Text = "Timeclock";
            this.buttonTimeclock.UseVisualStyleBackColor = true;
            this.buttonTimeclock.Click += new System.EventHandler(this.ButtonTimeclock_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.buttonEnroll, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonTimeclock, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 261);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // FormMain
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_Closing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private void FormMain_Closing(object sender, FormClosingEventArgs e)
        {
            if (!reallyClose) { return; }
            Application.Exit();
        }

        private void ButtonEnroll_Click(object sender, EventArgs e)
        {
            reallyClose = false;
            this.Close();
            reallyClose = true;
            new FormEnroll().Show();
        }

        private void ButtonTimeclock_Click(object sender, EventArgs e)
        {
            reallyClose = false;
            this.Close();
            reallyClose = true;
            new FormTimeclock().Show();
        }
    }
}
