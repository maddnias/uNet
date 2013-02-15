using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CertUtil
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "SSL Certificate (*.cer)|*.cer";
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (!ofd.CheckFileExists)
                        throw new FileNotFoundException();

                    txtCert.Text = ofd.FileName;

                    var cert = new X509Certificate2(File.ReadAllBytes(ofd.FileName));

                    lblCertName.Text = "Certificate Name: " + cert.SubjectName.Name.Split('=')[1];
                    lblThumbprint.Text = "Thumbprint: " + cert.Thumbprint;
                    lblKeyLen2.Text = "Key length: " + cert.PublicKey.Key.KeySize;
                    txtIDString.Text = cert.SubjectName.Name.Split('=')[1] + "|" + cert.Thumbprint + "|" + cert.PublicKey.Key.KeySize;

                }
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, txtIDString.Text);
            MessageBox.Show("Copied to clipboard!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "SSL Certificate (*.cer)|*.cer";
                sfd.OverwritePrompt = true;
  
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    txtCertSave.Text = sfd.FileName;
                }
            }
        }

        private void btnGenerateCert_Click(object sender, EventArgs e)
        {
            string mcPath = "";

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Makecert (makecert.exe)|makecert.exe";
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (!ofd.CheckFileExists)
                        throw new FileNotFoundException();

                    mcPath = ofd.FileName;
                }
            }

            var proc = new Process();
            var startInfo = new ProcessStartInfo(mcPath, CreateArgumentString())
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            proc.StartInfo = startInfo;
            proc.Start();

            proc.WaitForExit();

            var output = proc.StandardOutput.ReadToEnd();

            if (output.Contains("Succeeded"))
                MessageBox.Show("Successfully generated SSL certificate!");
            else
                MessageBox.Show("Failed to generate certificate\nInfo:\n\n" + output);

        }

        private string CreateArgumentString()
        {
            //makecert -r -pe -n "CN=name" -e 10/10/2013 -ss my -eku 1.3.6.1.5.5.7.3.1 certfile.cer

            if (numKeyLen.Value % 1024 != 0 || numKeyLen.Value < 1024)
                throw new Exception("Invalid key length");

            string output = "";

            output += @"-r -pe -n """;
            output += "CN=" + txtCertName.Text + @""" ";
            output += (txtExpiration.Text != "" ? "-e " + txtExpiration.Text : "") + " ";
            output += "-ss my ";
            output += "-len " + numKeyLen.Value + " ";
            output += @"""" + txtCertSave.Text + @"""";

            return output;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            txtExpiration.Text = dateTimePicker1.Value.ToString("MM/dd/yyyy", new CultureInfo("en-US").DateTimeFormat);
        }
    }
}
