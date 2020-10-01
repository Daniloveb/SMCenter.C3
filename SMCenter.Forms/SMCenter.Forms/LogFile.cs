using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


namespace SMCenter.Forms
{
    public class LogFile
    {
        private string _FilePath;
        private bool _boolDebug;
        //private StreamWriter SWriter;

        public LogFile(string FilePath, bool boolDebug)
        {
            try
            {
                _FilePath = FilePath;
                _boolDebug = boolDebug;
                //SWriter = File.AppendText(FilePath);
            }
           catch (Exception e)
                {
                    //Trace.WriteLine(DateTime.Now + " : " + "GetSession Error : " + e.Message);
                    MessageBox.Show(DateTime.Now + " : " + "LogFile Error : " + e.Message);
                    return;
                }
        }

        //public static void Main()
        //{
        //    using (StreamWriter w = File.AppendText("log.txt"))
        //    {
        //        Log("Test1", w);
        //        Log("Test2", w);
        //    }

        //    using (StreamReader r = File.OpenText("log.txt"))
        //    {
        //        DumpLog(r);
        //    }
        //}

        public void Write(string logMessage, bool ErrorMessage)
        {
            if (_boolDebug == true || _boolDebug != true && ErrorMessage == true)
            {
                using ( StreamWriter SWriter = new StreamWriter(_FilePath, true))
                {
                    SWriter.WriteLine(DateTime.Now + " : " + logMessage);
                    Trace.WriteLine(DateTime.Now + " : " + logMessage);
                    //WriteTextLogHandlerClass.EventHandler(DateTime.Now + " : " + logMessage);
                }
            }
        }

        //public void DumpLog(StreamReader r)
        //{
        //    string line;
        //    while ((line = r.ReadLine()) != null)
        //    {
        //        Console.WriteLine(line);
        //    }
        //}
    }

    //public static class SelectTreeViewObjectHandlerClass
    //{
    //    public delegate void EMOEvent(bool Ok, EnterpriseManagementObject _EMO, ManagementPackClass _mpClass);
    //    public static EMOEvent EventHandler;
    //}
    public static class WriteTextLogHandlerClass
    {
        public delegate void WriteLog(string logMessage);
        public static WriteLog EventHandler;
    }
}
