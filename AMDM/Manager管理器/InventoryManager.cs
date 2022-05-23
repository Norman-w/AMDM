using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * 药品的库存信息管理器,用于获取库存信息,增加出库记录
 * 
 */
namespace AMDM.Manager
{
    public class InventoryManager
    {
        #region 全局变量
        SQLDataTransmitter client = null;
        #endregion
        #region 构造函数
        public InventoryManager(SQLDataTransmitter sqlClient)
        {
            this.client = sqlClient;
        }
        #endregion
        #region 强制设置库存的数量为多少:
        /// <summary>
        /// 强制将药品的数量设置为多少
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="medicine"></param>
        /// <param name="bindingInfo">绑定信息的指针,如果设置完了库存以后,更新了库存之后把这个bindinginfo内的库存信息也变更一下</param>
        /// <param name="wantInventory"></param>
        /// <returns></returns>
        public bool ForceResetInventory(AMDM_Grid grid,AMDM_Medicine medicine,string actionRecordId, int currentInventory, int wantInventory)
        {
            //差了几个(正整数形式)
            int howMany = Math.Abs((Math.Abs(currentInventory) - Math.Abs(wantInventory)));
            #region 添加库存记录
            //是不是将要设置的数量比实际的数量多
            bool isMinus = wantInventory < currentInventory;
            #region 如果是相对于实际数量有减少,就增加一个出库单
            if (isMinus)
            {
                DateTime now = DateTime.Now;
                AMDM_DeliveryRecord record = new AMDM_DeliveryRecord()
                {
                    Canceled = false,
                    Details = new List<AMDM_DeliveryRecordDetail_data>(),
                    EndTime = now,
                    Finished = true,
                    Memo = "矫正库存时添加出库记录",
                    StartTime = now,
                    TotalKindCount = 1,
                    TotalMedicineCount = howMany,
                    PrescriptionId = actionRecordId
                };
                #region 插入记录出库单
                if (client.InsertDeliveryRecord(record) == false)
                {
                    Console.WriteLine("强制设置药品的库存时,插入出库记录单失败,尚未插入详单数据");
                    return false;
                }
                #endregion
                AMDM_DeliveryRecordDetail_data detail = new AMDM_DeliveryRecordDetail_data()
                {
                    ParentId = record.Id,
                    Count = howMany,
                    EndTime = now,
                    FloorIndex = grid.FloorIndex,
                    GridIndex = grid.IndexOfFloor,
                    MedicineBarcode = medicine.Barcode,
                    MedicineId = medicine.Id,
                    MedicineName = medicine.Name,
                    StartTime = now,
                    StockIndex = grid.StockIndex
                };
                #region 插入出库记录明细
                if (client.InsertDeliveryRecordDetail(detail) == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("强制设置药品的库存时,插入出库记录的明细失败");
                    Console.ResetColor();
                    return false;
                }
                #endregion

                #region 把已经装载的实体药品发送出去
                //获取已经装载的实体药品
                var medicinesObject = client.GetCanDeliveryMedicinesObject(medicine.Id,
                    GetMedicinesObjectSortModeEnum.ObjectIdAsc,
                    howMany
                    );
                foreach (var m in medicinesObject)
                {
                   bool currentMedicineObjectUpdRet = client.UpdateMedicineObject(m.Id, record.Id, DateTime.Now);
                   if (currentMedicineObjectUpdRet)
                   {
                       Utils.LogSuccess("已装载的实体药品已强制设置为已出仓");
                   }
                   else
                   {
                       Utils.LogError("已装载的实体药品,在执行强制设置为已出仓时发生错误");
                   }
                }
                #endregion

                //加入到实体当中.
                record.Details.Add(detail);
            }
            #endregion
            #region 如果是相对于实际数量有增加,就增加一个入库单
            else
            {
                DateTime now = DateTime.Now;
                AMDM_InstockRecord record = new AMDM_InstockRecord()
                {
                    Canceled = false,
                    CreateTime = now,
                    Details = new List<AMDM_InstockRecordDetail>(),
                    EntriesCount = 1,
                    FinishTime = now,
                    Memo = actionRecordId+"矫正库存,实际药槽内库存大于数据存储的库存",
                    StockId = grid.StockId,
                    TotalMedicineCount = wantInventory - currentInventory,
                    Type = "库存矫正", 
                };
                #region 插入记录
                if (client.InsertInstockRecord(record) == false)
                {
                    Console.WriteLine("强制设置药品的库存时,插入入库记录失败,尚未插入入库详单数据");
                    return false;
                }
                #endregion
                AMDM_InstockRecordDetail detail = new AMDM_InstockRecordDetail()
                {
                    ParentId = record.Id,
                    Count = wantInventory - currentInventory,
                    MedicineBarcode = medicine.Barcode,
                    MedicineId = medicine.Id,
                    MedicineName = medicine.Name,
                     InstockTime = now, StockIndex = grid.StockIndex, FloorIndex = grid.FloorIndex,  GridIndex = grid.IndexOfFloor, Index = 0, Memo = "矫正库存,药机内实际库存不足,增加入库记录"
                };


                if (client.InsertInstockRecordDetail(detail) == false)
                {
                    Console.WriteLine("强制设置药品的库存时,插入出库记录的明细失败");
                    return false;
                }
                #region 把新增加的药品实体添加到数据库
                for (int i = 0; i < howMany; i++)
                {
                    var newObj = client.AddMedicineObject(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor, medicine.Id, DateTime.Now, record.Id, null, null);
                    if (newObj!= null)
                    {
                        Utils.LogSuccess("调整库存时新增药品实体完成");
                    }
                    else
                    {
                        Utils.LogError("调整库存时新增药品实体失败!!");
                    }
                }
                #endregion

                //加入到实体当中.
                record.Details.Add(detail);
            }
            #endregion
            #endregion
            #region 修改记录的库存数字
            //2022年1月16日17:09:37 从此刻起已经不使用绑定信息表中的库存信息了
            //if (client.UpdateBindingInfoSavedInventoryInfoField(grid.StockIndex, grid.FloorIndex, grid.IndexOfFloor, 
            //    //isMinus?( wantInventory- currentInventory) : (wantInventory - currentInventory)
            //    wantInventory - currentInventory
            //    ) == false)
            //{
            //    Console.WriteLine("增加药品出库记录完成,更新绑定信息中记录的库存信息失败");
            //    return false;
            //}
            #endregion
            //返回处理结果
            return true;
        }
        #endregion
        #region 读取本地有没有对某个处方进行付药
        /// <summary>
        /// 根据处方编号,获取本机保存的交付记录中有没有给这个处方进行取药操作
        /// </summary>
        /// <param name="prescriptionId">处方编号</param>
        /// <param name="checkDetails">是否检查明细信息,如果明细信息中的取药数量不是0的话,就提示已经取过了.不过这样好像不合理??? 后来改成了 根据 IsError字段获取</param>
        /// <returns></returns>
        public bool GetIsDeliveriedByPrescriptionId(string prescriptionId, bool checkDetails = false)
        {
            if (App.DebugCommandServer.Setting.IgnoreDeliveriedChecking == true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("注意,当前为调试状态,付药单完结状态强制不进行校验");
                Console.WriteLine("注意,当前为调试状态,付药单完结状态强制不进行校验");
                Console.WriteLine("注意,当前为调试状态,付药单完结状态强制不进行校验");
                Console.ResetColor();
                return false;
            }

            List<AMDM_DeliveryRecord_data> datas = client.GetDeliveryRecordByPrescriptionId(prescriptionId);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("当前处方获取到的交付记录数量:{0}", datas.Count);
            Console.ResetColor();
            if (datas!= null  && datas.Count >0)
            {
                bool deliveried = false;
                bool hasErrRecordDetail = false;
                for (int i = 0; i < datas.Count; i++)
                {
                    AMDM_DeliveryRecord_data current = datas[i];
                    if (current.Finished)
                    {
                        //Console.ForegroundColor = ConsoleColor.Red;
                        //Console.WriteLine("当前处方有已经完成的付药记录不能重复付药");
                        //Console.ResetColor();
                        //return false;
                        deliveried = true;
                        break;
                    }
                    if (checkDetails)
                    {
                        List<AMDM_DeliveryRecordDetail_data> details = client.GetDeliveryRecordDetails(current.Id);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("当前付药记录获取到的明细数量:{0}", details.Count);
                        for (int j = 0; j < details.Count; j++)
                        {
                            AMDM_DeliveryRecordDetail_data detail = details[j];
                            if (detail.IsError)
                            {
                                hasErrRecordDetail = true;
                            }
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("当前付药记录明细中付药的数量:{0}", detail.Count);
                            Console.ResetColor();
                            if (detail.Count > 0)
                            {
                                deliveried = true;
                                break;
                            }
                        }
                    }
                    Console.ResetColor();
                }
                if (hasErrRecordDetail)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("当前处方有发生过错误的付药记录");
                    Console.ResetColor();
                }
                if (deliveried)
                {
                    return true;
                }
                //每一次取药都没完成  那就是没有完成
                else
                {
                    return false;
                }
            }
            return false;
        }
        #region 2022年1月16日18:45:36 作废的检验药品是否可以交付的方法
        ///// <summary>
        ///// 获取付药单上的药品信息是否可以交付
        ///// </summary>
        ///// <param name="medicinesIdList"></param>
        ///// <returns></returns>
        //public bool GetMedicinesInOrderCanDelivery(AMDM_MedicineOrder order)
        //{
        //    List<long> medicinesIdList = new List<long>();
        //    Dictionary<long, int> medicinesNeedCount = new Dictionary<long, int>();
        //    //先获取到药品都在哪一个格子中,直接获取到库存信息 看能不能交付
        //    for (int i = 0; i < order.Details.Count; i++)
        //    {
        //        var detail = order.Details[i];
        //        if (medicinesIdList.Contains(detail.MedicineId) == false)
        //        {
        //            medicinesIdList.Add(detail.MedicineId);
        //            medicinesNeedCount.Add(detail.MedicineId, 0);
        //        }

        //        medicinesNeedCount[detail.MedicineId] += detail.Count;
        //    }
        //    Console.WriteLine("付药单中所需的药品信息:\r\n{0}", Newtonsoft.Json.JsonConvert.SerializeObject(order.Details, Newtonsoft.Json.Formatting.Indented));
        //    #region 获取这些药品所在的格子的绑定信息
        //    List<GridMedicineBindingInfo_data> bindings = client.GetBindingsInfoList(medicinesIdList);
        //    if (bindings == null || bindings.Count == 0)
        //    {
        //        Console.WriteLine("根据药单没有找到可交付的药品信息");
        //        return false;
        //    }
        //    #endregion
        //    //可以交付的药品的id
        //    List<long> cantFulfillMedicinesIds = new List<long>();
        //    //库存中药品id对应的药品数量信息
        //    Dictionary<long, int> inventoryMedicinesCountDic = new Dictionary<long, int>();
        //    //根据存储的不同的格子的库存,计算每种药品的总共库存数量
        //    for (int i = 0; i < bindings.Count; i++)
        //    {
        //        GridMedicineBindingInfo_data currentBinding = bindings[i];
        //        if (inventoryMedicinesCountDic.ContainsKey(currentBinding.MedicineId) == false)
        //        {
        //            inventoryMedicinesCountDic.Add(currentBinding.MedicineId, 0);
        //        }
        //        inventoryMedicinesCountDic[currentBinding.MedicineId] += currentBinding.CurrentInventory;
        //    }
        //    Console.WriteLine("当前药仓中所需药品的数量信息:\r\n{0}", Newtonsoft.Json.JsonConvert.SerializeObject(inventoryMedicinesCountDic, Newtonsoft.Json.Formatting.Indented));
        //    //根据需要的库存,对比当前的库存能不能满足所需要的药品
        //    foreach (var item in medicinesNeedCount)
        //    {
        //        long medicineId = item.Key;
        //        long needCount = item.Value;
        //        if (inventoryMedicinesCountDic.ContainsKey(medicineId))
        //        {
        //            if (inventoryMedicinesCountDic[medicineId] >= needCount)
        //            {
        //                Console.ForegroundColor = ConsoleColor.DarkGreen;
        //                Console.WriteLine("药品{0}可以付药,现有库存{1}, 所需库存 {2}", medicineId, inventoryMedicinesCountDic[medicineId], needCount);
        //                Console.ResetColor();
        //                continue;
        //            }
        //            else
        //            {
        //                Console.ForegroundColor = ConsoleColor.Yellow;
        //                Console.WriteLine("药品{0}库存不够不能付药,现有库存{1}, 所需库存 {2}", medicineId, inventoryMedicinesCountDic[medicineId], needCount);
        //                Console.ResetColor();
        //                cantFulfillMedicinesIds.Add(medicineId);
        //            }
        //        }
        //        else
        //        {
        //            Console.ForegroundColor = ConsoleColor.Red;
        //            Console.WriteLine(string.Format("获取付药单上的药品信息是否可以交付\r\n没有找到药品编号:{0}的库存信息", medicineId));
        //            Console.ResetColor();
        //            return false;
        //        }
        //    }
        //    return cantFulfillMedicinesIds.Count == 0;
        //}
        #endregion
        /// <summary>
        /// 获取付药单上的药品信息是否可以交付
        /// </summary>
        /// <param name="medicinesIdList"></param>
        /// <returns></returns>
        public bool GetMedicinesInOrderCanDelivery(AMDM_MedicineOrder order)
        {
            List<long> medicinesIdList = new List<long>();
            Dictionary<long, int> medicinesNeedCount = new Dictionary<long, int>();
            //先获取到药品都在哪一个格子中,直接获取到库存信息 看能不能交付
            for (int i = 0; i < order.Details.Count; i++)
            {
                var detail = order.Details[i];
                if (medicinesIdList.Contains(detail.MedicineId) == false)
                {
                    medicinesIdList.Add(detail.MedicineId);
                    medicinesNeedCount.Add(detail.MedicineId, 0);
                }

                medicinesNeedCount[detail.MedicineId] += detail.Count;
            }
            //根据需要的库存,对比当前的库存能不能满足所需要的药品
            foreach (var item in medicinesNeedCount)
            {
                long medicineId = item.Key;
                int needCount = item.Value;
                var mode = App.Setting.ExpirationStrictControlSetting.Enable ? GetMedicinesObjectSortModeEnum.ExpirationDateAsc : GetMedicinesObjectSortModeEnum.ObjectIdAsc;
                var objs = client.GetCanDeliveryMedicinesObject(medicineId,mode, needCount);
                if (objs.Count<needCount)
                {
                    Utils.LogWarnning("需要的药品没有指定的数量 药品id, 需要数量, 现有数量 分别是", medicineId, needCount, objs.Count);
                    return false;
                }
            }
            return true;
        }
        #endregion
        #region 获取能取某药品的格子,按照上次出库时间排序 找上次出库时间最早的排在前面
        #region 2022年1月16日17:18:53  之前的
        ///// <summary>
        ///// 获取能取某药品的格子,按照上次出库时间排序 找上次出库时间最早的排在前面,key是格子,value是格子的数量
        ///// </summary>
        ///// <param name="medicineId"></param>
        ///// <param name="needCount"></param>
        ///// <returns></returns>
        //public Dictionary<AMDM_Grid_data, int> GetBestGrids(long medicineId, int needCount)
        //{
        //    Dictionary<AMDM_Grid_data, int> grids = new Dictionary<AMDM_Grid_data, int>();
        //    //先获取药品所绑定了的格子
        //    List<GridMedicineBindingInfo_data> bindings = client.GetFulfillAbleGridBindingsOrderByInstockTimeAsc(medicineId);
        //    //再获取这些格子的最后上药时间.
        //    int canFulfillCount = 0;
        //    //如果单条记录够用的话,就不获取下一条记录了.
        //    for (int i = 0; i < bindings.Count; i++)
        //    {
        //        GridMedicineBindingInfo_data binding = bindings[i];
        //        canFulfillCount += binding.CurrentInventory;
        //        Console.ForegroundColor = ConsoleColor.Yellow;
        //        Console.WriteLine("获取到的绑定信息是:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(binding, Newtonsoft.Json.Formatting.Indented));
        //        Console.ResetColor();
        //        AMDM_Grid_data grid = client.GetGrid(binding.StockIndex, binding.FloorIndex, binding.GridIndex);
        //        Console.WriteLine("获取到的格子是:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(grid, Newtonsoft.Json.Formatting.Indented));
        //        grids.Add(grid, binding.CurrentInventory);
        //        if (canFulfillCount >= needCount)
        //        {
        //            break;
        //        }
        //    }
        //    return grids;
        //}
        #endregion
        /// <summary>
        /// 获取能取某药品的格子,按照上次出库时间排序 找上次出库时间最早的排在前面,key是格子,value是格子上可以取的药品的数量
        /// </summary>
        /// <param name="medicineId"></param>
        /// <param name="needCount"></param>
        /// <returns></returns>
        public Dictionary<AMDM_Grid_data, List<AMDM_MedicineObject_data>> GetBestGrids(long medicineId,GetMedicinesObjectSortModeEnum sortMode, int needCount)
        {
            Dictionary<AMDM_Grid_data, List<AMDM_MedicineObject_data>> grids = new Dictionary<AMDM_Grid_data, List<AMDM_MedicineObject_data>>();
            var objs = client.GetCanDeliveryMedicinesObject(medicineId,
                sortMode,
                needCount);
            Dictionary<string, List<AMDM_MedicineObject_data>> gridsIndexAndCount = new Dictionary<string, List<AMDM_MedicineObject_data>>();
            Dictionary<string, AMDM_Grid_data> gridsDic = new Dictionary<string, AMDM_Grid_data>();
            foreach (var item in objs)
            {
                string key = string.Format("{0}->{1}->{2}", item.StockIndex, item.FloorIndex, item.GridIndex);
                if (gridsIndexAndCount.ContainsKey(key) == false)
                {
                    gridsIndexAndCount.Add(key, new List<AMDM_MedicineObject_data>());
                    gridsDic.Add(key, client.GetGrid(item.StockIndex, item.FloorIndex, item.GridIndex));
                }
                gridsIndexAndCount[key].Add(item);
            }


            foreach (var item in gridsIndexAndCount)
            {
                var key = item.Key;
                var grid = gridsDic[key];
                var count = item.Value;

                grids.Add(grid, count);
            }

            return grids;
        }
        #endregion
        #region 一步一步的创建付药单,添加付药单记录,完结付药单
        /// <summary>
        /// 创建一个空的付药单记录,插入数据后返回对象
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>
        public AMDM_DeliveryRecord CreateDeliveryRecord(string prescriptionId)
        {
            AMDM_DeliveryRecord record = new AMDM_DeliveryRecord()
            {
                Canceled = false,
                Details = new List<AMDM_DeliveryRecordDetail_data>(),
                PrescriptionId = prescriptionId,
                StartTime = DateTime.Now,
                EndTime = null,
            };
            if (client.InsertDeliveryRecord(record))
            {
                return record;
            }
            return null;
        }
        /// <summary>
        /// 添加开始出库药品的记录详情,注意不记录结束时间.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="medicineId"></param>
        /// <param name="medicineName"></param>
        /// <param name="medicineBarcode"></param>
        /// <param name="count"></param>
        /// <param name="isErr"></param>
        /// <param name="errmsg"></param>
        /// <param name="stockIndex"></param>
        /// <param name="floorIndex"></param>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        public AMDM_DeliveryRecordDetail_data StartDeliveryOneMedicine(ref AMDM_DeliveryRecord record, long medicineId, string medicineName, string medicineBarcode, int count, int stockIndex, int floorIndex, int gridIndex)
        {
            AMDM_DeliveryRecordDetail_data detail = new AMDM_DeliveryRecordDetail_data()
            {
                Count = count,
                //EndTime = DateTime.Now,
                //ErrMsg = errmsg,
                FloorIndex = floorIndex,
                GridIndex = gridIndex,
                //IsError = isErr,
                MedicineBarcode = medicineBarcode,
                MedicineId = medicineId,
                MedicineName = medicineName,
                ParentId = record.Id,
                StartTime = DateTime.Now,
                StockIndex = stockIndex
            };
            //记录到数据库后添加到record中
            if(client.InsertDeliveryRecordDetail(detail))
            {
                record.Details.Add(detail);
                record.TotalKindCount = record.Details.Count;
                record.TotalMedicineCount += detail.Count;
                return detail;
            }
            else
            {
                Console.WriteLine("向数据库中插入付药明细失败");
                return null;
            }
        }
        /// <summary>
        /// 完结一个药品的出库记录,赋值完成时间和错误信息
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="isError"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool EndDeliveryOneMedicine(AMDM_DeliveryRecordDetail_data detailRef,
            //long medicineObjectId,  
            bool isError, string errMsg)
        {
            DateTime now = DateTime.Now;
            //记录到数据库中以后 赋值detail的信息
            if (detailRef!= null)
            {
                if (client.FinishedDeliveryRecordDetail(detailRef.Id, isError, errMsg, now))
                {
                    detailRef.IsError = isError;
                    detailRef.ErrMsg = errMsg;
                    detailRef.EndTime = now;
                    //detailRef.MedicineObjectId = medicineObjectId;
                    return true;
                }
                else
                {
                    Console.WriteLine("记录完结单次药品出库失败");
                    return false;
                }
            }
            else
            {
                //清空药槽操作的时候
                return true;
            }
        }

        /// <summary>
        /// 向数据库通知完成付药单,保存数据库完成后,把数据赋值给record的指针
        /// </summary>
        /// <param name="record"></param>
        /// <param name="finished"></param>
        /// <param name="canceled"></param>
        /// <param name="snapshotImageFilePath"></param>
        /// <returns></returns>
        public bool FinishDeliveryRecord(ref AMDM_DeliveryRecord record, bool finished, bool canceled, string memo)
        {
            DateTime now = DateTime.Now;
            bool updSqlRet = client.FinishedDeliveryRecord(record.Id,record.TotalKindCount, record.TotalMedicineCount, finished,canceled, now, memo);
            if (updSqlRet)
            {
                record.Finished = finished;
                record.Canceled = canceled;
                record.EndTime = now;
                //record.SnapshotImageFile = snapshotImageFilePath;
                return true;
            }
            else
            {
                Console.WriteLine("更新付药单记录主单据失败");
                return false;
            }
        }
        /// <summary>
        /// 更新付药单的截图信息
        /// </summary>
        /// <param name="record"></param>
        /// <param name="snapshotImageFilePath"></param>
        /// <returns></returns>
        public bool FinishedDeliveryRecordSnapshotCapture(ref AMDM_DeliveryRecord record, string snapshotImageFilePath)
        {
            bool updSqlRet = client.FinishedDeliveryRecordSnapshotCapture(record.Id, snapshotImageFilePath);
            if (updSqlRet)
            {
                record.SnapshotImageFile = snapshotImageFilePath;
                return true;
            }
            else
            {
                Console.WriteLine("更新付药单记录主单据的图像路径信息失败");
                return false;
            }
        }
        #endregion
        #region 获取完整的付药单记录
        /// <summary>
        /// 获取完整的付药单记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AMDM_DeliveryRecord GetDeliveryRecordFull(long id)
        {
            AMDM_DeliveryRecord_data data = client.GetDeliveryRecordById(id);
            AMDM_DeliveryRecord record = new AMDM_DeliveryRecord();
            if (data == null)
            {
                return null;
            }
            Newtonsoft.Json.JsonConvert.PopulateObject(Newtonsoft.Json.JsonConvert.SerializeObject(data), record);
            record.Details = client.GetDeliveryRecordDetails(data.Id);
            data = null;
            if (record.Details == null || record.Details.Count == 0)
            {
                Utils.LogWarnning("获取到付药单,没有获取到付药单内的明细数据", record.Id);
                return null;
            }
            return record;
        }
        #endregion
        #region 一步一步的创建上药单, 添加上药记录,完结上药单
        public AMDM_InstockRecord CreateInstockRecord(int machineId, int stockId, long nurseId, string type)
        {
            DateTime now = DateTime.Now;
            AMDM_InstockRecord record = new AMDM_InstockRecord()
            {
                Canceled = false,
                CreateTime = now,
                Details = new List<AMDM_InstockRecordDetail>(),
                StockId = stockId, NurseID= nurseId, MachineId = machineId,
                Type = type,
            };

            if (client.InsertInstockRecord(record)== false)
            {
                return null;
            }
            return record;
        }
        public AMDM_InstockRecordDetail AddInstockDetail(ref AMDM_InstockRecord record, int stockIndex, int floorIndex, int gridIndex, long medicineId, string medicineName, string medicineBarcode, int count)
        {
            AMDM_InstockRecordDetail detail = new AMDM_InstockRecordDetail()
            {
                 Count = count, FloorIndex = floorIndex, GridIndex = gridIndex, StockIndex = stockIndex, ParentId = record.Id, MedicineName = medicineName, MedicineId = medicineId, MedicineBarcode = medicineBarcode, 
                  InstockTime = DateTime.Now, Index = record.Details.Count
            };
            if (client.InsertInstockRecordDetail(detail) == false)
            {
                return null;
            }
            record.Details.Add(detail);
            record.TotalMedicineCount += detail.Count;
            record.EntriesCount = record.Details.Count;
            return detail;
        }

        public bool FinishInstockRecord(ref AMDM_InstockRecord record, bool canceled)
        {
            DateTime now = DateTime.Now;
            bool updSqlRet = client.FinishedInstockRecord(record.Id, record.EntriesCount, record.TotalMedicineCount, canceled,now, null);
            if (updSqlRet)
            {
                record.FinishTime = now;
                record.Canceled = canceled;
                //record.SnapshotImageFile = snapshotImageFilePath;
                return true;
            }
            else
            {
                Console.WriteLine("更新付药单记录主单据失败");
                return false;
            }
        }
        #endregion
    }
}
