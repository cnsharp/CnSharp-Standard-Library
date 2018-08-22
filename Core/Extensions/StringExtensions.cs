using System;

namespace CnSharp.Extensions
{
    public static class StringExtensions
    {
      
        /// <summary>
        ///     SBC case string to DBC case string
        /// </summary>
        /// <param name="sbcString"></param>
        /// <returns></returns>
        public static string ToDBC(this string sbcString)
        {
            var res = sbcString.ToCharArray();

            for (var i = 0; i < res.Length; i++)
            {
                if (res[i] == SbcBlankToInt)
                {
                    res[i] = Convert.ToChar(DbcBlankToInt);
                    continue;
                }

                if (res[i] >= MinSbcCaseToInt && res[i] <= MaxSbcCaseToInt)
                {
                    res[i] = Convert.ToChar(res[i] - Margin);
                }
            }

            return new string(res);
        }

        /// <summary>
        ///     DBC case string to SBC case string
        /// </summary>
        /// <param name="dbcString"></param>
        /// <returns></returns>
        public static String ToSBC(this String dbcString)
        {
            if (dbcString == null)
            {
                return null;
            }
            var c = dbcString.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                if (c[i] == SbcBlankToInt)
                {
                    c[i] = (char) SbcBlankToInt;
                }
                else if (c[i] <= MaxDbcCaseToInt)
                {
                    c[i] = (char) (c[i] + Margin);
                }
            }
            return new String(c);
        }

        #region Constants and Fields

        /// <summary>
        ///     DBC blank
        /// </summary>
        private const int DbcBlankToInt = 32;

        /// <summary>
        ///     margin between DBC case character to it's corresponding SBC case character
        /// </summary>
        private const int Margin = 65248;

        // 最大的有效半角英文字符转换成int型数据的值
        private const int MaxDbcCaseToInt = 126;

        private const int MaxSbcCaseToInt = 65374;

        // 最小的有效半角英文字符转换成int型数据的值
        private const int MinDbcCaseToInt = 33;

        private const int MinSbcCaseToInt = 65281;

        private const int SbcBlankToInt = 12288;

        #endregion
    }
}