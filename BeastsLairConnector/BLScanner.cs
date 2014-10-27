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
                ImageUrls.AddRange(CurrentPage.Images.Where(img => !ImageUrls.Contains(img)));
                CurrentPage = new BLPage(CurrentPage.NextPageUrl);
            }
        }
    }
}
