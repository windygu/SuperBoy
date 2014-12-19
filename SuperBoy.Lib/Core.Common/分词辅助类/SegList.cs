using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Common
{
    /// <summary>
    ///     �ִʸ�����
    /// </summary>
    public class SegList
    {
        public int MaxLength;
        private readonly ArrayList _mSeg;

        public SegList()
        {
            _mSeg = new ArrayList();
            MaxLength = 0;
        }

        public int Count
        {
            get { return _mSeg.Count; }
        }

        public void Add(object obj)
        {
            _mSeg.Add(obj);
            if (MaxLength < obj.ToString().Length)
            {
                MaxLength = obj.ToString().Length;
            }
        }

        public object GetElem(int i)
        {
            return i < Count ? _mSeg[i] : null;
        }

        public void SetElem(int i, object obj)
        {
            _mSeg[i] = obj;
        }

        public bool Contains(object obj)
        {
            return _mSeg.Contains(obj);
        }

        /// <summary>
        ///     ����������
        /// </summary>
        public void Sort()
        {
            Sort(this);
        }

        /// <summary>
        ///     ����������
        /// </summary>
        public void Sort(SegList list)
        {
            for (var i = 0; i < list.Count - 1; ++i)
            {
                var max = i;
                for (var j = i + 1; j < list.Count; ++j)
                {
                    var str1 = list.GetElem(j).ToString();
                    var str2 = list.GetElem(max).ToString();
                    var l1 = str1 == "null" ? 0 : str1.Length;

                    var l2 = str2 == "null" ? 0 : str2.Length;

                    if (l1 > l2)
                        max = j;
                }
                var o = list.GetElem(max);
                list.SetElem(max, list.GetElem(i));
                list.SetElem(i, o);
            }
        }
    }

    /// <summary>
    ///     �ִ���
    /// </summary>
    //----------------����----------------------
    //Segment seg = new Segment();
    //seg.InitWordDics();
    //seg.EnablePrefix = true;
    //seg.Separator =" ";
    //seg.SegmentText("�ַ���", false).Trim();
    //-------------------------------------------
    public class Segment
    {
        #region ˽���ֶ�

        private string _mDicPath = HttpContext.Current.Server.MapPath("bin/ShootSeg/sDict.dic");
        private string _mNoisePath = HttpContext.Current.Server.MapPath("bin/ShootSeg/sNoise.dic");
        private string _mNumberPath = HttpContext.Current.Server.MapPath("bin/ShootSeg/sNumber.dic");
        private string _mWordPath = HttpContext.Current.Server.MapPath("bin/ShootSeg/sWord.dic");
        private string _mPrefixPath = HttpContext.Current.Server.MapPath("bin/ShootSeg/sPrefix.dic");
        private Hashtable _htWords;
        private ArrayList _alNoise;
        private ArrayList _alNumber;
        private ArrayList _alWord;
        private ArrayList _alPrefix;

        /// <summary>
        ///     �ָ���
        /// </summary>
        private string _mSeparator = " ";

        /// <summary>
        ///     ������֤���ֵ�������ʽ
        /// </summary>
        private readonly string _strChinese = "[\u4e00-\u9fa5]";

        #endregion

        #region ��������

        /// <summary>
        ///     �����ʵ�·��
        /// </summary>
        public string DicPath
        {
            get { return _mDicPath; }
            set { _mDicPath = value; }
        }

        /// <summary>
        ///     ���ݻ��溯��
        /// </summary>
        /// <param name="key">������</param>
        /// <param name="val">���������</param>
        private static void SetCache(string key, object val)
        {
            if (val == null) val = " ";
            HttpContext.Current.Application.Lock();
            HttpContext.Current.Application.Set(key, val);
            HttpContext.Current.Application.UnLock();
        }

        /// <summary>
        ///     ��ȡ����
        /// </summary>
        private static object GetCache(string key)
        {
            return HttpContext.Current.Application.Get(key);
        }

        /// <summary>
        ///     ��ʱ����
        /// </summary>
        public string NoisePath
        {
            get { return _mNoisePath; }
            set { _mNoisePath = value; }
        }

        /// <summary>
        ///     ���ִʵ�·��
        /// </summary>
        public string NumberPath
        {
            get { return _mNumberPath; }
            set { _mNumberPath = value; }
        }

        /// <summary>
        ///     ��ĸ�ʵ�·��
        /// </summary>
        public string WordPath
        {
            get { return _mWordPath; }
            set { _mWordPath = value; }
        }

        /// <summary>
        ///     ����ǰ׺�ֵ� ���ھ�������
        /// </summary>
        public string PrefixPath
        {
            get { return _mPrefixPath; }
            set { _mPrefixPath = value; }
        }

        /// <summary>
        ///     �Ƿ�������������
        /// </summary>
        public bool EnablePrefix
        {
            get
            {
                if (_alPrefix.Count == 0)
                    return false;
                return true;
            }
            set
            {
                if (value)
                    _alPrefix = LoadWords(PrefixPath, _alPrefix);
                else
                    _alPrefix = new ArrayList();
            }
        }

        /// <summary>
        ///     ��ʱÿ�ν��м��ػ�ִʶ���������Ա�ʾΪ��һ�ζ�������ʱ��
        ///     �Ѿ�ȷ�����뵫�ִʲ������ַ����϶�ʱ����Ϊ0
        /// </summary>
        public double EventTime { get; private set; }

        /// <summary>
        ///     �ָ���,Ĭ��Ϊ�ո�
        /// </summary>
        public string Separator
        {
            get { return _mSeparator; }
            set { if (value != "" && value != null) _mSeparator = value; }
        }

        #endregion

        #region ���췽��

        /// <summary>
        ///     ���췽��
        /// </summary>
        public Segment()
        {
            EventTime = 0;
        }

        /// <summary>
        ///     ���췽��
        /// </summary>
        public Segment(string pDicPath, string pNoisePath, string pNumberPath, string pWordPath)
        {
            EventTime = 0;
            _mWordPath = pDicPath;
            _mWordPath = pNoisePath;
            _mWordPath = pNumberPath;
            _mWordPath = pWordPath;
            InitWordDics();
        }

        #endregion

        #region ���з���

        /// <summary>
        ///     ���ش��б�
        /// </summary>
        public void InitWordDics()
        {
            var start = DateTime.Now;
            if (GetCache("jcms_dict") == null)
            {
                _htWords = new Hashtable();
                var father = _htWords;
                var forfather = _htWords;

                string strChar1;
                string strChar2;

                var reader = new StreamReader(DicPath, Encoding.UTF8);
                var strline = reader.ReadLine();

                SegList list;
                var child = new Hashtable();

                long i = 0;
                while (strline != null && strline.Trim() != "")
                {
                    i++;
                    strChar1 = strline.Substring(0, 1);
                    strChar2 = strline.Substring(1, 1);
                    if (!_htWords.ContainsKey(strChar1))
                    {
                        father = new Hashtable();
                        _htWords.Add(strChar1, father);
                    }
                    else
                    {
                        father = (Hashtable) _htWords[strChar1];
                    }

                    if (!father.ContainsKey(strChar2))
                    {
                        list = new SegList();
                        if (strline.Length > 2)
                            list.Add(strline.Substring(2));
                        else
                            list.Add("null");
                        father.Add(strChar2, list);
                    }
                    else
                    {
                        list = (SegList) father[strChar2];
                        if (strline.Length > 2)
                        {
                            list.Add(strline.Substring(2));
                        }
                        else
                        {
                            list.Add("null");
                        }
                        father[strChar2] = list;
                    }
                    _htWords[strChar1] = father;
                    strline = reader.ReadLine();
                }
                try
                {
                    reader.Close();
                }
                catch
                {
                }
                SetCache("jcms_dict", _htWords);
            }
            _htWords = (Hashtable) GetCache("jcms_dict");

            _alNoise = LoadWords(NoisePath, _alNoise);
            _alNumber = LoadWords(NumberPath, _alNumber);
            _alWord = LoadWords(WordPath, _alWord);
            _alPrefix = LoadWords(PrefixPath, _alPrefix);

            var duration = DateTime.Now - start;
            EventTime = duration.TotalMilliseconds;
        }

        /// <summary>
        ///     �����ı����鵽ArrayList
        /// </summary>
        public ArrayList LoadWords(string strPath, ArrayList list)
        {
            var reader = new StreamReader(strPath, Encoding.UTF8);
            list = new ArrayList();
            var strline = reader.ReadLine();
            while (strline != null)
            {
                list.Add(strline);
                strline = reader.ReadLine();
            }
            try
            {
                reader.Close();
            }
            catch
            {
            }
            return list;
        }

        /// <summary>
        ///     ������б�
        /// </summary>
        public void OutWords()
        {
            var idEnumerator1 = _htWords.GetEnumerator();
            while (idEnumerator1.MoveNext())
            {
                var idEnumerator2 = ((Hashtable) idEnumerator1.Value).GetEnumerator();
                while (idEnumerator2.MoveNext())
                {
                    var aa = (SegList) idEnumerator2.Value;
                    for (var i = 0; i < aa.Count; i++)
                    {
                        Console.WriteLine(idEnumerator1.Key + idEnumerator2.Key.ToString() + aa.GetElem(i));
                    }
                }
            }
        }

        /// <summary>
        ///     ���ArrayList
        /// </summary>
        public void OutArrayList(ArrayList list)
        {
            if (list == null) return;
            for (var i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list[i].ToString());
            }
        }

        /// <summary>
        ///     �ִʹ���,��֧�ֻس�
        /// </summary>
        /// <param name="strText">Ҫ�ִʵ��ı�</param>
        /// <returns>�ִʺ���ı�</returns>
        public string SegmentText(string strText)
        {
            strText = (strText + "$").Trim();
            if (_htWords == null) return strText;
            if (strText.Length < 3) return strText;
            var start = DateTime.Now;
            var length = 0;
            var preFix = 0;
            var word = false;
            var number = false;
            var reText = "";
            var strPrefix = "";
            var strLastChar = "";
            var strLastWords = Separator;

            for (var i = 0; i < strText.Length - 1; i++)
            {
                #region ����ÿһ���ֵĴ������

                var strChar1 = strText.Substring(i, 1);
                var strChar2 = strText.Substring(i + 1, 1).Trim();
                bool yes;
                SegList l;
                Hashtable h;

                if (reText.Length > 0) strLastChar = reText.Substring(reText.Length - 1);

                if (strChar1 == " ")
                {
                    if ((number || word) && strLastChar != Separator) reText += Separator;
                    yes = true;
                }
                else
                    yes = false;

                var charType = GetCharType(strChar1);
                switch (charType)
                {
                    case 1:

                        #region  ��������֣�������ֵ���һλ����ĸҪ�ͺ�������ַֿ�

                        if (word)
                        {
                            reText += Separator;
                        }
                        word = false;
                        number = true;
                        strLastWords = "";
                        break;

                        #endregion

                    case 2:
                    case 5:

                        #region �������ĸ

                        if (number)
                            strLastWords = Separator;
                        else
                            strLastWords = "";

                        word = true;
                        number = false;
                        break;

                        #endregion

                    case 3:
                    case 4:

                        #region ��һ����ϣ���Ƿ�����ؼ��֣������������ڶ�����ϣ��

                        //��һ�����Ƿ�Ϊ��ĸ
                        if (word) reText += Separator;

                        #region �����һ���Ƿ������֣���������������������ֺ�����ʵ�

                        if (number && charType != 4)
                        {
                            h = (Hashtable) _htWords["n"];
                            if (h.ContainsKey(strChar1))
                            {
                                l = (SegList) h[strChar1];
                                if (l.Contains(strChar2))
                                {
                                    reText += strChar1 + strChar2 + Separator;
                                    yes = true;
                                    i++;
                                }
                                else if (l.Contains("null"))
                                {
                                    reText += strChar1 + Separator;
                                    yes = true;
                                }
                            }
                            else
                                reText += Separator;
                        }

                        #endregion

                        //�Ǻ������ֵĺ���
                        if (charType == 3)
                        {
                            word = false;
                            number = false;
                            strLastWords = Separator;
                        }
                        else
                        {
                            word = false;
                            number = true;
                            strLastWords = "";
                        }

                        //�ڶ�����ϣ��ȡ��
                        h = (Hashtable) _htWords[strChar1];

                        //�ڶ�����ϣ���Ƿ�����ؼ���
                        if (h.ContainsKey(strChar2))
                        {
                            #region  �ڶ��������ؼ���

                            //ȡ��ArrayList����
                            l = (SegList) h[strChar2];

                            //����ÿһ������ ���Ƿ�����ϳɴ�
                            for (var j = 0; j < l.Count; j++)
                            {
                                var have = false;
                                var strChar3 = l.GetElem(j).ToString();

                                //����ÿһ��ȡ���Ĵʽ��м��,���Ƿ�ƥ�䣬���ȱ���
                                if ((strChar3.Length + i + 2) < strText.Length)
                                {
                                    //��i+2��ȡ��m���ȵ���
                                    var strChar = strText.Substring(i + 2, strChar3.Length).Trim();
                                    if (strChar3 == strChar && !yes)
                                    {
                                        if (strPrefix != "")
                                        {
                                            reText += strPrefix + Separator;
                                            strPrefix = "";
                                            preFix = 0;
                                        }
                                        reText += strChar1 + strChar2 + strChar;
                                        i += strChar3.Length + 1;
                                        have = true;
                                        yes = true;
                                        break;
                                    }
                                }
                                else if ((strChar3.Length + i + 2) == strText.Length)
                                {
                                    var strChar = strText.Substring(i + 2).Trim();
                                    if (strChar3 == strChar && !yes)
                                    {
                                        if (strPrefix != "")
                                        {
                                            reText += strPrefix + Separator;
                                            strPrefix = "";
                                            preFix = 0;
                                        }
                                        reText += strChar1 + strChar2 + strChar;
                                        i += strChar3.Length + 1;
                                        have = true;
                                        yes = true;
                                        break;
                                    }
                                }

                                if (!have && j == l.Count - 1 && l.Contains("null") && !yes)
                                {
                                    if (preFix == 1)
                                    {
                                        reText += strPrefix + strChar1 + strChar2;
                                        strPrefix = "";
                                        preFix = 0;
                                    }
                                    else if (preFix > 1)
                                    {
                                        reText += strPrefix + strLastWords + strChar1 + strChar2;
                                        strPrefix = "";
                                        preFix = 0;
                                    }
                                    else
                                    {
                                        if (charType == 4) reText += strChar1 + strChar2;
                                        else reText += strChar1 + strChar2;
                                        strLastWords = Separator;
                                        number = false;
                                    }
                                    i++;
                                    yes = true;
                                    break;
                                }
                                if (have)
                                {
                                    break;
                                }
                            }

                            #endregion

                            //���û��ƥ�仹������һ��������������ֻ�������֣����������ֿ�ͷ�Ĵ��ﲻ����
                            if (!yes && l.Contains("null"))
                            {
                                if (preFix == 1)
                                {
                                    reText += strPrefix + strChar1 + strChar2;
                                    strPrefix = "";
                                    preFix = 0;
                                }
                                else if (preFix > 1)
                                {
                                    reText += strPrefix + strLastWords + strChar1 + strChar2;
                                    strPrefix = "";
                                    preFix = 0;
                                }
                                else
                                {
                                    if (charType == 4) reText += strChar1 + strChar2;
                                    else reText += strChar1 + strChar2;
                                    strLastWords = Separator;
                                    number = false;
                                }
                                i++;
                                yes = true;
                            }
                            if (reText.Length > 0) strLastChar = reText.Substring(reText.Length - 1);
                            if (charType == 4 && GetCharType(strLastChar) == 4)
                            {
                                number = true;
                            }
                            else if (strLastChar != Separator) reText += Separator;
                        }

                        #endregion

                        break;
                    default:

                        #region δ֪�ַ�,��������Ƨ��,Ҳ�����Ǳ�����֮��

                        if (word && !yes)
                        {
                            reText += Separator;
                        }
                        else if (number && !yes)
                        {
                            reText += Separator;
                        }
                        number = false;
                        word = false;
                        strLastWords = Separator;
                        break;

                        #endregion
                }
                if (!yes && number || !yes && word)
                {
                    reText += strChar1;
                    yes = true;
                }
                if (!yes)
                {
                    #region ������������

                    if (preFix == 0)
                    {
                        if (_alPrefix.Contains(strChar1 + strChar2))
                        {
                            i++;
                            strPrefix = strChar1 + strChar2;
                            preFix++;
                        }
                        else if (_alPrefix.Contains(strChar1))
                        {
                            if (!number)
                            {
                                strPrefix = strChar1;
                                preFix++;
                            }
                            else
                            {
                                reText += strChar1 + strLastWords;
                                number = false;
                                word = false;
                            }
                        }
                        else
                        {
                            if (preFix == 3)
                            {
                                reText += strPrefix + Separator + strChar1 + Separator;
                                strPrefix = "";
                                preFix = 0;
                            }
                            else if (preFix > 0)
                            {
                                if (Regex.IsMatch(strChar1, _strChinese))
                                {
                                    strPrefix += strChar1;
                                    preFix++;
                                }
                                else
                                {
                                    reText += strPrefix + Separator + strChar1 + Separator;
                                    strPrefix = "";
                                    preFix = 0;
                                }
                            }
                            else
                            {
                                reText += strChar1 + strLastWords;
                                number = false;
                                word = false;
                            }
                        }
                    }
                    else
                    {
                        if (preFix == 3)
                        {
                            reText += strPrefix + Separator + strChar1 + Separator;
                            strPrefix = "";
                            preFix = 0;
                        }
                        else if (preFix > 0)
                        {
                            if (Regex.IsMatch(strChar1, _strChinese))
                            {
                                strPrefix += strChar1;
                                preFix++;
                            }
                            else
                            {
                                reText += strPrefix + Separator + strChar1 + Separator;
                                strPrefix = "";
                                preFix = 0;
                            }
                        }
                        else
                        {
                            reText += strChar1 + strLastWords;
                            number = false;
                        }
                    }

                    #endregion
                }
                length = i;

                #endregion
            }

            #region ����ֹ���һ���ֵĶ�ʧ

            if (length < strText.Length - 1)
            {
                var strLastChar1 = strText.Substring(strText.Length - 1).Trim();
                var strLastChar2 = strText.Substring(strText.Length - 2).Trim();

                if (reText.Length > 0) strLastChar = reText.Substring(reText.Length - 1);
                if (preFix != 0)
                {
                    reText += strPrefix + strLastChar1;
                }
                else
                {
                    switch (GetCharType(strLastChar1))
                    {
                        case 1:
                            if (strLastChar1 != "." && strLastChar1 != "��")
                                reText += strLastChar1;
                            else
                                reText += Separator + strLastChar1;
                            break;
                        case 2:
                        case 5:
                            if (_alWord.Contains(strLastChar2))
                                reText += strLastChar1;
                            break;
                        case 3:
                        case 4:
                            if ((number || word) && strLastChar != Separator)
                                reText += Separator + strLastChar1;
                            else
                                reText += strLastChar1;
                            break;
                        default:
                            if (strLastChar != Separator)
                                reText += Separator + strLastChar1;
                            else
                                reText += strLastChar1;
                            break;
                    }
                }
                if (reText.Length > 0) strLastChar = (reText.Substring(reText.Length - 1));
                if (strLastChar != Separator) reText += Separator;
            }

            #endregion

            var duration = DateTime.Now - start;
            EventTime = duration.TotalMilliseconds;
            return reText.Replace(" $", ""); //�������һ���ֵģ���ȥ��
        }

        /// <summary>
        ///     ���طִʹ���,֧�ֻس�
        /// </summary>
        public string SegmentText(string strText, bool enter)
        {
            if (enter)
            {
                var start = DateTime.Now;
                var strArr = strText.Split('\n');

                var reText = "";
                for (var i = 0; i < strArr.Length; i++)
                {
                    reText += SegmentText(strArr[i]) + "\r\n";
                }

                var duration = DateTime.Now - start;
                EventTime = duration.TotalMilliseconds;
                return reText;
            }
            return SegmentText(strText);
        }

        #region �ж��ַ�����

        /// <summary>
        ///     �ж��ַ�����,0Ϊδ֪,1Ϊ����,2Ϊ��ĸ,3Ϊ����,4Ϊ��������
        /// </summary>
        private int GetCharType(string pChar)
        {
            var charType = 0;
            if (_alNumber.Contains(pChar)) charType = 1;
            if (_alWord.Contains(pChar)) charType = 2;
            if (_htWords.ContainsKey(pChar)) charType += 3;
            return charType;
        }

        #endregion

        #region �Լ��صĴʵ���������д��

        /// <summary>
        ///     �Լ��صĴʵ���������д��
        /// </summary>
        public void SortDic()
        {
            SortDic(false);
        }

        /// <summary>
        ///     �Լ��صĴʵ���������д��
        /// </summary>
        /// <param name="reload">�Ƿ����¼���</param>
        public void SortDic(bool reload)
        {
            var start = DateTime.Now;
            var sw = new StreamWriter(DicPath, false, Encoding.UTF8);

            var idEnumerator1 = _htWords.GetEnumerator();
            while (idEnumerator1.MoveNext())
            {
                var idEnumerator2 = ((Hashtable) idEnumerator1.Value).GetEnumerator();
                while (idEnumerator2.MoveNext())
                {
                    var aa = (SegList) idEnumerator2.Value;
                    aa.Sort();
                    for (var i = 0; i < aa.Count; i++)
                    {
                        if (aa.GetElem(i).ToString() == "null")
                            sw.WriteLine(idEnumerator1.Key + idEnumerator2.Key.ToString());
                        else
                            sw.WriteLine(idEnumerator1.Key + idEnumerator2.Key.ToString() + aa.GetElem(i));
                    }
                }
            }
            sw.Close();

            if (reload) InitWordDics();

            var duration = DateTime.Now - start;
            EventTime = duration.TotalMilliseconds;
        }

        #endregion

        /// <summary>
        ///     ɾ��������ȫ��ͬ�Ĵ�,��ʱ����!
        /// </summary>
        /// <returns>��ͬ��������</returns>
        public int Optimize()
        {
            var l = 0;
            var start = DateTime.Now;

            var htOptimize = new Hashtable();
            var reader = new StreamReader(DicPath, Encoding.UTF8);
            var strline = reader.ReadLine();
            while (strline != null && strline.Trim() != "")
            {
                if (!htOptimize.ContainsKey(strline))
                    htOptimize.Add(strline, null);
                else
                    l++;
            }
            Console.WriteLine("ready");
            try
            {
                reader.Close();
            }
            catch
            {
            }
            var sw = new StreamWriter(DicPath, false, Encoding.UTF8);
            var ide = htOptimize.GetEnumerator();
            while (ide.MoveNext())
                sw.WriteLine(ide.Key.ToString());
            try
            {
                sw.Close();
            }
            catch
            {
            }
            var duration = DateTime.Now - start;
            EventTime = duration.TotalMilliseconds;
            return l;
        }

        #endregion
    }
}