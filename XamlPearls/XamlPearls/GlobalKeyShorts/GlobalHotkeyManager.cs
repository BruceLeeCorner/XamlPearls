﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace XamlPearls.GlobalKeyShorts
{
    public static class GlobalHotkeyManager
    {
        private const int WM_HOTKEY = 0x312;

        private static List<(int id, Window window, IntPtr intPtr, Action<HotKeyModel> action, HotKeyModel model)> _hotKeyInfos;

        static GlobalHotkeyManager()
        {
            _hotKeyInfos = new List<(int id, Window window, IntPtr intPtr, Action<HotKeyModel> action, HotKeyModel model)>();
        }

        public static IEnumerable<HotKeyModel> GetAllHotkeys()
        {
            return _hotKeyInfos.Select(item =>
            new HotKeyModel(item.model.Name, item.model.HoldCtrl, item.model.HoldShift, item.model.HoldAlt, item.model.HoldWin, item.model.Key));
        }

        public static IEnumerable<HotKeyModel> GetHotkeysOnWindow(Window window)
        {
            return _hotKeyInfos.Where(item => ReferenceEquals(window, item.window)).Select(item =>
            new HotKeyModel(item.model.Name, item.model.HoldCtrl, item.model.HoldShift, item.model.HoldAlt, item.model.HoldWin, item.model.Key));
        }

        /// <summary>
        /// action在创建window的线程上执行,也就是UI线程
        /// </summary>
        /// <param name="window"></param>
        /// <param name="hotKeyModel"></param>
        /// <param name="action"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void RegisterGlobalHotKey(this Window window, HotKeyModel hotKeyModel, Action<HotKeyModel> action)
        {
            if (hotKeyModel == null)
            {
                throw new ArgumentNullException(nameof(hotKeyModel));
            }

            if (string.IsNullOrWhiteSpace(hotKeyModel.Name))
            {
                throw new ArgumentException("Hotkey name can't be whitespace or null.", nameof(hotKeyModel));
            }

            if (hotKeyModel.Key == Keys.None)
            {
                throw new ArgumentException("The 'Key' can't be None.", nameof(hotKeyModel));
            }

            if ((hotKeyModel.HoldWin || hotKeyModel.HoldCtrl || hotKeyModel.HoldShift || hotKeyModel.HoldAlt) == false)
            {
                throw new ArgumentException("Select at least one modify key.", nameof(hotKeyModel));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (_hotKeyInfos.Any(item => item.model.Name == hotKeyModel.Name))
            {
                throw new ArgumentException($"Duplicate hotkey name {hotKeyModel.Name}.", nameof(hotKeyModel));
            }

            var m_Hwnd = new WindowInteropHelper(window).Handle;

            if (_hotKeyInfos.All(item => !ReferenceEquals(item.window, window)))
            {
                HwndSource hWndSource = HwndSource.FromHwnd(m_Hwnd);
                hWndSource?.AddHook(WndProc);
            }

            var id = GetHotkeyId();
            if (_hotKeyInfos.Any(item => item.id == id))
            {
                throw new ArgumentException($"Duplicate hotkey id {id}");
            }
            var isSuccess = RegisterHotKey(m_Hwnd, id, hotKeyModel.GetModifierKeys(), (int)hotKeyModel.Key);

            if (!isSuccess)
            {
                throw new ArgumentException($"Failed to register the hotkey.", nameof(hotKeyModel));
            }
            _hotKeyInfos.Add((id, window, m_Hwnd, action, hotKeyModel));
            window.Closed += (object sender, EventArgs e) =>
            {
                UnregisterGlobalHotKey(hotKeyModel.Name);
            };
        }

        /// <summary>
        /// action在创建window的线程上执行,也就是UI线程
        /// </summary>
        /// <param name="window"></param>
        /// <param name="hotKeyModel"></param>
        /// <param name="action"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void RegisterGlobalHotKey(HotKeyModel hotKeyModel, Action<HotKeyModel> action, Window window)
        {
            window.RegisterGlobalHotKey(hotKeyModel, action);
        }

        public static void UnregisterGlobalHotKey(string name)
        {
            var index = _hotKeyInfos.FindIndex(item => item.model.Name == name);
            if (index == -1)
            {
                throw new ArgumentException($"Hotkey name {name} doesn't exist.");
            }

            (int id, Window window, IntPtr m_Hwnd, Action<HotKeyModel> action, HotKeyModel model) = _hotKeyInfos[index];
            bool isSuccess = UnregisterHotKey(m_Hwnd, id);
            if (isSuccess)
            {
                _hotKeyInfos.RemoveAt(index);
                if (!_hotKeyInfos.Any(item => ReferenceEquals(item.window, window)))
                {
                    HwndSource hWndSource = HwndSource.FromHwnd(m_Hwnd);
                    hWndSource?.RemoveHook(WndProc);
                }
            }
        }

        private static int GetHotkeyId()
        {
            var availableInfo = _hotKeyInfos.Where(item => item.id >= 0x0000 && item.id <= 0xFFFF).OrderBy(item => item.id).FirstOrDefault(item => !_hotKeyInfos.Any(item2 => item2.id == item.id + 1));
            var id = availableInfo == default ? 0x0000 : availableInfo.id + 1;
            if (id > 0XFFFF)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Please always unregister hotkey before the window closes.");
            }
            return id;
        }

        #region win32 api

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        #endregion win32 api

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd">Window句柄</param>
        /// <param name="msg">Windows消息类别</param>
        /// <param name="wideParam">快捷键id</param>
        /// <param name="longParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private static IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wideParam, IntPtr longParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_HOTKEY: // 只处理热键消息
                    int id = wideParam.ToInt32();
                    var info = _hotKeyInfos.Single(item => item.id == id);
                    info.action?.Invoke(info.model);
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }
    }
}