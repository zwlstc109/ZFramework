using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    
    public abstract class FsmState
    {
        public Fsm Owner { get;  set; }
        public string StateName { get; set; }
        public FsmState()
        {
            
        }

        public virtual void OnInit()
        {

        }
        public virtual void OnEnter(object userData=null)
        {

        }

        public virtual void OnUpdate()
        {

        }
        public virtual void OnLeave()
        {

        }
        public virtual void OnDestory()
        {

        }
        public void ChangeState<T>()where T : FsmState
        {
            Owner.ChangeState<T>();
        }
        public void ChangeState(string stateName)
        {
            Owner.ChangeState(stateName);
        }
    }
}