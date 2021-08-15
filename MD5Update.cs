using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;

/*
╔════════════════════════════════╗
║   © JRZ Soft Mx | MD5 Update   ║
╚════════════════════════════════╝
*/

namespace MD5Update
{
    public class MD5Update
    {
        //Class File
        #region
        public class FileUpdate
        {
            public string StrFil { get; set; }
            public string StrMd5 { get; set; }
            public long LonSiz { get; set; }
        }
        #endregion

        //Check
        #region
        public static bool Check(string strUrlUpdateFolder, bool bolUpdate = false)
        {
            bool bolRes = false;
            try
            {
                if (Uri.TryCreate(strUrlUpdateFolder, UriKind.Absolute, out Uri urlUpdate) && (urlUpdate.Scheme == Uri.UriSchemeHttp || urlUpdate.Scheme == Uri.UriSchemeHttps))
                {
                    string strJson = new WebClient().DownloadString(strUrlUpdateFolder);
                    List<FileUpdate> lisFiles = JsonConvert.DeserializeObject<List<FileUpdate>>(strJson);
                    List<FileUpdate> lisUpdate = new List<FileUpdate>();
                    if (lisFiles.Count == 0)
                    {
                        throw new Exception("Not files in Update URL");
                    }
                    foreach (FileUpdate Fil in lisFiles)
                    {
                        if (Fil.StrMd5 != MD5Hash(AppDomain.CurrentDomain.BaseDirectory + Fil.StrFil))
                        {
                            lisUpdate.Add(Fil);
                        }
                    }
                    if (lisUpdate.Count > 0)
                    {
                        string strUpdate = JsonConvert.SerializeObject(lisUpdate);
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "updt.json", strUpdate);
                        if (bolUpdate && DialogResult.Yes == MessageBox.Show("Do you want to update now?", "Update Available", MessageBoxButtons.YesNo))
                        {
                            if (DialogResult.OK == new FrmUpdate(lisUpdate, strUrlUpdateFolder).ShowDialog())
                            {
                                bolRes = true;
                            }
                        }
                        else
                        {
                            bolRes = true;
                        }
                    }
                    if (!bolRes)
                    {
                        if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\updt\" + AppDomain.CurrentDomain.FriendlyName))
                        {
                            File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\updt\" + AppDomain.CurrentDomain.FriendlyName);
                        }
                    }
                }
                else
                {
                    throw new Exception("Not valid Update URL");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return bolRes;
        }
        #endregion

        //Update
        #region
        public static bool Update(string strUrlUpdateFolder)
        {
            bool bolRes = false;
            try
            {
                string strJson = string.Empty;
                if (Uri.TryCreate(strUrlUpdateFolder, UriKind.Absolute, out Uri urlUpdate) && (urlUpdate.Scheme == Uri.UriSchemeHttp || urlUpdate.Scheme == Uri.UriSchemeHttps))
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "updt.json"))
                    {
                        strJson = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "updt.json");
                        List<FileUpdate> lisFiles = JsonConvert.DeserializeObject<List<FileUpdate>>(strJson);
                        if (lisFiles.Count == 0)
                        {
                            throw new Exception("Not files in updt.json");
                        }
                        if (DialogResult.OK == new FrmUpdate(lisFiles, strUrlUpdateFolder).ShowDialog())
                        {
                            bolRes = true;
                        }
                    }
                    else
                    {
                        throw new Exception("Before check for updates");
                    }
                }
                else
                {
                    throw new Exception("Not valid Update URL");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return bolRes;
        }
        #endregion

        //MD5 Hash
        #region
        public static string MD5Hash(string strFile)
        {
            string strRes = null;
            try
            {
                if (File.Exists(strFile))
                {
                    FileStream fsArc = File.OpenRead(strFile);
                    strRes = BitConverter.ToString(MD5.Create().ComputeHash(fsArc)).Replace("-", string.Empty);
                    fsArc.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return strRes;
        }
        #endregion   
    }
}
