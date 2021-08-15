MD5 Update is a library written in C# for easily allow easy add update functionality to desktops applications only need webserver with PHP for publish the update directory. 

## The NuGet Package

````powershell
PM> Install-Package MD5.Update
````

## How it works

Update all files in your application directory with (Libraries, Executables, etc). All file comparisons make with MD5 hash. No need configure FTP, Version App, only need website or public server with PHP.

Your need webserver with PHP for publish the files you want update on in update folder, in this folder your add index.php for make JSON with list of all files and directories in update folder with your md5 hash.

When your app start call the library, this get JSON file list from update folder, if any file doesn't exist o have distinct MD5 hash, will be downloaded if the file is the main app, its replaced with updt.exe.

## Using the code

### Index.php

````php
<?php
$_dat = array();
$_dir=new RecursiveDirectoryIterator(".");
foreach (new RecursiveIteratorIterator($_dir) as $_itm) {
	$_fil = str_replace(".".DIRECTORY_SEPARATOR, "", $_itm);
	if(!is_dir($_fil) && $_fil != "index.php"){		
		$_dat[]=array('StrFil' => "$_fil", 'StrMd5' => strtoupper(md5_file($_fil)), 'lonSiz' => filesize($_fil));
	}
}
echo json_encode($_dat, JSON_UNESCAPED_UNICODE);
?>
````

### Your App (HelloWorld.cs)

````csharp
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace HelloWorld
{
    public partial class HelloWorld : Form
    {
        public HelloWorld()
        {
		
            InitializeComponent();
            string strUrl = "http://yourdomain.com/app/";
            if (MD5Update.MD5Update.Check(strUrl, true))
            {
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"updt.exe", AppDomain.CurrentDomain.FriendlyName + " " + Process.GetCurrentProcess().ProcessName);
            }
	}
    }
}
````

### Updt.exe (Tool for replace yourapp).

````csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MD5Updt
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                List<string> lisArg = Environment.GetCommandLineArgs().ToList();
                if (lisArg.Count < 2)
                {
                    MessageBox.Show("Please provide App Excutable Name and Procees name");
                    Application.Exit();
                    return;
                }
                string strAppName = lisArg[1];
                string strAppProcees = lisArg[2];
                Process[] lisPro = Process.GetProcessesByName(strAppProcees);
                foreach (Process Pro in lisPro)
                {
                    if (Pro.Id != Process.GetCurrentProcess().Id)
                    {
                        Pro.Kill();
                        Thread.Sleep(1000);
                    }
                }
                string strAppMain = AppDomain.CurrentDomain.BaseDirectory + strAppName;
                string strAppUpdate = AppDomain.CurrentDomain.BaseDirectory + @"updt\" + strAppName;
                if (!File.Exists(strAppMain))
                {
                    MessageBox.Show("App Excutable dosent exists");
                    Application.Exit();
                    return;
                }
                if (!File.Exists(strAppUpdate))
                {
                    MessageBox.Show("App Excutable Updated dosent exists");
                    Application.Exit();
                    return;
                }
                File.Copy(strAppUpdate, strAppMain, true);
                long fileSize = 0;
                FileInfo currentFile = new FileInfo(strAppMain);
                while (fileSize < currentFile.Length)
                {
                    fileSize = currentFile.Length;
                    Thread.Sleep(1000);
                    currentFile.Refresh();
                }
                Process.Start(strAppMain);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("An error ocurred");
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"updt\log_" + DateTime.Now.ToString("yyyyMMddTHHmmss")  + " .txt", Ex.ToString());
                Application.Exit();
            }
        }
    }
}
