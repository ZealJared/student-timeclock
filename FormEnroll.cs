using Emgu.CV;
using MySql.Data.MySqlClient;
using SecuGen.FDxSDKPro.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace fingerprint
{
    public class FormEnroll : Form
    {
        Byte[] image;
        Int32 width = 300;
        Int32 height = 400;
        private PictureBox pictureBoxFingerprint;
        private Button buttonPicture;
        private Button buttonFingerprint;
        private TextBox textBoxName;
        private Label labelName;
        private ComboBox comboBoxStudent;
        private Button buttonConfirm;
        SGFingerPrintManager fpm;
        Int32 studentId = 0;
        Bitmap picture = null;
        private Button buttonBack;
        Byte[] template = null;
        private TableLayoutPanel tableLayoutPanel1;
        private IContainer components;
        Api api = new Api();
        private PictureBox pictureBoxPicture;
        private VideoCapture capture = null;
        public FormEnroll()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            fpm = new SGFingerPrintManager();
            fpm.Init(SGFPMDeviceName.DEV_FDU05);
            fpm.OpenDevice((Int32)SGFPMPortAddr.USB_AUTO_DETECT);
            fpm.SetTemplateFormat(SGFPMTemplateFormat.ANSI378);
            pictureBoxPicture.BackgroundImageLayout = ImageLayout.Zoom;
            Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
            {
                if (picture != null || studentId == 0) { return; }
                if (capture == null) {
                    capture = new VideoCapture(0, VideoCapture.API.DShow);
                    capture.FlipHorizontal = true;
                    // capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, 10);
                    // capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Buffersuze, 1);
                    // capture.Start();
                }
                if (capture.IsOpened) {
                    Bitmap frame = capture.QueryFrame().Bitmap;
                    pictureBoxPicture.BackgroundImage = frame;
                }
            });
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
                    String matchFound = FindMatch(scan);
                    if (matchFound.Length == 0)
                    {
                        File.WriteAllBytes("./" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".tmp", scan);
                        MessageBox.Show("No match found, saving...");
                    }
                    else
                    {
                        MessageBox.Show(matchFound);
                    }
                }
            }
            base.WndProc(ref message);
        }

        private string FindMatch(Byte[] scan)
        {
            String[] files = Directory.GetFiles("./");
            foreach (string file in files)
            {
                Byte[] saved = File.ReadAllBytes("./" + file);
                if (IsMatch(scan, saved))
                {
                    return file;
                }
            }
            return "";
        }

        private bool IsMatch(Byte[] scan, Byte[] saved)
        {
            Boolean doesMatch = false;
            fpm.MatchTemplate(scan, saved, SGFPMSecurityLevel.NORMAL, ref doesMatch);
            return doesMatch;
        }

        private Bitmap bytesToBitmap(Byte[] bytes)
        {
            int colorval;
            Bitmap bmp = new Bitmap(width, height);
            for (int col = 0; col < bmp.Width; col++)
            {
                for (int row = 0; row < bmp.Height; row++)
                {
                    colorval = (int)bytes[(row * width) + col];
                    bmp.SetPixel(col, row, Color.FromArgb(colorval, colorval, colorval));
                }
            }
            return bmp;
        }

        private void DrawImage()
        {
            Bitmap bmp = bytesToBitmap(image);
            bmp.Save("./" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp");
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEnroll));
            this.buttonPicture = new System.Windows.Forms.Button();
            this.buttonFingerprint = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.comboBoxStudent = new System.Windows.Forms.ComboBox();
            this.buttonConfirm = new System.Windows.Forms.Button();
            this.buttonBack = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxFingerprint = new System.Windows.Forms.PictureBox();
            this.pictureBoxPicture = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFingerprint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonPicture
            // 
            this.buttonPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.buttonPicture, 2);
            this.buttonPicture.Location = new System.Drawing.Point(3, 206);
            this.buttonPicture.Name = "buttonPicture";
            this.buttonPicture.Size = new System.Drawing.Size(136, 23);
            this.buttonPicture.TabIndex = 5;
            this.buttonPicture.Text = "Capture";
            this.buttonPicture.UseVisualStyleBackColor = true;
            this.buttonPicture.Click += new System.EventHandler(this.ButtonPicture_Click);
            // 
            // buttonFingerprint
            // 
            this.buttonFingerprint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.buttonFingerprint, 2);
            this.buttonFingerprint.Location = new System.Drawing.Point(145, 206);
            this.buttonFingerprint.Name = "buttonFingerprint";
            this.buttonFingerprint.Size = new System.Drawing.Size(136, 23);
            this.buttonFingerprint.TabIndex = 6;
            this.buttonFingerprint.Text = "Capture";
            this.buttonFingerprint.UseVisualStyleBackColor = true;
            this.buttonFingerprint.Click += new System.EventHandler(this.ButtonFingerprint_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxName, 2);
            this.textBoxName.Location = new System.Drawing.Point(145, 4);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(136, 20);
            this.textBoxName.TabIndex = 7;
            this.textBoxName.TextChanged += new System.EventHandler(this.TextBoxName_TextChanged);
            // 
            // labelName
            // 
            this.labelName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(104, 8);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(35, 13);
            this.labelName.TabIndex = 8;
            this.labelName.Text = "Name";
            // 
            // comboBoxStudent
            // 
            this.comboBoxStudent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.comboBoxStudent, 4);
            this.comboBoxStudent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStudent.FormattingEnabled = true;
            this.comboBoxStudent.Location = new System.Drawing.Point(3, 33);
            this.comboBoxStudent.Name = "comboBoxStudent";
            this.comboBoxStudent.Size = new System.Drawing.Size(278, 21);
            this.comboBoxStudent.TabIndex = 9;
            this.comboBoxStudent.SelectedIndexChanged += new System.EventHandler(this.ComboBoxStudent_SelectedIndexChanged);
            // 
            // buttonConfirm
            // 
            this.buttonConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.buttonConfirm, 4);
            this.buttonConfirm.Location = new System.Drawing.Point(3, 235);
            this.buttonConfirm.Name = "buttonConfirm";
            this.buttonConfirm.Size = new System.Drawing.Size(278, 23);
            this.buttonConfirm.TabIndex = 10;
            this.buttonConfirm.Text = "Confirm";
            this.buttonConfirm.UseVisualStyleBackColor = true;
            this.buttonConfirm.Click += new System.EventHandler(this.ButtonConfirm_Click);
            // 
            // buttonBack
            // 
            this.buttonBack.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonBack.Location = new System.Drawing.Point(3, 3);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(55, 23);
            this.buttonBack.TabIndex = 11;
            this.buttonBack.Text = "< Back";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.ButtonBack_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.comboBoxStudent, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonBack, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonConfirm, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonPicture, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxName, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxFingerprint, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonFingerprint, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxPicture, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 261);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // pictureBoxFingerprint
            // 
            this.pictureBoxFingerprint.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.pictureBoxFingerprint, 2);
            this.pictureBoxFingerprint.ErrorImage = null;
            this.pictureBoxFingerprint.Image = global::fingerprint.Properties.Resources.fingerprint;
            this.pictureBoxFingerprint.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxFingerprint.InitialImage")));
            this.pictureBoxFingerprint.Location = new System.Drawing.Point(145, 61);
            this.pictureBoxFingerprint.Name = "pictureBoxFingerprint";
            this.pictureBoxFingerprint.Size = new System.Drawing.Size(136, 139);
            this.pictureBoxFingerprint.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxFingerprint.TabIndex = 4;
            this.pictureBoxFingerprint.TabStop = false;
            // 
            // pictureBoxPicture
            // 
            this.pictureBoxPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.pictureBoxPicture, 2);
            this.pictureBoxPicture.Image = global::fingerprint.Properties.Resources.face;
            this.pictureBoxPicture.Location = new System.Drawing.Point(3, 61);
            this.pictureBoxPicture.Name = "pictureBoxPicture";
            this.pictureBoxPicture.Size = new System.Drawing.Size(136, 139);
            this.pictureBoxPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPicture.TabIndex = 12;
            this.pictureBoxPicture.TabStop = false;
            // 
            // FormEnroll
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormEnroll";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEnroll_Closing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFingerprint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPicture)).EndInit();
            this.ResumeLayout(false);

        }

        private List<ComboBoxItem> GetStudents(String name)
        {
            name = name.Replace(" ", "%");
            List<ComboBoxItem> results = new List<ComboBoxItem>();
            MySqlConnection conn = new MySqlConnection("server=afterschooltek.com;user=root;database=scouts;port=3306;password=PrettySillyPasswordString");
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT id, CONCAT(first_name, ' ', last_name) FROM students WHERE CONCAT(first_name, ' ', last_name) LIKE '%" + name + "%' ORDER BY modified DESC", conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Value = (Int32)rdr[0];
                item.Text = (string)rdr[1];
                results.Add(item);
            }
            rdr.Close();
            conn.Close();
            return results;
        }

        private void IdRequired()
        {
            MessageBox.Show("Select a student first.");
        }

        private void ButtonPicture_Click(object sender, EventArgs e)
        {
            if (studentId == 0)
            {
                IdRequired();
                return;
            }
            // VideoCapture capture = new VideoCapture();
            picture = capture.QueryFrame().Bitmap;
            // pictureBoxPicture.Image = picture;
        }

        private void ButtonFingerprint_Click(object sender, EventArgs e)
        {
            if (studentId == 0)
            {
                IdRequired();
                return;
            }
            Byte[] fingerprint = new Byte[300 * 400];
            fpm.GetImage(fingerprint);
            template = new Byte[400];
            fpm.CreateTemplate(fingerprint, template);
            pictureBoxFingerprint.Image = bytesToBitmap(fingerprint);
        }

        private void TextBoxName_TextChanged(object sender, EventArgs e)
        {
            comboBoxStudent.Items.Clear();
            foreach (ComboBoxItem item in GetStudents(textBoxName.Text))
            {
                comboBoxStudent.Items.Add(item);
            }
        }

        private async void ButtonConfirm_Click(object sender, EventArgs e)
        {
            if (picture == null || template == null)
            {
                MessageBox.Show("Picture and fingerprint are required.");
                return;
            }
            // create folder for student id locally
            String basePath = "./students/" + studentId.ToString();
            Directory.CreateDirectory(basePath);
            // save fingerprint template locally
            File.WriteAllBytes(basePath + "/fingerprint.bin", template);
            // save picture locally
            File.Delete(basePath + "/picture.jpg");
            picture.Save(basePath + "/picture.jpg");
            // zip folder
            File.Delete(basePath + ".zip");
            ZipFile.CreateFromDirectory(basePath, basePath + ".zip");
            // upload zip
            String base64zip = Convert.ToBase64String(File.ReadAllBytes(basePath + ".zip"));
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "studentId", studentId.ToString() },
                { "zip", base64zip }
            };
            dynamic responseObject = await api.Post("https://afterschooltek.com/scouts/new/admin/api/upload.php", data);
            // notify on completion
            MessageBox.Show("Saved.");
        }

        private void ComboBoxStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)comboBoxStudent.SelectedItem;
            studentId = item.Value;
        }
        public void FormEnroll_Closing(object sender, FormClosingEventArgs e)
        {
            fpm.CloseDevice();
            new FormMain().Show();
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public class ComboBoxItem
    {
        public string Text { get; set; }
        public Int32 Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
