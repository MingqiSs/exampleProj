using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{

    public class Utils
    {
        /// <summary>
        /// 
        /// </summary>
        public static long GetUsTimeDateSpan()
        {
            TimeZoneInfo easternZone;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    easternZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");//linux
                    break;
                case PlatformID.Win32NT:
                    easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); //window
                    break;
                default:
                    easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); //window
                                                                                                //  throw new PlatformNotSupportedException("不支持的系统");
                    break;
            }

            DateTime eastTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);

            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            var timeSpan = ((long)(eastTime.Date - startTime).TotalMilliseconds);

            return timeSpan;
        }

        public static DateTime GetTime(long unixTimeStamp)
        {

            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            return startTime.AddMilliseconds(unixTimeStamp);
        }
        public static int GetB2UShort(byte[] pacInput)
        {
            return BitConverter.ToInt16(pacInput, 20);//消息数包
        }
        public static long GetB2ULong(byte[] pacInput)
        {
            return BitConverter.ToInt64(pacInput, 20);//消息数包
        }
        /// <summary>
        /// 美股时间转换
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string GetUsDateTimeToStr(byte[] buffer)
        {
            var a = HexStringUtil.ByteArrayTo16String(buffer);
            var b = Convert.ToInt64(a, 16);
            var date = Utils.GetUsTimeDateSpan();
            var c = date + (b / 1000 / 1000);
            var time = Utils.GetTime(c);
            return time.ToString("HH:mm:ss:ffff");
        }
        /// <summary>
        /// 美股时间转换
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static DateTime GetUsDateTime(byte[] buffer)
        {
            var a = HexStringUtil.ByteArrayTo16String(buffer);
            var b = Convert.ToInt64(a, 16);
            var date = Utils.GetUsTimeDateSpan();
            var c = date + (b / 1000 / 1000);
            var time = Utils.GetTime(c);
            return time;
        }
        /// <summary>
        ///  美股时间转换
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static long GetUsDataTimestamp(byte[] buffer)
        {
            var a = HexStringUtil.ByteArrayTo16String(buffer);
            var b = Convert.ToInt64(a, 16);
            var date = Utils.GetUsTimeDateSpan();
            var c = date + (b / 1000 / 1000);
            return c;

        }
        ///// <summary>
        /////  美股时间转换
        ///// </summary>
        ///// <param name="buffer"></param>
        ///// <returns></returns>
        public static long GetUsTimestamp(byte[] buffer)
        {
            var a = HexStringUtil.ByteArrayTo16String(buffer);
            var b = Convert.ToInt64(a, 16);
            return (b / 1000 / 1000);
        }
        /// <summary>
        /// utctick to bj datetime
        /// </summary>
        /// <param name="utcTick"></param>
        /// <returns></returns>
        public static DateTime ConvertBJDatetime(long utcTick)
        {
            var time = Utils.GetTime(utcTick);
            time = time.AddHours(13);
            return time;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="utcTick"></param>
        /// <returns></returns>
        public static DateTime ConvertDateTime(long utcTick)
        {
            var time = Utils.GetTime(utcTick);
            return time;
        }
        /// <summary>
        ///北京时间转换美国东部时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime BeijingTimeToUsDateTime(DateTime date)
        {
            return AMESTime.BeijingTimeToUsDateTime(date);
        }

        /// <summary>
        /// 
        /// </summary>
        public static string GetPathBySysPlat()
        {
            return Path.Combine().Replace(@"\", "/");
            //TimeZoneInfo easternZone;
            //switch (Environment.OSVersion.Platform)
            //{
            //    case PlatformID.Unix:

            //        break;
            //    case PlatformID.Win32NT:

            //        break;
            //    default:

            //        break;
            //}
            //return Path.Combine().Replace(@"\", "/");
        }
      
    }

    public class AMESTime
    {
        private static DateTime _thisYearDaylightSavingTimeStart,
            _thisYearDaylightSavingTimeEnd;

        private const int TIMEZONE_OFFSET_DAY_SAVING_LIGHT = -12;
        private const int TIMEZONE_OFFSET = -13;

        public static DateTime BeijingTimeToUsDateTime(DateTime beijingTime)
        {
            int offsetHours = (IsNowAMESDayLightSavingTime ? TIMEZONE_OFFSET_DAY_SAVING_LIGHT : TIMEZONE_OFFSET);

            return beijingTime.AddHours(offsetHours);
        }

        public static DateTime AMESNow
        {
            get
            {
                return BeijingTimeToUsDateTime(DateTime.Now);
            }
        }
        public static bool IsNowAMESDayLightSavingTime
        {
            get
            {
                return DateTime.UtcNow > DayLightSavingStartTimeUtc
                    && DateTime.UtcNow < DayLightSavingEndTimeUtc;
            }
        }
        /// <summary>
        /// 夏令时开始时间
        /// </summary>
        static DateTime DayLightSavingStartTimeUtc
        {
            get
            {
                if (_thisYearDaylightSavingTimeStart.Year != DateTime.Now.Year)
                {
                    DateTime temp = new DateTime(DateTime.Now.Year, 3, 8, 0, 0, 0);
                    while (temp.DayOfWeek != DayOfWeek.Sunday)
                    {
                        temp = temp.AddDays(1);
                    }
                    _thisYearDaylightSavingTimeStart = temp.AddHours(TIMEZONE_OFFSET);
                }

                return _thisYearDaylightSavingTimeStart;
            }
        }
        /// <summary>
        /// 夏令时结束时间
        /// </summary>
        static DateTime DayLightSavingEndTimeUtc
        {
            get
            {
                if (_thisYearDaylightSavingTimeEnd.Year != DateTime.Now.Year)
                {
                    DateTime temp = new DateTime(DateTime.Now.Year, 11, 1, 0, 0, 0);
                    while (temp.DayOfWeek != DayOfWeek.Sunday)
                    {
                        temp = temp.AddDays(1);
                    }
                    _thisYearDaylightSavingTimeEnd = temp.AddHours(TIMEZONE_OFFSET_DAY_SAVING_LIGHT);
                }
                return _thisYearDaylightSavingTimeEnd;
            }
        }
    }
}
