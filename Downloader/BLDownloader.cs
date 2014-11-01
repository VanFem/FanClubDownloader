using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Threading;
using BeastsLairConnector;

namespace Downloader
{
    public class BLDownloader
    {
        public class ProgressChangedEventArgs : EventArgs
        {
            public ProgressChangedEventArgs(DownloadProgressChangedEventArgs args, int filesDownloaded, int filesToDownload)
            {
                CurrentDownloadProgressPercentage = args.ProgressPercentage;
                CurrentFileBytesReceived = args.BytesReceived;
                CurrentFileTotalBytesToReceive = args.TotalBytesToReceive;
                FilesDownloaded = filesDownloaded;
                TotalFilesToDownload = filesToDownload;
                ProgressPercentage = (100*filesDownloaded/filesToDownload);
            }

            public ProgressChangedEventArgs(int filesDownloaded, int filesToDownload)
            {
                FilesDownloaded = filesDownloaded;
                TotalFilesToDownload = filesToDownload;
                ProgressPercentage = (100 * filesDownloaded / filesToDownload);
            }

            public bool Cancelled { get; set; }
            public Exception Error { get; set; }
            public BLImage ImageDownloaded { get; set; }
            public int ProgressPercentage { get; set; }
            public int FilesDownloaded { get; set; }
            public int TotalFilesToDownload { get; set; }
            public int CurrentDownloadProgressPercentage { get; private set; }
            public long CurrentFileTotalBytesToReceive { get; private set; }
            public long CurrentFileBytesReceived { get; private set; }
        }

        public class DownloadCompletedEventArgs : EventArgs
        {
            public int FilesDownloaded { get ; set; }
            public bool Cancelled { get; set; }
        }
        
        public List<BLImage> FilesToDownload { get; set; }
        public int NewThreadPause { get; set; }
        public int MaxClientThreads { get; set; }
        public string DownloadLocation { get; set; }
        public bool IsRedownload { get; set; }
        
        
        private int _filesFinishedDownloading;
        public delegate void ProgressChangedHandler(object sender, ProgressChangedEventArgs e);
        public delegate void DownloadCompletedHandler(object sender, DownloadCompletedEventArgs e);
        public event ProgressChangedHandler ProgressChanged;
        public event ProgressChangedHandler FileDownloaded;
        public event DownloadCompletedHandler DownloadCompleted;

        public class WebDownload
        {
            public WebDownload(WebClient wc, string fileName, BLImage imageDownloaded)
            {
                Client = wc;
                FileName = fileName;
                ImageDownloaded = imageDownloaded;
            }

            public WebClient Client { get; private set; }
            public string FileName { get; private set; }
            public BLImage ImageDownloaded { get; private set; }
        }

        private List<WebDownload> _clients = new List<WebDownload>();

        
        
        public BLDownloader()
        {
            FilesToDownload = new List<BLImage>();
            MaxClientThreads = 3;
            NewThreadPause = 100;
        }

        public void DownloadAsync()
        {
            var bw = new BackgroundWorker();
            bw.DoWork += Download;
            bw.RunWorkerAsync();
        }

        private void Download(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            foreach (var file in FilesToDownload)
            {
                DownloadFile(file);
            }
        }

        private void DownloadFile(BLImage image)
        {
            if (image.Downloaded && !IsRedownload)
            {
                FileDownloaded(this, new ProgressChangedEventArgs(++_filesFinishedDownloading, FilesToDownload.Count)
                {
                    Cancelled = false,
                    ImageDownloaded = image,
                    Error = null,
                });
                return;
            }
            var dUri = new Uri(image.Url);
            string dPath = GetFreeFileName(image.Url);

            while (_clients.Count >= MaxClientThreads)
            {
                Thread.Sleep(100);
            }

            Thread.Sleep(NewThreadPause);

            var wc = new WebClient();
            wc.DownloadProgressChanged += DownloadProgressChanged;
            wc.DownloadFileCompleted += DownloadFileCompleted;
            Directory.CreateDirectory(Path.GetDirectoryName(dPath));
            if (!File.Exists(dPath))
            {
                File.Create(dPath).Close();
            }

            _clients.Add(new WebDownload(wc, dPath, image));
            wc.DownloadFileAsync(dUri, dPath);
        }

        private string GetFreeFileName(string url)
        {
            var fileName = Regex.Replace(url.Split('/').Last().Split('?')[0], @"\%[\da-f]{2}", "");
            var fileExtension = string.Empty;

            if (fileName.Contains('.'))
            {
                fileExtension = fileName.Split('.').Last();
                if (fileExtension.ToLower() == "php")
                {
                    fileExtension = "jpg";
                }
                fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
            }
            else
            {
                fileExtension = "jpg";
            }
            int i = 0;
            string filePath;
            filePath = string.IsNullOrEmpty(fileName)
                ? Path.Combine(DownloadLocation, fileName + "(" + i + ")." + fileExtension)
                : Path.Combine(DownloadLocation, fileName + "." + fileExtension);
            while (File.Exists(filePath))
            {
                i++;
                filePath = Path.Combine(DownloadLocation, fileName + "(" + i + ")."+fileExtension);
            }
            return filePath;
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var wd = _clients.Single(w => w.Client == sender as WebClient);
            _clients.RemoveAll(w => w.Client == sender as WebClient);

            if (e.Error != null)
            {
                _filesFinishedDownloading++;
                FileDownloaded(this, new ProgressChangedEventArgs(_filesFinishedDownloading, FilesToDownload.Count) {ImageDownloaded = wd.ImageDownloaded, Error = e.Error, Cancelled = false});
                File.Delete(wd.FileName);
                return;
            }

            if (e.Cancelled)
            {
                _filesFinishedDownloading++;
                FileDownloaded(this, new ProgressChangedEventArgs(_filesFinishedDownloading, FilesToDownload.Count) {ImageDownloaded = wd.ImageDownloaded, Error = null, Cancelled = true});
                return;
            }

            _filesFinishedDownloading++;
            wd.ImageDownloaded.Downloaded = true;
            wd.ImageDownloaded.LocalPath = wd.FileName;
            wd.ImageDownloaded.Content = Image.FromFile(wd.FileName);

            FileDownloaded(sender,
                new ProgressChangedEventArgs(_filesFinishedDownloading, FilesToDownload.Count)
                {
                    ImageDownloaded = wd.ImageDownloaded,
                    Error = null,
                    Cancelled = false
                });
            if (_filesFinishedDownloading == FilesToDownload.Count)
            {
                DownloadCompleted(this,
                    new DownloadCompletedEventArgs
                    {
                        Cancelled = false,
                        FilesDownloaded = FilesToDownload.Count,
                    });
            }
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //var wd = _clients.Single(w => w.Client == sender as WebClient);
            //ProgressChanged(sender, new ProgressChangedEventArgs(e, _filesFinishedDownloading, FilesToDownload.Count));
        }
    }
}
