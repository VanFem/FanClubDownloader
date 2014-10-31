using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace Downloader
{
    public class Downloader
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
            public string FileBeingDownloaded { get; set; }
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

        
        public List<string> FilesToDownload { get; private set; }
        public int NewThreadPause { get; set; }
        public int MaxClientThreads { get; set; }
        public string DownloadLocation { get; set; }
        
        
        private int _filesDownloaded;
        public delegate void ProgressChangedHandler(object sender, ProgressChangedEventArgs e);
        public delegate void DownloadCompletedHandler(object sender, DownloadCompletedEventArgs e);
        public event ProgressChangedHandler ProgressChanged;
        public event ProgressChangedHandler FileDownloaded;
        public event DownloadCompletedHandler DownloadCompleted;

        public class WebDownload
        {
            public WebDownload(WebClient wc, string fileName)
            {
                Client = wc;
                FileName = fileName;
            }

            public WebClient Client { get; private set; }
            public string FileName { get; private set; }
        }

        private List<WebDownload> _clients = new List<WebDownload>();

        
        
        public Downloader()
        {
            FilesToDownload = new List<string>();
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

        private void DownloadFile(string url)
        {
            var dUri = new Uri(url);
            string dPath = GetFreeFileName(url);

            while (_clients.Count >= MaxClientThreads)
            {
                Thread.Sleep(100);
            }

            Thread.Sleep(NewThreadPause);

            var wc = new WebClient();
            wc.DownloadProgressChanged += DownloadProgressChanged;
            wc.DownloadFileCompleted += DownloadFileCompleted;
            Directory.CreateDirectory(Path.GetDirectoryName(dPath));
            File.Create(dPath).Close();
            wc.DownloadFileAsync(dUri, dPath);
            _clients.Add(new WebDownload(wc, dPath));
        }

        private string GetFreeFileName(string url)
        {
            var fileName = Regex.Replace(url.Split('/').Last().Split('?')[0], @"\%[\da-f]{2}", "");
            var fileExtension = string.Empty;

            if (fileName.Contains('.'))
            {
                fileExtension = fileName.Split('.').Last();
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

            if (e.Cancelled)
            {
                DownloadCompleted(sender,
                    new DownloadCompletedEventArgs {Cancelled = true, FilesDownloaded = _filesDownloaded});
                
                return;
            }
            _filesDownloaded++;
            FileDownloaded(sender, new ProgressChangedEventArgs(_filesDownloaded, FilesToDownload.Count) { FileBeingDownloaded = wd.FileName });
            if (_filesDownloaded == FilesToDownload.Count)
            {
                DownloadCompleted(sender,
                new DownloadCompletedEventArgs { Cancelled = false, FilesDownloaded = FilesToDownload.Count });
            }
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var wd = _clients.Single(w => w.Client == sender as WebClient);
            ProgressChanged(sender, new ProgressChangedEventArgs(e, _filesDownloaded, FilesToDownload.Count){FileBeingDownloaded = wd.FileName});
        }
    }
}
