using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using BeastsLairConnector;
using FanClubLoader.Properties;

namespace FanClubLoader
{
    public class ImageLoader
    {
        public static readonly Size ImageListSize = new Size(170, 170);
        private BLPage _loadingPage;

        public ImageLoader(BLPage page)
        {
            _loadingPage = page;
        }

        public class ListUpdatedArgs : EventArgs
        {
            public Image AddedImage;
            public Image ThumbImage;

            public ListUpdatedArgs(Image addedImage, Image thumbImage)
            {
                AddedImage = addedImage;
                ThumbImage = thumbImage;
            }
        }
        
        public static Image ResizeImage(Image originalImage, int canvasWidth, int canvasHeight)
        {
            Image image = originalImage;
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            Image thumbnail = new Bitmap(canvasWidth, canvasHeight);
            var graphic = Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            graphic.Clear(Color.White); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            
            return thumbnail;
        }

        private Stream GetImageStreamFromUrl(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Timeout = 7000;
            return request.GetResponse().GetResponseStream();
        }

        public void LoadImages(BackgroundWorker bw)
        {
            foreach (var img in _loadingPage.Images)
            {
                if (bw.CancellationPending) return;
                Image image = null;
                if (img.Content != null)
                {
                    image = img.Content;
                } 
                else if (!string.IsNullOrEmpty(img.LocalPath))
                {
                    if (!File.Exists(img.LocalPath))
                    {
                        img.Downloaded = false;
                        img.LocalPath = null;
                    } else image = Image.FromFile(img.LocalPath);
                }

                if (image == null)
                {
                    try
                    {
                        var str = GetImageStreamFromUrl(img.Url);
                        image = Image.FromStream(str);
                        img.Content = image;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(img.Url + ": " + ex.Message);
                        image = Resources.imageCouldNotBeLoaded;
                        img.Content = image;
                    }
                }

                var thumb = img.Thumbnail ?? ResizeImage(image, ImageListSize.Width, ImageListSize.Height);
                img.Thumbnail = thumb;
                if (!bw.CancellationPending) bw.ReportProgress(0, new ListUpdatedArgs(image, thumb));
            }
        }

    }

}
