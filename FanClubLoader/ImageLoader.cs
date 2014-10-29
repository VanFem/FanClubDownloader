using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BeastsLairConnector;

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

        public delegate void ListBoxUpdateHandler(object sender, ListUpdatedArgs args);

        public event ListBoxUpdateHandler ListBoxUpdated;

        private static Image ResizeImage(Image originalImage, int canvasWidth, int canvasHeight)
        {
            Image image = originalImage;
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            System.Drawing.Image thumbnail =
                new Bitmap(canvasWidth, canvasHeight); // changed parm names
            System.Drawing.Graphics graphic =
                         System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            graphic.Clear(Color.White); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            /* ------------- end new code ---------------- */

            return thumbnail;
        }

        private Stream GetImageStreamFromUrl(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 1000;
                return request.GetResponse().GetResponseStream();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void LoadImages(BackgroundWorker bw)
        {
            foreach (var img in _loadingPage.Images)
            {
                if (bw.CancellationPending) return;
                Image image;
                if (img.Content != null)
                {
                    image = img.Content;
                }
                else
                {
                    try
                    {
                        var str = GetImageStreamFromUrl(img.Url);
                        image = Image.FromStream(str);
                        img.Content = image;
                    }
                    catch
                    {
                        continue;
                    }
                }
                var thumb = img.Thumbnail ?? ResizeImage(image, ImageListSize.Width, ImageListSize.Height);
                if (!bw.CancellationPending) bw.ReportProgress(0, new ListUpdatedArgs(image, thumb));
            }
        }

    }

}
