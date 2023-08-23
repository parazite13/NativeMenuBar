using System;
using System.Collections.Generic;
using UnityEngine;

namespace NativeMenuBar.Core
{
    [Serializable]
    public class MenuRootItem : AbstractMenuItem
    {
        internal MenuBar MenuBar { get; set; }
    }
}