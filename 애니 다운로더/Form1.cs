using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net;

namespace 애니_다운로더
{

    public partial class DownloadForm : Form
    {
        public HttpWebResponse webResponse { get; private set; }

        private const int SC_CLOSE = 0xF060;
        private const int MF_ENABLED = 0x0;
        private const int MF_GRAYED = 0x1;
        private const int MF_DISABLED = 0x2;

        private String animeURLFinal = "";
        private String animeName;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr hMenu, int wIDEnableItem, int wEnable);

        public DownloadForm(String url, String name)
        {
            InitializeComponent();
            animeURLFinal = GetFinalRedirectedUrlAsync(url);
            animeName = name;
        }


        private void downloadForm_Load(object sender, EventArgs e)
        {
            this.Text = animeName + " 다운로드 중...";
            EnableMenuItem(GetSystemMenu(this.Handle, false), SC_CLOSE, MF_GRAYED);
            
            Console.WriteLine(animeName + animeURLFinal);
        }

        private void DownloadForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            Console.WriteLine("no");
            e.Cancel = true;
        }

        private string GetFinalRedirectedUrlAsync(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AllowAutoRedirect = false;  // IMPORTANT
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/13.0.782.112 Safari/535.1";
            webRequest.Timeout = 10000;           // timeout 10s

            // Get the response ...
            using (webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                // Now look to see if it's a redirect
                if ((int)webResponse.StatusCode >= 300 && (int)webResponse.StatusCode <= 399)
                {
                    return webResponse.Headers["Location"];
                }
                return "";
            }
        }
    }
}
