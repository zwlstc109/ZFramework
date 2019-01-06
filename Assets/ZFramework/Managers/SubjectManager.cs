using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{   
    public abstract class SubjectArgs
    {
        public object sender = null;
        public abstract int SubjectId { get; }
    }

    public class SubjectManager : BaseManager
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Subject; } }

        internal override void Init()
        {
            //Z.Debug.Log("SubjectManager init");
            Z.Subject = this;
        }
        //用于轻量级无装箱版的字典的字典， 通过类型 找到对应的字典
        private Dictionary<Type, IDictionary> mDicDic = new Dictionary<Type, IDictionary>();
        //private SpinLock mSpinLock = new SpinLock();//不是很懂这个锁，还没自己写的简单锁好用...
        private SimpleSpinLock mSpinLock = new SimpleSpinLock();
        //private bool mSpinTaken=false;
        public SubjectManager()
        {
            mDicDic.Add(typeof(int), mIntSubjectDic);
            mDicDic.Add(typeof(float), mFloatSubjectDic);
            mDicDic.Add(typeof(bool), mBoolSubjectDic);
        }

        #region 重量级事件
        //相对重量级的管理，每种事件都需要一个类来定义，类本身作为事件的参数类型
        private Dictionary<int, Subject<SubjectArgs>> mSubjectDic = new Dictionary<int, Subject<SubjectArgs>>();

        public IObservable<T> GetSubject<T>() where T : SubjectArgs
        {
            //mSpinLock.Enter(ref mSpinTaken);

            int subjectId = typeof(T).GetHashCode();
            var subject = mSubjectDic.GetValue(subjectId);
            if (subject == null)
            {
                mSpinLock.Enter();
                subject = mSubjectDic.GetValue(subjectId);
                if (subject == null)
                {
                    subject = new Subject<SubjectArgs>();
                    mSubjectDic.Add(subjectId, subject);
                }
                mSpinLock.Leave();
            }
            //mSpinLock.Exit();

            return subject.Select(_ => _ as T);
        }
        //public bool HasSubject<T>()
        //{
        //    int subjectId = typeof(T).GetHashCode();
        //    var subject = mSubjectDic.GetValue(subjectId);
        //    return subject != null;
        //}
        public void Fire<T>(T e) where T : SubjectArgs
        {
           
            mSubjectDic.GetValue(e.SubjectId)?.OnNext(e);
            Z.Pool.Return(ref e);
        }
        #endregion
        #region 轻量级

        //轻量级管理，string作为key ,object为事件参数,但存在装拆箱的问题 非常适合用来广播无参事件
        private Dictionary<string, Subject<object>> mObjSubjectDic = new Dictionary<string, Subject<object>>();
        public IObservable<object> GetSubject(string name)
        {
            var subject = mObjSubjectDic.GetValue(name);
            if (subject == null)
            {
                subject = new Subject<object>();
                mObjSubjectDic.Add(name, subject);
            }
            return subject.Select(_ => _);//隐藏一下subject 其实没必要 ...
        }
        public void Fire(string name, object param)
        {
            var subject = mObjSubjectDic.GetValue(name);
            if (subject == null)
            {
                return;
            }
            subject.OnNext(param);
        }
        #endregion
        #region 轻量级无装箱版

        private Dictionary<string, Subject<int>> mIntSubjectDic = new Dictionary<string, Subject<int>>();
        private Dictionary<string, Subject<float>> mFloatSubjectDic = new Dictionary<string, Subject<float>>();
        private Dictionary<string, Subject<bool>> mBoolSubjectDic = new Dictionary<string, Subject<bool>>();
        
        public IObservable<T> GetSubject<T>(string name)
        {
            var dic = mDicDic[typeof(T)] as Dictionary<string, Subject<T>>;
            var subject = dic.GetValue(name);
            if (subject == null)
            {
                subject = new Subject<T>();
                dic.Add(name, subject);
            }
            return subject.Select(_ => _);
        }
        public void Fire<T>(string name, T param)
        {
            var dic = mDicDic[typeof(T)] as Dictionary<string, Subject<T>>;
            var subject = dic.GetValue(name);
            if (subject == null)
            {
                return;
            }
            subject.OnNext(param);
        }
        #endregion

        internal override void MgrUpdate()
        {
           
        }

        internal override void ShutDown()
        {
            foreach (var pair in mSubjectDic)
            {
                pair.Value.Dispose();
            }
            foreach (var pair in mObjSubjectDic)
            {
                pair.Value.Dispose();
            }
            foreach (var pair in mIntSubjectDic)
            {
                pair.Value.Dispose();
            }
        }
    }
}