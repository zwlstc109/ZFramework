using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Zframework
{
    internal static class AssetBundleManager 
    {
        
        private static string mManifestName = "assetmanifest";
        private static string mABLoadPath = Application.streamingAssetsPath + "/";
        private static Dictionary<string, AssetBundleItem> mAssetBundleItemDic = new Dictionary<string, AssetBundleItem>();
        internal static readonly string TextureKeySuffix = "_Texture";
        //private static SimpleSpinLock mSpinLock = new SimpleSpinLock();//考虑到
        internal static void LoadABManifest()
        {
            //if (!Z.Resource.LoadFromAssetBundle)
            //    return;
            string mAssetManifestPath = mABLoadPath + mManifestName;

            AssetBundle manifestAB = AssetBundle.LoadFromFile(mAssetManifestPath);
            TextAsset manifestBytes = manifestAB.LoadAsset<TextAsset>(mManifestName);
            if (manifestBytes == null)
            {
                Z.Debug.Error("资源清单读取失败");
                return ;
            }
            MemoryStream stream = new MemoryStream(manifestBytes.bytes);
            BinaryFormatter bf = new BinaryFormatter();
            AssetManifest manifest = (AssetManifest)bf.Deserialize(stream);
            stream.Close();

            for (int i = 0; i < manifest.AssetLst.Count; i++)
            {   
                AssetElement element = manifest.AssetLst[i];
                _ResourceItemDicAdd(element.Path, element);
                if (element.AssetName.EndsWith(".png"))//还能增加多种扩展名识别 到时候再加
                    _ResourceItemDicAdd(element.Path + TextureKeySuffix, element);//为了让同一个路径可以分别作为sprite和texture的key值 所以在字典中会存在两个pair 当请求的资源类型是Texture2d，会自动添加后缀来匹配另一个pair
               
            }
            //预留200个AB包壳
            Z.Pool.RegisterClassCustomPool(()=>new AssetBundleItem(), AssetBundleItem.Clean, 200);
            Z.Pool.RegisterClassPool<ABLoadedArgs>(60);//一次加载请求可能同时需要加载多个包 这时候每个包都要发一次事件 所以给多点 但上小于上面的数字
            return ;
        }
        private static void _ResourceItemDicAdd(string key, AssetElement element)
        {
            ResourceItem resItem = Z.Pool.Take<ResourceItem>();
            resItem.Path = element.Path;
            resItem.AssetName = element.AssetName;
            resItem.ABName = element.ABName;
            resItem.DependAB = element.DependAB;
            resItem.ClearDependFlag();
            Z.Resource.ResourceItemDic.Add(key, resItem);
        }
        /// <summary>
        /// 加载资源所在AB包和依赖包
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        internal static bool LoadAssetBundle(ResourceItem resItem)
        {           
            //如果资源壳里的包是加载过的
            if (resItem.AssetBundle != null)//TODO 寻找需要这个判断的代码 明明可以那边自己判断
            {
                return true;
            }
            //加载依赖
            if (resItem.DependAB != null)//不用递归 已经在打包时就找出了所有依赖   依赖只是资源和包的关系 包和包之间没有依赖关系
            {
                for (int i = 0; i < resItem.DependAB.Count; i++)
                {
                    _LoadAssetBundle(resItem.DependAB[i]);
                }
            }
            resItem.AssetBundle = _LoadAssetBundle(resItem.ABName);

            return resItem.AssetBundle!=null;
        }

        /// <summary>
        /// 加载单个assetbundle根据名字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static AssetBundle _LoadAssetBundle(string name)
        {
            AssetBundleItem abItem = mAssetBundleItemDic.GetValue(name);
            if (abItem==null)
            {
                AssetBundle assetBundle = null;
                string fullPath = mABLoadPath + name;
                if (File.Exists(fullPath))
                {
                    assetBundle = AssetBundle.LoadFromFile(fullPath);// ---正式的加载
                }
                
                if (assetBundle == null)
                {
                    Debug.LogError("加载AB包失败:" + fullPath);
                    return null;
                }

                abItem = Z.Pool.Take<AssetBundleItem>();
                abItem.AssetBundle = assetBundle;                
                mAssetBundleItemDic.Add(name, abItem);
            }
            abItem.RefCount++;
            return abItem.AssetBundle;
        }
        //public static void LoadAssetBundleAsync(string name)
        //{
        //    Z.core.StartCoroutine(_LoadAssetBundleAsync(name));
        //}
        /// <summary>
        /// 异步加载AB包
        /// </summary>
        /// <param name="resItem"></param>
        /// <returns></returns>
        internal static void LoadAssetBundleAsync(ResourceItem resItem)//异步加载资源时 如果包很大 那加载包也应该异步 比如场景包 
        {
            resItem.ClearDependFlag();
            //加载依赖
            if (resItem.DependAB != null)
            {
               
                for (int i = 0; i < resItem.DependAB.Count; i++)
                {
                    string tempDependName = resItem.DependAB[i];
                    Z.core.StartCoroutine(_LoadAssetBundleAsync(tempDependName,resItem));
                    Z.Subject.GetSubject<ABLoadedArgs>().Where(args=>ReferenceEquals(args.LoadedName,tempDependName)).First().Subscribe(_ABLoadedCallBack);
                }
            }
            Z.core.StartCoroutine(_LoadAssetBundleAsync(resItem.ABName,resItem));
            Z.Subject.GetSubject<ABLoadedArgs>().Where(args => ReferenceEquals(args.LoadedName, resItem.ABName)).First().Subscribe(_ABLoadedCallBack);
        }
        private static void _ABLoadedCallBack(ABLoadedArgs args)
        {
            var resItem = args.ResItem;
            for (int i = 0; i < resItem.DependAB.Count; i++)
            {
                //当前循环到的依赖项名字 匹配了传来的 已完成加载的依赖包名字
                if (ReferenceEquals(resItem.DependAB[i], args.LoadedName))
                    resItem.DependLoadedFlag[i] = true;
                //有一个flag为false 就退出
                if (!resItem.DependLoadedFlag[i])
                    return;
            }           
            //最后当resItem自己的包加载好时 赋值AsyncLoaded   
            if(resItem.AssetBundle!=null)//如果没有这个判断的话 当最后一个依赖项加载完但本包还没加载好 就会错误的赋值true
                resItem.AnsycLoaded = true;
        }
        internal static IObservable<ABLoadedArgs> GetAssetBundleLoadedSubject(ResourceItem resItem)
        {
            return Z.Subject.GetSubject<ABLoadedArgs>().Where(args=>resItem==args.ResItem&&args.ResItem.AnsycLoaded).First();
        }
       
        private static IEnumerator _LoadAssetBundleAsync(string abName,ResourceItem resItem)
        {
            AssetBundleItem abItem = mAssetBundleItemDic.GetValue(abName);
            if (abItem == null)
            {
                AssetBundleCreateRequest abRequest = null;
                string fullPath = mABLoadPath + abName;
                if (File.Exists(fullPath))
                {
                    abRequest = AssetBundle.LoadFromFileAsync(fullPath);// ---正式的加载
                }
                while (!abRequest.isDone)//等待加载完成
                    yield return null;
                if (abRequest.assetBundle == null)
                {
                    Debug.LogError("加载AB包失败:" + fullPath);
                }
                else
                {
                    if (ReferenceEquals(abName,resItem.ABName))
                        resItem.AssetBundle = abRequest.assetBundle;

                    abItem = Z.Pool.Take<AssetBundleItem>();
                    abItem.AssetBundle = abRequest.assetBundle;
                    mAssetBundleItemDic.Add(abName, abItem); //协程是在主线程等待 因此这个操作并不需要加锁 ...又一次体现了协程的优越性
                }              
            }            
            abItem.RefCount++;
            //发送加载完成事件 接收方过滤name即可
            Z.Subject.Fire(Z.Pool.Take<ABLoadedArgs>().Fill(resItem,abName));
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="item"></param>
        internal static void ReleaseResource(ResourceItem item)
        {
            if (item == null)
            {
                Z.Debug.Warning("要卸载的资源为空");
                return;
            }
            if (item.DependAB != null && item.DependAB.Count > 0)
            {
                for (int i = 0; i < item.DependAB.Count; i++)
                {
                    _ReleaseAssetBundle(item.DependAB[i]);
                }
            }
            _ReleaseAssetBundle(item.ABName);
        }
        /// <summary>
        /// 释放AB包
        /// </summary>
        /// <param name="name">包名</param>
        private static void _ReleaseAssetBundle(string name)
        {
            AssetBundleItem abItem = mAssetBundleItemDic.GetValue(name);          
            if (abItem!=null)
            {
                //减少引用计数
                abItem.RefCount--;
                if (abItem.RefCount <= 0)
                {
                    if (abItem.AssetBundle == null)//不太可能发生...
                    {
                        Z.Debug.Warning("准备卸载AB包时，发现壳内的包引用为空");
                    }
                    abItem.AssetBundle.Unload(true);//这个卸载Unity做的很智能 会把所有引用这个assetBundle的变量都置空，不知道怎么做的... 
                    Z.Pool.Return(ref abItem);
                    mAssetBundleItemDic.Remove(name);
                }
            }
        }
        
        //private static void _AddToABDic(string name,AssetBundleItem abItem)
        //{
        //    mSpinLock.Enter();
        //    mAssetBundleItemDic.Add(name, abItem);
        //    mSpinLock.Leave();
        //}
        //private static void _RemoveFromABDic(string name)
        //{
        //    mSpinLock.Enter();
        //    mAssetBundleItemDic.Remove(name);
        //    mSpinLock.Leave();
        //}
    }
    
    /// <summary>
    /// AB包壳
    /// </summary>
    internal class AssetBundleItem:ZObject
    {
        internal AssetBundle AssetBundle = null;
        internal int RefCount;
        //static AssetBundleItem()
        //{
        //    Z.Pool.RegisterClassCustomPool<AssetBundleItem>(() => new AssetBundleItem(), Clean, 100);
        //}
        internal static void Clean(AssetBundleItem item)
        {
            item.AssetBundle = null;
            item.RefCount = 0;
        }
    }
    /// <summary>
    /// AB包加载完成事件参数 
    /// </summary>
    internal class ABLoadedArgs : SubjectArgs
    {
        public static int Id = typeof(ABLoadedArgs).GetHashCode();
        public override int SubjectId { get { return Id; } }
        public ResourceItem ResItem;
        public string LoadedName;
      
        public ABLoadedArgs Fill(ResourceItem resItem,string loadedName) 
        {
            ResItem = resItem;
            LoadedName = loadedName;
            return this;
        }
    }
}