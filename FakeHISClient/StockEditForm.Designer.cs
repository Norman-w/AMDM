namespace FakeHISClient
{
    partial class StockEditForm
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
            this.components = new System.ComponentModel.Container();
            this.initByAverageBtn = new System.Windows.Forms.Button();
            this.initByTemplateFileBtn = new System.Windows.Forms.Button();
            this.addFloorBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.showGridsByPercentCheckbox = new System.Windows.Forms.CheckBox();
            this.gridEditMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toLeftMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toRightMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.removeLeftMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeRightMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFloorBySelectedWidthBtn = new System.Windows.Forms.Button();
            this.clearStockBtn = new System.Windows.Forms.Button();
            this.save2TemplateBtn = new System.Windows.Forms.Button();
            this.setDefaultGridWidMMBtn = new System.Windows.Forms.Button();
            this.moveToSelectedGridBtn = new System.Windows.Forms.Button();
            this.setDefaultFloorHeightMMBtn = new System.Windows.Forms.Button();
            this.setStockBaseInfoBtn = new System.Windows.Forms.Button();
            this.keepMove2RandomGridBtn = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.randomMedicineGettingModeCombox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.closeBtn = new System.Windows.Forms.Button();
            this.maxSizeBtn = new System.Windows.Forms.Button();
            this.minSizeBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.addFloorAtDownPart = new System.Windows.Forms.Button();
            this.testMedicineGettingBySelectedSpicalGridBtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.updateAndSaveGridIndexOfStockBtn = new System.Windows.Forms.Button();
            this.打开所有电磁锁按钮 = new System.Windows.Forms.Button();
            this.关闭所有电磁锁按钮 = new System.Windows.Forms.Button();
            this.打开所有空调按钮 = new System.Windows.Forms.Button();
            this.关闭所有空调按钮 = new System.Windows.Forms.Button();
            this.打开所有紫外线按钮 = new System.Windows.Forms.Button();
            this.关闭所有紫外线按钮 = new System.Windows.Forms.Button();
            this.读取药仓1温度 = new System.Windows.Forms.Button();
            this.设定药仓1空调温度为25按钮 = new System.Windows.Forms.Button();
            this.发送完成取药休息信号按钮 = new System.Windows.Forms.Button();
            this.机械手强制复位按钮 = new System.Windows.Forms.Button();
            this.gridEditMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // initByAverageBtn
            // 
            this.initByAverageBtn.Location = new System.Drawing.Point(278, 83);
            this.initByAverageBtn.Name = "initByAverageBtn";
            this.initByAverageBtn.Size = new System.Drawing.Size(133, 23);
            this.initByAverageBtn.TabIndex = 0;
            this.initByAverageBtn.Text = "使用平均值初始化药仓";
            this.initByAverageBtn.UseVisualStyleBackColor = true;
            this.initByAverageBtn.Click += new System.EventHandler(this.initByAverageBtn_Click);
            // 
            // initByTemplateFileBtn
            // 
            this.initByTemplateFileBtn.Location = new System.Drawing.Point(417, 83);
            this.initByTemplateFileBtn.Name = "initByTemplateFileBtn";
            this.initByTemplateFileBtn.Size = new System.Drawing.Size(161, 23);
            this.initByTemplateFileBtn.TabIndex = 0;
            this.initByTemplateFileBtn.Text = "加载模板文件初始化药仓";
            this.initByTemplateFileBtn.UseVisualStyleBackColor = true;
            this.initByTemplateFileBtn.Click += new System.EventHandler(this.initByTemplateFileBtn_Click);
            // 
            // addFloorBtn
            // 
            this.addFloorBtn.Location = new System.Drawing.Point(840, 112);
            this.addFloorBtn.Name = "addFloorBtn";
            this.addFloorBtn.Size = new System.Drawing.Size(438, 23);
            this.addFloorBtn.TabIndex = 1;
            this.addFloorBtn.Text = "添加层(包含一个默认药槽)";
            this.addFloorBtn.UseVisualStyleBackColor = true;
            this.addFloorBtn.Click += new System.EventHandler(this.addFloorBtn_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(59, 141);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1163, 564);
            this.panel1.TabIndex = 2;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // showGridsByPercentCheckbox
            // 
            this.showGridsByPercentCheckbox.AutoSize = true;
            this.showGridsByPercentCheckbox.Checked = true;
            this.showGridsByPercentCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGridsByPercentCheckbox.Location = new System.Drawing.Point(14, 87);
            this.showGridsByPercentCheckbox.Name = "showGridsByPercentCheckbox";
            this.showGridsByPercentCheckbox.Size = new System.Drawing.Size(108, 16);
            this.showGridsByPercentCheckbox.TabIndex = 3;
            this.showGridsByPercentCheckbox.Text = "按实际比例显示";
            this.showGridsByPercentCheckbox.UseVisualStyleBackColor = true;
            // 
            // gridEditMenu
            // 
            this.gridEditMenu.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gridEditMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toLeftMenuItem,
            this.toRightMenuItem,
            this.toolStripSeparator1,
            this.removeLeftMenuItem,
            this.removeRightMenuItem});
            this.gridEditMenu.Name = "gridEditMenu";
            this.gridEditMenu.Size = new System.Drawing.Size(252, 162);
            this.gridEditMenu.Text = "编辑药槽";
            this.gridEditMenu.Opening += new System.ComponentModel.CancelEventHandler(this.gridEditMenu_Opening);
            // 
            // toLeftMenuItem
            // 
            this.toLeftMenuItem.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.toLeftMenuItem.Name = "toLeftMenuItem";
            this.toLeftMenuItem.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.toLeftMenuItem.Size = new System.Drawing.Size(251, 30);
            this.toLeftMenuItem.Text = "<<--向左移动左挡板";
            this.toLeftMenuItem.Click += new System.EventHandler(this.toLeftMenuItem_Click);
            // 
            // toRightMenuItem
            // 
            this.toRightMenuItem.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.toRightMenuItem.Name = "toRightMenuItem";
            this.toRightMenuItem.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.toRightMenuItem.Size = new System.Drawing.Size(251, 30);
            this.toRightMenuItem.Text = "向右移动右挡板-->>";
            this.toRightMenuItem.Click += new System.EventHandler(this.toRightMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(248, 6);
            // 
            // removeLeftMenuItem
            // 
            this.removeLeftMenuItem.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.removeLeftMenuItem.Name = "removeLeftMenuItem";
            this.removeLeftMenuItem.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.removeLeftMenuItem.Size = new System.Drawing.Size(251, 30);
            this.removeLeftMenuItem.Text = "×←移除左侧挡板";
            this.removeLeftMenuItem.Click += new System.EventHandler(this.removeLeftMenuItem_Click);
            // 
            // removeRightMenuItem
            // 
            this.removeRightMenuItem.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.removeRightMenuItem.Name = "removeRightMenuItem";
            this.removeRightMenuItem.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.removeRightMenuItem.Size = new System.Drawing.Size(251, 30);
            this.removeRightMenuItem.Text = "移除右侧挡板→×";
            this.removeRightMenuItem.Click += new System.EventHandler(this.removeRightMenuItem_Click);
            // 
            // addFloorBySelectedWidthBtn
            // 
            this.addFloorBySelectedWidthBtn.Location = new System.Drawing.Point(14, 112);
            this.addFloorBySelectedWidthBtn.Name = "addFloorBySelectedWidthBtn";
            this.addFloorBySelectedWidthBtn.Size = new System.Drawing.Size(473, 23);
            this.addFloorBySelectedWidthBtn.TabIndex = 1;
            this.addFloorBySelectedWidthBtn.Text = "添加层(使用指定宽度的药槽)";
            this.addFloorBySelectedWidthBtn.UseVisualStyleBackColor = true;
            this.addFloorBySelectedWidthBtn.Click += new System.EventHandler(this.addFloorBySelectedWidthBtn_Click);
            // 
            // clearStockBtn
            // 
            this.clearStockBtn.Location = new System.Drawing.Point(128, 83);
            this.clearStockBtn.Name = "clearStockBtn";
            this.clearStockBtn.Size = new System.Drawing.Size(144, 23);
            this.clearStockBtn.TabIndex = 5;
            this.clearStockBtn.Text = "移除该药仓内的层与格";
            this.clearStockBtn.UseVisualStyleBackColor = true;
            this.clearStockBtn.Click += new System.EventHandler(this.clearStockBtn_Click);
            // 
            // save2TemplateBtn
            // 
            this.save2TemplateBtn.Location = new System.Drawing.Point(584, 83);
            this.save2TemplateBtn.Name = "save2TemplateBtn";
            this.save2TemplateBtn.Size = new System.Drawing.Size(75, 23);
            this.save2TemplateBtn.TabIndex = 6;
            this.save2TemplateBtn.Text = "保存为模板";
            this.save2TemplateBtn.UseVisualStyleBackColor = true;
            this.save2TemplateBtn.Click += new System.EventHandler(this.save2TemplateBtn_Click);
            // 
            // setDefaultGridWidMMBtn
            // 
            this.setDefaultGridWidMMBtn.Location = new System.Drawing.Point(668, 112);
            this.setDefaultGridWidMMBtn.Name = "setDefaultGridWidMMBtn";
            this.setDefaultGridWidMMBtn.Size = new System.Drawing.Size(166, 23);
            this.setDefaultGridWidMMBtn.TabIndex = 6;
            this.setDefaultGridWidMMBtn.Text = "设置默认药槽内径↔";
            this.setDefaultGridWidMMBtn.UseVisualStyleBackColor = true;
            this.setDefaultGridWidMMBtn.Click += new System.EventHandler(this.setDefaultGridWidMMBtn_Click);
            // 
            // moveToSelectedGridBtn
            // 
            this.moveToSelectedGridBtn.Location = new System.Drawing.Point(769, 36);
            this.moveToSelectedGridBtn.Name = "moveToSelectedGridBtn";
            this.moveToSelectedGridBtn.Size = new System.Drawing.Size(130, 23);
            this.moveToSelectedGridBtn.TabIndex = 4;
            this.moveToSelectedGridBtn.Text = "机械手移动到该药槽";
            this.moveToSelectedGridBtn.UseVisualStyleBackColor = true;
            this.moveToSelectedGridBtn.Click += new System.EventHandler(this.moveToSelectedGridBtn_Click);
            // 
            // setDefaultFloorHeightMMBtn
            // 
            this.setDefaultFloorHeightMMBtn.Location = new System.Drawing.Point(493, 112);
            this.setDefaultFloorHeightMMBtn.Name = "setDefaultFloorHeightMMBtn";
            this.setDefaultFloorHeightMMBtn.Size = new System.Drawing.Size(166, 23);
            this.setDefaultFloorHeightMMBtn.TabIndex = 7;
            this.setDefaultFloorHeightMMBtn.Text = "设置默认层高度↕";
            this.setDefaultFloorHeightMMBtn.UseVisualStyleBackColor = true;
            this.setDefaultFloorHeightMMBtn.Click += new System.EventHandler(this.setDefaultFloorHeightMMBtn_Click);
            // 
            // setStockBaseInfoBtn
            // 
            this.setStockBaseInfoBtn.Location = new System.Drawing.Point(668, 83);
            this.setStockBaseInfoBtn.Name = "setStockBaseInfoBtn";
            this.setStockBaseInfoBtn.Size = new System.Drawing.Size(96, 23);
            this.setStockBaseInfoBtn.TabIndex = 8;
            this.setStockBaseInfoBtn.Text = "设置药仓属性";
            this.setStockBaseInfoBtn.UseVisualStyleBackColor = true;
            this.setStockBaseInfoBtn.Click += new System.EventHandler(this.setStockBaseInfoBtn_Click);
            // 
            // keepMove2RandomGridBtn
            // 
            this.keepMove2RandomGridBtn.Location = new System.Drawing.Point(1180, 36);
            this.keepMove2RandomGridBtn.Name = "keepMove2RandomGridBtn";
            this.keepMove2RandomGridBtn.Size = new System.Drawing.Size(97, 23);
            this.keepMove2RandomGridBtn.TabIndex = 9;
            this.keepMove2RandomGridBtn.Text = "开始测试取药";
            this.keepMove2RandomGridBtn.UseVisualStyleBackColor = true;
            this.keepMove2RandomGridBtn.Click += new System.EventHandler(this.keepMove2RandomGridBtn_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 9);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(41, 12);
            this.statusLabel.TabIndex = 10;
            this.statusLabel.Text = "......";
            // 
            // randomMedicineGettingModeCombox
            // 
            this.randomMedicineGettingModeCombox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.randomMedicineGettingModeCombox.FormattingEnabled = true;
            this.randomMedicineGettingModeCombox.Location = new System.Drawing.Point(1015, 38);
            this.randomMedicineGettingModeCombox.Name = "randomMedicineGettingModeCombox";
            this.randomMedicineGettingModeCombox.Size = new System.Drawing.Size(159, 20);
            this.randomMedicineGettingModeCombox.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(905, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "取药测试模式选择:";
            // 
            // closeBtn
            // 
            this.closeBtn.BackColor = System.Drawing.Color.Bisque;
            this.closeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.closeBtn.ForeColor = System.Drawing.Color.Black;
            this.closeBtn.Location = new System.Drawing.Point(1268, 3);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(18, 18);
            this.closeBtn.TabIndex = 13;
            this.closeBtn.Tag = "";
            this.closeBtn.Text = "×";
            this.closeBtn.UseVisualStyleBackColor = false;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // maxSizeBtn
            // 
            this.maxSizeBtn.BackColor = System.Drawing.Color.Bisque;
            this.maxSizeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.maxSizeBtn.ForeColor = System.Drawing.Color.Black;
            this.maxSizeBtn.Location = new System.Drawing.Point(1244, 3);
            this.maxSizeBtn.Name = "maxSizeBtn";
            this.maxSizeBtn.Size = new System.Drawing.Size(18, 18);
            this.maxSizeBtn.TabIndex = 14;
            this.maxSizeBtn.Tag = "";
            this.maxSizeBtn.Text = "□";
            this.maxSizeBtn.UseVisualStyleBackColor = false;
            this.maxSizeBtn.Click += new System.EventHandler(this.maxSizeBtn_Click);
            // 
            // minSizeBtn
            // 
            this.minSizeBtn.BackColor = System.Drawing.Color.Bisque;
            this.minSizeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.minSizeBtn.ForeColor = System.Drawing.Color.Black;
            this.minSizeBtn.Location = new System.Drawing.Point(1220, 3);
            this.minSizeBtn.Name = "minSizeBtn";
            this.minSizeBtn.Size = new System.Drawing.Size(18, 18);
            this.minSizeBtn.TabIndex = 15;
            this.minSizeBtn.Tag = "";
            this.minSizeBtn.Text = "-";
            this.minSizeBtn.UseVisualStyleBackColor = false;
            this.minSizeBtn.Click += new System.EventHandler(this.minSizeBtn_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Location = new System.Drawing.Point(59, 711);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(820, 189);
            this.panel2.TabIndex = 16;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // addFloorAtDownPart
            // 
            this.addFloorAtDownPart.Location = new System.Drawing.Point(885, 711);
            this.addFloorAtDownPart.Name = "addFloorAtDownPart";
            this.addFloorAtDownPart.Size = new System.Drawing.Size(161, 32);
            this.addFloorAtDownPart.TabIndex = 17;
            this.addFloorAtDownPart.Text = "在特殊药品区加入添加一层";
            this.addFloorAtDownPart.UseVisualStyleBackColor = true;
            this.addFloorAtDownPart.Click += new System.EventHandler(this.addFloorAtDownPart_Click);
            // 
            // testMedicineGettingBySelectedSpicalGridBtn
            // 
            this.testMedicineGettingBySelectedSpicalGridBtn.Location = new System.Drawing.Point(1052, 711);
            this.testMedicineGettingBySelectedSpicalGridBtn.Name = "testMedicineGettingBySelectedSpicalGridBtn";
            this.testMedicineGettingBySelectedSpicalGridBtn.Size = new System.Drawing.Size(225, 32);
            this.testMedicineGettingBySelectedSpicalGridBtn.TabIndex = 18;
            this.testMedicineGettingBySelectedSpicalGridBtn.Text = "在选中的特殊药槽上执行取药测试";
            this.testMedicineGettingBySelectedSpicalGridBtn.UseVisualStyleBackColor = true;
            this.testMedicineGettingBySelectedSpicalGridBtn.Click += new System.EventHandler(this.testMedicineGettingBySelectedSpicalGridBtn_Click);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(1052, 792);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(225, 32);
            this.button1.TabIndex = 18;
            this.button1.Text = "在所有的特殊药槽上执行随机取药测试";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(1052, 830);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(225, 32);
            this.button2.TabIndex = 18;
            this.button2.Text = "在所有的特殊药槽上执行顺序取药测试";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(1052, 868);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(225, 32);
            this.button3.TabIndex = 18;
            this.button3.Text = "停止特殊药槽的取药测试";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(1052, 749);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(225, 32);
            this.button4.TabIndex = 18;
            this.button4.Text = "在选中的特殊药槽上执行连续取药测试";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // updateAndSaveGridIndexOfStockBtn
            // 
            this.updateAndSaveGridIndexOfStockBtn.Location = new System.Drawing.Point(885, 749);
            this.updateAndSaveGridIndexOfStockBtn.Name = "updateAndSaveGridIndexOfStockBtn";
            this.updateAndSaveGridIndexOfStockBtn.Size = new System.Drawing.Size(161, 32);
            this.updateAndSaveGridIndexOfStockBtn.TabIndex = 0;
            this.updateAndSaveGridIndexOfStockBtn.Text = "更新并保存药槽编号";
            this.updateAndSaveGridIndexOfStockBtn.UseVisualStyleBackColor = true;
            this.updateAndSaveGridIndexOfStockBtn.Click += new System.EventHandler(this.updateAndSaveGridIndexOfStockBtn_Click);
            // 
            // 打开所有电磁锁按钮
            // 
            this.打开所有电磁锁按钮.Location = new System.Drawing.Point(201, 3);
            this.打开所有电磁锁按钮.Name = "打开所有电磁锁按钮";
            this.打开所有电磁锁按钮.Size = new System.Drawing.Size(101, 27);
            this.打开所有电磁锁按钮.TabIndex = 19;
            this.打开所有电磁锁按钮.Text = "打开所有电磁锁";
            this.打开所有电磁锁按钮.UseVisualStyleBackColor = true;
            this.打开所有电磁锁按钮.Click += new System.EventHandler(this.打开所有电磁锁按钮_Click);
            // 
            // 关闭所有电磁锁按钮
            // 
            this.关闭所有电磁锁按钮.Location = new System.Drawing.Point(321, 3);
            this.关闭所有电磁锁按钮.Name = "关闭所有电磁锁按钮";
            this.关闭所有电磁锁按钮.Size = new System.Drawing.Size(101, 27);
            this.关闭所有电磁锁按钮.TabIndex = 19;
            this.关闭所有电磁锁按钮.Text = "关闭所有电磁锁";
            this.关闭所有电磁锁按钮.UseVisualStyleBackColor = true;
            this.关闭所有电磁锁按钮.Click += new System.EventHandler(this.关闭所有电磁锁按钮_Click);
            // 
            // 打开所有空调按钮
            // 
            this.打开所有空调按钮.Location = new System.Drawing.Point(441, 3);
            this.打开所有空调按钮.Name = "打开所有空调按钮";
            this.打开所有空调按钮.Size = new System.Drawing.Size(101, 27);
            this.打开所有空调按钮.TabIndex = 20;
            this.打开所有空调按钮.Text = "打开所有空调";
            this.打开所有空调按钮.UseVisualStyleBackColor = true;
            this.打开所有空调按钮.Click += new System.EventHandler(this.打开所有空调按钮_Click);
            // 
            // 关闭所有空调按钮
            // 
            this.关闭所有空调按钮.Location = new System.Drawing.Point(561, 3);
            this.关闭所有空调按钮.Name = "关闭所有空调按钮";
            this.关闭所有空调按钮.Size = new System.Drawing.Size(101, 27);
            this.关闭所有空调按钮.TabIndex = 20;
            this.关闭所有空调按钮.Text = "关闭所有空调";
            this.关闭所有空调按钮.UseVisualStyleBackColor = true;
            this.关闭所有空调按钮.Click += new System.EventHandler(this.关闭所有空调按钮_Click);
            // 
            // 打开所有紫外线按钮
            // 
            this.打开所有紫外线按钮.Location = new System.Drawing.Point(681, 3);
            this.打开所有紫外线按钮.Name = "打开所有紫外线按钮";
            this.打开所有紫外线按钮.Size = new System.Drawing.Size(101, 27);
            this.打开所有紫外线按钮.TabIndex = 20;
            this.打开所有紫外线按钮.Text = "打开所有紫外线";
            this.打开所有紫外线按钮.UseVisualStyleBackColor = true;
            this.打开所有紫外线按钮.Click += new System.EventHandler(this.打开所有紫外线按钮_Click);
            // 
            // 关闭所有紫外线按钮
            // 
            this.关闭所有紫外线按钮.Location = new System.Drawing.Point(801, 3);
            this.关闭所有紫外线按钮.Name = "关闭所有紫外线按钮";
            this.关闭所有紫外线按钮.Size = new System.Drawing.Size(101, 27);
            this.关闭所有紫外线按钮.TabIndex = 20;
            this.关闭所有紫外线按钮.Text = "关闭所有紫外线";
            this.关闭所有紫外线按钮.UseVisualStyleBackColor = true;
            this.关闭所有紫外线按钮.Click += new System.EventHandler(this.关闭所有紫外线按钮_Click);
            // 
            // 读取药仓1温度
            // 
            this.读取药仓1温度.Location = new System.Drawing.Point(921, 3);
            this.读取药仓1温度.Name = "读取药仓1温度";
            this.读取药仓1温度.Size = new System.Drawing.Size(101, 27);
            this.读取药仓1温度.TabIndex = 21;
            this.读取药仓1温度.Text = "读取药仓1温度";
            this.读取药仓1温度.UseVisualStyleBackColor = true;
            this.读取药仓1温度.Click += new System.EventHandler(this.读取药仓1温度_Click);
            // 
            // 设定药仓1空调温度为25按钮
            // 
            this.设定药仓1空调温度为25按钮.Location = new System.Drawing.Point(1028, 3);
            this.设定药仓1空调温度为25按钮.Name = "设定药仓1空调温度为25按钮";
            this.设定药仓1空调温度为25按钮.Size = new System.Drawing.Size(162, 26);
            this.设定药仓1空调温度为25按钮.TabIndex = 22;
            this.设定药仓1空调温度为25按钮.Text = "设定药仓1空调温度为25℃";
            this.设定药仓1空调温度为25按钮.UseVisualStyleBackColor = true;
            this.设定药仓1空调温度为25按钮.Click += new System.EventHandler(this.设定药仓1空调温度为25按钮_Click);
            // 
            // 发送完成取药休息信号按钮
            // 
            this.发送完成取药休息信号按钮.Location = new System.Drawing.Point(45, 3);
            this.发送完成取药休息信号按钮.Name = "发送完成取药休息信号按钮";
            this.发送完成取药休息信号按钮.Size = new System.Drawing.Size(150, 27);
            this.发送完成取药休息信号按钮.TabIndex = 0;
            this.发送完成取药休息信号按钮.Text = "发送完成取药休息信号";
            this.发送完成取药休息信号按钮.UseVisualStyleBackColor = true;
            this.发送完成取药休息信号按钮.Click += new System.EventHandler(this.发送完成取药休息信号按钮_Click);
            // 
            // 机械手强制复位按钮
            // 
            this.机械手强制复位按钮.Location = new System.Drawing.Point(47, 36);
            this.机械手强制复位按钮.Name = "机械手强制复位按钮";
            this.机械手强制复位按钮.Size = new System.Drawing.Size(105, 23);
            this.机械手强制复位按钮.TabIndex = 23;
            this.机械手强制复位按钮.Text = "机械手强制复位";
            this.机械手强制复位按钮.UseVisualStyleBackColor = true;
            this.机械手强制复位按钮.Click += new System.EventHandler(this.机械手强制复位按钮_Click);
            // 
            // StockEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1289, 912);
            this.Controls.Add(this.机械手强制复位按钮);
            this.Controls.Add(this.发送完成取药休息信号按钮);
            this.Controls.Add(this.设定药仓1空调温度为25按钮);
            this.Controls.Add(this.读取药仓1温度);
            this.Controls.Add(this.关闭所有紫外线按钮);
            this.Controls.Add(this.关闭所有空调按钮);
            this.Controls.Add(this.打开所有紫外线按钮);
            this.Controls.Add(this.打开所有空调按钮);
            this.Controls.Add(this.关闭所有电磁锁按钮);
            this.Controls.Add(this.打开所有电磁锁按钮);
            this.Controls.Add(this.updateAndSaveGridIndexOfStockBtn);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.testMedicineGettingBySelectedSpicalGridBtn);
            this.Controls.Add(this.addFloorAtDownPart);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.maxSizeBtn);
            this.Controls.Add(this.minSizeBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.randomMedicineGettingModeCombox);
            this.Controls.Add(this.keepMove2RandomGridBtn);
            this.Controls.Add(this.setStockBaseInfoBtn);
            this.Controls.Add(this.setDefaultFloorHeightMMBtn);
            this.Controls.Add(this.setDefaultGridWidMMBtn);
            this.Controls.Add(this.save2TemplateBtn);
            this.Controls.Add(this.clearStockBtn);
            this.Controls.Add(this.moveToSelectedGridBtn);
            this.Controls.Add(this.showGridsByPercentCheckbox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.addFloorBySelectedWidthBtn);
            this.Controls.Add(this.addFloorBtn);
            this.Controls.Add(this.initByTemplateFileBtn);
            this.Controls.Add(this.initByAverageBtn);
            this.Controls.Add(this.statusLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "StockEditForm";
            this.Text = "药仓属性与布局";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StockEditForm_FormClosing);
            this.Load += new System.EventHandler(this.Stock_Load);
            this.gridEditMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button initByAverageBtn;
        private System.Windows.Forms.Button initByTemplateFileBtn;
        private System.Windows.Forms.Button addFloorBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox showGridsByPercentCheckbox;
        private System.Windows.Forms.ToolStripMenuItem toLeftMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toRightMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeLeftMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeRightMenuItem;
        private System.Windows.Forms.ContextMenuStrip gridEditMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button addFloorBySelectedWidthBtn;
        private System.Windows.Forms.Button clearStockBtn;
        private System.Windows.Forms.Button save2TemplateBtn;
        private System.Windows.Forms.Button setDefaultGridWidMMBtn;
        private System.Windows.Forms.Button moveToSelectedGridBtn;
        private System.Windows.Forms.Button setDefaultFloorHeightMMBtn;
        private System.Windows.Forms.Button setStockBaseInfoBtn;
        private System.Windows.Forms.Button keepMove2RandomGridBtn;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ComboBox randomMedicineGettingModeCombox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Button maxSizeBtn;
        private System.Windows.Forms.Button minSizeBtn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button addFloorAtDownPart;
        private System.Windows.Forms.Button testMedicineGettingBySelectedSpicalGridBtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button updateAndSaveGridIndexOfStockBtn;
        private System.Windows.Forms.Button 打开所有电磁锁按钮;
        private System.Windows.Forms.Button 关闭所有电磁锁按钮;
        private System.Windows.Forms.Button 打开所有空调按钮;
        private System.Windows.Forms.Button 关闭所有空调按钮;
        private System.Windows.Forms.Button 打开所有紫外线按钮;
        private System.Windows.Forms.Button 关闭所有紫外线按钮;
        private System.Windows.Forms.Button 读取药仓1温度;
        private System.Windows.Forms.Button 设定药仓1空调温度为25按钮;
        private System.Windows.Forms.Button 发送完成取药休息信号按钮;
        private System.Windows.Forms.Button 机械手强制复位按钮;
    }
}