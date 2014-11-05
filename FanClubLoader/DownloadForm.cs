using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BeastsLairConnector;
using Downloader;

namespace FanClubLoader
{
    public partial class DownloadForm : Form
    {
        private BLThread _myThread;
        private List<BLImage> downloadList;
        private const string scanningProgressFormat = "Scanning page {0}...";

        private BackgroundWorker imageScanner;
        private BackgroundWorker imageDownloaderWorker;
        private BLDownloader imageDownloader;
        private int _pageMax;
        private int _postsPerPage;
        private const int DefaultPostsPerPage = 20;

        public delegate void DownloadFinishedHandler();
        public event DownloadFinishedHandler DownloadFinished;

        public DownloadForm(BLThread thread)
        {
            InitializeComponent();
            _myThread = thread;
            _postsPerPage = DefaultPostsPerPage;
            _pageMax = thread.OpeningPost.GetPageMax();
            
            numFromPage.Minimum = 1;
            numFromPage.Maximum = _pageMax;
            numFromPage.Value = 1;

            numToPage.Minimum = 1;
            numToPage.Maximum = _pageMax;
            numToPage.Value = _pageMax;

            cmbPostsPerPage.SelectedIndex = 1;
        }

        private Point convertPages(int pFrom, int pTo, int pPerFrom, int pPerTo)
        {
            int pFromNew = ((pFrom-1)*pPerFrom)/pPerTo + 1;
            int pToNew = (pTo*pPerFrom)/pPerTo;
            if (pTo*pPerFrom%pPerTo > 0)
            {
                pToNew++;
            }
            return new Point(pFromNew, pToNew);
        }

        private void cmbPostsPerPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newPostsPerPage = int.Parse(cmbPostsPerPage.Text);
            var newPages = convertPages((int) numFromPage.Value, (int) numToPage.Value, _postsPerPage, newPostsPerPage);
            int newMax = (_pageMax*DefaultPostsPerPage)/newPostsPerPage;
            if ((_pageMax*_postsPerPage)%newPostsPerPage > 0) newMax++;
            _postsPerPage = newPostsPerPage;
            numFromPage.Maximum = newMax;
            numToPage.Maximum = newMax;
            numFromPage.Value = Math.Min(newPages.X, numFromPage.Maximum);
            numToPage.Value = Math.Min(newPages.Y, numToPage.Maximum);
        }

        private void btnBrowseDownloadLocation_Click(object sender, EventArgs e)
        {
            dlFolderSelectDialog.SelectedPath = txtDownloadLocation.Text;
            if (dlFolderSelectDialog.ShowDialog() == DialogResult.OK)
            {
                txtDownloadLocation.Text = dlFolderSelectDialog.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (numFromPage.Value > numToPage.Value)
            {
                MessageBox.Show("Invalid values",
                    "Invalid 'From' and 'To' values. 'From' cannot be greater than 'To'. Please set the values correctly and try again.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (imageScanner != null && imageScanner.IsBusy)
            {
                imageScanner.CancelAsync();
                return;
            }

            var images = new List<BLImage>();
            button2.Text = "Cancel";
            button1.Enabled = false;
            bLImageBindingSource.DataSource = images;
            imageScanner = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            int fromP = (int)numFromPage.Value;
            int toP = (int)numToPage.Value;
            var converted = convertPages(fromP, toP, int.Parse(cmbPostsPerPage.Text), DefaultPostsPerPage);
            fromP = converted.X;
            toP = converted.Y;
            progressBar1.Maximum = toP - fromP + 1;
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
            imageScanner.DoWork += (o, args) => ScanImages((int)numFromPage.Value, (int)numToPage.Value);

            imageScanner.ProgressChanged += (o, args) =>
            {                
                images.InsertRange(0, args.UserState as List<BLImage>);
                bLImageBindingSource.ResetBindings(false);
                progressBar1.Value = args.ProgressPercentage - fromP + 1;
                lblProgressLabel.Text = string.Format(scanningProgressFormat, args.ProgressPercentage);
            };
            imageScanner.RunWorkerCompleted += (o, args) =>
            {
                button2.Text = "Scan";
                button1.Enabled = true;
                lblProgressLabel.Text = "";
            };

            imageScanner.RunWorkerAsync();
        }

        private void ScanImages(int firstPageNumber, int lastPageNumber)
        {
            int currentPageNumber = firstPageNumber;
            var currentPage = new BLPage(_myThread.GetBLPageByPageNumber(currentPageNumber).PageUrl);
            _myThread.ReplaceOrAddPage(currentPage);
            imageScanner.ReportProgress(currentPageNumber, currentPage.Images);
            
            while (!imageScanner.CancellationPending && currentPageNumber < lastPageNumber && !string.IsNullOrEmpty(currentPage.NextPageUrl))
            {
                var nextPage = new BLPage(currentPage.NextPageUrl);
                _myThread.ReplaceOrAddPage(nextPage);
                currentPage = nextPage;
                currentPageNumber++;
                imageScanner.ReportProgress(currentPageNumber, nextPage.Images);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDownloadLocation.Text))
            {
                return;
            }

            if (numFromPage.Value > numToPage.Value)
            {
                MessageBox.Show("Invalid values",
                    "Invalid 'From' and 'To' values. 'From' cannot be greater than 'To'. Please set the values correctly and try again.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int fromP = (int)numFromPage.Value;
            int toP = (int)numToPage.Value;
            var converted = convertPages(fromP, toP, int.Parse(cmbPostsPerPage.Text), DefaultPostsPerPage);
            fromP = converted.X;
            toP = converted.Y;
            progressBar1.Maximum = toP - fromP + 1;
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;

            imageDownloaderWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            imageDownloaderWorker.DoWork += (o, args) => CreateImageListToDownload(fromP, toP);
            imageDownloaderWorker.ProgressChanged += (o, args) =>
            {
                lblProgressLabel.Text = string.Format("Getting data from page {0}", args.ProgressPercentage);
                progressBar1.Value = args.ProgressPercentage - fromP + 1;
            };
            imageDownloaderWorker.RunWorkerCompleted += (o, args) =>
            {
                bLImageBindingSource.DataSource = downloadList;
                if (imageDownloaderWorker.CancellationPending) return;
                imageDownloaderWorker = new BackgroundWorker();
                imageDownloaderWorker.WorkerReportsProgress = true;
                imageDownloaderWorker.WorkerSupportsCancellation = true;
                progressBar1.Maximum = downloadList.Count;
                progressBar1.Minimum = 0;
                progressBar1.Value = 0;
                imageDownloaderWorker.DoWork += (sender1, eventArgs) => DownloadImages(false);
                imageDownloaderWorker.ProgressChanged += (sender1, eventArgs) =>
                {
                    lblProgressLabel.Text = "Downloading images... " + eventArgs.ProgressPercentage + "/" + downloadList.Count;
                    progressBar1.Value = eventArgs.ProgressPercentage;
                };
                imageDownloaderWorker.RunWorkerCompleted += (sender1, eventArgs) =>
                {
                    lblProgressLabel.Text = "Download completed.";
                    DownloadFinished();
                }; 
                progressBar1.Maximum = downloadList.Count;
                progressBar1.Minimum = 0;
                progressBar1.Value = 0;
                imageDownloaderWorker.RunWorkerAsync();
            };
            
            imageDownloaderWorker.RunWorkerAsync();
        }

        private void CreateImageListToDownload(int pageFrom, int pageTo)
        {
            downloadList = new List<BLImage>();
            for (int i = pageFrom; i <= pageTo && !imageDownloaderWorker.CancellationPending; i++)
            {
                var lpage = _myThread.LoadedPages.SingleOrDefault(lp => lp.CurrentPageNumber == i) ?? _myThread.GetBLPageByPageNumber(i);
                if (lpage == null)
                {
                    MessageBox.Show(string.Format("Cannot scan page {0}. Page unavailable.", i));
                }
                downloadList.AddRange(lpage.Images);
                imageDownloaderWorker.ReportProgress(i);
            }
        }



        private void DownloadImages(bool isRedownload)
        {
            bool downloaderFinished = false;
            imageDownloader = new BLDownloader
            {
                IsRedownload = isRedownload,
                FilesToDownload = downloadList,
                DownloadLocation = txtDownloadLocation.Text
            };
            imageDownloader.FileDownloaded += (sender, args) =>
            {
                var downloadedImage = args.ImageDownloaded;
                int ind = downloadList.IndexOf(downloadedImage);
                dataGridView1.Rows[ind].DefaultCellStyle.BackColor = args.ImageDownloaded.ErrorDownloading ? Color.DarkRed : Color.GreenYellow;
                if (args.Error == null)
                {
                    imageDownloaderWorker.ReportProgress(args.FilesDownloaded);
                    previewPictureBox.Image = downloadedImage.Content;
                }
                dataGridView1.InvalidateRow(ind);
            };
            imageDownloader.DownloadCompleted += (sender, args) => downloaderFinished = true;
            imageDownloader.DownloadAsync();

            while (!downloaderFinished)
            {
                Thread.Sleep(100);
            }

        }
    }
}
 