using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gwsoft.DataSpec;

namespace DataSpecTester
{
    partial class Desktop
    {
        Encoding getCurrentEncoding()
        {
            Encoding enc = Encoding.Default;
            if (cbxCharset.Text.Trim() != string.Empty && cbxCharset.Text != "ANSI")
            {
                //French Canadian (DOS) 863
                //Traditional Chinese (ISO 2022) 50229
                //(Chinese Simplified (GB18030) 54936)(Chinese Simplified)( (GB18030))( 54936)
                //([\\w\\.\\s\\-_\\(\\)]+)(\\s\\([\\w\\.\\s\\-_]+\\))?(\\s\\d+)
                string codePattern = "([\\w\\.\\s\\-_\\(\\)]+)(\\s\\([\\w\\.\\s\\-_]+\\))?(\\s\\d+)";
                Match m = Regex.Match(cbxCharset.Text.Trim(), codePattern);
                if (m.Success)
                {
                    enc = Encoding.GetEncoding(Convert.ToInt32(m.Groups[3].Value.Trim()));
                }
                else
                {
                    enc = Encoding.GetEncoding(cbxCharset.Text.Trim());
                }
            }
            return enc;
        }

        #region 查看数据页面
        private void btnViewData_Click(object sender, EventArgs e)
        {
            byte[] buffer;
            if (cmbxType.Text == "string")
            {
                Encoding enc = getCurrentEncoding();
                cbxCharset.Text = enc.WebName;
                buffer = enc.GetBytes(tbxInput.Text.Trim());
            }
            else
            {
                if (cbxBase64Binary.Checked && Regex.IsMatch(tbxInput.Text.Trim(), "\\d+"))
                {
                    #region BCD编码
                    /*
                     //十进制转二进制
                     Console.WriteLine(Convert.ToString(69, 2));
                     //二进制转十进制
                     Console.WriteLine(Convert.ToInt32(”100111101″, 2));

                     //十进制转八进制
                     Console.WriteLine(Convert.ToString(69, 8));
                     //八进制转十进制
                     Console.WriteLine(Convert.ToInt32(”76″, 8));

                     //十进制转十六进制
                     Console.WriteLine(Convert.ToString(69, 16));
                     //十六进制转十进制
                     Console.WriteLine(Convert.ToInt32(”FF”, 16));
                       */
                    switch (cmbxType.Text.ToLower())
                    {
                        case "byte":
                            tbxBinView.Text = FormatBCDString(Convert.ToString(Convert.ToByte(tbxInput.Text.Trim()), 2));
                            break;
                        case "short":
                            tbxBinView.Text = FormatBCDString(Convert.ToString(Convert.ToInt16(tbxInput.Text.Trim()), 2));
                            break;
                        case "ushort":
                            tbxBinView.Text = FormatBCDString(Convert.ToString(Convert.ToUInt16(tbxInput.Text.Trim()), 2));
                            break;
                        case "int":
                            tbxBinView.Text = FormatBCDString(Convert.ToString(Convert.ToInt32(tbxInput.Text.Trim()), 2));
                            break;
                        case "uint":
                            tbxBinView.Text = FormatBCDString(Convert.ToString(Convert.ToUInt32(tbxInput.Text.Trim()), 2));
                            break;
                        case "long":
                            tbxBinView.Text = FormatBCDString(Convert.ToString(Convert.ToInt64(tbxInput.Text.Trim()), 2));
                            break;
                        default:
                            tbxBinView.Text = "不支持";
                            break;
                    }
                    #endregion
                    return;
                }

                #region 转换为字节序
                switch (cmbxType.Text.ToLower())
                {
                    case "byte":
                        buffer = BitConverter.GetBytes(Convert.ToByte(tbxInput.Text.Trim()));
                        break;
                    case "char":
                        buffer = BitConverter.GetBytes(Convert.ToChar(tbxInput.Text.Trim()));
                        break;
                    case "short":
                        buffer = BitConverter.GetBytes(Convert.ToInt16(tbxInput.Text.Trim()));
                        break;
                    case "ushort":
                        buffer = BitConverter.GetBytes(Convert.ToUInt16(tbxInput.Text.Trim()));
                        break;
                    case "int":
                        buffer = BitConverter.GetBytes(Convert.ToInt32(tbxInput.Text.Trim()));
                        break;
                    case "uint":
                        buffer = BitConverter.GetBytes(Convert.ToUInt32(tbxInput.Text.Trim()));
                        break;
                    case "long":
                        buffer = BitConverter.GetBytes(Convert.ToInt64(tbxInput.Text.Trim()));
                        break;
                    case "ulong":
                        buffer = BitConverter.GetBytes(Convert.ToUInt64(tbxInput.Text.Trim()));
                        break;
                    case "float":
                        buffer = BitConverter.GetBytes(Convert.ToSingle(tbxInput.Text.Trim()));
                        break;
                    case "bool":
                        buffer = BitConverter.GetBytes(Convert.ToBoolean(tbxInput.Text.Trim()));
                        break;
                    case "double":
                        buffer = BitConverter.GetBytes(Convert.ToDouble(tbxInput.Text.Trim()));
                        break;
                    default:
                        buffer = new byte[0];
                        break;
                }
                #endregion
            }

            if (buffer != null && buffer.Length > 0)
            {
                if (cbxRevert.Checked) buffer = SpecUtil.ReverseBytes(buffer);
                ReportHexView(buffer);
            }
            else
            {
                ShowEror("数据转换失败：\r\n{0}", tbxInput.Text.Trim());
            }
        }

        string FormatBCDString(string str)
        {
            if (str.Length < 4)
            {
                return str.PadLeft(4, '0');
            }
            else
            {
                string bcdStr = "";
                for (int i = str.Length - 1, j = 1; i >= 0; i--)
                {
                    bcdStr = str[i] + bcdStr;
                    if (j % 4 == 0 && j != str.Length)
                    {
                        bcdStr = " " + bcdStr;
                    }
                    j++;
                }

                int lev = str.Length % 4;
                if (lev == 0)
                {
                    return bcdStr;
                }
                else
                {
                    return new string('0', 4 - lev) + bcdStr;
                }
            }

        }

        private void ReportHexView(byte[] binDat)
        {
            tbxBinView.Text = "总长度：" + binDat.Length.ToString() + "字节"
                + Environment.NewLine + Environment.NewLine;

            byte[] ascByte = new byte[16];
            int lastRead = 0;

            StringBuilder sb = new StringBuilder();
            for (int i = 0, j = binDat.Length; i < j; i++)
            {
                if (i == 0)
                {
                    sb.Append("00000000  ");
                }

                sb.Append(binDat[i].ToString("X2") + " ");
                lastRead = i % 16;
                ascByte[lastRead] = binDat[i];

                if (i > 0 && (i + 1) % 8 == 0 && (i + 1) % 16 != 0)
                {
                    sb.Append(" ");
                }

                if (i > 0 && (i + 1) % 16 == 0)
                {
                    sb.Append(" ");
                    foreach (byte chrB in ascByte)
                    {
                        if (chrB >= 0x20 && chrB <= 0x7E) //[32,126]
                        {
                            sb.Append((char)chrB);
                        }
                        else
                        {
                            sb.Append('.');
                        }
                    }

                    if (i + 1 != j)
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append((i + 1).ToString("X2").PadLeft(8, '0') + "  ");
                    }
                }
            }

            if (lastRead < 15)
            {
                sb.Append(new string(' ', (15 - lastRead) * 3));
                if (lastRead < 8) sb.Append(" ");
                sb.Append(" ");
                for (int m = 0; m <= lastRead; m++)
                {
                    byte charL = ascByte[m];
                    if (charL >= 0x20 && charL <= 0x7E) //[32,126]
                    {
                        sb.Append((char)charL);
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }
            }

            tbxBinView.Text += sb.ToString();
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            //获取有效二进制序列数据
            string rawHexStr = tbxBinView.Text.Trim().Replace("\r", "");

            if (cbxBase64Binary.Checked && Regex.IsMatch(rawHexStr, "[01\\s]+"))
            {
                rawHexStr = rawHexStr.Replace(" ", "");
                #region BCD解码
                switch (cmbxType.Text.ToLower())
                {
                    case "byte":
                        tbxInput.Text = Convert.ToByte(rawHexStr, 2).ToString();
                        break;
                    case "short":
                        tbxInput.Text = Convert.ToInt16(rawHexStr, 2).ToString();
                        break;
                    case "ushort":
                        tbxInput.Text = Convert.ToUInt16(rawHexStr, 2).ToString();
                        break;
                    case "int":
                        tbxInput.Text = Convert.ToInt32(rawHexStr, 2).ToString();
                        break;
                    case "uint":
                        tbxInput.Text = Convert.ToUInt32(rawHexStr, 2).ToString();
                        break;
                    case "long":
                        tbxInput.Text = Convert.ToInt64(rawHexStr, 2).ToString();
                        break;
                    case "ulong":
                        tbxInput.Text = Convert.ToUInt64(rawHexStr, 2).ToString();
                        break;
                    default:
                        tbxInput.Text = "不支持";
                        break;
                }
                #endregion
                return;
            }

            string[] hexStr = rawHexStr.Split('\n');
            StringBuilder rb = new StringBuilder();
            foreach (string line in hexStr)
            {
                if (Regex.IsMatch(line, @"^[0-9a-f]{8}\s{2}", RegexOptions.IgnoreCase) && line.Length > 10)
                {
                    if (line.Length >= 58)
                    {
                        rb.Append(line.Substring(10, 48));
                    }
                    else
                    {
                        rb.Append(line.Substring(10));
                    }
                }
                else if (Regex.IsMatch(line, @"^([0-9a-f]{2}\s{1,2})+", RegexOptions.IgnoreCase))
                {
                    rb.Append(line);
                }
            }
            rawHexStr = rb.ToString().Replace(" ", "");

            int total = rawHexStr.Length;
            if (total % 2 == 0)
            {
                byte[] rawBin = new byte[total / 2];
                for (int i = 0; i < total; i = i + 2)
                {
                    rawBin[i / 2] = Convert.ToByte(int.Parse(rawHexStr.Substring(i, 2),
                        NumberStyles.AllowHexSpecifier));
                }

                if (cbxRevert.Checked) rawBin = SpecUtil.ReverseBytes(rawBin);

                if (cmbxType.Text == "string")
                {
                    Encoding enc = getCurrentEncoding();
                    tbxInput.Text = enc.GetString(rawBin);
                }
                else
                {
                    switch (cmbxType.Text.ToLower())
                    {
                        case "byte":
                            tbxInput.Text = BitConverter.ToChar(rawBin, 0).ToString();
                            break;
                        case "short":
                            tbxInput.Text = BitConverter.ToInt16(rawBin, 0).ToString();
                            break;
                        case "ushort":
                            tbxInput.Text = BitConverter.ToUInt16(rawBin, 0).ToString();
                            break;
                        case "int":
                            tbxInput.Text = BitConverter.ToInt32(rawBin, 0).ToString();
                            break;
                        case "uint":
                            tbxInput.Text = BitConverter.ToUInt32(rawBin, 0).ToString();
                            break;
                        case "long":
                            tbxInput.Text = BitConverter.ToInt64(rawBin, 0).ToString();
                            break;
                        case "ulong":
                            tbxInput.Text = BitConverter.ToUInt64(rawBin, 0).ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void btnBase64En_Click(object sender, EventArgs e)
        {
            string strBase64 = tbxInput.Text.Trim();
            Encoding Enc = getCurrentEncoding();
            WriteLog("Base64 Encoding => CodePage:{0}, Name:{1}", Enc.CodePage, Enc.WebName);
            byte[] bytes = Enc.GetBytes(strBase64);
            strBase64 = Convert.ToBase64String(bytes);
            tbxBinView.Text = strBase64;
        }

        private void btnBase64De_Click(object sender, EventArgs e)
        {
            string strSourceText = tbxBinView.Text.Trim();
            byte[] bytes = Convert.FromBase64String(strSourceText);
            if (cbxBase64Binary.Checked && bytes.Length > 0)
            {
                SaveBinDataToFileDialog(this, bytes);
            }
            else
            {
                Encoding Enc = getCurrentEncoding();
                WriteLog("Base64 Decoding => CodePage:{0}, Name:{1}", Enc.CodePage, Enc.WebName);
                tbxInput.Text = Enc.GetString(bytes);
            }
        }


        private void btnSaveToBin_Click(object sender, EventArgs e)
        {
            byte[] fileBytes = SpecUtil.HexPatternStringToByteArray(tbxBinView.Text);
            if (fileBytes != null && fileBytes.Length > 0)
            {
                SaveBinDataToFileDialog(this, fileBytes);
            }
            else
            {
                ShowEror("{0}", "未能获取二进制文件内容！");
            }
        }
        #endregion

        void HexStringTextBox(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string str in data)
                {
                    FileStream fs = new FileStream(str, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    if (fs.Length > int.MaxValue)
                    {
                        ShowEror("文件大小{0}超过可以显示的最大长度[{1}]!", fs.Length, int.MaxValue);
                        return;
                    }

                    byte[] fileBin = new byte[fs.Length];
                    fs.Read(fileBin, 0, fileBin.Length);
                    fs.Close();
                    fs.Dispose();
                    TextBox tbxTarget = sender as TextBox;
                    if (tbxTarget != null)
                    {
                        if (!cbxBase64Binary.Checked)
                        {
                            tbxTarget.Text = SpecUtil.ByteArrayToHexString(fileBin);
                        }
                        else
                        {
                            if (tbxTarget.Name.Equals(tbxInput.Name))
                            {
                                //读取为文本文件
                                tbxInput.Text = getCurrentEncoding().GetString(fileBin);
                            }
                            else if (tbxTarget.Name.Equals(tbxBinView.Name))
                            {
                                //显示二进制序列的base64编码
                                tbxTarget.Text = Convert.ToBase64String(fileBin, Base64FormattingOptions.InsertLineBreaks);
                            }
                        }
                    }

                    break;
                }
            }
        }
    }
}
