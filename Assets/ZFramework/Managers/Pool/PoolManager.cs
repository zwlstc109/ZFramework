using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{

    public class PoolManager : BaseManager
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Pool; } }
        private Dictionary<string, IPool> mObjectPoolDic = new Dictionary<string, IPool>();

        internal override void Init()
        {
            Z.Pool = this;
        }

        /// <summary>
        /// 注册一个普通类池
        /// </summary>
        /// <typeparam name="T">池中存放的Object类型</typeparam>
        /// <param name="initialAmount">初始数量</param>
        /// <param name="holdAmount">保有数量</param>
        /// 建议只在主线程中注册池
        public void RegisterClassPool<T>(int initialAmount=20,int holdAmount=-1)where T : class, new()
        {
            string poolName = typeof(T).FullName;//fullName是留用的字符串
            var pool = mObjectPoolDic.GetValue(poolName);
            if (pool!=null)
            {
                Z.Log.Warning("重复注册Pool:" + typeof(T));
                return;
            }
            mObjectPoolDic.Add(poolName, new ClassPool<T>(initialAmount,holdAmount));
        }
        /// <summary>
        /// 注册一个自定义类池 池中对象可以指定创建方法 和清洗方法    
        /// </summary>
        /// <typeparam name="T">池中存放的Object类型</typeparam>
        /// <param name="factoryMethod">创建方法</param>
        /// <param name="cleanMethod"> 清洗方法</param>
        /// <param name="initialAmount">初始数量</param>
        /// <param name="holdAmount">保有数量</param>
        public void RegisterClassCustomPool<T>(Func<T> factoryMethod, Action<T> cleanMethod = null, int initialAmount = 0, int holdAmount = -1) where T : class, new()
        {
            string poolName = typeof(T).FullName;
            var pool = mObjectPoolDic.GetValue(poolName);
            if (pool != null)
            {
                Z.Log.Warning("重复初始化Pool:" + typeof(T));
                return;
            }
            mObjectPoolDic.Add(poolName, new CustomClassPool<T>(factoryMethod,cleanMethod,initialAmount, holdAmount));
        }
        /// <summary>
        /// 从池中拿取一个 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Take<T>() where T : class, new()
        {
            string poolName = typeof(T).FullName;
            var pool = mObjectPoolDic.GetValue(poolName);
            if (pool == null)//必须先在主线程中注册过，才能使用池
            {
                Z.Log.Warning("未初始化Pool:" + typeof(T));
                return null;
            }
            return pool.Spawn() as T;
        }
        /// <summary>
        /// 归还到池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void Return<T>(ref T t) where T : class
        {
            if (t == null)
            {
                Z.Log.Warning("试图往池中归还一个空引用");
                return;
            }
            string poolName = typeof(T).FullName;
            var pool = mObjectPoolDic.GetValue(poolName);
            if (pool == null)
            {
                Z.Log.Warning("未初始化Pool:" + typeof(T));
                return;
            }            
            pool.Despawn(t);
            t = null;//清空引用
        }

        internal override void MgrUpdate()
        {
          
        }

        internal override void ShutDown()
        {
            foreach (var pool in mObjectPoolDic)
            {
                pool.Value.ClearAll();
            }
        }
    }
}