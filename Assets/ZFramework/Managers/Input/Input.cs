using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{
    
   public interface IInputer
    {
        float Horizontal1 { get; }
        float Horizontal2 { get; }
    }
}