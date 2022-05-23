namespace FakeHISClient
{
    partial class SelectGridWidthForm
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
            this.width95MMBtn = new System.Windows.Forms.Button();
            this.width85MMBtn = new System.Windows.Forms.Button();
            this.width75MMBtn = new System.Windows.Forms.Button();
            this.width65MMBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.add5MMBtn = new System.Windows.Forms.Button();
            this.minus5MMBtn = new System.Windows.Forms.Button();
            this.customSizeBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // width95MMBtn
            // 
            this.width95MMBtn.Location = new System.Drawing.Point(12, 12);
            this.width95MMBtn.Name = "width95MMBtn";
            this.width95MMBtn.Size = new System.Drawing.Size(456, 58);
            this.width95MMBtn.TabIndex = 0;
            this.width95MMBtn.Text = "95毫米";
            this.width95MMBtn.UseVisualStyleBackColor = true;
            this.width95MMBtn.Click += new System.EventHandler(this.width95MMBtn_Click);
            // 
            // width85MMBtn
            // 
            this.width85MMBtn.Location = new System.Drawing.Point(12, 85);
            this.width85MMBtn.Name = "width85MMBtn";
            this.width85MMBtn.Size = new System.Drawing.Size(456, 58);
            this.width85MMBtn.TabIndex = 0;
            this.width85MMBtn.Text = "85毫米";
            this.width85MMBtn.UseVisualStyleBackColor = true;
            this.width85MMBtn.Click += new System.EventHandler(this.width85MMBtn_Click);
            // 
            // width75MMBtn
            // 
            this.width75MMBtn.Location = new System.Drawing.Point(12, 166);
            this.width75MMBtn.Name = "width75MMBtn";
            this.width75MMBtn.Size = new System.Drawing.Size(456, 58);
            this.width75MMBtn.TabIndex = 0;
            this.width75MMBtn.Text = "75毫米";
            this.width75MMBtn.UseVisualStyleBackColor = true;
            this.width75MMBtn.Click += new System.EventHandler(this.width75MMBtn_Click);
            // 
            // width65MMBtn
            // 
            this.width65MMBtn.Location = new System.Drawing.Point(12, 243);
            this.width65MMBtn.Name = "width65MMBtn";
            this.width65MMBtn.Size = new System.Drawing.Size(456, 58);
            this.width65MMBtn.TabIndex = 0;
            this.width65MMBtn.Text = "65毫米";
            this.width65MMBtn.UseVisualStyleBackColor = true;
            this.width65MMBtn.Click += new System.EventHandler(this.width65MMBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(10, 347);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 29);
            this.label1.TabIndex = 2;
            // 
            // add5MMBtn
            // 
            this.add5MMBtn.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.add5MMBtn.Location = new System.Drawing.Point(421, 344);
            this.add5MMBtn.Name = "add5MMBtn";
            this.add5MMBtn.Size = new System.Drawing.Size(47, 58);
            this.add5MMBtn.TabIndex = 3;
            this.add5MMBtn.Text = "+";
            this.add5MMBtn.UseVisualStyleBackColor = true;
            this.add5MMBtn.Click += new System.EventHandler(this.add5MMBtn_Click);
            // 
            // minus5MMBtn
            // 
            this.minus5MMBtn.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.minus5MMBtn.Location = new System.Drawing.Point(12, 344);
            this.minus5MMBtn.Name = "minus5MMBtn";
            this.minus5MMBtn.Size = new System.Drawing.Size(47, 58);
            this.minus5MMBtn.TabIndex = 3;
            this.minus5MMBtn.Text = "-";
            this.minus5MMBtn.UseVisualStyleBackColor = true;
            this.minus5MMBtn.Click += new System.EventHandler(this.minus5MMBtn_Click);
            // 
            // customSizeBtn
            // 
            this.customSizeBtn.Location = new System.Drawing.Point(65, 344);
            this.customSizeBtn.Name = "customSizeBtn";
            this.customSizeBtn.Size = new System.Drawing.Size(350, 58);
            this.customSizeBtn.TabIndex = 4;
            this.customSizeBtn.Text = "设为  100  毫米";
            this.customSizeBtn.UseVisualStyleBackColor = true;
            this.customSizeBtn.Click += new System.EventHandler(this.customSizeBtn_Click);
            // 
            // SelectGridWidthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 433);
            this.Controls.Add(this.customSizeBtn);
            this.Controls.Add(this.minus5MMBtn);
            this.Controls.Add(this.add5MMBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.width65MMBtn);
            this.Controls.Add(this.width75MMBtn);
            this.Controls.Add(this.width85MMBtn);
            this.Controls.Add(this.width95MMBtn);
            this.Name = "SelectGridWidthForm";
            this.Text = "选择药槽尺寸";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button width95MMBtn;
        private System.Windows.Forms.Button width85MMBtn;
        private System.Windows.Forms.Button width75MMBtn;
        private System.Windows.Forms.Button width65MMBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button add5MMBtn;
        private System.Windows.Forms.Button minus5MMBtn;
        private System.Windows.Forms.Button customSizeBtn;
    }
}