using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace OK_API
{
    public class OpenKneeboardAPI
    {
        private static string DllPath { get; set; } = @"C:\Path\To\OpenKneeboard_CAPI64.dll"; // Replace with actual path

        static OpenKneeboardAPI()
        {
            var OK_exePath = @"c:\Program Files\OpenKneeboard\bin\";
            var OpenKneeboard_CAPI64 = $@"{OK_exePath}\OpenKneeboard_CAPI32.dll";
            if (!File.Exists(OpenKneeboard_CAPI64))
            {
                Console.WriteLine($"OpenKneeboard could not be found under: {OpenKneeboard_CAPI64}");
                return;
            }
            OPENKNEEBOARD_CAPI = (OPENKNEEBOARD_CAPIDelegate)
                FunctionLoader.LoadFunction<OPENKNEEBOARD_CAPIDelegate>(
                    OpenKneeboard_CAPI64, "OpenKneeboard_send_utf8");
        }
        public delegate void OPENKNEEBOARD_CAPIDelegate(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] messageName,
            int messageNameByteCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] messageValue,
            int messageValueByteCount
        );

        static private OPENKNEEBOARD_CAPIDelegate OPENKNEEBOARD_CAPI;

        public void sendCommand(string cmdName, string cmdVar)
        {
            // Convert strings to UTF-8 byte arrays directly
            var cmdNameU8 = Encoding.UTF8.GetBytes(cmdName);
            var cmdVarU8 = Encoding.UTF8.GetBytes(cmdVar);
            // No need to convert back to strings, use the byte arrays directly
            OPENKNEEBOARD_CAPI(cmdNameU8, cmdNameU8.Length, cmdVarU8, cmdVarU8.Length);
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