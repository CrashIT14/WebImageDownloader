using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebImageDownloader
{
    class Model
    {
        private static Model instance = null;

        private Model()
        {
            // Prevent creation
        }

        public static Model GetInstance()
        {
            if (instance == null)
            {
                instance = new Model();
            }

            return instance;
        }
    }
}
