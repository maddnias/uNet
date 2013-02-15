namespace CertUtil
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtIDString = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtCert = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblCertName = new System.Windows.Forms.Label();
            this.lblThumbprint = new System.Windows.Forms.Label();
            this.lblIDString = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtCertSave = new System.Windows.Forms.TextBox();
            this.lblSaveLocation = new System.Windows.Forms.Label();
            this.lblCertName2 = new System.Windows.Forms.Label();
            this.txtCertName = new System.Windows.Forms.TextBox();
            this.lblExpiration = new System.Windows.Forms.Label();
            this.txtExpiration = new System.Windows.Forms.TextBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.lblKeyLen = new System.Windows.Forms.Label();
            this.numKeyLen = new System.Windows.Forms.NumericUpDown();
            this.btnGenerateCert = new System.Windows.Forms.Button();
            this.lblKeyLen2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyLen)).BeginInit();
            this.SuspendLayout();
            // 
            // txtIDString
            // 
            this.txtIDString.BackColor = System.Drawing.Color.White;
            this.txtIDString.Location = new System.Drawing.Point(105, 111);
            this.txtIDString.Name = "txtIDString";
            this.txtIDString.ReadOnly = true;
            this.txtIDString.Size = new System.Drawing.Size(253, 20);
            this.txtIDString.TabIndex = 0;
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(364, 108);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 1;
            this.btnCopy.Text = "&Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnBrowse);
            this.groupBox1.Controls.Add(this.txtCert);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(445, 53);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(350, 17);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(89, 23);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "&Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtCert
            // 
            this.txtCert.BackColor = System.Drawing.Color.White;
            this.txtCert.Location = new System.Drawing.Point(9, 19);
            this.txtCert.Name = "txtCert";
            this.txtCert.ReadOnly = true;
            this.txtCert.Size = new System.Drawing.Size(335, 20);
            this.txtCert.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblKeyLen2);
            this.groupBox2.Controls.Add(this.lblIDString);
            this.groupBox2.Controls.Add(this.lblThumbprint);
            this.groupBox2.Controls.Add(this.lblCertName);
            this.groupBox2.Controls.Add(this.txtIDString);
            this.groupBox2.Controls.Add(this.btnCopy);
            this.groupBox2.Location = new System.Drawing.Point(12, 71);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(445, 142);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // lblCertName
            // 
            this.lblCertName.AutoSize = true;
            this.lblCertName.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCertName.Location = new System.Drawing.Point(6, 16);
            this.lblCertName.Name = "lblCertName";
            this.lblCertName.Size = new System.Drawing.Size(108, 16);
            this.lblCertName.TabIndex = 2;
            this.lblCertName.Text = "Certificate Name:";
            // 
            // lblThumbprint
            // 
            this.lblThumbprint.AutoSize = true;
            this.lblThumbprint.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThumbprint.Location = new System.Drawing.Point(6, 46);
            this.lblThumbprint.Name = "lblThumbprint";
            this.lblThumbprint.Size = new System.Drawing.Size(79, 16);
            this.lblThumbprint.TabIndex = 3;
            this.lblThumbprint.Text = "Thumbprint:";
            // 
            // lblIDString
            // 
            this.lblIDString.AutoSize = true;
            this.lblIDString.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIDString.Location = new System.Drawing.Point(6, 110);
            this.lblIDString.Name = "lblIDString";
            this.lblIDString.Size = new System.Drawing.Size(93, 16);
            this.lblIDString.TabIndex = 4;
            this.lblIDString.Text = "Identity String:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnGenerateCert);
            this.groupBox3.Controls.Add(this.numKeyLen);
            this.groupBox3.Controls.Add(this.lblKeyLen);
            this.groupBox3.Controls.Add(this.dateTimePicker1);
            this.groupBox3.Controls.Add(this.lblExpiration);
            this.groupBox3.Controls.Add(this.txtExpiration);
            this.groupBox3.Controls.Add(this.lblCertName2);
            this.groupBox3.Controls.Add(this.txtCertName);
            this.groupBox3.Controls.Add(this.lblSaveLocation);
            this.groupBox3.Controls.Add(this.btnSave);
            this.groupBox3.Controls.Add(this.txtCertSave);
            this.groupBox3.Location = new System.Drawing.Point(12, 219);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(445, 272);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Certificate Generation";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(353, 40);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(86, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "&Browse...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtCertSave
            // 
            this.txtCertSave.BackColor = System.Drawing.Color.White;
            this.txtCertSave.Location = new System.Drawing.Point(12, 42);
            this.txtCertSave.Name = "txtCertSave";
            this.txtCertSave.ReadOnly = true;
            this.txtCertSave.Size = new System.Drawing.Size(335, 20);
            this.txtCertSave.TabIndex = 5;
            // 
            // lblSaveLocation
            // 
            this.lblSaveLocation.AutoSize = true;
            this.lblSaveLocation.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSaveLocation.Location = new System.Drawing.Point(7, 22);
            this.lblSaveLocation.Name = "lblSaveLocation";
            this.lblSaveLocation.Size = new System.Drawing.Size(84, 16);
            this.lblSaveLocation.TabIndex = 5;
            this.lblSaveLocation.Text = "Save location";
            // 
            // lblCertName2
            // 
            this.lblCertName2.AutoSize = true;
            this.lblCertName2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCertName2.Location = new System.Drawing.Point(8, 72);
            this.lblCertName2.Name = "lblCertName2";
            this.lblCertName2.Size = new System.Drawing.Size(102, 16);
            this.lblCertName2.TabIndex = 7;
            this.lblCertName2.Text = "Certificate name";
            // 
            // txtCertName
            // 
            this.txtCertName.BackColor = System.Drawing.Color.White;
            this.txtCertName.Location = new System.Drawing.Point(10, 91);
            this.txtCertName.Name = "txtCertName";
            this.txtCertName.Size = new System.Drawing.Size(209, 20);
            this.txtCertName.TabIndex = 8;
            // 
            // lblExpiration
            // 
            this.lblExpiration.AutoSize = true;
            this.lblExpiration.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExpiration.Location = new System.Drawing.Point(8, 128);
            this.lblExpiration.Name = "lblExpiration";
            this.lblExpiration.Size = new System.Drawing.Size(93, 16);
            this.lblExpiration.TabIndex = 9;
            this.lblExpiration.Text = "Expiration date";
            // 
            // txtExpiration
            // 
            this.txtExpiration.BackColor = System.Drawing.Color.White;
            this.txtExpiration.Location = new System.Drawing.Point(10, 147);
            this.txtExpiration.Name = "txtExpiration";
            this.txtExpiration.Size = new System.Drawing.Size(209, 20);
            this.txtExpiration.TabIndex = 10;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(239, 147);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 11;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // lblKeyLen
            // 
            this.lblKeyLen.AutoSize = true;
            this.lblKeyLen.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKeyLen.Location = new System.Drawing.Point(9, 182);
            this.lblKeyLen.Name = "lblKeyLen";
            this.lblKeyLen.Size = new System.Drawing.Size(70, 16);
            this.lblKeyLen.TabIndex = 12;
            this.lblKeyLen.Text = "Key Length";
            // 
            // numKeyLen
            // 
            this.numKeyLen.Increment = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numKeyLen.Location = new System.Drawing.Point(11, 201);
            this.numKeyLen.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numKeyLen.Name = "numKeyLen";
            this.numKeyLen.Size = new System.Drawing.Size(120, 20);
            this.numKeyLen.TabIndex = 13;
            this.numKeyLen.Value = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            // 
            // btnGenerateCert
            // 
            this.btnGenerateCert.Location = new System.Drawing.Point(9, 237);
            this.btnGenerateCert.Name = "btnGenerateCert";
            this.btnGenerateCert.Size = new System.Drawing.Size(430, 27);
            this.btnGenerateCert.TabIndex = 14;
            this.btnGenerateCert.Text = "&Generate SSL Certificate";
            this.btnGenerateCert.UseVisualStyleBackColor = true;
            this.btnGenerateCert.Click += new System.EventHandler(this.btnGenerateCert_Click);
            // 
            // lblKeyLen2
            // 
            this.lblKeyLen2.AutoSize = true;
            this.lblKeyLen2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKeyLen2.Location = new System.Drawing.Point(6, 77);
            this.lblKeyLen2.Name = "lblKeyLen2";
            this.lblKeyLen2.Size = new System.Drawing.Size(72, 16);
            this.lblKeyLen2.TabIndex = 5;
            this.lblKeyLen2.Text = "Key length:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 503);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "uNet CertUtil";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyLen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtIDString;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtCert;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblIDString;
        private System.Windows.Forms.Label lblThumbprint;
        private System.Windows.Forms.Label lblCertName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblSaveLocation;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtCertSave;
        private System.Windows.Forms.Button btnGenerateCert;
        private System.Windows.Forms.NumericUpDown numKeyLen;
        private System.Windows.Forms.Label lblKeyLen;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label lblExpiration;
        private System.Windows.Forms.TextBox txtExpiration;
        private System.Windows.Forms.Label lblCertName2;
        private System.Windows.Forms.TextBox txtCertName;
        private System.Windows.Forms.Label lblKeyLen2;
    }
}

