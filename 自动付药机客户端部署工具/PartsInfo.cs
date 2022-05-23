using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
 * 零部件的信息,包含id,名称,创建时间 有效期  当前状态等信息.
 */
namespace 自动付药机客户端部署工具
{
    public class PartsInfo
    {
        /// <summary>
        /// id,用于存储到数据库的唯一标志
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 编码,可以不同于id
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 序列号
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 部件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public Nullable<DateTime> CreateTime { get; set; }

        /// <summary>
        /// 截止日期,有效期.当零件是耗材或者是易损件的时候需要该信息
        /// </summary>
        public Nullable<DateTime> ExprationTime { get; set; }
    }
}
