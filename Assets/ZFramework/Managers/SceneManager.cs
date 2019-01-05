using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.SceneManagement;
namespace Zframework
{  
    public enum SceneFadeMode
    {
        FadeIn,
        FadeOut
    }
    /// <summary>
    /// 场景管理 包括淡入淡出等幕布类功能
    /// </summary>
    public class SceneManager : BaseManager //TODO 现在场景没有结合AB包
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Scene; } }
        public AsyncOperation Async { get; private set; }
        public float LoadingProgress { get; private set; }
        //public string SceneName { get; private set; }
        public bool Already { get; private set; }
        private Action<object> mDoneCallback = null;
        public string LoadingUIPath = null;
        public string EmptyUIPath = null;
        private IDisposable mEmptyDispose;
        internal override void Init()
        {
            Z.Debug.Log("SceneManager init");
            Z.Scene = this;
        }
        /// <summary>
        /// 开启一个黑幕渐变 (期间锁定所有UI响应)
        /// </summary>
        /// <param name="mode">渐变模式</param>
        public void Fade(SceneFadeMode mode)
        {
            if (string.IsNullOrEmpty(EmptyUIPath))
            {
                Z.Debug.Warning("请在Scene节点下设置渐变面板路径");
                return;
            }
            switch (mode)
            {
                case SceneFadeMode.FadeIn:
                    Z.UI.Open(EmptyUIPath, Z.UI.Top, true, true);
                    break;
                case SceneFadeMode.FadeOut:
                    Z.UI.Open(EmptyUIPath, Z.UI.Top, true, false);
                    break;
                default:
                    break;
            }
        }
        public void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            Fade(SceneFadeMode.FadeOut);
        }

        public void LoadSceneAsync(string sceneName,Action<object> doneCallback=null,bool fade=true,object userData=null)
        {
            

            AsyncOperation asyncScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            asyncScene.allowSceneActivation = false;
            if (fade)
            {
                Fade(SceneFadeMode.FadeIn);
                mEmptyDispose=Z.Subject.GetSubject("Z_UIComplete").Subscribe(_ => { _LoadSceneAsync(asyncScene, doneCallback,fade, userData); mEmptyDispose.Dispose(); });
            }
            else
                _LoadSceneAsync(asyncScene, doneCallback,fade, userData);
        }
        private void _LoadSceneAsync(AsyncOperation asyncScene, Action<object> doneCallback ,bool fade ,object userData = null)
        {
            mDoneCallback = doneCallback;
            StartCoroutine(LoadAsync(asyncScene, userData,fade));
            Z.UI.Open(LoadingUIPath, Z.UI.Top);
        }
        public void SetLoadScenePath(string path)
        {
            
        }
        IEnumerator LoadAsync(AsyncOperation asyncScene, object userData,bool fade)
        {
     
            Already = false;

            //UnityEngine.SceneManagement.SceneManager.LoadScene("Empty", LoadSceneMode.Single);
           
            LoadingProgress = 0;
            int targetProgress = 0;
           
            if (asyncScene != null && !asyncScene.isDone)
            {

                while (asyncScene.progress < 0.9f)
                {
                    targetProgress = (int)asyncScene.progress * 100;//TODO 
                    yield return null;
                    //平滑过渡
                    while (LoadingProgress < targetProgress)
                    {
                        ++LoadingProgress;
                        yield return null;
                    }

                }

                ClearCache();
                asyncScene.allowSceneActivation = true;

                //自行加载剩余的10%
                targetProgress = 100;
                while (LoadingProgress < targetProgress - 2)
                {
                    ++LoadingProgress;
                    yield return null;
                }
                LoadingProgress = 100;
                Already = true;
                mDoneCallback?.Invoke(userData);
                mDoneCallback = null;
                Fade(SceneFadeMode.FadeOut);
            }
          
        }
        //转场清理默认资源组
        private void ClearCache()
        {
            Z.Resource.Release(0);
            Z.Unit.Release(0);
        }

        internal override void MgrUpdate()
        {
           
        }

        internal override void ShutDown()
        {
           
        }
    }
}