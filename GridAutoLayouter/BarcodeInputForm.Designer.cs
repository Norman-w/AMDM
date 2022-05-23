namespace GridAutoLayouter
{
    partial class BarcodeInputForm
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.确定按钮 = new System.Windows.Forms.Button();
            this.取消按钮 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 40F);
            this.textBox1.Location = new System.Drawing.Point(12, 88);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(672, 68);
            this.textBox1.TabIndex = 0;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // 确定按钮
            // 
            this.确定按钮.Font = new System.Drawing.Font("微软雅黑", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.确定按钮.Location = new System.Drawing.Point(471, 180);
            this.确定按钮.Name = "确定按钮";
            this.确定按钮.Size = new System.Drawing.Size(213, 81);
            this.确定按钮.TabIndex = 1;
            this.确定按钮.Text = "确定";
            this.确定按钮.UseVisualStyleBackColor = true;
            this.确定按钮.Click += new System.EventHandler(this.确定按钮_Click);
            // 
            // 取消按钮
            // 
            this.取消按钮.Font = new System.Drawing.Font("微软雅黑", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.取消按钮.Location = new System.Drawing.Point(12, 180);
            this.取消按钮.Name = "取消按钮";
            this.取消按钮.Size = new System.Drawing.Size(213, 81);
            this.取消按钮.TabIndex = 1;
            this.取消按钮.Text = "取消";
            this.取消按钮.UseVisualStyleBackColor = true;
            this.取消按钮.Click += new System.EventHandler(this.取消按钮_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 24.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(696, 43);
            this.label1.TabIndex = 2;
            this.label1.Text = "标题内容";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BarcodeInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 310);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.取消按钮);
            this.Controls.Add(this.确定按钮);
            this.Controls.Add(this.textBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BarcodeInputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BarcodeInputForm";
            this.Load += new System.EventHandler(this.BarcodeInputForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button 确定按钮;
        private System.Windows.Forms.Button 取消按钮;
        private System.Windows.Forms.Label label1;
    }
}