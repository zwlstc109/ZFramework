using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{  //由于所有的池要交给PoolManager中的一个字典管理，所以IPool就不带泛型了 转型的工作交给PoolManager
    public interface IPool 
    {
        object Spawn();
        void Despawn(object t);
        void ClearAll();
        int Count { get; }//测试使用
    }
    public interface IFactory<T>
    {
        T Create();
    }
    public class CustomFactory<T> : IFactory<T>
    {
        private Func<T> mFactoryMethod;
        public CustomFactory(Func<T> factoryMethod)
        {
            mFactoryMethod = factoryMethod;
        }

        public T Create()
        {
            return mFactoryMethod();
        }
    }
    public class ClassPool<T> : IPool where T:class,new()
    {
        protected Stack<T> mStack = new Stack<T>();
        protected IFactory<T> mFactory;
        public int Count { get { return mStack.Count; }}
        protected int mHoldAmount;//保有数量
        protected int mSpinLockRoot=0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialAmount">初始数量</param>
        /// <param name="holdAmount">保有数量</param>
        internal ClassPool(int initialAmount=20,int holdAmount=-1)
        {
            mFactory = new CustomFactory<T>(() => new T());
            Z.Obs.ForLoop(initialAmount, i => mStack.Push(mFactory.Create()));
            mHoldAmount = holdAmount == -1 ? initialAmount : holdAmount;
        }

        public void ClearAll()
        {
            Z.Lock.LOCK(ref mSpinLockRoot);
            mStack.Clear();
            Z.Lock.UNLOCK(ref mSpinLockRoot);
        }
                             
        public object Spawn()//由于Pool没有公共构造函数，所以只能调PoolManager的接口进行使用，返回对象的转型交给PoolManager
        {
            T t;
            Z.Lock.LOCK(ref mSpinLockRoot);
            t = Count > 0 ? mStack.Pop() : mFactory.Create();
            var z = t as ZObject;
            if (z != null)
            {
                z.destroy = false;
            }
            Z.Lock.UNLOCK(ref mSpinLockRoot);
            //Z.Log.Log(CurCount);
            return t;
        }

        public virtual void Despawn(object t)
        {

            Z.Lock.LOCK(ref mSpinLockRoot);
            if (Count < mHoldAmount)
            {
                var z = t as ZObject;
                if (z!=null)
                {
                    z.destroy = true;
                }

                mStack.Push(t as T);
            }
            Z.Lock.UNLOCK(ref mSpinLockRoot);

            //Z.Log.Log(CurCount);
        }
    }
    //一种自定义池 可以指定生成物体的方法 和清洗方法
    public class CustomClassPool<T> : ClassPool<T> where T:class,new()
    {
        protected Action<T> mCleanMethod;
        public CustomClassPool(Func<T> factoryMethod,Action<T> cleanMethod=null, int initialAmount = 0, int holdAmount = -1):base(initialAmount,holdAmount)
        {
            mFactory = new CustomFactory<T>(factoryMethod);
            mCleanMethod = cleanMethod;
        }
        public override void Despawn(object t)
        {
            Z.Lock.LOCK(ref mSpinLockRoot);
            if (Count < mHoldAmount)
            {
                mCleanMethod?.Invoke(t as T);
                var z = t as ZObject;
                if (z != null)
                {
                    z.destroy = false;
                }
                mStack.Push(t as T);
            }

            Z.Lock.UNLOCK(ref mSpinLockRoot);
        }
    }
   

   
}