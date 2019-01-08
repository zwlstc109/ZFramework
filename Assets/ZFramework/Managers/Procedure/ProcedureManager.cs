using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using System.Reflection;
namespace Zframework
{

    public sealed class ProcedureManager : BaseManager
    {
        [SerializeField]
        private string[] m_AvailableProcedureTypeNames = null;
        [SerializeField]
        private string m_EntranceProcedureTypeName = null;
        protected override int MgrIndex { get { return (int)ManagerIndex.Procedure; } }
        private int mProcedureFsmId;
        public ProcedureBase CurrentProcedure { get { return Z.Fsm.GetFsmCurrentState(mProcedureFsmId) as ProcedureBase; } }
        internal override void Init()
        {
            //Z.Debug.Log("ProcedureManager init");
            Z.Procedure = this;

            ProcedureBase[] procedures = new ProcedureBase[m_AvailableProcedureTypeNames.Length];
            ProcedureBase entrance = null;
            for (int i = 0; i < m_AvailableProcedureTypeNames.Length; i++)
            {
                Type procedureType = Z.Assembly.GetType(m_AvailableProcedureTypeNames[i]);
                if (procedureType == null)
                {
                    Z.Debug.ErrorFormat("无法在程序集中找到此流程 '{0}'.", m_AvailableProcedureTypeNames[i]);
                    return;
                }

                procedures[i] = (ProcedureBase)Activator.CreateInstance(procedureType);
                if (procedures[i] == null)
                {
                    Z.Debug.ErrorFormat("无法实例化流程实例 '{0}'.", m_AvailableProcedureTypeNames[i]);
                    return;
                }

                if (m_EntranceProcedureTypeName == m_AvailableProcedureTypeNames[i])
                {
                    entrance = procedures[i];
                }
            }

            if (entrance == null)
            {
                Z.Debug.ErrorFormat("流程无法启动：请在Procedure节点上设置初始流程");
                return;
            }
            mProcedureFsmId = Z.Fsm.RegistFsm(procedures, this, false);
            Z.Fsm.GetFsm(mProcedureFsmId).StartFsm(entrance);
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