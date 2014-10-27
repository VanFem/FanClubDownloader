using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace BeastsLairConnector
{
    public class BLForum
    {


        private string _baseUrl;

        public string ForumUrl { get; private set; }
        public List<BLPage> ForumThreads { get; private set; }

        public BLForum()
        {
            ForumThreads = new List<BLPage>();
        }

        public BLForum(string forumUrl) : this()
        {
            ForumUrl = forumUrl;
            Load(forumUrl);
        }

        public void Load(string url)
        {
            var uri = new Uri(url);

            _baseUrl = uri.Scheme + "://" + uri.Authority;
            ForumUrl = url;
            Console.WriteLine("Parsing forum "+url);
            ParseNextThreadPage(url);
        }

        
        private void ParseNextThreadPage(string url)
        {
            var web = new HtmlWeb();
            var document = web.Load(url);
            var forums = document.DocumentNode.SelectNodes("//h3[contains(concat(' ',@class,' '),' threadtitle ')]//a");
            foreach (var forum in forums)
            {
                var threadUrl = forum.GetAttributeValue("href", null);
                if (!IsValidNonRelativeUrl(threadUrl))
                {
                    threadUrl = _baseUrl + "/" + threadUrl;
                }

                if (ForumThreads.All(f => f.PageUrl != threadUrl))
                {
                    ForumThreads.Add(new BLPage(threadUrl, false));
                    Console.WriteLine("Found thread "+threadUrl);
                }
            }

            var nextPageAnchor =
                  document.DocumentNode.Descendants("a")
                      .FirstOrDefault(d => d.Attributes.Contains("rel") && (d.Attributes["rel"].Value == "next"));
            
            string nextPageUrl = null;
            if (nextPageAnchor != null)
            {
                nextPageUrl = nextPageAnchor.GetAttributeValue("href", null);
                if (!IsValidNonRelativeUrl(nextPageUrl))
                {
                    nextPageUrl = _baseUrl + "/" + nextPageUrl;
                }
            }

            if (!string.IsNullOrEmpty(nextPageUrl))
            {
                ParseNextThreadPage(nextPageUrl);
            }
        }

        private bool IsValidNonRelativeUrl(string url)
        {
            Uri uri;
            return (Uri.TryCreate(url, UriKind.Absolute, out uri));
        }

    }
}
