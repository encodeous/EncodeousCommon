using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EncodeousCommon.Input.Keyboard.KeyboardHook
{
    /// <summary>
    /// An argument class for <see cref="KeyboardHookWrapper"/>
    /// </summary>
    public class KeyboardArgs
    {
        public Keys DownKey;
        public bool Handled;
        public KeyboardArgs(Keys key)
        {
            DownKey = key;
        }
    }
}
