using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AMDM
{
    /// <summary>
    /// 药品交付过程中,显示药品明细的行,也可以作为标题行,设置isTitle就可以了.
    /// </summary>
    public partial class MedicineOrderDetailShower : UserControl
    {
        public MedicineOrderDetailShower()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化 如果是标题 后面三个参数都不用
        /// </summary>
        /// <param name="isTitle"></param>
        /// <param name="pos"></param>
        /// <param name="name"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool Init(bool isTitle, int pos = 0, string name = null, int count = 0)
        {
            if (isTitle)
            {
                posLabel.Text = "序号";
                nameLabel.Text = "药品名称";
                countLabel.Text = "数量";
            }
            else
            {
                posLabel.Text = pos.ToString();
                nameLabel.Text = name;
                countLabel.Text = count.ToString();
            }
            return true;
        }
    }
}
