using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
namespace ZframeworkReserve
{
    //考虑那种参数不复杂的事件（比如没有参数） 可以用这个发
    public static class RPLink
    {
        private static Dictionary<string, BoolReactiveProperty> mBoolRpDic = new Dictionary<string, BoolReactiveProperty>();
        private static Dictionary<string, ReactiveProperty<int>> mIntRpDic = new Dictionary<string, ReactiveProperty<int>>();
        public static ReactiveProperty<int> GetIntRp(string name)
        {
            ReactiveProperty<int> boolrp = null;
            if (!mIntRpDic.TryGetValue(name, out boolrp))
            {
                boolrp = new ReactiveProperty<int>(0);
                mIntRpDic.Add(name, boolrp);
            }
            return boolrp;
        }
        public static BoolReactiveProperty GetRp(string name, bool initialValueIfEmpty = true)
        {
            BoolReactiveProperty boolrp = null;
            if (!mBoolRpDic.TryGetValue(name, out boolrp))
            {
                boolrp = new BoolReactiveProperty(initialValueIfEmpty);
                mBoolRpDic.Add(name, boolrp);
            }
            return boolrp;
        }

        //事先约定的一些名字
        public static readonly string EventMaskEnable = "EventMaskEnable_todoList";
    }
}