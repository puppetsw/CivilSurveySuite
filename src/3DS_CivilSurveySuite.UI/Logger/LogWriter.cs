using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using Microsoft.Win32.SafeHandles;

namespace _3DS_CivilSurveySuite.UI.Logger
{
    public class LogWriter : ILogWriter
    {
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint OPEN_CREATE = 4;
        private const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

        private readonly string _logFileName;
        private const string LogName = "debug.log";

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, FileShare dwShareMode,
            [Optional] IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes,
            [Optional] IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint SetFilePointer([In] SafeFileHandle hFile, int lDistanceToMove,
            [In, Optional] IntPtr lpDistanceToMoveHigh, SeekOrigin dwMoveMethod);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten, [Optional] IntPtr lpOverlapped);

        public LogWriter()
        {
            // Set logfile path to be current assembly directory.
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            var directory = Path.GetDirectoryName(path);
            _logFileName = Path.Combine(directory, LogName);
        }

        public async Task WriteLineToLogAsync(string text)
        {
            await Task.Run(() => WriteLineToLog(text));
        }

        public void WriteLineToLog(string text)
        {
            if (string.IsNullOrEmpty(_logFileName))
            {
                throw new ArgumentNullException(nameof(_logFileName), @"Log file was null or empty.");
            }

            using (SafeFileHandle fileHandle = CreateFile(_logFileName, GENERIC_WRITE, FileShare.Read, IntPtr.Zero,
                       OPEN_CREATE, FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero))
            {
                if (fileHandle.IsInvalid)
                {
                    Debug.WriteLine($"Error creating log file: {_logFileName}");
                    return;
                }

                byte[] buffer = Encoding.UTF8.GetBytes("\n" + text);
                SetFilePointer(fileHandle, 0, IntPtr.Zero, SeekOrigin.End);
                WriteFile(fileHandle, buffer, (uint)buffer.Length, out var dBytesWritten, IntPtr.Zero);

                Debug.WriteLine($"Logged event: {text}");
            }
        }

        public string GetCurrentFileName()
        {
            return _logFileName;
        }
    }
}
