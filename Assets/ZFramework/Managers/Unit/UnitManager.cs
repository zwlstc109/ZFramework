using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{

    public class UnitManager : BaseManager //TODO 异步加载
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Unit; } }
        //为了快速获取池中的unit,必须要一个字典
        internal Dictionary<string, Stack<Unit>> mUnitPool = new Dictionary<string, Stack<Unit>>();
        [SerializeField]internal Transform poolRoot=null;
        

        private List<UnitGroup> mUnitGroupLst = new List<UnitGroup>();
        internal override void Init()
        {
            Z.Unit = this;
            Z.Pool.RegisterClassPool<Unit>(1000);
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
                var resItem = Z.Resource.LoadResourceItem<GameObject>(path, unitGroup.prefabGroupIndex);//每一个resItem都会被加入到资源组中（不管他所在的unit发生什么变故，他总会在组释放的时候正确的扣掉引用计数）               
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
            if (lst!=null)
            {           
                while (lst.Count > 0)//在池中的unit只是unit组中的'残影' 可能在入池后被整组释放（释放后归入类池，内存被clean）（而入池前空GO的unit会被排除） 所以用一个while循环 直到找到有GO的unit为止
                {
                    var unit = lst.Pop();//此unit只是残影 出栈后不需要归还到类池
                    if (unit.GO != null)
                    {
                        var another = Z.Pool.Take<Unit>();
                        unit.Move(another);//把空的unit留在组中lst的原地 统一遍历时会处理  --到处充满留空操作 是为了消除在容器中删除的开销
                        another.GO.transform.SetParent(transform,false);
                        return another;
                    }
                }
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
    /// <summary>
    /// GameObject包装类 存有实例化过的GO
    /// </summary>
    public class Unit:ZObject
    {
        internal ResourceItem ResItem;
        public GameObject GO;
        //所在的unit组
        internal UnitGroup Group;
        
        /// <summary>
        /// 释放自己直接入池  
        /// </summary>
        public void ReleaseSelf(bool destroy)//入池和setActiveFalse的区别 前者可以在load请求时被拿走
        {
            Group.ReleaseOne(this,destroy);
        }
        /// <summary>
        /// 转移自己的数据到另一个Unit 并置空自己
        /// </summary>
        /// <param name="another"></param>
        internal void Move(Unit another)
        {
            another.ResItem = ResItem;//这个赋值似乎没有必要，不管unit有什么变故，resItem已经在GO加载时就被添加进资源组了
            another.GO = GO;
            another.Group = Group;

            ResItem = null;
            GO = null;
        }
    }

    public class UnitGroup
    {
        public int prefabGroupIndex;
        private List<Unit> mUnitsOutPool = new List<Unit>();
        private List<Unit> mUnitsInPool = new List<Unit>();
        internal UnitGroup()
        {
            prefabGroupIndex = Z.Resource.RegistGroup();
        }
        internal void Add(Unit unit)
        {
            mUnitsOutPool.Add(unit);
        }
        
       
       /// <summary>
       /// 很舒服的释放接口
       /// </summary>
       /// <param name="unit"></param>
       /// <param name="destroy"></param>
        internal void ReleaseOne(Unit unit,bool destroy=true) //提前释放 
        {
            //mUnitLst.Remove(unit);//不删（不是链表，中间删除，消耗巨大） 留着空壳 等统一释放时再处理
            if (unit.GO==null)
            {
                Z.Debug.Warning("要释放的GO为空，是否手动释放过？");
                return;
            }
            if (destroy)
                UnityEngine.Object.Destroy(unit.GO);
            else if (unit.GO != null)
            {
                var another = Z.Pool.Take<Unit>();
                unit.Move(another);                    
                _PutIntoPool(another);
            }
           
        }
        internal void ReleaseGroup(bool destroy=true)
        {
            if (!destroy)
            {
                for (int i = mUnitsOutPool.Count-1; i >=0 ; i--)
                {
                    var unit = mUnitsOutPool[i];
                    _PutIntoPool(unit);
                    mUnitsOutPool.RemoveAt(i);
                }
            }
            else
            {
                for (int i = mUnitsOutPool.Count - 1; i >= 0; i--)
                {
                    var unit = mUnitsOutPool[i];
                    if (unit.GO!=null)//判空是需要的 因为有的unit会提前调用releaseSelf
                        UnityEngine.Object.Destroy(unit.GO);
                    mUnitsOutPool.RemoveAt(i);
                    Z.Pool.Return(ref unit);//类池归还
                }
                //
                for (int i = mUnitsInPool.Count - 1; i >= 0; i--)
                {
                    var unit = mUnitsInPool[i];
                    if (unit.GO!=null)//判空是需要的 因为有的unit会提前调用releaseSelf
                    {
                        //if (Z.Unit.mUnitPool.ContainsKey(unit.ResItem.Path))
                        //{
                        //    Z.Unit.mUnitPool.Remove(unit.ResItem.Path);
                        //}
                        UnityEngine.Object.Destroy(unit.GO);
                    }                  
                    mUnitsInPool.RemoveAt(i);
                    Z.Pool.Return(ref unit);
                }
                Z.Resource.Release(prefabGroupIndex, true);
            }

        }

        private void _PutIntoPool(Unit unit)
        {
            if (unit.GO==null)
            {
                Z.Debug.Warning("提前调用过Destory?");//也可能提前调用过releaseSelf
                Z.Pool.Return(ref unit);
                return;
            }
            var stack = Z.Unit.mUnitPool.GetValue(unit.ResItem.Path);
            if (stack == null)
            {
                stack = new Stack<Unit>();//TODO 类池 避免GC 好像也不用 因为不会去移除字典的pair
                Z.Unit.mUnitPool[unit.ResItem.Path] = stack;
            }
            //实际储存Unit
            stack.Push(unit);
            mUnitsInPool.Add(unit);
            unit.GO.transform.SetParent(Z.Unit.poolRoot,false);           
        }
    }
}