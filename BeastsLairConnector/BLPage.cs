using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using HtmlAgilityPack;

namespace BeastsLairConnector
{
    [DataContract]
    public class BLPage
    {
        private HtmlDocument Document;
        [DataMember]
        public List<BLImage> Images { get; set; }
        [DataMember]
        public string NextPageUrl;
        [DataMember]
        public string PageUrl { get; set; }

        private string _baseUrl;

        public BLPage()
        {
            Images = new List<BLImage>();
        }

        public BLPage(string url)
            : this()
        {
            PageUrl = url;
            Load(url);
        }

        public void Load()
        {
            if (string.IsNullOrEmpty(PageUrl))
            {
                throw new InvalidOperationException("Cannot load page without initializing.");
            }
            Load(PageUrl);
        }

        public void Load(string url)
        {
            //try
            //{
            var uri = new Uri(url);
            
            _baseUrl = uri.Scheme + "://" + uri.Authority;

            var web = new HtmlWeb();
            Document = web.Load(url);
            PageUrl = url;
            ParseDocument();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    throw;
            //}
        }


        private void ParseDocument()
        {
            ReadImages();
            ReadNextPageUrl();
        }

        public Image GetCachedImageWithIndex(int index)
        {
            var c = 0;
            foreach (var img in Images.Where(img => img.Content != null))
            {
                if (c == index)
                {
                    return img.Content;
                }
                c++;
            }

            return null;
        }

        private void ReadImages()
        {
            var images = Document.DocumentNode.SelectNodes("//div[contains(concat(' ',@class,' '),' content ')]//img[@src]");
            if (images == null) return;
            foreach (var img in images)
            {
                var src = img.GetAttributeValue("src", null);
                if (src == null) continue;
                src = WebUtility.HtmlDecode(src);

                if (src.Contains("http://forums.nrvnqsr.com/attachment.php"))
                {
                    Images.Add(new BLImage {Url = GetFullAttachmentUrl(src)});
                }
                else if (IsValidNonRelativeUrl(src) && Images.All(b => b.Url != src))
                {
                    Images.Add(new BLImage { Url = GetFullAttachmentUrl(src) });
                }
            }
        }

        private string GetFullAttachmentUrl(string url)
        {
            return url.EndsWith("&thumb=1") ? url.Substring(0, url.Length - "&thumb=1".Length) : url;
        }

        private bool IsValidNonRelativeUrl(string url)
        {
            Uri uri;
            return (Uri.TryCreate(url, UriKind.Absolute, out uri));
        }

        private void ReadNextPageUrl()
        {
            var nextPageAnchor =
                   Document.DocumentNode.Descendants("a")
                       .FirstOrDefault(d => d.Attributes.Contains("rel") && (d.Attributes["rel"].Value == "next"));


            if (nextPageAnchor != null)
            {
                NextPageUrl = nextPageAnchor.GetAttributeValue("href", null);
                if (!IsValidNonRelativeUrl(NextPageUrl))
                {
                    NextPageUrl = _baseUrl + "/" + NextPageUrl;
                }
            }
        }

        public string GetNextPageUrl()
        {
            if (!string.IsNullOrEmpty(NextPageUrl))
            {
                return NextPageUrl;
            }
            if (Document == null) return null;
            ReadNextPageUrl();
            return NextPageUrl;
        }

        public int ReadCurrentPageNumber()
        {
            if (Document == null) return -1;
            var currPageNumberNode =
                Document.DocumentNode.SelectSingleNode(
                    "//div[contains(concat(' ',@class,' '),'pagination_top')]//span[contains(concat(' ',@class,' '),' selected ')]//a");
            if (currPageNumberNode == null) return -1;
            int currPageNumber;
            if (Int32.TryParse(currPageNumberNode.InnerText, out currPageNumber))
            {
                return currPageNumber;
            }
            return -1;
        }

        public int GetPageMax()
        {
            if (Document == null) return -1;
            var lastPageAnchor =
                Document.DocumentNode.SelectSingleNode(
                    "//span[contains(concat(' ',@class,' '),'first_last')]//a//img[contains(@alt,'Last')]");
            if (lastPageAnchor == null)
            {
                return ReadCurrentPageNumber();
            }
            var pageLink = lastPageAnchor.ParentNode.GetAttributeValue("href", null);
            if (pageLink == null) return -1;
            int pageNum;
            if (Int32.TryParse(pageLink.Split(new[] {"/page"}, StringSplitOptions.RemoveEmptyEntries).Last().Split('?')[0], out pageNum))
            {
                return pageNum;
            }
            return -1;

        }
    }
}
