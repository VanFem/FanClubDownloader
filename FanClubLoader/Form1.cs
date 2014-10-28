using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BeastsLairConnector;
using Encoder = System.Text.Encoder;

namespace FanClubLoader
{
    public partial class Form1 : Form
    {
        private static readonly Size ImageListSize = new Size(170,170);

        private BeastsLair _bl;
        private BackgroundWorker _forumLoader;
        private BLForum _selectedForum;
        private BLThread _selectedThread;
        private int _currentPageIndex;
        private List<Image> _imagesTemporaryList;

        public Form1()
        {
            InitializeComponent();
            _forumLoader = new BackgroundWorker();
            _forumLoader.DoWork += (sender, args) => { _bl = new BeastsLair("http://forums.nrvnqsr.com"); };
            _forumLoader.RunWorkerCompleted += (sender, args) =>
            {
                if (_bl.Forums.Count != null)
                {
                    cmbForumSelect.Items.AddRange(_bl.Forums.ToArray());
                    cmbForumSelect.ValueMember = "ForumUrl";
                    cmbForumSelect.DisplayMember = "ForumName";
                }
            };
            _forumLoader.RunWorkerAsync();
        }

        private void cmbForumSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbForumSelect.Enabled = false;
            _selectedForum = (cmbForumSelect.SelectedItem as BLForum);
            _forumLoader = new BackgroundWorker();
            _forumLoader.DoWork += (s, a) => _selectedForum.Load();
            _forumLoader.RunWorkerCompleted += (o, args) =>
            {
                bLThreadBindingSource.DataSource = _selectedForum.ForumThreads;
                cmbForumSelect.Enabled = true;
            };
            _forumLoader.RunWorkerAsync();
        }

        private void UpdateListBox()
        {
            lblPageNum.Text = (_currentPageIndex+1).ToString();
            listView1.LargeImageList = new ImageList();
            listView1.LargeImageList.ImageSize = ImageListSize;
            listView1.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
            _imagesTemporaryList = _selectedThread.LoadedPages[_currentPageIndex].Images.Select(im =>
            {
                var str = GetImageStreamFromUrl(im);
                try
                {
                    return str != null ? Image.FromStream(str) : null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }).Where(s => s != null).ToList();

            listView1.LargeImageList.Images.AddRange(_imagesTemporaryList.Select(im => resizeImage(im, ImageListSize.Width, ImageListSize.Height)).ToArray());

            int i = 0;
            listView1.TileSize = new Size(128, 128);
            listView1.Items.Clear();
            foreach (var img in listView1.LargeImageList.Images)
            {
                listView1.Items.Add(new ListViewItem("", i++));
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            _selectedThread = _selectedForum.ForumThreads[e.RowIndex];
            _selectedThread.OpeningPost = new BLPage(_selectedThread.OpeningPostUrl);
            _selectedThread.LoadedPages.Add(_selectedThread.OpeningPost);
            _currentPageIndex = 0;
            lblPagesAmt.Text = _selectedThread.PagesAmount.ToString();


            lblThreadName.Text = _selectedForum.ForumThreads[e.RowIndex].ThreadName;
            lblAuthorName.Text = _selectedThread.Author;

            UpdateListBox();

            if (_selectedThread.LoadedPages[_currentPageIndex].Images.Count > 0)
            {
                var str = GetImageStreamFromUrl(_selectedThread.LoadedPages[_currentPageIndex].Images[0]);
                pictureBox1.Image = str != null ? Image.FromStream(str) : null;
            }
            else
            {
                pictureBox1.Image = null;
            }
        }

        private Stream GetImageStreamFromUrl(string url)
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(url);
                request.Timeout = 1000;
                return request.GetResponse().GetResponseStream();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private Image resizeImage(Image originalImage, int canvasWidth, int canvasHeight)
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

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                pictureBox1.Image = _imagesTemporaryList[listView1.SelectedIndices[0]];
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = txtDownloadLocation.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDownloadLocation.Text = folderBrowserDialog1.SelectedPath;
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (_currentPageIndex == _selectedThread.LoadedPages.Count-1)
            {
                if (_selectedThread.LoadedPages.Last().GetNextPageUrl() != null)
                {
                    _selectedThread.LoadedPages.Add(new BLPage(_selectedThread.LoadedPages.Last().GetNextPageUrl()));
                    _currentPageIndex++;
                    UpdateListBox();
                }
            }
            else
            {
                _currentPageIndex++;
                UpdateListBox();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_currentPageIndex == 0) return;
            _currentPageIndex--;
            UpdateListBox();
        }

    }
}
