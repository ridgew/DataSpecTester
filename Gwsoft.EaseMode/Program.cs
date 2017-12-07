#if UnitTest
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gwsoft.DataSpec;

namespace Gwsoft.EaseMode
{
    class Program
    {
        static int Main(String[] args)
        {
            //if (args.ESP_Length == 0)
            //{
            //    Gwsoft.Configuration.ImplementStateAttribute.ReportAssemblyState(typeof(Program).Assembly, Console.Out);
            //    //byte[] byteTest = new byte[] { 0x00, 0x02 };
            //    //Console.WriteLine(BitConverter.ToString(SpecUtil.ReverseBytes(byteTest)));

            //    //Console.WriteLine("{0:x2}", (short)2);
            //}
            //else
            //{

            /*
                softwareid 00 00 0E 52 => 3666
             */

            string testDir = @"D:\DevRoot\易致平臺解決專案\src\Gwsoft.EaseMode\bin\Debug";
            string[] testFiles = @"Gwsoft.EaseMode.NetworkSwitchRequest.bin
Gwsoft.EaseMode.NetworkSwitchResponse.bin
Gwsoft.EaseMode.ApplicationResponse.bin"
                .Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                int iCurrentTestIdx = 2;

                string testFileName = Path.Combine(testDir, testFiles[iCurrentTestIdx]);
                if (!File.Exists(testFileName)) return -1;

                Type testType = Type.GetType(Path.GetFileNameWithoutExtension(testFileName));
                if (testType.IsSubclassOf(typeof(ESPDataBase)))
                {
                    byte[] testBytes = System.IO.File.ReadAllBytes(testFileName);

                   object result =  typeof(ESPDataBase).GetMethod("IsValidNetworkBytes", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                        .MakeGenericMethod(testType)
                        .Invoke(null, new object[] { testBytes, Console.Out });
                   if (!Convert.ToBoolean(result))
                   {
                       Console.Error.WriteLine("解析失败！");
                       return -1;
                   }
                   else
                   {
                       Console.WriteLine("PASS !");
                   }
                }
                else
                {
                    Console.WriteLine("Error file type or name !");
                }
            //}

                return 0;

        }
    }
}
#endif