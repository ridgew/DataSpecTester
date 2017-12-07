#if TEST
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Gwsoft.DataSpec2.NUnitTest
{
    public class IniSpecFileTest
    {

        public void SpecReadInContextTest()
        {
            SpecFile spec = SpecFile.ParseContractFile("..\\..\\Contracts\\test.RnR", SpecFileFormat.Ini);
            string testBytesStr = @"03 F2 00 00 00 08 00 00 00 04 C8 02 00 B8";
            byte[] readBytes = TypeCache.HexPatternStringToByteArray(testBytesStr);

            MemoryStream readSrc = new MemoryStream(readBytes);
            MemoryStream writeSrc = new MemoryStream();

            using (StreamContext sc = new StreamContext(spec, readSrc, writeSrc))
            {
                Dictionary<string, object> ReqDict = sc.GetContextRequest();
                if (!SpecData.ReadAs<short>(ReqDict, "ESP_SuccessFlag").Equals((short)1010))
                {
                    Console.WriteLine("*读取数据错误！");
                }
                else
                {
                    sc.SetPosition(0);

                    Dictionary<string, object> result = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
                    //result["ESP_SuccessFlag"] = (short)1010;
                    result["ESP_SuccessFlag"] = "Success";
                    result["ESP_CustomCode"] = (int)0008;
                    result["ESP_LeaveLength"] = (int)4;
                    result["ESP_TransferData"] = TypeCache.HexPatternStringToByteArray(@"C8 02 00 B8");
                    sc.WriteContextResponse(result);
                }

                if (writeSrc.Length > 0)
                {
                    string result = TypeCache.ByteArrayToHexString(writeSrc.ToArray());
                    Console.WriteLine(result);
                    Debug.Assert(testBytesStr == result);
                }
                else
                {
                    Console.WriteLine("*没有写入数据！");
                }
                writeSrc.Dispose();
            }


        }

        public void EnumContractTest()
        {
            EnumContract en = new EnumContract { BaseType = "short", TypeName = "StatusCode" };
            en.SetEnumItem("Success", 1010);
            en.SetEnumItem("Failed", -1);

            object val = en.GetEnumUnderlyingValue("Success");
            Debug.Assert(val.GetType().Equals(typeof(short)));
            Debug.Assert(val.Equals((short)1010), "枚举值定义不等于1010");

            //Console.WriteLine(EnumContract.DataEquals(1010, en["Success"].Value));
        }

        public void SpecImportTest()
        {
            SpecFile file = SpecFile.ParseContractFile("..\\..\\Contracts\\ResponseBase.spec", SpecFileFormat.Ini);
            Console.WriteLine(file.AllImportContracts.Length);
        }

        public void ExpressionParseTest()
        {
            SpecFile spec = SpecFile.ParseContractFile("..\\..\\Contracts\\test.RnR", SpecFileFormat.Ini);
            string testBytesStr = @"03 F2 00 00 00 08 00 00 00 04 C8 02 00 B8";
            byte[] readBytes = TypeCache.HexPatternStringToByteArray(testBytesStr);
            MemoryStream readSrc = new MemoryStream(readBytes);

            using (StreamContext sc = new StreamContext(spec, readSrc))
            {
                Dictionary<string, object> ReqDict = sc.GetContextRequest();
                SpecExpression specExp = new SpecExpression("{ ESP_CustomeCode == 8 }");
                Debug.Assert(specExp.IsPass(sc));

                specExp = new SpecExpression("{ ESP_SuccessFlag == 1010 }");
                Debug.Assert(specExp.IsPass(sc));

                specExp = new SpecExpression("{ ESP_LeaveLength == 4 }");
                Debug.Assert(specExp.IsPass(sc));
                
            }
        }
    }
}
#endif