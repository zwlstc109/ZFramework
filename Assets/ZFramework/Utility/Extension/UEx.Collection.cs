using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    
    public static partial class UnityExtension
    {
        public static Tvalue GetValue<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dic, Tkey key)
        {
            Tvalue value = default;
            dic.TryGetValue(key, out value);
            return value;
        }
        public static Tvalue GetValue<Tkey, Tvalue>(this ConcurrentDictionary<Tkey, Tvalue> dic, Tkey key)
        {
            Tvalue value = default;
            dic.TryGetValue(key, out value);
            return value;
        }

    }
}