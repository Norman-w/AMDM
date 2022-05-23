namespace AMDM
{
    partial class ExpirationDataSelectForm
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
            this.cancelBtn = new System.Windows.Forms.Button();
            this.submitBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.yearLabel = new System.Windows.Forms.Button();
            this.monthLabel = new System.Windows.Forms.Button();
            this.dayLabel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelBtn
            // 
            this.cancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelBtn.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cancelBtn.ForeColor = System.Drawing.Color.Coral;
            this.cancelBtn.Location = new System.Drawing.Point(7, 379);
            this.cancelBtn.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(321, 68);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // submitBtn
            // 
            this.submitBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.submitBtn.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.submitBtn.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.submitBtn.Location = new System.Drawing.Point(839, 379);
            this.submitBtn.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.submitBtn.Name = "submitBtn";
            this.submitBtn.Size = new System.Drawing.Size(321, 68);
            this.submitBtn.TabIndex = 2;
            this.submitBtn.Text = "确认";
            this.submitBtn.UseVisualStyleBackColor = true;
            this.submitBtn.Click += new System.EventHandler(this.submitBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label1.Location = new System.Drawing.Point(503, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 42);
            this.label1.TabIndex = 1;
            this.label1.Text = "有效期至:";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.cancelBtn);
            this.panel1.Controls.Add(this.dayLabel);
            this.panel1.Controls.Add(this.monthLabel);
            this.panel1.Controls.Add(this.yearLabel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.submitBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1171, 476);
            this.panel1.TabIndex = 4;
            // 
            // yearLabel
            // 
            this.yearLabel.Font = new System.Drawing.Font("微软雅黑", 60F, System.Drawing.FontStyle.Bold);
            this.yearLabel.Location = new System.Drawing.Point(7, 98);
            this.yearLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.yearLabel.Name = "yearLabel";
            this.yearLabel.Size = new System.Drawing.Size(485, 189);
            this.yearLabel.TabIndex = 3;
            this.yearLabel.Text = "____年";
            this.yearLabel.UseVisualStyleBackColor = true;
            this.yearLabel.Click += new System.EventHandler(this.yearLabel_Click);
            // 
            // monthLabel
            // 
            this.monthLabel.Font = new System.Drawing.Font("微软雅黑", 60F, System.Drawing.FontStyle.Bold);
            this.monthLabel.Location = new System.Drawing.Point(510, 98);
            this.monthLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.monthLabel.Name = "monthLabel";
            this.monthLabel.Size = new System.Drawing.Size(318, 189);
            this.monthLabel.TabIndex = 4;
            this.monthLabel.Text = "__月";
            this.monthLabel.UseVisualStyleBackColor = true;
            this.monthLabel.Click += new System.EventHandler(this.monthLabel_Click);
            // 
            // dayLabel
            // 
            this.dayLabel.Font = new System.Drawing.Font("微软雅黑", 60F, System.Drawing.FontStyle.Bold);
            this.dayLabel.Location = new System.Drawing.Point(853, 98);
            this.dayLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.dayLabel.Name = "dayLabel";
            this.dayLabel.Size = new System.Drawing.Size(307, 189);
            this.dayLabel.TabIndex = 4;
            this.dayLabel.Text = "__日";
            this.dayLabel.UseVisualStyleBackColor = true;
            this.dayLabel.Click += new System.EventHandler(this.dayLabel_Click);
            // 
            // ExpirationDataSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1171, 476);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExpirationDataSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ExpirationDataSelectForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button submitBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button dayLabel;
        private System.Windows.Forms.Button monthLabel;
        private System.Windows.Forms.Button yearLabel;
    }
}