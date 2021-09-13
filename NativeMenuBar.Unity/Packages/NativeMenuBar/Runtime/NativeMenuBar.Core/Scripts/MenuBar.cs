using AOT;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NativeMenuBar.Core
{
    public class MenuBar : MonoBehaviour
    {
        #region DLL Import
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        [DllImport("NativeMenuBar.Windows.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StartPlugin();

        [DllImport("NativeMenuBar.Windows.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint AddMenuRoot(string menu);

        [DllImport("NativeMenuBar.Windows.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint AddMenuItem(string menuRoot, string menuItem);

        [DllImport("NativeMenuBar.Windows.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsMenuItemSelected(uint menuId);

        [DllImport("NativeMenuBar.Windows.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetMenuItemSelected(uint menuId, bool state);

        [DllImport("NativeMenuBar.Windows.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool IsMenuItemEnabled(uint menuId);

        [DllImport("NativeMenuBar.Windows.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetMenuItemEnabled(uint menuId, bool state);

        [DllImport("NativeMenuBar.Windows.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetMenuItemCallback([MarshalAs(UnmanagedType.FunctionPtr)] MenuItemClicked menuItemClickedCallback);

        [DllImport("NativeMenuBar.Windows.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void BuildMenu();
#elif !UNITY_EDITOR && UNITY_STANDALONE_OSX
// TODO
#elif !UNITY_EDITOR && UNITY_STANDALONE_LINUX
// TODO
#else
        // No implementation as fallback
        internal static void StartPlugin() { }
        internal static uint AddMenuRoot(string menu) { return default; }
        internal static uint AddMenuItem(string menuRoot, string menuItem) { return default; }
        internal static bool IsMenuItemSelected(uint menuId) { return default; }
        internal static void SetMenuItemSelected(uint menuId, bool state) { }
        internal static bool IsMenuItemEnabled(uint menuId) { return default; }
        internal static void SetMenuItemEnabled(uint menuId, bool state) { }
        internal static void SetMenuItemCallback(MenuItemClicked menuItemClickedCallback) { }
        internal static void BuildMenu() { }
#endif

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void MenuItemClicked(uint menuItemId);
        #endregion

        [SerializeField]
        private List<MenuRootItem> menuRootItems = default;

        public MenuRootItem[] MenuRoots => menuRootItems.ToArray();

        public MenuItem[] MenuItems => menuRootItems.SelectMany(mri => mri.MenuItems).ToArray();

        private static readonly List<uint> menuItemClickedIds = new List<uint>();

        private void Awake()
        {
            StartPlugin();

            foreach (var menuRootItem in menuRootItems)
            {
                menuRootItem.Id = AddMenuRoot(menuRootItem.Name);
                foreach (var menuItem in menuRootItem.MenuItems)
                {
                    var menuItemName = menuItem.ShortcutCombination != MenuItem.CombinationKeys.None
                        ? $"{menuItem.Name}\t{menuItem.ShortcutCombination}+{menuItem.Shortcut.ToString().ToUpperInvariant()}"
                        : $"{menuItem.Name}";
                    menuItem.Id = AddMenuItem(menuRootItem.Name, menuItemName);
                }
            }

            SetMenuItemCallback(OnMenuItemClicked);
            BuildMenu();
        }

        private void Update()
        {
            // Check the shortcuts to trigger events
            foreach (var menuItem in MenuItems)
            {
                var keycodes = menuItem.ShortcutKeys;
                if (keycodes.Any(k => Input.GetKey(k)))
                {
                    if (Input.GetKeyDown(menuItem.Shortcut.ToString().ToLowerInvariant()))
                    {
                        menuItem.Action?.Invoke();
                    }
                }
            }

            // Trigger events for clicked menu items
            foreach (var menuItemId in menuItemClickedIds)
            {
                var menuItem = MenuItems.FirstOrDefault(m => m.Id == menuItemId);
                if (menuItem != null)
                {
                    menuItem.Action?.Invoke();
                }
            }
            menuItemClickedIds.Clear();
        }

        [MonoPInvokeCallback(typeof(MenuItemClicked))]
        internal static void OnMenuItemClicked(uint menuItemId)
        {
            menuItemClickedIds.Add(menuItemId);
        }
    }
}