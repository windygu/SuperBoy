//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2010 , Jirisoft , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;

namespace Core.Systems
{
    /// <summary>
    /// ResourceManagerWrapper
    /// 资源管理器
    /// 
    ///	修改纪录
    ///		2007.05.16 版本：1.0 JiRiGaLa	重新调整代码的规范化。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.05.16</date>
    /// </author> 
    /// </summary>
    public class ResourceManagerWrapper
    {
        private volatile static ResourceManagerWrapper _instance = null;
        private static object _locker = new Object();
        private static string _currentLanguage = "en-us";

        public static ResourceManagerWrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new ResourceManagerWrapper();
                        }
                    }
                }
                return _instance;
            }
        }

        private ResourceManager _resourceManager;

        public ResourceManagerWrapper()
        {
        }

        public void LoadResources(string path)
        {
            _resourceManager = ResourceManager.Instance;
            _resourceManager.Init(path);
        }

        public string Get(string key)
        {
            return _resourceManager.Get(_currentLanguage, key);
        }

        public string Get(string lanauage, string key)
        {
            return _resourceManager.Get(lanauage, key);
        }

        public Hashtable GetLanguages()
        {
            return _resourceManager.GetLanguages();
        }

        public Hashtable GetLanguages(string path)
        {
            return _resourceManager.GetLanguages(path);
        }

        public void Serialize(string path, string language, string key, string value)
        {
            var resources = this.GetResources(path, language);
            resources.Set(key, value);
            var filePath = path + "\\" + language + ".xml";
            _resourceManager.Serialize(resources, filePath);
        }

        public Resources GetResources(string path, string language)
        {
            var filePath = path + "\\" + language + ".xml";
            return _resourceManager.GetResources(filePath);
        }

        public Resources GetResources(string language)
        {
            return _resourceManager.LanguageResources[language];
        }
    }
}