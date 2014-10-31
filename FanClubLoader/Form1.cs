using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using BeastsLairConnector;
using FanClubLoader.Properties;

namespace FanClubLoader
{
    public partial class Form1 : Form
    {

        private const string DataFileName = "cache.dat";
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
            ReadConfigFromFile();
            cmbForumSelect.ValueMember = "ForumUrl";
            cmbForumSelect.DisplayMember = "ForumName";
            if (_bl == null)
            {
                _forumLoader = new BackgroundWorker();
                _forumLoader.DoWork += (sender, args) => { _bl = new BeastsLair("http://forums.nrvnqsr.com"); };
                _forumLoader.RunWorkerCompleted += (sender, args) =>
                {
                    if (_bl.Forums.Count != 0)
                    {
                        cmbForumSelect.Items.AddRange(_bl.Forums.ToArray());
                    }
                };
                _forumLoader.RunWorkerAsync();
            }
            else
            {
                if (_bl.Forums.Count != 0)
                {
                    cmbForumSelect.Items.AddRange(_bl.Forums.ToArray());
                    
                }
            }
        }

        private void cmbForumSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedForum = (cmbForumSelect.SelectedItem as BLForum);
            if (_selectedForum.ForumThreads.Count == 0)
            {
                InitSelectedForum();
            }
            else
            {
                bLThreadBindingSource.DataSource = _selectedForum.ForumThreads;
            }

            dataGridView1.Select(); // For insta-mouse wheel usage
        }

        private void InitSelectedForum()
        {
            btnRefreshThreadList.Enabled = false;
            btnRefreshForumList.Enabled = false;
            cmbForumSelect.Enabled = false;
            _forumLoader = new BackgroundWorker();
            _forumLoader.DoWork += (s, a) => _selectedForum.Load();
            _forumLoader.RunWorkerCompleted += (o, args) =>
            {
                bLThreadBindingSource.DataSource = _selectedForum.ForumThreads;
                cmbForumSelect.Enabled = true;
                btnRefreshForumList.Enabled = true;
                btnRefreshThreadList.Enabled = true;
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
            var resizedLoading = ImageLoader.ResizeImage(Resources.Loading, ImageLoader.ImageListSize.Width, ImageLoader.ImageListSize.Height);
            for (int i = 0; i < _selectedThread.LoadedPages[_currentPageIndex].Images.Count; i++)
            {
                listView1.LargeImageList.Images.Add(resizedLoading);
                listView1.Items.Add(new ListViewItem("", i));
            }
            
            
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
                int currentlyLoadedImages = 0;
                int maxImages = _selectedThread.LoadedPages[_currentPageIndex].Images.Count;
                var loader = new ImageLoader(_selectedThread.LoadedPages[_currentPageIndex]);
                _imageLoader = new BackgroundWorker();
                _imageLoader.DoWork += (o, args) => loader.LoadImages(_imageLoader);
                _imageLoader.RunWorkerCompleted += (o, args) =>
                {
                    btnNextPage.Enabled = true;
                    btnPrevPage.Enabled = true;
                    WriteConfigToFile();
                };
                _imageLoader.WorkerReportsProgress = true;
                _imageLoader.WorkerSupportsCancellation = true;
                _imageLoader.ProgressChanged += (o, ar) =>
                {
                    if (_imageLoader.CancellationPending) return;
                    var args = ar.UserState as ImageLoader.ListUpdatedArgs;
                    if (args == null) return;
                    if (listView1.LargeImageList == null)
                    {
                        listView1.LargeImageList = new ImageList
                        {
                            ImageSize = ImageLoader.ImageListSize,
                            ColorDepth = ColorDepth.Depth32Bit
                        };
                    }
                    currentlyLoadedImages++;
                    if (listView1.LargeImageList.Images.Count < currentlyLoadedImages)
                    {
                        listView1.LargeImageList.Images.Add(args.ThumbImage);
                        listView1.Items.Add(new ListViewItem(string.Empty, listView1.Items.Count));
                    }
                    else
                    {
                        listView1.LargeImageList.Images[currentlyLoadedImages - 1] = args.ThumbImage;
                        FillListView(maxImages);
                    }
                    
                    if (pictureBox1.Image == null)
                    {
                        pictureBox1.Image = args.AddedImage;
                    }
                };
                _imageLoader.RunWorkerAsync();
            };

            _asyncLauncher.RunWorkerAsync();
        }

        private void FillListView(int max)
        {
            listView1.Items.Clear();
            for (int i = 0; i < max; i++)
            {
                listView1.Items.Add(new ListViewItem("", i));
            }
        }
        
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            _selectedThread = _selectedForum.ForumThreads[e.RowIndex];
            if (_selectedThread.OpeningPost == null)
            {
                _selectedThread.OpeningPost = new BLPage(_selectedThread.OpeningPostUrl);
                _selectedThread.LoadedPages.Add(_selectedThread.OpeningPost);
            }
            pictureBox1.Image = null;
            _currentPageIndex = 0;
            InitThreadDetails();
            UpdateListAsync();
        }

        private void InitThreadDetails()
        {
            lblLastUpdated.Text = _selectedThread.GetDateString;
            lblPagesAmt.Text = _selectedThread.PagesAmount.ToString();
            lblThreadName.Text = _selectedThread.ThreadName;
            lblAuthorName.Text = _selectedThread.Author;
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

        private void nextPageButton_Click(object sender, EventArgs e)
        {
            if (_selectedThread == null) return;
            btnNextPage.Enabled = false;
            btnPrevPage.Enabled = false;

            if (_currentPageIndex == _selectedThread.LoadedPages.Count-1)
            {
                if (_selectedThread.LoadedPages.Last().GetNextPageUrl() == null) return;
                _selectedThread.LoadedPages.Add(_selectedThread.RemoveQuotesAndRepeats(new BLPage(_selectedThread.LoadedPages.Last().GetNextPageUrl())));
                _currentPageIndex++;
                UpdateListAsync();
            }
            else
            {
                _currentPageIndex++;
                UpdateListAsync();
            }
        }

        private void prevPageButton_Click(object sender, EventArgs e)
        {
            if (_currentPageIndex == 0) return;
            if (_selectedThread == null) return;
            btnNextPage.Enabled = false;
            btnPrevPage.Enabled = false;
            _currentPageIndex--;
            UpdateListAsync();
        }

        private void refreshThreadButton_Click(object sender, EventArgs e)
        {
            if (_selectedThread == null) return;
            _currentPageIndex = 0;
            _selectedThread.LoadedPages.Clear();
            _selectedThread.OpeningPost = new BLPage(_selectedThread.OpeningPostUrl);
            _selectedThread.LoadedPages.Add(_selectedThread.OpeningPost);
            _selectedThread.PagesAmount = _selectedThread.OpeningPost.GetPageMax();
            _selectedThread.LastUpdated = DateTime.Now;
            InitThreadDetails();
            UpdateListAsync();
        }

        private void WriteConfigToFile()
        {
            var xser = new DataContractSerializer(typeof(BeastsLair));
            var tw = new FileStream(DataFileName, FileMode.Create);
            xser.WriteObject(tw, _bl);
            tw.Close();
        }

        private void ReadConfigFromFile()
        {
            if (File.Exists(DataFileName))
            {
                try
                {
                    var fs = new FileStream(DataFileName, FileMode.Open);
                    XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                    var ser = new DataContractSerializer(typeof(BeastsLair));
                    _bl = (BeastsLair)ser.ReadObject(reader, true);
                    reader.Close();
                    fs.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Cannot read data: " + e.Message);
                }
            }
        }

        private void InitBLComboBox()
        {
            cmbForumSelect.Enabled = false;
            btnRefreshThreadList.Enabled = false;
            btnRefreshForumList.Enabled = false;
            _forumLoader = new BackgroundWorker();
            _forumLoader.DoWork += (sender, args) => { _bl = new BeastsLair("http://forums.nrvnqsr.com"); };
            _forumLoader.RunWorkerCompleted += (sender, args) =>
            {
                if (_bl.Forums.Count != 0)
                {
                    cmbForumSelect.Items.AddRange(_bl.Forums.ToArray());
                }
                cmbForumSelect.Enabled = true;
                btnRefreshThreadList.Enabled = true;
                btnRefreshForumList.Enabled = true;
            };
            _forumLoader.RunWorkerAsync();
            
        }

        private void refreshForumList_Click(object s, EventArgs e)
        {
            bLThreadBindingSource.DataSource = null;
            cmbForumSelect.Items.Clear();
            InitBLComboBox();
        }

        private void refreshThreadList_Click(object sender, EventArgs e)
        {
            _selectedForum.ForumThreads.Clear();
            InitSelectedForum();
        }

        private void btnScanThread_Click(object sender, EventArgs e)
        {
            var scanForm = new ThreadScan(_selectedThread);
            scanForm.Show();
            scanForm.StartScan();
        }
    }
}
