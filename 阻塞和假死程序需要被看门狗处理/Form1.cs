using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 阻塞和假死程序需要被看门狗处理
{
    public partial class Form1 : Form
    {
        int distoryNeedCostMS = 0;
        public Form1()
        {
            InitializeComponent();

            /////////////测试上来就让关闭必须延迟30秒
            this.distoryNeedCostMS = 30000;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (distoryNeedCostMS >0)
            {
                Console.WriteLine("我释放需要30秒,请等我,不等我的话直接干掉我");
                System.Threading.Thread.Sleep(distoryNeedCostMS);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.distoryNeedCostMS = 30000;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            while(true)
            {
                System.Threading.Thread.Sleep(2000);
                Console.WriteLine("我正在卡死自己的线程里面,我仿佛只会发送这一句话了");
            }
        }
    }
}
