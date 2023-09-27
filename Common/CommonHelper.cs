using System;
using System.Collections.Generic;
using System.Text;
using TinyPinyin;

namespace WpSpider.Common
{
    public class CommonHelper
    {
        public static long GetTimeStamp()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }

        public static string GetPinyin(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in str)
            {
                if (!char.IsWhiteSpace(item))
                {
                    if (PinyinHelper.IsChinese(item))
                    {
                        sb.Append(PinyinHelper.GetPinyin(item));
                    }
                }
            }
            return sb.ToString().ToLower();
        }
    }
}
