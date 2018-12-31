using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
 
   /// <summary>
   /// 框架入口
   /// </summary>
    public static partial class Z
    {   //关于为何不直接写在UniRx源码里 因为考虑到耦合性 同时也能对自己到底加了哪些东西有所管理 
        /// <summary>
        /// Observable工具
        /// </summary>
        public static class Obs
        {
            #region 常用Observable
            private static IObservable<long> mEveryUpdate = UniRx.Observable.EveryUpdate();
            private static IObservable<long> mEveryFixedUpdate = UniRx.Observable.EveryFixedUpdate();
            /// <summary>鼠标左键的observable</summary>
            public static IObservable<long> LeftClick()
            {
                return mEveryUpdate.Where(_ => Input.GetMouseButtonDown(0));
            }
            /// <summary>鼠标右键的observable</summary>
            public static IObservable<long> RightClick()
            {
                return mEveryUpdate.Where(_ => Input.GetMouseButtonDown(1));
            }
            /// <summary>Update的observable</summary>
            public static IObservable<long> Update()
            {
                return mEveryUpdate;
            }
            /// <summary>FixedUpdate的observable</summary>
            public static  IObservable<long> FixedUpdate()
            {
                return mEveryFixedUpdate;
            }
            #endregion
            #region 计时器
            /// <summary>
            /// 在主线程立即运行工作项，可以用来在其他线程时给主线程传递要做的事情
            /// </summary>
            /// <param name="action"></param>
            public static void Timer(Action action)
            {
               Observable.Timer(TimeSpan.FromSeconds(0)).Subscribe(_=>action());
            }
            public static IObservable<long> Timer(float delay)
            {
                return Observable.Timer(TimeSpan.FromSeconds(delay));
            }
            public static IObservable<long> Timer(float dueTime, float period)
            {
                return Observable.Timer(TimeSpan.FromSeconds(dueTime), TimeSpan.FromSeconds(period));
            }
            #endregion
            #region for循环
            public static void ForLoop(int loopTime,Action<int> action)
            {
                Observable.Range(0, loopTime).Subscribe(_ => action(_));
            }
            #endregion
        }

    }
}