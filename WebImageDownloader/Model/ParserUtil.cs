using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebImageDownloader.Model
{
    class ParserUtil
    {
        // xpath codes
        public const string XPathImg = "//img";
        public const string XPathLink = "//a";

        public const string HtmlClass = "class";
        public const string HtmlId = "id";

        public static HtmlNodeCollection GetImgNodes(HtmlDocument document)
        {
            var collection = document.DocumentNode.SelectNodes(XPathImg);

            return collection ?? (collection = new HtmlNodeCollection(null));
        }

        public static HtmlNodeCollection GetLinkNodes(HtmlDocument document)
        {
            var collection = document.DocumentNode.SelectNodes(XPathLink);

            return collection ?? (collection = new HtmlNodeCollection(null));
        }

        public static List<HtmlNode> FilterNodes(HtmlNodeCollection nodes, params IFilter[] filters)
        {
            var targets = new List<HtmlNode>();

            if (filters == null)
            {
                return targets;
            }

            foreach (var node in nodes)
            {
                targets.AddRange(from filter in filters where filter.MatchesFilter(node) select node);
            }

            return targets;
        }

        public static List<string> GetTargetsFromFilters(HtmlDocument document, params IFilter[] filters)
        {
            var targets = new List<string>();
            if (document == null || filters == null || filters.Length < 1)
            {
                return targets;
            }

            foreach (var node in document.DocumentNode.Descendants())
            {
                targets.AddRange(from filter in filters where filter.MatchesFilter(node)
                                 select GetTargetFromFilter(node, filter));
            }

            return targets;
        }

        public static string GetTargetFromFilter(HtmlNode node, IFilter filter)
        {
            switch (filter.GetNodeType())
            {
                case NodeType.Image:
                    return node.GetAttributeValue("src", "");
                case NodeType.Link:
                    return node.GetAttributeValue("href", "");
                default:
                    return "";
            }
        }

        public static Uri GetUriFromTargetString(string target, string baseUrl)
        {
            var formattedLink = target;
            if (target.StartsWith("//"))
            {
                formattedLink = target.Replace("//", "http://");
            }
            var targetUri = new Uri(formattedLink);
            if (!targetUri.IsAbsoluteUri)
            {
                targetUri = new Uri(baseUrl + "/" + target);
            }
            return targetUri;
        }

        public static string GetFileNameFromUri(Uri uri)
        {
            string result;
            if (uri.Segments.Length > 0)
            {
                result = uri.Segments[uri.Segments.Length - 1];
                if (result == @"/")
                {
                    result = uri.Host;
                }
            }
            else
            {
                result = uri.Host;
            }
            return result;
        }
    }
}
