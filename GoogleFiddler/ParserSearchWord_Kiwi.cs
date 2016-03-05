using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleFiddler
{
    public static class ParserSearchWord_Kiwi
    {
        /// <summary>
        /// 获取字符串在另一个字符串中第n次出现的位置。
        /// </summary>
        /// <param name="str">基础字符串</param>
        /// <param name="cstr">检查的字符串</param>
        /// <param name="p1">检查的开始位置</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int getpos(string str, string cstr, int p1, int count)
        {
            int idx = p1;
            int posc = 0;

            idx = str.IndexOf(cstr, p1);
            if (idx != -1) posc = 1;
            else return idx;
            if (posc == count) return idx;
            else
            {
                idx += cstr.Length;
                do
                {
                    idx = str.IndexOf(cstr, idx);
                    if (idx != -1)
                    {
                        posc++;
                        if (posc == count) break;
                    }
                    else break;
                    idx += cstr.Length;
                } while (true);

            }

            return idx;
        }
    }
}
