using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{/// <summary>
/// 内置资源组 包括资源和Unit都内置这些索引   TODO　Unit组其实不用持有Audio
/// </summary>
    public enum BuiltinGroup 
    {
        Default,//默认组 过场清理
        Preload,//预加载组 手动清理
        UI,//UI组
        Audio,//音频
        Custom1,//自定义1
        Custom2,//自定义2
        Count
    }
    internal static class ResourceGroupManager
    {
        private static List<ResourceGroup> mGroupLst = new List<ResourceGroup>();
        internal static void Init()
        {
            Z.Obs.ForLoop((int)BuiltinGroup.Count, _ => mGroupLst.Add(new ResourceGroup()));
        }
        /// <summary>
        /// 注册一个资源组 使得可以统一释放对象
        /// </summary>
        /// <typeparam name="T">资源组类型</typeparam>
        /// <returns>一个号牌，一些接口需要号牌参数</returns>
        internal static int RegistGroup<T>() where T:ResourceGroup,new() //很喜欢这种方式 让用户保留一个int 来作为号牌
        {
            mGroupLst.Add(new T());
            return mGroupLst.Count - 1;
        }
        
        /// <summary>
        /// 加入资源组 以待释放
        /// </summary>
        /// <param name="resItem"></param>
        /// <param name="groupIndex"></param>
        internal static void AddTo(this ResourceItem resItem, int groupIndex)
        {
            if (resItem.Asset==null)
            {
                Z.Debug.Warning("空壳无法加入资源组 "+resItem.AssetName+" "+resItem.Path);
                return;
            }

            if (groupIndex>mGroupLst.Count-1)
            {
                groupIndex = 0;
                Z.Debug.Warning("申请加入的资源组不存在");
            }
            mGroupLst[groupIndex].Add(resItem);
        }

        internal static void Release(int groupIndex,bool destroyCache=true)
        {
            if (groupIndex > mGroupLst.Count - 1)
            {
                groupIndex = 0;
                Z.Debug.Warning("申请释放的资源组不存在");
            }
            mGroupLst[groupIndex].Release(destroyCache);
        }

        internal static void ReleaseAll()
        {
            for (int i = 0; i < mGroupLst.Count; i++)
            {
                mGroupLst[i].Release();
            }
        }
    }


    /// <summary>
    /// 主要用于统一卸载资源
    /// </summary>
    public class ResourceGroup
    {
        protected List<ResourceItem> mResItemLst = new List<ResourceItem>();
             
        public void Add(ResourceItem resItem)
        {
            mResItemLst.Add(resItem);
        }
        public virtual void Release(bool destroyCache=true)
        {
            for (int i = mResItemLst.Count-1; i >=0 ; i--)
            {
                var resItem = mResItemLst[i];
                resItem.RefCount--;
                if (resItem.RefCount>0)//同一种资源的壳只会留下一个供正式卸载
                {
                    mResItemLst.RemoveAt(i);                   
                }
                else// refcount==0
                {
                    if (destroyCache)
                    {
                        AssetBundleManager.ReleaseResource(resItem);//而AB包卸载的时候 相关的预制体和实体化的物体会被删除...
#if UNITY_EDITOR
                        if (resItem.Asset is GameObject)
                        {
                            //UnityEngine.Object.DestroyImmediate(resItem.Asset,true);//删掉预制体 并不会把实例化的实体删除... 
                        }
                        else
                        {
                            Resources.UnloadAsset(resItem.Asset);                         
                        }
                        if (!Z.Resource.LoadFromAssetBundle/*&&Z.Resource.ResourceItemDic.ContainsKey(resItem.Path)*/)
                        {
                            Z.Resource.ResourceItemDic.Remove(resItem.Path);
                            
                        }
                      
#endif
                        mResItemLst.RemoveAt(i);

                        resItem.Asset = null;
                        Resources.UnloadUnusedAssets();
#if UNITY_EDITOR  //TODO 两个一样的宏 很尴尬 
                        if (!Z.Resource.LoadFromAssetBundle/*&&Z.Resource.ResourceItemDic.ContainsKey(resItem.Path)*/)
                        {
                            Z.Pool.Return(ref resItem);

                        }                      
#endif
                    }
                }
              
            }
            
        }
    }
}