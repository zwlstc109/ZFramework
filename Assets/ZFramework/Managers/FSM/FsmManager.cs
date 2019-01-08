using System;
using System.Collections.Generic;
using UnityEngine;
namespace Zframework
{

    public sealed class FsmManager : BaseManager
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Fsm; } }
        private List<Fsm> mFsmLst = new List<Fsm>();
        private Stack<int> mIdleFsmLst = new Stack<int>();//存放闲置的fsm 考虑fsm回收再利用
        internal override void Init()
        {
            //Z.Debug.Log("FSMManager init");
            Z.Fsm = this;
        }
        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <param name="states">状态们</param>
        /// <param name="owner">状态机持有者</param>
        /// <returns>状态机Id 可根据Id拿到此状态机</returns>
        public int RegistFsm(IEnumerable<FsmState> states, object owner,bool autoStart=true)
        {
            var fsm = new Fsm(states, mFsmLst.Count, owner,autoStart);//考虑FSM加个回收状态，Manager加个Id的容器，如果有闲置的Id就先用这个FSm 不new
            mFsmLst.Add(fsm);
            return mFsmLst.Count - 1;
        }
        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <param name="states">状态们</param>
        /// <param name="owner">状态机持有者</param>
        /// <returns>状态机Id 可根据Id拿到此状态机</returns>
        public int RegistFsm(IEnumerable<Tuple<string,FsmState>> states,object owner, bool autoStart = true)
        {
            var fsm = new Fsm(states,mFsmLst.Count,owner,autoStart);
            mFsmLst.Add(fsm);
            return mFsmLst.Count-1;
        }
        public Fsm GetFsm(int fsmId)
        {
            if (fsmId>=mFsmLst.Count)
            {
                Debug.Log("状态机Id越界");
                return null;
            }
            return mFsmLst[fsmId];
        }
        public FsmState GetFsmCurrentState(int fsmId)
        {
            return GetFsm(fsmId).CurState;
        }
        public void ChangeFsmState<T>(int fsmId)where T : FsmState
        {
            GetFsm(fsmId).ChangeState<T>();
        }
        internal override void MgrUpdate()
        {
            for (int i = 0; i <mFsmLst.Count ; i++)
            {
                mFsmLst[i].Update();
            }
        }

        internal override void ShutDown()
        {
           
        }
    }
}