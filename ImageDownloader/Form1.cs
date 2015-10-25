using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace ImageDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnCancel.Enabled = false;
            btnRun.Enabled = true;
            if (count == 0)
            {
                if (MessageBox.Show("Download is completed successfully, would you like to open the output folder?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Process.Start(tbOutput.Text);
                }
            }
            else
            {
                if (MessageBox.Show("Download is completed with unexpected situations, would you like to open the output folder?\r\n"+errorMessage, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Process.Start(tbOutput.Text);
                }
            }
        }

        private int count;

        private string errorMessage = string.Empty;

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            count = 0;
            WebClient webClient = new WebClient();
            string[] urls = File.ReadAllLines(tbFile.Text);
            for(int index = 0; index < urls.Length; index++)
            {
                string[] segments = urls[index].Split('/');
                StringBuilder outputPath = new StringBuilder(tbOutput.Text);
                int j;
                for (j = 3; j < segments.Length - 1; j++)
                {
                    outputPath.Append("\\").Append(segments[j]);
                    if (Directory.Exists(outputPath.ToString()) == false)
                    {
                        Directory.CreateDirectory(outputPath.ToString());
                    }
                }
                string outputFileName = outputPath.Append("\\").Append(segments[j]).ToString();
                try {
                    webClient.DownloadFile(urls[index], outputFileName);
                }
                catch(Exception ex)
                {
                    count++;
                    errorMessage += ex.Message + "\r\n";
                }
                backgroundWorker1.ReportProgress((int)(index + 1) / urls.Length * 100);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            if(d.ShowDialog() == DialogResult.OK)
            {
                this.tbFile.Text = d.FileName;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            if(d.ShowDialog()== DialogResult.OK)
            {
                this.tbOutput.Text = d.SelectedPath;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = true;
            btnRun.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            btnCancel.Enabled = false;
            btnRun.Enabled = true;
        }
    }
}
