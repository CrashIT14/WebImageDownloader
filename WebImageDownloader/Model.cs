using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebImageDownloader
{
    class Model
    {
        private static Model _instance = null;

        private Model()
        {
            // Prevent creation
        }

        public static Model GetInstance()
        {
            return _instance ?? (_instance = new Model());
        }

        public void DownloadUrl(string url)
        {
            var uri = new Uri(url);
            // Get the name to use when creating directory for this page
            var name = uri.Segments[uri.Segments.Length - 1];
        }
    }
}
