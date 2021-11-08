using Medo.Diagnostics;
using Medo.Windows.Forms;
using System;
using System.Threading;
using System.Windows.Forms;

namespace DiskPreclear;

internal static class App {

    private static readonly Mutex SetupMutex = new(false, @"Global\Medo64_DiskPreclear");

    [STAThread]
    private static void Main() {
        ApplicationConfiguration.Initialize();
        Application.SetCompatibleTextRenderingDefault(false);
        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

        UnhandledCatch.UnhandledException += UnhandledCatch_UnhandledException;
        UnhandledCatch.Attach();

        Application.Run(new MainForm());

        SetupMutex.Close();
    }

    private static void UnhandledCatch_UnhandledException(object? sender, UnhandledCatchEventArgs e) {
        ErrorReport.ShowDialog(null, new Uri("https://medo64.com/feedback/"), e.Exception);
#if DEBUG
        throw e.Exception;
#endif
    }

}
