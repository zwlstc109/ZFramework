﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
namespace Zframework
{

    /// <summary>
    /// 资源管理
    /// </summary>
    public class ResourceManager : BaseManager
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Resource; } }
        //是否从AB加载
        public bool LoadFromAssetBundle = false;
        //所有资源的壳  壳里可能没资源(刚从清单读出引用，还未加载) 也可能有资源 可能引用计数大于0(缓存着) 也可能等于0(保留着)  正式卸载资源时 只置空壳里的asset 不移除pair
        //为加载资源服务
        internal Dictionary<string, ResourceItem> ResourceItemDic = new Dictionary<string, ResourceItem>();
        
        
        internal override void Init()
        {
            Z.Log.Log("ResourceManager init");
            Z.Resource = this;
            Z.Pool.RegisterClassCustomPool(() => new ResourceItem(), ResourceItem.Clean, 500);
            ResourceGroupManager.Init();
            if (LoadFromAssetBundle)
            {
                AssetBundleManager.LoadABManifest();//AB清单解析 先放这里 正式应该在流程启动后在流程里解析
            }          
        }
        public T LoadResource<T>(string path,int groupIndex = 0) where T : UnityEngine.Object
        {
            var resItem = LoadResourceItem<T>(path,groupIndex);
            if (resItem!=null)
            {
                return resItem.Asset as T;
            }
            return null;
        }
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源crc</param>
        /// <param name="groupIndex">请求的资源组别 默认0（过场自动清理）</param>
        /// <returns></returns>
        internal ResourceItem LoadResourceItem<T>(string path,int groupIndex=0)where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                Z.Log.Error("请求的资源地址为空");
                return null;
            }
            //获得内含资源的资源壳
            ResourceItem resItem = _GetCachedResItem(path,groupIndex);
            if (resItem != null)
            {
                return resItem;
            }         
#if UNITY_EDITOR
            if (!LoadFromAssetBundle)
            {               
                resItem = Z.Pool.Take<ResourceItem>();
                resItem.Asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                resItem.Path = path;
            }
#endif
            if (LoadFromAssetBundle)
            {
                resItem = ResourceItemDic.GetValue(path);//只要加载过清单 肯定会得到对应的壳
                if (resItem == null)
                {
                    Z.Log.Error("请求加载的资源不在清单内 "+path);
                    return null;
                }
                if (AssetBundleManager.LoadResourceAB(resItem))
                {
                    resItem.Asset = resItem.AssetBundle.LoadAsset<T>(resItem.AssetName);
                }
            }
            if (resItem.Asset != null)
            {             
                if (!LoadFromAssetBundle)//只有编辑器下加载资源需要添加这个pair
                {
                    ResourceItemDic.Add(path, resItem);
                }
                IncreaseRefCount(resItem,groupIndex);
                return resItem;
            }
            Z.Pool.Return(ref resItem);
            Z.Log.Error("加载资源失败");
            return null;
        }
        /// <summary>
        /// 注册资源组
        /// </summary>
        /// <typeparam name="T">资源组类型</typeparam>
        /// <returns>号牌</returns>
        public int RegistGroup<T>() where T : ResourceGroup, new()
        {
            return ResourceGroupManager.RegistGroup<T>();
        }

        public int RegistGroup() 
        {
            return RegistGroup<ResourceGroup>();
        }
        /// <summary>
        /// 清理资源组
        /// </summary>
        /// <param name="groupIndex"></param>
        /// <param name="destroyCache">是否真的卸载资源</param>
        public void Release(int groupIndex, bool destroyCache = true)
        {
            ResourceGroupManager.Release(groupIndex, destroyCache);
        }

        //private void cacheAsset(Object asset, ResourceItem resItem, uint crc)
        //{
        //    if (ResourceItemDic.ContainsKey(crc))
        //    {

        //    }
        //}

        /// <summary>
        /// 获得内含资源的资源壳
        /// </summary>
        /// <param name="path"></param>
        /// <param name="refCountAdd"></param>
        /// <returns></returns>
        private ResourceItem _GetCachedResItem(string path,int groupIndex ,int refCountAdd=1)
        {
            var resItem = ResourceItemDic.GetValue(path);
            if (resItem!=null&& resItem.Asset != null)
            {
                IncreaseRefCount(resItem,groupIndex,refCountAdd);
                return resItem;
            }
            return null;
        }
        internal void IncreaseRefCount(ResourceItem resItem,int groupIndex ,int refCountAdd=1)
        {
            resItem.RefCount += refCountAdd;
            resItem.LastUseTime= Time.realtimeSinceStartup;//先存着再说
            Z.Obs.ForLoop(refCountAdd, _ => resItem.AddTo(groupIndex));//考虑到释放资源组时的操作即减少引用计数，则哪里增加的就在哪里加入组中，这样可保证加了多少，等减掉的时候就能不多不少得减掉 应该没错..
        }
        //internal override void MgrUpdate()
        //{
           
        //}

        internal override void ShutDown()
        {
            ResourceGroupManager.ReleaseAll();
        }
    }
    /// <summary>
    /// 资源壳 记录了资源所在AB包、包依赖、引用计数等信息
    /// </summary>
    public class ResourceItem
    {
        ////资源路径的CRC  感觉不用存这个值 因为在字典中已经关联了 先放着
        //public uint Crc = 0;
        public string Path = string.Empty;
        //资源名字
        public string AssetName = string.Empty;
        //所在的AB 名字
        public string ABName = string.Empty;
        //依赖的AB
        public List<string> DependAB = null;
        //加载完的AB
        public AssetBundle AssetBundle = null;
        //-----------------------------------------------------
        /// <summary>持有的资源引用</summary>
        public UnityEngine.Object Asset = null;
        //资源唯一标识
        public int Guid = 0;
        //资源最后所使用的时间
        public float LastUseTime = 0.0f;
        //引用计数
        protected int mRefCount = 0;
        //是否跳场景清掉
        public bool Clear = true;
        public int RefCount
        {
            get { return mRefCount; }
            set
            {
                mRefCount = value;
                mRefCount = mRefCount < 0 ? 0 : mRefCount;
            }
        }
        public static void Clean(ResourceItem item)
        {
            //item.Crc = 0;
            item.Path = string.Empty;
            item.AssetName = string.Empty;
            item.ABName = string.Empty;
            item.DependAB = null;
            item.AssetBundle = null;

            item.Asset = null;
            item.Guid = 0;
            item.LastUseTime = 0.0f;
            item.mRefCount = 0;
        }
    }
  
}