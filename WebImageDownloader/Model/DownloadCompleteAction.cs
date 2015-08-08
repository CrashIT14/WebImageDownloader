using System.Diagnostics;
using System.IO;
using Se.Creotec.WPFToastMessage;

namespace WebImageDownloader.Model
{
    public class DownloadCompleteAction : ToastMessage.IToastAction
    {
        private readonly string _localPath;

        public DownloadCompleteAction(string localPath)
        {
            _localPath = localPath;
        }

        public void DoToastAction()
        {
            if (Directory.Exists(_localPath))
            {
                Process.Start(_localPath);
            }
        }
    }
}