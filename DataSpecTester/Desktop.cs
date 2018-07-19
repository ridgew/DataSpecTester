using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using Gwsoft.DataSpec;

namespace DataSpecTester
{
    public partial class Desktop : Form
    {
        public Desktop()
        {
            InitializeComponent();

            System.Diagnostics.Trace.Listeners.Add(new TraceOut(WriteLog));
            TesterPlugConfig.Instance.Config();

            LoadPlug();

            //TesterPlugConfig.Instance.CurrentPlug.SetTraceWriter(WriteOutput);
            if (TesterPlugConfig.Instance.CurrentPlug != null)
                FillParseType(TesterPlugConfig.Instance.CurrentPlug.GetPlugHostTypes(typeof(ESPDataBase)), false);
        }

        #region 按钮操作


        private void btnParse_Click(object sender, EventArgs e)
        {
            if ((int)VieMode > 1) return;

            if (TesterPlugConfig.Instance.CurrentPlug == null)
            {
                ShowEror("没有加载任何解析插件在目录{0}中！", TesterPlugConfig.Instance.PlugInDirectory);
            }
            else
            {
                try
                {
                    TextBox targetTextBox = null;
                    bool blnSucess = false;
                    if (string.IsNullOrEmpty(cbxParseType.Text))
                    {

                        //new System.Threading.Thread(() =>
                        // {

                        string resultType = string.Empty;
                        blnSucess = TesterPlugConfig.Instance.CurrentPlug.Parse(GetCurrentViewBytes(out targetTextBox)
                                 , (VieMode == InterfaceViewMode.RequestView) ? tvRequestView : tvRespView,
                                 WriteOutput, out resultType);

                        //if (InvokeRequired)
                        //{
                        cbxParseType.Text = "";
                        cbxParseType.SelectedText = resultType;
                        //}

                        //}).Start();
                    }
                    else
                    {
                        blnSucess = TesterPlugConfig.Instance.CurrentPlug.TryParseAsType(GetCurrentViewBytes(out targetTextBox),
                             cbxParseType.Text,
                             (VieMode == InterfaceViewMode.RequestView) ? tvRequestView : tvRespView,
                             WriteOutput);
                    }

                    if (targetTextBox != null)
                    {
                        targetTextBox.BackColor = (blnSucess) ? System.Drawing.ColorTranslator.FromHtml("#D6F7D6") : System.Drawing.ColorTranslator.FromHtml("#FCCBCB");

                        if (blnSucess && VieMode == InterfaceViewMode.RequestView)
                        {
                            TreeNodeInstanceBind nodeBind = tvRequestView.Nodes[0].Tag as TreeNodeInstanceBind;
                            //保存请求对象引用
                            TesterPlugConfig.Instance.CurrentPlug.ExchangeStore("Plug_Current_Request", nodeBind.NodeItem);
                        }
                    }
                }
                catch (Exception plugExp)
                {
                    plugExp = SpecUtil.GetTriggerException(plugExp);
                    ShowEror("插件解析失败，错误消息及跟踪：\r\nMessage:{0} \r\nStackTrace:{1}",
                        plugExp.Message,
                        plugExp.StackTrace);
                }
            }
        }

        private void btnSendTest_Click(object sender, EventArgs e)
        {
            if (TesterPlugConfig.Instance.CurrentPlug == null)
            {
                ShowEror("没有加载任何解析插件在目录{0}中！", TesterPlugConfig.Instance.PlugInDirectory);
            }
            else
            {
                bool blnGetRemote = true;
                byte[] respBytes = new byte[0];
                try
                {
                    TesterPlugConfig.Instance.CurrentPlug.ExchangeStore("Plug_Current_ReadObjectFromStream", cbxReadObjectFromStream.Checked);

                    respBytes = TesterPlugConfig.Instance.CurrentPlug.GetTestResponse(SpecUtil.HexPatternStringToByteArray(tbxRequestBin.Text), WriteOutput);
                }
                catch (Exception plugExp)
                {
                    plugExp = SpecUtil.GetTriggerException(plugExp);
                    blnGetRemote = false;
                    ShowEror("插件测试失败，错误消息及跟踪：\r\nMessage:{0} \r\nStackTrace:{1}",
                        plugExp.Message,
                        plugExp.StackTrace);
                }

                if (blnGetRemote)
                {
                    tabControlData.SelectedTab = tabPageResp;
                    if (respBytes != null && respBytes.Length > 0)
                    {
                        if (!tbxHexview.Focused)
                        {
                            outPanel.SelectedTab = tabPageHexview;
                            tbxHexview.Focus();
                        }

                        //设置返回字节的16进制数据（无ASCII）
                        tbxRespBin.Text = SpecUtil.ByteArrayToHexString(respBytes);

                        tbxHexview.Text = SpecUtil.GetHexViewString(respBytes);
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            rtxLog.Clear();
            rtxOutput.Clear();
        }
        #endregion


        private InterfaceViewMode VieMode = InterfaceViewMode.RequestView;


        private void FileDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Desktop_DragEnter(object sender, DragEventArgs e)
        {
            FileDragEnter(sender, e);
        }

        private void Desktop_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string str in data)
                {
                    if (str.ToLower().EndsWith(".dll") || str.ToLower().EndsWith(".exe"))
                    {
                        WriteLog("解析程序集文件:{0}", str);

                        bool blnIsValid = true;
                        Type[] subTypes = GetAssemblyTypes(str, out blnIsValid);
                        if (blnIsValid)
                        {
                            if (subTypes.Length > 0)
                            {
                                FillParseType(subTypes, true);
                            }
                        }
                        else
                        {
                            MessageBox.Show(this, string.Format("程序集文件{0}无效！", str),
                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        WriteLog("解析完成，共获取类型:{0}", subTypes.Length);
                    }
                    break;
                }
            }
        }

        private void tbxHexview_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string str in data)
                {
                    tbxHexview.Text = SpecUtil.GetHexViewString(System.IO.File.ReadAllBytes(str));
                    break;
                }
            }
        }

        private void tbxHexview_DragEnter(object sender, DragEventArgs e)
        {
            FileDragEnter(sender, e);
        }

        byte[] GetCurrentViewBytes(out TextBox targetTextBox)
        {

            byte[] retBytes = new byte[0];
            targetTextBox = null;
            switch (VieMode)
            {
                case InterfaceViewMode.RequestView:
                    targetTextBox = tbxRequestBin;
                    retBytes = SpecUtil.HexPatternStringToByteArray(tbxRequestBin.Text);
                    break;

                case InterfaceViewMode.ResponseView:
                    targetTextBox = tbxRespBin;
                    retBytes = SpecUtil.HexPatternStringToByteArray(tbxRespBin.Text);
                    break;

                case InterfaceViewMode.OtherView:
                    break;

                default:
                    break;
            }
            return retBytes;
        }


        void WriteLog(string logFormat, params object[] args)
        {
            if (!rtxLog.Focused)
            {
                outPanel.SelectedTab = tabPageLog;
                //rtxLog.Focus();
            }
            rtxLog.AppendText(string.Format(logFormat, args) + "\r\n");
        }

        void WriteOutput(string logFormat, params object[] args)
        {
            if (!rtxOutput.Focused)
            {
                outPanel.SelectedTab = tabPageOutput;
                //rtxOutput.Focus();
            }
            rtxOutput.AppendText(string.Format(logFormat, args) + "\r\n");
        }


        Type[] GetAssemblyTypes(string asmFile, out bool isValidFile)
        {
            isValidFile = true;
            Type[] retTypes = Type.EmptyTypes;
            try
            {
                Assembly asm = Assembly.LoadFile(asmFile);
                Type baseType = typeof(ESPDataBase);
                //Type objTransType = typeof(ObjectTransferOrderAttribute);
                List<Type> subTypeList = new List<Type>();
                foreach (Type t in asm.GetTypes())
                {
                    if (t.IsSubclassOf(baseType) && !t.IsAbstract && t.IsPublic)
                    {
                        subTypeList.Add(t);
                    }
                }
                retTypes = subTypeList.ToArray();
            }
            catch (Exception)
            {
                isValidFile = false;
            }
            return retTypes;
        }

        private void btnVieHex_Click(object sender, EventArgs e)
        {
            if (!tbxHexview.Focused)
            {
                outPanel.SelectedTab = tabPageHexview;
                tbxHexview.Focus();
            }

            switch (VieMode)
            {
                case InterfaceViewMode.RequestView:
                    tbxHexview.Text = SpecUtil.HexPatternStringToByteArray(tbxRequestBin.Text).GetHexViewString();
                    break;

                case InterfaceViewMode.ResponseView:
                    tbxHexview.Text = SpecUtil.HexPatternStringToByteArray(tbxRespBin.Text).GetHexViewString();
                    break;

                case InterfaceViewMode.OtherView:
                    break;

                default:
                    break;
            }

        }

        private void btnGetDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();

            DialogResult dResult = d.ShowDialog(this);
            if (dResult == DialogResult.OK)
            {
                tbxPlugIn.Text = d.SelectedPath;
            }

        }


        private void tabControlData_Selected(object sender, TabControlEventArgs e)
        {
            VieMode = (InterfaceViewMode)e.TabPageIndex;
            //if (e.TabPageIndex < 2) Desktop_Resize(this, EventArgs.Empty);
        }


        internal static void ShowEror(Form target, string msgFormat, params object[] fmtArgs)
        {
            MessageBox.Show(target, string.Format(msgFormat, fmtArgs),
                            target.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static Desktop _instance;
        static Desktop Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        internal static void ShowEror(string msgFormat, params object[] fmtArgs)
        {
            Form target = Desktop.Instance;
            MessageBox.Show(target, string.Format(msgFormat, fmtArgs),
                            target.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            //ListViewItem item = new ListViewItem("√ 名称 程序集 说明描述内容".Split(' '));

            //listviewPlugIn.Items.Add(item);

            //WriteLog("ListView Count:{0}", listviewPlugIn.Items.Count);

            //listviewPlugIn.Update();
        }

        void LoadPlug()
        {
            string[] listStr = new string[4];
            foreach (Type plugType in TesterPlugConfig.Instance.PlugTypes)
            {
                listStr = new string[4];
                if (TesterPlugConfig.Instance.CurrentPlug == null)
                {
                    listStr[0] = "□";
                }
                else
                {
                    listStr[0] = (plugType == TesterPlugConfig.Instance.CurrentPlug.GetType()) ? "■" : "□";
                }
                listStr[1] = plugType.FullName;
                listStr[2] = plugType.Assembly.FullName;
                listStr[3] = "Ease Test PlugIn";

                ListViewItem item = new ListViewItem(listStr);
                listviewPlugIn.Items.Add(item);
            }
        }

        void FillParseType(Type[] subTypes, bool clearOld)
        {
            if (clearOld) cbxParseType.Items.Clear();
            foreach (Type subType in subTypes)
            {
                cbxParseType.Items.Add(subType.AssemblyQualifiedName);
            }
        }

        void HandlerReFactoryText(object sender, KeyEventArgs e)
        {
            //重排
            if (e.Control && e.KeyCode == Keys.F2)
            {
                TextBox tbxSender = (TextBox)sender;
                tbxSender.Text = RefactoryHexString(tbxSender.Text);
                WriteOutput("执行文本重排完毕！");
            }

            //还原
            if (e.Control && e.KeyCode == Keys.F8)
            {
                TextBox tbxRawTarget = (VieMode == InterfaceViewMode.RequestView) ? tbxRequestBin : tbxRespBin;
                TreeView tvTarget = (VieMode == InterfaceViewMode.RequestView) ? tvRequestView : tvRespView;
                TreeNodeInstanceBind nodeBind = tvTarget.Nodes[0].Tag as TreeNodeInstanceBind;
                if (nodeBind != null)
                {
                    tbxRawTarget.Text = SpecUtil.ByteArrayToHexString(((ESPDataBase)nodeBind.NodeItem).GetNetworkBytes());
                    WriteOutput("执行文本还原完毕！");
                }
            }
        }

        #region 调试数据保存
        private void tbxHexview_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.HexAsciiView = tbxHexview.Text;
            Properties.Settings.Default.Save();
        }

        private void tbxRequestBin_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RequestHex = tbxRequestBin.Text;
            Properties.Settings.Default.Save();
        }

        private void tbxRespBin_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ResponseHex = tbxRespBin.Text;
            Properties.Settings.Default.Save();
        }

        private void cbxParseType_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.HexParseType = cbxParseType.Text;
            Properties.Settings.Default.Save();
        }
        #endregion



        private void Desktop_Load(object sender, EventArgs e)
        {
            Instance = this;

            string strIP = null;
            IPHostEntry HosyEntry = Dns.GetHostEntry((Dns.GetHostName()));
            if (HosyEntry.AddressList.Length > 0)
            {
                foreach (IPAddress ip in HosyEntry.AddressList)
                {
                    strIP = ip.ToString();
                    cmbInterfaces.Items.Add(strIP);
                }

                if (cmbInterfaces.Items.Count == 1)
                {
                    cmbInterfaces.Text = strIP;
                }
            }

            cbxCharset.Items.Clear();
            foreach (System.Text.EncodingInfo eInfo in System.Text.Encoding.GetEncodings())
            {
                ////Traditional Chinese (ISO 2022) 50229
                cbxCharset.Items.Add(string.Format("{0} ({1}) {2}", eInfo.DisplayName, eInfo.Name, eInfo.CodePage));
            }

        }

        private void Desktop_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bContinueCapturing)
            {
                if (mainSocket != null)
                {
                    mainSocket.Close();
                }
            }
        }

        private void tabControlData_SelectedIndexChanged(object sender, EventArgs e)
        {
            //请求和回应数据视图
            if (tabControlData.SelectedIndex < 2)
            {
                Desktop_Resize(sender, e);
            }
        }

        private void Desktop_Resize(object sender, EventArgs e)
        {
            tvRequestView.Height = splitContainerRequest.Height - tbxReqTvDetail.Height - 2;
            splitContainerRequest.SplitterDistance = 308;

            tvRespView.Height = splitContainerResponse.Height - tbxRespTvDetail.Height - 2;
            splitContainerResponse.SplitterDistance = 308;

            cbxParseType.Width = this.Width - cbxParseType.Location.X - 15;
            cbxCharset.Width = this.Width - cbxCharset.Location.X - 25;

            //btnViewData
            if (tabControlData.SelectedTab.Equals(tabPageViewData))
            {
                //宽度最大化
                tbxInput.Width = this.Width - tbxInput.Left - 30;
                tbxBinView.Width = tbxInput.Width;

                int spaceHeight = tbxBinView.Top - cmbxType.Bottom;
                //高度平分
                if (spaceHeight > 50)
                {
                    int addHeight = spaceHeight - 50;
                    tbxBinView.Top -= addHeight;
                    tbxBinView.Height += spaceHeight - 50;
                }
                else
                {
                    //tbxInput.Height = 87;
                    tbxBinView.Top = 143;
                    tbxBinView.Height = 140;
                    tbxBinView.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                }
            }
        }

        private void TextBoxContentClearHandler(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = string.Empty;
        }

    }
}
