using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

using BeastsLairConnector;

namespace FanClubLoader
{
    public partial class Form1 : Form
    {
        private BeastsLair _bl;
        private BackgroundWorker _forumLoader;
        private BackgroundWorker _asyncLauncher;
        private BackgroundWorker _imageLoader;
        private BLForum _selectedForum;
        private BLThread _selectedThread;
        private int _currentPageIndex;

        public Form1()
        {
            InitializeComponent();
            _forumLoader = new BackgroundWorker();
            _forumLoader.DoWork += (sender, args) => { _bl = new BeastsLair("http://forums.nrvnqsr.com"); };
            _forumLoader.RunWorkerCompleted += (sender, args) =>
            {
                if (_bl.Forums.Count != 0)
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

        private void UpdateListAsync()
        {
            listView1.TileSize = ImageLoader.ImageListSize;
            if (listView1.LargeImageList == null)
            {
                listView1.LargeImageList = new ImageList
                {
                    ImageSize = ImageLoader.ImageListSize,
                    ColorDepth = ColorDepth.Depth32Bit
                };
            }
            else
            {
                listView1.LargeImageList.Images.Clear();
            }
            listView1.Items.Clear();
            lblPageNum.Text = (_currentPageIndex + 1).ToString();

            _asyncLauncher = new BackgroundWorker();

            _asyncLauncher.DoWork += (o1, a1) =>
            {
                if (_imageLoader != null && _imageLoader.IsBusy)
                {
                    _imageLoader.CancelAsync();
                    while (_imageLoader.IsBusy)
                    {
                        Thread.Sleep(100);
                    }
                }
            };

            _asyncLauncher.RunWorkerCompleted += (o1, a1) =>
            {
                var loader = new ImageLoader(_selectedThread.LoadedPages[_currentPageIndex]);
                _imageLoader = new BackgroundWorker();
                _imageLoader.DoWork += (o, args) => loader.LoadImages(_imageLoader);
                _imageLoader.RunWorkerCompleted += (o, args) =>
                {
                    btnNextPage.Enabled = true;
                    btnPrevPage.Enabled = true;
                };
                _imageLoader.WorkerReportsProgress = true;
                _imageLoader.WorkerSupportsCancellation = true;
                _imageLoader.ProgressChanged += (o, ar) =>
                {
                    if (_imageLoader.CancellationPending) return;
                    var args = ar.UserState as ImageLoader.ListUpdatedArgs;
                    if (args == null) return;
                    listView1.LargeImageList.Images.Add(args.ThumbImage);
                    listView1.Items.Add(new ListViewItem(string.Empty, listView1.Items.Count));
                    if (pictureBox1.Image == null)
                    {
                        pictureBox1.Image = args.AddedImage;
                    }
                };
                _imageLoader.RunWorkerAsync();
            };

            _asyncLauncher.RunWorkerAsync();
        }
        
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            _selectedThread = _selectedForum.ForumThreads[e.RowIndex];
            _selectedThread.OpeningPost = new BLPage(_selectedThread.OpeningPostUrl);
            _selectedThread.LoadedPages.Add(_selectedThread.OpeningPost);
            pictureBox1.Image = null;
            _currentPageIndex = 0;
            lblPagesAmt.Text = _selectedThread.PagesAmount.ToString();
            lblThreadName.Text = _selectedForum.ForumThreads[e.RowIndex].ThreadName;
            lblAuthorName.Text = _selectedThread.Author;
            UpdateListAsync();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                pictureBox1.Image =_selectedThread.LoadedPages[_currentPageIndex].GetCachedImageWithIndex(listView1.SelectedIndices[0]);
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
            btnNextPage.Enabled = false;
            btnPrevPage.Enabled = false;

            if (_currentPageIndex == _selectedThread.LoadedPages.Count-1)
            {
                if (_selectedThread.LoadedPages.Last().GetNextPageUrl() == null) return;
                _selectedThread.LoadedPages.Add(new BLPage(_selectedThread.LoadedPages.Last().GetNextPageUrl()));
                _currentPageIndex++;
                UpdateListAsync();
            }
            else
            {
                _currentPageIndex++;
                UpdateListAsync();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            btnNextPage.Enabled = false;
            btnPrevPage.Enabled = false;
            if (_currentPageIndex == 0) return;
            _currentPageIndex--;
            UpdateListAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _currentPageIndex = 0;
            _selectedThread.LoadedPages.Clear();
            _selectedThread.OpeningPost = new BLPage(_selectedThread.OpeningPostUrl);
            _selectedThread.LoadedPages.Add(_selectedThread.OpeningPost);
            _selectedThread.PagesAmount = _selectedThread.OpeningPost.GetPageMax();
            _selectedThread.LastUpdated = DateTime.Now;
            UpdateListAsync();
        }

    }
}
