using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BeastsLairConnector
{
    [DataContract]
    public class BLThread
    {
        private const string pageByUrlFormat = "{0}/{1}/page{2}";

        [DataMember]
        public string OpeningPostUrl { get; set; }
        [DataMember]
        public BLPage OpeningPost { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public string ThreadName { get; set; }
        [DataMember]
        public int PagesAmount { get; set; }
        [DataMember]
        public int ImagesAmount { get; set; }
        [DataMember]
        public int ImagesDownloaded { get; set; }
        [DataMember]
        public string DownloadLocation { get; set; }
        [DataMember]
        public DateTime LastUpdated { get; set; }
        [DataMember]
        public List<BLPage> LoadedPages { get; set; }
        
        [DataMember]
        public string SyncPath { get; set; }
        [DataMember]
        public int SyncFromPage { get; set; }


        public string GetDateString
        {
            get
            {
                return LastUpdated.Year == 1
                    ? "Never"
                    : LastUpdated.ToString("dd MMM yyyy hh:mm:ss", new CultureInfo("en-US"));
            }
        }

        public string ImagesDisp
        {
            get
            {
                return ImagesAmount > 0 ? string.Format("{0}/{1}", ImagesDownloaded, ImagesAmount) : "N/A";
            }
        }

        public BLPage GetBLPageByPageNumber(int pageNumber)
        {
            if (LoadedPages.Any(lp => lp.CurrentPageNumber == pageNumber))
            {
                return LoadedPages.First(lp => lp.CurrentPageNumber == pageNumber);
            }
            string pageUrl = OpeningPostUrl;
            if (OpeningPostUrl.Contains('/'))
            {
                pageUrl = OpeningPostUrl.Substring(0, OpeningPostUrl.LastIndexOf('/'));
            }
            var pageUrlIdentity = OpeningPostUrl.Split('/').Last().Split('?')[0];
            var bPage = new BLPage(string.Format(pageByUrlFormat, pageUrl, pageUrlIdentity, pageNumber));
            LoadedPages.Add(RemoveQuotesAndRepeats(bPage));
            return bPage;
        }

        public void ReplaceOrAddPage(BLPage page)
        {
            if (page.CurrentPageNumber != -1)
            {
                LoadedPages.RemoveAll(lp => lp.CurrentPageNumber == page.CurrentPageNumber);
                LoadedPages.Add(RemoveQuotesAndRepeats(page));
            }
            Debug.WriteLine("Invalid page added");
        }

        public BLPage RemoveQuotesAndRepeats(BLPage newPage)
        {
            newPage.Images.RemoveAll(im => LoadedPages.Any(lp => lp.Images.Any(lpi => lpi.Url == im.Url)));
            return newPage;
        }

        public BLThread()
        {
            LoadedPages = new List<BLPage>();
        }
    }
}
