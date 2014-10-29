using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using HtmlAgilityPack;

namespace BeastsLairConnector
{
    [DataContract]
    public class BeastsLair
    {
        [IgnoreDataMember]
        public HtmlDocument Document;
        
        private string _baseUrl;

        [DataMember]
        public List<BLForum> Forums { get; set; }

        public BeastsLair()
        {
            Forums = new List<BLForum>();
        }

        public BeastsLair(string url) : this()
        {
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
            var forumContainers =
                Document.DocumentNode.SelectNodes("//li[contains(concat(' ',@class,' '),' forumbit_nopost ')]");
            if (forumContainers == null) return;
            foreach (var fC in forumContainers)
            {
                ParseForumContainer(fC);
            }}

        private void ParseForumContainer(HtmlNode fC)
        {
            var forumTitleNode = fC.SelectSingleNode(".//span[contains(concat(' ',@class,' '), 'forumtitle')]//a");
            var forumList = fC.NextSibling;
            var forumTitle = forumTitleNode.InnerHtml;
            if (forumList == null) return;
            var forum = forumList.SelectSingleNode(".//h2[contains(concat(' ',@class,' '), 'forumtitle')]//a");
            while (
                forumList != null && !forumList.GetAttributeValue("class", "").Contains("forumbit_nopost"))
            {
                if (forum != null)
                {
                    var forumUrl = forum.GetAttributeValue("href", null);
                    if (!IsValidNonRelativeUrl(forumUrl))
                    {
                        forumUrl = _baseUrl + "/" + forumUrl;
                    }
                    var forumName = forumTitle + ": " + WebUtility.HtmlDecode(forum.InnerHtml);
                    Forums.Add(new BLForum(forumUrl) {ForumName = forumName});
                }
                forumList = forumList.NextSibling;
                if (forumList !=null)
                    forum = forumList.SelectSingleNode(".//h2[contains(concat(' ',@class,' '), 'forumtitle')]//a");
            }

        }

        private bool IsValidNonRelativeUrl(string url)
        {
            Uri uri;
            return (Uri.TryCreate(url, UriKind.Absolute, out uri));
        }

    }
}
