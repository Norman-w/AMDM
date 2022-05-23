using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    #region 控制面板组件设置
    /// <summary>
    /// 控制面板接口服务器的设置类
    /// </summary>
    public class ControlPanelInterfaceServerSettingClass
    {
        /// <summary>
        /// 开启一个控制面板的对外接口时,使用的httpserver的url是什么 要以/结尾
        /// </summary>
        public int HttpServerPort { get; set; }
    }
    #endregion
}
