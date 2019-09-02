using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace SLCM.Controllers.Common
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 获得config中的值
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string GetAppConfig(string strKey)
        {
            ConfigurationManager.RefreshSection("appSettings");
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key == strKey)
                {
                    return ConfigurationManager.AppSettings[strKey];
                }
            }
            return null;
        }

        /// <summary>
        /// 创建错误日志
        /// </summary>
        /// <param name="path">日志地址 在当前工作目录+ErrorLog下</param>
        /// <param name="strFunctionName">问题出现的模块</param>
        /// <param name="strErrorMethod">问题出现的方法</param>
        /// <param name="strErrorDescription">错误信息</param>
        public static void CreateErrorLogTxt(string path, string strFunctionName, string strErrorMethod,
            string strErrorDescription)
        {
            string strPath; //错误文件的路径
            DateTime dt = DateTime.Now;
            try
            {
                //工程目录下 创建日志文件夹 
                strPath = Thread.GetDomain().BaseDirectory + "\\Log\\Error\\" + path;

                if (Directory.Exists(strPath) == false) //工程目录下 Log目录 '目录是否存在,为true则没有此目录
                {
                    Directory.CreateDirectory(strPath); //建立目录　Directory为目录对象
                }
                strPath = strPath + "\\" + dt.ToString("yyyyMM");

                if (Directory.Exists(strPath) == false) //目录是否存在  '工程目录下 Log\月 目录   yyyymm
                {
                    Directory.CreateDirectory(strPath); //建立目录//日志文件，以 日 命名 
                }
                strPath = strPath + "\\" + dt.ToString("dd") + ".log";
                StreamWriter fileWriter = new StreamWriter(strPath, true); //创建日志文件
                fileWriter.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>START");
                fileWriter.WriteLine("Time: " + dt.ToString("HH:mm:ss.fff"));
                fileWriter.WriteLine("问题出现的模块: " + strFunctionName);
                fileWriter.WriteLine("问题出现的方法: " + strErrorMethod);
                fileWriter.WriteLine("错误信息: " + strErrorDescription);
                fileWriter.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<END");
                fileWriter.Close(); //关闭StreamWriter对象
            }
            catch (Exception ex)
            {
                strErrorDescription += "--" + ex.Message;
            }
        }

        private static object _ErrorLogLock = new object();
        private static string ErrorLogPath = Thread.GetDomain().BaseDirectory + "\\Log\\Error\\";
        private static void Error(string folder, string title, string text)
        {
            try
            {
                var path = ErrorLogPath + folder + "\\";
                var msg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "\t" + title + "\t" + text + Environment.NewLine;

                lock (_ErrorLogLock)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    File.AppendAllText(path + DateTime.Now.ToString("yyyyMMdd") + ".log", msg);
                }
            }
            catch { }
        }
        public static void Error(string folder, string title, string text, string paras = " - ")
        {
            text += "\t" + paras;
            Error(folder, title, text);
        }


        private static object _InfoLogLock = new object();
        private static string InfoLogPath = Thread.GetDomain().BaseDirectory + "\\Log\\Info\\";
        private static void Info(string folder, string title, string text)
        {
            try
            {
                var path = InfoLogPath + folder + "\\";
                var msg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "\t" + title + "\t" + text + Environment.NewLine;

                lock (_InfoLogLock)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    File.AppendAllText(path + DateTime.Now.ToString("yyyyMMddHH") + ".log", msg);
                }
            }
            catch { }
        }
        public static void Info(string folder, string title, string text, string paras = " - ")
        {
            text += "\t" + paras;
            Info(folder, title, text);
        }

        private static object trackLogLock = new object();
        private static string trackLogPath = Thread.GetDomain().BaseDirectory + "\\Log\\Trace\\";
        public static void TraceLog(string filename, string text)
        {
            TraceLog(filename, " - ", text);
        }
        public static void TraceLog(string filename, string title, string text)
        {
            try
            {
                string msg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "\t" + title + "\t" + text + Environment.NewLine;
                lock (trackLogLock)
                {
                    if (!Directory.Exists(trackLogPath))
                    {
                        Directory.CreateDirectory(trackLogPath);
                    }
                    File.AppendAllText(trackLogPath + filename + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log", msg);
                }
            }
            catch { }
        }
    }
}