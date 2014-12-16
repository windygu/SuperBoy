using System;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;

namespace SuperBoy.View
{
    /// <summary>
    /// 多媒体类，此类存放多媒体资料，数据以及调用多媒体方法
    /// </summary>
    class MultiMedia
    {
        #region 播放MP3
        /*调用方式
         * name = 你的mp3路径
              StringBuilder shortpath=new StringBuilder(80);
              int result=GetShortPathName(name,shortpath,shortpath.Capacity);
              name=shortpath.ToString();
              mciSendString(@"close all",null,0,0);
              mciSendString(@"open "+name+" alias song",null,0,0); //打开
              mciSendString("play song",null,0,0); //播放
         */
        public static uint SndAsync = 0x0001; // play asynchronously 
        public static uint SndFilename = 0x00020000; // name is file name
        [DllImport("winmm.dll")]
        public static extern int mciSendString(string mStrCmd, string mStrReceive, int mV1, int mV2);

        [DllImport("Kernel32", CharSet = CharSet.Auto)]
        public static extern Int32 GetShortPathName(String path, StringBuilder shortPath, Int32 shortPathLength);
        #endregion

        /// <summary>
        /// 播放Mp3等文件
        /// </summary>
        /// <param name="pathName"></param>
        private static void NewMusic(string pathName)
        {
            pathName = "D:\\自定义\\模板2\\我该干什么1.0\\bin\\Debug\\img\\新娘不是我.mp3";
            var shortpath = new StringBuilder(80);
            var result = GetShortPathName(pathName, shortpath, shortpath.Capacity);
            pathName = shortpath.ToString();
            mciSendString(@"close all", null, 0, 0);
            mciSendString(@"open " + pathName + " alias song", null, 0, 0); //打开
            mciSendString("play song", null, 0, 0);
        }
        /// <summary>
        /// wav音乐播放
        /// </summary>
        /// <param name="path"></param>
        public void Music(string path)
        {
            //   MessageBox.Show(Application.StartupPath.ToString());
            try
            {
                var sndPlayer = new SoundPlayer(path);
                //sndPlayer.PlayLooping(); 
                sndPlayer.Play();
            }
            catch (Exception)
            {

            }
        }
    }
}
