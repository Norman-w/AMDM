using System;
using System.Collections.Generic;
using System.Text;
/*
 * 2021年12月23日15:41:46 构建出了账户信息相关.为了使用到账户登录系统中.
 */
namespace AMDM_Domain
{
    /// <summary>
    /// 账户信息
    /// </summary>
    public class Account
    {
        long _id = 0;
        /// <summary>
        /// ID(流水号自增量)
        /// </summary>
        public long Id { get { return _id; } set { _id = value; } }

        String _department = null;
        /// <summary>
        /// 在医院中的所属科室
        /// </summary>
        public String Department { get { return _department; } set { _department = value; } }

        String _name = null;
        /// <summary>
        /// 姓名
        /// </summary>
        public String Name { get { return _name; } set { _name = value; } }

        /// <summary>
        /// 手机号,用于接收库存预警提醒
        /// </summary>
        public string Mobile { get; set; }

        String _sex = null;
        /// <summary>
        /// 性别
        /// </summary>
        public String Sex { get { return _sex; } set { _sex = value; } }

        Int32 _age = 0;
        /// <summary>
        /// 年龄
        /// </summary>
        public Int32 Age { get { return _age; } set { _age = value; } }

        String _username = null;
        /// <summary>
        /// 自定义指定的用户名
        /// </summary>
        public String UserName { get { return _username; } set { _username = value; } }

        String _password = null;
        /// <summary>
        /// 账号
        /// </summary>
        public String Password { get { return _password; } set { _password = value; } }

        String _passwordmd5 = null;
        /// <summary>
        /// 账号的md5值
        /// </summary>
        public String PasswordMD5 { get { return _passwordmd5; } set { _passwordmd5 = value; } }

        Nullable<DateTime> _createtime = null;
        /// <summary>
        /// 创建账户的时间
        /// </summary>
        public Nullable<DateTime> CreateTime { get { return _createtime; } set { _createtime = value; } }

        Nullable<DateTime> _modifiedtime = null;
        /// <summary>
        /// 修改信息的时间,或者是修改密码的时间
        /// </summary>
        public Nullable<DateTime> ModifiedTime { get { return _modifiedtime; } set { _modifiedtime = value; } }
    }
}
