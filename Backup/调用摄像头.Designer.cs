namespace 摄像头
{
    partial class 调用摄像头
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
            this.panel = new System.Windows.Forms.Panel();
            this.preViewBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Location = new System.Drawing.Point(28, 12);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(445, 388);
            this.panel.TabIndex = 0;
            // 
            // preViewBtn
            // 
            this.preViewBtn.Location = new System.Drawing.Point(479, 12);
            this.preViewBtn.Name = "preViewBtn";
            this.preViewBtn.Size = new System.Drawing.Size(75, 23);
            this.preViewBtn.TabIndex = 1;
            this.preViewBtn.Text = "预览";
            this.preViewBtn.UseVisualStyleBackColor = true;
            this.preViewBtn.Click += new System.EventHandler(this.preViewBtn_Click);
            // 
            // 调用摄像头
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 412);
            this.Controls.Add(this.preViewBtn);
            this.Controls.Add(this.panel);
            this.Name = "调用摄像头";
            this.Text = "摄像头测试";
            this.Load += new System.EventHandler(this.调用摄像头_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Button preViewBtn;
    }
}

