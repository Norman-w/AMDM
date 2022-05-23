using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AMDM.Manager
{
    /// <summary>
    /// 格子绑定的药品信息管理器
    /// </summary>
    public class GridMedicineBiddingManager
    {
        public GridMedicineBiddingManager(SQLDataTransmitter sqlClient)
        {
            this.client = sqlClient;
        }
        #region 数据交互器
        SQLDataTransmitter client = null;
        #endregion
        /// <summary>
        /// 绑定药品到格子
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="yxLocation"></param>
        /// <returns></returns>
        public bool BindMedicine2Grid(string barcode, Point yxLocation)
        {
            return false;
        }
        /// <summary>
        /// 绑定药品到格子
        /// </summary>
        /// <param name="medicine"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public bool BindMedicine2Grid(AMDM_Medicine medicine, AMDM_Grid grid)
        {
            AMDM_Clip_data binding = new AMDM_Clip_data();
            binding.BindingTime = DateTime.Now;
            //binding.CurrentInventory = 0;
            binding.FloorIndex = grid.FloorIndex;
            binding.GridIndex = grid.IndexOfFloor;
            binding.MedicineBarcode = medicine.Barcode;
            binding.MedicineId = medicine.Id;
            binding.MedicineName = medicine.Name;
            binding.StockIndex = grid.StockIndex;

            return client.AddBindingInfo(binding);
        }
        /// <summary>
        /// 获取药品绑定的格子
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public List<AMDM_Clip_data> GetBindedGridList(string barcode)
        {
            return null;
        }
        /// <summary>
        /// 根据药品id获取已经绑定了所有该药品的绑定信息
        /// </summary>
        /// <param name="medicineId"></param>
        /// <returns></returns>
        public List<AMDM_Clip_data> GetBindedGridList(Nullable<int> stockIndex, long medicineId)
        {
            return client.GetBindingsInfoList(stockIndex, medicineId);
        }
        /// <summary>
        /// 获取格子上绑定的药品
        /// </summary>
        /// <param name="yxLocation"></param>
        /// <returns></returns>
        public AMDM_Medicine GetBindedMedicine(Point yxLocation)
        {
            return null;
        }

        

        /// <summary>
        /// 获取药仓上的所有药品绑定信息
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public List<AMDM_Clip_data> GetStockAllBindedMedicine(AMDM_Stock stock)
        {
            return  client.GetBindingInfos4Stock<AMDM_Clip_data>(stock.IndexOfMachine);
        }

        /// <summary>
        /// 获取药仓上的所有药品绑定信息,同时获取他的实际装载的弹药的信息
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public List<AMDM_Clip> GetStockAllBindedMedicineWithMedicineObject(int stockIndex)
        {
            var list = client.GetBindingInfos4Stock<AMDM_Clip>(stockIndex);
            this.GetMedicinesObjectInfo(ref list);
            return list;
        }

        #region 获取弹夹内的子弹集合
        /// <summary>
        /// 获取弹夹内的子弹数量
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public int GetMedicinesObjectCount(AMDM_Grid grid)
        {
            return client.GetMedicinesObject(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor).Count;
        }
        /// <summary>
        /// 填充所有的GridMedicineBindingInfo里面装在的药品的实例信息
        /// </summary>
        /// <param name="bindInfos"></param>
        public void GetMedicinesObjectInfo(ref List<AMDM_Clip> bindInfos)
        {
            for (int i = 0; i < bindInfos.Count; i++)
            {
                var current = bindInfos[i];
                current.MedicineObjects = client.GetMedicinesObject(current.StockIndex, current.FloorIndex, current.GridIndex);
            }
        }
        public List<AMDM_MedicineObject_data> GetMedicinesObject(int stockIndex, int floorIndex, int gridIndex)
        {
            return client.GetMedicinesObject(stockIndex, floorIndex, gridIndex);
        }
        #endregion

        #region 解除药品和格子的绑定
        public bool UnBindMedicine(AMDM_Grid grid, AMDM_Medicine medicine)
        {
            return client.RemoveBindingInfo(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor);
        }
        public bool UnBindMedicine(int stockIndex, int floorIndex, int gridIndex)
        {
            return client.RemoveBindingInfo(stockIndex, floorIndex, gridIndex);
        }
        #endregion

        #region 获取绑定
        public AMDM_Clip_data GetBindingInfo(AMDM_Grid grid)
        {
            return client.GetBindingInfo(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor);
        }
        #endregion

        #region 药槽绑定信息目前设计为用来保存药品的库存信息 2021年11月6日20:05:47 修改药槽的内的药品数量 就直接改绑定的数量
        /// <summary>
        /// 添加或者减少药品的库存量信息,inOutCount字段输入负数就是减少
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <param name="floorIndex"></param>
        /// <param name="gridIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool InOutMedicineCount(AMDM_Grid_data grid,long medicineId,long inOutRecordId, Nullable<DateTime> expirationDate, int inOutCount, Nullable<int> destCount)
        {
            if (inOutCount == 0)
            {
                Utils.LogError("进出弹夹的药品数量必须为非0的整数");
                return false;
            }
            int countAbs = Math.Abs(inOutCount);
            #region 入库的时候
            if (inOutCount > 0)
            {
                for (int i = 0; i < countAbs; i++)
                {
                    //新建弹药,也就是同时装入了弹夹
                    AMDM_MedicineObject_data newMedicineObject = client.AddMedicineObject(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor, medicineId, DateTime.Now,
                       inOutRecordId, null, expirationDate);
                    if (newMedicineObject == null)
                    {
                        Utils.LogBug("InOutMedicineCount中 新建药品实体信息失败,总数和当前:", countAbs, i);
                        return false;
                    }
                }
                //执行到这里  已经把药品实体添加到弹夹了,重新获取数量
                var nowObjects = client.GetMedicinesObject(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor);
                if (destCount!= null)
                {
                    if (destCount.Value == nowObjects.Count)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //没有给定目标期望的是多少数量,可以不需要校验.
                }
                return true;
            }
            #endregion
            #region 出库的时候
            else
            {
               //先获取可以出库的子弹
                var canDeliveryMedicinesObject = client.GetMedicinesObject(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor);
                if (canDeliveryMedicinesObject == null || canDeliveryMedicinesObject.Count<countAbs)
                {
                    Utils.LogWarnning("药槽内可以取出的药品数量不足,期望的数量和实际的数量是:", countAbs, canDeliveryMedicinesObject == null? "null":canDeliveryMedicinesObject.Count.ToString());
                    return false;
                }
                for (int i = 0; i < countAbs; i++)
                {
                    var currentWaitDeliveryObject = canDeliveryMedicinesObject[i];
                    bool currentUpdDeliveryRet = client.UpdateMedicineObject(currentWaitDeliveryObject.Id, inOutRecordId, DateTime.Now);
                    if (currentUpdDeliveryRet == false)
                    {
                        Utils.LogBug("药品出槽时发生了数据更新错误,未能将目标药物实体更新,ID:", currentUpdDeliveryRet);
                        return false;
                    }
                }
                //执行到这里  已经把药品实体添加到弹夹了,重新获取数量
                var nowObjects = client.GetMedicinesObject(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor);
                if (destCount != null)
                {
                    if (destCount.Value == nowObjects.Count)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //没有给定目标期望的是多少数量,可以不需要校验.
                }
                return true;
            }
            #endregion

            #region 2022年1月16日14:12:58  之前的代码

            //bool updateRet = client.UpdateBindingInfoSavedInventoryInfoField(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor, inOutCount);
            //if (updateRet)
            //{
            //    //重新获取药品的数量
            //    Nullable<int> ret = client.GetBindingInfoSavedInventoryCount(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor);
            //    if (ret == null)
            //    {
            //        throw new NotImplementedException("修改库存信息成功,重新获取库存数量时发生错误");
            //    }
            //    if (destCount != null)
            //    {
            //        if (ret.Value != destCount)
            //        {
            //            throw new NotImplementedException("修改库存信息成功,但修改后的总数量与预期数量不同");
            //        }
            //    }
                
            //}
            //else
            //{
            //    return false;
            //}
            //return true;

            #endregion
        }
        /// <summary>
        /// 将一组药品设置为已出库
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public bool OutMedicineObjects(List<AMDM_MedicineObject_data> objs, long recordId)
        {
            if (objs == null || objs.Count < 1)
            {
                return false;
            }

            for (int i = 0; i < objs.Count; i++)
            {
                var currentWaitDeliveryObject = objs[i];
                bool currentUpdDeliveryRet = client.UpdateMedicineObject(currentWaitDeliveryObject.Id, recordId, DateTime.Now);
                if (currentUpdDeliveryRet == false)
                {
                    Utils.LogBug("药品出槽时发生了数据更新错误,未能将目标药物实体更新,ID:", currentUpdDeliveryRet);
                    continue;
                }
            }
            return true;

        }
        /// <summary>
        /// 清空药品的库存信息 ,通常只有在初始化和管理员强制操作的时候才会使用
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public bool ZeroMedicineCount(AMDM_Grid grid, long deliveryRecordId)
        {
            #region 2022年1月16日14:14:11
            //return client.UpdateBindingInfoSavedInventoryInfoFieldSetZero(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor);
            #endregion
            //获取原来的这个位置上的所有的子弹 把他们都打出去
            var objects = client.GetMedicinesObject(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor);
            if (objects.Count == 0)
            {
                Utils.LogWarnning("将药槽内药品数量置0时,检查到库存数量已经是0了");
                return true;
            }
            foreach (var item in objects)
            {
                bool currentRet = client.UpdateMedicineObject(item.Id, deliveryRecordId, DateTime.Now);
                if (currentRet == false)
                {
                    Utils.LogBug("将药槽内药品数量置0时发生单个药品的数据更新错误:", item);
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 设定弹夹为已卡药2022年2月16日14:19:20
        /// <summary>
        /// 设定弹夹已卡药,卡药的弹夹就不能再次付药了.需要强制清空药槽以后才可以
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <param name="floorIndex"></param>
        /// <param name="gridIndex"></param>
        /// <param name="stucked"></param>
        /// <returns></returns>
        public bool SetClipStucked(int stockIndex, int floorIndex, int gridIndex, bool stucked)
        {
            AMDM_Clip_data gf = client.GetBindingInfo(stockIndex, floorIndex, gridIndex);
            if (gf == null)
            {
                Utils.LogError("错误,设置弹夹已卡药错误,没有获取到这个弹夹", stockIndex, floorIndex, gridIndex);
                return false;
            }
            gf.Stuck = stucked;
            return client.UpdateBindingInfo(gf);
        }
        #endregion
    }
}
