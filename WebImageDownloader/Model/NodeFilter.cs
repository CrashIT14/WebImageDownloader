using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebImageDownloader.Model
{
    class NodeFilter : IFilter
    {
        public NodeType GetNodeType()
        {
            throw new NotImplementedException();
        }

        public string GetId()
        {
            throw new NotImplementedException();
        }

        public List<string> GetClasses()
        {
            throw new NotImplementedException();
        }

        public bool MatchesFilter(HtmlNode node)
        {
            throw new NotImplementedException();
        }
    }
}
