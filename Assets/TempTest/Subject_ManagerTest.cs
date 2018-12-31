using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Threading.Tasks;
using Zframework;

public class Subject_ManagerTest : MonoBehaviour
{   //注销器集合，所有订阅的注销器都往这里塞
    CompositeDisposable mSubjectUnRegister = new CompositeDisposable();
    IEnumerator Start()
    {
     
             ////只接收开心程度大于10的发射...
             yield return new WaitForSeconds(.1f);

        Z.Pool.RegisterClassPool<HappySubjectArgs>(5);
        //HappySubjectArgs args=new HappySubjectArgs();
        //Z.Subject.GetSubject<HappySubjectArgs>().Where(e => e.HappyDegree > 10)
        //   .Subscribe(e => print("开心程度:" + e.HappyDegree + "...看起来很开心")).AddTo(mSubjectUnRegister);
        //        Debug.Log("play enter");
        for (int i = 0; i < 20; i++)
        {
            var t = new Task(() =>
            {
                Z.Subject.GetSubject<HappySubjectArgs>()
           .Subscribe(e => Debug.Log("开心程度:" + e.HappyDegree + "...看起来很开心"))/*.AddTo(mSubjectUnRegister)*/;
            });
            t.ContinueWith(_ => Debug.Log("e00"), TaskContinuationOptions.OnlyOnFaulted);
            t.Start();
        }
        Observable.EveryUpdate().Where(__ => Input.GetMouseButtonDown(0))
.Select((__, count) => count)
.Subscribe(count => Z.Subject.Fire(Z.Pool.Take<HappySubjectArgs>().Fill(count)));

    }
    void OnDestroy()
    {
        //此对象销毁时注销器注销
        mSubjectUnRegister.Dispose();
    }
}
public class HappySubjectArgs : SubjectArgs
{

    public static readonly int ID = typeof(HappySubjectArgs).GetHashCode();
    public override int SubjectId { get { return ID; } }
    public int HappyDegree = 0;
    public HappySubjectArgs Fill(int d) { HappyDegree = d; return this; }
}

