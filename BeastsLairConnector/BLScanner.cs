using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeastsLairConnector
{
    public class BLScanner
    {
        public List<string> ImageUrls = new List<string>(); 

        public BLPage CurrentPage { get; set; }

        public void Scan(string url) 
        {
            CurrentPage = new BLPage(url);
            while (CurrentPage.NextPageUrl != null)
            {
                ImageUrls.AddRange(CurrentPage.Images.Where(img => ImageUrls.All(im => im != img.Url)).Select(im => im.Url));
                CurrentPage = new BLPage(CurrentPage.NextPageUrl);
            }
        }
    }
}
