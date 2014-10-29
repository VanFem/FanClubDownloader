using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BeastsLairConnector
{
    [DataContract]
    public class BLThread
    {
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
