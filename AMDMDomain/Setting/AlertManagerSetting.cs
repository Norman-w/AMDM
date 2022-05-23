using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain.Setting
{
    public class DurantionInfo
    {
        public Nullable<DateTime> Start { get; set; }
        public Nullable<DateTime> End { get; set; }
        /// <summary>
        /// 该设定中的日期信息是否为实际信息.如果是设置为实际信息的话,如果通过这个区间信息来控制执行的话将会只进行一次.如果不是实模式,那就是每天按照这个时间都会执行
        /// 如果不是实模式,半夜11:30开始执行的,结束时间是1:00这种的,首先看一下 结束时间是否大于开始时间,如果不是,错误的值.如果是,那么重复周期就设置为1天.实际还有更好的模式.
        /// 比如通过设定生成和预览有哪些时间点.
        /// 比如可以设定重复模式为  日  周   月  年 等等.
        /// </summary>
        public bool RealDay { get; set; }
    }
    public class AlertManagerSettingClass
    {
        /// <summary>
        /// 相同的硬件故障信息重新发送延迟时间毫秒
        /// </summary>
        public double SameHardwareMsgReSendDelayMS { get; set; }
        /// <summary>
        /// 相同的软件故障信息重新发送延迟时间毫秒
        /// </summary>
        public double SameSoftwarePartMsgReSendDelayMS { get; set; }
        /// <summary>
        /// 相同的低药品库存信息重发送延迟时间毫秒
        /// </summary>
        public double SameLowInventoryMsgReSendDelayMS { get; set; }
        /// <summary>
        /// 相同的药品有效期预警消息重新发送延迟时间毫秒
        /// </summary>
        public double SameExpirationAlertMsgReSendDelayMS { get; set; }
    }
}
