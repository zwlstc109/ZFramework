using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    //public interface IFsm //考虑写两种状态机 一种普通的 状态不重复的， 另一种状态可重复
    //{
    //    void Update();
    //}
    /// <summary>
    /// 状态机
    /// </summary>
    /// <typeparam name="T">状态机持有者</typeparam>
    public class Fsm /*: IFsm */
    {
        private List<FsmState> mFsmStateLst = new List<FsmState>();//考虑到有限状态机状态就那么几个 没必要弄个字典 查找的时候遍历就行了 用stateName作为key来查找
        //private Dictionary<string, FsmState> mStateDic;
        public int FsmId { get; private set; }

        public FsmState CurState { get; private set; }
        public object Owner { get; private set; }
        /// <summary>
        /// 状态类名作为名字的状态机构建
        /// </summary>
        /// <param name="states"></param>
        /// <param name="fsmId"></param>
        /// <param name="owner"></param>
        internal Fsm(IEnumerable<FsmState> states,int fsmId,object owner):this(fsmId,owner)
        {
            foreach (var state in states)
            {
                string stateName = state.GetType().FullName/* + FsmId.ToString()*/;
                AddState(stateName, state);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="states">带名字的状态列表,名字要固定地址!!!</param>
        /// <param name="fsmId"></param>
        /// <param name="owner"></param>
        internal Fsm(IEnumerable<Tuple<string, FsmState>> states, int fsmId,object owner):this(fsmId,owner)
        {
            //mStateDic = new Dictionary<string, FsmState>();
            foreach (var stateTuple in states)
            {
                string stateName = stateTuple.Item1/*+FsmId.ToString()*/;
                AddState(stateName, stateTuple.Item2);
            }
        }
        private Fsm(int fsmId,object owner) { FsmId = fsmId;Owner = owner; }

        private void AddState(string stateName,FsmState state)
        {
            if (mFsmStateLst.Find(s =>ReferenceEquals(s.StateName, stateName)) == null)//地址比较的基础是名字字符串是固定地址!!
            {
                state.StateName = stateName;
                mFsmStateLst.Add(state);
                if (CurState == null)//tuples的第一个会被当作初始状态
                {
                    CurState = state;
                    state.OnEnter();
                }
                state.Owner = this;
                state.OnInit();
            }
            else
            {
                Z.Log.Error("状态机状态名字重复");
            }           
        }
        internal void ChangeState<T>()where T:FsmState//提供一种随手根据类型切换状态的方式
        {
            ChangeState(typeof(T).FullName);
        }
        internal void ChangeState(string stateName,object userData=null)
        {
            //string tempStateName = stateName + FsmId.ToString();
            var s= mFsmStateLst.Find(state => ReferenceEquals(state.StateName, stateName));//地址比较的基础是名字字符串是固定地址!!
            if (s != null)
            {
                CurState.OnLeave();
                CurState = s;
                CurState.OnEnter(userData);
            }
            else
            {
                Z.Log.Error("切换状态机状态失败：未找到对应状态 "+stateName);
            }
        }

        internal void Update()
        {
            if (CurState != null)
            {
                CurState.OnUpdate();
            }
        }

        internal void ShutDown()
        {
            CurState.OnLeave();
            for (int i = 0; i < mFsmStateLst.Count; i++)
            {
                mFsmStateLst[i].OnDestory();
            }
        }
       
    }
}