using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.SceneManagement;
namespace Zframework
{  


    public class SceneManager : BaseManager //TODO 现在场景没有结合AB包
    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Scene; } }
        public AsyncOperation Async { get; private set; }
        public float LoadingProgress { get; private set; }
        public string SceneName { get; private set; }
        public bool Already { get; private set; }
        private Action<object> mDoneCallback = null;
        public string LoadingUIPath = null;
        internal override void Init()
        {
            Z.Debug.Log("SceneManager init");
            Z.Scene = this;
        }
       
        public void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        public void LoadSceneAsync(string sceneName,Action<object> doneCallback=null,object userData=null)
        {
            mDoneCallback = doneCallback;
            StartCoroutine(LoadAsync(sceneName, userData));
            Z.UI.Open(LoadingUIPath);
        }
        public void SetLoadScenePath(string path)
        {
            
        }
        IEnumerator LoadAsync(string name,object userData)
        {
     
            Already = false;

            UnityEngine.SceneManagement.SceneManager.LoadScene("Empty", LoadSceneMode.Single);
           
            LoadingProgress = 0;
            int targetProgress = 0;
            AsyncOperation asyncScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
            if (asyncScene != null && !asyncScene.isDone)
            {
                asyncScene.allowSceneActivation = false;
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

                SceneName = name;

                ClearCache();

                //自行加载剩余的10%
                targetProgress = 100;
                while (LoadingProgress < targetProgress - 2)
                {
                    ++LoadingProgress;
                    yield return null;
                }
                LoadingProgress = 100;
              
                Already = true;
                //asyncScene.allowSceneActivation = true;
                mDoneCallback?.Invoke(userData);
                mDoneCallback = null;
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