namespace AMDM
{
    partial class MedicineInventoryManageForm
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
            this.gridsInUpPartShowerPanel = new System.Windows.Forms.Panel();
            this.simulatScanMedicine100BarcodeBtn = new System.Windows.Forms.Button();
            this.simulatScanMedicine001BarcodeBtn = new System.Windows.Forms.Button();
            this.simulatScanMedicineBarcodeBtn = new System.Windows.Forms.Button();
            this.medicineKindCountLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.currentMedicineBarcodeLabel = new System.Windows.Forms.Label();
            this.currentMedicineNameLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.closeBtn = new System.Windows.Forms.Button();
            this.maxSizeBtn = new System.Windows.Forms.Button();
            this.minSizeBtn = new System.Windows.Forms.Button();
            this.gridsInDownPartShowerPanel = new System.Windows.Forms.Panel();
            this.medicineBindingManageBtn = new System.Windows.Forms.Button();
            this.currentStockIndexLabel = new System.Windows.Forms.Label();
            this.swithStockBtn = new System.Windows.Forms.Button();
            this.currentStockTemperatureLabel = new System.Windows.Forms.Label();
            this.ACStatusLabel = new System.Windows.Forms.Label();
            this.rePrintDeliveryRecordPaperBtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.simulatScanMedicineDYSBarcodeBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // gridsInUpPartShowerPanel
            // 
            this.gridsInUpPartShowerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridsInUpPartShowerPanel.Location = new System.Drawing.Point(12, 36);
            this.gridsInUpPartShowerPanel.Name = "gridsInUpPartShowerPanel";
            this.gridsInUpPartShowerPanel.Size = new System.Drawing.Size(1342, 610);
            this.gridsInUpPartShowerPanel.TabIndex = 0;
            this.gridsInUpPartShowerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gridsInUpPartShowerPanel_Paint);
            // 
            // simulatScanMedicine100BarcodeBtn
            // 
            this.simulatScanMedicine100BarcodeBtn.Location = new System.Drawing.Point(988, 792);
            this.simulatScanMedicine100BarcodeBtn.Name = "simulatScanMedicine100BarcodeBtn";
            this.simulatScanMedicine100BarcodeBtn.Size = new System.Drawing.Size(111, 23);
            this.simulatScanMedicine100BarcodeBtn.TabIndex = 3;
            this.simulatScanMedicine100BarcodeBtn.Text = "模拟扫描100药品";
            this.simulatScanMedicine100BarcodeBtn.UseVisualStyleBackColor = true;
            this.simulatScanMedicine100BarcodeBtn.Visible = false;
            this.simulatScanMedicine100BarcodeBtn.Click += new System.EventHandler(this.simulatScanMedicine100BarcodeBtn_Click);
            // 
            // simulatScanMedicine001BarcodeBtn
            // 
            this.simulatScanMedicine001BarcodeBtn.Location = new System.Drawing.Point(867, 792);
            this.simulatScanMedicine001BarcodeBtn.Name = "simulatScanMedicine001BarcodeBtn";
            this.simulatScanMedicine001BarcodeBtn.Size = new System.Drawing.Size(115, 23);
            this.simulatScanMedicine001BarcodeBtn.TabIndex = 3;
            this.simulatScanMedicine001BarcodeBtn.Text = "模拟扫描001药品";
            this.simulatScanMedicine001BarcodeBtn.UseVisualStyleBackColor = true;
            this.simulatScanMedicine001BarcodeBtn.Visible = false;
            this.simulatScanMedicine001BarcodeBtn.Click += new System.EventHandler(this.simulatScanMedicine001BarcodeBtn_Click);
            // 
            // simulatScanMedicineBarcodeBtn
            // 
            this.simulatScanMedicineBarcodeBtn.Location = new System.Drawing.Point(747, 792);
            this.simulatScanMedicineBarcodeBtn.Name = "simulatScanMedicineBarcodeBtn";
            this.simulatScanMedicineBarcodeBtn.Size = new System.Drawing.Size(114, 23);
            this.simulatScanMedicineBarcodeBtn.TabIndex = 2;
            this.simulatScanMedicineBarcodeBtn.Text = "模拟扫描条码";
            this.simulatScanMedicineBarcodeBtn.UseVisualStyleBackColor = true;
            this.simulatScanMedicineBarcodeBtn.Visible = false;
            this.simulatScanMedicineBarcodeBtn.Click += new System.EventHandler(this.simulatScanMedicineBarcodeBtn_Click);
            // 
            // medicineKindCountLabel
            // 
            this.medicineKindCountLabel.AutoSize = true;
            this.medicineKindCountLabel.Font = new System.Drawing.Font("宋体", 12F);
            this.medicineKindCountLabel.Location = new System.Drawing.Point(1067, 656);
            this.medicineKindCountLabel.Name = "medicineKindCountLabel";
            this.medicineKindCountLabel.Size = new System.Drawing.Size(32, 16);
            this.medicineKindCountLabel.TabIndex = 1;
            this.medicineKindCountLabel.Text = "113";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Gray;
            this.label7.Location = new System.Drawing.Point(996, 763);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "条码:";
            // 
            // currentMedicineBarcodeLabel
            // 
            this.currentMedicineBarcodeLabel.AutoSize = true;
            this.currentMedicineBarcodeLabel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.currentMedicineBarcodeLabel.Location = new System.Drawing.Point(1051, 759);
            this.currentMedicineBarcodeLabel.Name = "currentMedicineBarcodeLabel";
            this.currentMedicineBarcodeLabel.Size = new System.Drawing.Size(112, 16);
            this.currentMedicineBarcodeLabel.TabIndex = 1;
            this.currentMedicineBarcodeLabel.Text = "6948417754135";
            // 
            // currentMedicineNameLabel
            // 
            this.currentMedicineNameLabel.AutoSize = true;
            this.currentMedicineNameLabel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.currentMedicineNameLabel.Location = new System.Drawing.Point(1051, 727);
            this.currentMedicineNameLabel.Name = "currentMedicineNameLabel";
            this.currentMedicineNameLabel.Size = new System.Drawing.Size(160, 16);
            this.currentMedicineNameLabel.TabIndex = 1;
            this.currentMedicineNameLabel.Text = "测试药品的名称xxx22";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Gray;
            this.label6.Location = new System.Drawing.Point(996, 731);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "名称:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Gray;
            this.label5.Location = new System.Drawing.Point(996, 706);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "当前扫描的药品:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Gray;
            this.label3.Location = new System.Drawing.Point(996, 660);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "药品种类数:";
            // 
            // closeBtn
            // 
            this.closeBtn.BackColor = System.Drawing.Color.Bisque;
            this.closeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.closeBtn.ForeColor = System.Drawing.Color.Black;
            this.closeBtn.Location = new System.Drawing.Point(1292, 5);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(62, 23);
            this.closeBtn.TabIndex = 16;
            this.closeBtn.Tag = "";
            this.closeBtn.Text = "补药完成";
            this.closeBtn.UseVisualStyleBackColor = false;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // maxSizeBtn
            // 
            this.maxSizeBtn.BackColor = System.Drawing.Color.Bisque;
            this.maxSizeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.maxSizeBtn.ForeColor = System.Drawing.Color.Black;
            this.maxSizeBtn.Location = new System.Drawing.Point(1258, 5);
            this.maxSizeBtn.Name = "maxSizeBtn";
            this.maxSizeBtn.Size = new System.Drawing.Size(29, 23);
            this.maxSizeBtn.TabIndex = 17;
            this.maxSizeBtn.Tag = "";
            this.maxSizeBtn.Text = "□";
            this.maxSizeBtn.UseVisualStyleBackColor = false;
            this.maxSizeBtn.Visible = false;
            this.maxSizeBtn.Click += new System.EventHandler(this.maxSizeBtn_Click);
            // 
            // minSizeBtn
            // 
            this.minSizeBtn.BackColor = System.Drawing.Color.Bisque;
            this.minSizeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.minSizeBtn.ForeColor = System.Drawing.Color.Black;
            this.minSizeBtn.Location = new System.Drawing.Point(1225, 5);
            this.minSizeBtn.Name = "minSizeBtn";
            this.minSizeBtn.Size = new System.Drawing.Size(29, 23);
            this.minSizeBtn.TabIndex = 18;
            this.minSizeBtn.Tag = "";
            this.minSizeBtn.Text = "-";
            this.minSizeBtn.UseVisualStyleBackColor = false;
            this.minSizeBtn.Visible = false;
            this.minSizeBtn.Click += new System.EventHandler(this.minSizeBtn_Click);
            // 
            // gridsInDownPartShowerPanel
            // 
            this.gridsInDownPartShowerPanel.Location = new System.Drawing.Point(12, 652);
            this.gridsInDownPartShowerPanel.Name = "gridsInDownPartShowerPanel";
            this.gridsInDownPartShowerPanel.Size = new System.Drawing.Size(973, 144);
            this.gridsInDownPartShowerPanel.TabIndex = 19;
            this.gridsInDownPartShowerPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gridsInDownPartShowerPanel_Paint);
            // 
            // medicineBindingManageBtn
            // 
            this.medicineBindingManageBtn.BackColor = System.Drawing.Color.Bisque;
            this.medicineBindingManageBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.medicineBindingManageBtn.ForeColor = System.Drawing.Color.Black;
            this.medicineBindingManageBtn.Location = new System.Drawing.Point(698, 7);
            this.medicineBindingManageBtn.Name = "medicineBindingManageBtn";
            this.medicineBindingManageBtn.Size = new System.Drawing.Size(76, 25);
            this.medicineBindingManageBtn.TabIndex = 18;
            this.medicineBindingManageBtn.Tag = "";
            this.medicineBindingManageBtn.Text = "布药";
            this.medicineBindingManageBtn.UseVisualStyleBackColor = false;
            this.medicineBindingManageBtn.Click += new System.EventHandler(this.medicineBindingManageBtn_Click);
            // 
            // currentStockIndexLabel
            // 
            this.currentStockIndexLabel.AutoSize = true;
            this.currentStockIndexLabel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.currentStockIndexLabel.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.currentStockIndexLabel.ForeColor = System.Drawing.Color.Black;
            this.currentStockIndexLabel.Location = new System.Drawing.Point(12, 7);
            this.currentStockIndexLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.currentStockIndexLabel.Name = "currentStockIndexLabel";
            this.currentStockIndexLabel.Size = new System.Drawing.Size(44, 21);
            this.currentStockIndexLabel.TabIndex = 21;
            this.currentStockIndexLabel.Text = "01仓";
            this.currentStockIndexLabel.Visible = false;
            // 
            // swithStockBtn
            // 
            this.swithStockBtn.BackColor = System.Drawing.Color.Bisque;
            this.swithStockBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.swithStockBtn.ForeColor = System.Drawing.Color.Black;
            this.swithStockBtn.Location = new System.Drawing.Point(78, 7);
            this.swithStockBtn.Name = "swithStockBtn";
            this.swithStockBtn.Size = new System.Drawing.Size(46, 25);
            this.swithStockBtn.TabIndex = 18;
            this.swithStockBtn.Tag = "";
            this.swithStockBtn.Text = "切换";
            this.swithStockBtn.UseVisualStyleBackColor = false;
            this.swithStockBtn.Visible = false;
            // 
            // currentStockTemperatureLabel
            // 
            this.currentStockTemperatureLabel.AutoSize = true;
            this.currentStockTemperatureLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.currentStockTemperatureLabel.Location = new System.Drawing.Point(1190, 10);
            this.currentStockTemperatureLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.currentStockTemperatureLabel.Name = "currentStockTemperatureLabel";
            this.currentStockTemperatureLabel.Size = new System.Drawing.Size(34, 17);
            this.currentStockTemperatureLabel.TabIndex = 22;
            this.currentStockTemperatureLabel.Text = "18℃";
            // 
            // ACStatusLabel
            // 
            this.ACStatusLabel.AutoSize = true;
            this.ACStatusLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ACStatusLabel.Location = new System.Drawing.Point(1116, 10);
            this.ACStatusLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ACStatusLabel.Name = "ACStatusLabel";
            this.ACStatusLabel.Size = new System.Drawing.Size(68, 17);
            this.ACStatusLabel.TabIndex = 22;
            this.ACStatusLabel.Text = "空调未启动";
            this.ACStatusLabel.Visible = false;
            // 
            // rePrintDeliveryRecordPaperBtn
            // 
            this.rePrintDeliveryRecordPaperBtn.BackColor = System.Drawing.Color.Bisque;
            this.rePrintDeliveryRecordPaperBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rePrintDeliveryRecordPaperBtn.ForeColor = System.Drawing.Color.Black;
            this.rePrintDeliveryRecordPaperBtn.Location = new System.Drawing.Point(616, 7);
            this.rePrintDeliveryRecordPaperBtn.Name = "rePrintDeliveryRecordPaperBtn";
            this.rePrintDeliveryRecordPaperBtn.Size = new System.Drawing.Size(76, 25);
            this.rePrintDeliveryRecordPaperBtn.TabIndex = 18;
            this.rePrintDeliveryRecordPaperBtn.Tag = "";
            this.rePrintDeliveryRecordPaperBtn.Text = "补打详单";
            this.rePrintDeliveryRecordPaperBtn.UseVisualStyleBackColor = false;
            this.rePrintDeliveryRecordPaperBtn.Visible = false;
            this.rePrintDeliveryRecordPaperBtn.Click += new System.EventHandler(this.rePrintDeliveryRecordPaperBtn_Click);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Location = new System.Drawing.Point(991, 652);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(363, 144);
            this.button1.TabIndex = 23;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // simulatScanMedicineDYSBarcodeBtn
            // 
            this.simulatScanMedicineDYSBarcodeBtn.Location = new System.Drawing.Point(1105, 792);
            this.simulatScanMedicineDYSBarcodeBtn.Name = "simulatScanMedicineDYSBarcodeBtn";
            this.simulatScanMedicineDYSBarcodeBtn.Size = new System.Drawing.Size(115, 23);
            this.simulatScanMedicineDYSBarcodeBtn.TabIndex = 3;
            this.simulatScanMedicineDYSBarcodeBtn.Text = "模拟扫描地榆升白胶囊";
            this.simulatScanMedicineDYSBarcodeBtn.UseVisualStyleBackColor = true;
            this.simulatScanMedicineDYSBarcodeBtn.Visible = false;
            this.simulatScanMedicineDYSBarcodeBtn.Click += new System.EventHandler(this.simulatScanMedicineDYSBarcodeBtn_Click);
            // 
            // MedicineInventoryManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1366, 806);
            this.Controls.Add(this.ACStatusLabel);
            this.Controls.Add(this.simulatScanMedicine100BarcodeBtn);
            this.Controls.Add(this.currentStockTemperatureLabel);
            this.Controls.Add(this.simulatScanMedicineDYSBarcodeBtn);
            this.Controls.Add(this.simulatScanMedicine001BarcodeBtn);
            this.Controls.Add(this.currentStockIndexLabel);
            this.Controls.Add(this.simulatScanMedicineBarcodeBtn);
            this.Controls.Add(this.gridsInDownPartShowerPanel);
            this.Controls.Add(this.medicineKindCountLabel);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.maxSizeBtn);
            this.Controls.Add(this.currentMedicineBarcodeLabel);
            this.Controls.Add(this.swithStockBtn);
            this.Controls.Add(this.currentMedicineNameLabel);
            this.Controls.Add(this.rePrintDeliveryRecordPaperBtn);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.medicineBindingManageBtn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.minSizeBtn);
            this.Controls.Add(this.gridsInUpPartShowerPanel);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MedicineInventoryManageForm";
            this.Text = "上药(药品库存管理)";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MedicineInventoryManageForm_FormClosed);
            this.Load += new System.EventHandler(this.MedicineInventoryManageForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel gridsInUpPartShowerPanel;
        private System.Windows.Forms.Label medicineKindCountLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label currentMedicineBarcodeLabel;
        private System.Windows.Forms.Label currentMedicineNameLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button simulatScanMedicine100BarcodeBtn;
        private System.Windows.Forms.Button simulatScanMedicine001BarcodeBtn;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Button maxSizeBtn;
        private System.Windows.Forms.Button minSizeBtn;
        private System.Windows.Forms.Button simulatScanMedicineBarcodeBtn;
        private System.Windows.Forms.Panel gridsInDownPartShowerPanel;
        private System.Windows.Forms.Button medicineBindingManageBtn;
        private System.Windows.Forms.Label currentStockIndexLabel;
        private System.Windows.Forms.Button swithStockBtn;
        private System.Windows.Forms.Label currentStockTemperatureLabel;
        private System.Windows.Forms.Label ACStatusLabel;
        private System.Windows.Forms.Button rePrintDeliveryRecordPaperBtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button simulatScanMedicineDYSBarcodeBtn;
    }
}