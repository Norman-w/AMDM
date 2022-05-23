namespace FakeHISServer
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.createMedicineOrderBtn = new System.Windows.Forms.Button();
            this.changeBalanceStatusBtn = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.columnId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnBalance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnFulfill = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.changeFulfillStatusBtn = new System.Windows.Forms.Button();
            this.printMedicineOrderBtn = new System.Windows.Forms.Button();
            this.reloadDataBtn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.sendMsgTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.receiveMsgTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.sendToAMDMBtn = new System.Windows.Forms.Button();
            this.logTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.sendToHTTPServerBtn = new System.Windows.Forms.Button();
            this.requestCombox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.AMDMAddressTextbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.httpServerAddressTextbox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.reStartHpptServerBtn = new System.Windows.Forms.Button();
            this.printerListCombox = new System.Windows.Forms.ComboBox();
            this.sqlServerAddressTextbox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.sqlServerPortTextbox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.sqlServerDatabaseTextbox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.sqlServerUserTextbox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.sqlServerPassTextbox = new System.Windows.Forms.TextBox();
            this.updateSQLserverSettingAndTryConnectBtn = new System.Windows.Forms.Button();
            this.createAllCurrentInventoryMedicineOrderBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // createMedicineOrderBtn
            // 
            this.createMedicineOrderBtn.Location = new System.Drawing.Point(321, 202);
            this.createMedicineOrderBtn.Name = "createMedicineOrderBtn";
            this.createMedicineOrderBtn.Size = new System.Drawing.Size(150, 27);
            this.createMedicineOrderBtn.TabIndex = 0;
            this.createMedicineOrderBtn.Text = "模拟医生创建付药单";
            this.createMedicineOrderBtn.UseVisualStyleBackColor = true;
            this.createMedicineOrderBtn.Click += new System.EventHandler(this.createMedicineOrderBtn_Click);
            // 
            // changeBalanceStatusBtn
            // 
            this.changeBalanceStatusBtn.Location = new System.Drawing.Point(321, 440);
            this.changeBalanceStatusBtn.Name = "changeBalanceStatusBtn";
            this.changeBalanceStatusBtn.Size = new System.Drawing.Size(150, 41);
            this.changeBalanceStatusBtn.TabIndex = 0;
            this.changeBalanceStatusBtn.Text = "模拟窗口改变结清状态";
            this.changeBalanceStatusBtn.UseVisualStyleBackColor = true;
            this.changeBalanceStatusBtn.Click += new System.EventHandler(this.changeBalanceStatusBtn_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnId,
            this.columnBalance,
            this.columnFulfill});
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(303, 519);
            this.dataGridView1.TabIndex = 1;
            // 
            // columnId
            // 
            this.columnId.HeaderText = "编号";
            this.columnId.Name = "columnId";
            this.columnId.ReadOnly = true;
            // 
            // columnBalance
            // 
            this.columnBalance.HeaderText = "结清";
            this.columnBalance.Name = "columnBalance";
            this.columnBalance.ReadOnly = true;
            // 
            // columnFulfill
            // 
            this.columnFulfill.HeaderText = "交付";
            this.columnFulfill.Name = "columnFulfill";
            this.columnFulfill.ReadOnly = true;
            // 
            // changeFulfillStatusBtn
            // 
            this.changeFulfillStatusBtn.Location = new System.Drawing.Point(321, 490);
            this.changeFulfillStatusBtn.Name = "changeFulfillStatusBtn";
            this.changeFulfillStatusBtn.Size = new System.Drawing.Size(150, 41);
            this.changeFulfillStatusBtn.TabIndex = 0;
            this.changeFulfillStatusBtn.Text = "模拟付药机改变交付状态";
            this.changeFulfillStatusBtn.UseVisualStyleBackColor = true;
            this.changeFulfillStatusBtn.Click += new System.EventHandler(this.changeFulfillStatusBtn_Click);
            // 
            // printMedicineOrderBtn
            // 
            this.printMedicineOrderBtn.Location = new System.Drawing.Point(321, 264);
            this.printMedicineOrderBtn.Name = "printMedicineOrderBtn";
            this.printMedicineOrderBtn.Size = new System.Drawing.Size(150, 26);
            this.printMedicineOrderBtn.TabIndex = 0;
            this.printMedicineOrderBtn.Text = "模拟医生打印付药单";
            this.printMedicineOrderBtn.UseVisualStyleBackColor = true;
            this.printMedicineOrderBtn.Click += new System.EventHandler(this.printMedicineOrderBtn_Click);
            // 
            // reloadDataBtn
            // 
            this.reloadDataBtn.Location = new System.Drawing.Point(321, 152);
            this.reloadDataBtn.Name = "reloadDataBtn";
            this.reloadDataBtn.Size = new System.Drawing.Size(150, 44);
            this.reloadDataBtn.TabIndex = 2;
            this.reloadDataBtn.Text = "刷新数据";
            this.reloadDataBtn.UseVisualStyleBackColor = true;
            this.reloadDataBtn.Click += new System.EventHandler(this.reloadDataBtn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(322, 296);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(149, 138);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // sendMsgTextBox
            // 
            this.sendMsgTextBox.AcceptsReturn = true;
            this.sendMsgTextBox.Location = new System.Drawing.Point(477, 202);
            this.sendMsgTextBox.Multiline = true;
            this.sendMsgTextBox.Name = "sendMsgTextBox";
            this.sendMsgTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.sendMsgTextBox.Size = new System.Drawing.Size(442, 94);
            this.sendMsgTextBox.TabIndex = 5;
            this.sendMsgTextBox.Text = "{\"Type\":\"Heart beat,Hello world!\"}";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(478, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "接收信息:";
            // 
            // receiveMsgTextbox
            // 
            this.receiveMsgTextbox.AcceptsReturn = true;
            this.receiveMsgTextbox.Location = new System.Drawing.Point(477, 27);
            this.receiveMsgTextbox.Multiline = true;
            this.receiveMsgTextbox.Name = "receiveMsgTextbox";
            this.receiveMsgTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.receiveMsgTextbox.Size = new System.Drawing.Size(442, 157);
            this.receiveMsgTextbox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(475, 187);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "发送信息:";
            // 
            // sendToAMDMBtn
            // 
            this.sendToAMDMBtn.Location = new System.Drawing.Point(801, 333);
            this.sendToAMDMBtn.Name = "sendToAMDMBtn";
            this.sendToAMDMBtn.Size = new System.Drawing.Size(118, 23);
            this.sendToAMDMBtn.TabIndex = 7;
            this.sendToAMDMBtn.Text = "发送到付药机";
            this.sendToAMDMBtn.UseVisualStyleBackColor = true;
            this.sendToAMDMBtn.Click += new System.EventHandler(this.sendToAMDMBtn_Click);
            // 
            // logTextbox
            // 
            this.logTextbox.AcceptsReturn = true;
            this.logTextbox.Location = new System.Drawing.Point(477, 374);
            this.logTextbox.Multiline = true;
            this.logTextbox.Name = "logTextbox";
            this.logTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextbox.Size = new System.Drawing.Size(442, 113);
            this.logTextbox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(477, 359);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "日志信息:";
            // 
            // sendToHTTPServerBtn
            // 
            this.sendToHTTPServerBtn.Location = new System.Drawing.Point(528, 333);
            this.sendToHTTPServerBtn.Name = "sendToHTTPServerBtn";
            this.sendToHTTPServerBtn.Size = new System.Drawing.Size(254, 23);
            this.sendToHTTPServerBtn.TabIndex = 7;
            this.sendToHTTPServerBtn.Text = "发送到HTTP服务器(本程序内测试)";
            this.sendToHTTPServerBtn.UseVisualStyleBackColor = true;
            this.sendToHTTPServerBtn.Click += new System.EventHandler(this.sendToHTTPServerBtn_Click);
            // 
            // requestCombox
            // 
            this.requestCombox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.requestCombox.FormattingEnabled = true;
            this.requestCombox.Items.AddRange(new object[] {
            "√付药机->HIS 根据扫码信息获取给药单详细信息",
            "√HIS->付药机 获取付药机当前的库存信息",
            "×付药机->HIS 报送付药单已经完成付药",
            "×付药机->HIS 从HIS系统获取药品信息",
            "×付药机->HIS 报送付药机当前库存信息",
            "×付药机->HIS 报送付药机某商品已付完(缺药/待补药)[X,Y,NUM,NAME,BARCODE...]",
            "×付药机->HIS 检查HIS系统内的新增或已修改药品信息",
            "√自定义信息(上方输入框内容)"});
            this.requestCombox.Location = new System.Drawing.Point(528, 302);
            this.requestCombox.Name = "requestCombox";
            this.requestCombox.Size = new System.Drawing.Size(389, 20);
            this.requestCombox.TabIndex = 8;
            this.requestCombox.SelectedIndexChanged += new System.EventHandler(this.requestCombox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(475, 305);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "请求名:";
            // 
            // AMDMAddressTextbox
            // 
            this.AMDMAddressTextbox.Location = new System.Drawing.Point(681, 510);
            this.AMDMAddressTextbox.Name = "AMDMAddressTextbox";
            this.AMDMAddressTextbox.Size = new System.Drawing.Size(236, 21);
            this.AMDMAddressTextbox.TabIndex = 10;
            this.AMDMAddressTextbox.Text = "http://192.168.2.163/router.aspx";
            this.AMDMAddressTextbox.TextChanged += new System.EventHandler(this.AMDMAddressTextbox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(679, 490);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "付药机地址";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(478, 492);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "付药机地址";
            // 
            // httpServerAddressTextbox
            // 
            this.httpServerAddressTextbox.Location = new System.Drawing.Point(477, 510);
            this.httpServerAddressTextbox.Name = "httpServerAddressTextbox";
            this.httpServerAddressTextbox.Size = new System.Drawing.Size(189, 21);
            this.httpServerAddressTextbox.TabIndex = 10;
            this.httpServerAddressTextbox.Text = "http://192.168.2.122:9000/";
            this.httpServerAddressTextbox.TextChanged += new System.EventHandler(this.httpServerAddressTextbox_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(478, 492);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "本机HTTP服务器地址";
            // 
            // reStartHpptServerBtn
            // 
            this.reStartHpptServerBtn.Location = new System.Drawing.Point(477, 537);
            this.reStartHpptServerBtn.Name = "reStartHpptServerBtn";
            this.reStartHpptServerBtn.Size = new System.Drawing.Size(440, 39);
            this.reStartHpptServerBtn.TabIndex = 12;
            this.reStartHpptServerBtn.Text = "启动服务";
            this.reStartHpptServerBtn.UseVisualStyleBackColor = true;
            this.reStartHpptServerBtn.TextChanged += new System.EventHandler(this.button1_TextChanged);
            this.reStartHpptServerBtn.Click += new System.EventHandler(this.reStartHpptServerBtn_Click);
            // 
            // printerListCombox
            // 
            this.printerListCombox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.printerListCombox.FormattingEnabled = true;
            this.printerListCombox.Location = new System.Drawing.Point(322, 238);
            this.printerListCombox.MaxDropDownItems = 20;
            this.printerListCombox.Name = "printerListCombox";
            this.printerListCombox.Size = new System.Drawing.Size(149, 20);
            this.printerListCombox.TabIndex = 13;
            this.printerListCombox.SelectedIndexChanged += new System.EventHandler(this.printerListCombox_SelectedIndexChanged);
            // 
            // sqlServerAddressTextbox
            // 
            this.sqlServerAddressTextbox.Location = new System.Drawing.Point(322, 27);
            this.sqlServerAddressTextbox.Name = "sqlServerAddressTextbox";
            this.sqlServerAddressTextbox.Size = new System.Drawing.Size(149, 21);
            this.sqlServerAddressTextbox.TabIndex = 14;
            this.sqlServerAddressTextbox.Text = "127.0.0.1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(321, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 12);
            this.label8.TabIndex = 6;
            this.label8.Text = "MYSQL服务器IP";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(321, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 6;
            this.label9.Text = "端口";
            // 
            // sqlServerPortTextbox
            // 
            this.sqlServerPortTextbox.Location = new System.Drawing.Point(347, 51);
            this.sqlServerPortTextbox.Name = "sqlServerPortTextbox";
            this.sqlServerPortTextbox.Size = new System.Drawing.Size(40, 21);
            this.sqlServerPortTextbox.TabIndex = 14;
            this.sqlServerPortTextbox.Text = "9999";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(321, 96);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 6;
            this.label10.Text = "库名";
            // 
            // sqlServerDatabaseTextbox
            // 
            this.sqlServerDatabaseTextbox.Location = new System.Drawing.Point(347, 93);
            this.sqlServerDatabaseTextbox.Name = "sqlServerDatabaseTextbox";
            this.sqlServerDatabaseTextbox.Size = new System.Drawing.Size(124, 21);
            this.sqlServerDatabaseTextbox.TabIndex = 14;
            this.sqlServerDatabaseTextbox.Text = "his_server";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(392, 54);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 12);
            this.label11.TabIndex = 6;
            this.label11.Text = "用户";
            // 
            // sqlServerUserTextbox
            // 
            this.sqlServerUserTextbox.Location = new System.Drawing.Point(419, 51);
            this.sqlServerUserTextbox.Name = "sqlServerUserTextbox";
            this.sqlServerUserTextbox.Size = new System.Drawing.Size(52, 21);
            this.sqlServerUserTextbox.TabIndex = 14;
            this.sqlServerUserTextbox.Text = "root";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(321, 75);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(29, 12);
            this.label12.TabIndex = 6;
            this.label12.Text = "密码";
            // 
            // sqlServerPassTextbox
            // 
            this.sqlServerPassTextbox.Location = new System.Drawing.Point(348, 72);
            this.sqlServerPassTextbox.Name = "sqlServerPassTextbox";
            this.sqlServerPassTextbox.PasswordChar = '*';
            this.sqlServerPassTextbox.Size = new System.Drawing.Size(123, 21);
            this.sqlServerPassTextbox.TabIndex = 14;
            this.sqlServerPassTextbox.Text = "woshinidie";
            // 
            // updateSQLserverSettingAndTryConnectBtn
            // 
            this.updateSQLserverSettingAndTryConnectBtn.Location = new System.Drawing.Point(321, 117);
            this.updateSQLserverSettingAndTryConnectBtn.Name = "updateSQLserverSettingAndTryConnectBtn";
            this.updateSQLserverSettingAndTryConnectBtn.Size = new System.Drawing.Size(150, 23);
            this.updateSQLserverSettingAndTryConnectBtn.TabIndex = 15;
            this.updateSQLserverSettingAndTryConnectBtn.Text = "更新设置并尝试连接";
            this.updateSQLserverSettingAndTryConnectBtn.UseVisualStyleBackColor = true;
            this.updateSQLserverSettingAndTryConnectBtn.Click += new System.EventHandler(this.updateSQLserverSettingAndTryConnectBtn_Click);
            // 
            // createAllCurrentInventoryMedicineOrderBtn
            // 
            this.createAllCurrentInventoryMedicineOrderBtn.Location = new System.Drawing.Point(12, 537);
            this.createAllCurrentInventoryMedicineOrderBtn.Name = "createAllCurrentInventoryMedicineOrderBtn";
            this.createAllCurrentInventoryMedicineOrderBtn.Size = new System.Drawing.Size(459, 39);
            this.createAllCurrentInventoryMedicineOrderBtn.TabIndex = 16;
            this.createAllCurrentInventoryMedicineOrderBtn.Text = "创建取空整个药仓的付药单";
            this.createAllCurrentInventoryMedicineOrderBtn.UseVisualStyleBackColor = true;
            this.createAllCurrentInventoryMedicineOrderBtn.Click += new System.EventHandler(this.createAllCurrentInventoryMedicineOrderBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 579);
            this.Controls.Add(this.createAllCurrentInventoryMedicineOrderBtn);
            this.Controls.Add(this.updateSQLserverSettingAndTryConnectBtn);
            this.Controls.Add(this.sqlServerPassTextbox);
            this.Controls.Add(this.sqlServerDatabaseTextbox);
            this.Controls.Add(this.sqlServerUserTextbox);
            this.Controls.Add(this.sqlServerPortTextbox);
            this.Controls.Add(this.sqlServerAddressTextbox);
            this.Controls.Add(this.printerListCombox);
            this.Controls.Add(this.reStartHpptServerBtn);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.httpServerAddressTextbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.AMDMAddressTextbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.requestCombox);
            this.Controls.Add(this.sendToHTTPServerBtn);
            this.Controls.Add(this.sendToAMDMBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.logTextbox);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.receiveMsgTextbox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sendMsgTextBox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.reloadDataBtn);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.printMedicineOrderBtn);
            this.Controls.Add(this.changeFulfillStatusBtn);
            this.Controls.Add(this.changeBalanceStatusBtn);
            this.Controls.Add(this.createMedicineOrderBtn);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "FakeHISServer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button createMedicineOrderBtn;
        private System.Windows.Forms.Button changeBalanceStatusBtn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button changeFulfillStatusBtn;
        private System.Windows.Forms.Button printMedicineOrderBtn;
        private System.Windows.Forms.Button reloadDataBtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnId;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnBalance;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnFulfill;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox sendMsgTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox receiveMsgTextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button sendToAMDMBtn;
        private System.Windows.Forms.TextBox logTextbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button sendToHTTPServerBtn;
        private System.Windows.Forms.ComboBox requestCombox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox AMDMAddressTextbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox httpServerAddressTextbox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button reStartHpptServerBtn;
        private System.Windows.Forms.ComboBox printerListCombox;
        private System.Windows.Forms.TextBox sqlServerAddressTextbox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox sqlServerPortTextbox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox sqlServerDatabaseTextbox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox sqlServerUserTextbox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox sqlServerPassTextbox;
        private System.Windows.Forms.Button updateSQLserverSettingAndTryConnectBtn;
        private System.Windows.Forms.Button createAllCurrentInventoryMedicineOrderBtn;
    }
}

