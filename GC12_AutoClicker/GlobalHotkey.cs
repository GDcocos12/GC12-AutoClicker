using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace GC12_AutoClicker
{
    public class GlobalHotkey : IDisposable
    {
        private readonly int _id;
        private readonly Window _window;
        private readonly Action _callback;
        private HwndSource _source;

        public GlobalHotkey(ModifierKeys modifierKeys, Key key, Window window, Action callback)
        {
            _id = GetHashCode();
            _window = window;
            _callback = callback;

            var helper = new WindowInteropHelper(window);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);

            RegisterHotKey(helper.Handle, _id, (uint)modifierKeys, (uint)KeyInterop.VirtualKeyFromKey(key));
        }

        public bool Register()
        {
            var helper = new WindowInteropHelper(_window);
            return RegisterHotKey(helper.Handle, _id, (uint)ModifierKeys.None, (uint)KeyInterop.VirtualKeyFromKey(_startStopHotkey));

        }

        public void Unregister()
        {
            var helper = new WindowInteropHelper(_window);
            UnregisterHotKey(helper.Handle, _id);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY && wParam.ToInt32() == _id)
            {
                _callback?.Invoke();
                handled = true;
            }
            return IntPtr.Zero;
        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public void Dispose()
        {
            Unregister();
            _source.RemoveHook(HwndHook);
            _source = null;
        }

        private Key _startStopHotkey;
    }
}
