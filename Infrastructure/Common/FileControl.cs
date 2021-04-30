using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Infrastructure.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class FileControl
    {
        //backup\20201222\
        private static string logpath = AppSetting.GetConfig("FileConfig:SaveDir");

        private static string KLineDirPath = AppSetting.GetConfig("FileConfig:KLineDir");
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static void BytesToFile(string type, int channelID, byte[] buffer)
        {
            try
            {
                string fileName = @$"{logpath}{channelID}\" + $"{DateTime.Now.ToString("yyyy-MM-dd HH_mm")}-{type}.bin";
                if (!File.Exists(fileName))
                {
                    File.WriteAllBytes(fileName, buffer);
                    return;
                }
                using (System.IO.FileStream file = new System.IO.FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    file.Seek(0, SeekOrigin.End);
                    file.Write(buffer, 0, buffer.Length);
                    file.Dispose();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IO exception has been thrown!");
                Console.WriteLine(ex.ToString());
                return;
            }
        }
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static void BytesToFile2(string path, string type, byte[] buffer)
        {
            try
            {
                //以美国东部时间来存储
                string fileName = path + $"{Utils.BeijingTimeToUsDateTime(DateTime.Now).ToString("yyyy_MM_dd_HH")}_{type}.bin";
                if (!File.Exists(fileName))
                {
                    File.WriteAllBytes(fileName, buffer);
                    return;
                }
                using (System.IO.FileStream file = new System.IO.FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    file.Seek(0, SeekOrigin.End);
                    file.Write(buffer, 0, buffer.Length);
                    file.Dispose();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IO exception has been thrown!");
                Console.WriteLine(ex.ToString());
                return;
            }
        }
        /// <summary>
        /// 盘中归档文件保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static void BytesToFile3(string path, DateTime dt, byte[] buffer)
        {
            try
            {
                //以美国东部时间来存储
                string fileName = path + $"{dt.ToString("yyyy_MM_dd_HH_mm")}.bin";
                if (!File.Exists(fileName))
                {
                    File.WriteAllBytes(fileName, buffer);
                    return;
                }
                using (System.IO.FileStream file = new System.IO.FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    file.Seek(0, SeekOrigin.End);
                    file.Write(buffer, 0, buffer.Length);
                    file.Dispose();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IO exception has been thrown!");
                Console.WriteLine(ex.ToString());
                return;
            }
        }
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static void KLineToFile(string path, string filename, byte[] buffer)
        {
            try
            {
                string fileName = path + $"{filename}.bin";
                if (!File.Exists(fileName))
                {
                    File.WriteAllBytes(fileName, buffer);
                    return;
                }
                using (System.IO.FileStream file = new System.IO.FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    file.Seek(0, SeekOrigin.End);
                    file.Write(buffer, 0, buffer.Length);
                    file.Dispose();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IO exception has been thrown!");
                Console.WriteLine(ex.ToString());
                return;
            }
        }
        /// <summary>
        /// 从文件中读取处理好的结构数据
        /// </summary>
        /// <param name="dir"></param>
        public static byte[] ReadFilesDelay(string dir, string key)
        {
            string filePath = $"{dir}{key}.bin";
            if (File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            return new byte[0];
        }
        /// <summary>
        /// 删除延迟行情数据
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteFiles(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    File.Delete(i.FullName);      //删除指定文件
                }
            }
        }
        /// <summary>
        /// 覆盖写入，删除文件重写
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="buffer"></param>
        public static void KLineToFile_ReWrite(string path, string filename, byte[] buffer)
        {
            try
            {
                string fileName = path + $"{filename}.bin";
                File.Delete(fileName);
                if (!File.Exists(fileName))
                {
                    File.WriteAllBytes(fileName, buffer);
                    return;
                }
                //using (System.IO.FileStream file = new System.IO.FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                //{
                //    file.Seek(0, SeekOrigin.Begin);
                //    file.Write(buffer, 0, buffer.Length);
                //    file.Dispose();
                //}
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IO exception has been thrown!");
                Console.WriteLine(ex.ToString());
                return;
            }
        }
        /// <summary>
        /// 文件读取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ReadFileToBytes(string filename)
        {
            try
            {
                //  string path = logpath + $"{filename}.bin";
                string path = filename;
                if (!File.Exists(path))
                {
                    return null;
                }
                byte[] buffer = File.ReadAllBytes(path);
                return buffer;
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IO exception has been thrown!");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 文件读取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ReadFileToBytes(string path, byte[] buffer, long offset, long length)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return false;
                }
                using (System.IO.FileStream file = new System.IO.FileStream(path, FileMode.Open, FileAccess.ReadWrite))
                {
                    file.Seek(offset, SeekOrigin.Begin);
                    file.Read(buffer, 0, buffer.Length);
                    file.Dispose();
                    return true;
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IO exception has been thrown!");
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        /// <summary>
        /// 从文件中读取处理好的结构数据
        /// </summary>
        /// <param name="dir"></param>
        public static bool ReadAllFiles(string dir, Action<string, byte[]> func)
        {
            string path = $"{logpath}{dir}";
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                for (int i = 0; i < files.Length; i++)
                {
                    byte[] lines = File.ReadAllBytes($"{files[i]}");
                    func(files[i].Substring(files[i].Length - 25, 25), lines);
                }
            }
            return true;
        }
        /// <summary>
        /// 从文件中读取处理好的结构数据
        /// </summary>
        /// <param name="dir"></param>
        public static bool ReadAllFilesBytes(string dir, Action<string, byte[]> func)
        {
            string path = $"{logpath}{dir}";
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                for (int i = 0; i < files.Length; i++)
                {
                    long length = 64 * 1024;
                    long offset = 0;
                    bool isc = true;
                    byte[] buffer = new byte[length];
                    using (System.IO.FileStream file = new System.IO.FileStream(files[i], FileMode.Open, FileAccess.Read))
                    {
                        do
                        {
                            if (offset + length + 1 <= file.Length)
                            {

                                file.Seek(offset, SeekOrigin.Begin);
                                file.Read(buffer, 0, buffer.Length);
                                offset += length;
                            }
                            else
                            {
                                length = file.Length - offset;
                                buffer = new byte[length];
                                file.Seek(-length, SeekOrigin.End);
                                file.Read(buffer, 0, buffer.Length);
                                offset = 0;
                                isc = false;
                            }
                            func(files[i].Substring(files[i].Length - 25, 25), buffer);

                        } while (isc);
                        file.Dispose();
                    }

                }
            }
            return true;
        }
        /// <summary>
        /// 获取文件名称
        /// </summary>
        /// <returns></returns>
        public static List<string> ReadFileFullName(int channelID)
        {
            var list = new List<string>();
            DirectoryInfo folder = new DirectoryInfo(logpath + channelID);

            foreach (FileInfo file in folder.GetFiles("*.bin"))
            {
                list.Add(file.FullName);
            }
            return list;
        }

        /// <summary>
        /// 从文件中读取处理好的结构数据
        /// </summary>
        /// <param name="dir"></param>
        public static bool ReadAllFilesRecover(string dir, string key, Action<string, byte[]> func)
        {
            string path = $"{dir}";
            if (Directory.Exists(path))
            {
                DirectoryInfo folder = new DirectoryInfo(path);//目录信息
                FileInfo[] fs = folder.GetFiles($"*_{key}.bin");
                FileInfo[] files = fs.OrderBy(s => s.CreationTime).ToArray();
                for (int i = 0; i < files.Length; i++)
                {
                    byte[] lines = File.ReadAllBytes($"{files[i].FullName}");
                    func(files[i].Name, lines);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 从文件中读取处理好的结构数据
        /// </summary>
        /// <param name="dir"></param>
        public static bool ReadAllFilesRecoverV2(string dir, string key, Action<string, byte[]> func)
        {
            string path = $"{dir}";
            if (Directory.Exists(path))
            {
                DirectoryInfo folder = new DirectoryInfo(path);//目录信息
                FileInfo[] fs = folder.GetFiles($"*_{key}.bin");
                // FileInfo[] files = fs.OrderBy(s => s.CreationTime).ToArray();
                FileInfo[] files = fs.OrderBy(s => Convert.ToInt32(s.Name.Substring(11, 2))).ToArray();
                for (int i = 0; i < files.Length; i++)
                {
                    byte[] lines = File.ReadAllBytes($"{files[i].FullName}");
                    func(files[i].Name, lines);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 文件读取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="byteSize"></param>
        /// <param name="skip"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static List<T> ReadFileToBytes<T>(string filename, int byteSize, int skip, int length)
        {
            try
            {
                string fname = KLineDirPath + filename;
                if (!File.Exists(fname))
                {
                    return null;
                }
                byte[] buffer = new byte[byteSize * length];
                using (System.IO.FileStream file = new System.IO.FileStream(fname, FileMode.Open, FileAccess.Read))
                {
                    //        文件中不存在需要数据                文件包含当前页的所有数据                                        文件包含当前页的部分数据
                    //long n=file.Length <= skip * byteSize ? 0 : file.Length > byteSize * (skip + length) ? 0 - byteSize * length : skip * byteSize - file.Length;
                    long n = file.Length > byteSize * (skip + length) ? 0 - byteSize * (skip + length) : 0 - file.Length;
                    file.Seek(n, SeekOrigin.End);
                    long len = 0 - n;
                    buffer = new byte[len];
                    file.Read(buffer, 0, buffer.Length);
                    file.Dispose();
                }
                List<T> list = new List<T>();
                bool flag = true;
                int offset = 0;
                byte[] item = new byte[byteSize];
                while (flag)
                {
                    Buffer.BlockCopy(buffer, offset, item, 0, byteSize);
                    T i = (T)StructHelper.BytesToStuct(item, typeof(T));
                    list.Add(i);
                    offset += byteSize;
                    if (offset >= buffer.Length)
                    {
                        flag = false;
                    }
                }
                return list;
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IO exception has been thrown!");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 文件读取-只返回分页数据，不返回全部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="byteSize"></param>
        /// <param name="skip"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static List<T> ReadFileToBytes_ForPage<T>(string filename, int byteSize, int skip, int length)
        {
            try
            {
                string fname = KLineDirPath + filename;
                if (!File.Exists(fname))
                {
                    return new List<T>();
                }
                byte[] buffer = new byte[byteSize * length];
                long start = 0;//开始截取数据的位置
                long n = 0;//截取数据的字节数
                using (System.IO.FileStream file = new System.IO.FileStream(fname, FileMode.Open, FileAccess.Read))
                {
                    long a = file.Length - (skip + length) * byteSize;
                    long b = file.Length - skip * byteSize;
                    if (a >= 0)
                    {
                        //包含全部数据
                        start = 0 - (skip + length) * byteSize;
                        n = length * byteSize;
                    }
                    else if (a < 0 && b >= 0)
                    {
                        //包含要获取的部分数据
                        start = 0 - file.Length;
                        n = file.Length - skip * byteSize;
                    }
                    else
                    {
                        //不包含任何需要的数据
                        n = 0;
                    }
                    file.Seek(start, SeekOrigin.End);
                    buffer = new byte[n];
                    file.Read(buffer, 0, buffer.Length);
                    file.Dispose();
                }
                List<T> list = new List<T>();
                if (buffer.Length == 0)
                {
                    return list;
                }
                bool flag = true;
                int offset = 0;
                byte[] item = new byte[byteSize];
                while (flag)
                {
                    Buffer.BlockCopy(buffer, offset, item, 0, byteSize);
                    T i = (T)StructHelper.BytesToStuct(item, typeof(T));
                    list.Add(i);
                    offset += byteSize;
                    if (offset >= buffer.Length)
                    {
                        flag = false;
                    }
                }
                return list;
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IO exception has been thrown!");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
