using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;

namespace WebImageDownloader
{
    class Model
    {
        private static Model _instance = null;
        private const string HTML_CLASS = "class";
        private const string HTML_ID = "id";

        private Model()
        {
            // Prevent creation
        }

        public static Model GetInstance()
        {
            return _instance ?? (_instance = new Model());
        }

        public async void DownloadUrl(string url, string localPath)
        {
            var uri = new Uri(url);
            var client = new WebClient();
            var webSource = await Task.Run(() => client.DownloadString(uri));
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(webSource);
            var collection = htmlDoc.DocumentNode.SelectNodes("//img");
            List<string> targets = new List<string>();
            foreach (var link in collection)
            {
                targets.Add(link.GetAttributeValue("src", ""));
            }

            foreach (var link in targets)
            {
                var formattedLink = link;
                if (link.StartsWith("//"))
                {
                    formattedLink = link.Replace("//", "http://");
                }
                var targetUri = new Uri(formattedLink);
                if (!targetUri.IsAbsoluteUri)
                {
                    targetUri = new Uri(url + "/" + link);
                }
                new WebClient().DownloadFileAsync(targetUri,
                    localPath + @"\" + targetUri.Segments[targetUri.Segments.Length - 1]);
            }
            
        }
    }
}
