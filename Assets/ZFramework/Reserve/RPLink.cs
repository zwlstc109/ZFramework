using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
namespace Zframework
{
    //考虑那种参数不复杂的事件（比如没有参数） 可以用这个发
    public static class RPLink
    {
        private static Dictionary<string, BoolReactiveProperty> mBoolRpDic = new Dictionary<string, BoolReactiveProperty>();

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