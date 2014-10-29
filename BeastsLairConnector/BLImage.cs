using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BeastsLairConnector
{
    [Serializable]
    public class BLImage
    {
        public string Url { get; set; }
        public DateTime PostDate { get; set; }
        public bool Downloaded { get; set; }
        public string LocalPath { get; set; }
        [XmlIgnore]
        public Image Content { get; set; }
        [XmlIgnore]
        public Image Thumbnail { get; set; }
    }
}
