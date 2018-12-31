using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyYieldInstruction
{
    public abstract bool MoveNext();
}
public class MyWaitForSecond : MyYieldInstruction
{
    private float time;
    public MyWaitForSecond(float t) { time = t; }

    public override bool MoveNext()
    {
        return (time -= Time.deltaTime) > 0f;
    }
}
public class MyCoroutine : MyYieldInstruction//这个用来支持等待其他协程的协程
{
    public MyCoroutine(IEnumerator e) { mRefrence = e; }
    public IEnumerator mRefrence;
    public bool stop = false;
    public override bool MoveNext()
    {
        return CoroutineManager.Manager.HasCoroutine(mRefrence);
    }
}

public class CoroutineManager
{
    public static CoroutineManager Manager = new CoroutineManager();
    Dictionary<IEnumerator, MyYieldInstruction> mIterDic = new Dictionary<IEnumerator, MyYieldInstruction>();
    //提供给外界开启协程的接口
    public MyCoroutine StartCorouine(IEnumerator iter)
    {
        //先运行到第一个yield处
        iter.MoveNext();
        //得到yield 返回的 指令对象
        MyYieldInstruction operation = (MyYieldInstruction)iter.Current;
        //加入字典 进行管理
        mIterDic.Add(iter, operation);
        return new MyCoroutine(iter);
    }
    public bool HasCoroutine(IEnumerator e)
    {
        MyYieldInstruction temp;
        return mIterDic.TryGetValue(e, out temp);
    }
    public void Update()
    {   //每帧进行遍历
        GoOnForeach(mIterDic.GetEnumerator());
    }
    //可以删除元素的遍历
    void GoOnForeach(IEnumerator dicIter)
    {
        while (dicIter.MoveNext())
        {
            var dicIterItem = (KeyValuePair<IEnumerator, MyYieldInstruction>)dicIter.Current;
  
                                          //指令对象结束
            if (dicIterItem.Value==null||!dicIterItem.Value.MoveNext())
            {
                //如果这个协程后面还有yield  就替换新的指令对象(用copy的方式)
                if (dicIterItem.Key.MoveNext())
                {
                    if (dicIterItem.Value != null)
                    {
                        GoOnForeach(dicIter);
                        mIterDic[dicIterItem.Key] = (MyYieldInstruction)dicIterItem.Key.Current;
                        break;
                    }                  
                }
                //否则 删除这个协程
                else
                {                  
                    GoOnForeach(dicIter);
                    mIterDic.Remove(dicIterItem.Key);
                    break;
                }
            }
        }
    }
}
public class CoroutineTest : MonoBehaviour
{
    void Start()
    {
        CoroutineManager.Manager.StartCorouine(testWaitTime(1));
        CoroutineManager.Manager.StartCorouine(testWaitTime(2));
        //CoroutineManager.Manager.StartCorouine(testWaitNull()); //支持null
        //CoroutineManager.Manager.StartCorouine(testWaitCoroutine());//支持等待其他协程结束
    }
    void Update()
    {
        CoroutineManager.Manager.Update();
    }
    IEnumerator testWaitTime(int index)
    {
        print(index + " start wait...");
        yield return new MyWaitForSecond(2);
        print(index + " 2s after");
        yield return new MyWaitForSecond(2);
        print(index + " 4s after");
        print(index + " end");

    }
   
    IEnumerator testWaitNull()
    {
        for (int i = 0; i < 100; i++)
        {
            print(i);
            yield return null;
        }
    }

    IEnumerator testWaitCoroutine()
    {
        yield return CoroutineManager.Manager.StartCorouine(testWaitTime2());
        print("cool");
    }
    IEnumerator testWaitTime2()
    {
        yield return new MyWaitForSecond(3);
    }
}
