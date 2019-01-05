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
                ResourceItem resItem = Z.Pool.Take<ResourceItem>();
                resItem.Path = element.Path;
                resItem.AssetName = element.AssetName;
                resItem.ABName = element.ABName;
                resItem.DependAB = element.DependAB;
                Z.Resource.ResourceItemDic.Add(resItem.Path, resItem);
                //if (Z.Resource.ResourceItemDic.ContainsKey(resItem.Path))
                //{   //路径生成了一样的crc 可能性较小 但不保证不发生
                //    Z.Log.ErrorFormat("相同的Crc!! 资源名:{0}ab包名:{1}    资源名:{2}ab包名:{3}" ,resItem.AssetName,resItem.ABName, Z.Resource.ResourceItemDic[resItem.Crc].AssetName, Z.Resource.ResourceItemDic[resItem.Crc].ABName);
                //    return;
                //}
                //else
                //{
                   
                //}
            }
            //预留200个AB包壳
            Z.Pool.RegisterClassCustomPool(() => new AssetBundleItem(), AssetBundleItem.Clean, 200);
            return ;
        }

        /// <summary>
        /// 加载资源所在AB包和依赖包
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        internal static bool LoadResourceAB(ResourceItem resItem)
        {           
            //如果资源壳里的包是加载过的
            if (resItem.AssetBundle != null)
            {
                return true;
            }
            //加载依赖
            if (resItem.DependAB != null)
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
}