using System;
using System.Windows.Forms;

namespace DiskPreclear;

internal static class App {

    [STAThread]
    private static void Main() {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }

}
