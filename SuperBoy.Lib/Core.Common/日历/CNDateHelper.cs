﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Common
{  
    /// <summary>
    /// 中国日历
    /// </summary>          
    //-------------------------------------------------------------------------------
    //调用:
    //CNDateHelper c = new CNDateHelper(new DateTime(1990, 01, 15));
    //StringBuilder dayInfo = new StringBuilder();
    //dayInfo.Append("阳历：" + c.DateString + "\r\n");
    //dayInfo.Append("农历：" + c.ChineseDateString + "\r\n");
    //dayInfo.Append("星期：" + c.WeekDayStr);
    //dayInfo.Append("时辰：" + c.ChineseHour + "\r\n");
    //dayInfo.Append("属相：" + c.AnimalString + "\r\n");
    //dayInfo.Append("节气：" + c.ChineseTwentyFourDay + "\r\n");
    //dayInfo.Append("前一个节气：" + c.ChineseTwentyFourPrevDay + "\r\n");
    //dayInfo.Append("下一个节气：" + c.ChineseTwentyFourNextDay + "\r\n");
    //dayInfo.Append("节日：" + c.DateHoliday + "\r\n");
    //dayInfo.Append("干支：" + c.GanZhiDateString + "\r\n");
    //dayInfo.Append("星宿：" + c.ChineseConstellation + "\r\n");
    //dayInfo.Append("星座：" + c.Constellation + "\r\n");
    //-------------------------------------------------------------------------------
    public  class CnDateHelper
    {
        public CnDateHelper()
        {
            
        }
        #region 内部变量
        private DateTime _date;
        private DateTime _datetime;
        private int _cYear;
        private int _cMonth;
        private int _cDay;
        private bool _cIsLeapMonth; //当月是否闰月
        private bool _cIsLeapYear;  //当年是否有闰月
        #endregion
    
        #region 内部结构
  
        #region 阳历放假天数和节日
        /// <summary>
        /// 阳历放假天数和节日
        /// </summary>
        private struct SolarHolidayStruct
        {
            public int Month;
            public int Day;
            public int Recess; //假期长度
            public string HolidayName;
            public SolarHolidayStruct(int month, int day, int recess, string name)
            {
                Month = month;
                Day = day;
                Recess = recess;
                HolidayName = name;
            }
        }
        #endregion
    
        #region 农历放假天数和节日
        /// <summary>
        /// 农历放假天数和节日
        /// </summary>
        private struct LunarHolidayStruct
        {
            public int Month;
            public int Day;
            public int Recess;
            public string HolidayName;

            public LunarHolidayStruct(int month, int day, int recess, string name)
            {
                Month = month;
                Day = day;
                Recess = recess;
                HolidayName = name;
            }
        }
        #endregion

        #region 按某月第几个星期几放假天数和节日
        /// <summary>
        ///  按某月第几个星期几放假天数和节日
        /// </summary>
        private struct WeekHolidayStruct
        {
            public int Month;
            public int WeekAtMonth;
            public int WeekDay;
            public string HolidayName;

            public WeekHolidayStruct(int month, int weekAtMonth, int weekDay, string name)
            {
                Month = month;
                WeekAtMonth = weekAtMonth;
                WeekDay = weekDay;
                HolidayName = name;
            }
        }
        #endregion
        
        #endregion

        #region 基础数据

        #region 基本常量
        private const int MinYear = 1900;
        private const int MaxYear = 2050;
        private static DateTime _minDay = new DateTime(1900, 1, 30);
        private static DateTime _maxDay = new DateTime(2049, 12, 31);
        private const int GanZhiStartYear = 1864; //干支计算起始年
        private static DateTime _ganZhiStartDay = new DateTime(1899, 12, 22);//起始日
        private const string HzNum = "零一二三四五六七八九";
        private const int AnimalStartYear = 1900; //1900年为鼠年
        private static DateTime _chineseConstellationReferDay = new DateTime(2007, 9, 13);//28星宿参考值,本日为角
        #endregion

        #region 阴历数据
        /// <summary>
        /// 来源于网上的农历数据
        /// </summary>
        /// <remarks>
        /// 数据结构如下，共使用17位数据
        /// 第17位：表示闰月天数，0表示29天   1表示30天
        /// 第16位-第5位（共12位）表示12个月，其中第16位表示第一月，如果该月为30天则为1，29天为0
        /// 第4位-第1位（共4位）表示闰月是哪个月，如果当年没有闰月，则置0
        ///</remarks>
        private static int[] _lunarDateArray = new int[]{
                0x04BD8,0x04AE0,0x0A570,0x054D5,0x0D260,0x0D950,0x16554,0x056A0,0x09AD0,0x055D2,
                0x04AE0,0x0A5B6,0x0A4D0,0x0D250,0x1D255,0x0B540,0x0D6A0,0x0ADA2,0x095B0,0x14977,
                0x04970,0x0A4B0,0x0B4B5,0x06A50,0x06D40,0x1AB54,0x02B60,0x09570,0x052F2,0x04970,
                0x06566,0x0D4A0,0x0EA50,0x06E95,0x05AD0,0x02B60,0x186E3,0x092E0,0x1C8D7,0x0C950,
                0x0D4A0,0x1D8A6,0x0B550,0x056A0,0x1A5B4,0x025D0,0x092D0,0x0D2B2,0x0A950,0x0B557,
                0x06CA0,0x0B550,0x15355,0x04DA0,0x0A5B0,0x14573,0x052B0,0x0A9A8,0x0E950,0x06AA0,
                0x0AEA6,0x0AB50,0x04B60,0x0AAE4,0x0A570,0x05260,0x0F263,0x0D950,0x05B57,0x056A0,
                0x096D0,0x04DD5,0x04AD0,0x0A4D0,0x0D4D4,0x0D250,0x0D558,0x0B540,0x0B6A0,0x195A6,
                0x095B0,0x049B0,0x0A974,0x0A4B0,0x0B27A,0x06A50,0x06D40,0x0AF46,0x0AB60,0x09570,
                0x04AF5,0x04970,0x064B0,0x074A3,0x0EA50,0x06B58,0x055C0,0x0AB60,0x096D5,0x092E0,
                0x0C960,0x0D954,0x0D4A0,0x0DA50,0x07552,0x056A0,0x0ABB7,0x025D0,0x092D0,0x0CAB5,
                0x0A950,0x0B4A0,0x0BAA4,0x0AD50,0x055D9,0x04BA0,0x0A5B0,0x15176,0x052B0,0x0A930,
                0x07954,0x06AA0,0x0AD50,0x05B52,0x04B60,0x0A6E6,0x0A4E0,0x0D260,0x0EA65,0x0D530,
                0x05AA0,0x076A3,0x096D0,0x04BD7,0x04AD0,0x0A4D0,0x1D0B6,0x0D250,0x0D520,0x0DD45,
                0x0B5A0,0x056D0,0x055B2,0x049B0,0x0A577,0x0A4B0,0x0AA50,0x1B255,0x06D20,0x0ADA0,
                0x14B63        
                };

        #endregion

        #region 星座名称
        private static string[] _constellationName = 
                { 
                    "白羊座", "金牛座", "双子座", 
                    "巨蟹座", "狮子座", "处女座", 
                    "天秤座", "天蝎座", "射手座", 
                    "摩羯座", "水瓶座", "双鱼座"
                };
        #endregion

        #region 二十四节气
        private static string[] _lunarHolidayName = 
                    { 
                    "小寒", "大寒", "立春", "雨水", 
                    "惊蛰", "春分", "清明", "谷雨", 
                    "立夏", "小满", "芒种", "夏至", 
                    "小暑", "大暑", "立秋", "处暑", 
                    "白露", "秋分", "寒露", "霜降", 
                    "立冬", "小雪", "大雪", "冬至"
                    };
        #endregion

        #region 二十八星宿
        private static string[] _chineseConstellationName =
            {
                  //四       五      六        日      一        二      三  
                "角木蛟","亢金龙","女土蝠","房日兔","心月狐","尾火虎","箕水豹",
                "斗木獬","牛金牛","氐土貉","虚日鼠","危月燕","室火猪","壁水獝",
                "奎木狼","娄金狗","胃土彘","昴日鸡","毕月乌","觜火猴","参水猿",
                "井木犴","鬼金羊","柳土獐","星日马","张月鹿","翼火蛇","轸水蚓" 
            };
        #endregion

        #region 节气数据
        private static string[] _solarTerm = new string[] { "小寒", "大寒", "立春", "雨水", "惊蛰", "春分", "清明", "谷雨", "立夏", "小满", "芒种", "夏至", "小暑", "大暑", "立秋", "处暑", "白露", "秋分", "寒露", "霜降", "立冬", "小雪", "大雪", "冬至" };
        private static int[] _sTermInfo = new int[] { 0, 21208, 42467, 63836, 85337, 107014, 128867, 150921, 173149, 195551, 218072, 240693, 263343, 285989, 308563, 331033, 353350, 375494, 397447, 419210, 440795, 462224, 483532, 504758 };
        #endregion

        #region 农历相关数据
        private static string _ganStr = "甲乙丙丁戊己庚辛壬癸";
        private static string _zhiStr = "子丑寅卯辰巳午未申酉戌亥";
        private static string _animalStr = "鼠牛虎兔龙蛇马羊猴鸡狗猪";
        private static string _nStr1 = "日一二三四五六七八九";
        private static string _nStr2 = "初十廿卅";
        private static string[] _monthString =
                {
                    "出错","正月","二月","三月","四月","五月","六月","七月","八月","九月","十月","十一月","腊月"
                };
        #endregion

        #region 按公历计算的节日
        private static SolarHolidayStruct[] _sHolidayInfo = new SolarHolidayStruct[]{
            new SolarHolidayStruct(1, 1, 1, "元旦"),
            new SolarHolidayStruct(2, 2, 0, "世界湿地日"),
            new SolarHolidayStruct(2, 10, 0, "国际气象节"),
            new SolarHolidayStruct(2, 14, 0, "情人节"),
            new SolarHolidayStruct(3, 1, 0, "国际海豹日"),
            new SolarHolidayStruct(3, 5, 0, "学雷锋纪念日"),
            new SolarHolidayStruct(3, 8, 0, "妇女节"), 
            new SolarHolidayStruct(3, 12, 0, "植树节 孙中山逝世纪念日"), 
            new SolarHolidayStruct(3, 14, 0, "国际警察日"),
            new SolarHolidayStruct(3, 15, 0, "消费者权益日"),
            new SolarHolidayStruct(3, 17, 0, "中国国医节 国际航海日"),
            new SolarHolidayStruct(3, 21, 0, "世界森林日 消除种族歧视国际日 世界儿歌日"),
            new SolarHolidayStruct(3, 22, 0, "世界水日"),
            new SolarHolidayStruct(3, 24, 0, "世界防治结核病日"),
            new SolarHolidayStruct(4, 1, 0, "愚人节"),
            new SolarHolidayStruct(4, 7, 0, "世界卫生日"),
            new SolarHolidayStruct(4, 22, 0, "世界地球日"),
            new SolarHolidayStruct(5, 1, 1, "劳动节"), 
            new SolarHolidayStruct(5, 2, 1, "劳动节假日"),
            new SolarHolidayStruct(5, 3, 1, "劳动节假日"),
            new SolarHolidayStruct(5, 4, 0, "青年节"), 
            new SolarHolidayStruct(5, 8, 0, "世界红十字日"),
            new SolarHolidayStruct(5, 12, 0, "国际护士节"), 
            new SolarHolidayStruct(5, 31, 0, "世界无烟日"), 
            new SolarHolidayStruct(6, 1, 0, "国际儿童节"), 
            new SolarHolidayStruct(6, 5, 0, "世界环境保护日"),
            new SolarHolidayStruct(6, 26, 0, "国际禁毒日"),
            new SolarHolidayStruct(7, 1, 0, "建党节 香港回归纪念 世界建筑日"),
            new SolarHolidayStruct(7, 11, 0, "世界人口日"),
            new SolarHolidayStruct(8, 1, 0, "建军节"), 
            new SolarHolidayStruct(8, 8, 0, "中国男子节 父亲节"),
            new SolarHolidayStruct(8, 15, 0, "抗日战争胜利纪念"),
            new SolarHolidayStruct(9, 9, 0, "毛主席逝世纪念"), 
            new SolarHolidayStruct(9, 10, 0, "教师节"), 
            new SolarHolidayStruct(9, 18, 0, "九·一八事变纪念日"),
            new SolarHolidayStruct(9, 20, 0, "国际爱牙日"),
            new SolarHolidayStruct(9, 27, 0, "世界旅游日"),
            new SolarHolidayStruct(9, 28, 0, "孔子诞辰"),
            new SolarHolidayStruct(10, 1, 1, "国庆节 国际音乐日"),
            new SolarHolidayStruct(10, 2, 1, "国庆节假日"),
            new SolarHolidayStruct(10, 3, 1, "国庆节假日"),
            new SolarHolidayStruct(10, 6, 0, "老人节"), 
            new SolarHolidayStruct(10, 24, 0, "联合国日"),
            new SolarHolidayStruct(11, 10, 0, "世界青年节"),
            new SolarHolidayStruct(11, 12, 0, "孙中山诞辰纪念"), 
            new SolarHolidayStruct(12, 1, 0, "世界艾滋病日"), 
            new SolarHolidayStruct(12, 3, 0, "世界残疾人日"), 
            new SolarHolidayStruct(12, 20, 0, "澳门回归纪念"), 
            new SolarHolidayStruct(12, 24, 0, "平安夜"), 
            new SolarHolidayStruct(12, 25, 0, "圣诞节"), 
            new SolarHolidayStruct(12, 26, 0, "毛主席诞辰纪念")
           };
        #endregion

        #region 按农历计算的节日
        private static LunarHolidayStruct[] _lHolidayInfo = new LunarHolidayStruct[]{
            new LunarHolidayStruct(1, 1, 1, "春节"), 
            new LunarHolidayStruct(1, 15, 0, "元宵节"), 
            new LunarHolidayStruct(5, 5, 0, "端午节"), 
            new LunarHolidayStruct(7, 7, 0, "七夕情人节"),
            new LunarHolidayStruct(7, 15, 0, "中元节 盂兰盆节"), 
            new LunarHolidayStruct(8, 15, 0, "中秋节"), 
            new LunarHolidayStruct(9, 9, 0, "重阳节"), 
            new LunarHolidayStruct(12, 8, 0, "腊八节"),
            new LunarHolidayStruct(12, 23, 0, "北方小年(扫房)"),
            new LunarHolidayStruct(12, 24, 0, "南方小年(掸尘)"),
            //new LunarHolidayStruct(12, 30, 0, "除夕")  //注意除夕需要其它方法进行计算
        };
        #endregion

        #region 按某月第几个星期几
        private static WeekHolidayStruct[] _wHolidayInfo = new WeekHolidayStruct[]{
            new WeekHolidayStruct(5, 2, 1, "母亲节"), 
            new WeekHolidayStruct(5, 3, 1, "全国助残日"), 
            new WeekHolidayStruct(6, 3, 1, "父亲节"), 
            new WeekHolidayStruct(9, 3, 3, "国际和平日"), 
            new WeekHolidayStruct(9, 4, 1, "国际聋人节"), 
            new WeekHolidayStruct(10, 1, 2, "国际住房日"), 
            new WeekHolidayStruct(10, 1, 4, "国际减轻自然灾害日"),
            new WeekHolidayStruct(11, 4, 5, "感恩节")
        };
        #endregion
        #endregion

        #region 构造函数
        #region 公历日期初始化
        /// <summary>
        /// 用一个标准的公历日期来初使化
        /// </summary>
        public CnDateHelper(DateTime dt)
        {
            int i;
            int leap;
            int temp;
            int offset;

            CheckDateLimit(dt);

            _date = dt.Date;
            _datetime = dt;

            //农历日期计算部分
            leap = 0;
            temp = 0;

            //计算两天的基本差距
            var ts = _date - _minDay;
            offset = ts.Days;

            for (i = MinYear; i <= MaxYear; i++)
            {
                //求当年农历年天数
                temp = GetChineseYearDays(i);
                if (offset - temp < 1)
                    break;
                else
                {
                    offset = offset - temp;
                }
            }
            _cYear = i;

            //计算该年闰哪个月
            leap = GetChineseLeapMonth(_cYear);

            //设定当年是否有闰月
            if (leap > 0)
            {
                _cIsLeapYear = true;
            }
            else
            {
                _cIsLeapYear = false;
            }

            _cIsLeapMonth = false;
            for (i = 1; i <= 12; i++)
            {
                //闰月
                if ((leap > 0) && (i == leap + 1) && (_cIsLeapMonth == false))
                {
                    _cIsLeapMonth = true;
                    i = i - 1;
                    temp = GetChineseLeapMonthDays(_cYear); //计算闰月天数
                }
                else
                {
                    _cIsLeapMonth = false;
                    temp = GetChineseMonthDays(_cYear, i);  //计算非闰月天数
                }

                offset = offset - temp;
                if (offset <= 0) break;
            }

            offset = offset + temp;
            _cMonth = i;
            _cDay = offset;
        }
        #endregion

        #region 农历日期初始化
        /// <summary>
        /// 用农历的日期来初使化
        /// </summary>
        /// <param name="cy">农历年</param>
        /// <param name="cm">农历月</param>
        /// <param name="cd">农历日</param>
        /// <param name="LeapFlag">闰月标志</param>
        public CnDateHelper(int cy, int cm, int cd, bool leapMonthFlag)
        {
            int i, leap, temp, offset;

            CheckChineseDateLimit(cy, cm, cd, leapMonthFlag);

            _cYear = cy;
            _cMonth = cm;
            _cDay = cd;

            offset = 0;

            for (i = MinYear; i < cy; i++)
            {
                //求当年农历年天数
                temp = GetChineseYearDays(i);
                offset = offset + temp;
            }

            //计算该年应该闰哪个月
            leap = GetChineseLeapMonth(cy);
            if (leap != 0)
            {
                this._cIsLeapYear = true;
            }
            else
            {
                this._cIsLeapYear = false;
            }

            if (cm != leap)
            {
                //当前日期并非闰月
                _cIsLeapMonth = false;
            }
            else
            {
                //使用用户输入的是否闰月月份
                _cIsLeapMonth = leapMonthFlag;
            }

            //当年没有闰月||计算月份小于闰月
            if ((_cIsLeapYear == false) || (cm < leap))
            {
                for (i = 1; i < cm; i++)
                {
                    temp = GetChineseMonthDays(cy, i);//计算非闰月天数
                    offset = offset + temp;
                }

                //检查日期是否大于最大天
                if (cd > GetChineseMonthDays(cy, cm))
                {
                    throw new Exception("不合法的农历日期");
                }
                //加上当月的天数
                offset = offset + cd;
            }

            //是闰年，且计算月份大于或等于闰月
            else
            {
                for (i = 1; i < cm; i++)
                {
                    //计算非闰月天数
                    temp = GetChineseMonthDays(cy, i);
                    offset = offset + temp;
                }

                //计算月大于闰月
                if (cm > leap)
                {
                    temp = GetChineseLeapMonthDays(cy);   //计算闰月天数
                    offset = offset + temp;               //加上闰月天数

                    if (cd > GetChineseMonthDays(cy, cm))
                    {
                        throw new Exception("不合法的农历日期");
                    }
                    offset = offset + cd;
                }

                //计算月等于闰月
                else
                {
                    //如果需要计算的是闰月，则应首先加上与闰月对应的普通月的天数
                    if (this._cIsLeapMonth == true)         //计算月为闰月
                    {
                        temp = GetChineseMonthDays(cy, cm); //计算非闰月天数
                        offset = offset + temp;
                    }

                    if (cd > GetChineseLeapMonthDays(cy))
                    {
                        throw new Exception("不合法的农历日期");
                    }
                    offset = offset + cd;
                }
            }
            _date = _minDay.AddDays(offset);
        }
        #endregion
        #endregion

        #region 私有函数
        #region 传回农历y年m月的总天数 GetChineseMonthDays
        /// <summary>
        /// 传回农历y年m月的总天数
        /// </summary>
        private int GetChineseMonthDays(int year, int month)
        {
            if (BitTest32((_lunarDateArray[year - MinYear] & 0x0000FFFF), (16 - month)))
            {
                return 30;
            }
            else
            {
                return 29;
            }
        }
        #endregion

        #region 传回农历 y年闰哪个月 1-12 , 没闰传回 0 GetChineseLeapMonth
        /// <summary>
        /// 传回农历 y年闰哪个月 1-12 , 没闰传回 0
        /// </summary>
        private int GetChineseLeapMonth(int year)
        {
            return _lunarDateArray[year - MinYear] & 0xF;
        }
        #endregion

        #region  传回农历y年闰月的天数 GetChineseLeapMonthDays
        /// <summary>
        /// 传回农历y年闰月的天数
        /// </summary>
        private int GetChineseLeapMonthDays(int year)
        {
            if (GetChineseLeapMonth(year) != 0)
            {
                if ((_lunarDateArray[year - MinYear] & 0x10000) != 0)
                {
                    return 30;
                }
                else
                {
                    return 29;
                }
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region 取农历年一年的天数 GetChineseYearDays
        /// <summary>
        /// 取农历年一年的天数
        /// </summary>
        private int GetChineseYearDays(int year)
        {
            int i, f, sumDay, info;

            sumDay = 348; //29天*12个月
            i = 0x8000;
            info = _lunarDateArray[year - MinYear] & 0x0FFFF;

            //计算12个月中有多少天为30天
            for (var m = 0; m < 12; m++)
            {
                f = info & i;
                if (f != 0)
                {
                    sumDay++;
                }
                i = i >> 1;
            }
            return sumDay + GetChineseLeapMonthDays(year);
        }
        #endregion

        #region 获得当前时间的时辰 GetChineseHour
        /// <summary>
        /// 获得当前时间的时辰
        /// </summary> 
        private string GetChineseHour(DateTime dt)
        {
            int hour, minute, offset, i;
            int indexGan;
            string ganHour, zhiHour;
            string tmpGan;

            //计算时辰的地支
            hour = dt.Hour;    //获得当前时间小时
            minute = dt.Minute;  //获得当前时间分钟

            if (minute != 0) hour += 1;
            offset = hour / 2;
            if (offset >= 12) offset = 0;
            //zhiHour = zhiStr[offset].ToString();

            //计算天干
            var ts = this._date - _ganZhiStartDay;
            i = ts.Days % 60;

            //ganStr[i % 10] 为日的天干,(n*2-1) %10得出地支对应,n从1开始
            indexGan = ((i % 10 + 1) * 2 - 1) % 10 - 1;

            tmpGan = _ganStr.Substring(indexGan) + _ganStr.Substring(0, indexGan + 2);//凑齐12位
            //ganHour = ganStr[((i % 10 + 1) * 2 - 1) % 10 - 1].ToString();

            return tmpGan[offset].ToString() + _zhiStr[offset].ToString();
        }
        #endregion

        #region 检查公历日期是否符合要求 CheckDateLimit
        /// <summary>
        /// 检查公历日期是否符合要求
        /// </summary>
        private void CheckDateLimit(DateTime dt)
        {
            if ((dt < _minDay) || (dt > _maxDay))
            {
                throw new Exception("超出可转换的日期");
            }
        }
        #endregion

        #region 检查农历日期是否合理 CheckChineseDateLimit
        /// <summary>
        /// 检查农历日期是否合理
        /// </summary>
        private void CheckChineseDateLimit(int year, int month, int day, bool leapMonth)
        {
            if ((year < MinYear) || (year > MaxYear))
            {
                throw new Exception("非法农历日期");
            }
            if ((month < 1) || (month > 12))
            {
                throw new Exception("非法农历日期");
            }
            if ((day < 1) || (day > 30)) //中国的月最多30天
            {
                throw new Exception("非法农历日期");
            }
            var leap = GetChineseLeapMonth(year);// 计算该年应该闰哪个月
            if ((leapMonth == true) && (month != leap))
            {
                throw new Exception("非法农历日期");
            }
        }
        #endregion

        #region 将0-9转成汉字形式 ConvertNumToChineseNum
        /// <summary>
        /// 将0-9转成汉字形式
        /// </summary>
        private string ConvertNumToChineseNum(char n)
        {
            if ((n < '0') || (n > '9')) return "";
            switch (n)
            {
                case '0':
                    return HzNum[0].ToString();
                case '1':
                    return HzNum[1].ToString();
                case '2':
                    return HzNum[2].ToString();
                case '3':
                    return HzNum[3].ToString();
                case '4':
                    return HzNum[4].ToString();
                case '5':
                    return HzNum[5].ToString();
                case '6':
                    return HzNum[6].ToString();
                case '7':
                    return HzNum[7].ToString();
                case '8':
                    return HzNum[8].ToString();
                case '9':
                    return HzNum[9].ToString();
                default:
                    return "";
            }
        }
        #endregion

        #region 测试某位是否为真 BitTest32
        /// <summary>
        /// 测试某位是否为真
        /// </summary>
        private bool BitTest32(int num, int bitpostion)
        {
            if ((bitpostion > 31) || (bitpostion < 0))
                throw new Exception("Error Param: bitpostion[0-31]:" + bitpostion.ToString());

            var bit = 1 << bitpostion;

            if ((num & bit) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region 将星期几转成数字表示 ConvertDayOfWeek
        /// <summary>
        /// 将星期几转成数字表示
        /// </summary>
        private int ConvertDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 1;
                case DayOfWeek.Monday:
                    return 2;
                case DayOfWeek.Tuesday:
                    return 3;
                case DayOfWeek.Wednesday:
                    return 4;
                case DayOfWeek.Thursday:
                    return 5;
                case DayOfWeek.Friday:
                    return 6;
                case DayOfWeek.Saturday:
                    return 7;
                default:
                    return 0;
            }
        }
        #endregion

        #region 比较当天是不是指定的第周几 CompareWeekDayHoliday
        /// <summary>
        /// 比较当天是不是指定的第周几
        /// </summary>
        private bool CompareWeekDayHoliday(DateTime date, int month, int week, int day)
        {
            var ret = false;

            if (date.Month == month) //月份相同
            {
                if (ConvertDayOfWeek(date.DayOfWeek) == day) //星期几相同
                {
                    var firstDay = new DateTime(date.Year, date.Month, 1);//生成当月第一天
                    var i = ConvertDayOfWeek(firstDay.DayOfWeek);
                    var firWeekDays = 7 - ConvertDayOfWeek(firstDay.DayOfWeek) + 1; //计算第一周剩余天数

                    if (i > day)
                    {
                        if ((week - 1) * 7 + day + firWeekDays == date.Day)
                        {
                            ret = true;
                        }
                    }
                    else
                    {
                        if (day + firWeekDays + (week - 2) * 7 == date.Day)
                        {
                            ret = true;
                        }
                    }
                }
            }

            return ret;
        }
        #endregion
        #endregion

        #region  公有方法
        #region 节日
        #region 计算中国农历节日 newCalendarHoliday
        /// <summary>
        /// 计算中国农历节日
        /// </summary>
        public string NewCalendarHoliday
        {
            get
            {
                var tempStr = "";
                if (this._cIsLeapMonth == false) //闰月不计算节日
                {
                    foreach (var lh in _lHolidayInfo)
                    {
                        if ((lh.Month == this._cMonth) && (lh.Day == this._cDay))
                        {

                            tempStr = lh.HolidayName;
                            break;

                        }
                    }

                    //对除夕进行特别处理
                    if (this._cMonth == 12)
                    {
                        var i = GetChineseMonthDays(this._cYear, 12); //计算当年农历12月的总天数
                        if (this._cDay == i) //如果为最后一天
                        {
                            tempStr = "除夕";
                        }
                    }
                }
                return tempStr;
            }
        }
        #endregion

        #region 按某月第几周第几日计算的节日 WeekDayHoliday
        /// <summary>
        /// 按某月第几周第几日计算的节日
        /// </summary>
        public string WeekDayHoliday
        {
            get
            {
                var tempStr = "";
                foreach (var wh in _wHolidayInfo)
                {
                    if (CompareWeekDayHoliday(_date, wh.Month, wh.WeekAtMonth, wh.WeekDay))
                    {
                        tempStr = wh.HolidayName;
                        break;
                    }
                }
                return tempStr;
            }
        }
        #endregion

        #region 按公历日计算的节日 DateHoliday
        /// <summary>
        /// 按公历日计算的节日
        /// </summary>
        public string DateHoliday
        {
            get
            {
                var tempStr = "";

                foreach (var sh in _sHolidayInfo)
                {
                    if ((sh.Month == _date.Month) && (sh.Day == _date.Day))
                    {
                        tempStr = sh.HolidayName;
                        break;
                    }
                }
                return tempStr;
            }
        }
        #endregion
        #endregion

        #region 公历日期
        #region 取对应的公历日期Date
        /// <summary>
        /// 取对应的公历日期
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
        #endregion

        #region 取星期几 WeekDay
        /// <summary>
        /// 取星期几
        /// </summary>
        public DayOfWeek WeekDay
        {
            get { return _date.DayOfWeek; }
        }
        #endregion

        #region 周几的字符 WeekDayStr
        /// <summary>
        /// 周几的字符
        /// </summary>
        public string WeekDayStr
        {
            get
            {
                switch (_date.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        return "星期日";
                    case DayOfWeek.Monday:
                        return "星期一";
                    case DayOfWeek.Tuesday:
                        return "星期二";
                    case DayOfWeek.Wednesday:
                        return "星期三";
                    case DayOfWeek.Thursday:
                        return "星期四";
                    case DayOfWeek.Friday:
                        return "星期五";
                    default:
                        return "星期六";
                }
            }
        }
        #endregion
  
        #region 根据日期值获得周一的日期
        /// <summary>
        /// 根据日期值获得周一的日期
        /// </summary>
        /// <param name="dt">输入日期</param>
        /// <returns>周一的日期</returns>
        public static DateTime GetMondayDateByDate(DateTime dt)
        {
            double d = 0;
            switch ((int)dt.DayOfWeek)
            {
                //case 1: d = 0; break;
                case 2: d = -1; break;
                case 3: d = -2; break;
                case 4: d = -3; break;
                case 5: d = -4; break;
                case 6: d = -5; break;
                case 0: d = -6; break;
            }
            return dt.AddDays(d);
        }
        #endregion 
      
        #region 传回公历y年m月的总天数
      
        /// <summary>
        /// 传回公历y年m月的总天数
        /// </summary>
        public static int GetDaysByMonth(int y, int m)
        {
            /*第一种*/
            var days = new int[] { 31, DateTime.IsLeapYear(y) ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            return days[m - 1];
            /*第二种*/
            ////公历月份
            //int[] solarMonth = new int[] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            //if (m == 2)
            //{
            //    return (((y % 4 == 0) && (y % 100 != 0) || (y % 400 == 0)) ? 29 : 28);
            //}
            //else
            //{
            //    return (solarMonth[m]);
            //}
        }
       #endregion
   
        #region 公历日期中文表示法 如一九九七年七月一日 DateString
        /// <summary>
        /// 公历日期中文表示法 如一九九七年七月一日
        /// </summary>
        public string DateString
        {
            get
            {
                return "公元" + this._date.ToLongDateString();
            }
        }
        #endregion

        #region 当前是否公历闰年IsLeapYear
        /// <summary>
        /// 当前是否公历闰年
        /// </summary>
        public bool IsLeapYear
        {
            get
            {
                return DateTime.IsLeapYear(this._date.Year);
            }
        }
        #endregion

        #region 28星宿计算 ChineseConstellation
        /// <summary>
        /// 28星宿计算
        /// </summary>
        public string ChineseConstellation
        {
            get
            {
                var offset = 0;
                var modStarDay = 0;

                var ts = this._date - _chineseConstellationReferDay;
                offset = ts.Days;
                modStarDay = offset % 28;
                return (modStarDay >= 0 ? _chineseConstellationName[modStarDay] : _chineseConstellationName[27 + modStarDay]);
            }
        }
        #endregion

        #region 时辰 ChineseHour
        /// <summary>
        /// 时辰
        /// </summary>
        public string ChineseHour
        {
            get
            {
                return GetChineseHour(_datetime);
            }
        }
        #endregion

        #endregion

        #region 农历日期
        #region 是否闰月 IsChineseLeapMonth
        /// <summary>
        /// 是否闰月
        /// </summary>
        public bool IsChineseLeapMonth
        {
            get { return this._cIsLeapMonth; }
        }
        #endregion

        #region 当年是否有闰月 IsChineseLeapYear
        /// <summary>
        /// 当年是否有闰月
        /// </summary>
        public bool IsChineseLeapYear
        {
            get
            {
                return this._cIsLeapYear;
            }
        }
        #endregion

        #region  农历日 ChineseDay
        /// <summary>
        /// 农历日
        /// </summary>
        public int ChineseDay
        {
            get { return this._cDay; }
        }
        #endregion

        #region 农历日中文表示 ChineseDayString
        /// <summary>
        /// 农历日中文表示
        /// </summary>
        public string ChineseDayString
        {
            get
            {
                switch (this._cDay)
                {
                    case 0:
                        return "";
                    case 10:
                        return "初十";
                    case 20:
                        return "二十";
                    case 30:
                        return "三十";
                    default:
                        return _nStr2[(int)(_cDay / 10)].ToString() + _nStr1[_cDay % 10].ToString();

                }
            }
        }
        #endregion

        #region 农历的月份 ChineseMonth
        /// <summary>
        /// 农历的月份
        /// </summary>
        public int ChineseMonth
        {
            get { return this._cMonth; }
        }
        #endregion

        #region 农历月份字符串 ChineseMonthString
        /// <summary>
        /// 农历月份字符串
        /// </summary>
        public string ChineseMonthString
        {
            get
            {
                return _monthString[this._cMonth];
            }
        }
        #endregion

        #region  取农历年份 ChineseYear
        /// <summary>
        /// 取农历年份
        /// </summary>
        public int ChineseYear
        {
            get { return this._cYear; }
        }
        #endregion

        #region 取农历年字符串如，一九九七年 ChineseYearString
        /// <summary>
        /// 取农历年字符串如，一九九七年
        /// </summary>
        public string ChineseYearString
        {
            get
            {
                var tempStr = "";
                var num = this._cYear.ToString();
                for (var i = 0; i < 4; i++)
                {
                    tempStr += ConvertNumToChineseNum(num[i]);
                }
                return tempStr + "年";
            }
        }
        #endregion

        #region 取农历日期表示法：农历一九九七年正月初五 ChineseDateString
        /// <summary>
        /// 取农历日期表示法：农历一九九七年正月初五
        /// </summary>
        public string ChineseDateString
        {
            get
            {
                if (this._cIsLeapMonth == true)
                {
                    return "农历" + ChineseYearString + "闰" + ChineseMonthString + ChineseDayString;
                }
                else
                {
                    return "农历" + ChineseYearString + ChineseMonthString + ChineseDayString;
                }
            }
        }
        #endregion

        #endregion
 
        #region 定气法计算二十四节气,二十四节气是按地球公转来计算的，并非是阴历计算的  ChineseTwentyFourDay
        /// <summary>
        /// 定气法计算二十四节气,二十四节气是按地球公转来计算的，并非是阴历计算的
        /// </summary>
        /// <remarks>
        /// 节气的定法有两种。古代历法采用的称为"恒气"，即按时间把一年等分为24份，
        /// 每一节气平均得15天有余，所以又称"平气"。现代农历采用的称为"定气"，即
        /// 按地球在轨道上的位置为标准，一周360°，两节气之间相隔15°。由于冬至时地
        /// 球位于近日点附近，运动速度较快，因而太阳在黄道上移动15°的时间不到15天。
        /// 夏至前后的情况正好相反，太阳在黄道上移动较慢，一个节气达16天之多。采用
        /// 定气时可以保证春、秋两分必然在昼夜平分的那两天。
        /// </remarks>
        public string ChineseTwentyFourDay
        {
            get
            {
                var baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
                DateTime newDate;
                double num;
                int y;
                var tempStr = "";

                y = this._date.Year;

                for (var i = 1; i <= 24; i++)
                {
                    num = 525948.76 * (y - 1900) + _sTermInfo[i - 1];

                    newDate = baseDateAndTime.AddMinutes(num);//按分钟计算
                    if (newDate.DayOfYear == _date.DayOfYear)
                    {
                        tempStr = _solarTerm[i - 1];
                        break;
                    }
                }
                return tempStr;
            }
        }

        #region 当前日期前一个最近节气
        /// <summary>
        ///    当前日期前一个最近节气
        /// </summary>
        public string ChineseTwentyFourPrevDay
        {
            get
            {
                var baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
                DateTime newDate;
                double num;
                int y;
                var tempStr = "";

                y = this._date.Year;

                for (var i = 24; i >= 1; i--)
                {
                    num = 525948.76 * (y - 1900) + _sTermInfo[i - 1];

                    newDate = baseDateAndTime.AddMinutes(num);//按分钟计算

                    if (newDate.DayOfYear < _date.DayOfYear)
                    {
                        tempStr = string.Format("{0}[{1}]", _solarTerm[i - 1], newDate.ToString("yyyy-MM-dd"));
                        break;
                    }
                }

                return tempStr;
            }

        }

        #endregion

        #region  当前日期后一个最近节气

        /// <summary>
        /// 当前日期后一个最近节气
        /// </summary>
        public string ChineseTwentyFourNextDay
        {
            get
            {
                var baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
                DateTime newDate;
                double num;
                int y;
                var tempStr = "";

                y = this._date.Year;

                for (var i = 1; i <= 24; i++)
                {
                    num = 525948.76 * (y - 1900) + _sTermInfo[i - 1];

                    newDate = baseDateAndTime.AddMinutes(num);//按分钟计算

                    if (newDate.DayOfYear > _date.DayOfYear)
                    {
                        tempStr = string.Format("{0}[{1}]", _solarTerm[i - 1], newDate.ToString("yyyy-MM-dd"));
                        break;
                    }
                }
                return tempStr;
            }

        }
        #endregion
        #endregion

        #region 星座
        /// <summary>
        /// 计算指定日期的星座序号 
        /// </summary>
        public int GetConstellation()
        {
           
                var index = 0;
                int y, m, d;
                y = _date.Year;
                m = _date.Month;
                d = _date.Day;
                y = m * 100 + d;

                if (((y >= 321) && (y <= 419))) { index = 0; }
                else if ((y >= 420) && (y <= 520)) { index = 1; }
                else if ((y >= 521) && (y <= 620)) { index = 2; }
                else if ((y >= 621) && (y <= 722)) { index = 3; }
                else if ((y >= 723) && (y <= 822)) { index = 4; }
                else if ((y >= 823) && (y <= 922)) { index = 5; }
                else if ((y >= 923) && (y <= 1022)) { index = 6; }
                else if ((y >= 1023) && (y <= 1121)) { index = 7; }
                else if ((y >= 1122) && (y <= 1221)) { index = 8; }
                else if ((y >= 1222) || (y <= 119)) { index = 9; }
                else if ((y >= 120) && (y <= 218)) { index = 10; }
                else if ((y >= 219) && (y <= 320)) { index = 11; }
                else { index = -1; }

                return index;
            
        }
        
        /// <summary>
        /// 计算指定日期的星座名称
        /// </summary>
        /// <returns></returns>
        public string GetConstellationName()
        {

            int constellation;

            constellation = GetConstellation();

            if ((constellation >= 0) && (constellation <= 11))

            { return _constellationName[constellation]; }

            else

            { return ""; };

        }
        #endregion

        #region 属相
        #region Animal
        /// <summary>
        /// 计算属相的索引，注意虽然属相是以农历年来区别的，但是目前在实际使用中是按公历来计算的
        /// 鼠年为1,其它类推
        /// </summary>
        public int Animal
        {
            get
            {
                var offset = _date.Year - AnimalStartYear;
                return (offset % 12) + 1;
            }
        }
        #endregion

        #region 取属相字符串 AnimalString
        /// <summary>
        /// 取属相字符串
        /// </summary>
        public string AnimalString
        {
            get
            {
                var offset = _date.Year - AnimalStartYear; //阳历计算
                //int offset = this._cYear - AnimalStartYear;　农历计算
                return _animalStr[offset % 12].ToString();
            }
        }
        #endregion
        #endregion
            
        #region 天干地支
        #region 取农历年的干支表示法如 乙丑年 GanZhiYearString
        /// <summary>
        /// 取农历年的干支表示法如 乙丑年
        /// </summary>
        public string GanZhiYearString
        {
            get
            {
                string tempStr;
                var i = (this._cYear - GanZhiStartYear) % 60; //计算干支
                tempStr = _ganStr[i % 10].ToString() + _zhiStr[i % 12].ToString() + "年";
                return tempStr;
            }
        }
        #endregion

        #region 取干支的月表示字符串，注意农历的闰月不记干支 GanZhiMonthString
        /// <summary>
        /// 取干支的月表示字符串，注意农历的闰月不记干支
        /// </summary>
        public string GanZhiMonthString
        {
            get
            {
                //每个月的地支总是固定的,而且总是从寅月开始
                int zhiIndex;
                string zhi;
                if (this._cMonth > 10)
                {
                    zhiIndex = this._cMonth - 10;
                }
                else
                {
                    zhiIndex = this._cMonth + 2;
                }
                zhi = _zhiStr[zhiIndex - 1].ToString();

                //根据当年的干支年的干来计算月干的第一个
                var ganIndex = 1;
                string gan;
                var i = (this._cYear - GanZhiStartYear) % 60; //计算干支
                switch (i % 10)
                {
                    #region ...
                    case 0: //甲
                        ganIndex = 3;
                        break;
                    case 1: //乙
                        ganIndex = 5;
                        break;
                    case 2: //丙
                        ganIndex = 7;
                        break;
                    case 3: //丁
                        ganIndex = 9;
                        break;
                    case 4: //戊
                        ganIndex = 1;
                        break;
                    case 5: //己
                        ganIndex = 3;
                        break;
                    case 6: //庚
                        ganIndex = 5;
                        break;
                    case 7: //辛
                        ganIndex = 7;
                        break;
                    case 8: //壬
                        ganIndex = 9;
                        break;
                    case 9: //癸
                        ganIndex = 1;
                        break;
                    #endregion
                }
                gan = _ganStr[(ganIndex + this._cMonth - 2) % 10].ToString();

                return gan + zhi + "月";
            }
        }
        #endregion

        #region 取干支日表示法 GanZhiDayString
        /// <summary>
        /// 取干支日表示法
        /// </summary>
        public string GanZhiDayString
        {
            get
            {
                int i, offset;
                var ts = this._date - _ganZhiStartDay;
                offset = ts.Days;
                i = offset % 60;
                return _ganStr[i % 10].ToString() + _zhiStr[i % 12].ToString() + "日";
            }
        }
        #endregion

        #region 取当前日期的干支表示法如 甲子年乙丑月丙庚日 GanZhiDateString
        /// <summary>
        /// 取当前日期的干支表示法如 甲子年乙丑月丙庚日
        /// </summary>
        public string GanZhiDateString
        {
            get
            {
                return GanZhiYearString + GanZhiMonthString + GanZhiDayString;
            }
        }
        #endregion
        #endregion
        #endregion

        #region 更具生日获取星座

        /// <summary>
        /// 根据生日获取星座
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static string GeConstellation(DateTime birthday)
        {
            var birthdayF = 0.00F;
            if (birthday.Month == 1 && birthday.Day < 20)
            {
                birthdayF = float.Parse(string.Format("13.{0}", birthday.Day));
            }
            else
            {
                birthdayF = float.Parse(string.Format("{0}.{1}", birthday.Month, birthday.Day));
            }
            float[] atomBound = { 1.20F, 2.20F, 3.21F, 4.21F, 5.21F, 6.22F, 7.23F, 8.23F, 9.23F, 10.23F, 11.21F, 12.22F, 13.20F };
            string[] atoms = { "水瓶座", "双鱼座", "白羊座", "金牛座", "双子座", "巨蟹座", "狮子座", "处女座", "天秤座", "天蝎座", "射手座", "魔羯座" };

            var ret = "靠！外星人啊。";
            for (var i = 0; i < atomBound.Length - 1; i++)
            {
                if (atomBound[i] <= birthdayF && atomBound[i + 1] > birthdayF)
                {
                    ret = atoms[i];
                    break;
                }
            }
            return ret == "靠！外星人啊。" ? "魔羯座" : ret;
        }

        #endregion
    }
}
