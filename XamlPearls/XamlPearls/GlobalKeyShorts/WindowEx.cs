using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows;
using System.Reflection;

// 共享dll和exe的id好像有范围限制，要探究。
namespace XamlPearls.GlobalKeyShorts
{
    public static class WindowEx
    {
        private const int WM_HOTKEY = 0x312;

        private static List<(int id, Action<HotKeyModel> action, HotKeyModel model)> _hotKeyInfos;

        /// <summary>
        /// 快捷键的ID的范围必须是0X0000-0XFFFF，且在dll和exe中的范围也不同。
        /// </summary>
        /// <returns></returns>
        private static int GetHotkeyId()
        {
            var available = _hotKeyInfos.Where(item => item.id >= 0x0000 && item.id <= 0xFFFF).OrderBy(item => item.id).FirstOrDefault(item => !_hotKeyInfos.Any(item2 => item2.id == item.id + 1));
            var id = available == default ? 0x0000 : available.id + 1;
            if (id > 0XFFFF)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Please always unregister hotkey before the window closes.");
            }
            return id;
        }

        static WindowEx()
        {
            _hotKeyInfos = new List<(int id, Action<HotKeyModel> action, HotKeyModel model)>();
        }

        public static bool RegisterGlobalHotKey(this Window window, HotKeyModel hotKeyModel, Action<HotKeyModel> action) // action在创建window的线程上执行
        {
            window.SourceInitialized += (object sender, EventArgs e) =>
            {

            };
            window.Closed += Window_Closed;

            if (string.IsNullOrWhiteSpace(hotKeyModel.Name))
            {
                throw new ArgumentException("Hotkey name can't be whitespace or null.", nameof(hotKeyModel));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (hotKeyModel == null)
            {
                throw new ArgumentNullException(nameof(hotKeyModel));
            }

            if (_hotKeyInfos.Any(item => item.model.Name == hotKeyModel.Name))
            {
                throw new ArgumentException($"Duplicate hotkey name {hotKeyModel.Name}.", nameof(hotKeyModel));
            }

            var m_Hwnd = new WindowInteropHelper(window).Handle;
            HwndSource hWndSource = HwndSource.FromHwnd(m_Hwnd);
            if (!WindowHookFlag.GetIsEnabled(window))
            {
                hWndSource?.AddHook(WndProc);
                WindowHookFlag.SetIsEnabled(window, true);
            }
            var id = GetHotkeyId();
            var isSuccess = RegisterHotKey(m_Hwnd, id, hotKeyModel.GetModifierKeys(), (int)hotKeyModel.SelectKey);

            if (isSuccess)
            {
                _hotKeyInfos.Add((id, action, hotKeyModel));
            }
            return isSuccess;
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Window_SourceInitialized(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public static bool UnregisterGlobalHotKey(this Window window, string name)
        {
            var index = _hotKeyInfos.FindIndex(item => item.model.Name == name);
            if (index == -1)
            {
                throw new ArgumentException($"{name} doesn't exist.");
            }

            var m_Hwnd = new WindowInteropHelper(window).Handle;
            bool isSuccess = UnregisterHotKey(m_Hwnd, _hotKeyInfos[index].id) != 0;
            if (isSuccess)
            {
                _hotKeyInfos.RemoveAt(index);
            }
            return isSuccess;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys modifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);

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
                case WM_HOTKEY:
                    int id = wideParam.ToInt32();
                    var info = _hotKeyInfos.Single(item => item.id == id);
                    info.action?.Invoke(info.model);
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        private static class WindowHookFlag
        {
            public static readonly DependencyProperty IsHookedProperty =
                DependencyProperty.RegisterAttached(
                    nameof(IsHookedProperty),
                    typeof(bool),
                    typeof(WindowHookFlag),
                    new PropertyMetadata(false));

            public static bool GetIsEnabled(UIElement element)
            {
                return (bool)element.GetValue(IsHookedProperty);
            }

            public static void SetIsEnabled(UIElement element, bool value)
            {
                element.SetValue(IsHookedProperty, value);
            }
        }
    }
}