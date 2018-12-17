using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Common
{
    public class FormatDataCode
    {
        /// <summary>
        /// 获取全局Code
        /// </summary>
        /// <param name="flag">前缀</param>
        /// <param name="currentMaxCode">当前code</param>
        /// <param name="totalLenth">总长度</param>
        /// <returns>新code</returns>
        public static string GetNewLoacalCode(string flag, string currentMaxCode, int totalLenth = 10)
        {
            try
            {
                int currentCode = 0;
                if (!string.IsNullOrEmpty(currentMaxCode))
                {
                    currentCode = Convert.ToInt32(currentMaxCode.Replace(flag, string.Empty)) + 1;
                }

                string newNum = (currentCode > 0 ? currentCode : 1).ToString();

                int flagLenth = flag.Length;
                int zeroCount = totalLenth - flagLenth - newNum.Length;

                StringBuilder newCode = new StringBuilder();
                newCode.Append(flag);
                for (int index = 0; index < zeroCount; index++)
                {
                    newCode.Append("0");
                }

                newCode.Append(newNum);

                return newCode.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取子级code
        /// </summary>
        /// <param name="currentCode">当前最大code</param>
        /// <param name="parentCode">父级code</param>
        /// <returns>新code</returns>
        public static string GetNewMaxCode(string currentCode, string parentCode)
        {
            try
            {
                int newNum = 1;
                if (string.IsNullOrEmpty(parentCode))
                {
                    if (!string.IsNullOrEmpty(currentCode))
                    {
                        newNum = Convert.ToInt32(currentCode) + 1;
                    }

                    return newNum > 10 ? newNum.ToString() : $"0{newNum}";
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentCode))
                    {
                        string childCode = currentCode.TrimStart('0').TrimStart('1');
                        newNum = Convert.ToInt32(childCode) + 1;
                    }

                    return newNum > 10 ? $"{parentCode}{newNum.ToString()}" : $"{parentCode}0{newNum}";
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
