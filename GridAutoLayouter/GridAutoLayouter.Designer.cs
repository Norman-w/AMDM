namespace GridAutoLayouter
{
    partial class GridAutoLayouter
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
            this.粗略计算按钮 = new System.Windows.Forms.Button();
            this.药品信息列表 = new System.Windows.Forms.DataGridView();
            this.columnMedicineId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnBarcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnLong = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnWidth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnHeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.随机添加按钮 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.按剩余空间排序按钮 = new System.Windows.Forms.Button();
            this.尝试计算所有的可能性按钮 = new System.Windows.Forms.Button();
            this.按从小到大顺序排列格子选择框 = new System.Windows.Forms.CheckBox();
            this.按首格大小排序层选择框 = new System.Windows.Forms.CheckBox();
            this.自动随机添加药品库商品并批量设置宽度按钮 = new System.Windows.Forms.Button();
            this.打印选中行贴纸按钮 = new System.Windows.Forms.Button();
            this.打印所有行的贴纸按钮 = new System.Windows.Forms.Button();
            this.随机添加药品库药品按钮 = new System.Windows.Forms.Button();
            this.打印首行贴纸按钮 = new System.Windows.Forms.Button();
            this.floorsDGV = new System.Windows.Forms.DataGridView();
            this.columnFloorIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnFloorWidth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnMedicineCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnFloorRemaining = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.开始扫码并设置药盒宽度按钮 = new System.Windows.Forms.Button();
            this.设置按钮 = new System.Windows.Forms.Button();
            this.显示模拟功能按钮 = new System.Windows.Forms.CheckBox();
            this.移除选定药品按钮 = new System.Windows.Forms.Button();
            this.清空药品列表按钮 = new System.Windows.Forms.Button();
            this.保存当前工程按钮 = new System.Windows.Forms.Button();
            this.从文件读取工程按钮 = new System.Windows.Forms.Button();
            this.应用药槽和药品绑定信息按钮 = new System.Windows.Forms.Button();
            this.包含超高和正常的混合层选择框 = new System.Windows.Forms.CheckBox();
            this.修改尺寸按钮 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.printerYScaleRateTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.changePrinterYScaleRateBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.药品信息列表)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.floorsDGV)).BeginInit();
            this.SuspendLayout();
            // 
            // 粗略计算按钮
            // 
            this.粗略计算按钮.Location = new System.Drawing.Point(647, 490);
            this.粗略计算按钮.Name = "粗略计算按钮";
            this.粗略计算按钮.Size = new System.Drawing.Size(144, 52);
            this.粗略计算按钮.TabIndex = 0;
            this.粗略计算按钮.Text = "粗略计算(忽略高度)";
            this.粗略计算按钮.UseVisualStyleBackColor = true;
            this.粗略计算按钮.Click += new System.EventHandler(this.粗略计算按钮_Click);
            // 
            // 药品信息列表
            // 
            this.药品信息列表.AllowUserToAddRows = false;
            this.药品信息列表.AllowUserToDeleteRows = false;
            this.药品信息列表.AllowUserToResizeRows = false;
            this.药品信息列表.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.药品信息列表.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnMedicineId,
            this.columnIndex,
            this.columnBarcode,
            this.columnName,
            this.columnLong,
            this.columnWidth,
            this.columnHeight});
            this.药品信息列表.Location = new System.Drawing.Point(12, 455);
            this.药品信息列表.Name = "药品信息列表";
            this.药品信息列表.ReadOnly = true;
            this.药品信息列表.RowHeadersVisible = false;
            this.药品信息列表.RowTemplate.Height = 23;
            this.药品信息列表.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.药品信息列表.Size = new System.Drawing.Size(624, 263);
            this.药品信息列表.TabIndex = 1;
            // 
            // columnMedicineId
            // 
            this.columnMedicineId.HeaderText = "药品ID";
            this.columnMedicineId.Name = "columnMedicineId";
            this.columnMedicineId.ReadOnly = true;
            this.columnMedicineId.Visible = false;
            // 
            // columnIndex
            // 
            this.columnIndex.HeaderText = "序号";
            this.columnIndex.Name = "columnIndex";
            this.columnIndex.ReadOnly = true;
            // 
            // columnBarcode
            // 
            this.columnBarcode.HeaderText = "条码";
            this.columnBarcode.Name = "columnBarcode";
            this.columnBarcode.ReadOnly = true;
            // 
            // columnName
            // 
            this.columnName.HeaderText = "名称";
            this.columnName.Name = "columnName";
            this.columnName.ReadOnly = true;
            // 
            // columnLong
            // 
            this.columnLong.HeaderText = "长度(进深)";
            this.columnLong.Name = "columnLong";
            this.columnLong.ReadOnly = true;
            // 
            // columnWidth
            // 
            this.columnWidth.HeaderText = "宽度(槽宽)";
            this.columnWidth.Name = "columnWidth";
            this.columnWidth.ReadOnly = true;
            // 
            // columnHeight
            // 
            this.columnHeight.HeaderText = "高度(厚度)";
            this.columnHeight.Name = "columnHeight";
            this.columnHeight.ReadOnly = true;
            // 
            // 随机添加按钮
            // 
            this.随机添加按钮.Enabled = false;
            this.随机添加按钮.Location = new System.Drawing.Point(647, 455);
            this.随机添加按钮.Name = "随机添加按钮";
            this.随机添加按钮.Size = new System.Drawing.Size(147, 29);
            this.随机添加按钮.TabIndex = 2;
            this.随机添加按钮.Text = "随机添加(虚拟药品)";
            this.随机添加按钮.UseVisualStyleBackColor = true;
            this.随机添加按钮.Click += new System.EventHandler(this.随机添加按钮_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(941, 437);
            this.panel1.TabIndex = 3;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // 按剩余空间排序按钮
            // 
            this.按剩余空间排序按钮.Location = new System.Drawing.Point(800, 490);
            this.按剩余空间排序按钮.Name = "按剩余空间排序按钮";
            this.按剩余空间排序按钮.Size = new System.Drawing.Size(153, 54);
            this.按剩余空间排序按钮.TabIndex = 4;
            this.按剩余空间排序按钮.Text = "按剩余空间排序";
            this.按剩余空间排序按钮.UseVisualStyleBackColor = true;
            this.按剩余空间排序按钮.Click += new System.EventHandler(this.按剩余空间排序按钮_Click);
            // 
            // 尝试计算所有的可能性按钮
            // 
            this.尝试计算所有的可能性按钮.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.尝试计算所有的可能性按钮.ForeColor = System.Drawing.Color.Green;
            this.尝试计算所有的可能性按钮.Location = new System.Drawing.Point(647, 594);
            this.尝试计算所有的可能性按钮.Name = "尝试计算所有的可能性按钮";
            this.尝试计算所有的可能性按钮.Size = new System.Drawing.Size(178, 52);
            this.尝试计算所有的可能性按钮.TabIndex = 5;
            this.尝试计算所有的可能性按钮.Text = "(2)计算所有可用方案并使用最优方案排布";
            this.尝试计算所有的可能性按钮.UseVisualStyleBackColor = true;
            this.尝试计算所有的可能性按钮.Click += new System.EventHandler(this.尝试计算所有的可能性按钮_Click);
            // 
            // 按从小到大顺序排列格子选择框
            // 
            this.按从小到大顺序排列格子选择框.AutoSize = true;
            this.按从小到大顺序排列格子选择框.ForeColor = System.Drawing.Color.Teal;
            this.按从小到大顺序排列格子选择框.Location = new System.Drawing.Point(647, 550);
            this.按从小到大顺序排列格子选择框.Name = "按从小到大顺序排列格子选择框";
            this.按从小到大顺序排列格子选择框.Size = new System.Drawing.Size(162, 16);
            this.按从小到大顺序排列格子选择框.TabIndex = 6;
            this.按从小到大顺序排列格子选择框.Text = "按从小到大顺序排列格子(";
            this.按从小到大顺序排列格子选择框.UseVisualStyleBackColor = true;
            this.按从小到大顺序排列格子选择框.CheckedChanged += new System.EventHandler(this.按从小到大顺序排列格子选择框_CheckedChanged);
            // 
            // 按首格大小排序层选择框
            // 
            this.按首格大小排序层选择框.AutoSize = true;
            this.按首格大小排序层选择框.Location = new System.Drawing.Point(647, 572);
            this.按首格大小排序层选择框.Name = "按首格大小排序层选择框";
            this.按首格大小排序层选择框.Size = new System.Drawing.Size(120, 16);
            this.按首格大小排序层选择框.TabIndex = 6;
            this.按首格大小排序层选择框.Text = "按首格大小排序层";
            this.按首格大小排序层选择框.UseVisualStyleBackColor = true;
            // 
            // 自动随机添加药品库商品并批量设置宽度按钮
            // 
            this.自动随机添加药品库商品并批量设置宽度按钮.Enabled = false;
            this.自动随机添加药品库商品并批量设置宽度按钮.Location = new System.Drawing.Point(831, 594);
            this.自动随机添加药品库商品并批量设置宽度按钮.Name = "自动随机添加药品库商品并批量设置宽度按钮";
            this.自动随机添加药品库商品并批量设置宽度按钮.Size = new System.Drawing.Size(122, 54);
            this.自动随机添加药品库商品并批量设置宽度按钮.TabIndex = 7;
            this.自动随机添加药品库商品并批量设置宽度按钮.Text = "自动随机添加药品库药品并批量设置宽度";
            this.自动随机添加药品库商品并批量设置宽度按钮.UseVisualStyleBackColor = true;
            this.自动随机添加药品库商品并批量设置宽度按钮.Click += new System.EventHandler(this.批量设置宽度按钮_Click);
            // 
            // 打印选中行贴纸按钮
            // 
            this.打印选中行贴纸按钮.ForeColor = System.Drawing.Color.Teal;
            this.打印选中行贴纸按钮.Location = new System.Drawing.Point(1043, 490);
            this.打印选中行贴纸按钮.Name = "打印选中行贴纸按钮";
            this.打印选中行贴纸按钮.Size = new System.Drawing.Size(98, 29);
            this.打印选中行贴纸按钮.TabIndex = 8;
            this.打印选中行贴纸按钮.Text = "打印选中行贴纸";
            this.打印选中行贴纸按钮.UseVisualStyleBackColor = true;
            this.打印选中行贴纸按钮.Click += new System.EventHandler(this.打印选中行贴纸按钮_Click);
            // 
            // 打印所有行的贴纸按钮
            // 
            this.打印所有行的贴纸按钮.ForeColor = System.Drawing.Color.Teal;
            this.打印所有行的贴纸按钮.Location = new System.Drawing.Point(1147, 490);
            this.打印所有行的贴纸按钮.Name = "打印所有行的贴纸按钮";
            this.打印所有行的贴纸按钮.Size = new System.Drawing.Size(138, 29);
            this.打印所有行的贴纸按钮.TabIndex = 9;
            this.打印所有行的贴纸按钮.Text = "(3)打印所有行的贴纸";
            this.打印所有行的贴纸按钮.UseVisualStyleBackColor = true;
            this.打印所有行的贴纸按钮.Click += new System.EventHandler(this.打印所有行的贴纸按钮_Click);
            // 
            // 随机添加药品库药品按钮
            // 
            this.随机添加药品库药品按钮.Enabled = false;
            this.随机添加药品库药品按钮.Location = new System.Drawing.Point(800, 455);
            this.随机添加药品库药品按钮.Name = "随机添加药品库药品按钮";
            this.随机添加药品库药品按钮.Size = new System.Drawing.Size(153, 29);
            this.随机添加药品库药品按钮.TabIndex = 10;
            this.随机添加药品库药品按钮.Text = "随机添加(药品库药品)";
            this.随机添加药品库药品按钮.UseVisualStyleBackColor = true;
            this.随机添加药品库药品按钮.Click += new System.EventHandler(this.随机添加药品库药品按钮_Click);
            // 
            // 打印首行贴纸按钮
            // 
            this.打印首行贴纸按钮.Enabled = false;
            this.打印首行贴纸按钮.Location = new System.Drawing.Point(959, 490);
            this.打印首行贴纸按钮.Name = "打印首行贴纸按钮";
            this.打印首行贴纸按钮.Size = new System.Drawing.Size(78, 29);
            this.打印首行贴纸按钮.TabIndex = 11;
            this.打印首行贴纸按钮.Text = "打印首行";
            this.打印首行贴纸按钮.UseVisualStyleBackColor = true;
            this.打印首行贴纸按钮.Click += new System.EventHandler(this.打印第一行贴纸按钮_Click);
            // 
            // floorsDGV
            // 
            this.floorsDGV.AllowUserToAddRows = false;
            this.floorsDGV.AllowUserToDeleteRows = false;
            this.floorsDGV.AllowUserToResizeRows = false;
            this.floorsDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.floorsDGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnFloorIndex,
            this.columnFloorWidth,
            this.columnMedicineCount,
            this.columnFloorRemaining});
            this.floorsDGV.Location = new System.Drawing.Point(959, 12);
            this.floorsDGV.Name = "floorsDGV";
            this.floorsDGV.ReadOnly = true;
            this.floorsDGV.RowHeadersVisible = false;
            this.floorsDGV.RowTemplate.Height = 23;
            this.floorsDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.floorsDGV.Size = new System.Drawing.Size(326, 437);
            this.floorsDGV.TabIndex = 1;
            this.floorsDGV.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.floorsDGV_CellContentClick);
            // 
            // columnFloorIndex
            // 
            this.columnFloorIndex.FillWeight = 70F;
            this.columnFloorIndex.HeaderText = "层号";
            this.columnFloorIndex.Name = "columnFloorIndex";
            this.columnFloorIndex.ReadOnly = true;
            this.columnFloorIndex.Width = 70;
            // 
            // columnFloorWidth
            // 
            this.columnFloorWidth.FillWeight = 80F;
            this.columnFloorWidth.HeaderText = "总宽";
            this.columnFloorWidth.Name = "columnFloorWidth";
            this.columnFloorWidth.ReadOnly = true;
            this.columnFloorWidth.Width = 80;
            // 
            // columnMedicineCount
            // 
            this.columnMedicineCount.FillWeight = 70F;
            this.columnMedicineCount.HeaderText = "槽数";
            this.columnMedicineCount.Name = "columnMedicineCount";
            this.columnMedicineCount.ReadOnly = true;
            this.columnMedicineCount.Width = 70;
            // 
            // columnFloorRemaining
            // 
            this.columnFloorRemaining.FillWeight = 80F;
            this.columnFloorRemaining.HeaderText = "剩余空间";
            this.columnFloorRemaining.Name = "columnFloorRemaining";
            this.columnFloorRemaining.ReadOnly = true;
            this.columnFloorRemaining.Width = 80;
            // 
            // 开始扫码并设置药盒宽度按钮
            // 
            this.开始扫码并设置药盒宽度按钮.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.开始扫码并设置药盒宽度按钮.ForeColor = System.Drawing.Color.Green;
            this.开始扫码并设置药盒宽度按钮.Location = new System.Drawing.Point(959, 594);
            this.开始扫码并设置药盒宽度按钮.Name = "开始扫码并设置药盒宽度按钮";
            this.开始扫码并设置药盒宽度按钮.Size = new System.Drawing.Size(326, 91);
            this.开始扫码并设置药盒宽度按钮.TabIndex = 12;
            this.开始扫码并设置药盒宽度按钮.Text = "(1)开始扫描药品";
            this.开始扫码并设置药盒宽度按钮.UseVisualStyleBackColor = true;
            this.开始扫码并设置药盒宽度按钮.Click += new System.EventHandler(this.开始扫码并设置药盒宽度按钮_Click);
            // 
            // 设置按钮
            // 
            this.设置按钮.Location = new System.Drawing.Point(959, 559);
            this.设置按钮.Name = "设置按钮";
            this.设置按钮.Size = new System.Drawing.Size(105, 29);
            this.设置按钮.TabIndex = 13;
            this.设置按钮.Text = "设置";
            this.设置按钮.UseVisualStyleBackColor = true;
            this.设置按钮.Click += new System.EventHandler(this.设置按钮_Click);
            // 
            // 显示模拟功能按钮
            // 
            this.显示模拟功能按钮.AutoSize = true;
            this.显示模拟功能按钮.Location = new System.Drawing.Point(1070, 536);
            this.显示模拟功能按钮.Name = "显示模拟功能按钮";
            this.显示模拟功能按钮.Size = new System.Drawing.Size(96, 16);
            this.显示模拟功能按钮.TabIndex = 14;
            this.显示模拟功能按钮.Text = "显示模拟功能";
            this.显示模拟功能按钮.UseVisualStyleBackColor = true;
            this.显示模拟功能按钮.CheckedChanged += new System.EventHandler(this.显示模拟功能按钮_CheckedChanged);
            // 
            // 移除选定药品按钮
            // 
            this.移除选定药品按钮.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.移除选定药品按钮.Location = new System.Drawing.Point(739, 689);
            this.移除选定药品按钮.Name = "移除选定药品按钮";
            this.移除选定药品按钮.Size = new System.Drawing.Size(86, 29);
            this.移除选定药品按钮.TabIndex = 15;
            this.移除选定药品按钮.Text = "移除选定药品";
            this.移除选定药品按钮.UseVisualStyleBackColor = true;
            this.移除选定药品按钮.Click += new System.EventHandler(this.移除选定药品按钮_Click);
            // 
            // 清空药品列表按钮
            // 
            this.清空药品列表按钮.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.清空药品列表按钮.Location = new System.Drawing.Point(831, 659);
            this.清空药品列表按钮.Name = "清空药品列表按钮";
            this.清空药品列表按钮.Size = new System.Drawing.Size(122, 59);
            this.清空药品列表按钮.TabIndex = 15;
            this.清空药品列表按钮.Text = "清空药品列表";
            this.清空药品列表按钮.UseVisualStyleBackColor = true;
            this.清空药品列表按钮.Click += new System.EventHandler(this.清空药品列表按钮_Click);
            // 
            // 保存当前工程按钮
            // 
            this.保存当前工程按钮.Location = new System.Drawing.Point(1070, 558);
            this.保存当前工程按钮.Name = "保存当前工程按钮";
            this.保存当前工程按钮.Size = new System.Drawing.Size(98, 30);
            this.保存当前工程按钮.TabIndex = 16;
            this.保存当前工程按钮.Text = "保存当前工程";
            this.保存当前工程按钮.UseVisualStyleBackColor = true;
            this.保存当前工程按钮.Click += new System.EventHandler(this.保存当前工程按钮_Click);
            // 
            // 从文件读取工程按钮
            // 
            this.从文件读取工程按钮.Location = new System.Drawing.Point(1174, 558);
            this.从文件读取工程按钮.Name = "从文件读取工程按钮";
            this.从文件读取工程按钮.Size = new System.Drawing.Size(111, 30);
            this.从文件读取工程按钮.TabIndex = 16;
            this.从文件读取工程按钮.Text = "从文件读取工程";
            this.从文件读取工程按钮.UseVisualStyleBackColor = true;
            this.从文件读取工程按钮.Click += new System.EventHandler(this.从文件读取工程按钮_Click);
            // 
            // 应用药槽和药品绑定信息按钮
            // 
            this.应用药槽和药品绑定信息按钮.ForeColor = System.Drawing.Color.Teal;
            this.应用药槽和药品绑定信息按钮.Location = new System.Drawing.Point(959, 689);
            this.应用药槽和药品绑定信息按钮.Name = "应用药槽和药品绑定信息按钮";
            this.应用药槽和药品绑定信息按钮.Size = new System.Drawing.Size(326, 29);
            this.应用药槽和药品绑定信息按钮.TabIndex = 17;
            this.应用药槽和药品绑定信息按钮.Text = "(4)应用药槽和药品绑定信息并挂载药仓";
            this.应用药槽和药品绑定信息按钮.UseVisualStyleBackColor = true;
            this.应用药槽和药品绑定信息按钮.Click += new System.EventHandler(this.应用药槽和药品绑定信息按钮_Click);
            // 
            // 包含超高和正常的混合层选择框
            // 
            this.包含超高和正常的混合层选择框.AutoSize = true;
            this.包含超高和正常的混合层选择框.ForeColor = System.Drawing.Color.Teal;
            this.包含超高和正常的混合层选择框.Location = new System.Drawing.Point(805, 550);
            this.包含超高和正常的混合层选择框.Name = "包含超高和正常的混合层选择框";
            this.包含超高和正常的混合层选择框.Size = new System.Drawing.Size(162, 16);
            this.包含超高和正常的混合层选择框.TabIndex = 18;
            this.包含超高和正常的混合层选择框.Text = "包含超高和正常的混合层)";
            this.包含超高和正常的混合层选择框.UseVisualStyleBackColor = true;
            // 
            // 修改尺寸按钮
            // 
            this.修改尺寸按钮.Location = new System.Drawing.Point(647, 689);
            this.修改尺寸按钮.Name = "修改尺寸按钮";
            this.修改尺寸按钮.Size = new System.Drawing.Size(69, 29);
            this.修改尺寸按钮.TabIndex = 19;
            this.修改尺寸按钮.Text = "修改尺寸";
            this.修改尺寸按钮.UseVisualStyleBackColor = true;
            this.修改尺寸按钮.Click += new System.EventHandler(this.修改尺寸按钮_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(647, 656);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(178, 29);
            this.button2.TabIndex = 19;
            this.button2.Text = "为此药品增加药槽";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // printerYScaleRateTextbox
            // 
            this.printerYScaleRateTextbox.Location = new System.Drawing.Point(1060, 463);
            this.printerYScaleRateTextbox.Name = "printerYScaleRateTextbox";
            this.printerYScaleRateTextbox.ReadOnly = true;
            this.printerYScaleRateTextbox.Size = new System.Drawing.Size(158, 21);
            this.printerYScaleRateTextbox.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(959, 466);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 21;
            this.label1.Text = "打印机缩放系数:";
            // 
            // changePrinterYScaleRateBtn
            // 
            this.changePrinterYScaleRateBtn.Location = new System.Drawing.Point(1224, 461);
            this.changePrinterYScaleRateBtn.Name = "changePrinterYScaleRateBtn";
            this.changePrinterYScaleRateBtn.Size = new System.Drawing.Size(61, 23);
            this.changePrinterYScaleRateBtn.TabIndex = 22;
            this.changePrinterYScaleRateBtn.Text = "设置";
            this.changePrinterYScaleRateBtn.UseVisualStyleBackColor = true;
            this.changePrinterYScaleRateBtn.Click += new System.EventHandler(this.changePrinterYScaleRateBtn_Click);
            // 
            // GridAutoLayouter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1297, 730);
            this.Controls.Add(this.changePrinterYScaleRateBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.printerYScaleRateTextbox);
            this.Controls.Add(this.设置按钮);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.修改尺寸按钮);
            this.Controls.Add(this.包含超高和正常的混合层选择框);
            this.Controls.Add(this.应用药槽和药品绑定信息按钮);
            this.Controls.Add(this.从文件读取工程按钮);
            this.Controls.Add(this.保存当前工程按钮);
            this.Controls.Add(this.清空药品列表按钮);
            this.Controls.Add(this.移除选定药品按钮);
            this.Controls.Add(this.显示模拟功能按钮);
            this.Controls.Add(this.开始扫码并设置药盒宽度按钮);
            this.Controls.Add(this.打印首行贴纸按钮);
            this.Controls.Add(this.随机添加药品库药品按钮);
            this.Controls.Add(this.打印所有行的贴纸按钮);
            this.Controls.Add(this.打印选中行贴纸按钮);
            this.Controls.Add(this.自动随机添加药品库商品并批量设置宽度按钮);
            this.Controls.Add(this.按首格大小排序层选择框);
            this.Controls.Add(this.按从小到大顺序排列格子选择框);
            this.Controls.Add(this.尝试计算所有的可能性按钮);
            this.Controls.Add(this.按剩余空间排序按钮);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.随机添加按钮);
            this.Controls.Add(this.floorsDGV);
            this.Controls.Add(this.药品信息列表);
            this.Controls.Add(this.粗略计算按钮);
            this.Name = "GridAutoLayouter";
            this.Text = "GridAutoLayouter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GridAutoLayouter_FormClosing);
            this.Load += new System.EventHandler(this.GridAutoLayouter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.药品信息列表)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.floorsDGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button 粗略计算按钮;
        private System.Windows.Forms.DataGridView 药品信息列表;
        private System.Windows.Forms.Button 随机添加按钮;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button 按剩余空间排序按钮;
        private System.Windows.Forms.Button 尝试计算所有的可能性按钮;
        private System.Windows.Forms.CheckBox 按从小到大顺序排列格子选择框;
        private System.Windows.Forms.CheckBox 按首格大小排序层选择框;
        private System.Windows.Forms.Button 自动随机添加药品库商品并批量设置宽度按钮;
        private System.Windows.Forms.Button 打印选中行贴纸按钮;
        private System.Windows.Forms.Button 打印所有行的贴纸按钮;
        private System.Windows.Forms.Button 随机添加药品库药品按钮;
        private System.Windows.Forms.Button 打印首行贴纸按钮;
        private System.Windows.Forms.DataGridView floorsDGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnFloorIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnFloorWidth;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnMedicineCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnFloorRemaining;
        private System.Windows.Forms.Button 开始扫码并设置药盒宽度按钮;
        private System.Windows.Forms.Button 设置按钮;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnMedicineId;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnBarcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnLong;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnWidth;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnHeight;
        private System.Windows.Forms.CheckBox 显示模拟功能按钮;
        private System.Windows.Forms.Button 移除选定药品按钮;
        private System.Windows.Forms.Button 清空药品列表按钮;
        private System.Windows.Forms.Button 保存当前工程按钮;
        private System.Windows.Forms.Button 从文件读取工程按钮;
        private System.Windows.Forms.Button 应用药槽和药品绑定信息按钮;
        private System.Windows.Forms.CheckBox 包含超高和正常的混合层选择框;
        private System.Windows.Forms.Button 修改尺寸按钮;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox printerYScaleRateTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button changePrinterYScaleRateBtn;
    }
}