﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;
using Size = System.Drawing.Size;
using System.Runtime.InteropServices;

namespace 摄像头
{
    public partial class 调用摄像头2 : Form
    {
        public 调用摄像头2()
        {
            InitializeComponent();
        }

        VideoWork vw = null;

        private void 调用摄像头_Load(object sender, EventArgs e)
        {
            vw = new VideoWork(this.panel.Handle, 0, 0, panel.Width, panel.Height);
        }

        private void preViewBtn_Click(object sender, EventArgs e)
        {
            vw.Start();
        }
    }
}
