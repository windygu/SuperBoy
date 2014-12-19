using System;
using System.Globalization;
using System.Threading;

namespace Core.Systems
{
    public class CultureInfoUtil
    {
        /// <summary>
        /// ��ʼ�����Ի���
        /// </summary>
        public static void InitializeCulture()
        {
            var language = LoadLanguage();

            if (!string.IsNullOrEmpty(language))
            {
                var culture = new CultureInfo(language);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }

        public static string LoadLanguage()
        {
            const string key = "language";
            var language = RegistryHelper.GetValue(key);

            if (string.IsNullOrEmpty(language))
            {
                //����û�δ�������ԣ������ǰϵͳΪ���ģ���ô��ʾ���ģ�������ʾӢ��
                if (Thread.CurrentThread.CurrentCulture.Name.IndexOf("CN", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    language = "zh-CN";
                }
                else
                {
                    language = "en-US"; //��������Ĭ��ΪӢ��
                }
            }
            return language;
        }

        /// <summary>
        /// Region��Ϣ�����ģ�:
        ///       Name:                            CN
        ///       DisplayName:                     �л����񹲺͹�
        ///       EnglishName:                     People's Republic of China
        ///       IsMetric:                        True
        ///       ThreeLetterISORegionName:        CHN
        ///       ThreeLetterWindowsRegionName: CHN
        ///       TwoLetterISORegionName:          CN
        ///       CurrencySymbol:                  ��
        ///       ISOCurrencySymbol:               CNY
        /// </summary>
        public static RegionInfo CurrentRegion
        {
            get
            {
                return RegionInfo.CurrentRegion;
            }
        }
    }
}
