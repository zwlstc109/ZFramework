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
        public static class Rd
        {
            /// <summary>
            /// 100以内随机概率
            /// </summary>
            /// <param name="percent"></param>
            /// <returns></returns>
            public static bool HitPercent(int percent)
            {
                return UnityEngine.Random.Range(0, 100) < percent;
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