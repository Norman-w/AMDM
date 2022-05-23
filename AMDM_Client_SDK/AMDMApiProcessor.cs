using AMDM.Manager;
using ServerBase;
using System;
using System.Collections.Generic;
using System.Web;
using System.Diagnostics;
using AMDMClientSDK.Domain;
using System.Configuration;
using AMDM_Domain;

public class AMDMApiProcessor
{
    #region 全局client
    SQLDataTransmitter client;
    /// <summary>
    /// 保存截图文件的地址是哪里,本地地址,后面以/结尾
    /// </summary>
    string client_side_sdk_snapshot_local_path = null;
    /// <summary>
    /// 保存截图文件的相对地址是哪里,url相对地址,也就是 比如a.com域名下的  /b/c/虚拟目录下   这个参数就是  /b/c
    /// </summary>
    string client_side_sdk_snapshot_url_path = null;
    #endregion
    public AMDMApiProcessor()
    {
        

        //注意这里值能使用主程序集中的appSettings 也就是如果用web调用,那就需要再web中指定这些设置.
        string ip = ConfigurationManager.AppSettings["client_side_sdk_ip"];
        string user = ConfigurationManager.AppSettings["client_side_sdk_user"];
        string pass = ConfigurationManager.AppSettings["client_side_sdk_pass"];
        string database = ConfigurationManager.AppSettings["client_side_sdk_database"];
        string port_str = ConfigurationManager.AppSettings["client_side_sdk_port"];
        this.client_side_sdk_snapshot_local_path = ConfigurationManager.AppSettings["client_side_sdk_snapshot_local_path"];
        this.client_side_sdk_snapshot_url_path = ConfigurationManager.AppSettings["client_side_sdk_snapshot_url_path"];
        //client = new SQLDataTransmitter(ip, "root", "woshinidie", "amdm_local", 10000);
        client = new SQLDataTransmitter(ip, user,pass,database, Convert.ToInt32(port_str));
    }
    public void SetClient(SQLDataTransmitter client)
    {
        this.client = client;
    }
    public GetCurrentAllInventoryResponse DoGetCurrentAllInventoryRequest(GetCurrentAllInventoryRequest req, string session)
    {
        GetCurrentAllInventoryResponse rsp = req.AllocResponse();
        
        rsp.Inventory = client.GetMedicinesInventory(req.StockIndex);

        return rsp;
    }
    void join(AMDM_MachineInventory m, AMDM_GridInventory grid)
    {
        var stock = findStock(m, grid);
        var floor = findFloor(stock, grid);
        floor.Grids.Add(grid);
    }
    AMDM_StockInventory findStock(AMDM_MachineInventory m, AMDM_GridInventory grid)
    {
        if (m.Stocks == null)
        {
            m.Stocks = new List<AMDM_StockInventory>();
        }
        if (m.Stocks.Count == 0)
        {
            
        }
        else
        {
            foreach (var item in m.Stocks)
            {
                if (item.StockIndex == grid.StockIndex)
                {
                    return item;
                }
            }
        }
        var stock = new AMDM_StockInventory()
        {
            StockIndex = grid.StockIndex
        };
        m.Stocks.Add(stock);
        return stock;
    }
    AMDM_FloorInventory findFloor(AMDM_StockInventory s, AMDM_GridInventory grid)
    {
        if (s.Floors == null)
        {
            s.Floors = new List<AMDM_FloorInventory>();
        }
        if (s.Floors.Count == 0)
        {

        }
        else
        {
            foreach (var item in s.Floors)
            {
                if (item.FloorIndex == grid.FloorIndex)
                {
                    return item;
                }
            }
        }
        var floor = new AMDM_FloorInventory()
        {
            StockIndex = grid.StockIndex,
            FloorIndex = grid.FloorIndex,
            TotalCounnt = 0,
            Grids = new List<AMDM_GridInventory>()
        };
        s.Floors.Add(floor);
        return floor;
    }
    #region 2021年12月22日21:23:18  补充一种支持使用格子的方式来获取库存信息 也就是获取格子信息带着库存信息 这样方便展示格子和库存
    //由于使用了medicine object 的形式 所以这种方式需要改良
    //2022年1月18日10:45:14  改良后 可以看到每一个格子的当前库存信息和所装载的内容
    public GridInventoryGetResponse DoGridInventoryGetRequest(GridInventoryGetRequest req, string session)
    {
        GridInventoryGetResponse rsp = req.AllocResponse();
        try
        {
            req.Validate();
        }
        catch (Exception paramerr)
        {
            rsp.ErrMsg = paramerr.Message;
            rsp.ErrCode = "400";
            return rsp;
        }
        rsp.GridsInventory = new List<AMDM_GridInventory>();
        //获取这个药仓中的所有层的长度
        //先获取格子信息
        var medicinesObj = client.GetMedicinesObjectWithClipStuckAndGridAndMedicine(req.StockIndex);
        Dictionary<string,AMDM_GridInventory> gridsDic = new Dictionary<string,AMDM_GridInventory>();

        foreach (var mobj in medicinesObj)
        {
            //if (mobj.FloorIndex ==11 && mobj.GridIndex == 0)
            //{
                
            //}
            string gridKey = string.Format("{0}->{1}->{2}", mobj.StockIndex, mobj.FloorIndex, mobj.GridIndex);
            List<AMDM_MedicineObject_data> currentList = null;
            #region 如果不存在格子信息,添加格子
            if (gridsDic.ContainsKey(gridKey) == false)
            {
                currentList = new List<AMDM_MedicineObject_data>();
                AMDM_GridInventory i = new AMDM_GridInventory()
                            {
                                GridIndex = mobj.GridIndex,
                                MedicinesObject = new List<AMDM_MedicineObject_data>(),
                                Barcode = mobj.Barcode,
                                Company = mobj.Company,
                                Count = 0,
                                 GridIndexOfStock = mobj.IndexOfStock,
                                FloorIndex = mobj.FloorIndex,
                                Name = mobj.Name,
                                StockIndex = mobj.StockIndex,
                                Max = mobj.Max,
                                Stuck = mobj.Stuck,
                                IdOfHIS = mobj.IdOfHIS,
                            };
                gridsDic.Add(gridKey, i);
            }


            #endregion
            #region 如果已经存在格子信息,找到格子和格子的实体表

            else
            {
                currentList = gridsDic[gridKey].MedicinesObject;
            }
            #endregion
            #region 在格子中加入一个实体药品

            if (mobj.InStockRecordId>0)
            {
                AMDM_MedicineObject_data medicineObject = new AMDM_MedicineObject_data()
                {
                    ExpirationDate = mobj.ExpirationDate,
                    InStockTime = mobj.InStockTime,
                    GridIndex = mobj.GridIndex,
                    MedicineId = mobj.GridIndex,
                    FloorIndex = mobj.FloorIndex,
                    StockIndex = mobj.FloorIndex,
                    InStockRecordId = mobj.InStockRecordId,
                    OutStockRecordId = mobj.OutStockRecordId,
                    OutStockTime = mobj.OutStockTime,
                    Id = mobj.Id,
                    ProductionDate = mobj.ProductionDate
                };

                currentList.Add(medicineObject);
                gridsDic[gridKey].Count++;
            }
            else
            {
                //只有获取到了格子和药品信息没有获取到药品实体信息.
            }
            #endregion
        }
        //获取库存信息

        rsp.Machine = new AMDM_MachineInventory();
        foreach (var item in gridsDic)
        {
            join(rsp.Machine, item.Value);
        }

        //rsp.GridsInventory = new List<AMDM_GridInventory>(gridsDic.Values);

        //#region 把行数据组合成tree
        //AMDM_MachineInventory_forsort iTree = new AMDM_MachineInventory_forsort();
        //foreach (var item in rsp.GridsInventory)
        //{
        //    AMDM_GridInventory current = item;
        //    int stockIndex = current.StockIndex;
        //    int floorIndex = current.FloorIndex;
        //    int gridIndex = current.GridIndex;
        //    if (iTree.StocksInventory.ContainsKey(stockIndex) == false)
        //    {
        //        iTree.StocksInventory.Add(stockIndex, new AMDM_StockInventory_forsort() { 
        //         StockIndex = stockIndex, TotalCount = 0, FloorsInventory = new Dictionary<int,AMDM_FloorInventory_forsort>()}
        //            );
        //    }
        //    AMDM_StockInventory_forsort stockInventory = iTree.StocksInventory[stockIndex];
        //    if (stockInventory.FloorsInventory.ContainsKey(floorIndex) == false)
        //    {
        //        stockInventory.FloorsInventory.Add(floorIndex, new AMDM_FloorInventory_forsort()
        //        {
        //            GridsInventory = new Dictionary<int, AMDM_GridInventory>(),
        //            TotalCounnt = 0,
        //            FloorIndex = floorIndex,
        //            StockIndex = stockIndex,
        //        }
        //            );
        //    }
        //    AMDM_FloorInventory_forsort floorInventory = stockInventory.FloorsInventory[floorIndex];
        //    if (floorInventory.GridsInventory.ContainsKey(gridIndex) == false)
        //    {
        //        floorInventory.GridsInventory.Add(gridIndex, current);
        //    }
        //    else
        //    {
        //        floorInventory.GridsInventory[gridIndex].Count += current.Count;
        //    }
        //    floorInventory.TotalCounnt += current.Count;
        //    stockInventory.TotalCount += current.Count;
        //    iTree.TotalCount += current.Count;
        //}


        //#endregion
        //#region 把每个仓里面的层数据,上层的反转过来
        //foreach (var stock in iTree.StocksInventory)
        //{
        //    Dictionary<int, AMDM_FloorInventory_forsort> upPartFloors = new Dictionary<int, AMDM_FloorInventory_forsort>();
        //    Dictionary<int, AMDM_FloorInventory_forsort> downPartFloors = new Dictionary<int, AMDM_FloorInventory_forsort>();
        //    var iem = stock.Value.FloorsInventory.GetEnumerator();
        //    while(iem.MoveNext())
        //    {
        //        var floor = iem.Current.Value;
        //        if (floor.FloorIndex >=0)
        //        {
        //            upPartFloors.Add(floor.FloorIndex, floor);
        //        }
        //        else
        //        {
        //            downPartFloors.Add(floor.FloorIndex, floor);
        //        }
        //        //stock.Value.FloorsInventory.Remove(iem.Current.Key);
        //    }
        //    stock.Value.FloorsInventory.Clear();
        //    //上层的已经整理完毕.看看
        //    //把每一个insert回去,看看
        //    upPartFloors = Utils.Z2ADictionary(upPartFloors);

        //    foreach (var floor in upPartFloors)
        //    {
        //        stock.Value.FloorsInventory.Add(floor.Key, floor.Value);
        //    }
        //    foreach (var floor in downPartFloors)
        //    {
        //        stock.Value.FloorsInventory.Add(floor.Key, floor.Value);
        //    }
        //}
        //#endregion
        //#region 把Tree使用Dictionary保存的数据保存成List,方便前端直接map输出
        //rsp.Machine = new AMDM_MachineInventory();
        //rsp.Machine.Stocks = new List<AMDM_StockInventory>();
        //rsp.Machine.TotalCount = iTree.TotalCount;
        //foreach (var stock in iTree.StocksInventory)
        //{
        //    rsp.Machine.Stocks.Add(stock.Value);
        //    stock.Value.Floors = new List<AMDM_FloorInventory>();
        //    foreach (var floor in stock.Value.FloorsInventory)
        //    {
        //        stock.Value.Floors.Add(floor.Value);
        //        floor.Value.Grids = new List<AMDM_GridInventory>();
        //        foreach (var grid in floor.Value.GridsInventory)
        //        {
        //            floor.Value.Grids.Add(grid.Value);
        //            //floor.Value.TotalCounnt += grid.Value.Count;
        //        }
        //        floor.Value.GridsInventory.Clear();
        //        //stock.Value.TotalCount += floor.Value.TotalCounnt;
        //    }
        //    stock.Value.FloorsInventory.Clear();
        //    //rsp.Machine.TotalCount += stock.Value.TotalCount;
        //}
        //iTree.StocksInventory.Clear();
        //iTree = null;
        //rsp.GridsInventory = null;


        //#endregion
        return rsp;
    }
    #endregion
    public MedicinesGetResponse DoMedicinesGetRequest(MedicinesGetRequest req, string session)
    {
        MedicinesGetResponse rsp = req.AllocResponse();
        #region 校验入参
        try
        {
            req.Validate();
        }
        catch (Exception checkParamErr)
        {
            rsp.ErrMsg = checkParamErr.Message;
            return rsp;
        }
        #endregion
        #region 根据条件获取数据
        if (req.GetTotalRecordCount == true)
        {
            rsp.TotalRecordCount = client.GetMedicinesKindCount(req.Tags,req.Barcode);
        }
        rsp.Medicines = client.GetMedicines(req.Fields, req.Tags, req.Barcode, req.PageNum.Value, req.PageSize.Value+1, true);
        if (rsp.Medicines == null)
        {
            rsp.ErrMsg = "获取药品请求过程发生错误,返回了错误的结果集";
            rsp.ErrCode = "500";
            return rsp;
        }
        if (rsp.Medicines.Count > req.PageSize.Value)
        {
            rsp.HasNext = true;
            rsp.Medicines.RemoveAt(req.PageSize.Value);//删掉最后一个
        }
        #endregion

        return rsp;
    }
    public GridsGetResponse DoGridsGetRequest(GridsGetRequest req, string session)
    {
        GridsGetResponse rsp = req.AllocResponse();
        #region 校验入参
        try
        {
            req.Validate();
        }
        catch (Exception checkParamErr)
        {
            rsp.ErrMsg = checkParamErr.Message;
            return rsp;
        }
        #endregion
        rsp.Grids = client.GetGridsEx(req.StockIndex, req.FloorIndex, req.GridIndex);
        return rsp;
    }
    public MedicineAddResponse DoMedicineAddRequest(MedicineAddRequest req, string session)
    {
        MedicineAddResponse rsp = req.AllocResponse();
        try
        {
            req.Validate();
            
        }
        catch (Exception err)
        {
            rsp.ErrMsg = err.Message;
            rsp.ErrCode = "400";
            return rsp;
        }
        try
        {
            
            AMDM_Domain.AMDM_Medicine medicine = Newtonsoft.Json.JsonConvert.DeserializeObject<AMDM_Domain.AMDM_Medicine>(req.MedicineInfoJson);
            #region 先检查是否有这个药品的his码
            Dictionary<string,long> hasedMedicines = client.GetMedicinesIdByHISMedicineIdsList(new List<string>() {medicine.IdOfHIS});
            if (hasedMedicines!= null && hasedMedicines.Count>0)
            {
                rsp.ErrMsg = string.Format("已经存在该HIS药品编码的药品");
                rsp.ErrCode = "401";
                return rsp;
            }
            #endregion
            if (client.AddMedicine(medicine))
            {
                rsp.NewMedicine = medicine;
                rsp.Success = true;
            }
            else
            {
                rsp.ErrMsg = "保存新药品数据发生未知错误";
                rsp.ErrCode = "500";
                return rsp;
            }
        }
        catch (Exception err)
        {
            rsp.ErrMsg = "添加药品信息失败:" + err.Message;
            rsp.ErrCode = "500";
            return rsp;
        }
        return rsp;
    }

    public MedicineUpdateResponse DoMedicineUpdateRequest(MedicineUpdateRequest req, string session)
    {
        MedicineUpdateResponse rsp = req.AllocResponse();
        try
        {
            req.Validate();
        }
        catch (Exception err)
        {
            rsp.ErrMsg = err.Message;
            rsp.ErrCode = "400";
            return rsp;
        }
        AMDM_Domain.AMDM_Medicine medicine = new AMDM_Domain.AMDM_Medicine();
        Newtonsoft.Json.JsonConvert.PopulateObject(Newtonsoft.Json.JsonConvert.SerializeObject(req), medicine);
        bool updateRet = client.UpdateMedicine(req.Id.Value,
            req.IdOfHIS,
            req.Name,
            req.Barcode,
            req.Company,
            req.BoxLongMM,
            req.BoxWidthMM, 
            req.BoxHeightMM,
            req.CLMED,
            req.SLMED,
            req.DTOEA,
            req.CTOLIA
            );
        if (updateRet == false)
        {
            rsp.ErrMsg = "执行数据更新错误";
            rsp.ErrCode = "500";
            return rsp;
        }
        rsp.UpdatedMedicine = client.GetMedicinesById(req.Id.Value);
        return rsp;
    }

    public MedicineDeleteResponse DoMedicineDeleteRequest(MedicineDeleteRequest req, string session)
    {
        MedicineDeleteResponse rsp = req.AllocResponse();
        try
        {
            req.Validate();
        }
        catch (Exception err)
        {
            rsp.ErrMsg = err.Message;
            rsp.ErrCode = "400";
            return rsp;
        }
        if (client.DeleteMedicine(req.Id.Value) == false)
        {
            rsp.ErrMsg = "删除药品数据失败";
            rsp.ErrCode = "400";
        }
        return rsp;
    }

    public InstockRecordsGetResponse DoInstockRecordsGetRequest(InstockRecordsGetRequest req, string session)
    {
        InstockRecordsGetResponse rsp = req.AllocResponse();
        try
        {
            req.Validate();
        }
        catch (Exception paramErr)
        {
            rsp.ErrMsg = paramErr.Message;
            rsp.ErrCode = "400";
            return rsp;
        }
        if (req.GetTotalRecordCount.Value)
        {
            rsp.TotalRecordCount = client.GetInstockRecordsCount(req.StartCreate, req.EndCreate, req.CancelStatus);
        }
        rsp.InstockRecords = client.GetInstockRecords(req.Fields, req.StartCreate, req.EndCreate, req.CancelStatus, req.PageNum.Value, req.PageSize.Value+1, true);
        if (rsp.InstockRecords.Count>req.PageSize.Value)
        {
            rsp.HasNext = true;
            rsp.InstockRecords.RemoveAt(req.PageSize.Value);
        }
        return rsp;
    }


    public DeliveryRecordsGetResponse DoDeliveryRecordsGetRequest(DeliveryRecordsGetRequest req, string session)
    {
        DeliveryRecordsGetResponse rsp = req.AllocResponse();
        try
        {
            req.Validate();
        }
        catch (Exception paramErr)
        {
            rsp.ErrMsg = paramErr.Message;
            rsp.ErrCode = "400";
            return rsp;
        }
        if (req.GetTotalRecordCount.Value)
        {
            rsp.TotalRecordCount = client.GetDeliveryRecordsCount(req.PrescriptionId,req.PatientName, req.StartCreate,req.EndCreate,req.FinishStatus);
        }
        rsp.DeliveryRecords = client.GetDeliveryRecords(req.Fields,req.PrescriptionId,req.PatientName, req.StartCreate, req.EndCreate, req.CancelStatus,req.FinishStatus, req.PageNum.Value, req.PageSize.Value + 1, true);
        if (rsp.DeliveryRecords.Count > req.PageSize.Value)
        {
            rsp.HasNext = true;
            rsp.DeliveryRecords.RemoveAt(req.PageSize.Value);
        }
        return rsp;
    }

    public SnapshotsGetResponse DoSnapshotsGetRequest(SnapshotsGetRequest req, string session)
    {
        SnapshotsGetResponse rsp = req.AllocResponse();
        try
        {
            req.Validate();
        }
        catch (Exception paramErr)
        {
            rsp.ErrMsg = paramErr.Message;
            rsp.ErrCode = "400";
            return rsp;
        }
        List<AMDM_Domain.AMDM_Snapshot_data> datas = client.GetSnapshots(req.ParentType.Value, req.ParentId.Value);
        #region 赋值他们的url路径,把本地路径信息去掉
        if (datas!= null)
        {
            foreach (var item in datas)
            {
                #region 构造实际的url地址
                string url = "";
                if (string.IsNullOrEmpty(item.LocalFilePath) == false && string.IsNullOrEmpty(this.client_side_sdk_snapshot_url_path) == false)
                {
                    string fileName = System.IO.Path.GetFileName(item.LocalFilePath);
                    string urlFull = string.Format("/{0}/{1}", this.client_side_sdk_snapshot_url_path.Trim('/'), fileName);
                    url = urlFull;
                }
                #endregion
                AMDM_Domain.AMDM_Snapshot snap = new AMDM_Domain.AMDM_Snapshot()
                {
                    Id = item.Id,
                    Time = item.Time,
                    ParentId = item.ParentId,
                    Location = item.Location,
                    Because = item.Because,
                    //LocalFilePath = item.LocalFilePath,
                    ParentType = item.ParentType,
                    FileUrl = url,
                };
                rsp.Snapshots.Add(snap);
            }
        }
        #endregion
        return rsp;
    }

    #region 2022年2月21日11:44:35 药槽使用记录图预览模式
    public ClipInOutRecordSnapGetResponse DoClipInOutRecordSnapGetRequest(ClipInOutRecordSnapGetRequest req, string session)
    {
        ClipInOutRecordSnapGetResponse rsp = req.AllocResponse();
        try
        {
            req.Validate();
        }
        catch (Exception paramErr)
        {
            rsp.ErrMsg = paramErr.Message;
            rsp.ErrCode = "400";
            return rsp;
        }
        bool hasNext = false;
        rsp.Snaps = client.GetClipInOutRecordSnap(req.StockIndex, req.FloorIndex, req.GridIndex,req.StartTime,req.EndTime, req.PageSize, req.PageNum,req.OrderByTimeDescMode, out hasNext);
        if (rsp.Snaps != null)
        {
            foreach (var item in rsp.Snaps)
            {
                #region 构造实际的url地址
                string url = "";
                if (string.IsNullOrEmpty(item.LocalFilePath) == false && string.IsNullOrEmpty(this.client_side_sdk_snapshot_url_path) == false)
                {
                    string fileName = System.IO.Path.GetFileName(item.LocalFilePath);
                    string urlFull = string.Format("/{0}/{1}", this.client_side_sdk_snapshot_url_path.Trim('/'), fileName);
                    url = urlFull;
                }
                #endregion
                item.FileUrl = url;
            }
        }
        return rsp;
    }
    #endregion

    public LogsGetResponse DoLogsGetRequest(LogsGetRequest req, string session)
    {
        LogsGetResponse rsp = req.AllocResponse();
        try
        {
            req.Validate();
        }
        catch (Exception err)
        {
            rsp.ErrMsg = err.Message;
            rsp.ErrCode = "400";
            return rsp;
        }
        Nullable<int> l = null;
        if (string.IsNullOrEmpty(req.Level) == false)
        {
            switch (req.Level.ToLower())
            {
                case "info":
                    l = 0;
                    break;
                case "warning":
                    l = 1;
                    break;
                case "error":
                    l = 2;
                    break;
                case "bug":
                    l = 3;
                    break;
                default:
                    break;
            }
        }
        rsp.TotalResultCount = client.GetLogsTotalResultCount(l, req.TitleTag, req.MessageTag, req.StartTime, req.EndTime);
        List<Dictionary<string, object>> records = client.GetLogs(l, req.TitleTag, req.MessageTag, req.StartTime, req.EndTime, req.PageNum, req.PageSize);
        if (records.Count>req.PageSize)
        {
            records.RemoveAt(records.Count - 1);
            rsp.HasNext = true;
        }
        rsp.Logs = records;
        return rsp;
    }

    public DeliveryRecordDeleteResponse DoDeliveryRecordDeleteRequest(DeliveryRecordDeleteRequest req, string session)
    {
        DeliveryRecordDeleteResponse rsp = req.AllocResponse();
        try
        {
            req.Validate();
        }
        catch (Exception err)
        {
            rsp.ErrMsg = err.Message;
            rsp.ErrCode = "400";
            return rsp;
        }
        var record = client.GetDeliveryRecordById(req.Id.Value);
        if (record == null)
        {
            rsp.ErrMsg = "未查询到付药记录";
            rsp.ErrCode = "400";
            return rsp;
        }
        //var details = client.GetDeliveryRecordDetails(record.Id);
        //if (details != null)
        //{
        //    for (int i = 0; i < details.Count; i++)
        //    {
        //        var current = details[i];

        //        client.FinishedDeliveryRecordDetail(current.Id, true, "强制标记未取药", DateTime.Now);
        //    }
        //}
        bool ret = client.FinishedDeliveryRecord(record.Id, record.TotalKindCount, record.TotalMedicineCount, false, true, DateTime.Now, "管理员账户手动关闭付药单");
        if (!ret)
        {
            rsp.ErrMsg = "以获取到付药记录,尝试关闭时发生错误";
            rsp.ErrCode = "500";
        }
        return rsp;
    }
}