namespace DataSpecTester
{
    partial class NodeTagSet
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbxIsESPData = new System.Windows.Forms.CheckBox();
            this.tbxStartIndex = new System.Windows.Forms.TextBox();
            this.tbxStoreLength = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxNodeType = new System.Windows.Forms.TextBox();
            this.tbxNodeSubType = new System.Windows.Forms.TextBox();
            this.tbxNodeItem = new System.Windows.Forms.TextBox();
            this.btnNewESPWizard = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnESPVerify = new System.Windows.Forms.Button();
            this.cbxReadFromRoot = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbxIsESPData
            // 
            this.cbxIsESPData.AutoSize = true;
            this.cbxIsESPData.Location = new System.Drawing.Point(26, 152);
            this.cbxIsESPData.Name = "cbxIsESPData";
            this.cbxIsESPData.Size = new System.Drawing.Size(126, 16);
            this.cbxIsESPData.TabIndex = 0;
            this.cbxIsESPData.Text = "是否是ESP对象封装";
            this.cbxIsESPData.UseVisualStyleBackColor = true;
            // 
            // tbxStartIndex
            // 
            this.tbxStartIndex.Location = new System.Drawing.Point(143, 15);
            this.tbxStartIndex.Name = "tbxStartIndex";
            this.tbxStartIndex.Size = new System.Drawing.Size(141, 21);
            this.tbxStartIndex.TabIndex = 1;
            this.tbxStartIndex.Text = "0";
            // 
            // tbxStoreLength
            // 
            this.tbxStoreLength.Location = new System.Drawing.Point(143, 47);
            this.tbxStoreLength.Name = "tbxStoreLength";
            this.tbxStoreLength.Size = new System.Drawing.Size(141, 21);
            this.tbxStoreLength.TabIndex = 1;
            this.tbxStoreLength.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "根容器中的开始索引";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "存储字节长度";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "节点数据类型";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "节点包含数据类型";
            // 
            // tbxNodeType
            // 
            this.tbxNodeType.Location = new System.Drawing.Point(143, 83);
            this.tbxNodeType.Name = "tbxNodeType";
            this.tbxNodeType.Size = new System.Drawing.Size(294, 21);
            this.tbxNodeType.TabIndex = 3;
            // 
            // tbxNodeSubType
            // 
            this.tbxNodeSubType.Location = new System.Drawing.Point(143, 115);
            this.tbxNodeSubType.Name = "tbxNodeSubType";
            this.tbxNodeSubType.Size = new System.Drawing.Size(294, 21);
            this.tbxNodeSubType.TabIndex = 3;
            // 
            // tbxNodeItem
            // 
            this.tbxNodeItem.BackColor = System.Drawing.SystemColors.Info;
            this.tbxNodeItem.Location = new System.Drawing.Point(27, 177);
            this.tbxNodeItem.Multiline = true;
            this.tbxNodeItem.Name = "tbxNodeItem";
            this.tbxNodeItem.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxNodeItem.Size = new System.Drawing.Size(410, 61);
            this.tbxNodeItem.TabIndex = 3;
            // 
            // btnNewESPWizard
            // 
            this.btnNewESPWizard.Enabled = false;
            this.btnNewESPWizard.Location = new System.Drawing.Point(158, 145);
            this.btnNewESPWizard.Name = "btnNewESPWizard";
            this.btnNewESPWizard.Size = new System.Drawing.Size(75, 23);
            this.btnNewESPWizard.TabIndex = 4;
            this.btnNewESPWizard.Text = "创建向导..";
            this.btnNewESPWizard.UseVisualStyleBackColor = true;
            this.btnNewESPWizard.Click += new System.EventHandler(this.btnNewESPWizard_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(143, 253);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "确定更新";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(251, 253);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消设置";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnESPVerify
            // 
            this.btnESPVerify.Enabled = false;
            this.btnESPVerify.Location = new System.Drawing.Point(251, 145);
            this.btnESPVerify.Name = "btnESPVerify";
            this.btnESPVerify.Size = new System.Drawing.Size(75, 23);
            this.btnESPVerify.TabIndex = 4;
            this.btnESPVerify.Text = "校验数据";
            this.btnESPVerify.UseVisualStyleBackColor = true;
            this.btnESPVerify.Click += new System.EventHandler(this.btnESPVerify_Click);
            // 
            // cbxReadFromRoot
            // 
            this.cbxReadFromRoot.AutoSize = true;
            this.cbxReadFromRoot.Location = new System.Drawing.Point(338, 149);
            this.cbxReadFromRoot.Name = "cbxReadFromRoot";
            this.cbxReadFromRoot.Size = new System.Drawing.Size(96, 16);
            this.cbxReadFromRoot.TabIndex = 5;
            this.cbxReadFromRoot.Text = "从绑定源读取";
            this.cbxReadFromRoot.UseVisualStyleBackColor = true;
            // 
            // NodeTagSet
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(456, 293);
            this.Controls.Add(this.cbxReadFromRoot);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnESPVerify);
            this.Controls.Add(this.btnNewESPWizard);
            this.Controls.Add(this.tbxNodeItem);
            this.Controls.Add(this.tbxNodeSubType);
            this.Controls.Add(this.tbxNodeType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbxStoreLength);
            this.Controls.Add(this.tbxStartIndex);
            this.Controls.Add(this.cbxIsESPData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NodeTagSet";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "选中节点值设置";
            this.Load += new System.EventHandler(this.NodeTagSet_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbxIsESPData;
        private System.Windows.Forms.TextBox tbxStartIndex;
        private System.Windows.Forms.TextBox tbxStoreLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxNodeType;
        private System.Windows.Forms.TextBox tbxNodeSubType;
        private System.Windows.Forms.TextBox tbxNodeItem;
        private System.Windows.Forms.Button btnNewESPWizard;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnESPVerify;
        private System.Windows.Forms.CheckBox cbxReadFromRoot;
    }
}