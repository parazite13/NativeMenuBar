using System;
using System.Collections.Generic;
using UnityEngine;

namespace NativeMenuBar.Core
{
    [Serializable]
    public class MenuRootItem
    {
        [SerializeField]
        private string name = default;
        public string Name
        {
            get => name;
            set => name = value;
        }

        [SerializeField]
        private List<MenuItem> menuItems = default;
        public List<MenuItem> MenuItems
        {
            get => menuItems;
            set => menuItems = value;
        }

        public uint Id { get; internal set; }

        internal MenuBar MenuBar { get; set; }
    }
}