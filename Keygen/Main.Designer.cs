namespace Keygen
{
    partial class Main
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
            this.生成公钥私钥对 = new System.Windows.Forms.Button();
            this.填写设备的SerialNumber = new System.Windows.Forms.Button();
            this.获取设备的硬件信息 = new System.Windows.Forms.Button();
            this.生成私钥信息对象 = new System.Windows.Forms.Button();
            this.保存私钥信息到文件 = new System.Windows.Forms.Button();
            this.提取公钥准备安装 = new System.Windows.Forms.Button();
            this.使用公钥文件加密器加密公钥文件 = new System.Windows.Forms.Button();
            this.把设备信息保存到json然后base64 = new System.Windows.Forms.Button();
            this.使用私钥将这个base64签名 = new System.Windows.Forms.Button();
            this.生成guid作为公钥加密文件的文件名 = new System.Windows.Forms.Button();
            this.生成guid作为设备签名信息文件的文件名 = new System.Windows.Forms.Button();
            this.将生成的公钥文件名转换成加密的byte数组 = new System.Windows.Forms.Button();
            this.将生成的签名文件名转换成加密的byte数组 = new System.Windows.Forms.Button();
            this.公钥 = new System.Windows.Forms.TextBox();
            this.私钥 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.设备sn = new System.Windows.Forms.TextBox();
            this.设备硬件信息 = new System.Windows.Forms.TextBox();
            this.私钥信息对象 = new System.Windows.Forms.TextBox();
            this.私钥保存文件位置 = new System.Windows.Forms.TextBox();
            this.加密后的公钥文件 = new System.Windows.Forms.TextBox();
            this.公钥的目标文件名 = new System.Windows.Forms.TextBox();
            this.公钥的目标文件名加密后的byte数组 = new System.Windows.Forms.TextBox();
            this.设备信息base64 = new System.Windows.Forms.TextBox();
            this.设备信息base64的私钥签过的名 = new System.Windows.Forms.TextBox();
            this.设备签名信息文件名 = new System.Windows.Forms.TextBox();
            this.签名文件目标文件名的加密后byte = new System.Windows.Forms.TextBox();
            this.保存公钥资源文件和文件名文件 = new System.Windows.Forms.Button();
            this.保存签名资源文件和文件名文件 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.完成btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // 生成公钥私钥对
            // 
            this.生成公钥私钥对.Location = new System.Drawing.Point(12, 24);
            this.生成公钥私钥对.Name = "生成公钥私钥对";
            this.生成公钥私钥对.Size = new System.Drawing.Size(187, 31);
            this.生成公钥私钥对.TabIndex = 0;
            this.生成公钥私钥对.Text = "1生成公钥私钥对";
            this.生成公钥私钥对.UseVisualStyleBackColor = true;
            this.生成公钥私钥对.Click += new System.EventHandler(this.生成公钥私钥对_Click);
            // 
            // 填写设备的SerialNumber
            // 
            this.填写设备的SerialNumber.Location = new System.Drawing.Point(433, 207);
            this.填写设备的SerialNumber.Name = "填写设备的SerialNumber";
            this.填写设备的SerialNumber.Size = new System.Drawing.Size(187, 43);
            this.填写设备的SerialNumber.TabIndex = 0;
            this.填写设备的SerialNumber.Text = "2填写设备的SerialNumber";
            this.填写设备的SerialNumber.UseVisualStyleBackColor = true;
            this.填写设备的SerialNumber.Click += new System.EventHandler(this.填写设备的SerialNumber_Click);
            // 
            // 获取设备的硬件信息
            // 
            this.获取设备的硬件信息.Location = new System.Drawing.Point(12, 256);
            this.获取设备的硬件信息.Name = "获取设备的硬件信息";
            this.获取设备的硬件信息.Size = new System.Drawing.Size(187, 43);
            this.获取设备的硬件信息.TabIndex = 0;
            this.获取设备的硬件信息.Text = "3获取设备的硬件信息";
            this.获取设备的硬件信息.UseVisualStyleBackColor = true;
            this.获取设备的硬件信息.Click += new System.EventHandler(this.获取设备的硬件信息_Click);
            // 
            // 生成私钥信息对象
            // 
            this.生成私钥信息对象.Location = new System.Drawing.Point(12, 305);
            this.生成私钥信息对象.Name = "生成私钥信息对象";
            this.生成私钥信息对象.Size = new System.Drawing.Size(187, 60);
            this.生成私钥信息对象.TabIndex = 0;
            this.生成私钥信息对象.Text = "4生成私钥信息对象(包含SN,设备信息,私钥)";
            this.生成私钥信息对象.UseVisualStyleBackColor = true;
            this.生成私钥信息对象.Click += new System.EventHandler(this.生成私钥信息对象_Click);
            // 
            // 保存私钥信息到文件
            // 
            this.保存私钥信息到文件.Location = new System.Drawing.Point(12, 371);
            this.保存私钥信息到文件.Name = "保存私钥信息到文件";
            this.保存私钥信息到文件.Size = new System.Drawing.Size(187, 42);
            this.保存私钥信息到文件.TabIndex = 0;
            this.保存私钥信息到文件.Text = "5保存私钥信息到文件";
            this.保存私钥信息到文件.UseVisualStyleBackColor = true;
            this.保存私钥信息到文件.Click += new System.EventHandler(this.保存私钥信息到文件_Click);
            // 
            // 提取公钥准备安装
            // 
            this.提取公钥准备安装.Location = new System.Drawing.Point(12, 419);
            this.提取公钥准备安装.Name = "提取公钥准备安装";
            this.提取公钥准备安装.Size = new System.Drawing.Size(187, 42);
            this.提取公钥准备安装.TabIndex = 0;
            this.提取公钥准备安装.Text = "6提取公钥准备安装";
            this.提取公钥准备安装.UseVisualStyleBackColor = true;
            this.提取公钥准备安装.Click += new System.EventHandler(this.提取公钥准备安装_Click);
            // 
            // 使用公钥文件加密器加密公钥文件
            // 
            this.使用公钥文件加密器加密公钥文件.Location = new System.Drawing.Point(205, 419);
            this.使用公钥文件加密器加密公钥文件.Name = "使用公钥文件加密器加密公钥文件";
            this.使用公钥文件加密器加密公钥文件.Size = new System.Drawing.Size(187, 42);
            this.使用公钥文件加密器加密公钥文件.TabIndex = 0;
            this.使用公钥文件加密器加密公钥文件.Text = "7使用公钥文件加密器加密公钥文件";
            this.使用公钥文件加密器加密公钥文件.UseVisualStyleBackColor = true;
            this.使用公钥文件加密器加密公钥文件.Click += new System.EventHandler(this.使用公钥文件加密器加密公钥文件_Click);
            // 
            // 把设备信息保存到json然后base64
            // 
            this.把设备信息保存到json然后base64.Location = new System.Drawing.Point(12, 467);
            this.把设备信息保存到json然后base64.Name = "把设备信息保存到json然后base64";
            this.把设备信息保存到json然后base64.Size = new System.Drawing.Size(187, 42);
            this.把设备信息保存到json然后base64.TabIndex = 0;
            this.把设备信息保存到json然后base64.Text = "11把设备信息保存到json然后base64";
            this.把设备信息保存到json然后base64.UseVisualStyleBackColor = true;
            this.把设备信息保存到json然后base64.Click += new System.EventHandler(this.把设备信息保存到json然后base64_Click);
            // 
            // 使用私钥将这个base64签名
            // 
            this.使用私钥将这个base64签名.Location = new System.Drawing.Point(398, 467);
            this.使用私钥将这个base64签名.Name = "使用私钥将这个base64签名";
            this.使用私钥将这个base64签名.Size = new System.Drawing.Size(107, 42);
            this.使用私钥将这个base64签名.TabIndex = 0;
            this.使用私钥将这个base64签名.Text = "12使用私钥将这个base64签名";
            this.使用私钥将这个base64签名.UseVisualStyleBackColor = true;
            this.使用私钥将这个base64签名.Click += new System.EventHandler(this.使用私钥将这个base64签名_Click);
            // 
            // 生成guid作为公钥加密文件的文件名
            // 
            this.生成guid作为公钥加密文件的文件名.Location = new System.Drawing.Point(626, 419);
            this.生成guid作为公钥加密文件的文件名.Name = "生成guid作为公钥加密文件的文件名";
            this.生成guid作为公钥加密文件的文件名.Size = new System.Drawing.Size(187, 42);
            this.生成guid作为公钥加密文件的文件名.TabIndex = 0;
            this.生成guid作为公钥加密文件的文件名.Text = "8生成guid作为公钥加密文件的文件名";
            this.生成guid作为公钥加密文件的文件名.UseVisualStyleBackColor = true;
            this.生成guid作为公钥加密文件的文件名.Click += new System.EventHandler(this.生成guid作为公钥加密文件的文件名_Click);
            // 
            // 生成guid作为设备签名信息文件的文件名
            // 
            this.生成guid作为设备签名信息文件的文件名.Location = new System.Drawing.Point(626, 467);
            this.生成guid作为设备签名信息文件的文件名.Name = "生成guid作为设备签名信息文件的文件名";
            this.生成guid作为设备签名信息文件的文件名.Size = new System.Drawing.Size(187, 42);
            this.生成guid作为设备签名信息文件的文件名.TabIndex = 0;
            this.生成guid作为设备签名信息文件的文件名.Text = "13生成guid作为设备签名信息文件的文件名";
            this.生成guid作为设备签名信息文件的文件名.UseVisualStyleBackColor = true;
            this.生成guid作为设备签名信息文件的文件名.Click += new System.EventHandler(this.生成guid作为设备签名信息文件的文件名_Click);
            // 
            // 将生成的公钥文件名转换成加密的byte数组
            // 
            this.将生成的公钥文件名转换成加密的byte数组.Location = new System.Drawing.Point(1047, 419);
            this.将生成的公钥文件名转换成加密的byte数组.Name = "将生成的公钥文件名转换成加密的byte数组";
            this.将生成的公钥文件名转换成加密的byte数组.Size = new System.Drawing.Size(187, 42);
            this.将生成的公钥文件名转换成加密的byte数组.TabIndex = 0;
            this.将生成的公钥文件名转换成加密的byte数组.Text = "9将生成的公钥文件名转换成加密的byte数组";
            this.将生成的公钥文件名转换成加密的byte数组.UseVisualStyleBackColor = true;
            this.将生成的公钥文件名转换成加密的byte数组.Click += new System.EventHandler(this.将生成的公钥文件名转换成加密的byte数组_Click);
            // 
            // 将生成的签名文件名转换成加密的byte数组
            // 
            this.将生成的签名文件名转换成加密的byte数组.Location = new System.Drawing.Point(1047, 467);
            this.将生成的签名文件名转换成加密的byte数组.Name = "将生成的签名文件名转换成加密的byte数组";
            this.将生成的签名文件名转换成加密的byte数组.Size = new System.Drawing.Size(187, 42);
            this.将生成的签名文件名转换成加密的byte数组.TabIndex = 0;
            this.将生成的签名文件名转换成加密的byte数组.Text = "14将生成的签名文件名转换成加密的byte数组";
            this.将生成的签名文件名转换成加密的byte数组.UseVisualStyleBackColor = true;
            this.将生成的签名文件名转换成加密的byte数组.Click += new System.EventHandler(this.将生成的签名文件名转换成加密的byte数组_Click);
            // 
            // 公钥
            // 
            this.公钥.Location = new System.Drawing.Point(12, 74);
            this.公钥.Multiline = true;
            this.公钥.Name = "公钥";
            this.公钥.Size = new System.Drawing.Size(608, 127);
            this.公钥.TabIndex = 1;
            // 
            // 私钥
            // 
            this.私钥.Location = new System.Drawing.Point(626, 24);
            this.私钥.Multiline = true;
            this.私钥.Name = "私钥";
            this.私钥.Size = new System.Drawing.Size(942, 389);
            this.私钥.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "公钥";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(624, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "私钥";
            // 
            // 设备sn
            // 
            this.设备sn.Location = new System.Drawing.Point(12, 207);
            this.设备sn.Multiline = true;
            this.设备sn.Name = "设备sn";
            this.设备sn.Size = new System.Drawing.Size(415, 43);
            this.设备sn.TabIndex = 1;
            // 
            // 设备硬件信息
            // 
            this.设备硬件信息.Location = new System.Drawing.Point(205, 256);
            this.设备硬件信息.Multiline = true;
            this.设备硬件信息.Name = "设备硬件信息";
            this.设备硬件信息.Size = new System.Drawing.Size(415, 43);
            this.设备硬件信息.TabIndex = 1;
            // 
            // 私钥信息对象
            // 
            this.私钥信息对象.Location = new System.Drawing.Point(205, 305);
            this.私钥信息对象.Multiline = true;
            this.私钥信息对象.Name = "私钥信息对象";
            this.私钥信息对象.Size = new System.Drawing.Size(415, 60);
            this.私钥信息对象.TabIndex = 1;
            // 
            // 私钥保存文件位置
            // 
            this.私钥保存文件位置.Location = new System.Drawing.Point(205, 371);
            this.私钥保存文件位置.Multiline = true;
            this.私钥保存文件位置.Name = "私钥保存文件位置";
            this.私钥保存文件位置.Size = new System.Drawing.Size(415, 42);
            this.私钥保存文件位置.TabIndex = 1;
            // 
            // 加密后的公钥文件
            // 
            this.加密后的公钥文件.Location = new System.Drawing.Point(398, 419);
            this.加密后的公钥文件.Multiline = true;
            this.加密后的公钥文件.Name = "加密后的公钥文件";
            this.加密后的公钥文件.Size = new System.Drawing.Size(222, 42);
            this.加密后的公钥文件.TabIndex = 1;
            // 
            // 公钥的目标文件名
            // 
            this.公钥的目标文件名.Location = new System.Drawing.Point(819, 419);
            this.公钥的目标文件名.Multiline = true;
            this.公钥的目标文件名.Name = "公钥的目标文件名";
            this.公钥的目标文件名.Size = new System.Drawing.Size(222, 42);
            this.公钥的目标文件名.TabIndex = 1;
            // 
            // 公钥的目标文件名加密后的byte数组
            // 
            this.公钥的目标文件名加密后的byte数组.Location = new System.Drawing.Point(1240, 419);
            this.公钥的目标文件名加密后的byte数组.Multiline = true;
            this.公钥的目标文件名加密后的byte数组.Name = "公钥的目标文件名加密后的byte数组";
            this.公钥的目标文件名加密后的byte数组.Size = new System.Drawing.Size(198, 42);
            this.公钥的目标文件名加密后的byte数组.TabIndex = 1;
            // 
            // 设备信息base64
            // 
            this.设备信息base64.Location = new System.Drawing.Point(205, 467);
            this.设备信息base64.Multiline = true;
            this.设备信息base64.Name = "设备信息base64";
            this.设备信息base64.Size = new System.Drawing.Size(187, 42);
            this.设备信息base64.TabIndex = 1;
            // 
            // 设备信息base64的私钥签过的名
            // 
            this.设备信息base64的私钥签过的名.Location = new System.Drawing.Point(511, 467);
            this.设备信息base64的私钥签过的名.Multiline = true;
            this.设备信息base64的私钥签过的名.Name = "设备信息base64的私钥签过的名";
            this.设备信息base64的私钥签过的名.Size = new System.Drawing.Size(109, 42);
            this.设备信息base64的私钥签过的名.TabIndex = 1;
            // 
            // 设备签名信息文件名
            // 
            this.设备签名信息文件名.Location = new System.Drawing.Point(819, 467);
            this.设备签名信息文件名.Multiline = true;
            this.设备签名信息文件名.Name = "设备签名信息文件名";
            this.设备签名信息文件名.Size = new System.Drawing.Size(222, 42);
            this.设备签名信息文件名.TabIndex = 1;
            // 
            // 签名文件目标文件名的加密后byte
            // 
            this.签名文件目标文件名的加密后byte.Location = new System.Drawing.Point(1240, 467);
            this.签名文件目标文件名的加密后byte.Multiline = true;
            this.签名文件目标文件名的加密后byte.Name = "签名文件目标文件名的加密后byte";
            this.签名文件目标文件名的加密后byte.Size = new System.Drawing.Size(198, 42);
            this.签名文件目标文件名的加密后byte.TabIndex = 1;
            // 
            // 保存公钥资源文件和文件名文件
            // 
            this.保存公钥资源文件和文件名文件.Location = new System.Drawing.Point(1444, 419);
            this.保存公钥资源文件和文件名文件.Name = "保存公钥资源文件和文件名文件";
            this.保存公钥资源文件和文件名文件.Size = new System.Drawing.Size(124, 42);
            this.保存公钥资源文件和文件名文件.TabIndex = 0;
            this.保存公钥资源文件和文件名文件.Text = "10保存公钥资源文件和文件名文件";
            this.保存公钥资源文件和文件名文件.UseVisualStyleBackColor = true;
            this.保存公钥资源文件和文件名文件.Click += new System.EventHandler(this.保存公钥资源文件和文件名文件_Click);
            // 
            // 保存签名资源文件和文件名文件
            // 
            this.保存签名资源文件和文件名文件.Location = new System.Drawing.Point(1444, 467);
            this.保存签名资源文件和文件名文件.Name = "保存签名资源文件和文件名文件";
            this.保存签名资源文件和文件名文件.Size = new System.Drawing.Size(124, 42);
            this.保存签名资源文件和文件名文件.TabIndex = 0;
            this.保存签名资源文件和文件名文件.Text = "15保存签名资源文件和文件名文件";
            this.保存签名资源文件和文件名文件.UseVisualStyleBackColor = true;
            this.保存签名资源文件和文件名文件.Click += new System.EventHandler(this.保存签名资源文件和文件名文件_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 582);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "客户端使用部分测试↓";
            this.label3.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "注册机生成部分↓";
            // 
            // 完成btn
            // 
            this.完成btn.Location = new System.Drawing.Point(1240, 515);
            this.完成btn.Name = "完成btn";
            this.完成btn.Size = new System.Drawing.Size(328, 63);
            this.完成btn.TabIndex = 5;
            this.完成btn.Text = "完成";
            this.完成btn.UseVisualStyleBackColor = true;
            this.完成btn.Click += new System.EventHandler(this.完成btn_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1580, 590);
            this.Controls.Add(this.完成btn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.私钥);
            this.Controls.Add(this.签名文件目标文件名的加密后byte);
            this.Controls.Add(this.公钥的目标文件名加密后的byte数组);
            this.Controls.Add(this.设备签名信息文件名);
            this.Controls.Add(this.公钥的目标文件名);
            this.Controls.Add(this.设备信息base64);
            this.Controls.Add(this.设备信息base64的私钥签过的名);
            this.Controls.Add(this.加密后的公钥文件);
            this.Controls.Add(this.私钥保存文件位置);
            this.Controls.Add(this.私钥信息对象);
            this.Controls.Add(this.设备硬件信息);
            this.Controls.Add(this.设备sn);
            this.Controls.Add(this.公钥);
            this.Controls.Add(this.生成guid作为设备签名信息文件的文件名);
            this.Controls.Add(this.生成guid作为公钥加密文件的文件名);
            this.Controls.Add(this.使用私钥将这个base64签名);
            this.Controls.Add(this.将生成的签名文件名转换成加密的byte数组);
            this.Controls.Add(this.保存签名资源文件和文件名文件);
            this.Controls.Add(this.保存公钥资源文件和文件名文件);
            this.Controls.Add(this.将生成的公钥文件名转换成加密的byte数组);
            this.Controls.Add(this.把设备信息保存到json然后base64);
            this.Controls.Add(this.使用公钥文件加密器加密公钥文件);
            this.Controls.Add(this.提取公钥准备安装);
            this.Controls.Add(this.保存私钥信息到文件);
            this.Controls.Add(this.生成私钥信息对象);
            this.Controls.Add(this.获取设备的硬件信息);
            this.Controls.Add(this.填写设备的SerialNumber);
            this.Controls.Add(this.生成公钥私钥对);
            this.Name = "Main";
            this.Text = "Keygen";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button 生成公钥私钥对;
        private System.Windows.Forms.Button 填写设备的SerialNumber;
        private System.Windows.Forms.Button 获取设备的硬件信息;
        private System.Windows.Forms.Button 生成私钥信息对象;
        private System.Windows.Forms.Button 保存私钥信息到文件;
        private System.Windows.Forms.Button 提取公钥准备安装;
        private System.Windows.Forms.Button 使用公钥文件加密器加密公钥文件;
        private System.Windows.Forms.Button 把设备信息保存到json然后base64;
        private System.Windows.Forms.Button 使用私钥将这个base64签名;
        private System.Windows.Forms.Button 生成guid作为公钥加密文件的文件名;
        private System.Windows.Forms.Button 生成guid作为设备签名信息文件的文件名;
        private System.Windows.Forms.Button 将生成的公钥文件名转换成加密的byte数组;
        private System.Windows.Forms.Button 将生成的签名文件名转换成加密的byte数组;
        private System.Windows.Forms.TextBox 公钥;
        private System.Windows.Forms.TextBox 私钥;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox 设备sn;
        private System.Windows.Forms.TextBox 设备硬件信息;
        private System.Windows.Forms.TextBox 私钥信息对象;
        private System.Windows.Forms.TextBox 私钥保存文件位置;
        private System.Windows.Forms.TextBox 加密后的公钥文件;
        private System.Windows.Forms.TextBox 公钥的目标文件名;
        private System.Windows.Forms.TextBox 公钥的目标文件名加密后的byte数组;
        private System.Windows.Forms.TextBox 设备信息base64;
        private System.Windows.Forms.TextBox 设备信息base64的私钥签过的名;
        private System.Windows.Forms.TextBox 设备签名信息文件名;
        private System.Windows.Forms.TextBox 签名文件目标文件名的加密后byte;
        private System.Windows.Forms.Button 保存公钥资源文件和文件名文件;
        private System.Windows.Forms.Button 保存签名资源文件和文件名文件;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button 完成btn;
    }
}

