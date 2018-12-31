using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
#pragma warning disable 1998 
#pragma warning disable 0164
public class AsyncFunctionTest : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region 原始方法
    private static async Task<string> Method1Async()
    {
        Thread.Sleep(2000);
        return "method 1 done";
    }
    private static async Task<string> Method2Async()
    {
        Thread.Sleep(2000);
        return "method 2 done";
    }
    private static async Task<string> MyMethodAsync(int argument)
    {
        int local = argument;
        try
        {
            string result1 = await Method1Async();
            for (int i = 0; i < 3; i++)
            {
                string result2 = await Method2Async();
            }
        }
        catch (System.Exception)
        {
            print("Catch");
        }
        finally
        {
            print("Finally");
        }
        return "Done";
    }
    #endregion


    #region 编译器转换成的样子

    private static Task<string> MyAsyncMethod(int argument)//将异步函数 直接编译成为这个函数
    {
        StateMachine stateMachine = new StateMachine()
        {
            m_builder = AsyncTaskMethodBuilder<string>.Create(),
            m_state = -1,
            m_argument = argument
        };
        stateMachine.m_builder.Start(ref stateMachine);
        return stateMachine.m_builder.Task;//直接返回一个Task  然后可以点出延续任务
    }

    private struct StateMachine : IAsyncStateMachine
    {
        public AsyncTaskMethodBuilder<string> m_builder;
        public int m_state;

        public int m_argument, m_local, m_x;
        public string m_resultString1;
        public string m_resultString2;

        private TaskAwaiter<string> m_awaiterString1;
        private TaskAwaiter<string> m_awaiterString2;



        void IAsyncStateMachine.MoveNext()
        {
            string result = null;   //最终task的结果

            try
            {
                bool executeFinally = true; //先嘉定逻辑上离开try块
                if (m_state==-1)//如果第一次运行
                {
                    m_local = m_argument;//原始方法就从头开始执行
                }

                try
                {
                    TaskAwaiter<string> awaiterString1;
                    TaskAwaiter<string> awaiterString2;

                    switch (m_state)
                    {
                        case -1:
                            awaiterString1 = Method1Async().GetAwaiter();
                            if (!awaiterString1.IsCompleted)
                            {
                                m_state = 0;//Method1要异步完成
                                m_awaiterString1 = awaiterString1;//缓存awaiter
                                m_builder.AwaitUnsafeOnCompleted(ref awaiterString1, ref this);

                                executeFinally = false;
                                return;
                            }
                            break;
                        case 0:
                            awaiterString1 = m_awaiterString1;//进来的时候 再恢复到这个缓存的awaiter
                            break;
                        case 1:
                            awaiterString2 = m_awaiterString2;
                            goto ForLoopEpilog;
                            
                        default:
                            break;
                    }
                    m_resultString1 = awaiterString1.GetResult();

                    ForLoopPrologue:
                    m_x = 0;
                    goto ForLoopBody;

                    ForLoopEpilog:
                    m_resultString2 = awaiterString2.GetResult();
                    m_x++;

                    ForLoopBody:
                    if (m_x<3)
                    {
                        awaiterString2 = Method2Async().GetAwaiter();
                        if (!awaiterString2.IsCompleted)
                        {
                            m_state = 1;
                            m_awaiterString2 = awaiterString2;

                            m_builder.AwaitUnsafeOnCompleted(ref awaiterString2, ref this);
                            executeFinally = false;
                            return;
                        }
                        goto ForLoopEpilog;
                    }
                }
                catch (System.Exception)
                {
                    print("Catch");
                }
                finally
                {
                    if (executeFinally)
                    {
                        print("Finally");
                    }
                }
                result = "Done";

            }
            catch (System.Exception exception)
            {

                m_builder.SetException(exception);
                return;
            }
            m_builder.SetResult(result);
        }

        void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
        {
            throw new System.NotImplementedException();
        }
    }
    #endregion
}

