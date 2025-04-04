using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using ReaLTaiizor.Controls;  
namespace FileDownloader
{
    public class FileDownloadManager
    {
        private CyberProgressBar progressBar;

        public FileDownloadManager(CyberProgressBar progressBar)
        {
            this.progressBar = progressBar;
        }

        public async Task DownloadFileAsync(string url, string destinationPath)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += (s, e) =>
                {
                    progressBar.Value = e.ProgressPercentage;
                };
                await client.DownloadFileTaskAsync(new Uri(url), destinationPath);
            }
        }

        public void ExtractZipFile(string zipFilePath, string destinationFolder)
        {
            if (File.Exists(zipFilePath))
            {
                try
                {
                    ZipFile.ExtractToDirectory(zipFilePath, destinationFolder);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error extracting ZIP file: " + ex.Message);
                }
            }
        }
    }
}
