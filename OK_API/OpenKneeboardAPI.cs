using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace OK_API
{
    public class OpenKneeboardAPI
    {
        private static string DllPath { get; set; } = @"C:\Path\To\OpenKneeboard_CAPI32.dll"; // Replace with actual path

        static OpenKneeboardAPI()
        {
            var OK_exePath = @"c:\Program Files\OpenKneeboard\bin\";
            var OpenKneeboard_CAPI = $@"{OK_exePath}\OpenKneeboard_CAPI32.dll";
            if (!File.Exists(OpenKneeboard_CAPI))
            {
                Console.WriteLine($"OpenKneeboard could not be found under: {OpenKneeboard_CAPI}");
                return;
            }
            OPENKNEEBOARD_CAPI = (OPENKNEEBOARD_CAPIDelegate)
                FunctionLoader.LoadFunction<OPENKNEEBOARD_CAPIDelegate>(
                    OpenKneeboard_CAPI, "OpenKneeboard_send_utf8");
        }
        public delegate void OPENKNEEBOARD_CAPIDelegate(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] messageName,
            ulong messageNameByteCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] messageValue,
            ulong messageValueByteCount
        );

        static private OPENKNEEBOARD_CAPIDelegate OPENKNEEBOARD_CAPI;

        public void SendCommand(string cmdName, string cmdVar)
        {
            var utf8 = Encoding.UTF8;
            var messageName = utf8.GetBytes(cmdName);
            var messageValue = utf8.GetBytes(cmdVar);
            OPENKNEEBOARD_CAPI(messageName, (ulong)messageName.Length, messageValue, (ulong)messageValue.Length);
        }
    }

    public class FunctionLoader
    {
        [DllImport("Kernel32.dll")]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        public static Delegate LoadFunction<T>(string dllPath, string functionName)
        {
            var hModule = LoadLibrary(dllPath);
            var functionAddress = GetProcAddress(hModule, functionName);
            return Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(T));
        }
    }
}