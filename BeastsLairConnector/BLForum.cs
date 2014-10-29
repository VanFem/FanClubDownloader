using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using HtmlAgilityPack;

namespace BeastsLairConnector
{
    [DataContract]
    public class BLForum
    {
        private string _baseUrl;

        [DataMember]
        public string ForumName { get; set; }
        [DataMember]
        public string ForumUrl { get; set; }
        [DataMember]
        public List<BLThread> ForumThreads { get; private set; }

        public BLForum()
        {
            ForumThreads = new List<BLThread>();
        }

        public BLForum(string forumUrl) : this()
        {
            ForumUrl = forumUrl;
        }

        public void Load()
        {
            if (string.IsNullOrEmpty(ForumUrl))
            {
                throw new InvalidOperationException();
            }

            var uri = new Uri(ForumUrl);
            _baseUrl = uri.Scheme + "://" + uri.Authority;
            ParseNextThreadPage(ForumUrl);
         
        }

        public void Load(string url)
        {
            ForumUrl = url;
            Load();
        }

        
        private void ParseNextThreadPage(string url)
        {
            var web = new HtmlWeb();
            var document = web.Load(url);
            var threads = document.DocumentNode.SelectNodes("//h3[contains(concat(' ',@class,' '),' threadtitle ')]//a[contains(concat(' ',@class,' '),' title ')]");
            foreach (var thread in threads)
            {
                var threadUrl = thread.GetAttributeValue("href", null);
                var authorNode =
                    thread.ParentNode.ParentNode.SelectSingleNode(
                        ".//div[contains(concat(' ',@class,' '),' author ')]//span[contains(concat(' ',@class,' '),' label ')]//a");

                var pageAmtNode =
                    thread.ParentNode.ParentNode.SelectSingleNode(
                        ".//div[contains(concat(' ',@class,' '),' author ')]//dl[contains(concat(' ',@class,' '),' pagination ')]//dd//span[last()]//a");
                int pageAmt = 0;
                if (pageAmtNode != null)
                {
                    Int32.TryParse(pageAmtNode.InnerText.Trim(), out pageAmt);
                }
                
                
                if (!IsValidNonRelativeUrl(threadUrl))
                {
                    threadUrl = _baseUrl + "/" + threadUrl;
                }

                
                if (ForumThreads.All(op => op.OpeningPostUrl != threadUrl))
                {
                    ForumThreads.Add(new BLThread {OpeningPostUrl = threadUrl, ThreadName = WebUtility.HtmlDecode(thread.InnerHtml), Author = WebUtility.HtmlDecode(authorNode.InnerHtml), PagesAmount = pageAmt}); 
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
