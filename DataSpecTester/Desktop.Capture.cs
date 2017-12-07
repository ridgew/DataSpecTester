using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using DataSpecTester.Cap;
using DataSpecTester.Html;
using System.IO;
using System.Net;
using Gwsoft.DataSpec;
using System.Text.RegularExpressions;

namespace DataSpecTester
{
    partial class Desktop
    {
        #region 数据采集
        private System.Net.Sockets.Socket mainSocket;      //The socket which captures all incoming packets
        private byte[] byteData = new byte[4096];
        private bool bContinueCapturing = false;            //A flag to check if packets are to be captured or not

        private delegate void AddTreeNode(TreeNode node);

        private void OnAddTreeNode(TreeNode node)
        {
            NEXT_NODE_ITEM++;

            node.Text = "#" + NEXT_NODE_ITEM.ToString("000") + " => " + node.Text;
            treeView.Nodes.Add(node);
        }

        //假定网关目标地址
        private string _AssumeMaskAddress = string.Empty;

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cmbInterfaces.Text == "")
            {
                MessageBox.Show("请选择一个网卡接口进行抓包！", Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _AssumeMaskAddress = System.Text.RegularExpressions.Regex.Replace(cmbInterfaces.Text, "\\.(\\d{1,3})$", ".255");

                if (!bContinueCapturing)
                {
                    //Start capturing the packets...

                    btnStart.Text = "暂停(&S)";

                    bContinueCapturing = true;

                    //For sniffing the socket to capture the packets has to be a raw socket, with the
                    //address family being of type internetwork, and protocol being IP
                    mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);

                    //Bind the socket to the selected IP address
                    mainSocket.Bind(new IPEndPoint(IPAddress.Parse(cmbInterfaces.Text), 0));

                    //Set the socket  options
                    mainSocket.SetSocketOption(SocketOptionLevel.IP,            //Applies only to IP packets
                                               SocketOptionName.HeaderIncluded, //Set the include the header
                                               true);                           //option to true

                    byte[] incoming = new byte[4] { 1, 0, 0, 0 };
                    byte[] outgoing = new byte[4] { 1, 0, 0, 0 };

                    //Socket.IOControl is analogous to the WSAIoctl method of Winsock 2
                    mainSocket.IOControl(IOControlCode.ReceiveAll, //Equivalent to SIO_RCVALL constant of Winsock 2
                                         incoming,
                                         outgoing);

                    //Start receiving the packets asynchronously
                    mainSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                        new AsyncCallback(OnReceive), mainSocket);
                }
                else
                {
                    btnStart.Text = "抓包(&S)";
                    bContinueCapturing = false;

                    //To stop capturing the packets close the socket
                    mainSocket.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                int nReceived = mainSocket.EndReceive(ar);
                //Analyze the bytes received...
                ParseData(byteData, nReceived);

                if (bContinueCapturing)
                {
                    byteData = new byte[4096];

                    //Another call to BeginReceive so that we continue to receive the incoming
                    //packets
                    mainSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                        new AsyncCallback(OnReceive), null);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                WriteLog("{0}\r\n{1}", ex.Message, ex.StackTrace);
                //MessageBox.Show(ex.Message, "MJsniffer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RightClickSwitch(object sender, MouseEventArgs e)
        {

            //WriteLog("{0}", sender);
            //WriteLog("{0}", e.Button);
            CheckBox cbxTarget = (CheckBox)sender;
            if (e.Button == MouseButtons.Right)
            {
                cbxTarget.Visible = false;

                cbxUdpOnly.Visible = !cbxTcpOnly.Visible;
                cbxTcpOnly.Visible = !cbxUdpOnly.Visible;
            }
        }

        private void ParseData(byte[] byteData, int nReceived)
        {
            IPHeader ipHeader = new IPHeader(byteData, nReceived);

            //忽略广播包
            if (cbxIgnoreBroadCast.Checked &&
                (ipHeader.DestinationAddress.Equals(IPAddress.Broadcast)
                || ipHeader.DestinationAddress.ToString().Equals(_AssumeMaskAddress))
            )
            {
                return;
            }

            string tcpPortString = string.Empty;

            if (cbxTcpOnly.Checked || cbxUdpOnly.Checked)
            {
                if (cbxTcpOnly.Checked &&
                    ipHeader.ProtocolType != IpProtocol.TCP)
                {
                    return;
                }

                if (cbxUdpOnly.Checked &&
                    ipHeader.ProtocolType != IpProtocol.UDP)
                {
                    return;
                }

                IPAddress destIp = null;
                string targetIp = tbxDest.Text.Trim();

                if (!string.IsNullOrEmpty(targetIp))
                {
                    int idx = targetIp.IndexOf(':');
                    if (idx != -1)
                    {
                        tcpPortString = targetIp.Substring(idx + 1);
                        if (tcpPortString.Equals("*")) tcpPortString = string.Empty;

                        targetIp = targetIp.Substring(0, idx);
                    }

                    if (!targetIp.Equals("*"))
                    {
                        //符合指定ip地址
                        if (IPAddress.TryParse(targetIp, out destIp) &&
                            !(ipHeader.DestinationAddress.Equals(destIp)
                            || ipHeader.SourceAddress.Equals(destIp))
                        )
                        {
                            return;
                        }
                    }
                }
            }

            TreeNode rootNode = new TreeNode();
            //Since all protocol packets are encapsulated in the IP datagram
            //so we start by parsing the IP header and see what protocol data
            //is being carried by it
            TreeNode ipNode = MakeIPTreeNode(ipHeader);
            rootNode.Nodes.Add(ipNode);

            //Now according to the protocol being carried by the IP datagram we parse 
            //the data field of the datagram
            switch (ipHeader.ProtocolType)
            {
                case IpProtocol.TCP:

                    TCPHeader tcpHeader = new TCPHeader(ipHeader.Data,              //IPHeader.Data stores the data being 
                        //carried by the IP datagram
                                                        ipHeader.MessageLength);//Length of the data field 

                    //符合特定端口
                    if (tcpPortString != string.Empty &&
                        !(tcpPortString.Equals(tcpHeader.DestinationPort)
                        || tcpPortString.Equals(tcpHeader.SourcePort))
                    )
                    {
                        return;
                    }

                    if (tcpHeader.MessageLength > 0)
                    {
                        rootNode.ForeColor = NODE_NOT_EMPTY_COLOR; // System.Drawing.ColorTranslator.FromHtml("#009900");
                        //rootNode.NodeFont = new System.Drawing.Font("verdana", 9, System.Drawing.FontStyle.Bold);
                    }

                    TreeNode tcpNode = MakeTCPTreeNode(tcpHeader);
                    rootNode.Nodes.Add(tcpNode);

                    //If the port is equal to 53 then the underlying protocol is DNS
                    //Note: DNS can use either TCP or UDP thats why the check is done twice
                    if (tcpHeader.DestinationPort == "53" || tcpHeader.SourcePort == "53")
                    {
                        TreeNode dnsNode = MakeDNSTreeNode(tcpHeader.Data, (int)tcpHeader.MessageLength);
                        rootNode.Nodes.Add(dnsNode);
                    }

                    break;

                case IpProtocol.UDP:

                    UDPHeader udpHeader = new UDPHeader(ipHeader.Data,              //IPHeader.Data stores the data being 
                        //carried by the IP datagram
                                                       (int)ipHeader.MessageLength);//Length of the data field                    

                    TreeNode udpNode = MakeUDPTreeNode(udpHeader);

                    rootNode.Nodes.Add(udpNode);

                    //If the port is equal to 53 then the underlying protocol is DNS
                    //Note: DNS can use either TCP or UDP thats why the check is done twice
                    if (udpHeader.DestinationPort == "53" || udpHeader.SourcePort == "53")
                    {

                        TreeNode dnsNode = MakeDNSTreeNode(udpHeader.Data,
                            //Length of UDP header is always eight bytes so we subtract that out of the total 
                            //length to find the length of the data
                                                           Convert.ToInt32(udpHeader.Length) - 8);
                        rootNode.Nodes.Add(dnsNode);
                    }

                    break;

                case IpProtocol.Unknown:
                    break;
            }

            AddTreeNode addTreeNode = new AddTreeNode(OnAddTreeNode);

            rootNode.Text = ipHeader.ProtocolType.ToString() + " of "
                + ipHeader.SourceAddress.ToString() + ":" + ipHeader.SourcePort + " -> "
                + ipHeader.DestinationAddress.ToString() + ":" + ipHeader.DestinationPort;

            //Thread safe adding of the nodes
            treeView.Invoke(addTreeNode, new object[] { rootNode });
        }

        #region 界面辅助
        //Helper function which returns the information contained in the IP header as a
        //tree node
        private TreeNode MakeIPTreeNode(IPHeader ipHeader)
        {
            TreeNode ipNode = new TreeNode(string.Format("{0}/IP {1}", ipHeader.ProtocolType, ipHeader.Version));
            ipNode.Nodes.Add("Header Length: " + ipHeader.HeaderLength);
            ipNode.Nodes.Add("Differntiated Services: " + ipHeader.DifferentiatedServices);

            TreeNode dataNode = new TreeNode("Message/Total Length: " + ipHeader.MessageLength + "/" + ipHeader.TotalLength);
            //dataNode.ToolTipText = SpecUtil.GetHexViewString(ipHeader.Data);
            dataNode.ForeColor = System.Drawing.Color.Blue;
            dataNode.Tag = ipHeader.Data;
            ipNode.Nodes.Add(dataNode);

            ipNode.Nodes.Add("Identification: " + ipHeader.Identification);
            ipNode.Nodes.Add("Flags: " + ipHeader.Flags);
            ipNode.Nodes.Add("Fragmentation Offset: " + ipHeader.FragmentationOffset);
            ipNode.Nodes.Add("Time to live: " + ipHeader.TTL);

            ipNode.Nodes.Add("Checksum: " + ipHeader.Checksum);
            //ipNode.Nodes.Add("Source: " + ipHeader.SourceAddress.ToString());
            //ipNode.Nodes.Add("Destination: " + ipHeader.DestinationAddress.ToString());

            return ipNode;
        }

        //Helper function which returns the information contained in the TCP header as a
        //tree node
        private TreeNode MakeTCPTreeNode(TCPHeader tcpHeader)
        {
            TreeNode tcpNode = new TreeNode("TCP Data: (" + tcpHeader.MessageLength + ")");
            tcpNode.Nodes.Add("Sequence Number: " + tcpHeader.SequenceNumber);

            if (tcpHeader.AcknowledgementNumber != "")
                tcpNode.Nodes.Add("Acknowledgement Number: " + tcpHeader.AcknowledgementNumber);

            tcpNode.Nodes.Add("Header Length: " + tcpHeader.HeaderLength);
            tcpNode.Nodes.Add("Flags: " + tcpHeader.Flags);
            tcpNode.Nodes.Add("Window Size: " + tcpHeader.WindowSize);
            tcpNode.Nodes.Add("Checksum: " + tcpHeader.Checksum);

            if (tcpHeader.UrgentPointer != "")
                tcpNode.Nodes.Add("Urgent Pointer: " + tcpHeader.UrgentPointer);

            TreeNode dataNode = new TreeNode("MessageLength: " + tcpHeader.MessageLength);
            dataNode.ForeColor = System.Drawing.Color.Blue;

            dataNode.Tag = tcpHeader.Data;
            tcpNode.Nodes.Add(dataNode);

            return tcpNode;
        }

        //Helper function which returns the information contained in the UDP header as a
        //tree node
        private TreeNode MakeUDPTreeNode(UDPHeader udpHeader)
        {
            return new TreeNode(string.Format("UDP Length: {0}, Checksum: {1}", udpHeader.Length, udpHeader.Checksum));
        }

        //Helper function which returns the information contained in the DNS header as a
        //tree node
        private TreeNode MakeDNSTreeNode(byte[] byteData, int nLength)
        {
            DNSHeader dnsHeader = new DNSHeader(byteData, nLength);

            TreeNode dnsNode = new TreeNode("DNS");
            dnsNode.Nodes.Add("Identification: " + dnsHeader.Identification);
            dnsNode.Nodes.Add("Flags: " + dnsHeader.Flags);
            dnsNode.Nodes.Add("Questions: " + dnsHeader.TotalQuestions);
            dnsNode.Nodes.Add("Answer RRs: " + dnsHeader.TotalAnswerRRs);
            dnsNode.Nodes.Add("Authority RRs: " + dnsHeader.TotalAuthorityRRs);
            dnsNode.Nodes.Add("Additional RRs: " + dnsHeader.TotalAdditionalRRs);

            return dnsNode;
        }
        #endregion

        private void btnCapClear_Click(object sender, EventArgs e)
        {
            treeView.Nodes.Clear();
            NEXT_NODE_ITEM = 0;
        }

        private HtmlDataProvider dataProvider = new HtmlDataProvider();

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            #region 改变并还原上次选中节点的背景颜色
            e.Node.BackColor = System.Drawing.Color.YellowGreen;
            TreeNode lastNode = lastSelectedNode[2];
            if (lastNode != null)
            {
                lastNode.BackColor = System.Drawing.Color.Transparent;
            }
            lastSelectedNode[2] = e.Node;
            #endregion

            object nodeTag = e.Node.Tag;
            if (nodeTag != null && nodeTag.GetType().Equals(typeof(byte[])))
            {
                rtxOutput.Clear();
                byte[] bindBytes = (byte[])nodeTag;
                WriteOutput("{0}", SpecUtil.GetHexViewString(bindBytes));

                dataProvider.RawBytes = bindBytes;
                htmlBrowser.Navigate(dataProvider);

                SetContextMenuItem(true, true, true, true);
            }
            else
            {
                SetContextMenuItem(false, false, true, false);
            }

        }

        private void SetContextMenuItem(params bool[] itemState)
        {
            if (itemState != null && itemState.Length > 0)
            {
                int targetIdx = 0, offset = 0;

                for (int i = 0, j = datCapMenu.Items.Count;
                    i < j && (i + offset) < itemState.Length;
                    i++)
                {
                    if (datCapMenu.Items[i] is ToolStripSeparator)
                    {
                        targetIdx += 1;
                        offset -= 1;
                        continue;
                    }

                    datCapMenu.Items[targetIdx].Enabled = itemState[i + offset];
                    targetIdx++;
                }
            }
        }

        #region 静态功能函数
        /// <summary>
        /// 下一个节点编号
        /// </summary>
        internal int NEXT_NODE_ITEM = 0;

        /// <summary>
        /// 数据项匹配模式
        /// </summary>
        internal const string ITEM_NODE_PATTERN = "#(\\d+)\\s=>\\s(\\w+)\\sof\\s(\\d+\\.\\d+\\.\\d+\\.\\d+:\\d+)\\s->\\s(\\d+\\.\\d+\\.\\d+\\.\\d+:\\d+)";

        /// <summary>
        /// 包含有效数据的节点文本颜色
        /// </summary>
        internal static System.Drawing.Color NODE_NOT_EMPTY_COLOR = System.Drawing.Color.DarkGreen;

        /// <summary>
        /// 重排HEX字符为16字节的标准格式
        /// </summary>
        public static string RefactoryHexString(string srcStr)
        {
            return SpecUtil.ByteArrayToHexString(SpecUtil.HexPatternStringToByteArray(srcStr));
        }

        /// <summary>
        /// 保存字节序列为文件对话框
        /// </summary>
        public static void SaveBinDataToFileDialog(IWin32Window owner, byte[] fileBytes)
        {
            SaveFileDialog FDialog = new SaveFileDialog();

            FDialog.Filter = "二进制文件 (*.bin)|*.bin|所有文件 (*.*)|*.*";
            FDialog.FilterIndex = 1;
            FDialog.RestoreDirectory = true;

            if (FDialog.ShowDialog(owner) == DialogResult.OK)
            {
                using (Stream fStream = FDialog.OpenFile())
                {
                    fStream.Write(fileBytes, 0, fileBytes.Length);
                    fStream.Close();
                }
            }
            FDialog.Dispose();
        }

        /// <summary>
        /// 复制节点二进制数据到剪贴板(HEX格式)
        /// </summary>
        /// <param name="sNode">The s node.</param>
        /// <param name="blnAppend">if set to <c>true</c> [BLN append].</param>
        public static void CopyTreeNodeHexData(TreeNode sNode, bool blnAppend)
        {
            if (sNode != null)
            {
                object nodeTag = sNode.Tag;
                if (nodeTag != null && nodeTag.GetType().Equals(typeof(byte[])))
                {
                    if (!blnAppend)
                    {
                        Clipboard.SetData(DataFormats.Text, SpecUtil.ByteArrayToHexString((byte[])nodeTag) + Environment.NewLine);
                    }
                    else
                    {
                        Clipboard.SetData(DataFormats.Text,
                            Clipboard.GetData(DataFormats.Text).ToString() +
                            SpecUtil.ByteArrayToHexString((byte[])nodeTag) + Environment.NewLine);
                    }
                }
            }
        }

        /// <summary>
        /// 附加节点上的字节序列到模板流
        /// </summary>
        public static void AppendTreeNodeBinData(TreeNode sNode, Stream targetStream)
        {
            if (sNode != null)
            {
                object nodeTag = sNode.Tag;
                if (nodeTag != null && nodeTag.GetType().Equals(typeof(byte[])))
                {
                    byte[] dat = (byte[])nodeTag;
                    if (dat.Length > 0)
                    {
                        targetStream.Write(dat, 0, dat.Length);
                    }
                }
            }
        }

        #endregion

        private void tStripItemCopy_Click(object sender, EventArgs e)
        {
            CopyTreeNodeHexData(treeView.SelectedNode, false);
        }

        private void tStripItemAppendCopy_Click(object sender, EventArgs e)
        {
            CopyTreeNodeHexData(treeView.SelectedNode, true);
        }

        private void tStripItemDeleteNode_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode.Remove();
        }

        private void tStripItemCopyText_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                Clipboard.SetData(DataFormats.Text, treeView.SelectedNode.Text);
            }
        }

        //提取节点中的消息数据
        private void tStripItemExtract_Click(object sender, EventArgs e)
        {
            TreeNode selNode = treeView.SelectedNode;
            if (selNode == null) return;

            ExtractCapForm extrForm = new ExtractCapForm();
            extrForm.TargetLevel = selNode.Level;

            string strNumBegin = null, strHost = null, strDest = null, strProtocol = null;
            string strNodeName = selNode.Text;
            int idx = strNodeName.IndexOf(':');
            if (idx > 0) strNodeName = strNodeName.Substring(0, idx);

            //WriteOutput("选中节点深度为:{0}", selNode.Level);

            if (selNode.Level == 2)
            {
                string itemText = selNode.Parent.Parent.Text;
                Match m = Regex.Match(itemText, ITEM_NODE_PATTERN);
                if (m.Success)
                {
                    //#017 => TCP of 192.168.8.119:1035 -> 118.123.205.211:8000
                    //-------Found MATCH---Length:5----
                    //(#017 => TCP of 192.168.8.119:1035 -> 118.123.205.211:8000)(017)(TCP)(192.168.8.119:1035)(118.123.205.211:8000)
                    strNumBegin = m.Groups[1].Value.TrimStart('0');
                    strProtocol = m.Groups[2].Value + "/IP";
                    strHost = m.Groups[3].Value;
                    strDest = m.Groups[4].Value;
                }
            }

            extrForm.SetSrcData(treeView, strNumBegin, strHost, strDest, strProtocol, strNodeName);
            extrForm.ShowDialog(this);
        }

        private void tStripItemRemoveEmpty_Click(object sender, EventArgs e)
        {
            List<TreeNode> nodes2remove = new List<TreeNode>();
            for (int i = 0, j = treeView.Nodes.Count; i < j; i++)
            {
                if (!treeView.Nodes[i].ForeColor.Equals(NODE_NOT_EMPTY_COLOR))
                {
                    nodes2remove.Add(treeView.Nodes[i]);
                }
            }

            for (int m = 0, n = nodes2remove.Count; m < n; m++)
            {
                nodes2remove[m].Remove();
            }
            WriteLog("* 清除无效数据节点，已删除{0}项。", nodes2remove.Count);

        }
        #endregion
    }
}
