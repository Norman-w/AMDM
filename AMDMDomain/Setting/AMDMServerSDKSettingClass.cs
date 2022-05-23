using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 付药机系统服务器的sdk设置(调用场景基于sdk的使用方,也就是sdkClient的设置 2022年3月22日13:01:47 目前是使用sql数据直连的方式,但是实际上应该client和server之间连接使用web,但是web的鉴权太复杂了所以暂时写这样的
    /// </summary>
    public class AMDMServerSDKSettingClass : SqlConfigClass
    {
    }
}
