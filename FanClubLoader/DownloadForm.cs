using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BeastsLairConnector;

namespace FanClubLoader
{
    public partial class DownloadForm : Form
    {
        private BLThread _myThread;
        private int _pageMax;
        private int _postsPerPage;
        private const string DefaultPostsPerPage = "20";

        public DownloadForm(BLThread thread)
        {
            InitializeComponent();
            _myThread = thread;
            cmbPostsPerPage.SelectedText = DefaultPostsPerPage;
            _postsPerPage = Int32.Parse(cmbPostsPerPage.SelectedText);
            _pageMax = thread.OpeningPost.GetPageMax();
            
            numFromPage.Minimum = 1;
            numFromPage.Maximum = _pageMax;
            numFromPage.Value = 1;

            numToPage.Minimum = 1;
            numToPage.Maximum = _pageMax;
            numToPage.Value = _pageMax;
        }

        private Point convertPages(int pFrom, int pTo, int pPerFrom, int pPerTo)
        {
            int pFromNew = (pFrom*pPerFrom)/pPerTo;
            int pToNew = (pTo*pPerFrom)/pPerTo;
            if (pTo*pPerFrom%pPerTo > 0)
            {
                pToNew++;
            }
            return new Point(pFromNew, pToNew);
        }

        private void cmbPostsPerPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newPostsPerPage = int.Parse(cmbPostsPerPage.SelectedText);
            var newPages = convertPages((int) numFromPage.Value, (int) numToPage.Value, _postsPerPage, newPostsPerPage);
            int newMax = (_pageMax*_postsPerPage)/newPostsPerPage;
            if ((_pageMax*_postsPerPage)%newPostsPerPage > 0) newMax++;
            numFromPage.Maximum = newMax;
            numToPage.Maximum = newMax;
            numFromPage.Value = newPages.X;
            numToPage.Value = newPages.Y;
        }

        private void btnBrowseDownloadLocation_Click(object sender, EventArgs e)
        {
            dlFolderSelectDialog.SelectedPath = txtDownloadLocation.Text;
            if (dlFolderSelectDialog.ShowDialog() == DialogResult.OK)
            {
                txtDownloadLocation.Text = dlFolderSelectDialog.SelectedPath;
            }
        }
    }
}
