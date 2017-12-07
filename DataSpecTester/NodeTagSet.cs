using System;
using System.Windows.Forms;
using Gwsoft.DataSpec;

namespace DataSpecTester
{
    public partial class NodeTagSet : Form
    {
        public NodeTagSet()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取或设置节点绑定的对象
        /// </summary>
        public TreeNodeInstanceBind BindInstance { get; set; }

        /// <summary>
        /// 根字节流绑定对象
        /// </summary>
        public HybridStream RootStream { get; set; } 

        private void NodeTagSet_Load(object sender, EventArgs e)
        {
            if (BindInstance == null) return;

            tbxNodeType.Text = BindInstance.NodeType.AssemblyQualifiedName;
            tbxNodeSubType.Text = BindInstance.SubType != null ? BindInstance.SubType.AssemblyQualifiedName : "";

            tbxStartIndex.Text = BindInstance.StoreIndex.ToString();
            tbxStoreLength.Text = BindInstance.StoreLength.ToString();

            cbxIsESPData.Checked = BindInstance.IsESPData;
            if (BindInstance.NodeType.IsSubclassOf(typeof(ESPDataBase)))
            {
                ESPDataBase instance = BindInstance.NodeItem as ESPDataBase;
                if (instance != null)
                {
                    tbxNodeItem.Text = SpecUtil.ByteArrayToHexString(instance.GetNetworkBytes());
                }
            }
            else if (BindInstance.NodeType.Equals(typeof(byte[])))
            {
                byte[] targetBytes = BindInstance.NodeItem as byte[];
                if (targetBytes != null && targetBytes.Length > 0)
                {
                    tbxNodeItem.Text = SpecUtil.ByteArrayToHexString(targetBytes);
                }
            }
            else
            {
                tbxNodeItem.Text = BindInstance.NodeItem.ToString();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (BindInstance == null) BindInstance = new TreeNodeInstanceBind();

            BindInstance.StoreIndex = Convert.ToInt64(tbxStartIndex.Text.Trim());
            BindInstance.StoreLength = Convert.ToInt64(tbxStoreLength.Text.Trim());

            BindInstance.TagModified = true;

            if (BindInstance.IsESPData)
            {
                ESPDataBase instance = Activator.CreateInstance(BindInstance.NodeType) as ESPDataBase;
                if (instance != null)
                {
                    try
                    {
                        if (cbxReadFromRoot.Checked && RootStream != null)
                        {
                            RootStream.Result.Position = BindInstance.StoreIndex;
                            SpecUtil.BindFromNetworkStream(instance, RootStream.Result, BindInstance.StoreIndex, false);
                            BindInstance.StoreLength = instance.GetContentLength();
                        }
                        else
                        {
                            if (tbxNodeItem.Text.Trim() != string.Empty)
                            {
                                SpecUtil.BindFromNetworkStream(instance,
                                  SpecUtil.HexPatternStringToByteArray(tbxNodeItem.Text.Trim()).AsMemoryStream(),
                                  0, false);
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        Desktop.ShowEror(this, "Message:{0}\r\n{1}", exp.Message, exp.StackTrace);
                    }
                    BindInstance.NodeItem = instance;
                }
            }
            else
            {
                if (cbxReadFromRoot.Checked)
                {
                    RootStream.Result.Position = BindInstance.StoreIndex;
                    if (BindInstance.NodeType.Equals(typeof(byte[])))
                    {
                        BindInstance.NodeItem = RootStream.ReadSpecialLength((int)BindInstance.StoreLength);
                    }
                    else
                    {
                        BindInstance.NodeItem = RootStream.ReadAsValue(BindInstance.NodeType);
                    }
                }
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnNewESPWizard_Click(object sender, EventArgs e)
        {

        }

        private void btnESPVerify_Click(object sender, EventArgs e)
        {

        }

    }
}
