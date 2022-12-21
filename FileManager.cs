using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;

namespace Zalirun.Extentions
{
    public enum WriteMode
    {
        Overwrite,
        Append,
    }
    public static class FileManager
    {
        private static readonly ILogger s_logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly object s_locker = new object();

        public static void WriteText(string path, string text, WriteMode mode)
        {
            lock (s_locker)
            {
                switch (mode)
                {
                    case WriteMode.Overwrite:
                        try
                        {
                            System.IO.File.WriteAllText(path, text);
                        }
                        catch (Exception e)
                        {
                            s_logger.Error(e);
                        }
                        break;
                    case WriteMode.Append:
                        try
                        {
                            using (StreamWriter sw = System.IO.File.AppendText(path))
                            {
                                sw.WriteLine(text);
                            }
                        }
                        catch (Exception e)
                        {
                            s_logger.Error(e);
                        }
                        break;
                }
            }
        }

        public static string ReadText(string path, Action<string> onLineReaded = null)
        {
            if (!System.IO.File.Exists(path))
            {
                return "";
            }

            lock (s_locker)
            {
                try
                {
                    string data = "";
                    using (StreamReader sr = System.IO.File.OpenText(path))
                    {
                        string s = "";
                        while ((s = sr.ReadLine()) != null)
                        {
                            data += s;
                            onLineReaded?.Invoke(s);
                        }
                    }
                    return data;
                }
                catch (Exception e)
                {
                    s_logger.Error(e);
                }
                return "";
            }
        }

        public static void WriteJson<T>(string path, T data)
        {
            lock (s_locker)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    System.IO.File.WriteAllText(path, json);
                }
                catch (Exception e)
                {
                    s_logger.Error(e);
                }
            }
        }

        public static T ReadJson<T>(string path)
        {
            s_logger.Info($"Begin reading file : {path}");
            if (!System.IO.File.Exists(path))
            {
                s_logger.Info($"File not found at path: {path}");
                return default;
            }

            lock (s_locker)
            {
                using (FileStream openStream = System.IO.File.OpenRead(path))
                {
                    try
                    {
                        string data = "";
                        using (StreamReader sr = System.IO.File.OpenText(path))
                        {
                            data += sr.ReadToEnd();
                        }
                        T obj = JsonConvert.DeserializeObject<T>(data);
                        return obj;
                    }
                    catch (Exception e)
                    {
                        s_logger.Error(e);
                        return default(T);
                    }
                }
            }

        }
    }
}
