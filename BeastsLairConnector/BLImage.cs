using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace BeastsLairConnector
{
    [DataContract]
    public class BLImage
    {
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public DateTime PostDate { get; set; }
        [DataMember]
        public bool Downloaded { get; set; }
        [DataMember]
        public string LocalPath { get; set; }
        [DataMember]
        public int PageNumber { get; set; }
        [DataMember]
        public bool ErrorDownloading { get; set; }

        [IgnoreDataMember]
        public Image Content { get; set; }
        [IgnoreDataMember]
        public Image Thumbnail { get; set; }

        public string ShortFileName
        {
            get { return FileName.Length > 60 ? FileName.Substring(0, 60) + "..." : FileName; }
        }
        
        public string DateString
        {
            get { return PostDate.Year < 2 ? "N/A" : PostDate.ToString("dd MMM yyyy a\\t hh:mm tt", CultureInfo.InvariantCulture); }
        }

        public string FileName
        {
            get { return string.IsNullOrEmpty(Url) ? string.Empty : Regex.Replace(Url.Split('/').Last().Split('?')[0], @"\%[\da-f]{2}", ""); }
        }
    }
}
