using System.Diagnostics;

namespace CrashHandler;

public class Program {
    private const string NumberType = "number";

    public static void Main(string[] args) {
        const string OnGracefulExitMessage = "OK";
        const string DotNetCoreDumpFileName = "os-core-dump";
        try {
            var expectedArgs = new[] {
                new { Label = "parent process pid to be monitored", Type = NumberType },
                new { Label = "process relaunch delay in ms", Type = NumberType },
                new { Label = "command line arguments to forward to the relaunched process", Type = "" },
            };

            // if (args.Length < expectedArgs.Length) {
            //     Console.Error.WriteLine(
            //         "Invalid number of arguments. Expected: " + Environment.NewLine + string.Join(" ", expectedArgs.Select(a => $"[{a.Label}]"))
            //     );
            //     return;
            // }

            var parsedValues = new object[expectedArgs.Length];

            for (var i = 0; i < expectedArgs.Length-1; i++) {
                if (expectedArgs[i].Type == NumberType) {
                    if (!int.TryParse(args[i], out var parsedArg)) {
                        Console.Error.WriteLine($"Argument {i + 1} is invalid. Expected {NumberType}.");
                        return;
                    }

                    parsedValues[i] = parsedArg;
                } else {
                    parsedValues[i] = args[i];
                }
            }
            
             
            var currentProcess = Process.GetCurrentProcess();
            var osCoreDumpPath =  Path.Combine(Path.GetDirectoryName(currentProcess.MainModule.FileName), DotNetCoreDumpFileName);
            File.Delete(osCoreDumpPath); // Check if exists!!

            var parentProcessPid = (int)parsedValues[0];
            var parentProcess = Process.GetProcessById(parentProcessPid);
            var parentProcessPath = parentProcess.MainModule.FileName;
            
            Console.WriteLine($"CrashHandler is listening main process!! :: {parsedValues[0]}");

            var read = Console.ReadLine(); 
            if (read != OnGracefulExitMessage) {
                Console.WriteLine($"CrashHandler :: NOT Graceful Exit Detected!! '{read}'");
                //expected termination message was not received, abrupt exit, lets restart
                var delay = (int)parsedValues[1];
                if (delay > 0) {
                    Thread.Sleep(delay);
                }
                Console.WriteLine($"CrashHandler :: After Delay!!!");
                //Process.Start(parentProcessPath, (string)parsedValues[2]);
            }
            else
            {
                Console.Error.WriteLine($"CrashHandler :: EXIT with SUCCESS!");
            }
        } catch (Exception e) {
            Console.Error.WriteLine(e.Message);
        }
    }
}