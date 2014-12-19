using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Core.Systems
{
    /// <summary>
    /// MP3文件播放操作辅助类
    /// </summary>
    public class Mp3Helper
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, System.Text.StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);
        
        public static void Play(string mp3FileName,bool repeat)
        {
            mciSendString("open \"" + mp3FileName + "\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero);
            mciSendString("play MediaFile" + (repeat ? " repeat" :String.Empty), null, 0, IntPtr.Zero);
        }
        public static void Play(byte[] mp3EmbeddedResource, bool repeat)
        {
            ExtractResource(mp3EmbeddedResource, Path.GetTempPath() + "resource.tmp");
            mciSendString("open \"" + Path.GetTempPath() + "resource.tmp" + "\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero);
            mciSendString("play MediaFile" + (repeat ? " repeat" : String.Empty), null, 0, IntPtr.Zero);
        }

        public static void Pause()
        {
            mciSendString("stop MediaFile", null, 0, IntPtr.Zero);
        }

        public static void Stop()
        {
            mciSendString("close MediaFile", null, 0, IntPtr.Zero);
        }

        private static void ExtractResource(byte[] res,string filePath)
        {
            FileStream fs;
            BinaryWriter bw;

            if (!File.Exists(filePath))
            {
                fs = new FileStream(filePath, FileMode.OpenOrCreate);
                bw = new BinaryWriter(fs);

                foreach (var b in res)
                    bw.Write(b);

                bw.Close();
                fs.Close();
            }
        }
    }
}
