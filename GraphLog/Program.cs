using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GraphLog.DL2_analyse;

namespace GraphLog
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Must be used to avoid problem with DPI. All forms must use "AutoScaleMode" = DPI
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormDL2());
        }

        // Must be used to avoid problem with DPI.
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
