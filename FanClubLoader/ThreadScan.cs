using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BeastsLairConnector;

namespace FanClubLoader
{
    public partial class ThreadScan : Form
    {
        private int currentPage;
        private const string ScannedPagesFormat = "Scanning page {0} of {1}, {2} images";
        private BackgroundWorker bwScanner;

        public ThreadScan(BLThread threadToScan)
        {
            InitializeComponent();
            lblScanningThread.Text = "Scanning thread: " + threadToScan.ThreadName;
            var maxPage = threadToScan.OpeningPost.GetPageMax();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = maxPage;
            bwScanner = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            bwScanner.DoWork += (sender, args) => ScanThread(threadToScan, bwScanner);
            bwScanner.ProgressChanged += (sender, args) =>
            {
                if (args.ProgressPercentage != -1)
                {
                    progressBar1.Value = args.ProgressPercentage > progressBar1.Maximum ? progressBar1.Maximum : args.ProgressPercentage;
                }


                lblThreadScanProgress.Text = string.Format(ScannedPagesFormat, args.ProgressPercentage,
                    maxPage, (int)args.UserState);
            };
            bwScanner.RunWorkerCompleted += (sender, args) =>
            {
                MessageBox.Show("Scan completed.");
                DialogResult = bwScanner.CancellationPending ? DialogResult.Abort : DialogResult.OK;
                threadToScan.ImagesAmount = threadToScan.LoadedPages.Sum(lp => lp.Images.Count);
                button1.Enabled = true;
                Close();
            };
          
        }

        public void StartScan()
        {
            bwScanner.RunWorkerAsync();
        }

        private void ScanThread(BLThread threadToScan, BackgroundWorker bw)
        {
            if (threadToScan.LoadedPages == null)
            {
                threadToScan.LoadedPages = new List<BLPage>();
                threadToScan.OpeningPost = new BLPage(threadToScan.OpeningPostUrl);
                threadToScan.LoadedPages.Add(threadToScan.OpeningPost);
            }
            
            var lastPage = threadToScan.LoadedPages.Last();
            lastPage.Load();
            currentPage = lastPage.CurrentPageNumber;
            var nextPageUrl = lastPage.GetNextPageUrl();
            bw.ReportProgress(currentPage, lastPage.Images.Count);
            while (!bw.CancellationPending && !string.IsNullOrEmpty(nextPageUrl))
            {
                var nextPage = threadToScan.RemoveQuotesAndRepeats(new BLPage(nextPageUrl));
                threadToScan.LoadedPages.Add(nextPage);
                currentPage = nextPage.CurrentPageNumber;
                nextPageUrl = nextPage.GetNextPageUrl();
                bw.ReportProgress(currentPage, nextPage.Images.Count);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            bwScanner.CancelAsync();
        }
    }
}
