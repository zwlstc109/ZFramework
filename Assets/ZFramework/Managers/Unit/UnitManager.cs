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
        [SerializeField]internal Transform poolRoot;
        

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

        public GameObject LoadUnit(string path,int unitGroupIndex=0)
        {
            if (unitGroupIndex>=mUnitGroupLst.Count)
            {
                Z.Log.Error("Unit显示失败:没有注册过的Unit组");
                return null;
            }
            var unitGroup = mUnitGroupLst[unitGroupIndex];
            var unit = _GetUnitFromPool(path);           
            if (unit == null)
            {                
                var resItem = Z.Resource.LoadResourceItem<GameObject>(path, unitGroup.prefabGroupIndex);               
                unit = Z.Pool.Take<Unit>();
                unit.Group = unitGroup;
                unit.mResItem = resItem;
                unit.GO = Instantiate(resItem.Asset as GameObject);
                unit.GO.transform.Reset();//测试
            }
            unit.AddTo(unitGroup);//不管是不是从池中获取都要重新加入到unit组中
            return unit.GO;
        }
        public void Release(int unitGroupIndex,bool intoPool = false)
        {
            if (unitGroupIndex >= mUnitGroupLst.Count)
            {
                Z.Log.Error("释放Unit池失败:没有注册过的Unit组");
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
        internal ResourceItem mResItem;
        public GameObject GO;
        internal UnitGroup Group;
        //通知自己的组，自己被从池(Dic)中取走
        internal void LeaveFromPoolNotify()
        {
            Group.PoolListener(this);
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
            mUnitsInPool.Remove(unit);
        }
        internal void ReleaseGroup(bool destroy=true)
        {
            if (!destroy)
            {
                for (int i = mUnitLst.Count-1; i >=0 ; i--)
                {
                    var unit = mUnitLst[i];
                    var stack = Z.Unit.mUnitPool.GetValue(unit.mResItem.Path);
                    if (stack==null)
                    {
                        stack = new Stack<Unit>();
                        Z.Unit.mUnitPool[unit.mResItem.Path] = stack;
                    }
                    stack.Push(unit);
                    mUnitsInPool.Add(unit);
                    unit.GO.transform.SetParent(Z.Unit.poolRoot);
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
                }
                for (int i = mUnitsInPool.Count - 1; i >= 0; i--)
                {
                    var unit = mUnitsInPool[i];
                    if (Z.Unit.mUnitPool.ContainsKey(unit.mResItem.Path))
                    {
                        Z.Unit.mUnitPool.Remove(unit.mResItem.Path);
                    }
                    UnityEngine.Object.Destroy(unit.GO);
                    mUnitsInPool.RemoveAt(i);
                }
                Z.Resource.Release(prefabGroupIndex, true);
            }

        }
    }
}