using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace ABBDataManagerSystem
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 订阅全局异常处理事件
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        // WPF UI线程未捕获异常处理事件
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // 处理异常
            HandleException(e.Exception);

            // 设置为已处理
            e.Handled = true;
        }

        // 非UI线程未捕获异常处理事件
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // 处理异常
            HandleException(e.ExceptionObject as Exception);
        }

        // 处理异常的统一方法
        private void HandleException(Exception ex)
        {
            if (ex != null)
            {
                // 记录异常日志或显示消息
                MessageBox.Show($"An unhandled exception occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // 你可以在这里添加更多的异常处理逻辑，比如写入日志文件
                LogException(ex);
            }
        }

        private static bool ReadConfig()
        {
            string filePath = "Config.txt";
            if (!File.Exists(filePath))
            {
                return false;
            }
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (!line.Contains("Enable Dump"))
                    {
                        continue;
                    }
                    var vs = line.Split(":");
                    if (vs.Length <= 1)
                    {
                        return false;
                    }
                    var v = vs[1];
                    if (v.Trim() == "1" || v.Trim().ToLower() == "true")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void LogException(Exception error)
        {
            string str;
            var strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now + "\r\n";
            if (error != null)
            {
                str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("应用程序线程错误:{0}", error);
            }
            Log.Error(str);

            if (ReadConfig())
            {
                new Thread(() =>
                {
                    string message = error != null ? error.GetType().Name : "Unknow";
                    GenerateDumpFile($"UnhandledException_{message}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.dmp");
                }).Start();
            }
        }

        static void GenerateDumpFile(string dumpFilePath)
        {
            using (var process = Process.GetCurrentProcess())
            using (var fs = new FileStream(dumpFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                MiniDumpWriteDump(
                    process.Handle,
                    process.Id,
                    fs.SafeFileHandle,
                    MINIDUMP_TYPE.MiniDumpWithFullMemory,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero
                );
            }
        }

        [DllImport("Dbghelp.dll")]
        private static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            int processId,
            SafeHandle hFile,
            MINIDUMP_TYPE dumpType,
            IntPtr expParam,
            IntPtr userStreamParam,
            IntPtr callbackParam
        );

        private enum MINIDUMP_TYPE : uint
        {
            MiniDumpNormal = 0x00000000,
            MiniDumpWithDataSegs = 0x00000001,
            MiniDumpWithFullMemory = 0x00000002,
            MiniDumpWithHandleData = 0x00000004,
            MiniDumpFilterMemory = 0x00000008,
            MiniDumpScanMemory = 0x00000010,
            MiniDumpWithUnloadedModules = 0x00000020,
            MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
            MiniDumpFilterModulePaths = 0x00000080,
            MiniDumpWithProcessThreadData = 0x00000100,
            MiniDumpWithPrivateReadWriteMemory = 0x00000200,
            MiniDumpWithoutOptionalData = 0x00000400,
            MiniDumpWithFullMemoryInfo = 0x00000800,
            MiniDumpWithThreadInfo = 0x00001000,
            MiniDumpWithCodeSegs = 0x00002000,
            MiniDumpWithoutAuxiliaryState = 0x00004000,
            MiniDumpWithFullAuxiliaryState = 0x00008000,
            MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
            MiniDumpIgnoreInaccessibleMemory = 0x00020000,
            MiniDumpWithTokenInformation = 0x00040000,
        }
    }
}
