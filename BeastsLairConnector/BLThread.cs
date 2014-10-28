using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeastsLairConnector
{
    public class BLThread
    {
        public string OpeningPostUrl { get; set; }
        public BLPage OpeningPost { get; set; }
        public string Author { get; set; }
        public string ThreadName { get; set; }
        public int PagesAmount { get; set; }
        public int ImagesAmount { get; set; }
        public int ImagesDownloaded { get; set; }
        public string DownloadLocation { get; set; }
        public DateTime LastUpdated { get; set; }

        public List<BLPage> LoadedPages { get; set; }

        public string ImagesDisp
        {
            get
            {
                return "N/A";
                return string.Format("{0}/{1}", ImagesDownloaded, ImagesAmount);
            }
        }

        public BLThread()
        {
            LoadedPages = new List<BLPage>();
        }

    }
}
