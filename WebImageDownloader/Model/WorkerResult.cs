namespace WebImageDownloader.Model
{
    public class WorkerResult
    {
        public int Downloaded { get; }
        public int Total { get; }
        public string LocalPath { get; }

        public WorkerResult(int downloaded, int totalt, string localPath)
        {
            Downloaded = downloaded;
            Total = totalt;
            LocalPath = localPath;
        }
    }
}