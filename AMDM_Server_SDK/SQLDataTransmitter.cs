using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AMDM_Server_SDK
{
    public class SQLDataTransmitter : MysqlClient
    {
        public SQLDataTransmitter(string ip, string user, string pass, string database, int port)
            :base(ip,user,pass,database,port)
        {

        }
        const string TableName_Account = "account";
        #region 添加账户信息
        string getPasswordMD5(string password)
        {
            string md5String = null;
            if (password != null)
            {
                //md5 = MD5.Create(password).ToString();
                byte[] result = Encoding.Default.GetBytes(password);    //tbPass为输入密码的文本框
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] output = md5.ComputeHash(result);
                md5String = BitConverter.ToString(output).Replace("-", "");  //tbMd5pass为输出加密文本的文本框
            }
            return md5String;
        }
        public Account AccountAdd(string department, string name, string sex, Nullable<int> age, string username, string password)
        {
            
            Account acc = new Account()
            {
                Age = age == null? 0: age.Value,
                CreateTime = DateTime.Now,
                Department = department,
                ModifiedTime = DateTime.Now,
                Name = name,
                Password = password,
                PasswordMD5 = getPasswordMD5(password),
                Sex = sex,
                UserName = username
            };
            SqlInsertRecordParamsV2<Account> pr = new SqlInsertRecordParamsV2<Account>(TableName_Account, acc, "*", "id", null, null);
            acc.Id = base.InsertDataV2(pr);
            if (acc.Id>0)
            {
                return acc;
            }
            Utils.LogError("向数据库中加入新的账户信息错误", department, name, sex, age, username, password);
            return null;
        }
        #endregion
        #region 获取账户信息
        public List<Account> AccountsGet(string fields, Nullable<long> id, string username, string passwordMD5, string name, string department, bool removePassInfo = true)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Account;
            
            bool hasUserField = false;
            if (string.IsNullOrEmpty(fields) == true)
            {
                Utils.LogError("AccountsGet:必须给定正确的字段信息");
                return null;
            }
            gpr.SetFields(fields);
            if (id!= null)
            {
                gpr.WhereEquals.Add("id", id);
                hasUserField = true;
            }
            
            if (string.IsNullOrEmpty(username) == false)
            {
                gpr.WhereEquals.Add("username", username);
                hasUserField = true;
            }
            if (string.IsNullOrEmpty(passwordMD5) == false && hasUserField)
            {
                gpr.WhereEquals.Add("passwordmd5", passwordMD5);
            }
            else if(string.IsNullOrEmpty(passwordMD5) == false && hasUserField == false)
            {
                string msg = "AccountsGet:如需校验密码,请先指定账户信息";
                //throw new ArgumentException(msg);
                Utils.LogError(msg);
                return null;
            }
            if (string.IsNullOrEmpty(name) == false)
            {
                gpr.WhereEquals.Add("name", name);
            }
            if (string.IsNullOrEmpty(department) == false)
            {
                gpr.WhereEquals.Add("department", department);
            }
            List<Account> ret = base.GetDatas<Account>(gpr);
            if (ret != null && removePassInfo)
            {
                foreach (var item in ret)
                {
                    item.Password = null;
                    item.PasswordMD5 = null;
                }
            }
            return ret;
        }
        #endregion

        #region 删除账户
        public bool AccountDelete(Nullable<long> id)
        {
            if (id == null)
            {
                return false;
            }
            SqlDeleteRecordParams dpr = new SqlDeleteRecordParams();
            dpr.TableName = TableName_Account;
            dpr.WhereEquals.Add("id", id);

            return base.DeleteData(dpr) > 0;
        }
        #endregion
        #region 更新账户
        public bool AccountUpdate(long id, string name, string username, string password, string department, string sex, Nullable<int> age, string mobile)
        {
            if (id <1)
            {
                Utils.LogError("AccountUpdate:无效的账户Id");
                return false;
            }
            if (name == null && username  == null && password == null && department == null && sex == null && age == null && mobile == null)
            {
                Utils.LogError("AccountUpdate:无效的将修改字段");
                return false;
            }
            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = TableName_Account;
            upr.WhereEquals.Add("id", id);
            if (name != null)
            {
                upr.UpdateFieldNameAndValues.Add("name", name);
            }
            if (username != null)
            {
                upr.UpdateFieldNameAndValues.Add("username", username);
            }
            if (password != null)
            {
                upr.UpdateFieldNameAndValues.Add("password", password);
                upr.UpdateFieldNameAndValues.Add("passwordmd5", getPasswordMD5(password));
            }
            if (department != null)
            {
                upr.UpdateFieldNameAndValues.Add("department", department);
            }
            if (sex != null)
            {
                upr.UpdateFieldNameAndValues.Add("sex", sex);
            }
            if (age != null)
            {
                upr.UpdateFieldNameAndValues.Add("age", age);
            }
            if(!string.IsNullOrEmpty(mobile))
            {
                upr.UpdateFieldNameAndValues.Add("mobile", mobile);
            }

            int updateCount = base.UpdateData(upr);
            if (updateCount >1)
            {
                Utils.LogBug("修改账户信息时,更新了大于1条的信息", updateCount);
            }
            return updateCount == 1;
        }
        #endregion
    }
}
