using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
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
        [DataMember]
        public int CurrentPageNumber { get; set; }

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
            ReadCurrentPageNumber();
            ReadImages();
            ReadNextPageUrl();}

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
                var dt = ReadImageDate(img);
                if (src.Contains("http://forums.nrvnqsr.com/attachment.php"))
                {
                    Images.Add(new BLImage {Url = GetFullAttachmentUrl(src), PageNumber = CurrentPageNumber, PostDate = dt});
                }
                else if (IsValidNonRelativeUrl(src) && Images.All(b => b.Url != src))
                {
                    Images.Add(new BLImage { Url = GetProperUrl(src), PageNumber = CurrentPageNumber, PostDate = dt});
                }
            }
        }

        private string GetProperUrl(string url)
        {
            return url.EndsWith("/") ? url.Substring(0, url.Length - 1) : url;
        }

        private DateTime ReadImageDate(HtmlNode imageNode)
        {
            var dt = new DateTime();
            HtmlNode parentNode;
            parentNode = imageNode.ParentNode;
            while (parentNode != null && !(parentNode.GetAttributeValue("class", "").Contains("postdetails")))
            {
                parentNode = parentNode.ParentNode;
            }
            if (parentNode == null) return dt;

            HtmlNode prevChild = parentNode.PreviousSibling;
            while (prevChild != null && !(prevChild.GetAttributeValue("class", "").Contains("posthead")))
            {
                prevChild = prevChild.PreviousSibling;
            }
            if (prevChild == null) return dt;

            var dateNode = prevChild.SelectSingleNode(".//span[contains(concat(' ',@class,' '),' date ')]");
            if (dateNode == null) return dt;
            var pattern = @"((?<=\d)(st|nd|rd|th)|\s)";
            string dateText = Regex.Replace(WebUtility.HtmlDecode(dateNode.InnerText),
                pattern, "");
            if (dateText.StartsWith("Today"))
            {
                dateText = dateText.Substring(5);
                dt = DateTime.ParseExact(dateText, ",hh:mmtt", CultureInfo.InvariantCulture);
                dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, dt.Hour, dt.Minute, 0);
            }
            else if (dateText.StartsWith("Yesterday"))
            {
                dateText = dateText.Substring(9);
                dt = DateTime.ParseExact(dateText, ",hh:mmtt", CultureInfo.InvariantCulture);
                var yt = DateTime.Now.AddDays(-1);
                dt = new DateTime(yt.Year, yt.Month, yt.Day, dt.Hour, dt.Minute, 0);
            } else {
                dt = DateTime.ParseExact(dateText, "MMMMd,yyyy,hh:mmtt", CultureInfo.InvariantCulture);
            }
            return dt;
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

        private void ReadCurrentPageNumber()
        {          
            var currPageNumberNode =
                Document.DocumentNode.SelectSingleNode(
                    "//div[contains(concat(' ',@class,' '),'pagination_top')]//span[contains(concat(' ',@class,' '),' selected ')]//a");
            if (currPageNumberNode == null)
            {
                CurrentPageNumber = -1;
                return;
            }
            int currPageNumber;
            if (Int32.TryParse(currPageNumberNode.InnerText, out currPageNumber))
            {
                CurrentPageNumber = currPageNumber;
            }
            else
            {
                CurrentPageNumber = -1;
            }
        }

        public int GetPageMax()
        {
            if (Document == null)
            {
                if (string.IsNullOrEmpty(PageUrl))
                {
                    return -1;
                }
                Load();
            }
            var lastPageAnchor =
                Document.DocumentNode.SelectSingleNode(
                    "//span[contains(concat(' ',@class,' '),'first_last')]//a//img[contains(@alt,'Last')]");
            if (lastPageAnchor == null)
            {
                return CurrentPageNumber;
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
