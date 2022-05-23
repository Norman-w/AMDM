using System;
using System.Collections.Generic;
using System.Text;

/*
 * 2021年12月18日18:12:59 由于需要通过外部http的方式 调取接口访问amdm的状态 比如温度 湿度  工作状态等.
 * 所以添加了这么一个类来表示机器状态相关的信息
 */
namespace AMDM_Domain
{
    /// <summary>
    /// 付药机的外设的当前状态
    /// </summary>
    public class AMDMPeripheralsStatus
    {
        ///// <summary>
        ///// 该状态属于哪个机器的,每个机器都有一个单独的sn,用这个字段来区分机器
        ///// </summary>
        //public string SN { get; set; }
        /// <summary>
        /// 紫外线灯是否正在工作,所有药仓只用8个紫外线灯,不需要单独的药仓单独控制紫外线灯
        /// </summary>
        public bool UVLampIsWorking { get; set; }


        /// <summary>
        /// 所有药仓的空调状态
        /// </summary>
        public List<WarehouseACStatus> WarehousesACStatus { get; set; }

        /// <summary>
        /// 获取当前状态的简明介绍字符串
        /// </summary>
        /// <returns></returns>
        public string GetDescriptionString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("紫外线灯状态:{0}\r\n温控:\r\n", this.UVLampIsWorking ? "开启" : "关闭");
            if (WarehousesACStatus!= null)
            {
                for (int i = 0; i < this.WarehousesACStatus.Count; i++)
                {
                    WarehouseACStatus current = this.WarehousesACStatus[i];
                    sb.AppendFormat("药仓{0}: 当前温度{1} 设定温度{2} 空调:{3}\r\n",
                        i + 1,
                        current.CurrentTemperature,
                        current.DestTemperature,
                        current.IsACWorking ? "开" : "关"
                        );
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// 仓库的空调,2021年12月19日11:31:07  查询了一下  stock更多的是表示库存 而不是仓库 所以更正一下
    /// </summary>
    public  class WarehouseACStatus
    {
        /// <summary>
        /// 药仓在机器中的索引位置,也就是stockindex
        /// </summary>
        public long WarehouseIndexId { get; set; }
        /// <summary>
        /// 目标温度是多少,只要设定一个值就可以,具体的是否在工作由下位机控制
        /// </summary>
        public float DestTemperature { get; set; }

        /// <summary>
        /// 当前温度是多少
        /// </summary>
        public float CurrentTemperature { get; set; }

        /// <summary>
        /// 空调是否正在工作
        /// </summary>
        public bool IsACWorking { get; set; }
    }

}
