﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace BeastsLairConnector
{
    public class BLPage
    {
        public HtmlDocument Document { get; private set; }
        public List<string> Images { get; private set; }
        public string NextPageUrl;

        private string _baseUrl;

        public BLPage()
        {
            Images = new List<string>();
        }

        public BLPage(string url)
            : this()
        {
            Load(url);
        }

        public void Load(string url)
        {
            //try
            //{

            var uri = new Uri(url);

            _baseUrl = uri.Scheme + "://" + uri.Authority;

            var web = new HtmlWeb();
            Document = web.Load(url);
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

        private void ReadImages()
        {
            var images = Document.DocumentNode.SelectNodes("//div[contains(concat(' ',@class,' '),' content ')]//img[@src]");

            Images.AddRange(images.Select(img => img.GetAttributeValue("src", null))
                .Where(src => IsValidNonRelativeUrl(src) && !Images.Contains(src)).ToList());
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
    }
}
