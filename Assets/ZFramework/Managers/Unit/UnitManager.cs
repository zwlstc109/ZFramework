using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{

    public class UnitManager : BaseManager
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Unit; } }
        //为了快速获取池中的unit,必须要一个字典
        internal Dictionary<string, Stack<Unit>> mUnitPool = new Dictionary<string, Stack<Unit>>();
        [SerializeField]internal Transform poolRoot=null;
        

        private List<UnitGroup> mUnitGroupLst = new List<UnitGroup>();
        internal override void Init()
        {
            Z.Unit = this;
            Z.Pool.RegisterClassPool<Unit>(500);
            Z.Obs.ForLoop((int)BuiltinGroup.Count, _=>RegistUnitGroup());//TODO 资源组优化
           
        }
        public int RegistUnitGroup()
        {
            mUnitGroupLst.Add(new UnitGroup());
            return mUnitGroupLst.Count - 1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="unitGroupIndex"></param>
        /// <returns>GameObject包装类</returns>
        public Unit LoadUnit(string path,int unitGroupIndex=0)
        {
            if (unitGroupIndex>=mUnitGroupLst.Count)
            {
                Z.Debug.Error("Unit显示失败:没有注册过的Unit组");
                return null;
            }
            var unitGroup = mUnitGroupLst[unitGroupIndex];
            var unit = _GetUnitFromPool(path);           
            if (unit == null)
            {                
                var resItem = Z.Resource.LoadResourceItem<GameObject>(path, unitGroup.prefabGroupIndex);               
                unit = Z.Pool.Take<Unit>();
                unit.Group = unitGroup;
                unit.ResItem = resItem;
                unit.GO = Instantiate(resItem.Asset as GameObject);
                unit.GO.transform.Reset();//测试
            }
            unit.AddTo(unitGroup);//不管是不是从池中获取都要重新加入到unit组中
            return unit;
        }
        public void Release(int unitGroupIndex,bool intoPool = false)
        {
            if (unitGroupIndex >= mUnitGroupLst.Count)
            {
                Z.Debug.Error("释放Unit组失败:没有注册过的Unit组");
                return;
            }
            mUnitGroupLst[unitGroupIndex].ReleaseGroup(intoPool);
        }


        private Unit _GetUnitFromPool(string path)
        {
            var lst = mUnitPool.GetValue(path);
            if (lst!=null&&lst.Count>0)
            {
                var unit = lst.Pop();
                unit.LeaveFromPoolNotify();
                unit.GO.transform.SetParent(transform);
                return unit;
            }
            return null;
        }

        internal override void MgrUpdate()
        {
            
        }

        internal override void ShutDown()
        {
           
        }


    }

    public class Unit
    {
        internal ResourceItem ResItem;
        public GameObject GO;
        internal UnitGroup Group;
        //通知自己的组，自己被从池(Dic)中取走
        internal void LeaveFromPoolNotify()
        {
            Group.PoolListener(this);
        }
        /// <summary>
        /// 释放自己直接入池  
        /// </summary>
        public void ReleaseSelf()//入池和setActiveFalse的区别 前者可以在load请求时被拿走
        {
            Group.ReleaseOne(this);
        }
    }

    public class UnitGroup
    {
        public int prefabGroupIndex;
        private List<Unit> mUnitLst = new List<Unit>();
        private List<Unit> mUnitsInPool = new List<Unit>();
        internal UnitGroup()
        {
            prefabGroupIndex = Z.Resource.RegistGroup();
        }
        internal void Add(Unit unit)
        {
            mUnitLst.Add(unit);
        }
        //监听是否有unit从池(那个Dic)中取走 
        internal void PoolListener(Unit unit)
        {
            mUnitsInPool.Remove(unit);//TODO 遍历不是一种很好的味道
        }
        internal void ReleaseOne(Unit unit,bool destroy=true)
        {
            mUnitLst.Remove(unit);//TODO 现在两边都要遍历了... 在观察实际性能表现后 可以考虑增加dic 
            if (unit.GO==null)
            {
                Z.Debug.Warning("要释放的GO为空，是否手动释放过？");
                return;
            }
            if (destroy)
                UnityEngine.Object.Destroy(unit.GO);                        
            else if(unit.GO!=null)
                _ReleaseOne(unit);
           
        }
        internal void ReleaseGroup(bool destroy=true)
        {
            if (!destroy)
            {
                for (int i = mUnitLst.Count-1; i >=0 ; i--)
                {
                    var unit = mUnitLst[i];
                    _ReleaseOne(unit);
                    mUnitLst.RemoveAt(i);
                }
            }
            else
            {
                for (int i = mUnitLst.Count - 1; i >= 0; i--)
                {
                    var unit = mUnitLst[i];
                    UnityEngine.Object.Destroy(unit.GO);
                    mUnitLst.RemoveAt(i);
                    Z.Pool.Return(ref unit);//类池归还
                }
                for (int i = mUnitsInPool.Count - 1; i >= 0; i--)
                {
                    var unit = mUnitsInPool[i];
                    if (Z.Unit.mUnitPool.ContainsKey(unit.ResItem.Path))
                    {
                        Z.Unit.mUnitPool.Remove(unit.ResItem.Path);
                    }
                    UnityEngine.Object.Destroy(unit.GO);
                    mUnitsInPool.RemoveAt(i);
                    Z.Pool.Return(ref unit);
                }
                Z.Resource.Release(prefabGroupIndex, true);
            }

        }

        private void _ReleaseOne(Unit unit)
        {
            if (unit.GO==null)
            {
                Z.Debug.Warning("是否手动调用过Destory?");
                return;
            }
            var stack = Z.Unit.mUnitPool.GetValue(unit.ResItem.Path);
            if (stack == null)
            {
                stack = new Stack<Unit>();
                Z.Unit.mUnitPool[unit.ResItem.Path] = stack;
            }
            stack.Push(unit);
            mUnitsInPool.Add(unit);
            unit.GO.transform.SetParent(Z.Unit.poolRoot);           
        }
    }
}