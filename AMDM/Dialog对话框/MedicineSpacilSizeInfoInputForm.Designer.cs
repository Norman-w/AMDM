namespace AMDM
{
    partial class MedicineSpicalSizeInfoInputForm
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
            this.longMMNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.submitBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.longMMNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 33);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "长度(进深)mm:";
            // 
            // longMMNumericUpDown
            // 
            this.longMMNumericUpDown.Font = new System.Drawing.Font("宋体", 25F);
            this.longMMNumericUpDown.Location = new System.Drawing.Point(209, 21);
            this.longMMNumericUpDown.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.longMMNumericUpDown.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.longMMNumericUpDown.Name = "longMMNumericUpDown";
            this.longMMNumericUpDown.Size = new System.Drawing.Size(120, 46);
            this.longMMNumericUpDown.TabIndex = 2;
            this.longMMNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.longMMNumericUpDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // submitBtn
            // 
            this.submitBtn.Location = new System.Drawing.Point(209, 138);
            this.submitBtn.Name = "submitBtn";
            this.submitBtn.Size = new System.Drawing.Size(120, 53);
            this.submitBtn.TabIndex = 3;
            this.submitBtn.Text = "确认";
            this.submitBtn.UseVisualStyleBackColor = true;
            this.submitBtn.Click += new System.EventHandler(this.submitBtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F);
            this.label4.ForeColor = System.Drawing.Color.Coral;
            this.label4.Location = new System.Drawing.Point(49, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "提示文本框";
            // 
            // MedicineSpicalSizeInfoInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 206);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.submitBtn);
            this.Controls.Add(this.longMMNumericUpDown);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 16F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "MedicineSpicalSizeInfoInputForm";
            this.Text = "药盒尺寸设置";
            ((System.ComponentModel.ISupportInitialize)(this.longMMNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown longMMNumericUpDown;
        private System.Windows.Forms.Button submitBtn;
        private System.Windows.Forms.Label label4;
    }
}