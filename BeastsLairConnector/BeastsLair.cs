using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
