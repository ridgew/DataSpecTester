using System;
using System.IO;
using System.Windows.Forms;
using Gwsoft.DataSpec;

namespace DataSpecTester
{
    partial class Desktop
    {
        #region 协议对象解析
        TreeView GetCurrentTreeView()
        {
            return (VieMode == InterfaceViewMode.RequestView) ? tvRequestView : tvRespView;
        }

        private void espMItemCreateNew_Click(object sender, EventArgs e)
        {
            TreeView tv = GetCurrentTreeView();
            TreeNode node = tv.SelectedNode;
            TreeNodeInstanceBind bind = node.Tag as TreeNodeInstanceBind;
            if (bind == null)
            {
                ShowEror("节点绑定Tag对象类型不为{0}，操作不支持！", typeof(TreeNodeInstanceBind).FullName);
            }
            else
            {
                TreeNodeInstanceBind newBind = new TreeNodeInstanceBind
                {
                    IsArrayItem = bind.IsArrayItem,
                    IsESPData = bind.IsESPData,
                    IsFirstNode = bind.IsFirstNode,
                    StoreIndex = bind.StoreIndex,
                    NodeType = bind.NodeType,
                    SubType = bind.SubType
                };

                if (bind.IsESPData)
                {
                    ESPDataBase instance = Activator.CreateInstance(bind.NodeType) as ESPDataBase;
                    newBind.NodeItem = instance;
                    newBind.StoreLength = instance.GetContentLength();

                    node.Tag = newBind;

                    TesterPlugConfig.Instance.CurrentPlug.RefreshSelectNode(node, WriteLog);
                }
                else
                {
                    ShowEror("节点绑定Tag对象的值不为{0}，操作不支持！", typeof(ESPDataBase).FullName);
                }
            }
        }

        private void espMItemExtract_Click(object sender, EventArgs e)
        {
            TreeView tv = GetCurrentTreeView();
            TreeNode node = tv.SelectedNode;
            TreeNodeInstanceBind bind = node.Tag as TreeNodeInstanceBind;
            if (bind == null)
            {
                ShowEror("节点绑定Tag对象类型不为{0}，操作不支持！", typeof(TreeNodeInstanceBind).FullName);
            }
            else
            {
                byte[] fileBytes = new byte[0];

                if (bind.NodeType.IsSubclassOf(typeof(ESPDataBase)))
                {
                    ESPDataBase instance = bind.NodeItem as ESPDataBase;
                    if (instance != null)
                    {
                        fileBytes = instance.GetNetworkBytes();
                    }
                }
                else if (bind.NodeType.Equals(typeof(byte[])))
                {
                    fileBytes = bind.NodeItem as byte[];
                }
                if (fileBytes.Length > 0) SaveBinDataToFileDialog(this, fileBytes);
            }
        }

        private void espMItemSet_Click(object sender, EventArgs e)
        {
            TreeView tv = GetCurrentTreeView();
            TreeNode node = tv.SelectedNode;
            TreeNodeInstanceBind bind = node.Tag as TreeNodeInstanceBind;
            if (bind == null)
            {
                ShowEror("节点绑定Tag对象类型不为{0}，操作不支持！", typeof(TreeNodeInstanceBind).FullName);
            }
            else
            {
                NodeTagSet setForm = new NodeTagSet();
                TextBox targetTextBox = null;

                MemoryStream ms = GetCurrentViewBytes(out targetTextBox).AsMemoryStream();
                setForm.RootStream = new HybridStream(ms);

                TreeNode preNode = node.PrevNode;
                if (preNode != null && preNode.Tag is TreeNodeInstanceBind)
                {
                    TreeNodeInstanceBind preBind = preNode.Tag as TreeNodeInstanceBind;
                    if (preBind.TagModified)
                    {
                        bind.StoreIndex = preBind.StoreIndex + preBind.StoreLength;
                    }
                }

                setForm.BindInstance = bind;
                DialogResult dr = setForm.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    if (setForm.BindInstance != null)
                    {
                        node.Tag = setForm.BindInstance;
                        TesterPlugConfig.Instance.CurrentPlug.RefreshSelectNode(node, WriteLog);
                        WriteLog("刷新节点绑定:{0}", node.Text);
                    }
                }

                setForm.Dispose();
                ms.Dispose();
            }

        }

        private void espMItemUpdate_Click(object sender, EventArgs e)
        {
            TesterPlugConfig.Instance.CurrentPlug.RefreshTreeView(GetCurrentTreeView(), WriteLog);
        }

        internal PicViewForm viewForm = null;

        //查看图片数据
        private void espMItemViewPic_Click(object sender, EventArgs e)
        {
            TreeView tv = GetCurrentTreeView();
            TreeNode node = tv.SelectedNode;
            TreeNodeInstanceBind bind = node.Tag as TreeNodeInstanceBind;
            if (bind == null)
            {
                ShowEror("节点绑定Tag对象类型不为{0}，操作不支持！", typeof(TreeNodeInstanceBind).FullName);
            }
            else
            {
                if (bind.NodeType.Equals(typeof(Byte[])))
                {
                    if (viewForm == null)
                    {
                        //WriteLog("viewForm == null : {0}", viewForm == null);
                        viewForm = new PicViewForm();
                        viewForm.Owner = this;
                        viewForm.Show(this);
                    }
                    viewForm.PicBytes = bind.NodeItem as byte[];
                    viewForm.RefreshPic();
                    viewForm.Visible = true;
                }
            }

        }

        #endregion

        /// <summary>
        /// 报告字节索引位置
        /// </summary>
        private void ReportByteLocation(object sender, MouseEventArgs e)
        {

            TextBox tbxSender = (TextBox)sender;
            int lineNum = tbxSender.GetLineFromCharIndex(tbxSender.SelectionStart);
            int currentLineIdx = tbxSender.GetFirstCharIndexFromLine(lineNum);
            int currentLineEnd = tbxSender.SelectionStart + 3;
            //WriteLog("Index:{0}, End:{1}, SelectStart:{2}", currentLineIdx, currentLineEnd, tbxSender.SelectionStart);

            int resultIndex = lineNum * 16;

            if (currentLineEnd > currentLineIdx)
            {
                tbxSender.Select(currentLineIdx, currentLineEnd - currentLineIdx);

                resultIndex += tbxSender.SelectedText.Trim().Replace(" ", "").Length / 2 - 1;
                tbxSender.Select(currentLineEnd - 2, 2);

                if (tbxSender.SelectedText.StartsWith(" "))
                {
                    tbxSender.SelectionStart += 1;
                }

                if (tbxSender.SelectedText.EndsWith(" "))
                {
                    tbxSender.SelectionLength -= 1;
                }

                if (tbxSender.SelectionLength == 1)
                {
                    tbxSender.SelectionStart -= 1;
                    tbxSender.SelectionLength = 2;
                }
            }

            TextBox tbxTarget = (VieMode == InterfaceViewMode.RequestView) ? tbxReqTvDetail : tbxRespTvDetail;
            tbxTarget.Text = string.Format("当前字节索引:{0}(0x{1}), 行号:L{2}[{3}(0x{4})]",
                resultIndex,
                resultIndex.ToString("X8"),
                lineNum + 1,
                lineNum * 16, (lineNum * 16).ToString("X8"));

        }

        private TreeNode[] lastSelectedNode = new TreeNode[3];

        void ReportSelected(object sender, TreeViewEventArgs e)
        {
            TextBox tbxTarget = (VieMode == InterfaceViewMode.RequestView) ? tbxReqTvDetail : tbxRespTvDetail;
            TextBox tbxRawTarget = (VieMode == InterfaceViewMode.RequestView) ? tbxRequestBin : tbxRespBin;

            long[] resultRange = TesterPlugConfig.Instance.CurrentPlug.ReportNodeDetail((TreeView)sender, e.Node, (fmt, args) =>
            {
                tbxTarget.Text = string.Format(fmt, args);
            });

            #region 改变并还原上次选中节点的背景颜色
            e.Node.BackColor = System.Drawing.Color.YellowGreen;

            TreeNode lastNode = null;
            if (VieMode == InterfaceViewMode.RequestView)
            {
                lastNode = lastSelectedNode[0];
                lastSelectedNode[0] = e.Node;
            }
            else
            {
                lastNode = lastSelectedNode[1];
                lastSelectedNode[1] = e.Node;
            }

            if (lastNode != null)
            {
                lastNode.BackColor = System.Drawing.Color.Transparent;
            }
            #endregion

            if (e.Node.Level == 0)
            {
                tbxRawTarget.SelectAll();
                return;
            }

            int idxStart = (int)resultRange[0];
            int rangLen = (int)(resultRange[1] - resultRange[0]) + 1;
            if (rangLen > 0)
            {
                int realByteLen = 3, LineSplitLen = Environment.NewLine.Length, LineCharLen = 16 * realByteLen;
                int LineLen = LineCharLen + LineSplitLen;  //行字节长度
                int realStartOffset = idxStart * realByteLen;
                int targetLen = rangLen * realByteLen;


                //索引超过一行
                if (realStartOffset > LineLen)
                {
                    realStartOffset += (realStartOffset / LineCharLen) * LineSplitLen;  //计算跨行数
                }

                //总长度超过一行（增加行分隔长度)
                if (targetLen > LineLen)
                {
                    targetLen += (targetLen / LineCharLen) * LineSplitLen;
                }

                //超过两行或不满一行
                if (targetLen < LineLen || targetLen / LineLen > 2)
                {
                    int realEndOffset = realStartOffset + targetLen;
                    //开始位置大于单行长度的一半且末尾在新的一行有字符
                    if ((realStartOffset % LineLen) / (LineLen / 2) > 0
                        && realEndOffset % LineLen != 0 && (realStartOffset / LineLen) != (realEndOffset / LineLen))
                    {
                        targetLen += LineSplitLen;
                    }
                }

                WriteOutput("原始字节索引区域:[{0}({5})-{1}({6})], 可视化字符索引区域:[{3}-{4}]，总长度{2}",
                    idxStart, resultRange[1],
                    rangLen,
                    realStartOffset, realStartOffset + targetLen - 1,
                    idxStart.ToString("X2").PadLeft(8, '0'),
                    resultRange[1].ToString("X2").PadLeft(8, '0'));

                tbxRawTarget.Select(realStartOffset, targetLen);
                tbxRawTarget.ScrollToCaret();
                //tbxRawTarget.AutoScrollOffset = new System.Drawing.Point(realStartOffset, tbxRawTarget.SelectionLength);
            }

        }

    }
}
