using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Text;
using System.Globalization;

namespace Zframework
{
    
    public static class Extenxion
    {
        #region String
        /// <summary> Ordinal Equals </summary>
        public static bool OEquals(this string lhs, string rhs) //经测试 这些方法都不如Equals 原因不明...先放着把
        {
            return lhs.Equals(rhs, StringComparison.Ordinal);
        }
        /// <summary>Ordinal_IgnoreCase Equals </summary>
        public static bool OIEquals(this string lhs, string rhs)
        {
            return lhs.Equals(rhs, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>Ordinal_IgnoreCase Equals</summary>
        public static int OICompareTo(this string lhs,string rhs)
        {
            return string.Compare(lhs, rhs,StringComparison.OrdinalIgnoreCase);
        }
        #endregion
        #region StringBuilde
        /// <summary> store into(or get from) hashTable and return  </summary>
        public static string ToInternString(this StringBuilder builder)
        {
            return string.Intern(builder.ToString());
        }//我感觉string.Intern这个方法很可笑 用已经有的字符串再去申请一个自己 这是在干嘛？
         //不过用这个申请出来的string 在Equals的时候 可以使用RefrenceEquals 大大减少字符串比对时间 可能这就是这个api最大的作用...
        #endregion

        #region 临时放放
        //public static T LoadResource<T>(this ResourceManager manager, string path,int groupIndex=0) where T : UnityEngine.Object
        //{
        //    return manager.LoadResource<T>(Crc32.GetCrc32(path),groupIndex);
        //}
        #endregion
    }
}