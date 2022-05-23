namespace FakeHISClient
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
            this.plcIPCombox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.connectPlcBtn = new System.Windows.Forms.Button();
            this.rackCombox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.slotCombox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.readBtn = new System.Windows.Forms.Button();
            this.writeBtn = new System.Windows.Forms.Button();
            this.readBufferTextbox = new System.Windows.Forms.TextBox();
            this.writeBufferTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.readStartTextbox = new System.Windows.Forms.TextBox();
            this.readEndTextbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.writeStartTextbox = new System.Windows.Forms.TextBox();
            this.writeEndTextbox = new System.Windows.Forms.TextBox();
            this.writeTypeCombox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.readTypeCombox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.writeAndReadBtn = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.readDBNumberTexbox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.writeDBNumberTexbox = new System.Windows.Forms.TextBox();
            this.paramChangeLinkCheckbox = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.closePlcBtn = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.readStringBufferTextbox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.readCountTextBox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.readBufferX2Textbox = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.readBufferX2ToIntTextbox = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.writeBufferX2Textbox = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.writeBufferLongTextbox = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.writeBufferPer8IntTextbox = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.viewCameraViewBtn = new System.Windows.Forms.Button();
            this.stopCameraViewBtn = new System.Windows.Forms.Button();
            this.firstStockManageFormBtn = new System.Windows.Forms.Button();
            this.tscbxCameras = new System.Windows.Forms.ComboBox();
            this.captureByCameraBtn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.testSendMedicineGettingCMDBtn = new System.Windows.Forms.Button();
            this.medicineBindingManageBtn = new System.Windows.Forms.Button();
            this.animateControlContainerLabel = new System.Windows.Forms.Panel();
            this.label23 = new System.Windows.Forms.Label();
            this.medicineInventoryMangeBtn = new System.Windows.Forms.Button();
            this.medicineDeliveryBtn = new System.Windows.Forms.Button();
            this.qrCodeScanerComCombobox = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.videoPlayerPanel = new System.Windows.Forms.Panel();
            this.label26 = new System.Windows.Forms.Label();
            this.部署按钮 = new System.Windows.Forms.Button();
            this.simulatePrintDeliveryRecoryPaperBtn = new System.Windows.Forms.Button();
            this.simulateShowMedicineOrderBtn = new System.Windows.Forms.Button();
            this.showAutoCloseFormBtn = new System.Windows.Forms.Button();
            this.showAutoCloseMedicineOrderFormBtn = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.showMedicinesGettingStatusVideoFormBtn = new System.Windows.Forms.Button();
            this.trySpeakTooMuchTTSBtn = new System.Windows.Forms.Button();
            this.secondStockManageFormBtn = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.videoPlayerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // plcIPCombox
            // 
            this.plcIPCombox.FormattingEnabled = true;
            this.plcIPCombox.Location = new System.Drawing.Point(59, 8);
            this.plcIPCombox.Name = "plcIPCombox";
            this.plcIPCombox.Size = new System.Drawing.Size(143, 20);
            this.plcIPCombox.TabIndex = 0;
            this.plcIPCombox.Text = "192.168.2.100";
            this.plcIPCombox.SelectedIndexChanged += new System.EventHandler(this.plcIPCombox_SelectedIndexChanged);
            this.plcIPCombox.TextUpdate += new System.EventHandler(this.plcIPCombox_TextUpdate);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "PLC IP:";
            // 
            // connectPlcBtn
            // 
            this.connectPlcBtn.Location = new System.Drawing.Point(10, 158);
            this.connectPlcBtn.Name = "connectPlcBtn";
            this.connectPlcBtn.Size = new System.Drawing.Size(190, 50);
            this.connectPlcBtn.TabIndex = 2;
            this.connectPlcBtn.Text = "连接PLC";
            this.connectPlcBtn.UseVisualStyleBackColor = true;
            this.connectPlcBtn.Click += new System.EventHandler(this.connectPlcBtn_Click);
            // 
            // rackCombox
            // 
            this.rackCombox.FormattingEnabled = true;
            this.rackCombox.Location = new System.Drawing.Point(57, 99);
            this.rackCombox.Name = "rackCombox";
            this.rackCombox.Size = new System.Drawing.Size(143, 20);
            this.rackCombox.TabIndex = 0;
            this.rackCombox.Text = "0";
            this.rackCombox.SelectedIndexChanged += new System.EventHandler(this.rackCombox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "机架号";
            // 
            // slotCombox
            // 
            this.slotCombox.FormattingEnabled = true;
            this.slotCombox.Location = new System.Drawing.Point(57, 122);
            this.slotCombox.Name = "slotCombox";
            this.slotCombox.Size = new System.Drawing.Size(143, 20);
            this.slotCombox.TabIndex = 0;
            this.slotCombox.Text = "1";
            this.slotCombox.SelectedIndexChanged += new System.EventHandler(this.slotCombox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "插槽号";
            // 
            // readBtn
            // 
            this.readBtn.Location = new System.Drawing.Point(237, 156);
            this.readBtn.Name = "readBtn";
            this.readBtn.Size = new System.Drawing.Size(188, 46);
            this.readBtn.TabIndex = 3;
            this.readBtn.Text = "读取数据";
            this.readBtn.UseVisualStyleBackColor = true;
            this.readBtn.Click += new System.EventHandler(this.readBtn_Click);
            // 
            // writeBtn
            // 
            this.writeBtn.Location = new System.Drawing.Point(468, 481);
            this.writeBtn.Name = "writeBtn";
            this.writeBtn.Size = new System.Drawing.Size(76, 49);
            this.writeBtn.TabIndex = 3;
            this.writeBtn.Text = "写入数据";
            this.writeBtn.UseVisualStyleBackColor = true;
            this.writeBtn.Click += new System.EventHandler(this.writeBtn_Click);
            // 
            // readBufferTextbox
            // 
            this.readBufferTextbox.Location = new System.Drawing.Point(235, 226);
            this.readBufferTextbox.Multiline = true;
            this.readBufferTextbox.Name = "readBufferTextbox";
            this.readBufferTextbox.Size = new System.Drawing.Size(190, 136);
            this.readBufferTextbox.TabIndex = 4;
            // 
            // writeBufferTextbox
            // 
            this.writeBufferTextbox.Location = new System.Drawing.Point(468, 151);
            this.writeBufferTextbox.Multiline = true;
            this.writeBufferTextbox.Name = "writeBufferTextbox";
            this.writeBufferTextbox.Size = new System.Drawing.Size(190, 99);
            this.writeBufferTextbox.TabIndex = 4;
            this.writeBufferTextbox.Text = "d";
            this.writeBufferTextbox.TextChanged += new System.EventHandler(this.writeBufferTextbox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(235, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "读数据起始位置";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(235, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "读数据结束位置";
            // 
            // readStartTextbox
            // 
            this.readStartTextbox.Location = new System.Drawing.Point(347, 36);
            this.readStartTextbox.Name = "readStartTextbox";
            this.readStartTextbox.Size = new System.Drawing.Size(78, 21);
            this.readStartTextbox.TabIndex = 5;
            this.readStartTextbox.Text = "100";
            this.readStartTextbox.TextChanged += new System.EventHandler(this.readStartTextbox_TextChanged);
            // 
            // readEndTextbox
            // 
            this.readEndTextbox.Location = new System.Drawing.Point(347, 61);
            this.readEndTextbox.Name = "readEndTextbox";
            this.readEndTextbox.Size = new System.Drawing.Size(78, 21);
            this.readEndTextbox.TabIndex = 5;
            this.readEndTextbox.Text = "102";
            this.readEndTextbox.TextChanged += new System.EventHandler(this.readEndTextbox_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(468, 44);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "写数据起始位置";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(468, 72);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "写数据结束位置";
            // 
            // writeStartTextbox
            // 
            this.writeStartTextbox.Location = new System.Drawing.Point(580, 40);
            this.writeStartTextbox.Name = "writeStartTextbox";
            this.writeStartTextbox.Size = new System.Drawing.Size(78, 21);
            this.writeStartTextbox.TabIndex = 5;
            this.writeStartTextbox.Text = "100";
            this.writeStartTextbox.TextChanged += new System.EventHandler(this.writeStartTextbox_TextChanged);
            // 
            // writeEndTextbox
            // 
            this.writeEndTextbox.Location = new System.Drawing.Point(580, 68);
            this.writeEndTextbox.Name = "writeEndTextbox";
            this.writeEndTextbox.Size = new System.Drawing.Size(78, 21);
            this.writeEndTextbox.TabIndex = 5;
            this.writeEndTextbox.Text = "102";
            this.writeEndTextbox.TextChanged += new System.EventHandler(this.writeEndTextbox_TextChanged);
            // 
            // writeTypeCombox
            // 
            this.writeTypeCombox.FormattingEnabled = true;
            this.writeTypeCombox.Location = new System.Drawing.Point(537, 95);
            this.writeTypeCombox.Name = "writeTypeCombox";
            this.writeTypeCombox.Size = new System.Drawing.Size(121, 20);
            this.writeTypeCombox.TabIndex = 0;
            this.writeTypeCombox.Text = "Bool";
            this.writeTypeCombox.Visible = false;
            this.writeTypeCombox.SelectedIndexChanged += new System.EventHandler(this.writeTypeCombox_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(468, 99);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "写数据类型";
            this.label8.Visible = false;
            // 
            // readTypeCombox
            // 
            this.readTypeCombox.FormattingEnabled = true;
            this.readTypeCombox.Items.AddRange(new object[] {
            "Bool",
            "Int32",
            "Int64",
            "String"});
            this.readTypeCombox.Location = new System.Drawing.Point(304, 117);
            this.readTypeCombox.Name = "readTypeCombox";
            this.readTypeCombox.Size = new System.Drawing.Size(121, 20);
            this.readTypeCombox.TabIndex = 0;
            this.readTypeCombox.Text = "Int64";
            this.readTypeCombox.Visible = false;
            this.readTypeCombox.SelectedIndexChanged += new System.EventHandler(this.readTypeCombox_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(235, 121);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 1;
            this.label9.Text = "读数据类型";
            this.label9.Visible = false;
            // 
            // writeAndReadBtn
            // 
            this.writeAndReadBtn.Location = new System.Drawing.Point(563, 481);
            this.writeAndReadBtn.Name = "writeAndReadBtn";
            this.writeAndReadBtn.Size = new System.Drawing.Size(95, 49);
            this.writeAndReadBtn.TabIndex = 3;
            this.writeAndReadBtn.Text = "写入后读取数据";
            this.writeAndReadBtn.UseVisualStyleBackColor = true;
            this.writeAndReadBtn.Click += new System.EventHandler(this.writeAndReadBtn_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(235, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 12);
            this.label10.TabIndex = 1;
            this.label10.Text = "读数据DB编号";
            // 
            // readDBNumberTexbox
            // 
            this.readDBNumberTexbox.Location = new System.Drawing.Point(347, 11);
            this.readDBNumberTexbox.Name = "readDBNumberTexbox";
            this.readDBNumberTexbox.Size = new System.Drawing.Size(78, 21);
            this.readDBNumberTexbox.TabIndex = 5;
            this.readDBNumberTexbox.Text = "1";
            this.readDBNumberTexbox.TextChanged += new System.EventHandler(this.readDBNumberTexbox_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(468, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 12);
            this.label11.TabIndex = 1;
            this.label11.Text = "写数据DB编号";
            // 
            // writeDBNumberTexbox
            // 
            this.writeDBNumberTexbox.Location = new System.Drawing.Point(580, 12);
            this.writeDBNumberTexbox.Name = "writeDBNumberTexbox";
            this.writeDBNumberTexbox.Size = new System.Drawing.Size(78, 21);
            this.writeDBNumberTexbox.TabIndex = 5;
            this.writeDBNumberTexbox.Text = "1";
            this.writeDBNumberTexbox.TextChanged += new System.EventHandler(this.writeDBNumberTexbox_TextChanged);
            // 
            // paramChangeLinkCheckbox
            // 
            this.paramChangeLinkCheckbox.AutoSize = true;
            this.paramChangeLinkCheckbox.Checked = true;
            this.paramChangeLinkCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.paramChangeLinkCheckbox.Location = new System.Drawing.Point(440, 49);
            this.paramChangeLinkCheckbox.Name = "paramChangeLinkCheckbox";
            this.paramChangeLinkCheckbox.Size = new System.Drawing.Size(15, 14);
            this.paramChangeLinkCheckbox.TabIndex = 6;
            this.paramChangeLinkCheckbox.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(438, 36);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 12);
            this.label12.TabIndex = 7;
            this.label12.Text = "<>";
            // 
            // closePlcBtn
            // 
            this.closePlcBtn.Location = new System.Drawing.Point(10, 221);
            this.closePlcBtn.Name = "closePlcBtn";
            this.closePlcBtn.Size = new System.Drawing.Size(190, 50);
            this.closePlcBtn.TabIndex = 2;
            this.closePlcBtn.Text = "断开PLC";
            this.closePlcBtn.UseVisualStyleBackColor = true;
            this.closePlcBtn.Click += new System.EventHandler(this.closePlcBtn_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 307);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(190, 220);
            this.textBox1.TabIndex = 4;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 290);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 12);
            this.label13.TabIndex = 1;
            this.label13.Text = "日志信息:";
            // 
            // readStringBufferTextbox
            // 
            this.readStringBufferTextbox.Location = new System.Drawing.Point(237, 380);
            this.readStringBufferTextbox.Multiline = true;
            this.readStringBufferTextbox.Name = "readStringBufferTextbox";
            this.readStringBufferTextbox.Size = new System.Drawing.Size(190, 36);
            this.readStringBufferTextbox.TabIndex = 4;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(235, 365);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(77, 12);
            this.label14.TabIndex = 1;
            this.label14.Text = "换换为String";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(235, 94);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 12);
            this.label15.TabIndex = 1;
            this.label15.Text = "读取数量";
            // 
            // readCountTextBox
            // 
            this.readCountTextBox.Enabled = false;
            this.readCountTextBox.Location = new System.Drawing.Point(347, 90);
            this.readCountTextBox.Name = "readCountTextBox";
            this.readCountTextBox.Size = new System.Drawing.Size(78, 21);
            this.readCountTextBox.TabIndex = 5;
            this.readCountTextBox.Text = "2";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(235, 419);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(71, 12);
            this.label16.TabIndex = 1;
            this.label16.Text = "转换为2进制";
            // 
            // readBufferX2Textbox
            // 
            this.readBufferX2Textbox.Location = new System.Drawing.Point(237, 434);
            this.readBufferX2Textbox.Multiline = true;
            this.readBufferX2Textbox.Name = "readBufferX2Textbox";
            this.readBufferX2Textbox.Size = new System.Drawing.Size(190, 36);
            this.readBufferX2Textbox.TabIndex = 4;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(235, 481);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(113, 12);
            this.label17.TabIndex = 1;
            this.label17.Text = "合并各个位后的long";
            // 
            // readBufferX2ToIntTextbox
            // 
            this.readBufferX2ToIntTextbox.Location = new System.Drawing.Point(237, 496);
            this.readBufferX2ToIntTextbox.Multiline = true;
            this.readBufferX2ToIntTextbox.Name = "readBufferX2ToIntTextbox";
            this.readBufferX2ToIntTextbox.Size = new System.Drawing.Size(190, 36);
            this.readBufferX2ToIntTextbox.TabIndex = 4;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(466, 253);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(53, 12);
            this.label18.TabIndex = 1;
            this.label18.Text = "二进制值";
            // 
            // writeBufferX2Textbox
            // 
            this.writeBufferX2Textbox.Location = new System.Drawing.Point(468, 268);
            this.writeBufferX2Textbox.Multiline = true;
            this.writeBufferX2Textbox.Name = "writeBufferX2Textbox";
            this.writeBufferX2Textbox.Size = new System.Drawing.Size(190, 36);
            this.writeBufferX2Textbox.TabIndex = 4;
            this.writeBufferX2Textbox.TextChanged += new System.EventHandler(this.writeBufferX2Textbox_TextChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(468, 132);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(53, 12);
            this.label19.TabIndex = 1;
            this.label19.Text = "String值";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(466, 307);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(113, 12);
            this.label20.TabIndex = 1;
            this.label20.Text = "多位组合后的long值";
            // 
            // writeBufferLongTextbox
            // 
            this.writeBufferLongTextbox.Location = new System.Drawing.Point(468, 322);
            this.writeBufferLongTextbox.Multiline = true;
            this.writeBufferLongTextbox.Name = "writeBufferLongTextbox";
            this.writeBufferLongTextbox.Size = new System.Drawing.Size(190, 36);
            this.writeBufferLongTextbox.TabIndex = 4;
            this.writeBufferLongTextbox.TextChanged += new System.EventHandler(this.writeBufferLongTextbox_TextChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(466, 361);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(185, 12);
            this.label21.TabIndex = 1;
            this.label21.Text = "分段值(每8位分一个段转换成int)";
            // 
            // writeBufferPer8IntTextbox
            // 
            this.writeBufferPer8IntTextbox.Enabled = false;
            this.writeBufferPer8IntTextbox.Location = new System.Drawing.Point(468, 376);
            this.writeBufferPer8IntTextbox.Multiline = true;
            this.writeBufferPer8IntTextbox.Name = "writeBufferPer8IntTextbox";
            this.writeBufferPer8IntTextbox.Size = new System.Drawing.Size(190, 36);
            this.writeBufferPer8IntTextbox.TabIndex = 4;
            this.writeBufferPer8IntTextbox.TextChanged += new System.EventHandler(this.writeBufferLongTextbox_TextChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(235, 211);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(71, 12);
            this.label22.TabIndex = 1;
            this.label22.Text = "每8位分段值";
            // 
            // viewCameraViewBtn
            // 
            this.viewCameraViewBtn.Location = new System.Drawing.Point(665, 96);
            this.viewCameraViewBtn.Name = "viewCameraViewBtn";
            this.viewCameraViewBtn.Size = new System.Drawing.Size(82, 23);
            this.viewCameraViewBtn.TabIndex = 9;
            this.viewCameraViewBtn.Text = "打开摄像头";
            this.viewCameraViewBtn.UseVisualStyleBackColor = true;
            this.viewCameraViewBtn.Click += new System.EventHandler(this.viewCameraBtn_Click);
            // 
            // stopCameraViewBtn
            // 
            this.stopCameraViewBtn.Location = new System.Drawing.Point(753, 96);
            this.stopCameraViewBtn.Name = "stopCameraViewBtn";
            this.stopCameraViewBtn.Size = new System.Drawing.Size(79, 23);
            this.stopCameraViewBtn.TabIndex = 9;
            this.stopCameraViewBtn.Text = "关闭摄像头";
            this.stopCameraViewBtn.UseVisualStyleBackColor = true;
            this.stopCameraViewBtn.Click += new System.EventHandler(this.stopCameraViewBtn_Click);
            // 
            // firstStockManageFormBtn
            // 
            this.firstStockManageFormBtn.Location = new System.Drawing.Point(665, 299);
            this.firstStockManageFormBtn.Name = "firstStockManageFormBtn";
            this.firstStockManageFormBtn.Size = new System.Drawing.Size(105, 34);
            this.firstStockManageFormBtn.TabIndex = 11;
            this.firstStockManageFormBtn.Text = "第一药仓管理";
            this.firstStockManageFormBtn.UseVisualStyleBackColor = true;
            this.firstStockManageFormBtn.Click += new System.EventHandler(this.firstStockManageFormBtn_Click);
            // 
            // tscbxCameras
            // 
            this.tscbxCameras.FormattingEnabled = true;
            this.tscbxCameras.Location = new System.Drawing.Point(664, 69);
            this.tscbxCameras.Name = "tscbxCameras";
            this.tscbxCameras.Size = new System.Drawing.Size(223, 20);
            this.tscbxCameras.TabIndex = 12;
            // 
            // captureByCameraBtn
            // 
            this.captureByCameraBtn.Location = new System.Drawing.Point(839, 96);
            this.captureByCameraBtn.Name = "captureByCameraBtn";
            this.captureByCameraBtn.Size = new System.Drawing.Size(48, 23);
            this.captureByCameraBtn.TabIndex = 13;
            this.captureByCameraBtn.Text = "截图";
            this.captureByCameraBtn.UseVisualStyleBackColor = true;
            this.captureByCameraBtn.Click += new System.EventHandler(this.captureByCameraBtn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(665, 124);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(222, 166);
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // testSendMedicineGettingCMDBtn
            // 
            this.testSendMedicineGettingCMDBtn.Location = new System.Drawing.Point(666, 11);
            this.testSendMedicineGettingCMDBtn.Name = "testSendMedicineGettingCMDBtn";
            this.testSendMedicineGettingCMDBtn.Size = new System.Drawing.Size(221, 50);
            this.testSendMedicineGettingCMDBtn.TabIndex = 15;
            this.testSendMedicineGettingCMDBtn.Text = "测试发送取药命令给PLC";
            this.testSendMedicineGettingCMDBtn.UseVisualStyleBackColor = true;
            this.testSendMedicineGettingCMDBtn.Click += new System.EventHandler(this.testSendMedicineGettingCMDBtn_Click);
            // 
            // medicineBindingManageBtn
            // 
            this.medicineBindingManageBtn.Location = new System.Drawing.Point(666, 353);
            this.medicineBindingManageBtn.Name = "medicineBindingManageBtn";
            this.medicineBindingManageBtn.Size = new System.Drawing.Size(104, 36);
            this.medicineBindingManageBtn.TabIndex = 16;
            this.medicineBindingManageBtn.Text = "布药";
            this.medicineBindingManageBtn.UseVisualStyleBackColor = true;
            this.medicineBindingManageBtn.Click += new System.EventHandler(this.medicineBindingManageBtn_Click);
            // 
            // animateControlContainerLabel
            // 
            this.animateControlContainerLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.animateControlContainerLabel.Location = new System.Drawing.Point(893, 201);
            this.animateControlContainerLabel.Name = "animateControlContainerLabel";
            this.animateControlContainerLabel.Size = new System.Drawing.Size(256, 89);
            this.animateControlContainerLabel.TabIndex = 17;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(891, 177);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(107, 12);
            this.label23.TabIndex = 18;
            this.label23.Text = "控件动画效果演示:";
            // 
            // medicineInventoryMangeBtn
            // 
            this.medicineInventoryMangeBtn.Location = new System.Drawing.Point(777, 353);
            this.medicineInventoryMangeBtn.Name = "medicineInventoryMangeBtn";
            this.medicineInventoryMangeBtn.Size = new System.Drawing.Size(111, 36);
            this.medicineInventoryMangeBtn.TabIndex = 16;
            this.medicineInventoryMangeBtn.Text = "上药";
            this.medicineInventoryMangeBtn.UseVisualStyleBackColor = true;
            this.medicineInventoryMangeBtn.Click += new System.EventHandler(this.medicineInventoryMangeBtn_Click);
            // 
            // medicineDeliveryBtn
            // 
            this.medicineDeliveryBtn.Location = new System.Drawing.Point(665, 419);
            this.medicineDeliveryBtn.Name = "medicineDeliveryBtn";
            this.medicineDeliveryBtn.Size = new System.Drawing.Size(223, 111);
            this.medicineDeliveryBtn.TabIndex = 19;
            this.medicineDeliveryBtn.Text = "取药";
            this.medicineDeliveryBtn.UseVisualStyleBackColor = true;
            this.medicineDeliveryBtn.Click += new System.EventHandler(this.medicineDeliveryBtn_Click);
            // 
            // qrCodeScanerComCombobox
            // 
            this.qrCodeScanerComCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.qrCodeScanerComCombobox.FormattingEnabled = true;
            this.qrCodeScanerComCombobox.Location = new System.Drawing.Point(745, 396);
            this.qrCodeScanerComCombobox.Name = "qrCodeScanerComCombobox";
            this.qrCodeScanerComCombobox.Size = new System.Drawing.Size(143, 20);
            this.qrCodeScanerComCombobox.TabIndex = 20;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(664, 399);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(77, 12);
            this.label24.TabIndex = 1;
            this.label24.Text = "选择扫码串口";
            // 
            // videoPlayerPanel
            // 
            this.videoPlayerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.videoPlayerPanel.Controls.Add(this.label26);
            this.videoPlayerPanel.Location = new System.Drawing.Point(893, 31);
            this.videoPlayerPanel.Name = "videoPlayerPanel";
            this.videoPlayerPanel.Size = new System.Drawing.Size(256, 137);
            this.videoPlayerPanel.TabIndex = 21;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(128, 123);
            this.label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(125, 12);
            this.label26.TabIndex = 0;
            this.label26.Text = "遮罩在视频上方的文字";
            // 
            // 部署按钮
            // 
            this.部署按钮.Location = new System.Drawing.Point(894, 481);
            this.部署按钮.Name = "部署按钮";
            this.部署按钮.Size = new System.Drawing.Size(255, 49);
            this.部署按钮.TabIndex = 22;
            this.部署按钮.Text = "部  署";
            this.部署按钮.UseVisualStyleBackColor = true;
            this.部署按钮.Click += new System.EventHandler(this.部署按钮_Click);
            // 
            // simulatePrintDeliveryRecoryPaperBtn
            // 
            this.simulatePrintDeliveryRecoryPaperBtn.Location = new System.Drawing.Point(895, 299);
            this.simulatePrintDeliveryRecoryPaperBtn.Name = "simulatePrintDeliveryRecoryPaperBtn";
            this.simulatePrintDeliveryRecoryPaperBtn.Size = new System.Drawing.Size(254, 34);
            this.simulatePrintDeliveryRecoryPaperBtn.TabIndex = 23;
            this.simulatePrintDeliveryRecoryPaperBtn.Text = "模拟打印付药单";
            this.simulatePrintDeliveryRecoryPaperBtn.UseVisualStyleBackColor = true;
            this.simulatePrintDeliveryRecoryPaperBtn.Click += new System.EventHandler(this.simulatePrintDeliveryRecoryPaperBtn_Click);
            // 
            // simulateShowMedicineOrderBtn
            // 
            this.simulateShowMedicineOrderBtn.Location = new System.Drawing.Point(895, 354);
            this.simulateShowMedicineOrderBtn.Name = "simulateShowMedicineOrderBtn";
            this.simulateShowMedicineOrderBtn.Size = new System.Drawing.Size(124, 35);
            this.simulateShowMedicineOrderBtn.TabIndex = 24;
            this.simulateShowMedicineOrderBtn.Text = "模拟显示处方信息";
            this.simulateShowMedicineOrderBtn.UseVisualStyleBackColor = true;
            this.simulateShowMedicineOrderBtn.Click += new System.EventHandler(this.simulateShowMedicineOrderBtn_Click);
            // 
            // showAutoCloseFormBtn
            // 
            this.showAutoCloseFormBtn.Location = new System.Drawing.Point(1025, 354);
            this.showAutoCloseFormBtn.Name = "showAutoCloseFormBtn";
            this.showAutoCloseFormBtn.Size = new System.Drawing.Size(124, 35);
            this.showAutoCloseFormBtn.TabIndex = 24;
            this.showAutoCloseFormBtn.Text = "显示定时关闭窗口";
            this.showAutoCloseFormBtn.UseVisualStyleBackColor = true;
            this.showAutoCloseFormBtn.Click += new System.EventHandler(this.showAutoCloseFormBtn_Click);
            // 
            // showAutoCloseMedicineOrderFormBtn
            // 
            this.showAutoCloseMedicineOrderFormBtn.Location = new System.Drawing.Point(1025, 396);
            this.showAutoCloseMedicineOrderFormBtn.Name = "showAutoCloseMedicineOrderFormBtn";
            this.showAutoCloseMedicineOrderFormBtn.Size = new System.Drawing.Size(124, 35);
            this.showAutoCloseMedicineOrderFormBtn.TabIndex = 24;
            this.showAutoCloseMedicineOrderFormBtn.Text = "显示定时关闭付药单";
            this.showAutoCloseMedicineOrderFormBtn.UseVisualStyleBackColor = true;
            this.showAutoCloseMedicineOrderFormBtn.Click += new System.EventHandler(this.showAutoCloseMedicineOrderFormBtn_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(895, 395);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(124, 35);
            this.button2.TabIndex = 24;
            this.button2.Text = "显示正在取药中文字";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.showMedicinesGettingFullScreenFormBtn);
            // 
            // showMedicinesGettingStatusVideoFormBtn
            // 
            this.showMedicinesGettingStatusVideoFormBtn.Location = new System.Drawing.Point(895, 436);
            this.showMedicinesGettingStatusVideoFormBtn.Name = "showMedicinesGettingStatusVideoFormBtn";
            this.showMedicinesGettingStatusVideoFormBtn.Size = new System.Drawing.Size(124, 35);
            this.showMedicinesGettingStatusVideoFormBtn.TabIndex = 24;
            this.showMedicinesGettingStatusVideoFormBtn.Text = "显示正在取药中视频";
            this.showMedicinesGettingStatusVideoFormBtn.UseVisualStyleBackColor = true;
            this.showMedicinesGettingStatusVideoFormBtn.Click += new System.EventHandler(this.showMedicinesGettingStatusVideoFormBtn_Click);
            // 
            // trySpeakTooMuchTTSBtn
            // 
            this.trySpeakTooMuchTTSBtn.Location = new System.Drawing.Point(1025, 436);
            this.trySpeakTooMuchTTSBtn.Name = "trySpeakTooMuchTTSBtn";
            this.trySpeakTooMuchTTSBtn.Size = new System.Drawing.Size(124, 34);
            this.trySpeakTooMuchTTSBtn.TabIndex = 25;
            this.trySpeakTooMuchTTSBtn.Text = "尝试连续播放音频";
            this.trySpeakTooMuchTTSBtn.UseVisualStyleBackColor = true;
            this.trySpeakTooMuchTTSBtn.Click += new System.EventHandler(this.trySpeakTooMuchTTSBtn_Click);
            // 
            // secondStockManageFormBtn
            // 
            this.secondStockManageFormBtn.Location = new System.Drawing.Point(777, 299);
            this.secondStockManageFormBtn.Name = "secondStockManageFormBtn";
            this.secondStockManageFormBtn.Size = new System.Drawing.Size(110, 34);
            this.secondStockManageFormBtn.TabIndex = 11;
            this.secondStockManageFormBtn.Text = "第二药仓管理";
            this.secondStockManageFormBtn.UseVisualStyleBackColor = true;
            this.secondStockManageFormBtn.Click += new System.EventHandler(this.secondStockManageFormBtn_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(891, 10);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(167, 12);
            this.label25.TabIndex = 18;
            this.label25.Text = "视频播放在panel上测试的控件";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(470, 418);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 57);
            this.button3.TabIndex = 26;
            this.button3.Text = "Modbus测试";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1163, 548);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.trySpeakTooMuchTTSBtn);
            this.Controls.Add(this.showMedicinesGettingStatusVideoFormBtn);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.showAutoCloseMedicineOrderFormBtn);
            this.Controls.Add(this.showAutoCloseFormBtn);
            this.Controls.Add(this.simulateShowMedicineOrderBtn);
            this.Controls.Add(this.simulatePrintDeliveryRecoryPaperBtn);
            this.Controls.Add(this.部署按钮);
            this.Controls.Add(this.videoPlayerPanel);
            this.Controls.Add(this.qrCodeScanerComCombobox);
            this.Controls.Add(this.medicineDeliveryBtn);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.animateControlContainerLabel);
            this.Controls.Add(this.medicineInventoryMangeBtn);
            this.Controls.Add(this.medicineBindingManageBtn);
            this.Controls.Add(this.testSendMedicineGettingCMDBtn);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.captureByCameraBtn);
            this.Controls.Add(this.tscbxCameras);
            this.Controls.Add(this.secondStockManageFormBtn);
            this.Controls.Add(this.firstStockManageFormBtn);
            this.Controls.Add(this.stopCameraViewBtn);
            this.Controls.Add(this.viewCameraViewBtn);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.paramChangeLinkCheckbox);
            this.Controls.Add(this.writeEndTextbox);
            this.Controls.Add(this.readCountTextBox);
            this.Controls.Add(this.readEndTextbox);
            this.Controls.Add(this.writeStartTextbox);
            this.Controls.Add(this.writeDBNumberTexbox);
            this.Controls.Add(this.readDBNumberTexbox);
            this.Controls.Add(this.readStartTextbox);
            this.Controls.Add(this.writeBufferTextbox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.readBufferX2ToIntTextbox);
            this.Controls.Add(this.readBufferX2Textbox);
            this.Controls.Add(this.writeBufferPer8IntTextbox);
            this.Controls.Add(this.writeBufferLongTextbox);
            this.Controls.Add(this.writeBufferX2Textbox);
            this.Controls.Add(this.readStringBufferTextbox);
            this.Controls.Add(this.readBufferTextbox);
            this.Controls.Add(this.writeAndReadBtn);
            this.Controls.Add(this.writeBtn);
            this.Controls.Add(this.readBtn);
            this.Controls.Add(this.closePlcBtn);
            this.Controls.Add(this.connectPlcBtn);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.readTypeCombox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.writeTypeCombox);
            this.Controls.Add(this.slotCombox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rackCombox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.plcIPCombox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "AMDM";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.videoPlayerPanel.ResumeLayout(false);
            this.videoPlayerPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox plcIPCombox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button connectPlcBtn;
        private System.Windows.Forms.ComboBox rackCombox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox slotCombox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button readBtn;
        private System.Windows.Forms.Button writeBtn;
        private System.Windows.Forms.TextBox readBufferTextbox;
        private System.Windows.Forms.TextBox writeBufferTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox readStartTextbox;
        private System.Windows.Forms.TextBox readEndTextbox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox writeStartTextbox;
        private System.Windows.Forms.TextBox writeEndTextbox;
        private System.Windows.Forms.ComboBox writeTypeCombox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox readTypeCombox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button writeAndReadBtn;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox readDBNumberTexbox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox writeDBNumberTexbox;
        private System.Windows.Forms.CheckBox paramChangeLinkCheckbox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button closePlcBtn;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox readStringBufferTextbox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox readCountTextBox;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox readBufferX2Textbox;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox readBufferX2ToIntTextbox;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox writeBufferX2Textbox;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox writeBufferLongTextbox;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox writeBufferPer8IntTextbox;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button viewCameraViewBtn;
        private System.Windows.Forms.Button stopCameraViewBtn;
        private System.Windows.Forms.Button firstStockManageFormBtn;
        private System.Windows.Forms.ComboBox tscbxCameras;
        private System.Windows.Forms.Button captureByCameraBtn;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button testSendMedicineGettingCMDBtn;
        private System.Windows.Forms.Button medicineBindingManageBtn;
        private System.Windows.Forms.Panel animateControlContainerLabel;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button medicineInventoryMangeBtn;
        private System.Windows.Forms.Button medicineDeliveryBtn;
        private System.Windows.Forms.ComboBox qrCodeScanerComCombobox;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Panel videoPlayerPanel;
        private System.Windows.Forms.Button 部署按钮;
        private System.Windows.Forms.Button simulatePrintDeliveryRecoryPaperBtn;
        private System.Windows.Forms.Button simulateShowMedicineOrderBtn;
        private System.Windows.Forms.Button showAutoCloseFormBtn;
        private System.Windows.Forms.Button showAutoCloseMedicineOrderFormBtn;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button showMedicinesGettingStatusVideoFormBtn;
        private System.Windows.Forms.Button trySpeakTooMuchTTSBtn;
        private System.Windows.Forms.Button secondStockManageFormBtn;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Button button3;

    }
}

