using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    //想法 提供一种使用完毕自动回收(回收条件自定义 默认使用一次即回收)的类 
    //本质把本该散落在使用者处的变量 维护在这个类里
    public class ZCatridge:ZObject 
    {
        private Action mAction;
        bool used;
        public ZCatridge Fill(Action action)
        {
            mAction = action;
            used = false;
            return this;
        }
        public void Fire()
        {
            if (!used)
            {
                mAction.Invoke();
                used = true;
            }
        }

        //public static IObservable<T> Take<T>(Action action);d
        //{
        //    return Observable.Create()
        //}

    }
}