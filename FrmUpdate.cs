using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

/*
╔════════════════════════════════╗
║   © JRZ Soft Mx | MD5 Update   ║
╚════════════════════════════════╝
*/

namespace MD5Update
{
    public partial class FrmUpdate : Form
    {
        //Initialize
        #region
        public FrmUpdate(List<MD5Update.FileUpdate> lisUpdates, string strUrlUpdateFolder)
        {
            try
            {
                InitializeComponent();
                Download(lisUpdates, strUrlUpdateFolder);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        //Download
        #region
        public void Download(List<MD5Update.FileUpdate> lisUpdates, string strUrlUpdateFolder, bool bolAct = false)
        {
            try
            {
                bool bolUpdateApp = false;
                int intFiles = 0;
                long lonDownload = lisUpdates.Sum(v => v.LonSiz);
                long lonDownloaded = 0;
                lblFiles.Text = intFiles + "/" + lisUpdates.Count + " Files downloaded";
                foreach (MD5Update.FileUpdate Fil in lisUpdates)
                {
                    WebClient wcDownload = new WebClient();
                    Uri urAct = new Uri(strUrlUpdateFolder + Fil.StrFil);
                    string strFile = AppDomain.CurrentDomain.BaseDirectory + Fil.StrFil;
                    string strDirectory = Path.GetDirectoryName(strFile);
                    if (AppDomain.CurrentDomain.FriendlyName == Fil.StrFil)
                    {
                        bolUpdateApp = true;
                        strDirectory += @"\updt\";
                        strFile = strDirectory + Fil.StrFil;
                    }
                    if (!Directory.Exists(strDirectory))
                    {
                        Directory.CreateDirectory(strDirectory);
                    }
                    if (File.Exists(strFile))
                    {
                        File.Delete(strFile);
                    }
                    wcDownload.DownloadProgressChanged += new DownloadProgressChangedEventHandler(delegate (object sender, DownloadProgressChangedEventArgs e)
                    {
                        pbUpdate.Value = (((int) lonDownloaded + e.ProgressPercentage) / (int)lonDownload) * 100;
                        if(e.TotalBytesToReceive == e.BytesReceived)
                        {
                            lonDownloaded += e.TotalBytesToReceive;
                        }
                    });
                    wcDownload.DownloadFileCompleted += new AsyncCompletedEventHandler(delegate (object sender, AsyncCompletedEventArgs e)
                    {
                        if (e.Error == null && !e.Cancelled)
                        {
                            intFiles++;
                            lblFiles.Text = intFiles + "/" + lisUpdates.Count + " Files downloaded";
                            if (intFiles == lisUpdates.Count)
                            {
                                if (bolUpdateApp)
                                {
                                    DialogResult = DialogResult.OK;
                                }
                                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "updt.json"))
                                {
                                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "updt.json");
                                }
                                Close();
                            }
                        }
                    });
                    wcDownload.DownloadFileAsync(urAct, strFile);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
