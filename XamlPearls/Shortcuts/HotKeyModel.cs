using System;
using System.Windows.Input;

namespace XamlPearls.Shortcuts
{
    public class HotKeyModel
    {
        public HotKeyModel(string name, bool holdCtrl, bool holdShift, bool holdAlt, bool holdWin, Keys key)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(name);
#else
            if (name == null)
                throw new ArgumentNullException(nameof(name));
#endif
            Name = name;
            HoldCtrl = holdCtrl;
            HoldShift = holdShift;
            HoldAlt = holdAlt;
            HoldWin = holdWin;
            Key = key;
        }

        public bool HoldAlt { get; set; }
        public bool HoldCtrl { get; set; }
        public bool HoldShift { get; set; }
        public bool HoldWin { get; set; }
        public Keys Key { get; set; }
        public string Name { get; set; }

        public ModifierKeys GetModifierKeys()
        {
            var modifierKey = ModifierKeys.None;

            if (HoldWin)
            {
                modifierKey |= ModifierKeys.Windows;
            }

            if (HoldCtrl)
            {
                modifierKey |= ModifierKeys.Control;
            }

            if (HoldAlt)
            {
                modifierKey |= ModifierKeys.Alt;
            }

            if (HoldShift)
            {
                modifierKey |= ModifierKeys.Shift;
            }

            return modifierKey;
        }
    }
}