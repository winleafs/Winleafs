using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

//Source: https://github.com/antonpup/Aurora/blob/master/Project-Aurora/Project-Aurora/Utils/MemoryReader.cs

namespace Winleafs.Wpf.Helpers
{
    public class MemoryReader : IDisposable
    {
        private readonly IntPtr processHandle;
        private readonly IntPtr moduleAddress;

        #region MemoryReading
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        public static byte[] ReadMemory(IntPtr targetProcess, IntPtr address, int readCount, out int bytesRead)
        {
            byte[] buffer = new byte[readCount];

            ReadProcessMemory(targetProcess, address, buffer, readCount, out bytesRead);

            return buffer;
        }

        [DllImport("kernel32.dll")]
        private static extern int CloseHandle(IntPtr objectHandle);
        #endregion

        public MemoryReader(Process process)
        {
            processHandle = OpenProcess(0x0010, false, process.Id);
            moduleAddress = process.MainModule.BaseAddress;
        }

        public MemoryReader(string processName)
        {
            var process = Process.GetProcessesByName(processName).FirstOrDefault();
            processHandle = OpenProcess(0x0010, false, process.Id);
            moduleAddress = process.MainModule.BaseAddress;
        }

        public int ReadInt(IntPtr address)
        {
            int bytesRead = 0;

            return BitConverter.ToInt32(ReadMemory(processHandle, address, 4, out bytesRead), 0);
        }

        public int ReadInt(int baseAddress, int[] offsets)
        {
            return ReadInt(CalculateAddress(baseAddress, offsets));
        }

        public long ReadLong(IntPtr address)
        {
            int bytesRead = 0;

            return BitConverter.ToInt64(ReadMemory(processHandle, address, 8, out bytesRead), 0);
        }

        public long ReadLong(int baseAddress, int[] offsets)
        {
            return ReadLong(CalculateAddress(baseAddress, offsets));
        }

        public float ReadFloat(IntPtr address)
        {
            int bytesRead = 0;

            return BitConverter.ToSingle(ReadMemory(processHandle, address, 4, out bytesRead), 0);
        }

        public float ReadFloat(int baseAddress, int[] offsets)
        {
            return ReadFloat(CalculateAddress(baseAddress, offsets));
        }

        public double ReadDouble(IntPtr address)
        {
            int bytesRead = 0;

            return BitConverter.ToDouble(ReadMemory(processHandle, address, 8, out bytesRead), 0);
        }

        public double ReadDouble(int baseAddress, int[] offsets)
        {
            return ReadDouble(CalculateAddress(baseAddress, offsets));
        }

        private IntPtr CalculateAddress(int baseAddress, int[] offsets)
        {
            IntPtr currentAddress = IntPtr.Add(moduleAddress, baseAddress);

            if (offsets.Length > 0)
            {
                currentAddress = (IntPtr)ReadInt(currentAddress);

                for (int x = 0; x < offsets.Length - 1; x++)
                {
                    currentAddress = IntPtr.Add(currentAddress, offsets[x]);
                    currentAddress = (IntPtr)ReadInt(currentAddress);
                }

                currentAddress = IntPtr.Add(currentAddress, offsets[offsets.Length - 1]);
            }

            return currentAddress;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CloseHandle(processHandle);
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
