using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EncodeousCommon.Sys.Windows
{
    public class Desktop
    {
        #region Imports

        [DllImport("user32.dll")]
        public static extern IntPtr CreateDesktop(string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode, int dwFlags,
            int dwDesiredAccess, IntPtr lpsa);

        [DllImport("user32.dll")]
        private static extern bool CloseDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        private static extern IntPtr OpenDesktop(string lpszDesktop, uint dwFlags,
            bool fInherit, uint dwDesiredAccess);

        [DllImport("user32.dll")]
        private static extern IntPtr OpenInputDesktop(int dwFlags, bool fInherit, long dwDesiredAccess);

        [DllImport("user32.dll")]
        private static extern bool SwitchDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        private static extern bool EnumDesktops(IntPtr hwinsta, EnumDesktopProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetProcessWindowStation();

        [DllImport("user32.dll")]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDesktopWindowsProc lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SetThreadDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        private static extern IntPtr GetThreadDesktop(int dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool GetUserObjectInformation(IntPtr hObj, int nIndex, IntPtr pvInfo, int nLength, ref int lpnLengthNeeded);
        private delegate bool EnumDesktopProc(string lpszDesktop, IntPtr lParam);
        private delegate bool EnumDesktopWindowsProc(IntPtr desktopHandle, IntPtr lParam);
        #endregion

        #region Enums
        public enum DESKTOP_ACCESS : uint
        {
            DESKTOP_NONE = 0,
            DESKTOP_READOBJECTS = 0x0001,
            DESKTOP_CREATEWINDOW = 0x0002,
            DESKTOP_CREATEMENU = 0x0004,
            DESKTOP_HOOKCONTROL = 0x0008,
            DESKTOP_JOURNALRECORD = 0x0010,
            DESKTOP_JOURNALPLAYBACK = 0x0020,
            DESKTOP_ENUMERATE = 0x0040,
            DESKTOP_WRITEOBJECTS = 0x0080,
            DESKTOP_SWITCHDESKTOP = 0x0100,

            GENERIC_ALL = (DESKTOP_READOBJECTS | DESKTOP_CREATEWINDOW | DESKTOP_CREATEMENU |
                           DESKTOP_HOOKCONTROL | DESKTOP_JOURNALRECORD | DESKTOP_JOURNALPLAYBACK |
                           DESKTOP_ENUMERATE | DESKTOP_WRITEOBJECTS | DESKTOP_SWITCHDESKTOP),
        }


        #endregion

        #region Static Methods
        private static bool DesktopProc(string lpszDesktop, IntPtr lParam)
        {
            // add the desktop to the collection.
            desktops.Add(lpszDesktop);

            return true;
        }
        public static bool Exists(string name)
        {
            // enumerate desktops.
            string[] desktops = Desktop.GetDesktops();

            // return true if desktop exists.
            foreach (string desktop in desktops)
            {
                if (desktop == name) return true;
            }

            return false;
        }
        public static string[] GetDesktops()
        {
            // attempt to enum desktops.
            IntPtr windowStation = GetProcessWindowStation();

            // check we got a valid handle.
            if (windowStation == IntPtr.Zero) return new string[0];

            string[] desktop;

            // lock the object. thread safety and all.
            lock (desktops = new StringCollection())
            {
                bool result = EnumDesktops(windowStation, new EnumDesktopProc(DesktopProc), IntPtr.Zero);

                // something went wrong.
                if (!result) return new string[0];

                //	// turn the collection into an array.
                desktop = new string[desktops.Count];
                for (int i = 0; i < desktop.Length; i++) desktops[i] = desktops[i];
            }

            return desktop;
        }
        public static IntPtr DesktopHandleOfCurrentThread()
        {
            return GetThreadDesktop((int)ProcessManager.GetCurrentThreadId());
        }
        public static IntPtr DesktopHandleOfThread(int threadID)
        {
            return GetThreadDesktop(threadID);
        }

        public static void SetCurrentThreadDesktop(Desktop d)
        {
            SetThreadDesktop(d.Handle);
        }
        public static void SetCurrentThreadDesktop(IntPtr d)
        {
            SetThreadDesktop(d);
        }
        public static IntPtr CreateDesktopH(string name, DESKTOP_ACCESS desktopAccess)
        {
            return CreateDesktop(name, IntPtr.Zero, IntPtr.Zero, 0, (int)desktopAccess, IntPtr.Zero);
        }
        public static Desktop DesktopOfCurrentThread()
        {
            IntPtr v = GetThreadDesktop((int) ProcessManager.GetCurrentThreadId());
            return new Desktop(v, GetDesktopName(v));
        }
        public static Desktop DesktopOfThread(int threadID)
        {
            IntPtr v = GetThreadDesktop(threadID);
            return new Desktop(v,GetDesktopName(v));
            
        }
        public static string GetDesktopName(IntPtr desktopHandle)
        {
            // check its not a null pointer.
            // null pointers wont work.
            if (desktopHandle == IntPtr.Zero) return null;

            // get the length of the name.
            int needed = 0;
            string name = String.Empty;
            GetUserObjectInformation(desktopHandle, UOI_NAME, IntPtr.Zero, 0, ref needed);

            // get the name.
            IntPtr ptr = Marshal.AllocHGlobal(needed);
            bool result = GetUserObjectInformation(desktopHandle, UOI_NAME, ptr, needed, ref needed);
            name = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            // something went wrong.
            if (!result) return null;

            return name;
        }
        public static Desktop CreateDesktop(string name, DESKTOP_ACCESS desktopAccess)
        {
            return new Desktop(CreateDesktop(name, IntPtr.Zero, IntPtr.Zero, 0, (int)desktopAccess, IntPtr.Zero), name);
        }

        public static IntPtr OpenDesktopHandle(string name)
        {
            return OpenDesktop(name, 0, true, AccessRights);
        }

        public static Desktop OpenDesktop(string name)
        {
            return new Desktop(OpenDesktop(name, 0, true, AccessRights), name);
        }

        #endregion

        public IntPtr Handle;
        public string DesktopName;
        private static StringCollection desktops;
        private const short SW_HIDE = 0;
        private const short SW_NORMAL = 1;
        private const int STARTF_USESTDHANDLES = 0x00000100;
        private const int STARTF_USESHOWWINDOW = 0x00000001;
        private const int UOI_NAME = 2;
        private const int STARTF_USEPOSITION = 0x00000004;
        private const int NORMAL_PRIORITY_CLASS = 0x00000020;
        private const uint DESKTOP_CREATEWINDOW = 0x0002;
        private const uint DESKTOP_ENUMERATE = 0x0040;
        private const uint DESKTOP_WRITEOBJECTS = 0x0080;
        private const uint DESKTOP_SWITCHDESKTOP = 0x0100;
        private const uint DESKTOP_CREATEMENU = 0x0004;
        private const uint DESKTOP_HOOKCONTROL = 0x0008;
        private const uint DESKTOP_READOBJECTS = 0x0001;
        private const uint DESKTOP_JOURNALRECORD = 0x0010;
        private const uint DESKTOP_JOURNALPLAYBACK = 0x0020;
        private const uint AccessRights = DESKTOP_JOURNALRECORD | DESKTOP_JOURNALPLAYBACK | DESKTOP_CREATEWINDOW | DESKTOP_ENUMERATE | DESKTOP_WRITEOBJECTS | DESKTOP_SWITCHDESKTOP | DESKTOP_CREATEMENU | DESKTOP_HOOKCONTROL | DESKTOP_READOBJECTS;

        public Desktop(IntPtr handle, string name)
        {
            Handle = handle;
            if (name == "")
            {
                throw new Exception("Desktop Name was empty!");
            }
            DesktopName = name;
        }

        public Desktop(string name)
        {
            DesktopName = name;
            Handle = OpenDesktop(name, 0, true, AccessRights);
        }
        public bool Close()
        {
            // check there is a desktop open.
            if (Handle != IntPtr.Zero)
            {
                // close the desktop.
                bool result = CloseDesktop(Handle);

                if (result)
                {
                    Handle = IntPtr.Zero;

                    DesktopName = String.Empty;
                }

                return result;
            }

            // no desktop was open, so desktop is closed.
            return true;
        }
        public bool Show()
        {
            // make sure there is a desktop to open.
            if (Handle == IntPtr.Zero) return false;

            // attempt to switch desktops.
            bool result = SwitchDesktop(Handle);

            return result;
        }
    }
}
