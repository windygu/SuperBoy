using System;
using System.IO;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;


namespace Core.Zip
{
    /// <summary>
    /// 用ICSharpCode.SharpZipLib.dll进行压缩解压
    /// </summary>
    public class SharpZip
    {
        public SharpZip()
        { }

        /// <summary>
        /// 压缩
        /// </summary> 
        /// <param name="filename"> 压缩后的文件名(包含物理路径)</param>
        /// <param name="directory">待压缩的文件夹(包含物理路径)</param>
        public static void PackFiles(string filename, string directory)
        {
            try
            {
                var fz = new FastZip();
                fz.CreateEmptyDirectories = true;
                fz.CreateZip(filename, directory, true,"");
                fz = null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="file">待解压文件名(包含物理路径)</param>
        /// <param name="dir"> 解压到哪个目录中(包含物理路径)</param>
        public static bool UnpackFiles(string file, string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var s = new ZipInputStream(File.OpenRead(file));
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    var directoryName = Path.GetDirectoryName(theEntry.Name);
                    var fileName = Path.GetFileName(theEntry.Name);
                    if (directoryName != String.Empty)
                    {
                        Directory.CreateDirectory(dir + directoryName);
                    }
                    if (fileName != String.Empty)
                    {
                        var streamWriter = File.Create(dir + theEntry.Name);
                        var size = 2048;
                        var data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                }
                s.Close();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }


    public class ClassZip
    {
        #region 私有方法
        /// <summary>
        /// 递归压缩文件夹方法
        /// </summary>
        private static bool ZipFileDictory(string folderToZip, ZipOutputStream s, string parentFolderName)
        {
            var res = true;
            string[] folders, filenames;
            ZipEntry entry = null;
            FileStream fs = null;
            var crc = new Crc32();
            try
            {
                entry = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/"));
                s.PutNextEntry(entry);
                s.Flush();
                filenames = Directory.GetFiles(folderToZip);
                foreach (var file in filenames)
                {
                    fs = File.OpenRead(file);
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    entry = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/" + Path.GetFileName(file)));
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                if (entry != null)
                {
                    entry = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            folders = Directory.GetDirectories(folderToZip);
            foreach (var folder in folders)
            {
                if (!ZipFileDictory(folder, s, Path.Combine(parentFolderName, Path.GetFileName(folderToZip))))
                {
                    return false;
                }
            }
            return res;
        }

        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="folderToZip">待压缩的文件夹，全路径格式</param>
        /// <param name="zipedFile">压缩后的文件名，全路径格式</param>
        private static bool ZipFileDictory(string folderToZip, string zipedFile, int level)
        {
            bool res;
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }
            var s = new ZipOutputStream(File.Create(zipedFile));
            s.SetLevel(level);
            res = ZipFileDictory(folderToZip, s, "");
            s.Finish();
            s.Close();
            return res;
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileToZip">要进行压缩的文件名</param>
        /// <param name="zipedFile">压缩后生成的压缩文件名</param>
        private static bool ZipFile(string fileToZip, string zipedFile, int level)
        {
            if (!File.Exists(fileToZip))
            {
                throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
            }
            FileStream zipFile = null;
            ZipOutputStream zipStream = null;
            ZipEntry zipEntry = null;
            var res = true;
            try
            {
                zipFile = File.OpenRead(fileToZip);
                var buffer = new byte[zipFile.Length];
                zipFile.Read(buffer, 0, buffer.Length);
                zipFile.Close();

                zipFile = File.Create(zipedFile);
                zipStream = new ZipOutputStream(zipFile);
                zipEntry = new ZipEntry(Path.GetFileName(fileToZip));
                zipStream.PutNextEntry(zipEntry);
                zipStream.SetLevel(level);

                zipStream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (zipEntry != null)
                {
                    zipEntry = null;
                }
                if (zipStream != null)
                {
                    zipStream.Finish();
                    zipStream.Close();
                }
                if (zipFile != null)
                {
                    zipFile.Close();
                    zipFile = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return res;
        }
        #endregion

        #region 公有方法
       
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="fileToZip">待压缩的文件目录</param>
        /// <param name="zipedFile">生成的目标文件</param>
        /// <param name="level">压缩率0（无压缩）-9（压缩率最高）</param>
        public static bool Zip(String fileToZip, String zipedFile, int level)
        {
            if (Directory.Exists(fileToZip))
            {
                return ZipFileDictory(fileToZip, zipedFile, level);
            }
            else if (File.Exists(fileToZip))
            {
                return ZipFile(fileToZip, zipedFile, level);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="fileToUpZip">待解压的文件</param>
        /// <param name="zipedFolder">解压目标存放目录</param>
        public static void UnZip(string fileToUpZip, string zipedFolder)
        {
            if (!File.Exists(fileToUpZip))
            {
                return;
            }
            if (!Directory.Exists(zipedFolder))
            {
                Directory.CreateDirectory(zipedFolder);
            }
            ZipInputStream s = null;
            ZipEntry theEntry = null;
            string fileName;
            FileStream streamWriter = null;
            try
            {
                s = new ZipInputStream(File.OpenRead(fileToUpZip));
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.Name != String.Empty)
                    {
                        fileName = Path.Combine(zipedFolder, theEntry.Name);
                        if (fileName.EndsWith("/") || fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }
                        streamWriter = File.Create(fileName);
                        var size = 2048;
                        var data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter = null;
                }
                if (theEntry != null)
                {
                    theEntry = null;
                }
                if (s != null)
                {
                    s.Close();
                    s = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
        }
        #endregion
    }
  
}