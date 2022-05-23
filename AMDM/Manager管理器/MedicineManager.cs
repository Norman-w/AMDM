using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * 药品信息管理器,读取药品,拉取药品,保存药品等
 */
namespace AMDM.Manager
{
    public class MedicineManager
    {
        SQLDataTransmitter client = null;
        public MedicineManager(SQLDataTransmitter sqlClient)
        {
            this.client = sqlClient;
        }

        /// <summary>
        /// 根据药品的id列表获取药品信息
        /// </summary>
        /// <param name="medicinesIdList"></param>
        /// <returns></returns>
        public List<AMDM_Medicine> GetMedicines(List<long> medicinesIdList)
        {
            if (medicinesIdList.Count == 0)
            {
                return new List<AMDM_Medicine>();
            }
            return client.GetMedicinesByIdList(medicinesIdList);
        }
        /// <summary>
        /// 根据药品的条码获取药品
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public AMDM_Medicine GetMedicineByBarcode(string barcode)
        {
            return client.GetMedicineByBarcode(barcode);
        }
        /// <summary>
        /// 测试时使用,测试通过药品的index索引位置获取药品
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AMDM_Medicine TestGetMedicineByIndex(int index)
        {
            List<AMDM_Medicine> medicines =  client.GetMedicinesByIdList(new List<long> { index + 1 });
            if (medicines.Count == 1)
            {
                return medicines[0];
            }
            return null;
        }
        /// <summary>
        /// 测试获取地榆升白胶囊
        /// </summary>
        /// <returns></returns>
        public AMDM_Medicine TestGetMedicineDYS()
        {
            return client.GetMedicineByBarcode("6911502722656");
        }
        /// <summary>
        /// 从1-4000 的id号随便获取一个药品
        /// </summary>
        /// <returns></returns>
        public AMDM_Medicine GetRandomMedicine()
        {
            int id = new Random(Guid.NewGuid().GetHashCode()).Next(1, 4000);
            List<AMDM_Medicine> medicines = client.GetMedicinesByIdList(new List<long>() { id });
            if (medicines.Count == 1)
            {
                return medicines[0];
            }
            return null;
        }

        /// <summary>
        /// 重新初始化所有药品数据,会清除数据库中的所有药品信息,谨慎使用.
        /// 当从hisserver获取到数据以分页的方式返回的时候,也提供一个回调函数.
        /// </summary>
        /// <param name="medicines"></param>
        /// <returns></returns>
        public bool ReInitMedicinesInfo(IHISServerConnector hisServerConnector, 
            OnGetedMedicineInfoFromHisServerEventHandler onGetedMedicineInfoFromHisServer,
            Action<AMDM_Medicine, int, int> onAddedMedicine)
        {
            List<AMDM_Medicine> newAllMedicines = hisServerConnector.GetAllMedicines(onGetedMedicineInfoFromHisServer);
            client.RemoveAllMedicines();
            return client.AddMedicines(newAllMedicines, onAddedMedicine);
        }

        public bool UpdateMedicinesInfo(IHISServerConnector hisServerConnector, 
            DateTime start, DateTime end,
            OnGetedMedicineInfoFromHisServerEventHandler onGetedMedicineInfoFromHisServer,
            Action<AMDM_Medicine, int, int> onAddedMedicine)
        {
            List<AMDM_Medicine> newAllMedicines = hisServerConnector.GetModifiedMedicines(start,end,onGetedMedicineInfoFromHisServer);
            client.RemoveAllMedicines();
            return client.AddMedicines(newAllMedicines, onAddedMedicine);
        }


        #region 检查本机是否含有药品数据
        /// <summary>
        /// 检查本机是否含有药品数据
        /// </summary>
        /// <returns></returns>
        public long GetMedicinesKindCount()
        {
            return client.GetMedicinesKindCount(null,null);
        }
        #endregion
        #region 修改药盒尺寸信息
        /// <summary>
        /// 修改药盒的尺寸信息
        /// </summary>
        /// <param name="medicine"></param>
        /// <param name="longMM"></param>
        /// <param name="widthMM"></param>
        /// <param name="heightMM"></param>
        /// <returns></returns>
        public bool ResetMedicineBoxSize(ref AMDM_Medicine medicine, float longMM, float widthMM, float heightMM)
        {
            if (client.ResetMedicineSize(medicine.Id,longMM,widthMM,heightMM))
            {
                medicine.BoxLongMM = longMM;
                medicine.BoxWidthMM = widthMM;
                medicine.BoxHeightMM = heightMM;
                return true;
            }
            return false;
        }
        #endregion

        #region 转换远端的药品id列表到本地的id列表
        /// <summary>
        /// 转换his系统的药品编号id为本地的药品数据库中的药品id
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool ConvertHISMedicineId2LocalMedicineID(ref AMDM_MedicineOrder order)
        {
            List<string> hisMedicinesIdList = new List<string>();
            for (int i = 0; i < order.Details.Count; i++)
            {
                if (hisMedicinesIdList.Contains(string.Format("{0}", order.Details[i].MedicineId)) == false)
                {
                    hisMedicinesIdList.Add(string.Format("{0}", order.Details[i].MedicineId));
                }
            }
            Dictionary<string, long> medicinesIdsDic = client.GetMedicinesIdByHISMedicineIdsList(hisMedicinesIdList);
            if (medicinesIdsDic == null || medicinesIdsDic.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("转换his系统的药品id到本地系统的药品id错误");
                Console.ResetColor();
                return false;
            }
            //获取到了转换列表以后 修改record中的detail
            for (int i = 0; i < order.Details.Count; i++)
            {
                AMDM_MedicineOrderDetail current = order.Details[i];
                string hisMedicineID = string.Format("{0}", current.MedicineId);
                if (hisMedicinesIdList.Contains(hisMedicineID) == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("his系统需要的药品编号在本地药仓中没有找到对应的药品");
                    Console.ResetColor();
                    return false;
                }
            }
            //因为医生开药的时候可能开出来的有多行信息是一样的,但是实际取药过程中只需要一种一条记录就可以,这样好计算库存和计算最合适的格子.所以要把detail清空然后再重新的计算数量
            Dictionary<long, AMDM_MedicineOrderDetail> mergedDetails = new Dictionary<long, AMDM_MedicineOrderDetail>();
            for (int i = 0; i < order.Details.Count; i++)
            {
                AMDM_MedicineOrderDetail current = order.Details[i];
                if (current.Count == 0)
                {
                    continue;//如果当前的付药单的条目数量为0,就是不用取药
                }
                string hisMedicineID = string.Format("{0}", current.MedicineId);
                current.MedicineId = medicinesIdsDic[hisMedicineID];
                if (mergedDetails.ContainsKey(current.MedicineId) == false)
                {
                    mergedDetails.Add(current.MedicineId, current);
                }
                else
                {
                    mergedDetails[current.MedicineId].Count += current.Count;
                }
            }
            if (mergedDetails.Count != order.Details.Count && mergedDetails.Count!= 0)
            {
                order.Details.Clear();
                order.Details.AddRange(new List<AMDM_MedicineOrderDetail>(mergedDetails.Values));
            }
            if (mergedDetails.Count == 0 && order.Details.Count!= 0)
            {
                Utils.LogBug("转换his药品信息到本地药品信息时,发现有药品id一样的,合并完药品信息以后发生了错误,合并的药品的种类数量是0,但实际上付药单的药品条目数为", order.Details.Count);
            }
            return true;
        }
        #endregion

        #region 获取已经过期的药品
        /// <summary>
        /// 根据剩余时间信息获取药品(没有出库的),如果药品没有录入有效期信息,跳过不会返回,如果药品录入了有效期到什么时候,但是没有录入药品剩余多少天时提醒,使用设定中给定的默认剩余提醒天数
        /// </summary>
        /// <param name="defaultMinExpirationDays">如果药品没有保存有效期到达多少的时候进行提醒的信息,使用这个默认的天数来进行检测药品是否过期</param>
        /// <returns></returns>
        public List<AMDM_MedicineObject__Grid__Medicine> GetMedicineObjectsByExpirationLimit(int defaultMinExpirationDays)
        {
            return client.GetMedicineObjectsByExpirationLimit(defaultMinExpirationDays);
        }

        #endregion

        #region 获取低库存的商品
        public List<AMDM_MedicineInventory> GetLowInventoryMedicines(int defaultMinCountThredhold)
        {
            return client.GetLowInventoryMedicines(defaultMinCountThredhold);
        }
        #endregion
    }
}
