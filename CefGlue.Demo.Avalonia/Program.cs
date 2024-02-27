using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using ServiceStudio.Presenter;
using ServiceStudio.View;
using ServiceStudio.WebViewImplementation;
using AppLifetime = Avalonia.Controls.ApplicationLifetimes.ClassicDesktopStyleApplicationLifetime;

namespace ServiceStudio {
    public class Program {
        [STAThread]
        static int Main(string[] args) {
            return Start(args);
        }

        private static int Start(string[] args)
        {
            // Environment.SetEnvironmentVariable("DOTNET_DbgEnableMiniDump", "1");
            // Environment.SetEnvironmentVariable("DOTNET_DbgMiniDumpType", "1");
            Console.WriteLine($"DOTNET_DbgEnableMiniDump: {Environment.GetEnvironmentVariable("DOTNET_DbgEnableMiniDump")}");
            Console.WriteLine($"DOTNET_DbgMiniDumpType: {Environment.GetEnvironmentVariable("DOTNET_DbgMiniDumpType")}");
            
                
            //Environment.SetEnvironmentVariable("DOTNET_DbgMiniDumpName", "/var/folders/x4/k88wdsms0hb21kjj633d5cnh0000gp/T/Batatas");
            
            //Environment.SetEnvironmentVariable("COMPlus_DbgEnableMiniDump", "1");
            //Environment.SetEnvironmentVariable("COMPlus_DbgMiniDumpType", "1");
            Console.WriteLine($"COMPlus_DbgEnableMiniDump: {Environment.GetEnvironmentVariable("COMPlus_DbgEnableMiniDump")}");
            Console.WriteLine($"COMPlus_DbgMiniDumpType: {Environment.GetEnvironmentVariable("COMPlus_DbgMiniDumpType")}");


            LaunchCrashHandlerProcess();
            var t = Task.Run(async () => await Task.Delay(1000));
            t.Wait();
            
            
            var mainThread = Thread.CurrentThread;
            var appBuilder = new Lazy<AppBuilder>(() => BuildApp(mainThread));

            AppLifetime GetAppLifetime() => (AppLifetime)appBuilder.Value.Instance.ApplicationLifetime;

            StartRuntime(appBuilder, args, mainThread).Wait();

            if (!RunApplication(appBuilder)) {
                return 0; // view not started (probably an headless session), exit
            }
            throw new Exception("User Forced Exception inside Task");
            //crashHandlerInputWriter.WriteLine("OK");
            return GetAppLifetime().Start(args);
        }

        private static Task StartRuntime(Lazy<AppBuilder> appBuilder, string[] args, Thread mainThread) {
            var viewInitializationTaskSource = new TaskCompletionSource();
            ViewImplementationProvider.Starting += () => viewInitializationTaskSource.SetResult();

            var initializationTask = Task.Run(() => {
                ViewImplementationProvider.SetInstance(() => new ExtendedViewImplementationProvider());
                var runtime = new RuntimeImplementation(args,null, null, mainThread);
                runtime.Start();
            });

            // wait until end or view starts, whatever comes first... in the last case we must proceed for the view to start
            return Task.WhenAny(initializationTask, viewInitializationTaskSource.Task);
        }

        private static bool RunApplication(Lazy<AppBuilder> appBuilder) {
            if (ViewImplementationProvider.HasStarted) {
                ExtendedViewImplementationProvider.Instance.RunApplication((App)appBuilder.Value.Instance);
                return true;
            }

            return false;
        }

        private static AppBuilder BuildApp(Thread mainThread) {
            if (Thread.CurrentThread != mainThread) {
                throw new Exception("Must be called on main thread");
            }
            return AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .With(new Win32PlatformOptions {
                    UseWindowsUIComposition = false
                })
                .SetupWithLifetime(new AppLifetime() {
                    ShutdownMode = ShutdownMode.OnLastWindowClose,
                });
        }

        private static StreamWriter crashHandlerInputWriter;
        
        protected static void LaunchCrashHandlerProcess() {
            try {
                var currentProcess = Process.GetCurrentProcess();
                var currentProcessPath = Path.GetDirectoryName(currentProcess.MainModule.FileName);
                var processName = "CrashHandler" + (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "" : ".exe");
                var crashHandlerPath = Path.Combine(currentProcessPath, processName);
                
                if (!File.Exists(crashHandlerPath)) {
                    return;
                }
                
                var filestream = new FileStream("out.txt", FileMode.Create);
                var streamwriter = new StreamWriter(filestream);
                streamwriter.AutoFlush = true;
                
                
                Console.SetOut(streamwriter);
                Console.SetError(streamwriter);

                const int RelaunchServiceStudioDelayIsMs = 1500; // try to avoid conflicts with auto-update
                var processLaunchArguments = $"{currentProcess.Id} {RelaunchServiceStudioDelayIsMs}";
                var crashHandlerProcessStartInfo = new ProcessStartInfo {
                    FileName = crashHandlerPath,
                    Arguments = processLaunchArguments,
                    UseShellExecute = false,
                    RedirectStandardInput = true
                };
                
                

                crashHandlerInputWriter = Process.Start(crashHandlerProcessStartInfo).StandardInput;
            } catch {
                Console.WriteLine("CrashHandler process could not be launched");
            }
        }
    }
}
