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

        private readonly NodeType type;
        private readonly string id;
        private readonly string[] classes;

        public NodeFilter(NodeType type, string id, params string[] classes)
        {
            this.type = type;
            this.id = id;
            this.classes = classes;
        }

        public NodeType GetNodeType()
        {
            return type;
        }

        public string GetId()
        {
            return id;
        }

        public List<string> GetClasses()
        {
            return new List<string>(classes);
        }

        public bool MatchesFilter(HtmlNode node)
        {
            bool typeMatches = TypeMatches(node);
            bool idMatches = IdMatches(node);
            bool classesMatches = ClassesMatches(node);

            return typeMatches && idMatches && classesMatches;
        }

        private bool TypeMatches(HtmlNode node)
        {
            var nodeName = node.Name.ToLower();
            string filtertype = "NULL";
            switch (type)
            {
                case NodeType.Image:
                    filtertype = "img";
                    break;
                case NodeType.Link:
                    filtertype = "a";
                    break;
            }
            return nodeName == filtertype;
        }

        private bool IdMatches(HtmlNode node)
        {
            if (id.Trim() == "")
            {
                return true;
            }
            string nodeId;
            if ((nodeId = node.GetAttributeValue("id", null)) != null)
            {
                return id == nodeId;
            }

            return false;
        }

        private bool ClassesMatches(HtmlNode node)
        {
            if (classes == null || classes.Length < 1)
            {
                return true;
            }

            string classString;

            if ((classString = node.GetAttributeValue("class", null)) != null)
            {
                string[] nodeClasses = classString.Split(' ');
                if (nodeClasses.Length <= 0) return false;
                bool allMatch = false;
                foreach (string filterClass in classes)
                {
                    bool localMatch = false;
                    foreach (string nodeClass in nodeClasses)
                    {
                        if (filterClass == nodeClass)
                        {
                            localMatch = true;
                        }
                    }
                    allMatch = localMatch;
                }
                return allMatch;
            }
            return false;
        }
    }
}
