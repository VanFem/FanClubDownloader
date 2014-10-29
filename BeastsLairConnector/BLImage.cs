using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        [IgnoreDataMember]
        public Image Content { get; set; }
        [IgnoreDataMember]
        public Image Thumbnail { get; set; }
    }
}
