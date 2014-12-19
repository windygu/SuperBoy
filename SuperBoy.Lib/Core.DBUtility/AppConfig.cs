using System;
using System.IO;
using System.Xml;

namespace Core.DBUtility
{
    /// <summary>
    ///     ���ڻ�ȡ������Web.config/*.exe.config�нڵ����ݵĸ�����
    /// </summary>
    public sealed class AppConfig
    {
        private readonly string _filePath;

        /// <summary>
        ///     �ӵ�ǰĿ¼�а�˳�����Web.Config��*.App.Config�ļ���
        ///     ����ҵ�һ������ʹ������Ϊ�����ļ���������׳�һ��ArgumentNullException�쳣��
        /// </summary>
        public AppConfig()
        {
            var webconfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.Config");
            var appConfig = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.Replace(".vshost", "");

            if (File.Exists(webconfig))
            {
                _filePath = webconfig;
            }
            else if (File.Exists(appConfig))
            {
                _filePath = appConfig;
            }
            else
            {
                throw new ArgumentNullException("û���ҵ�Web.Config�ļ�����Ӧ�ó��������ļ�, ��ָ�������ļ�");
            }
        }

        /// <summary>
        ///     �û�ָ������������ļ�·��
        /// </summary>
        /// <param name="configFilePath">�����ļ�·��������·����</param>
        public AppConfig(string configFilePath)
        {
            _filePath = configFilePath;
        }

        /// <summary>
        ///     ���ó����config�ļ�
        /// </summary>
        /// <param name="keyName">����</param>
        /// <param name="keyValue">��ֵ</param>
        public void AppConfigSet(string keyName, string keyValue)
        {
            //���ڴ��ڶ��Add��ֵ��ʹ�÷���appSetting�Ĳ������ɹ�����ע��������䣬�����µķ�ʽ
            /* 
            string xpath = "//add[@key='" + keyName + "']";
            XmlDocument document = new XmlDocument();
            document.Load(filePath);

            XmlNode node = document.SelectSingleNode(xpath);
            node.Attributes["value"].Value = keyValue;
            document.Save(filePath); 
             */

            var document = new XmlDocument();
            document.Load(_filePath);

            var nodes = document.GetElementsByTagName("add");
            for (var i = 0; i < nodes.Count; i++)
            {
                //��ý���ǰԪ�ص�key����
                var attribute = nodes[i].Attributes["key"];
                //����Ԫ�صĵ�һ���������жϵ�ǰ��Ԫ���ǲ���Ŀ��Ԫ��
                if (attribute != null && (attribute.Value == keyName))
                {
                    attribute = nodes[i].Attributes["value"];
                    //��Ŀ��Ԫ���еĵڶ������Ը�ֵ
                    if (attribute != null)
                    {
                        attribute.Value = keyValue;
                        break;
                    }
                }
            }
            document.Save(_filePath);
        }

        /// <summary>
        ///     ��ȡ�����config�ļ��ļ�ֵ��
        ///     ������������ڣ����ؿ�
        /// </summary>
        /// <param name="keyName">����</param>
        /// <returns></returns>
        public string AppConfigGet(string keyName)
        {
            var strReturn = string.Empty;
            try
            {
                var document = new XmlDocument();
                document.Load(_filePath);

                var nodes = document.GetElementsByTagName("add");
                for (var i = 0; i < nodes.Count; i++)
                {
                    //��ý���ǰԪ�ص�key����
                    var attribute = nodes[i].Attributes["key"];
                    //����Ԫ�صĵ�һ���������жϵ�ǰ��Ԫ���ǲ���Ŀ��Ԫ��
                    if (attribute != null && (attribute.Value == keyName))
                    {
                        attribute = nodes[i].Attributes["value"];
                        if (attribute != null)
                        {
                            strReturn = attribute.Value;
                            break;
                        }
                    }
                }
            }
            catch
            {
                ;
            }

            return strReturn;
        }

        /// <summary>
        ///     ��ȡָ�������е������ֵ
        /// </summary>
        /// <param name="keyName">����</param>
        /// <param name="subKeyName">�Էֺ�(;)Ϊ�ָ�������������</param>
        /// <returns>��Ӧ�������Ƶ�ֵ������=�ź����ֵ��</returns>
        public string GetSubValue(string keyName, string subKeyName)
        {
            var connectionString = AppConfigGet(keyName).ToLower();
            var item = connectionString.Split(';');

            for (var i = 0; i < item.Length; i++)
            {
                var itemValue = item[i].ToLower();
                if (itemValue.IndexOf(subKeyName.ToLower()) >= 0) //�������ָ���Ĺؼ���
                {
                    var startIndex = item[i].IndexOf("="); //�Ⱥſ�ʼ��λ��
                    return item[i].Substring(startIndex + 1); //��ȡ�Ⱥź����ֵ��ΪValue
                }
            }
            return string.Empty;
        }

        #region һЩ���õ�����������

        /// <summary>
        ///     �������ļ���ȡȨ��ϵͳ���ӣ�������HWSecurity��ֵ��
        /// </summary>
        public string HwSecurity
        {
            get { return AppConfigGet("HWSecurity"); }
        }

        /// <summary>
        ///     ϵͳ�ı�ʶID��������System_ID��ֵ��
        /// </summary>
        public string SystemId
        {
            get { return AppConfigGet("System_ID"); }
        }

        /// <summary>
        ///     Ӧ�ó������ƣ�������ApplicationName��ֵ��
        /// </summary>
        public string AppName
        {
            get { return AppConfigGet("ApplicationName"); }
        }

        /// <summary>
        ///     ����������ƣ�������Manufacturer��ֵ��
        /// </summary>
        public string Manufacturer
        {
            get { return AppConfigGet("Manufacturer"); }
        }

        /// <summary>
        ///     ���ó����config�ļ���Enterprise Library�����ݿ����ӵ�ַ
        /// </summary>
        /// <param name="filePath">Web.config����*.exe.config�ļ��ľ���·��</param>
        /// <param name="keyName">����</param>
        /// <param name="keyValue">��ֵ</param>
        public void SetConnectionString(string keyName, string keyValue)
        {
            var document = new XmlDocument();
            document.Load(_filePath);

            var nodes = document.GetElementsByTagName("add");
            for (var i = 0; i < nodes.Count; i++)
            {
                //��ý���ǰԪ�ص�name����
                var att = nodes[i].Attributes["name"];
                //����Ԫ�صĵ�һ���������жϵ�ǰ��Ԫ���ǲ���Ŀ��Ԫ��
                if (att != null && (att.Value == keyName))
                {
                    att = nodes[i].Attributes["connectionString"];
                    if (att != null)
                    {
                        att.Value = keyValue;
                        break;
                    }
                }
            }
            document.Save(_filePath);
        }

        /// <summary>
        ///     ��ȡ�����config�ļ�Enterprise Library�����ݿ����ӵ�ַ
        /// </summary>
        /// <param name="filePath">Web.config����*.exe.config�ļ��ľ���·��</param>
        /// <param name="keyName">����</param>
        /// <returns></returns>
        public string GetConnectionString(string keyName)
        {
            var strReturn = string.Empty;
            try
            {
                var document = new XmlDocument();
                document.Load(_filePath);

                var nodes = document.GetElementsByTagName("add");
                for (var i = 0; i < nodes.Count; i++)
                {
                    //��ý���ǰԪ�ص�key����
                    var att = nodes[i].Attributes["name"];
                    //����Ԫ�صĵ�һ���������жϵ�ǰ��Ԫ���ǲ���Ŀ��Ԫ��
                    if (att != null && (att.Value == keyName))
                    {
                        att = nodes[i].Attributes["connectionString"];
                        if (att != null)
                        {
                            strReturn = att.Value;
                            break;
                        }
                    }
                }
            }
            catch
            {
                ;
            }

            return strReturn;
        }

        /// <summary>
        ///     ��ȡ���ݿ�������Ϣ
        /// </summary>
        /// <param name="keyName">�ڵ�����</param>
        /// <returns></returns>
        public DatabaseInfo GetDatabaseInfo(string keyName)
        {
            var connectionString = GetConnectionString(keyName);
            return new DatabaseInfo(connectionString);
        }

        /// <summary>
        ///     �������ݿ�������Ϣ
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="databaseInfo"></param>
        public void SetDatabaseInfo(string keyName, DatabaseInfo databaseInfo)
        {
            SetConnectionString(keyName, databaseInfo.ConnectionString);
        }

        #endregion
    }
}