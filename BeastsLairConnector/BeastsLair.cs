using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace BeastsLairConnector
{
    public class BeastsLair
    {
        public HtmlDocument Document;
        private string _baseUrl;

        public List<BLForum> Forums { get; private set; }

        public BeastsLair(string url)
        {
            Forums = new List<BLForum>();
            Load(url);
        }

        public void Load(string url)
        {
            var uri = new Uri(url);

            _baseUrl = uri.Scheme + "://" + uri.Authority;

            var web = new HtmlWeb();
            Document = web.Load(url);
            ParseForums();
        }

        private void ParseForums()
        {
           var forums = Document.DocumentNode.SelectNodes("//h2[contains(concat(' ',@class,' '),' forumtitle ')]//a");
            foreach (var forum in forums)
            {
                var forumurl = forum.GetAttributeValue("href", null);
                if (!IsValidNonRelativeUrl(forumurl))
                {
                    forumurl = _baseUrl + "/" + forumurl;
                }
                Forums.Add(new BLForum(forumurl));
            }
        }

        private bool IsValidNonRelativeUrl(string url)
        {
            Uri uri;
            return (Uri.TryCreate(url, UriKind.Absolute, out uri));
        }

    }
}
