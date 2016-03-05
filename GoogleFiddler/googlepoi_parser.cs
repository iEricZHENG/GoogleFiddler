using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleParse
{

    public class googlepoi {
        public string oid = "";
        public string classaddress = "";
        public string address = "";
        public string fname = "";
        public string name = "";
        public string category = "";
        public string phone = "";
        public string website = "";
        public string lat = "";
        public string lon = "";
        public string closed = "";

    }
    public class pairs
    {
        public int p1 = -1, p2 = -1;
        public pairs(int _p1, int _p2)
        {
            this.p1 = _p1; this.p2 = _p2;
        }
    }

    public class googlepoi_parser
    {
       
        public googlepoi_parser() { }
       static public List<googlepoi> GetItems(string str) {

            List<googlepoi> pois = new List<googlepoi>();
            pairs p = getparis(str, "{", "}", 0);
            // Console.WriteLine(p.p1+","+p.p2);
            string mstr = str.Substring(p.p1, p.p2 - p.p1 + 1);
            int pos = getpos(str, "[", p.p2, 3);
            p = getparis(str, "[", "]", pos);
            string itemstr = "";
            //获取数据列表的内容
            mstr = str.Substring(p.p1, p.p2 - p.p1 + 1);
            pos = 1;
            do
            {
                p = getparis(mstr, "[", "]", pos);

                itemstr = mstr.Substring(p.p1, p.p2 - p.p1 + 1);
               // Console.WriteLine(itemstr);
                googlepoi poi= parseItem(itemstr);
                if (poi != null) pois.Add(poi);
                pos = getpos(mstr, "[", p.p2, 1);
                             
                if (pos == -1) break;

            } while (true);
            return pois;
        }

        /// <summary>
        /// 解析一个poi的内容   //实体id，分级地址，全地址，带地址名称，名称，类型，联系电话，联系网站，经纬度，
        /// </summary>
        /// <param name="itemstr"></param>
        static googlepoi parseItem(string itemstr)
        {
            googlepoi poi = null;
            int pos = getpos(itemstr, "[", 1, 1);
            pairs p = getparis(itemstr, "[", "]", pos);

            List<string> items = new List<string>();

            //XXX,XXX,XXX
            string mstr = itemstr.Substring(p.p1 + 1, p.p2 - p.p1 - 1);
            int p0 = 0;
            int gap = 1;
            do
            {

                if (mstr.Substring(p0, 1) == "[")
                {
                    p = getparis(mstr, "[", "]", p0);
                    pos = getpos(mstr, ",", p.p2, 1);
                    gap = 1;
                }
                else if (mstr.Substring(p0).StartsWith("\\\""))
                {
                    pos = getpos(mstr, "\\\",", p0, 1);
                    gap = 3;
                }
                else {
                    pos = getpos(mstr, ",", p0, 1);
                    gap = 1;
                }
                if (pos == -1) break;
                items.Add(mstr.Substring(p0, pos - p0 + gap - 1));
                p0 = pos + gap;

            } while (true);
            if (items.Count > 10)
            {
                string id = rmChar(items[10]);
                string classaddress = rmChar(items[2]);
                string address = rmChar(items[39]);
                string fname = rmChar(items[18]);
                string name = rmChar(items[11]);
                string category = rmChar(items[13]);
                string phone = rmChar(items[3]);
                string website = rmChar(items[7]).Split(new char[] { ',' })[0];
                string lat = rmChar(items[9]).Split(new char[] { ',' })[2];
                string lon = rmChar(items[9]).Split(new char[] { ',' })[3];
                string close=(rmChar(items[23])=="")?"0":"1";
                poi = new googlepoi();

                poi.oid = id;
                poi.classaddress = classaddress;
                poi.address = address;
                poi.fname = fname;
                poi.name = name;
                poi.category = category;
                poi.phone = phone;
                poi.website = website;
                poi.lat = lat;
                poi.lon = lon;
                poi.closed = close;
              /*  Console.WriteLine("============================");
                
                Console.WriteLine(items[10]);//id
                Console.WriteLine(items[2]);//分级的地区
                Console.WriteLine(items[39]);//全地址

                Console.WriteLine(items[18]);//带地址的名称

                Console.WriteLine(items[11]);//名称
                Console.WriteLine(items[13]);//类型
                Console.WriteLine(items[3]);//电话
                Console.WriteLine(items[7]);//网址
                Console.WriteLine(items[9]);//坐标
              
                Console.WriteLine(id);
                Console.WriteLine(classaddress);
                Console.WriteLine(address);
                Console.WriteLine(fname);
                Console.WriteLine(name);
                Console.WriteLine(category);
                Console.WriteLine(phone);
                Console.WriteLine(website);
                Console.WriteLine(lat);
                Console.WriteLine(lon);

                Console.WriteLine("============================");
                Console.WriteLine("");  */
            }  /*
             \"0x344913d7cbe940b3:0xbc354b37ef810e6e\"
            [\"中国\",\"浙江省金华市婺城区\",\"中村路235号\"]\n
            \"中国浙江省金华市婺城区中村路235号\"
            \"浙江省金华市婺城区中村路235号春来茶楼\"
            \"春来茶楼\"
            [\"餐馆\"]\n
            [\"+86 400 716 1717\"]\n
            ["http:\/\/www.iigs.edu.pk\/","iigs.edu.pk",null,null,"1,AFQjCNG-AtpxRemXA4UT9AP1r2RWiGigFg,,0ahUKEwiA6cuP8M3KAhWHL6YKHdSBAGIQ61gICSgEMAA,,"]
            [null,null,29.089699,119.658908]\n
            
            */
            return poi;

        }

        /// <summary>
        /// 移除字符串中的无用字符。
        /// </summary>
        /// <param name="sv"></param>
        /// <returns></returns>
        static string rmChar(string sv)
        {
            string rv = sv.Replace(@"\n", "").Replace(@"[", "").Replace(@"]", "").Replace("\"", "").Replace("\\u0026", "&").Replace("\\", "");
            if (rv.ToLower() == "null") rv = "";
            return rv;
        }

        /// <summary>
        /// 获取字符串在另一个字符串中第n次出现的位置。
        /// </summary>
        /// <param name="str">基础字符串</param>
        /// <param name="cstr">检查的字符串</param>
        /// <param name="p1">检查的开始位置</param>
        /// <param name="count"></param>
        /// <returns></returns>
        static  int getpos(string str, string cstr, int p1, int count)
        {
            int idx = p1;
            int posc = 0;

            idx = str.IndexOf(cstr, p1);
            if (idx != -1) posc = 1;
            else return idx;
            if (posc == count) return idx;
            else {
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

        /// <summary>
        /// 获取成对的字符的开始，结束位置。
        /// </summary>
        /// <param name="str">查询的字符串</param>
        /// <param name="rtag">左半边符号</param>
        /// <param name="ltag">。。</param>
        /// <param name="p1">查询的启示位置</param>
        /// <returns></returns>
        static pairs getparis(string str, string rtag, string ltag, int p1)
        {
            pairs rv = new pairs(p1, p1);
            int p2 = p1;
            string mstr = "";

            do
            {
                p2 = str.IndexOf(ltag, p2);
                mstr = str.Substring(p1, p2 - p1 + 1);
                if (strcount(mstr, ltag) == strcount(mstr, rtag)) break;
                else p2++;
            } while (true);
            rv.p2 = p2;
            return rv;
        }

        /// <summary>
        /// 获取一个字符串在另一个字符串中出现的次数
        /// </summary>
        /// <param name="str">基础字符串</param>
        /// <param name="cstr">待检查字符串</param>
        /// <returns></returns>
        static int strcount(string str, string cstr)
        {
            int posc = 0;
            int idx = 0;
            idx = str.IndexOf(cstr);
            if (idx != -1) posc = 1;
            idx += cstr.Length;
            do
            {
                idx = str.IndexOf(cstr, idx);
                if (idx != -1) posc++;
                else break;
                idx += cstr.Length;
            } while (true);
            return posc;
        }
    }
}
