using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CivilSurveySuite.Shared.Services.Interfaces;

namespace CivilSurveySuite.UI.Logger
{
    public class Logger : ILogger
    {
        private readonly ILogWriter _writer;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        public Logger(ILogWriter writer)
        {
            _writer = writer;
        }

        public void Info(string info, [CallerMemberName] string caller = "")
            => LogAsync(type: "INFO", caller: caller, message: info);
        public void Info(string info, object obj, [CallerMemberName] string caller = "")
            => LogAsync(type: "INFO", caller: caller, message: string.Format(info, obj));
        public void Info(Exception ex, string info = "", [CallerMemberName] string caller = "")
            => LogAsync(type: "INFO", caller: caller, message: $"{info}\n\t{ex}");
        public void Warn(string warning, [CallerMemberName] string caller = "")
            => LogAsync(type: "WARN", caller: caller, message: warning);
        public void Warn(Exception ex, string warning = "", [CallerMemberName] string caller = "")
            => LogAsync(type: "WARN", caller: caller, message: $"{warning}\n\t{ex}");
        public void Error(string error, [CallerMemberName] string caller = "")
            => LogAsync(type: "ERROR", caller: caller, message: error);
        public void Error(Exception ex, string error = "", [CallerMemberName] string caller = "")
            => LogAsync(type: "ERROR", caller: caller, message: $"{error}\n\t{ex}");
        public void UnhandledError(Exception ex, string error = "", [CallerMemberName] string caller = "")
            => LogSync(type: "ERROR", caller: caller, message: $"{error}\n\t{ex}");

        public void ShowLog()
        {
            var fileName = _writer.GetCurrentFileName();
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            // Let system choose application to start
            Process.Start(fileName);

            // Wait a little to give application time to open
            Thread.Sleep(2000);
        }

        private static string FormatMessage(string type, string caller, string message) =>
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}] [{type}] [{caller}] [{message}]";

        private void LogSync(string type, string caller, string message)
        {
            _semaphoreSlim.Wait();
            try
            {
                _writer.WriteLineToLog(FormatMessage(type, caller, message));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Writing to log file failed with the following exception:\n{e}");
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async void LogAsync(string type, string caller, string message, int attemptNumber = 0)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                await _writer.WriteLineToLogAsync(FormatMessage(type, caller, message));
            }
            catch (IOException e) when (e.GetType() != typeof(FileNotFoundException))
            {
                if (attemptNumber < 5) // check the attempt count to prevent a stack overflow exception
                {
                    // Log is likely in use by another process instance, so wait then try again
                    await Task.Delay(50);
                    LogAsync(type, caller, message, attemptNumber + 1);
                }
                else
                {
                    Debug.WriteLine($"Writing to log file failed after 5 attempts with the following exception:\n{e}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Writing to log file failed with the following exception:\n{e}");
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
