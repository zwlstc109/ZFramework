using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using System.Reflection;
namespace Zframework
{

    public class ProcedureManager : BaseManager
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Procedure; } }
        private int mProcedureFsmId;
        internal override void Init()
        {
            Z.Log.Log("ProcedureManager init");
            Z.Procedure = this;

            var states = new FsmState[3]
            {
                new ProcedureLaunch(),
                new ProcedureMainMenu(),
                new ProcedurePlay()
            };

            mProcedureFsmId = Z.Fsm.CreateFsm(states, this);


        }
        public void ChangeProcedure<T>()where T : ProcedureBase
        {
            Z.Fsm.ChangeFsmState<T>(mProcedureFsmId);
        }
        internal override void MgrUpdate()
        {
            
        }

        internal override void ShutDown()
        {
            throw new NotImplementedException();
        }
    }
}