using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;
using System.Reflection;

namespace Core.Common
{
    public class Reflect<T> where T : class 
    {
        private static Hashtable _mObjCache = null;
        public static Hashtable ObjCache
        {
            get
            {
                if (_mObjCache == null)
                {
                    _mObjCache = new Hashtable();
                }
                return _mObjCache;
            }
        }

        public static T Create(string sName, string sFilePath)
        {
            return Create(sName, sFilePath, true);
        }
        public static T Create(string sName, string sFilePath, bool bCache)
        {
            var cacheKey = sFilePath + "." + sName;
            T objType = null;
            if (bCache)
            {
                objType = (T)ObjCache[cacheKey];    //从缓存读取 
                if (!ObjCache.ContainsKey(cacheKey))
                {
                    var assObj = CreateAssembly(sFilePath);
                    var obj = assObj.CreateInstance(cacheKey);
                    objType = (T)obj;

                    ObjCache.Add(cacheKey, objType);// 写入缓存 将DAL内某个对象装入缓存
                }
            }
            else
            {
                objType = (T)CreateAssembly(sFilePath).CreateInstance(cacheKey); //反射创建 
            }

            return objType;
        }

        public static Assembly CreateAssembly(string sFilePath)
        {
            var assObj = (Assembly)ObjCache[sFilePath];
            if (assObj == null)
            {
                assObj = Assembly.Load(sFilePath);
                if (!ObjCache.ContainsKey(sFilePath))
                {
                    ObjCache.Add(sFilePath, assObj);//将整个ITDB。DAL。DLL装入缓存
                }
            }
            return assObj;
        }
    }
}
