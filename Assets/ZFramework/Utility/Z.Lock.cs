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
            // 自旋锁

            /// <summary> 锁定 </summary>
            /// <param name="lockRoot">用于自旋锁的锁定节点（初始值必须为 0）</param>
            public static void LOCK(ref int lockRoot)
            {
                while (Interlocked.CompareExchange(ref lockRoot, 1, 0) == 1) Thread.SpinWait(20);
            }

            /// <summary> 解锁 </summary>
            /// <param name="lockRoot">用于自旋锁的锁定节点（初始值必须为 0）</param>
            public static void UNLOCK(ref int lockRoot) => Interlocked.Exchange(ref lockRoot, 0);
        }
    }
    
}

