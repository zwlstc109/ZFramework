using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace Zframework
{
    public static partial class Z
    {
        public static class Lock
        {
            //个人感觉书里的那个Morph并不好，目的是自旋，但是浪费了很多无谓的计算，【还不如直接用简易自旋锁】，自旋的线程还能被换走
            //public delegate TResult Morpher<TResult, TArgument>(TResult startVal, TArgument argVal);
            //public static TResult InterLockedOp<TResult, TArgument>(ref TResult target,TArgument argument,Morpher<TResult,TArgument> morpher ) where TResult : class
            //{
                
            //    TResult currentVal = target, startVal, desiredVal;
            //    //TResult result = default;
            //    do
            //    {
            //        startVal = currentVal;
            //        desiredVal = morpher(startVal, argument);
            //        currentVal = Interlocked.CompareExchange<TResult>(ref target, desiredVal, startVal);//交换时，target还是不是原来的startVal
            //       //Interlocked.CompareExchange
            //    } while (currentVal.Equals(startVal));
            //    return desiredVal;
            //}
           
            //public static void ModifyItem<T>(this List<T> collection,Predicate<T> predicate,T newItem) where T : class
            //{
            //   int index= collection.FindIndex(predicate);
            //    if (index == -1) return;
            //    InterLockedOp(ref collection[index], newItem, (i, n) => n);
            //}          
        }
    }
    /// <summary>
    /// 简易自旋锁
    /// </summary>
    public class SimpleSpinLock//CLR via C# 里的一个简易自旋锁 仅有一个线程进入Enter后可以逃离死循环（没有逃离的线程将在死循环中自旋），调用leave后 放开锁定 允许下一个线程逃离 ，操作系统无法检测到这种‘发呆’，于是不会阻塞（意味着线程不会被切换，也是自旋锁的优点），缺点是cpu在空转
    {
        private int mResource=0;
        //private int mCount=0;
        //public int LockedCount { get { return mCount; } }
        public void Enter()
        {
            //Interlocked.Increment(ref mCount);
            while (true)
            {
                if (Interlocked.Exchange(ref mResource, 1) == 0)
                {
                    //Interlocked.Decrement(ref mCount);
                    return;
                }
                Thread.Sleep(0);
            }        
        }
        public void Leave()
        {
            mResource = 0;
            //Debug.Log(mCount);
        }
    }
}

