using System;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace DataSpecTester
{
    public partial class ExtractCapForm : Form
    {
        public ExtractCapForm()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void ExtractCapForm_Load(object sender, EventArgs e)
        {
            tbxNumEnd.Focus();
        }

        private TreeView srcTreeview = null;
        private int colLevel = 2; //同类节点绑定数据的层次

        /// <summary>
        /// 拷贝数据的目标节点深度
        /// </summary>
        public int TargetLevel
        {
            get { return colLevel; }
            set { colLevel = value; }
        }

        public void SetSrcData(TreeView tarTree, string strNumBegin, string strSrcHost, string strDestHost, string strProtocol, string strNodeName)
        {
            srcTreeview = tarTree;

            if (!string.IsNullOrEmpty(strNumBegin)) tbxNumBegin.Text = strNumBegin;
            if (!string.IsNullOrEmpty(strSrcHost)) tbxSrcHost.Text = strSrcHost;
            if (!string.IsNullOrEmpty(strDestHost)) tbxDestHost.Text = strDestHost;
            if (!string.IsNullOrEmpty(strProtocol)) tbxProtocol.Text = strProtocol;
            if (!string.IsNullOrEmpty(strNodeName)) tbxNodeName.Text = strNodeName;
        }

        private void btnExchange_Click(object sender, EventArgs e)
        {
            string strTemp = tbxDestHost.Text;
            tbxDestHost.Text = tbxSrcHost.Text;
            tbxSrcHost.Text = strTemp;
        }

        private void AutoSwap<T>(ref T swapA, ref T swapB)
        {
            //btnExchange_Click(this, EventArgs.Empty);
            T objTemp = swapB;
            swapB = swapA;
            swapA = objTemp;

            System.Diagnostics.Trace.TraceInformation("*自动交换{0} <-> {1}", swapA, swapB);
        }

        private void btnDoCopy_Click(object sender, EventArgs e)
        {
            if (srcTreeview == null) return;

            string strStart = tbxNumBegin.Text.Trim();
            string strEnd = tbxNumEnd.Text.Trim();
            string strProtocol = tbxProtocol.Text;
            string strHost = tbxSrcHost.Text.Trim();
            string strDest = tbxDestHost.Text.Trim(); 

            MemoryStream ms = new MemoryStream(); 
            TreeNodeCollection itemCol = srcTreeview.Nodes;
            bool blnFindBeginNode = false;

            for (int i = 0, j = itemCol.Count; i < j; i++)
            {
                TreeNode itemNode = itemCol[i];
                TreeNode targetNode = null;

                string strNodeNum = itemNode.Text.Substring(1, itemNode.Text.IndexOf(' ') - 1).TrimStart('0');
                if (!blnFindBeginNode && strNodeNum.Equals(strStart)) blnFindBeginNode = true;
                if (!blnFindBeginNode) continue;

                //#017 => TCP of 192.168.8.119:1035 -> 118.123.205.211:8000
                //-------Found MATCH---Length:5----
                //(#017 => TCP of 192.168.8.119:1035 -> 118.123.205.211:8000)(017)(TCP)(192.168.8.119:1035)(118.123.205.211:8000)
                Match m = Regex.Match(itemNode.Text, Desktop.ITEM_NODE_PATTERN);

                #region 自动交换客户端与服务器端
                if (m.Success && itemNode.ForeColor.Equals(Desktop.NODE_NOT_EMPTY_COLOR))
                {
                    if (strProtocol.Equals(m.Groups[2].Value + "/IP") &&
                        strHost.Equals(m.Groups[4].Value) && strDest.Equals(m.Groups[3].Value)
                     )
                    {
                        if (cbxAutoSwap.Checked)
                        {
                            AutoSwap<string>(ref strHost, ref strDest);
                        }
                        else
                        {
                            //智能终止（请求或返回数据结束）
                            if (strEnd.Equals("$")) break;
                        }
                    }
                }
                #endregion
                
                #region 结束点为数字时中断
                if (strEnd != "*" && strEnd != "$" && 
                    Convert.ToInt64(strNodeNum) > Convert.ToInt64(strEnd))
                {
                     break;
                }
                #endregion

                #region 不符合主客户端参数跳过分析节点
                if (m.Success)
                {
                    if (!strProtocol.Equals(m.Groups[2].Value + "/IP")
                         || !strHost.Equals(m.Groups[3].Value)
                         || !strDest.Equals(m.Groups[4].Value))
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
                #endregion

                #region 设置目标节点targetNode
                targetNode = FindTargetNodeAt(itemNode, TargetLevel, tbxNodeName.Text.Trim());
                #endregion

                if (targetNode != null)
                {
                    System.Diagnostics.Trace.TraceInformation("*找到匹配节点:{0}, Level:{1}, 数据项:#{2}", targetNode.Text, targetNode.Level, strNodeNum);
                    Desktop.AppendTreeNodeBinData(targetNode, ms);
                }

            }

            if (ms.Length > 0)  Desktop.SaveBinDataToFileDialog(this, ms.ToArray());
            ms.Dispose();

        }


        private TreeNode FindTargetNodeAt(TreeNode cNode, int level, string strPre)
        {
            TreeNode tarNode = null;
            for (int i = 0, j = cNode.Nodes.Count; i < j; i++)
            {
                if (cNode.Nodes[i].Level < level)
                {
                    //System.Diagnostics.Trace.WriteLine(string.Format("Find Child Node of {0}", cNode.Nodes[i].Text));
                    tarNode = FindTargetNodeAt(cNode.Nodes[i], level, strPre);
                }

                if (tarNode != null) break;

                if (cNode.Nodes[i].Level == level &&
                    cNode.Nodes[i].Text.StartsWith(strPre))
                {
                    tarNode = cNode.Nodes[i];
                    break;
                }

            }
            return tarNode;
        }


    }
}
