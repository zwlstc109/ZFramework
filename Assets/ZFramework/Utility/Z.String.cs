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
            //提供全局的一个StringBuilder  但线程不安全
            private static System.Text.StringBuilder Builder = new System.Text.StringBuilder(200);
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
            public static string Fmt(string format, params object[] param)
            {
                string str = Builder.AppendFormat(format, param).ToString();
                Builder.Length = 0;
                return str;
            }
            public static string InternFmt(string format, params object[] param)
            {
                string str = Builder.AppendFormat(format, param).ToInternString();
                Builder.Length = 0;
                return str;
            }

        }

        //public class StringBuilder 
        //{


        //    private  System.Text.StringBuilder mStringBuilder = new System.Text.StringBuilder();

        //}
    }
}