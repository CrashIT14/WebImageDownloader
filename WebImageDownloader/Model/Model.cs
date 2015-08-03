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

        // HTML Agility Pack uses "//" for some reason
        private const string HtmlAgilityImg = "//img";
        private const string HtmlAgilityLink = "//a";

        private const string HtmlClass = "class";
        private const string HtmlId = "id";

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
            List<string> targets = new List<string>();
            foreach (var link in GetImgNodes(htmlDoc))
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

        private HtmlNodeCollection GetImgNodes(HtmlDocument document)
        {
            var collection = document.DocumentNode.SelectNodes(HtmlAgilityImg);

            return collection ?? (collection = new HtmlNodeCollection(null));
        }

        private HtmlNodeCollection GetLinkNodes(HtmlDocument document)
        {
            var collection = document.DocumentNode.SelectNodes(HtmlAgilityLink);

            return collection ?? (collection = new HtmlNodeCollection(null));
        }
    }
}
