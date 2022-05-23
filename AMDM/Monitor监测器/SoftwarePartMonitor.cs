using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDM
{
    public class SoftwarePartMonitor
    {
        private OnMonnitorDetectedErrorEventHandler onError;
        public SoftwarePartMonitor(OnMonnitorDetectedErrorEventHandler onError)
        {
            this.onError = onError;
        }
        public void RefreshStatus()
        {
            bool checkSqlConnectionRet = false;
            bool checkLogServerConnectionRet = false;
            bool checkHISConnectorRet = false;
            #region 检查sql连接
            //string sqlVersion = null;
            try
            {
                List<string> versions = App.sqlClient.MysqlSelectValue<string>("select version()");
                if (versions!= null && versions.Count == 1)
                {
                    //sqlVersion = versions[0];
                    //Utils.LogSuccess("数据服务器版本:", sqlVersion);
                    checkSqlConnectionRet = true;
                }
            }
            catch(Exception err)
            {
                Utils.LogError("MYSQL数据库服务器连接失败", err.Message);
            }
            #endregion
            #region 检查看门狗连接
            try
            {
                if (App.LogServerServicePipeClient.IsConnected)
                {
                    checkLogServerConnectionRet = true;
                }
            }
            catch (Exception err)
            {
                Utils.LogError("检查看门狗连接发生错误:", err.Message);
            }
            #endregion
            #region 检查HIS系统连接
            try
            {
                if (App.HISServerConnector.CheckConnect())
                {
                    checkHISConnectorRet = true;
                }
            }
            catch (Exception err)
            {
                Utils.LogError("检查HIS系统连接错误:", err.Message);
            }
            #endregion
            if (onError != null)
            {
                if (!checkSqlConnectionRet)
                {
                    onError(this, AMDM_Domain.MonitorDetectedErrorTypeEnum.软件组件数据库异常);
                    return;
                }
                if (!checkLogServerConnectionRet && !System.Diagnostics.Debugger.IsAttached)
                {
                    onError(this, AMDM_Domain.MonitorDetectedErrorTypeEnum.软件组件看门狗异常);
                    return;
                }
                if(!checkHISConnectorRet)
                {
                    onError(this, MonitorDetectedErrorTypeEnum.软件组件HIS系统连接异常);
                    return;
                }
            }
        }
    }
}
