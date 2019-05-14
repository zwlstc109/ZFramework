using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    public enum InputMode
    {
        Default,//键鼠

    }
    //TODO 构思中 
    //首先需要一种机制 将输入来的0和1 转换成另一种游戏可用的信号 比如按下了 持续按键中， 双击了 ，抬起了，延迟确认一个信号等等
    //每一种设备信号都需要这个机制封装(edit里设置的那些，甚至还有keyCode)，实际使用时，就可以new出一个这样的对象（暂定为ZButton）
    //然后指定他接受的输入源 使用场景类似为 var button=new ZButton("Horizontal"，xxx,xxx);//可能会需要不同的参数 来设置一些东西 比如延迟的等待时间之类的
    //这个字符串就代表unity内置的输入信号,edit里设置的那些
    //然后这个button可以接收 注册 OnPressed+=...  OnPressing+=... OnDoublePressed+=...之类的
    //而inputManager要做的事情之一 就是为用户提供注册这些button的接口 

    //改键到底是什么
    //
    /// <summary>
    /// 玩家输入控制
    /// </summary>
    public class InputManager : BaseManager 

    {
        protected override int MgrIndex { get { return (int)ManagerIndex.Input; } }
        public InputMode PlayerInputMode;
        internal IInputer PlayerInput = null;
        //private ZCatridge _mCatridge;//有意思的东西 装填完后只能发射一次
        public bool Enable { get; private set; } = true;
        public IObservable<string> CurrentKeyDown
        {
            get
            {           
                return Z.Subject.Get("Z_CurrentKey").Select(o => (string)o);
            }

        }
        internal override void Init()
        {
            Z.Input = this;
            //_mCatridge = Z.Pool.Take<ZCatridge>();

            var keyCodesCount = System.Enum.GetNames(typeof(KeyCode)).Length;
            Z.Obs.EveryUpdate.Subscribe(_ =>
            {

                if (Input.anyKeyDown)
                    for (int i = 0; i < keyCodesCount; i++)
                    {
                        var keycode = (KeyCode)i;
                        if (Input.GetKey(keycode))
                            Z.Subject.Fire("Z_CurrentKey", keycode.ToString());
                    }
            })/*.AddTo(,)*/;
        }

        internal override void MgrUpdate()
        {
            base.MgrUpdate();
            if (!Enable)
                Input.ResetInputAxes();//感觉这个方法比return好一点
        }


        internal override void ShutDown()
        {
            throw new NotImplementedException();
        }

    }
}