using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;
using Zframework;
using UniRx;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Runtime.CompilerServices;

public class ThreadTest : MonoBehaviour
{
    public int DoSth()
    {
        //做一些耗时工作
        //Thread.Sleep(2000);
        //返回一个结果
        return 1;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public async Task<int> VAsync()
    {
        Task t = new Task(() => Thread.Sleep(2000));
        t.Start();
        await t;
        return 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
