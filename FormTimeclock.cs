using SecuGen.FDxSDKPro.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Timers;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace fingerprint
{
    public class FormTimeclock : Form
    {
        Api api = new Api();
        SGFingerPrintManager fpm;
        Int32 width = 300;
        Int32 height = 400;
        private Button buttonBack;
        private PictureBox pictureBoxStudent;
        private Label labelAction;
        private Label labelName;
        Byte[] image;
        System.Timers.Timer timer;
        System.Media.SoundPlayer noMatchPlayer = new System.Media.SoundPlayer(Properties.Resources.NoMatch);
        System.Media.SoundPlayer clockInPlayer = new System.Media.SoundPlayer(Properties.Resources.ClockIn);
        private TableLayoutPanel tableLayoutPanel1;
        System.Media.SoundPlayer clockOutPlayer = new System.Media.SoundPlayer(Properties.Resources.ClockOut);
        public FormTimeclock()
        {
            timer = new System.Timers.Timer(5000);
            timer.Elapsed += Reset;
            timer.SynchronizingObject = this;
            Application.EnableVisualStyles();
            InitializeComponent();
            fpm = new SGFingerPrintManager();
            fpm.Init(SGFPMDeviceName.DEV_FDU05);
            fpm.OpenDevice((Int32)SGFPMPortAddr.USB_AUTO_DETECT);
            fpm.SetTemplateFormat(SGFPMTemplateFormat.ANSI378);
            fpm.EnableAutoOnEvent(true, (int)this.Handle);
        }
        private void NoMatch()
        {
            noMatchPlayer.Play();
            labelAction.Text = "No Match Found!";
            labelAction.ForeColor = Color.Red;
            labelName.Text = "Try Again.";
            pictureBoxStudent.Image = global::fingerprint.Properties.Resources.notFound;
            DelayReset();
        }
        private void DelayReset()
        {
            // TODO: delay
            timer.Start();
        }
        protected override void WndProc(ref Message message)
        {
            if (message.Msg == (int)SGFPMMessages.DEV_AUTOONEVENT)
            {
                if (message.WParam.ToInt32() == (Int32)SGFPMAutoOnEvent.FINGER_ON)
                {
                    image = new Byte[width * height];
                    fpm.GetImage(image);
                    Byte[] scan = new Byte[400];
                    fpm.CreateTemplate(image, scan);
                    Int32 matchFound = FindMatch(scan);
                    if (matchFound == 0)
                    {
                        NoMatch();
                    }
                    else
                    {
                        Clock(matchFound);
                    }
                }
            }
            base.WndProc(ref message);
        }
        private async void Clock(Int32 studentId)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "studentId", studentId.ToString() }
            };
            // get student name and clock in/out action from URL
            dynamic responseObject = await api.Post("https://afterschooltek.com/scouts/new/admin/api/clock.php", data);
            String fullName = null;
            String logEvent = null;
            try
            {
                String firstName = responseObject.student.FirstName;
                String lastName = responseObject.student.LastName;
                fullName = firstName + " " + lastName;
                logEvent = responseObject.lastLog.Event;
            } catch(Exception e)
            {
                MessageBox.Show(responseObject.notJson);
            }
            // populate and format related labels
            pictureBoxStudent.ImageLocation = "./students/" + studentId + "/picture.jpg";
            labelName.Text = fullName;
            if (logEvent == "end")
            {
                clockOutPlayer.Play();
                labelAction.Text = "Clocked Out.";
                labelAction.ForeColor = Color.Orange;
            }
            else
            {
                clockInPlayer.Play();
                labelAction.Text = "Clocked In.";
                labelAction.ForeColor = Color.Green;
            }
            // set timeout for reset
            DelayReset();
        }

        private Int32 FindMatch(Byte[] scan)
        {
            String[] studentDirectories = Directory.GetDirectories("./students");
            Int32 studentId;
            Byte[] saved;
            foreach (string studentDirectory in studentDirectories)
            {
                studentId = Int32.Parse(new DirectoryInfo(studentDirectory).Name);
                saved = File.ReadAllBytes(studentDirectory + "/fingerprint.bin");
                if (IsMatch(scan, saved))
                {
                    return studentId;
                }
            }
            return 0;
        }

        private bool IsMatch(Byte[] scan, Byte[] saved)
        {
            Boolean doesMatch = false;
            fpm.MatchTemplate(scan, saved, SGFPMSecurityLevel.NORMAL, ref doesMatch);
            return doesMatch;
        }
        private void InitializeComponent()
        {
            this.buttonBack = new System.Windows.Forms.Button();
            this.labelAction = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.pictureBoxStudent = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStudent)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonBack
            // 
            this.buttonBack.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonBack.Location = new System.Drawing.Point(3, 3);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(55, 23);
            this.buttonBack.TabIndex = 12;
            this.buttonBack.Text = "< Back";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.ButtonBack_Click);
            // 
            // labelAction
            // 
            this.labelAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAction.AutoSize = true;
            this.labelAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAction.Location = new System.Drawing.Point(64, 8);
            this.labelAction.Name = "labelAction";
            this.labelAction.Size = new System.Drawing.Size(217, 13);
            this.labelAction.TabIndex = 14;
            this.labelAction.Text = "Use your fingerprint to clock in/out.";
            // 
            // labelName
            // 
            this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelName.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelName, 2);
            this.labelName.Location = new System.Drawing.Point(3, 248);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(278, 13);
            this.labelName.TabIndex = 15;
            this.labelName.Text = "Waiting...";
            // 
            // pictureBoxStudent
            // 
            this.pictureBoxStudent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxStudent.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.SetColumnSpan(this.pictureBoxStudent, 2);
            this.pictureBoxStudent.Image = global::fingerprint.Properties.Resources.picture;
            this.pictureBoxStudent.Location = new System.Drawing.Point(3, 32);
            this.pictureBoxStudent.Name = "pictureBoxStudent";
            this.pictureBoxStudent.Size = new System.Drawing.Size(278, 213);
            this.pictureBoxStudent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxStudent.TabIndex = 13;
            this.pictureBoxStudent.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.labelAction, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelName, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonBack, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxStudent, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 261);
            this.tableLayoutPanel1.TabIndex = 16;
            // 
            // FormTimeclock
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormTimeclock";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTimeclock_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStudent)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }
        private void ButtonBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void FormTimeclock_Closing(object sender, FormClosingEventArgs e)
        {
            fpm.CloseDevice();
            new FormMain().Show();
        }

        private void Reset(object source, ElapsedEventArgs e)
        {
            labelAction.Text = "Use your fingerprint to clock in/out.";
            labelAction.ForeColor = Color.Black;
            labelName.Text = "Waiting...";
            pictureBoxStudent.Image = global::fingerprint.Properties.Resources.picture;
            timer.Stop();
        }
    }
}
