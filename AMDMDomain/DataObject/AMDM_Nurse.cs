using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 护士的信息表2021年11月26日09:42:24,用于登录和上药记录的相关信息记录
    /// </summary>
    public class AMDM_Nurse
    {
        Int32 _id = 0;
        /// <summary>
        /// 自动递增的id
        /// </summary>
        public Int32 Id { get { return _id; } set { _id = value; } }

        String _user = null;
        /// <summary>
        /// 用户名称
        /// </summary>
        public String User { get { return _user; } set { _user = value; } }

        String _pass = null;
        /// <summary>
        /// 用户密码,使用明文base64以后md5方式保存
        /// </summary>
        public String Pass { get { return _pass; } set { _pass = value; } }

        String _name = null;
        /// <summary>
        /// 姓名
        /// </summary>
        public String Name { get { return _name; } set { _name = value; } }

        Int32 _age = 0;
        /// <summary>
        /// 年龄
        /// </summary>
        public Int32 Age { get { return _age; } set { _age = value; } }

        String _sex = null;
        /// <summary>
        /// 性别
        /// </summary>
        public String Sex { get { return _sex; } set { _sex = value; } }

        String _mobile = null;
        /// <summary>
        /// 移动电话
        /// </summary>
        public String Mobile { get { return _mobile; } set { _mobile = value; } }

        DateTime _createtime = DateTime.Now;
        /// <summary>
        /// 创建角色的时间
        /// </summary>
        public DateTime CreateTime { get { return _createtime; } set { _createtime = value; } }

        DateTime _passlastmodifiedtime = DateTime.Now;
        /// <summary>
        /// 最后一次更改密码的时间
        /// </summary>
        public DateTime PassLastModifiedTime { get { return _passlastmodifiedtime; } set { _passlastmodifiedtime = value; } }
    }
}
