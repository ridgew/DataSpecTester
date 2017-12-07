namespace DataSpecTester
{
    partial class ExtractCapForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbxNumBegin = new System.Windows.Forms.TextBox();
            this.tbxNumEnd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxSrcHost = new System.Windows.Forms.TextBox();
            this.tbxDestHost = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbxProtocol = new System.Windows.Forms.TextBox();
            this.tbxNodeName = new System.Windows.Forms.TextBox();
            this.btnDoCopy = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnExchange = new System.Windows.Forms.Button();
            this.cbxAutoSwap = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "起止节点文本编号#";
            // 
            // tbxNumBegin
            // 
            this.tbxNumBegin.Location = new System.Drawing.Point(147, 27);
            this.tbxNumBegin.Name = "tbxNumBegin";
            this.tbxNumBegin.Size = new System.Drawing.Size(59, 21);
            this.tbxNumBegin.TabIndex = 1;
            this.tbxNumBegin.Text = "1";
            // 
            // tbxNumEnd
            // 
            this.tbxNumEnd.AutoCompleteCustomSource.AddRange(new string[] {
            "$",
            "*"});
            this.tbxNumEnd.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.tbxNumEnd.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbxNumEnd.Location = new System.Drawing.Point(234, 27);
            this.tbxNumEnd.Name = "tbxNumEnd";
            this.tbxNumEnd.Size = new System.Drawing.Size(55, 21);
            this.tbxNumEnd.TabIndex = 1;
            this.tbxNumEnd.Text = "$";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(214, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "数据发送方地址及端口";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "数据接收方地址及端口";
            // 
            // tbxSrcHost
            // 
            this.tbxSrcHost.Location = new System.Drawing.Point(147, 62);
            this.tbxSrcHost.Name = "tbxSrcHost";
            this.tbxSrcHost.Size = new System.Drawing.Size(191, 21);
            this.tbxSrcHost.TabIndex = 1;
            this.tbxSrcHost.Text = "219.141.174.203:1430";
            // 
            // tbxDestHost
            // 
            this.tbxDestHost.Location = new System.Drawing.Point(147, 89);
            this.tbxDestHost.Name = "tbxDestHost";
            this.tbxDestHost.Size = new System.Drawing.Size(191, 21);
            this.tbxDestHost.TabIndex = 1;
            this.tbxDestHost.Text = "192.168.8.119:1666";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(48, 121);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "IP数据协议类型";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "数据节点文本前缀";
            // 
            // tbxProtocol
            // 
            this.tbxProtocol.Location = new System.Drawing.Point(147, 118);
            this.tbxProtocol.Name = "tbxProtocol";
            this.tbxProtocol.Size = new System.Drawing.Size(59, 21);
            this.tbxProtocol.TabIndex = 1;
            this.tbxProtocol.Text = "TCP/IP";
            // 
            // tbxNodeName
            // 
            this.tbxNodeName.Location = new System.Drawing.Point(147, 145);
            this.tbxNodeName.Name = "tbxNodeName";
            this.tbxNodeName.Size = new System.Drawing.Size(191, 21);
            this.tbxNodeName.TabIndex = 1;
            this.tbxNodeName.Text = "MessageLength";
            // 
            // btnDoCopy
            // 
            this.btnDoCopy.Location = new System.Drawing.Point(77, 218);
            this.btnDoCopy.Name = "btnDoCopy";
            this.btnDoCopy.Size = new System.Drawing.Size(105, 23);
            this.btnDoCopy.TabIndex = 3;
            this.btnDoCopy.Text = "数据保存到文件";
            this.btnDoCopy.UseVisualStyleBackColor = true;
            this.btnDoCopy.Click += new System.EventHandler(this.btnDoCopy_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(209, 218);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "取消";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnExchange
            // 
            this.btnExchange.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExchange.Location = new System.Drawing.Point(344, 66);
            this.btnExchange.Name = "btnExchange";
            this.btnExchange.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnExchange.Size = new System.Drawing.Size(27, 40);
            this.btnExchange.TabIndex = 4;
            this.btnExchange.Text = "交换";
            this.btnExchange.UseVisualStyleBackColor = true;
            this.btnExchange.Click += new System.EventHandler(this.btnExchange_Click);
            // 
            // cbxAutoSwap
            // 
            this.cbxAutoSwap.AutoSize = true;
            this.cbxAutoSwap.Location = new System.Drawing.Point(299, 29);
            this.cbxAutoSwap.Name = "cbxAutoSwap";
            this.cbxAutoSwap.Size = new System.Drawing.Size(72, 16);
            this.cbxAutoSwap.TabIndex = 5;
            this.cbxAutoSwap.Text = "自动交换";
            this.cbxAutoSwap.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.SystemColors.Info;
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label7.ForeColor = System.Drawing.Color.DarkRed;
            this.label7.Location = new System.Drawing.Point(14, 175);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(370, 35);
            this.label7.TabIndex = 6;
            this.label7.Text = "  终止符： *表示结束，$会话终止，其他数字为中止节点编号\r\n  自动交换： 提取区间内的一连串交互数据（文本交互最佳）";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ExtractCapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 262);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cbxAutoSwap);
            this.Controls.Add(this.btnExchange);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDoCopy);
            this.Controls.Add(this.tbxNumEnd);
            this.Controls.Add(this.tbxDestHost);
            this.Controls.Add(this.tbxSrcHost);
            this.Controls.Add(this.tbxNodeName);
            this.Controls.Add(this.tbxProtocol);
            this.Controls.Add(this.tbxNumBegin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExtractCapForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "同级二进制数据提取(HEX文本)";
            this.Load += new System.EventHandler(this.ExtractCapForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxNumBegin;
        private System.Windows.Forms.TextBox tbxNumEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxSrcHost;
        private System.Windows.Forms.TextBox tbxDestHost;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbxProtocol;
        private System.Windows.Forms.TextBox tbxNodeName;
        private System.Windows.Forms.Button btnDoCopy;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnExchange;
        private System.Windows.Forms.CheckBox cbxAutoSwap;
        private System.Windows.Forms.Label label7;
    }
}