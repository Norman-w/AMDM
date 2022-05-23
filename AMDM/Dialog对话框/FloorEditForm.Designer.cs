namespace AMDM
{
    partial class FloorEditForm
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
            this.saveBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.bottomMMTextbox = new System.Windows.Forms.TextBox();
            this.topMMTextbox = new System.Windows.Forms.TextBox();
            this.floorIdLabel = new System.Windows.Forms.Label();
            this.stockIdLabel = new System.Windows.Forms.Label();
            this.indexOfStockLabel = new System.Windows.Forms.Label();
            this.widthMMTextbox = new System.Windows.Forms.TextBox();
            this.removeThisFloorBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(116, 193);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 29);
            this.saveBtn.TabIndex = 0;
            this.saveBtn.Text = "保存";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(92, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "所属药仓:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "ID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "在药仓中位置:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "宽度:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "底部(起点位)毫米值:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "顶部(顶点位)毫米值:";
            // 
            // bottomMMTextbox
            // 
            this.bottomMMTextbox.Location = new System.Drawing.Point(137, 129);
            this.bottomMMTextbox.Name = "bottomMMTextbox";
            this.bottomMMTextbox.Size = new System.Drawing.Size(54, 21);
            this.bottomMMTextbox.TabIndex = 2;
            this.bottomMMTextbox.Text = "0";
            // 
            // topMMTextbox
            // 
            this.topMMTextbox.Location = new System.Drawing.Point(137, 95);
            this.topMMTextbox.Name = "topMMTextbox";
            this.topMMTextbox.Size = new System.Drawing.Size(54, 21);
            this.topMMTextbox.TabIndex = 2;
            this.topMMTextbox.Text = "80";
            // 
            // floorIdLabel
            // 
            this.floorIdLabel.AutoSize = true;
            this.floorIdLabel.Location = new System.Drawing.Point(41, 9);
            this.floorIdLabel.Name = "floorIdLabel";
            this.floorIdLabel.Size = new System.Drawing.Size(29, 12);
            this.floorIdLabel.TabIndex = 1;
            this.floorIdLabel.Text = "1541";
            // 
            // stockIdLabel
            // 
            this.stockIdLabel.AutoSize = true;
            this.stockIdLabel.Location = new System.Drawing.Point(157, 9);
            this.stockIdLabel.Name = "stockIdLabel";
            this.stockIdLabel.Size = new System.Drawing.Size(17, 12);
            this.stockIdLabel.TabIndex = 1;
            this.stockIdLabel.Text = "14";
            // 
            // indexOfStockLabel
            // 
            this.indexOfStockLabel.AutoSize = true;
            this.indexOfStockLabel.Location = new System.Drawing.Point(157, 34);
            this.indexOfStockLabel.Name = "indexOfStockLabel";
            this.indexOfStockLabel.Size = new System.Drawing.Size(11, 12);
            this.indexOfStockLabel.TabIndex = 1;
            this.indexOfStockLabel.Text = "0";
            // 
            // widthMMTextbox
            // 
            this.widthMMTextbox.Location = new System.Drawing.Point(137, 59);
            this.widthMMTextbox.Name = "widthMMTextbox";
            this.widthMMTextbox.Size = new System.Drawing.Size(54, 21);
            this.widthMMTextbox.TabIndex = 2;
            this.widthMMTextbox.Text = "975";
            // 
            // removeThisFloorBtn
            // 
            this.removeThisFloorBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.removeThisFloorBtn.Location = new System.Drawing.Point(12, 193);
            this.removeThisFloorBtn.Name = "removeThisFloorBtn";
            this.removeThisFloorBtn.Size = new System.Drawing.Size(83, 29);
            this.removeThisFloorBtn.TabIndex = 3;
            this.removeThisFloorBtn.Text = "删除该层";
            this.removeThisFloorBtn.UseVisualStyleBackColor = false;
            this.removeThisFloorBtn.Click += new System.EventHandler(this.removeThisFloorBtn_Click);
            // 
            // FloorEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(203, 230);
            this.Controls.Add(this.removeThisFloorBtn);
            this.Controls.Add(this.widthMMTextbox);
            this.Controls.Add(this.topMMTextbox);
            this.Controls.Add(this.bottomMMTextbox);
            this.Controls.Add(this.indexOfStockLabel);
            this.Controls.Add(this.stockIdLabel);
            this.Controls.Add(this.floorIdLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.saveBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FloorEditForm";
            this.Text = "编辑整层";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox bottomMMTextbox;
        private System.Windows.Forms.TextBox topMMTextbox;
        private System.Windows.Forms.Label floorIdLabel;
        private System.Windows.Forms.Label stockIdLabel;
        private System.Windows.Forms.Label indexOfStockLabel;
        private System.Windows.Forms.TextBox widthMMTextbox;
        private System.Windows.Forms.Button removeThisFloorBtn;
    }
}