using AMDM_Domain;
using DomainBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Server_SDK.Domain
{
    public class AccountAddResponse : NetResponse
    {
        public Account NewAccount { get; set; }
    }

    public class AccountAddRequest : BaseRequest<AccountAddResponse>
    {
        #region 字段
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

        String _sex = null;
        /// <summary>
        /// 性别
        /// </summary>
        public String Sex { get { return _sex; } set { _sex = value; } }

        Nullable<int> _age = 0;
        /// <summary>
        /// 年龄
        /// </summary>
        public Nullable<int> Age { get { return _age; } set { _age = value; } }

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
        #endregion
        public override string GetApiName()
        {
            return "account.add";
        }
        public override bool GetIsSessionRequired()
        {
            return true;
        }
        public override IDictionary<string, string> GetParameters()
        {
            return base.GetParametersDicByPublicFieldAuto(this);
        }
        public override void Validate()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password) || UserName.Trim().Length<1 || Password.Trim().Length<6)
            {
                throw new ArgumentNullException("需要指定有效的用户名和密码,密码不能小于6个字符");
            }
        }
    }
}
