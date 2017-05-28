using CraigslistMarketTracker_Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CraigslistMarketTracker_Service.WebpageScraper
{
    public class DataScraper
    {
        private int _maxRecords = 2500;
        private int _pageSize = 120;
        private GlobalDataLoader _globalDataLoader { get; set; }
        private string _city { get; set; }
        private string _url { get; set; }

        private string City
        {
            get
            {
                return _city;
            }
            set
            {
                _city = value;
                _url = string.Format("{0}{1}", _globalDataLoader.globalUrl.Replace("https://", "https://" + value + "."), "search/");
            }
        }
        
        public DataScraper(GlobalDataLoader globalDataLoader, string city)
        {
            _globalDataLoader = globalDataLoader;
            City = city;
        }

        public void RetrieveNewData()
        {
            foreach (var cat in _globalDataLoader.forSaleCategoryList)
            {
                var categoryResultsList = new List<CraigslistSaleItemHeader>();

                for (int recordIndex = 0; recordIndex < (_maxRecords + _pageSize); recordIndex = recordIndex + _pageSize)
                {
                    var recordStart = (recordIndex < _maxRecords ? recordIndex : (_maxRecords - _pageSize));
                    var pageResults = ScrapeSaleItemHeaderByCategory(cat.CatId, recordStart, cat.LastScrapeDate);

                    categoryResultsList.AddRange(pageResults);

                    if (pageResults.Count() < _pageSize)
                        break;
                }

                Console.WriteLine(categoryResultsList.Count);

                var dataResponse = new DataAccess().BulkInsertHeaderRecord(categoryResultsList);

                if (categoryResultsList.Count > 0)
                {
                    var latestImportRecordDate = categoryResultsList.OrderByDescending(t => t.CreateDate).First().CreateDate;
                    cat.LastScrapeDate = latestImportRecordDate;
                }
            }
        }

        private List<CraigslistSaleItemHeader> ScrapeSaleItemHeaderByCategory(string catId, int recordStart, DateTime lastScrapDate)
        {
            var doc = new WebRequest().UrlRequest(string.Format("{0}{1}?s={2}", _url, catId, recordStart));
            var resultsArray = new List<CraigslistSaleItemHeader>();

            var results = doc.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("id") && d.Attributes["id"].Value.Contains("sortable-results")).FirstOrDefault();
            var ul = results.Descendants("ul").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("rows")).FirstOrDefault();
            var li = ul.Descendants("li").ToList();

            foreach (var item in li)
            {
                var resultInfo = item.Descendants("p").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("result-info")).FirstOrDefault();
                var resultMeta = resultInfo.Descendants("span").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("result-meta")).FirstOrDefault();
                var resultDateNode = resultInfo.Descendants("time").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("result-date")).FirstOrDefault();
                var resultLocationNode = resultMeta.Descendants("span").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("result-hood")).FirstOrDefault();

                var resultPostingId = item.Attributes["data-pid"].Value;
                var resultRePostId = item.Attributes["data-repost-of"] == null ? null : item.Attributes["data-repost-of"].Value;
                var resultTitle = resultInfo.Descendants("a").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("result-title")).FirstOrDefault().InnerText;
                var resultCreateDate = DateTime.Parse(resultDateNode.Attributes["datetime"].Value);
                var resultLocation = resultLocationNode == null ? null : resultLocationNode.InnerText.Replace("(", string.Empty).Replace(")", "");

                if (resultCreateDate > lastScrapDate)
                {
                    resultsArray.Add(new CraigslistSaleItemHeader()
                    {
                        PostingId = resultPostingId,
                        RePostId = resultRePostId,
                        Title = resultTitle,
                        CreateDate = resultCreateDate,
                        LocationTag = resultLocation
                    });
                }
            }

            return resultsArray;
        }
    }
}