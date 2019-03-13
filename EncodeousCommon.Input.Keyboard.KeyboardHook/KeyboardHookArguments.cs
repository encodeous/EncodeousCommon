using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncodeousCommon.Input.Keyboard.KeyboardHook
{
    /// <summary>
    /// Keyboard Hook Arguments class used by <see cref="KeyboardHook"/>
    /// </summary>
    public class KeyboardHookArguments
    {
        public int nCode;
        public IntPtr wParam;
        public IntPtr lParam;
        public bool Handled = false;

        public KeyboardHookArguments(int _nCode, IntPtr _wParam, IntPtr _lParam)
        {
            nCode = _nCode;
            wParam = _wParam;
            lParam = _lParam;
        }
    }
}
