using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;
using Zframework;
using System;
//using System.Linq;
using System.Threading.Tasks;
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif

#pragma warning disable 0219
class TestTest
{
    public TestTest(string s)
    {
        name = s;
    }
    public string name;
}
public class ZFrameworkTest : MonoBehaviour
{
    //readonly int num = 2;
    public Text mText;
    public AudioSource source;
    private AudioClip clip = null;
    private GameObject prefab=null;
    private void Awake()
    {
        //Zframework.Timer.IntializeDriver();
    }
    // Start is called before the first frame update
    ConcurrentBag<int> mBag = new ConcurrentBag<int>();
    ConcurrentDictionary<string, string> mDic = new ConcurrentDictionary<string, string>();
    ConcurrentStack<string> mStack = new ConcurrentStack<string>();
    IEnumerator Start()
    {
        //for (int i = 0; i < 100; i++)
        //{
        //    new Task(() => mStack.Push(i.ToString())).Start();
        //}
        yield return new WaitForSeconds(0.1f);
        clip = Z.Resource.LoadResource<AudioClip>("Assets/GameData/Sounds/senlin.mp3");
        source.clip = clip;
        source.Play();

        //Z.Obs.ForLoop(5,_ =>prefab=Z.Unit.LoadUnit("Assets/GameData/Prefabs/Attack.prefab"));
        Z.UI.OpenUI("Assets/GameData/Prefabs/UGUI/Panel/PnlMainMenu.prefab");
#if UNITY_EDITOR
        //string[] allDependences = AssetDatabase.GetDependencies("Assets/GameData/Prefabs/Attack.prefab");
        //Z.Obs.ForLoop(allDependences.Length, i => Z.Log.Log(allDependences[i]));
#endif

    }

    //readonly int total = 0;
    void Test<T>()
    {
        Debug.Log(typeof(T));
    }
    //long lastTime = System.DateTime.Now.Ticks;
    // Update is called once per frame
    void Update()
    {
        //long time2 = System.DateTime.Now.Ticks;
        //print( time2-lastTime);
        //lastTime = time2;
        if (Input.GetKeyDown(KeyCode.A))
        {
            Z.Unit.Release(0, true);
            print(prefab);
            //source.clip = null;
            //clip = null;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Z.Unit.Release(0, false);

            //source.clip = null;
            //clip = null;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Z.Obs.ForLoop(5, _ => Z.Unit.LoadUnit("Assets/GameData/Prefabs/Attack.prefab"));
            //source.clip = null;
            //clip = null;
        }
    }
}
