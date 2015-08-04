using System;

namespace WebImageDownloader.Model
{
    public class WorkerArgument
    {
        public WorkerArgument(string localPath, string url)
        {
            LocalPath = localPath;
            Url = url;
        }

        public string LocalPath { get; }

        public string Url { get; }
    }
}