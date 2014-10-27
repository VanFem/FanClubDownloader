using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace BeastsLairConnector
{
    class Program
    {
        static void Main(string[] args)
        {
            var scanner = new BLScanner();
            scanner.Scan("http://forums.nrvnqsr.com/showthread.php/4539-The-Huntress-of-Arcadia-Atalanta-s-FC");
            var downloader = new Downloader.Downloader();
            
            downloader.FilesToDownload.AddRange(scanner.ImageUrls);

            downloader.DownloadLocation = @"C:\ImagesDL\";

            downloader.MaxClientThreads = 5;
            downloader.NewThreadPause = 0;

            downloader.ProgressChanged += DownloaderOnProgressChanged;
            downloader.FileDownloaded += DownloaderOnFileDownloaded;
            downloader.DownloadCompleted += DownloaderOnDownloadCompleted;

            downloader.DownloadAsync();

            while (true)
            {
                Thread.Sleep(100);
            }
        }

        private static void DownloaderOnDownloadCompleted(object sender, Downloader.Downloader.DownloadCompletedEventArgs downloadCompletedEventArgs)
        {
            Console.WriteLine("Download completed. (Cancelled: {0}, files downloaded: {1})", downloadCompletedEventArgs.Cancelled, downloadCompletedEventArgs.FilesDownloaded);
        }

        private static void DownloaderOnFileDownloaded(object sender, Downloader.Downloader.ProgressChangedEventArgs progressChangedEventArgs)
        {
            Console.WriteLine("{0}: Download complete",
                progressChangedEventArgs.FileBeingDownloaded);
            Console.ReadLine();
        }

        private static void DownloaderOnProgressChanged(object sender, Downloader.Downloader.ProgressChangedEventArgs progressChangedEventArgs)
        {
            Console.WriteLine("{0}: {1}% ( {2} / {3} b)", progressChangedEventArgs.FileBeingDownloaded,
                progressChangedEventArgs.CurrentDownloadProgressPercentage,
                progressChangedEventArgs.CurrentFileBytesReceived,
                progressChangedEventArgs.CurrentFileTotalBytesToReceive);
        }
    }
}
