using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDM.Manager
{
    public class StartMedicinesGettingResult
    {
        /// <summary>
        /// 启动获取药品任务是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 启动取药任务如果没成功的话 这个notice是给用户看的提示
        /// </summary>
        public string Notice { get; set; }

        /// <summary>
        /// 启动取药任务的时候是否发生错误.如果发生错误报告HIS系统
        /// </summary>
        public bool IsError { get; set; }
        /// <summary>
        /// 启动药品任务如果发生错误,不提示错误信息,直接给HIS系统报告取药机错误信号
        /// </summary>
        public String ErrMsg { get; set; }

        /// <summary>
        /// 该订单本次取药产生的付药记录编号
        /// </summary>
        public long DeliveryRecordId { get; set; }
    }
}
