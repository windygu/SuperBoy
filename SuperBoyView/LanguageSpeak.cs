using SpeechLib;

namespace SuperBoy
{
    class LanguageSpeak
    {

        /// <summary>
        /// 语言核心
        /// </summary>
        /// <param name="language">输入语言即可播放</param>
        public static void languaeSpeak(string language)
        {
            if (language != "")
            {
                SpeechVoiceSpeakFlags SpFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync;
                //SpeechVoiceSpeakFlags.
                SpVoice Voice = new SpVoice();
                Voice.Voice = Voice.GetVoices().Item(0);
                Voice.Speak(language, SpFlags);
            }
        }

        public static void languaeSpeak()
        {
            //默认提示语
            languaeSpeak("");
        }
    }
}
