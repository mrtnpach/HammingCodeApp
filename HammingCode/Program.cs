using HammingCode.Views;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HammingCode.App
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ErrorDetectionView());
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    unsafe struct Msg
    {
        [FieldOffset(0)]
        public byte* c;
        [FieldOffset(0)]
        public long l;
    }
}
