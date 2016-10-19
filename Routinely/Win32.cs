using System.Runtime.InteropServices;

namespace Routinely
{
    public class Win32
    {
        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);
    }
}