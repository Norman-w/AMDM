using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDM.Manager
{
    #region 单个药品的取药任务类,包含所指向的格子和记录取药信息的对象
    public abstract class MedicineGettingTaskBase
    {
        public bool Started { get; set; }
        public bool Finished { get; set; }
        public bool IsError { get; set; }
        public StockMedicinesGettingErrorEnum ErrorType { get; set; }
        public string ErrMsg { get; set; }
        public AMDM_Grid_data Grid { get; set; }
    }
    /// <summary>
    /// 药品获取任务
    /// </summary>
    public class MedicineGettingSubTask : MedicineGettingTaskBase
    {
        public AMDM_DeliveryRecordDetail_data SubTask_RecordDetailRef { get; set; }
        public long SubTask_DeliveryRecordId { get; set; }
        public AMDM_MedicineObject_data MyProperty { get; set; }
    }
    /// <summary>
    /// 2022年3月2日14:10:56  由于台达的PLC是一个药槽发送一组任务 所以就记录组信息
    /// </summary>
    public class MedicinesGroupGettingTask : MedicineGettingTaskBase
    {
        public MedicinesGroupGettingTask()
        {
            this.SubTasks = new List<MedicineGettingSubTask>();
        }
        public AMDM_DeliveryRecordDetail_data RecordDetailRef { get; set; }
        public long DeliveryRecordId { get; set; }
        /// <summary>
        /// 2021年11月30日14:32:44 是否是清空药槽的任务,如果是清空药槽的任务的话,就不需要指定record,也不需要报送错误通知,只需要再300信号的时候 记录一下取到药品的数量即可.
        /// </summary>
        public bool IsClearGridTask { get; set; }

        /// <summary>
        /// 2022年3月1日16:26:04 新增的数量字段,因为新的PLC在发送数量的时候直接相同的药槽发送多个数量即可.不用每一个药品(同一个药槽)发送多次
        /// </summary>
        public int Count { get; set; }

        public List<AMDM_MedicineObject_data> MedicineObjects { get; set; }

        public List<MedicineGettingSubTask> SubTasks { get; set; }
    }
    #endregion
}
