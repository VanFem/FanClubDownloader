﻿using System;
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
    public partial class Form1 : Form
    {
        private BeastsLair _bl;
        private BackgroundWorker _forumLoader;
        private BLForum _selectedForum;

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
    }
}
