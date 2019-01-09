using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Text;
using System;
namespace Zframework
{
    
    public static partial  class Z
    {
        /// <summary>
        /// 字符串工具
        /// </summary>
        public static class Str
        {
            [ThreadStatic]
            private static System.Text.StringBuilder Builder = new System.Text.StringBuilder(1024);
            //直接append没有堆垃圾，是推荐的做法，但是写代码的时候可能会很累,而且似乎没法用到格式化器，比如浮点数的格式化
            //public static StringBuilder Apd(int x) { return Builder.Append(x); }
            //public static StringBuilder Apd(float x) { return Builder.Append(x); }
            //public static StringBuilder Apd(bool x) { return Builder.Append(x); }
            //public static StringBuilder Apd(decimal x) { return Builder.Append(x); }
            //public static StringBuilder Apd(byte x) { return Builder.Append(x); }
            //public static StringBuilder Apd(double x) { return Builder.Append(x); }
            //public static StringBuilder Apd(long x) { return Builder.Append(x); }
            //public static StringBuilder Apd(short x) { return Builder.Append(x); }
            //public static StringBuilder Apd(string x) { return Builder.Append(x); }
            //public static StringBuilder Apd(char x,int repeat) { return Builder.Append(x,repeat); }
            //new  public static string ToString()
            //{
            //    string str = Builder.ToString();
            //    Builder.Length = 0;
            //    return str;
            //}
            public static string Fmt(string format,object arg0)
            {
                Builder.Length = 0;
                string str = Builder.AppendFormat(format, arg0).ToString();
                Builder.Length = 0;
                return str;
            }
            public static string Fmt(string format, object arg0, object arg1)
            {
                Builder.Length = 0;
                string str = Builder.AppendFormat(format, arg0,arg1).ToString();
                Builder.Length = 0;
                return str;
            }
            public static string Fmt(string format, object arg0, object arg1, object arg2)
            {
                Builder.Length = 0;
                string str = Builder.AppendFormat(format, arg0,arg1,arg2).ToString();
                Builder.Length = 0;
                return str;
            }
            public static string Fmt(string format, params object[] param)
            {
                Builder.Length = 0;
                string str = Builder.AppendFormat(format, param).ToString();
                Builder.Length = 0;
                return str;
            }
            /// <summary>
            /// 输出的字符串的地址固定 可用来RefrenceEquals
            /// </summary>
            /// <param name="format"></param>
            /// <param name="param"></param>
            /// <returns></returns>
            public static string InternFmt(string format, params object[] param)
            {
                Builder.Length = 0;
                string str = Builder.AppendFormat(format, param).ToInternString();
                Builder.Length = 0;
                return str;
            }
            /// <summary>
            /// 获取资源没有扩展名的名字
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static string GetAssetNoExtensionName(string path)
            {
                string temp = path.Remove(0, path.LastIndexOf('/') + 1);
                return temp.Remove(temp.LastIndexOf('.'));
            }

          
        }

        //public class StringBuilder 
        //{


        //    private  System.Text.StringBuilder mStringBuilder = new System.Text.StringBuilder();

        //}
    }
}