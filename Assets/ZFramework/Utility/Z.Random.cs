using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
namespace Zframework
{
    
    public static partial class Z
    {/// <summary>
     /// 随机工具
     /// </summary>
     /// 
        public static class Rd
        {
            private static System. Random rd = new System.Random();
            /// <summary>
            /// 100以内随机概率
            /// </summary>
            /// <param name="percent"></param>
            /// <returns></returns>
            public static bool HitPercent(int percent)
            {
                return UnityEngine.Random.Range(0, 100) < percent;
            }
            public static bool SysHitPercent(int percent)
            {
                return rd.Next(0, 100) < percent;
            }
            public static int SysNumber(int lhs,int rhs)
            {
                return rd.Next(lhs, rhs);
            }
            /// <summary>
            /// 随机选一个
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="items"></param>
            /// <returns></returns>
            public static T Choose<T>(params T[] items)
            {
                return items[UnityEngine.Random.Range(0, items.Length)];
            }

           
        }
       
    }
}