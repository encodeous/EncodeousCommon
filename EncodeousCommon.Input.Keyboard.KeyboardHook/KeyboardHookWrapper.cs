using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EncodeousCommon.Input.Keyboard.KeyboardHook
{
    /// <summary>
    /// Nice and friendly Low Level Keyboard Hook class.
    /// </summary>
    public class KeyboardHookWrapper
    {
        public delegate void KeyboardHookCallback(KeyboardArgs keyboardArgs);

        public event KeyboardHookCallback KeyDown;
        public event KeyboardHookCallback KeyUp;
        /// <summary>
        /// Instantiates a new Low Level Keyboard Hook
        /// </summary>
        KeyboardHook lKH = new KeyboardHook();

        /// <summary>
        /// Initializes the Keyboard hook
        /// </summary>
        /// <param name="hook">Defaults to true. If set to false requires manual hook with ManualHook();</param>
        public KeyboardHookWrapper(bool hook = true)
        {
            if (hook)
            {
                Initialize();
            }
        }
        /// <summary>
        /// Manually hooks the keyboard
        /// </summary>
        public void ManualHook()
        {
            Initialize();
        }

        private void Initialize()
        {
            lKH.KeyboardEvent += OnKeyboardEvent;
            lKH.Hook();
        }
        /// <summary>
        /// Called every time the KeyboardEvent is fired, sets a maximum return time
        /// </summary>
        /// <param name="args"></param>
        private void OnKeyboardEvent(KeyboardHookArguments args)
        {
            try
            {
                Task task = Task.Run(() => KeyboardUpdate(args));
                task.Wait(TimeSpan.FromMilliseconds(14));
            }
            catch
            {
                // ignored
            }
        }
        /// <summary>
        /// Called every time KeyboardEvent is fired
        /// </summary>
        /// <param name="args"></param>
        private void KeyboardUpdate(KeyboardHookArguments args)
        {
            int iwParam = args.wParam.ToInt32();
            var lKey = (Keys)Marshal.ReadInt32(args.lParam);
            KeyboardArgs arguments = new KeyboardArgs(lKey);
            if (iwParam == WM_KEYDOWN || iwParam == WM_SYSKEYDOWN)
            {
                // Invokes the KeyDown Event
                KeyDown?.Invoke(arguments);
            }
            // Note: If the KeyUp Event takes too long, the results from KeyDown will be returned
            if (arguments.Handled)
            {
                // KeyDown is Handled
                args.Handled = true;
                return;
            }

            if (iwParam == WM_KEYUP || iwParam == WM_SYSKEYUP)
            {
                // Invokes the KeyUp Event
                KeyUp?.Invoke(arguments);
            }

            if (arguments.Handled)
            {
                // KeyUp is Handled
                args.Handled = true;
            }
        }

        private const int WM_KEYDOWN = 0x100;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;
    }
}
