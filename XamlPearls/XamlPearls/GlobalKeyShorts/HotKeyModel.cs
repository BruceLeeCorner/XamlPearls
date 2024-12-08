using System.Windows.Input;

namespace XamlPearls.GlobalKeyShorts
{
    /// <summary>
    /// 快捷键模型
    /// </summary>
    public class HotKeyModel
    {
        /// <summary>
        /// 设置项名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否勾选Ctrl按键
        /// </summary>
        public bool IsSelectCtrl { get; set; }

        /// <summary>
        /// 是否勾选Shift按键
        /// </summary>
        public bool IsSelectShift { get; set; }

        /// <summary>
        /// 是否勾选Alt按键
        /// </summary>
        public bool IsSelectAlt { get; set; }

        /// <summary>
        /// 选中的按键
        /// </summary>
        public Keys SelectKey { get; set; }

        public ModifierKeys GetModifierKeys()
        {
            var modifierKey = new ModifierKeys();
            // 注册热键
            if (IsSelectCtrl && !IsSelectShift && !IsSelectAlt)
            {
                modifierKey = ModifierKeys.Control;
            }
            else if (!IsSelectCtrl && IsSelectShift && !IsSelectAlt)
            {
                modifierKey = ModifierKeys.Shift;
            }
            else if (!IsSelectCtrl && !IsSelectShift && IsSelectAlt)
            {
                modifierKey = ModifierKeys.Alt;
            }
            else if (IsSelectCtrl && IsSelectShift && !IsSelectAlt)
            {
                modifierKey = ModifierKeys.Control | ModifierKeys.Shift;
            }
            else if (IsSelectCtrl && !IsSelectShift && IsSelectAlt)
            {
                modifierKey = ModifierKeys.Control | ModifierKeys.Alt;
            }
            else if (!IsSelectCtrl && IsSelectShift && IsSelectAlt)
            {
                modifierKey = ModifierKeys.Shift | ModifierKeys.Alt;
            }
            else if (IsSelectCtrl && IsSelectShift && IsSelectAlt)
            {
                modifierKey = ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt;
            }
            return modifierKey;
        }
    }
}