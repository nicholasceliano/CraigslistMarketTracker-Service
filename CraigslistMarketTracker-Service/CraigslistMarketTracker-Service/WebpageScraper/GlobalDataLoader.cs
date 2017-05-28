using CraigslistMarketTracker_Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CraigslistMarketTracker_Service.WebpageScraper
{
    public class GlobalDataLoader
    {
        public List<CraigslistCategory> forSaleCategoryList = new List<CraigslistCategory>();
        public string globalUrl = "https://craigslist.com/";

        public GlobalDataLoader()
        {
            LoadForSaleCategoryList();
        }

        private void LoadForSaleCategoryList()
        {
            var doc = new WebRequest().UrlRequest(globalUrl);

            var forSaleSection = doc.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("id") && d.Attributes["id"].Value.Contains("sss")).FirstOrDefault();
            var forSaleCats = forSaleSection.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("cats")).FirstOrDefault();
            var forSaleCatsListItems = forSaleCats.Descendants("a").ToList();

            foreach (var li in forSaleCatsListItems)
            {
                var catIdArray = li.Attributes["class"].Value.Split(' ').ToArray();
                var catName = li.Descendants("span").FirstOrDefault().InnerText;

                foreach (var catId in catIdArray)
                {
                    forSaleCategoryList.Add(new CraigslistCategory()
                    {
                        CatId = catId,
                        Name = catName,
                        LastScrapeDate = DateTime.Parse("5/28/2017 14:00:00")
                    });
                }
            }
        }

    }
}
