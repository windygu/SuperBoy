using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Core.Common
{
    /// <summary>
    /// 中国农历年处理类用net自带类
    /// </summary>
    public class ChineseCalendar
    {
        /// <summary>
        /// 实例化一个  ChineseLunisolarCalendar
        /// </summary>
        private static ChineseLunisolarCalendar _cCalendar = new ChineseLunisolarCalendar();

        /// <summary>
        /// 获取农历当前日期
        /// </summary>
        /// <returns></returns>
        public static string GetChineseDateTimeNow()
        {
            return GetChineseDateTime(DateTime.Now);
        }

        /// <summary>
        /// 根据公历获取农历日期
        /// </summary>
        /// <param name="datetime">公历日期</param>
        /// <returns></returns>
        public static string GetChineseDateTime(DateTime datetime)
        {
            var lyear = _cCalendar.GetYear(datetime);
            var lmonth = _cCalendar.GetMonth(datetime);
            var lday = _cCalendar.GetDayOfMonth(datetime);

            //获取闰月， 0 则表示没有闰月
            var leapMonth = _cCalendar.GetLeapMonth(lyear);

            var isleap = false;

            if (leapMonth > 0)
            {
                if (leapMonth == lmonth)
                {
                    //闰月
                    isleap = true;
                    lmonth--;
                }
                else if (lmonth > leapMonth)
                {
                    lmonth--;
                }
            }

            return string.Concat(GetLunisolarYear(lyear), "年", isleap ? "闰" : string.Empty, GetLunisolarMonth(lmonth), "月", GetLunisolarDay(lday));
        }


        /// <summary>
        /// 返回农历日期
        /// </summary>
        public static string Now
        {
            get
            {
                return GetChineseDateTimeNow();
            }
        }

        /// <summary>
        /// 最大支持日期
        /// </summary>
        public static DateTime MaxSupportedDateTime
        {
            get
            {
                return _cCalendar.MaxSupportedDateTime;
            }
        }

        /// <summary>
        /// 最小支持日期
        /// </summary>
        public static DateTime MinSupportedDateTime
        {
            get
            {
                return _cCalendar.MinSupportedDateTime;
            }
        }

        /// <summary>
        /// 返回生肖
        /// </summary>
        /// <param name="datetime">公历日期</param>
        /// <returns></returns>
        public static string GetShengXiao(DateTime datetime)
        {
            return _shengxiao[_cCalendar.GetTerrestrialBranch(_cCalendar.GetSexagenaryYear(datetime)) - 1];
        }

        #region 农历年

        /// <summary>
        /// 十天干
        /// </summary>
        private static string[] _tiangan = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };

        /// <summary>
        /// 十二地支
        /// </summary>
        private static string[] _dizhi = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };

        /// <summary>
        /// 十二生肖
        /// </summary>
        private static string[] _shengxiao = { "鼠", "牛", "虎", "免", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
        
        /// <summary>
        /// 返回农历天干地支年
        /// </summary>
        /// <param name="year">农历年</param>
        /// <returns></returns>
        public static string GetLunisolarYear(int year)
        {
            if (year > 3)
            {
                var tgIndex = (year - 4) % 10;
                var dzIndex = (year - 4) % 12;

                return string.Concat(_tiangan[tgIndex], _dizhi[dzIndex], "[", _shengxiao[dzIndex], "]");
            }

            throw new ArgumentOutOfRangeException("无效的年份!");
        }
        
        #endregion

        #region 农历月

        /// <summary>
        /// 农历月
        /// </summary>
        private static string[] _months = { "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二(腊)" };
        
        /// <summary>
        /// 返回农历月
        /// </summary>
        /// <param name="month">月份</param>
        /// <returns></returns>
        public static string GetLunisolarMonth(int month)
        {
            if (month < 13 && month > 0)
            {
                return _months[month - 1];
            }

            throw new ArgumentOutOfRangeException("无效的月份!");
        }

        #endregion

        #region 农历日

        /// <summary>
        ///
        /// </summary>
        private static string[] _days1 = { "初", "十", "廿", "三" };

        /// <summary>
        /// 日
        /// </summary>
        private static string[] _days = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };

        /// <summary>
        /// 返回农历日
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static string GetLunisolarDay(int day)
        {
            if (day > 0 && day < 32)
            {
                if (day != 20 && day != 30)
                {
                    return string.Concat(_days1[(day - 1) / 10], _days[(day - 1) % 10]);
                }
                else
                {
                    return string.Concat(_days[(day - 1) / 10], _days1[1]);
                }
            }

            throw new ArgumentOutOfRangeException("无效的日!");
        }

        #endregion
    }
}
