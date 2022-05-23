namespace 自动付药机客户端部署工具
{
    partial class 自动付药机客户端部署工具
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.publicKeyTextbox = new System.Windows.Forms.TextBox();
            this.partsSNDGV = new System.Windows.Forms.DataGridView();
            this.columnPartName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnPartSN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnPartCreateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.addPartsSNBtn = new System.Windows.Forms.Button();
            this.removePartsSNBtn = new System.Windows.Forms.Button();
            this.createMachineSNBtn = new System.Windows.Forms.Button();
            this.createPublickKeyBtn = new System.Windows.Forms.Button();
            this.machineProductionTimeTextbox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.machineSNLabel = new System.Windows.Forms.Label();
            this.installAMDMAPPBtn = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.amdmAPPInstallPathTextbox = new System.Windows.Forms.TextBox();
            this.chooseAMDMAPPInstallPathBtn = new System.Windows.Forms.Button();
            this.openLocalMYSQLBtn = new System.Windows.Forms.Button();
            this.chooseLocalMYSQLInstallPathBtn = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.localMYSQLInstallPathTextbox = new System.Windows.Forms.TextBox();
            this.chooseLogServerConfiggerInstallPathBtn = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.logServerConfiggerInstallPathTextbox = new System.Windows.Forms.TextBox();
            this.openAMDMAPPBtn = new System.Windows.Forms.Button();
            this.installLocalMYSQLBtn = new System.Windows.Forms.Button();
            this.openAMDMAPPFolderBtn = new System.Windows.Forms.Button();
            this.openLocalMYSQLFolderBtn = new System.Windows.Forms.Button();
            this.openLogServerConfiggerBtn = new System.Windows.Forms.Button();
            this.openLogServerConfiggerFolderBtn = new System.Windows.Forms.Button();
            this.installLogServerConfiggerBtn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.stocksDGV = new System.Windows.Forms.DataGridView();
            this.columnStockIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnStockSN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnStockCreateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnStockGridsCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnStockGridsUsingCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createStockByGuideBtn = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.createStockByManualtn = new System.Windows.Forms.Button();
            this.mountStockBtn = new System.Windows.Forms.Button();
            this.unmountStockBtn = new System.Windows.Forms.Button();
            this.deleteStockBtn = new System.Windows.Forms.Button();
            this.saveProject2FileBtn = new System.Windows.Forms.Button();
            this.loadProjectFromFile = new System.Windows.Forms.Button();
            this.selectSnapCaptureSavePathBtn = new System.Windows.Forms.Button();
            this.editStockByGuideBtn = new System.Windows.Forms.Button();
            this.editStockByMunalBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.partsSNDGV)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stocksDGV)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "序列号:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "生产日期:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "软件公钥:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 218);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "部件码:";
            // 
            // publicKeyTextbox
            // 
            this.publicKeyTextbox.Location = new System.Drawing.Point(6, 137);
            this.publicKeyTextbox.Multiline = true;
            this.publicKeyTextbox.Name = "publicKeyTextbox";
            this.publicKeyTextbox.Size = new System.Drawing.Size(253, 63);
            this.publicKeyTextbox.TabIndex = 1;
            // 
            // partsSNDGV
            // 
            this.partsSNDGV.AllowUserToAddRows = false;
            this.partsSNDGV.AllowUserToDeleteRows = false;
            this.partsSNDGV.AllowUserToResizeRows = false;
            this.partsSNDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.partsSNDGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnPartName,
            this.columnPartSN,
            this.columnPartCreateTime});
            this.partsSNDGV.Location = new System.Drawing.Point(8, 233);
            this.partsSNDGV.Name = "partsSNDGV";
            this.partsSNDGV.ReadOnly = true;
            this.partsSNDGV.RowHeadersVisible = false;
            this.partsSNDGV.RowTemplate.Height = 23;
            this.partsSNDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.partsSNDGV.Size = new System.Drawing.Size(251, 166);
            this.partsSNDGV.TabIndex = 2;
            // 
            // columnPartName
            // 
            this.columnPartName.FillWeight = 80F;
            this.columnPartName.HeaderText = "名称";
            this.columnPartName.Name = "columnPartName";
            this.columnPartName.ReadOnly = true;
            this.columnPartName.Width = 80;
            // 
            // columnPartSN
            // 
            this.columnPartSN.FillWeight = 80F;
            this.columnPartSN.HeaderText = "部件码";
            this.columnPartSN.Name = "columnPartSN";
            this.columnPartSN.ReadOnly = true;
            this.columnPartSN.Width = 80;
            // 
            // columnPartCreateTime
            // 
            this.columnPartCreateTime.HeaderText = "生产时间:";
            this.columnPartCreateTime.Name = "columnPartCreateTime";
            this.columnPartCreateTime.ReadOnly = true;
            // 
            // addPartsSNBtn
            // 
            this.addPartsSNBtn.Location = new System.Drawing.Point(6, 405);
            this.addPartsSNBtn.Name = "addPartsSNBtn";
            this.addPartsSNBtn.Size = new System.Drawing.Size(75, 23);
            this.addPartsSNBtn.TabIndex = 3;
            this.addPartsSNBtn.Text = "添加";
            this.addPartsSNBtn.UseVisualStyleBackColor = true;
            // 
            // removePartsSNBtn
            // 
            this.removePartsSNBtn.Location = new System.Drawing.Point(184, 405);
            this.removePartsSNBtn.Name = "removePartsSNBtn";
            this.removePartsSNBtn.Size = new System.Drawing.Size(75, 23);
            this.removePartsSNBtn.TabIndex = 3;
            this.removePartsSNBtn.Text = "移除";
            this.removePartsSNBtn.UseVisualStyleBackColor = true;
            // 
            // createMachineSNBtn
            // 
            this.createMachineSNBtn.Location = new System.Drawing.Point(216, 24);
            this.createMachineSNBtn.Name = "createMachineSNBtn";
            this.createMachineSNBtn.Size = new System.Drawing.Size(43, 23);
            this.createMachineSNBtn.TabIndex = 3;
            this.createMachineSNBtn.Text = "创建";
            this.createMachineSNBtn.UseVisualStyleBackColor = true;
            this.createMachineSNBtn.Click += new System.EventHandler(this.createMachineSNBtn_Click);
            // 
            // createPublickKeyBtn
            // 
            this.createPublickKeyBtn.Location = new System.Drawing.Point(216, 108);
            this.createPublickKeyBtn.Name = "createPublickKeyBtn";
            this.createPublickKeyBtn.Size = new System.Drawing.Size(43, 23);
            this.createPublickKeyBtn.TabIndex = 3;
            this.createPublickKeyBtn.Text = "创建";
            this.createPublickKeyBtn.UseVisualStyleBackColor = true;
            this.createPublickKeyBtn.Click += new System.EventHandler(this.createPublickKeyBtn_Click);
            // 
            // machineProductionTimeTextbox
            // 
            this.machineProductionTimeTextbox.Location = new System.Drawing.Point(71, 48);
            this.machineProductionTimeTextbox.Name = "machineProductionTimeTextbox";
            this.machineProductionTimeTextbox.Size = new System.Drawing.Size(188, 21);
            this.machineProductionTimeTextbox.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.machineSNLabel);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.machineProductionTimeTextbox);
            this.groupBox1.Controls.Add(this.createPublickKeyBtn);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.createMachineSNBtn);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.removePartsSNBtn);
            this.groupBox1.Controls.Add(this.publicKeyTextbox);
            this.groupBox1.Controls.Add(this.addPartsSNBtn);
            this.groupBox1.Controls.Add(this.partsSNDGV);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(267, 439);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "资产信息";
            // 
            // machineSNLabel
            // 
            this.machineSNLabel.AutoSize = true;
            this.machineSNLabel.Location = new System.Drawing.Point(69, 29);
            this.machineSNLabel.Name = "machineSNLabel";
            this.machineSNLabel.Size = new System.Drawing.Size(53, 12);
            this.machineSNLabel.TabIndex = 0;
            this.machineSNLabel.Text = "[......]";
            // 
            // installAMDMAPPBtn
            // 
            this.installAMDMAPPBtn.Location = new System.Drawing.Point(8, 76);
            this.installAMDMAPPBtn.Name = "installAMDMAPPBtn";
            this.installAMDMAPPBtn.Size = new System.Drawing.Size(107, 27);
            this.installAMDMAPPBtn.TabIndex = 6;
            this.installAMDMAPPBtn.Text = "安装";
            this.installAMDMAPPBtn.UseVisualStyleBackColor = true;
            this.installAMDMAPPBtn.Click += new System.EventHandler(this.installAMDMAPPBtn_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "付药机主程序安装目录:";
            // 
            // amdmAPPInstallPathTextbox
            // 
            this.amdmAPPInstallPathTextbox.Location = new System.Drawing.Point(8, 48);
            this.amdmAPPInstallPathTextbox.Name = "amdmAPPInstallPathTextbox";
            this.amdmAPPInstallPathTextbox.Size = new System.Drawing.Size(300, 21);
            this.amdmAPPInstallPathTextbox.TabIndex = 7;
            // 
            // chooseAMDMAPPInstallPathBtn
            // 
            this.chooseAMDMAPPInstallPathBtn.Location = new System.Drawing.Point(314, 49);
            this.chooseAMDMAPPInstallPathBtn.Name = "chooseAMDMAPPInstallPathBtn";
            this.chooseAMDMAPPInstallPathBtn.Size = new System.Drawing.Size(38, 21);
            this.chooseAMDMAPPInstallPathBtn.TabIndex = 6;
            this.chooseAMDMAPPInstallPathBtn.Text = "选择";
            this.chooseAMDMAPPInstallPathBtn.UseVisualStyleBackColor = true;
            this.chooseAMDMAPPInstallPathBtn.Click += new System.EventHandler(this.chooseAMDMAPPInstallPathBtn_Click);
            // 
            // openLocalMYSQLBtn
            // 
            this.openLocalMYSQLBtn.Location = new System.Drawing.Point(132, 164);
            this.openLocalMYSQLBtn.Name = "openLocalMYSQLBtn";
            this.openLocalMYSQLBtn.Size = new System.Drawing.Size(107, 27);
            this.openLocalMYSQLBtn.TabIndex = 6;
            this.openLocalMYSQLBtn.Text = "启动数据库";
            this.openLocalMYSQLBtn.UseVisualStyleBackColor = true;
            // 
            // chooseLocalMYSQLInstallPathBtn
            // 
            this.chooseLocalMYSQLInstallPathBtn.Location = new System.Drawing.Point(314, 138);
            this.chooseLocalMYSQLInstallPathBtn.Name = "chooseLocalMYSQLInstallPathBtn";
            this.chooseLocalMYSQLInstallPathBtn.Size = new System.Drawing.Size(38, 21);
            this.chooseLocalMYSQLInstallPathBtn.TabIndex = 6;
            this.chooseLocalMYSQLInstallPathBtn.Text = "选择";
            this.chooseLocalMYSQLInstallPathBtn.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 119);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "本机数据库安装路径";
            // 
            // localMYSQLInstallPathTextbox
            // 
            this.localMYSQLInstallPathTextbox.Location = new System.Drawing.Point(8, 138);
            this.localMYSQLInstallPathTextbox.Name = "localMYSQLInstallPathTextbox";
            this.localMYSQLInstallPathTextbox.Size = new System.Drawing.Size(300, 21);
            this.localMYSQLInstallPathTextbox.TabIndex = 7;
            // 
            // chooseLogServerConfiggerInstallPathBtn
            // 
            this.chooseLogServerConfiggerInstallPathBtn.Location = new System.Drawing.Point(314, 223);
            this.chooseLogServerConfiggerInstallPathBtn.Name = "chooseLogServerConfiggerInstallPathBtn";
            this.chooseLogServerConfiggerInstallPathBtn.Size = new System.Drawing.Size(38, 21);
            this.chooseLogServerConfiggerInstallPathBtn.TabIndex = 6;
            this.chooseLogServerConfiggerInstallPathBtn.Text = "选择";
            this.chooseLogServerConfiggerInstallPathBtn.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 205);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(125, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "看门狗部署器安装路径";
            // 
            // logServerConfiggerInstallPathTextbox
            // 
            this.logServerConfiggerInstallPathTextbox.Location = new System.Drawing.Point(8, 224);
            this.logServerConfiggerInstallPathTextbox.Name = "logServerConfiggerInstallPathTextbox";
            this.logServerConfiggerInstallPathTextbox.Size = new System.Drawing.Size(300, 21);
            this.logServerConfiggerInstallPathTextbox.TabIndex = 7;
            // 
            // openAMDMAPPBtn
            // 
            this.openAMDMAPPBtn.Location = new System.Drawing.Point(132, 76);
            this.openAMDMAPPBtn.Name = "openAMDMAPPBtn";
            this.openAMDMAPPBtn.Size = new System.Drawing.Size(107, 27);
            this.openAMDMAPPBtn.TabIndex = 6;
            this.openAMDMAPPBtn.Text = "打开应用";
            this.openAMDMAPPBtn.UseVisualStyleBackColor = true;
            // 
            // installLocalMYSQLBtn
            // 
            this.installLocalMYSQLBtn.Location = new System.Drawing.Point(8, 164);
            this.installLocalMYSQLBtn.Name = "installLocalMYSQLBtn";
            this.installLocalMYSQLBtn.Size = new System.Drawing.Size(107, 27);
            this.installLocalMYSQLBtn.TabIndex = 6;
            this.installLocalMYSQLBtn.Text = "安装";
            this.installLocalMYSQLBtn.UseVisualStyleBackColor = true;
            // 
            // openAMDMAPPFolderBtn
            // 
            this.openAMDMAPPFolderBtn.Location = new System.Drawing.Point(245, 76);
            this.openAMDMAPPFolderBtn.Name = "openAMDMAPPFolderBtn";
            this.openAMDMAPPFolderBtn.Size = new System.Drawing.Size(107, 27);
            this.openAMDMAPPFolderBtn.TabIndex = 6;
            this.openAMDMAPPFolderBtn.Text = "打开文件夹";
            this.openAMDMAPPFolderBtn.UseVisualStyleBackColor = true;
            // 
            // openLocalMYSQLFolderBtn
            // 
            this.openLocalMYSQLFolderBtn.Location = new System.Drawing.Point(245, 164);
            this.openLocalMYSQLFolderBtn.Name = "openLocalMYSQLFolderBtn";
            this.openLocalMYSQLFolderBtn.Size = new System.Drawing.Size(107, 27);
            this.openLocalMYSQLFolderBtn.TabIndex = 6;
            this.openLocalMYSQLFolderBtn.Text = "打开文件夹";
            this.openLocalMYSQLFolderBtn.UseVisualStyleBackColor = true;
            // 
            // openLogServerConfiggerBtn
            // 
            this.openLogServerConfiggerBtn.Location = new System.Drawing.Point(132, 251);
            this.openLogServerConfiggerBtn.Name = "openLogServerConfiggerBtn";
            this.openLogServerConfiggerBtn.Size = new System.Drawing.Size(107, 27);
            this.openLogServerConfiggerBtn.TabIndex = 6;
            this.openLogServerConfiggerBtn.Text = "运行部署器";
            this.openLogServerConfiggerBtn.UseVisualStyleBackColor = true;
            // 
            // openLogServerConfiggerFolderBtn
            // 
            this.openLogServerConfiggerFolderBtn.Location = new System.Drawing.Point(245, 251);
            this.openLogServerConfiggerFolderBtn.Name = "openLogServerConfiggerFolderBtn";
            this.openLogServerConfiggerFolderBtn.Size = new System.Drawing.Size(107, 27);
            this.openLogServerConfiggerFolderBtn.TabIndex = 6;
            this.openLogServerConfiggerFolderBtn.Text = "打开文件夹";
            this.openLogServerConfiggerFolderBtn.UseVisualStyleBackColor = true;
            // 
            // installLogServerConfiggerBtn
            // 
            this.installLogServerConfiggerBtn.Location = new System.Drawing.Point(8, 251);
            this.installLogServerConfiggerBtn.Name = "installLogServerConfiggerBtn";
            this.installLogServerConfiggerBtn.Size = new System.Drawing.Size(107, 27);
            this.installLogServerConfiggerBtn.TabIndex = 6;
            this.installLogServerConfiggerBtn.Text = "安装";
            this.installLogServerConfiggerBtn.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.logServerConfiggerInstallPathTextbox);
            this.groupBox2.Controls.Add(this.installAMDMAPPBtn);
            this.groupBox2.Controls.Add(this.localMYSQLInstallPathTextbox);
            this.groupBox2.Controls.Add(this.openAMDMAPPBtn);
            this.groupBox2.Controls.Add(this.amdmAPPInstallPathTextbox);
            this.groupBox2.Controls.Add(this.openAMDMAPPFolderBtn);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.chooseAMDMAPPInstallPathBtn);
            this.groupBox2.Controls.Add(this.chooseLogServerConfiggerInstallPathBtn);
            this.groupBox2.Controls.Add(this.openLocalMYSQLBtn);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.openLocalMYSQLFolderBtn);
            this.groupBox2.Controls.Add(this.chooseLocalMYSQLInstallPathBtn);
            this.groupBox2.Controls.Add(this.openLogServerConfiggerBtn);
            this.groupBox2.Controls.Add(this.installLocalMYSQLBtn);
            this.groupBox2.Controls.Add(this.installLogServerConfiggerBtn);
            this.groupBox2.Controls.Add(this.openLogServerConfiggerFolderBtn);
            this.groupBox2.Location = new System.Drawing.Point(302, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(366, 291);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "安装路径设置";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(691, 40);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 12);
            this.label9.TabIndex = 9;
            this.label9.Text = "截图保存路径:";
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(693, 63);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(300, 21);
            this.textBox7.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(691, 95);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 12);
            this.label10.TabIndex = 9;
            this.label10.Text = "主机局域网地址:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(691, 131);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(107, 12);
            this.label11.TabIndex = 9;
            this.label11.Text = "本药机局域网地址:";
            // 
            // stocksDGV
            // 
            this.stocksDGV.AllowUserToAddRows = false;
            this.stocksDGV.AllowUserToDeleteRows = false;
            this.stocksDGV.AllowUserToResizeRows = false;
            this.stocksDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.stocksDGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnStockIndex,
            this.columnStockSN,
            this.columnStockCreateTime,
            this.columnStockGridsCount,
            this.columnStockGridsUsingCount});
            this.stocksDGV.Location = new System.Drawing.Point(302, 352);
            this.stocksDGV.Name = "stocksDGV";
            this.stocksDGV.ReadOnly = true;
            this.stocksDGV.RowHeadersVisible = false;
            this.stocksDGV.RowTemplate.Height = 23;
            this.stocksDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.stocksDGV.Size = new System.Drawing.Size(366, 166);
            this.stocksDGV.TabIndex = 2;
            // 
            // columnStockIndex
            // 
            this.columnStockIndex.FillWeight = 60F;
            this.columnStockIndex.HeaderText = "序号";
            this.columnStockIndex.Name = "columnStockIndex";
            this.columnStockIndex.ReadOnly = true;
            this.columnStockIndex.Width = 60;
            // 
            // columnStockSN
            // 
            this.columnStockSN.FillWeight = 70F;
            this.columnStockSN.HeaderText = "序列号";
            this.columnStockSN.Name = "columnStockSN";
            this.columnStockSN.ReadOnly = true;
            this.columnStockSN.Width = 70;
            // 
            // columnStockCreateTime
            // 
            this.columnStockCreateTime.FillWeight = 120F;
            this.columnStockCreateTime.HeaderText = "创建时间";
            this.columnStockCreateTime.Name = "columnStockCreateTime";
            this.columnStockCreateTime.ReadOnly = true;
            this.columnStockCreateTime.Width = 120;
            // 
            // columnStockGridsCount
            // 
            this.columnStockGridsCount.FillWeight = 80F;
            this.columnStockGridsCount.HeaderText = "药槽总数";
            this.columnStockGridsCount.Name = "columnStockGridsCount";
            this.columnStockGridsCount.ReadOnly = true;
            this.columnStockGridsCount.Width = 80;
            // 
            // columnStockGridsUsingCount
            // 
            this.columnStockGridsUsingCount.FillWeight = 80F;
            this.columnStockGridsUsingCount.HeaderText = "已用槽数";
            this.columnStockGridsUsingCount.Name = "columnStockGridsUsingCount";
            this.columnStockGridsUsingCount.ReadOnly = true;
            this.columnStockGridsUsingCount.Width = 80;
            // 
            // createStockByGuideBtn
            // 
            this.createStockByGuideBtn.Location = new System.Drawing.Point(300, 524);
            this.createStockByGuideBtn.Name = "createStockByGuideBtn";
            this.createStockByGuideBtn.Size = new System.Drawing.Size(87, 52);
            this.createStockByGuideBtn.TabIndex = 3;
            this.createStockByGuideBtn.Text = "使用向导添加";
            this.createStockByGuideBtn.UseVisualStyleBackColor = true;
            this.createStockByGuideBtn.Click += new System.EventHandler(this.createStockByGuideBtn_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(300, 337);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(35, 12);
            this.label13.TabIndex = 0;
            this.label13.Text = "药仓:";
            // 
            // createStockByManualtn
            // 
            this.createStockByManualtn.Location = new System.Drawing.Point(393, 524);
            this.createStockByManualtn.Name = "createStockByManualtn";
            this.createStockByManualtn.Size = new System.Drawing.Size(70, 52);
            this.createStockByManualtn.TabIndex = 3;
            this.createStockByManualtn.Text = "手动添加";
            this.createStockByManualtn.UseVisualStyleBackColor = true;
            // 
            // mountStockBtn
            // 
            this.mountStockBtn.Location = new System.Drawing.Point(581, 524);
            this.mountStockBtn.Name = "mountStockBtn";
            this.mountStockBtn.Size = new System.Drawing.Size(41, 23);
            this.mountStockBtn.TabIndex = 3;
            this.mountStockBtn.Text = "挂载";
            this.mountStockBtn.UseVisualStyleBackColor = true;
            this.mountStockBtn.Click += new System.EventHandler(this.mountStockBtn_Click);
            // 
            // unmountStockBtn
            // 
            this.unmountStockBtn.Location = new System.Drawing.Point(628, 524);
            this.unmountStockBtn.Name = "unmountStockBtn";
            this.unmountStockBtn.Size = new System.Drawing.Size(40, 23);
            this.unmountStockBtn.TabIndex = 3;
            this.unmountStockBtn.Text = "卸载";
            this.unmountStockBtn.UseVisualStyleBackColor = true;
            // 
            // deleteStockBtn
            // 
            this.deleteStockBtn.Location = new System.Drawing.Point(581, 553);
            this.deleteStockBtn.Name = "deleteStockBtn";
            this.deleteStockBtn.Size = new System.Drawing.Size(87, 23);
            this.deleteStockBtn.TabIndex = 3;
            this.deleteStockBtn.Text = "删除";
            this.deleteStockBtn.UseVisualStyleBackColor = true;
            this.deleteStockBtn.Click += new System.EventHandler(this.deleteStockBtn_Click);
            // 
            // saveProject2FileBtn
            // 
            this.saveProject2FileBtn.Location = new System.Drawing.Point(925, 553);
            this.saveProject2FileBtn.Name = "saveProject2FileBtn";
            this.saveProject2FileBtn.Size = new System.Drawing.Size(103, 23);
            this.saveProject2FileBtn.TabIndex = 10;
            this.saveProject2FileBtn.Text = "保存工程到文件";
            this.saveProject2FileBtn.UseVisualStyleBackColor = true;
            this.saveProject2FileBtn.Click += new System.EventHandler(this.saveProject2FileBtn_Click);
            // 
            // loadProjectFromFile
            // 
            this.loadProjectFromFile.Location = new System.Drawing.Point(816, 553);
            this.loadProjectFromFile.Name = "loadProjectFromFile";
            this.loadProjectFromFile.Size = new System.Drawing.Size(103, 23);
            this.loadProjectFromFile.TabIndex = 10;
            this.loadProjectFromFile.Text = "从文件加载工程";
            this.loadProjectFromFile.UseVisualStyleBackColor = true;
            this.loadProjectFromFile.Click += new System.EventHandler(this.loadProjectFromFile_Click);
            // 
            // selectSnapCaptureSavePathBtn
            // 
            this.selectSnapCaptureSavePathBtn.Location = new System.Drawing.Point(999, 62);
            this.selectSnapCaptureSavePathBtn.Name = "selectSnapCaptureSavePathBtn";
            this.selectSnapCaptureSavePathBtn.Size = new System.Drawing.Size(38, 21);
            this.selectSnapCaptureSavePathBtn.TabIndex = 6;
            this.selectSnapCaptureSavePathBtn.Text = "选择";
            this.selectSnapCaptureSavePathBtn.UseVisualStyleBackColor = true;
            // 
            // editStockByGuideBtn
            // 
            this.editStockByGuideBtn.Location = new System.Drawing.Point(468, 524);
            this.editStockByGuideBtn.Name = "editStockByGuideBtn";
            this.editStockByGuideBtn.Size = new System.Drawing.Size(107, 23);
            this.editStockByGuideBtn.TabIndex = 11;
            this.editStockByGuideBtn.Text = "使用向导编辑";
            this.editStockByGuideBtn.UseVisualStyleBackColor = true;
            this.editStockByGuideBtn.Click += new System.EventHandler(this.editStockByGuideBtn_Click);
            // 
            // editStockByMunalBtn
            // 
            this.editStockByMunalBtn.Location = new System.Drawing.Point(468, 553);
            this.editStockByMunalBtn.Name = "editStockByMunalBtn";
            this.editStockByMunalBtn.Size = new System.Drawing.Size(107, 23);
            this.editStockByMunalBtn.TabIndex = 11;
            this.editStockByMunalBtn.Text = "手动编辑";
            this.editStockByMunalBtn.UseVisualStyleBackColor = true;
            this.editStockByMunalBtn.Click += new System.EventHandler(this.editStockByMunalBtn_Click);
            // 
            // 自动付药机客户端部署工具
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 593);
            this.Controls.Add(this.editStockByMunalBtn);
            this.Controls.Add(this.editStockByGuideBtn);
            this.Controls.Add(this.loadProjectFromFile);
            this.Controls.Add(this.saveProject2FileBtn);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.selectSnapCaptureSavePathBtn);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.stocksDGV);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.createStockByManualtn);
            this.Controls.Add(this.createStockByGuideBtn);
            this.Controls.Add(this.deleteStockBtn);
            this.Controls.Add(this.unmountStockBtn);
            this.Controls.Add(this.mountStockBtn);
            this.Name = "自动付药机客户端部署工具";
            this.Text = "自动付药机客户端部署工具";
            this.Load += new System.EventHandler(this.自动付药机客户端部署工具_Load);
            ((System.ComponentModel.ISupportInitialize)(this.partsSNDGV)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stocksDGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox publicKeyTextbox;
        private System.Windows.Forms.DataGridView partsSNDGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnPartName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnPartSN;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnPartCreateTime;
        private System.Windows.Forms.Button addPartsSNBtn;
        private System.Windows.Forms.Button removePartsSNBtn;
        private System.Windows.Forms.Button createMachineSNBtn;
        private System.Windows.Forms.Button createPublickKeyBtn;
        private System.Windows.Forms.TextBox machineProductionTimeTextbox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button installAMDMAPPBtn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox amdmAPPInstallPathTextbox;
        private System.Windows.Forms.Button chooseAMDMAPPInstallPathBtn;
        private System.Windows.Forms.Button openLocalMYSQLBtn;
        private System.Windows.Forms.Button chooseLocalMYSQLInstallPathBtn;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox localMYSQLInstallPathTextbox;
        private System.Windows.Forms.Button chooseLogServerConfiggerInstallPathBtn;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox logServerConfiggerInstallPathTextbox;
        private System.Windows.Forms.Button openAMDMAPPBtn;
        private System.Windows.Forms.Button installLocalMYSQLBtn;
        private System.Windows.Forms.Button openAMDMAPPFolderBtn;
        private System.Windows.Forms.Button openLocalMYSQLFolderBtn;
        private System.Windows.Forms.Button openLogServerConfiggerBtn;
        private System.Windows.Forms.Button openLogServerConfiggerFolderBtn;
        private System.Windows.Forms.Button installLogServerConfiggerBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridView stocksDGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnStockIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnStockSN;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnStockCreateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnStockGridsCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnStockGridsUsingCount;
        private System.Windows.Forms.Button createStockByGuideBtn;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button createStockByManualtn;
        private System.Windows.Forms.Button mountStockBtn;
        private System.Windows.Forms.Button unmountStockBtn;
        private System.Windows.Forms.Button deleteStockBtn;
        private System.Windows.Forms.Label machineSNLabel;
        private System.Windows.Forms.Button saveProject2FileBtn;
        private System.Windows.Forms.Button loadProjectFromFile;
        private System.Windows.Forms.Button selectSnapCaptureSavePathBtn;
        private System.Windows.Forms.Button editStockByGuideBtn;
        private System.Windows.Forms.Button editStockByMunalBtn;
    }
}

