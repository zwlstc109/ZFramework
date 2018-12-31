using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
namespace Zframework
{

    public static partial  class Extension
    {
        #region 临时放放
        internal static void AddTo(this Unit unit,UnitGroup unitGroup)
        {
            unitGroup.Add(unit);
        }
        internal static void AddTo(this PanelBase panel,UIGroup group)
        {
            group.Add(panel);
            panel.UIGroup = group;
        }
        #endregion
    }
}