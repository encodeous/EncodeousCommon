using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EncodeousCommon.Input.Keyboard.KeyboardHook
{
    /// <summary>
    /// A Low-Level class for Low level keyboard hooks.
    /// If you are trying to create a keyboard hook,
    /// The <see cref="KeyboardHookWrapper"/> class is recommended
    /// </summary>
    public class KeyboardHook
    {
        private delegate IntPtr KeyboardHookHandler(int nCode, IntPtr wParam, IntPtr lParam);

        private KeyboardHookHandler _hookHandler;

        public delegate void KeyboardHookCallback(KeyboardHookArguments args);

        public event KeyboardHookCallback KeyboardEvent;


        private IntPtr _hookId = IntPtr.Zero;

        public void Hook()
        {
            _hookHandler = HookFunc;
            _hookId = SetHook(_hookHandler);
        }
        public void UnHook()
        {
            UnhookWindowsHookEx(_hookId);
        }
        private IntPtr SetHook(KeyboardHookHandler proc)
        {
            using (ProcessModule module = Process.GetCurrentProcess().MainModule)
                return SetWindowsHookEx(13, proc, GetModuleHandle(module.ModuleName), 0);
        }


        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0) return CallNextHookEx(_hookId, nCode, wParam, lParam); //checks for nulls
            KeyboardHookArguments invokeArguments = new KeyboardHookArguments(nCode, wParam, lParam);
            KeyboardEvent?.Invoke(invokeArguments); //Invokes KeyboardEvent
            if (invokeArguments.Handled)
            {
                return (IntPtr)1;
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
        ~KeyboardHook()
        {
            UnHook();
        }

        #region Native Functions

        private const int WM_KEYDOWN = 0x100;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookHandler lpfn, IntPtr hMod,
            uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}
