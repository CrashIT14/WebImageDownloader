using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebImageDownloader
{

    public enum NodeType
    {
        Image,
        Link
    }

    interface IFilter
    {
        NodeType GetNodeType();
        string GetId();
        List<string> GetClasses(); 
        bool MatchesFilter(HtmlNode node);
    }
}
