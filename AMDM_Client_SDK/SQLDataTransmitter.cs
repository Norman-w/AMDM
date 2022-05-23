using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AMDM.Manager
{
//    public delegate void Action();
//    public delegate void Action<T1, T2>(T1 t1, T2 t2);
    public delegate void Action<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
    public delegate void Action<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);
    /// <summary>
    /// sql数据交互器
    /// </summary>
    public class SQLDataTransmitter : MysqlClient
    {
        #region 初始化函数
        public SQLDataTransmitter(string ip, string user, string pass, string database, int port)
            :base(ip,user,pass,database,port)
        {

        }
        #endregion
        #region 清空库的层和格子信息
        /// <summary>
        /// 清空一层
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public bool ClearFloorById(int floorId)
        {
            return base.MysqlExecute(string.Format("delete from {0} where floorid={1}", TableName_Grid, floorId)) > 0;
        }
        /// <summary>
        /// 清空一层
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public bool ClearFloorByIndex(int indexOfStock)
        {
            return base.MysqlExecute(string.Format("delete from {0} where floorIndex={1}", TableName_Grid, indexOfStock)) > 0;
        }
        public bool ClearStockByStockId(int stockId)
        {
            return base.MysqlExecute(string.Format("delete from {0} where stockid={1}", TableName_Floor, stockId)) > 0;
        }
        public bool ClearStockByStockIndex(int stockIndexOfMachine)
        {
            return base.MysqlExecute(string.Format("delete from {0} where id in (select id from stock where indexofmachine={1})", TableName_Floor, stockIndexOfMachine)) > 0;
        }
        #endregion
        #region 表名对应的字符串名称
        /// <summary>
        /// 药仓数据表名称
        /// </summary>
        const string TableName_Stock = "amdm_stock";
        /// <summary>
        /// 层板信息数据表名称
        /// </summary>
        const string TableName_Floor = "amdm_floor";
        /// <summary>
        /// 格子信息数据表名称
        /// </summary>
        const string TableName_Grid = "amdm_grid";
        /// <summary>
        /// 机器信息数据表名称
        /// </summary>
        const string TableName_Machine = "amdm_machine";
        /// <summary>
        /// 药品信息数据表名称
        /// </summary>
        const string TableName_Medicine = "amdm_medicine";
        /// <summary>
        /// 格子绑定的药品信息表名称
        /// </summary>
        const string TableName_Binding = "amdm_grid_medicine_binding_info";
        /// <summary>
        /// 药品交付记录表
        /// </summary>
        const string TableName_DeliveryRecord = "amdm_delivery_record";
        /// <summary>
        /// 药品交付记录明细表
        /// </summary>
        const string TableName_DeliveryRecordDetail = "amdm_delivery_record_detail";
        /// <summary>
        /// 入库记录表
        /// </summary>
        const string TableName_InstockRecord = "amdm_instock_record";
        /// <summary>
        /// 入库记录明细表
        /// </summary>
        const string TableName_InstockRecordDetail = "amdm_instock_record_detail";

        /// <summary>
        /// 护士的相关信息
        /// </summary>
        const string TableName_Nurse = "amdm_nurse";

        /// <summary>
        /// 用于拼音检索时的拼音关键字词典
        /// </summary>
        const string TableName_PinyinTagDic = "amdm_pinyin_tag_dic";

        /// <summary>
        /// 保存快照或者是单据截图,屏幕截图等的文件
        /// </summary>
        const string TableName_Snapshot = "amdm_snapshot";

        /// <summary>
        /// 2022年1月15日22:20:03 药仓内放置的实体的药品信息,根据保留的时长 已经被取走的  镜像保留多少天 比如15天
        /// </summary>
        const string TableName_MedicineObject = "amdm_medicine_object";
        #endregion
        #region 数据库交互部分,给定的参数应该为非类对象
        #region 药仓信息
        /// <summary>
        /// 添加药仓信息
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public bool AddStock(AMDM_Stock_data stock)
        {
            SqlInsertRecordParams ipr = new SqlInsertRecordParams();
            ipr.TableName = TableName_Stock;
            ipr.DataObject = stock;
            ipr.RemoveField("floors");
            ipr.RemoveField("id");
            stock.Id = (int)this.InsertData<AMDM_Stock_data>(ipr);
            if (stock.Id>0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除药仓信息.
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public bool RemoveStock(int stockId)
        {
            SqlDeleteRecordParams dpr = new SqlDeleteRecordParams();
            dpr.TableName = TableName_Stock;
            dpr.WhereEquals.Add("id", stockId);

            return base.DeleteData(dpr) > 0;
        }
        /// <summary>
        /// 获取药仓信息,不包含下面的子结构
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <returns></returns>
        public AMDM_Stock_data GetStock(int stockIndex)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Stock;
            gpr.SetFields("*");
                gpr.WhereEquals.Add("indexofmachine", stockIndex);
            List<AMDM_Stock_data> stocks = base.GetDatas<AMDM_Stock_data>(gpr);
            if (stocks != null && stocks.Count == 1)
            {
                return stocks[0];
            }
            return null;
        }
        public List<AMDM_Stock_data> GetALLStocks()
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Stock;
            gpr.SetFields("*");
            List<AMDM_Stock_data> stocks = base.GetDatas<AMDM_Stock_data>(gpr);
            return stocks;
        }
        /// <summary>
        /// 更新药仓的数据信息
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public bool UpdateStock(AMDM_Stock_data stock)
        {
            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = TableName_Stock;
            upr.DataObject = stock;
            upr.WhereEquals.Add("id", stock.Id);

            return base.UpdateData(upr)>0;
        }
        #endregion

        #region 层信息
        /// <summary>
        /// 添加新的层信息到数据库
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        public bool AddFloor(AMDM_Floor floor)
        {
            SqlInsertRecordParams ipr = new SqlInsertRecordParams();
            ipr.TableName = TableName_Floor;
            ipr.DataObject = floor;
            ipr.RemoveField("grids");
            ipr.RemoveField("id");
            floor.Id = (int)this.InsertData<AMDM_Floor_data>(ipr);
            if (floor.Id > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除层信息,根据索引
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public bool RemoveFloor(int floorId)
        {
            SqlDeleteRecordParams dpr = new SqlDeleteRecordParams();
            dpr.TableName = TableName_Floor;
            dpr.WhereEquals.Add("id", floorId);

            return base.DeleteData(dpr) > 0;
        }
        /// <summary>
        /// 更新层信息
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        public bool UpdateFloor(AMDM_Floor_data floor)
        {
            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = TableName_Floor;
            upr.DataObject = floor;
            upr.WhereEquals.Add("id", floor.Id);

            return base.UpdateData(upr)>0;
        }
        /// <summary>
        /// 查询层信息
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public AMDM_Floor GetFloor(int floorId)
        {
            return null;
        }
        /// <summary>
        /// 根据药仓Id,获取药仓下面的所有floor 不包括grid
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public List<AMDM_Floor_data> GetFloors(int stockId)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Floor;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("stockid", stockId);

            List<AMDM_Floor_data> floors = base.GetDatas<AMDM_Floor_data>(gpr);
            return floors;
        }
        #endregion

        #region 格子信息
        /// <summary>
        /// 保存格子信息到数据库
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public bool AddGrid(AMDM_Grid grid)
        {
            SqlInsertRecordParams ipr = new SqlInsertRecordParams();
            ipr.TableName = TableName_Grid;
            ipr.DataObject = grid;
            //ipr.RemoveField("indexofstock");
            ipr.RemoveField("id");
            grid.Id = (int)this.InsertData<AMDM_Grid_data>(ipr);
            if (grid.Id > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除药格信息.
        /// </summary>
        /// <param name="gridId">药格子id</param>
        /// <returns></returns>
        public bool RemoveGrid(int gridId)
        {
            SqlDeleteRecordParams dpr = new SqlDeleteRecordParams();
            dpr.TableName = TableName_Grid;
            dpr.WhereEquals.Add("id", gridId);
            return base.DeleteData(dpr) > 0;
        }
        /// <summary>
        /// 删除层内的所有药槽数据
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public bool RemoveGrids(int floorId)
        {
            SqlDeleteRecordParams dpr = new SqlDeleteRecordParams();
            dpr.TableName = TableName_Grid;
            dpr.WhereEquals.Add("floorid", floorId);

            return base.DeleteData(dpr) > 0;
        }
        /// <summary>
        /// 更新格子信息
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public bool UpdateGrid(AMDM_Grid grid)
        {
            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = TableName_Grid;
            upr.DataObject = grid;
            upr.WhereEquals.Add("id", grid.Id);

            return base.UpdateData(upr)>0;
        }
        /// <summary>
        /// 查询格子信息.
        /// </summary>
        /// <param name="gridId"></param>
        /// <returns></returns>
        public AMDM_Grid_data GetGrid(int gridId)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Grid;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("id", gridId);

            List<AMDM_Grid_data> grids = base.GetDatas<AMDM_Grid_data>(gpr);
            if (grids!= null && grids.Count == 1)
            {
                return grids[0];
            }
            return null;
        }
        /// <summary>
        /// 根据位置获取药槽信息
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <param name="floorIndex"></param>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        public AMDM_Grid_data GetGrid(int stockIndex, int floorIndex, int gridIndex)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Grid;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("stockindex", stockIndex);
            gpr.WhereEquals.Add("floorindex", floorIndex);
            gpr.WhereEquals.Add("indexoffloor", gridIndex);

            List<AMDM_Grid_data> grids = base.GetDatas<AMDM_Grid_data>(gpr);
            if (grids != null && grids.Count == 1)
            {
                return grids[0];
            }
            return null;
        }
        /// <summary>
        /// 查询层内的格子信息,根据给定的层id
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public List<AMDM_Grid_data> GetGrids(int floorId)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Grid;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("floorid", floorId);

            List<AMDM_Grid_data> grids = base.GetDatas<AMDM_Grid_data>(gpr);
            return grids;
        }
        /// <summary>
        /// 获取格子/药槽的信息 同时还包含弹夹和绑定了的药品信息.
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <param name="floorIndex"></param>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        public List<AMDM_Grid__Clip__Medicine> GetGridsEx(Nullable<int> stockIndex, Nullable<int> floorIndex, Nullable<int> gridIndex)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.AppendFormat(
                 @"
SELECT
	g.*, f.DepthMM,
	c.*, m.*
FROM
	{0} g
LEFT JOIN {3} AS f ON f.IndexOfStock = g.FloorIndex
LEFT JOIN {1} AS c ON (
	g.stockindex = c.StockIndex
	AND g.floorindex = c.FloorIndex
	AND g.IndexOfFloor = c.GridIndex
)
LEFT JOIN {2} AS m ON m.Id = c.MedicineId
", TableName_Grid,TableName_Binding,TableName_Medicine,TableName_Floor

                 );
            if (stockIndex!= null)
            {
                cmd.AppendFormat(@" WHERE
	g.StockIndex = {0}",stockIndex);
                if (floorIndex!= null)
                {
                    cmd.AppendFormat(" AND g.FloorIndex={0}", floorIndex);
                    if (gridIndex!= null)
                    {
                        cmd.AppendFormat(" AND g.IndexOfFloor={0}", gridIndex);
                    }
                }
            }
            cmd.Append(" ORDER BY g.stockindex asc, g.floorindex desc, g.indexoffloor asc");
            var ret = base.GetDatas<AMDM_Grid__Clip__Medicine>(cmd.ToString());
            if (ret!= null && ret.Count>0)
            {
                foreach (var item in ret)
                {
                    item.WidthMM = item.RightMM - item.LeftMM;
                    item.HeightMM = item.TopMM - item.BottomMM;
                    item.MaxLoadAbleCount = (int)Math.Floor(item.DepthMM / item.BoxLongMM);
                }
            }
            return ret;
        }
        /// <summary>
        /// 查询药仓中的所有的格子信息,根据stockid
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public List<AMDM_Grid_data> GetStockAllGrids(int stockId)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Grid;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("stockid", stockId);

            List<AMDM_Grid_data> grids = base.GetDatas<AMDM_Grid_data>(gpr);
            return grids;
        }
        public List<AMDM_Grid_data> GetStockAllGridsByStockIndex(int stockIndex)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Grid;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("stockindex", stockIndex);

            List<AMDM_Grid_data> grids = base.GetDatas<AMDM_Grid_data>(gpr);
            return grids;
        }
        #endregion

        #endregion
        #region 药品和格子信息的增删改查
        /// <summary>
        /// 增加绑定信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool AddBindingInfo(AMDM_Clip_data info)
        {
            SqlInsertRecordParams ipr = new SqlInsertRecordParams();
            ipr.TableName = TableName_Binding;
            ipr.DataObject = info;
            info.Id = base.InsertData<AMDM_Clip_data>(ipr);
            return info.Id > 0;
        }
        /// <summary>
        /// 删除绑定信息
        /// </summary>
        /// <param name="bindingInfoId"></param>
        /// <returns></returns>
        public bool RemoveBindingInfo(int bindingInfoId)
        {
            return false;
        }
        /// <summary>
        /// 移除格子的绑定信息
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <param name="floorIndex"></param>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        public bool RemoveBindingInfo(int stockIndex, int floorIndex, int gridIndex)
        {
            SqlDeleteRecordParams dpr = new SqlDeleteRecordParams();
            dpr.TableName = TableName_Binding;
            dpr.WhereEquals.Add("stockindex", stockIndex);
            dpr.WhereEquals.Add("floorindex", floorIndex);
            dpr.WhereEquals.Add("gridindex", gridIndex);

            return base.DeleteData(dpr) > 0;
        }
        /// <summary>
        /// 修改绑定信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool UpdateBindingInfo(AMDM_Clip_data info)
        {
            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = TableName_Binding;
            upr.WhereEquals.Add("id", info.Id);
            upr.DataObject = info;
            upr.UpdateFieldNameAndValues.Remove("id");
            return base.UpdateData(upr)>0;
        }

        #region 2022年1月16日13:43:33  不再在bind表中保存当前库存信息,binding表将被视为 弹夹信息
        ///// <summary>
        ///// 更新药品绑定表中存储的药品的库存信息
        ///// </summary>
        ///// <param name="stockIndex"></param>
        ///// <param name="floorIndex"></param>
        ///// <param name="gridIndex"></param>
        ///// <param name="inOutCount"></param>
        ///// <returns></returns>
        //public bool UpdateBindingInfoSavedInventoryInfoField_bak(int stockIndex, int floorIndex, int gridIndex, int inOutCount)
        //{
        //    //根据出入的多少 自动判断是更新最后入库时间字段还是最后出库时间字段
        //    string inOutSetCMD = null;
        //    if (inOutCount>0)
        //    {
        //        inOutSetCMD = string.Format(",lastinstocktime='{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //    }
        //    else if(inOutCount<0)
        //    {
        //        inOutSetCMD = string.Format(",lastdeliverytime='{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //    }
        //    string cmd = string.Format("update {0} set currentinventory=currentinventory+{4}{5} where stockindex={1} and floorindex={2} and gridindex={3}",
        //        TableName_Binding, stockIndex, floorIndex, gridIndex, inOutCount,
        //        inOutSetCMD
        //        );
        //    return base.MysqlExecute(cmd) > 0;
        //}
        ///// <summary>
        ///// 获取在绑定信息表中存储的库存信息,如果获取失败会返回null
        ///// </summary>
        ///// <param name="stockIndex"></param>
        ///// <param name="floorIndex"></param>
        ///// <param name="gridIndex"></param>
        ///// <returns></returns>
        //public Nullable<int> GetBindingInfoSavedInventoryCount_bak(int stockIndex, int floorIndex, int gridIndex)
        //{
        //    SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
        //    gpr.TableName = TableName_Binding;
        //    gpr.WhereEquals.Add("stockindex", stockIndex);
        //    gpr.WhereEquals.Add("floorindex", floorIndex);
        //    gpr.WhereEquals.Add("gridindex", gridIndex);
        //    gpr.SetFields("*");

        //    List<GridMedicineBindingInfo_data> bindings = base.GetDatas<GridMedicineBindingInfo_data>(gpr);
        //    if (bindings.Count == 1)
        //    {
        //        return bindings[0].CurrentInventory;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        #endregion

        #region 增加获取格子又直接能获取到库存信息字段的相关
        /// <summary>
        /// 获取药品的个性化库存信息 包含位置基本信息  药品基本信息 和数量
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <returns></returns>
        //由于使用了medicine object 来保存实际的药品 所以这种方式作废 使用更好的方式来获取弹夹和已装载弹药的相关信息 可以让窗口进行下钻
//        public List<AMDM_GridInventory> GetGridsInventory(Nullable<int> stockIndex)
//        {
//            try
//            {
//                string cmd = string.Format(
//                @"
//SELECT
//	g.StockIndex,
//	g.FloorIndex,
//	g.IndexOfFloor,
//	g.IndexOfStock,
//	m.Name,
//	m.Barcode,
//	m.Company,
//	IFNULL( b.CurrentInventory, 0 ) AS Count 
//FROM
//	{0} g
//	LEFT JOIN {1} AS b ON g.stockindex = b.stockindex 
//	AND g.FloorIndex = b.FloorIndex 
//	AND g.IndexOfFloor = b.GridIndex
//	LEFT JOIN {2} AS m ON m.Id = b.MedicineId 
//{3}
//",

//     TableName_Grid, TableName_Binding, TableName_Medicine,
//     stockIndex == null ? "" : string.Format(" WHERE g.stockindex={0}", stockIndex)
//     );
//                List<AMDM_GridInventory> ret = base.GetDatas<AMDM_GridInventory>(cmd);
//                return ret;
//            }
//            catch (Exception err)
//            {
//                Utils.LogError("在sqldata transmitter中 获取药品的个性化格式库存信息错误:", err.Message);
//                throw err;
//            }
//        }

        #endregion

        ///// <summary>
        ///// 更新呢药品绑定表中存储的药品的库存信息,并设置为0 这个操作是危险的 通常是初始化药库的时候使用
        ///// </summary>
        ///// <param name="stockIndex"></param>
        ///// <param name="floorIndex"></param>
        ///// <param name="gridIndex"></param>
        ///// <returns></returns>
        //public bool UpdateBindingInfoSavedInventoryInfoFieldSetZero(int stockIndex, int floorIndex, int gridIndex)
        //{
        //    string cmd = string.Format("update {0} set currentinventory=0 where stockindex={1} and floorindex={2} and gridindex={3}", TableName_Binding, stockIndex, floorIndex, gridIndex);
        //    return base.MysqlExecute(cmd) > 0;
        //}
        /// <summary>
        /// 获取绑定信息
        /// </summary>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public AMDM_Clip_data GetBindingInfo(int infoId)
        {
            return null;
        }

        /// <summary>
        /// 根据药品的id获取已经绑定的该药品的所有药槽
        /// </summary>
        /// <param name="medicineId"></param>
        /// <returns></returns>
        public List<AMDM_Clip_data> GetBindingsInfoList(Nullable<int> stockIndex, long medicineId)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.WhereEquals.Add("medicineid", medicineId);
            if (stockIndex!= null)
            {
                gpr.WhereEquals.Add("stockindex", stockIndex);
            }
            gpr.SetFields("*");

            return base.GetDatas<AMDM_Clip_data>(gpr);
        }
        /// <summary>
        /// 根据药品的id集合,获取已经绑定的这些药品的所有药槽
        /// </summary>
        /// <param name="medicineIdsList"></param>
        /// <returns></returns>
        public List<AMDM_Clip_data> GetBindingsInfoList(List<long> medicineIdsList)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.SetFields("*");
            gpr.WhereInInfos.Add("medicineid", medicineIdsList);

            return base.GetDatas<AMDM_Clip_data>(gpr);
        }
        #region 作废的使用his系统的id的
//        public List<GridMedicineBindingInfo_data> GetBindingsInfoListByHISMedicinesList(List<long> medicineIdsOfHisList)
//        {
//            StringBuilder inStr = new StringBuilder();
//            for (int i = 0; i < medicineIdsOfHisList.Count; i++)
//            {
//                if (inStr.Length > 0)
//                {
//                    inStr.Append(",");
//                }
//                inStr.Append(medicineIdsOfHisList[i]);
//            }
//            string cmd = string.Format(@"SELECT
//	*
//FROM
//	amdm_grid_medicine_binding_info
//WHERE
//	MedicineId IN (
//		SELECT
//			id
//		FROM
//			amdm_medicine
//		WHERE
//			idofhis IN ({0})
//	);", inStr);
        #endregion

        #region 转换his系统的药品id列表到本地的药品id列表
        /// <summary>
        /// 获取本地药品的id 使用his系统的id列表. key是his的id,value 是本地的id
        /// </summary>
        /// <param name="medicinesIdsOfHisList"></param>
        /// <returns></returns>
        public Dictionary<string, long> GetMedicinesIdByHISMedicineIdsList(List<string> medicinesIdsOfHisList)
        {
            Dictionary<string, long> ret = new Dictionary<string, long>();
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Medicine;
            gpr.WhereInInfos.Add("idofhis", medicinesIdsOfHisList);
            gpr.SetFields("id,idofhis");

            List<AMDM_Medicine> medicines = base.GetDatas<AMDM_Medicine>(gpr);
            if (medicines!= null && medicines.Count>0)
            {
                for (int i = 0; i < medicines.Count; i++)
                {
                    AMDM_Medicine current = medicines[i];
                    if (ret.ContainsKey(current.IdOfHIS) == false)
                    {
                        ret.Add(current.IdOfHIS, current.Id);
                    }
                }
            }
            return ret;
        }
        #endregion

        /// <summary>
        /// 根据仓,层,槽的索引 获取药品绑定信息
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <param name="floorIndex"></param>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        public AMDM_Clip_data GetBindingInfo(int stockIndex, int floorIndex, int gridIndex)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("stockindex", stockIndex);
            gpr.WhereEquals.Add("floorindex", floorIndex);
            gpr.WhereEquals.Add("gridIndex", gridIndex);

            List<AMDM_Clip_data> bindings = base.GetDatas<AMDM_Clip_data>(gpr);
            if (bindings.Count == 1)
            {
                return bindings[0];
            }
            return null;
        }

        /// <summary>
        /// 获取整个药仓的所有药品绑定信息
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <returns></returns>
        public List<T> GetBindingInfos4Stock<T>(int stockIndex) where T : AMDM_Clip_data
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.WhereEquals.Add("stockindex", stockIndex);
            gpr.SetFields("*");

            return base.GetDatas<T>(gpr);
        }
        /// <summary>
        /// 获取整个药机的所有的药品绑定信息
        /// </summary>
        /// <returns></returns>
        public List<AMDM_Clip_data> GetBindingInfos4WholeMachine()
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.SetFields("*");

            return base.GetDatas<AMDM_Clip_data>(gpr);
        }
        /// <summary>
        /// 获取药仓中绑定的药品信息,如果药仓需要重新初始化,必须要检查是否有药品绑定到此药仓.
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <returns></returns>
        public List<AMDM_Clip_data> GetStockBindingInfo(int stockIndex)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.WhereEquals.Add("stockindex", stockIndex);
            gpr.SetFields("*");

            List<AMDM_Clip_data> infos = base.GetDatas<AMDM_Clip_data>(gpr);
            return infos;
        }
        /// <summary>
        /// 获取绑定信息,通过药品的条码
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public AMDM_Clip_data GetBindingInfoByMedicine(string barcode)
        {
            return null;
        }
        /// <summary>
        /// 获取绑定信息,通过层格位置的索引
        /// </summary>
        /// <param name="yxLocation"></param>
        /// <returns></returns>
        public AMDM_Clip_data GetBindingInfo(Point yxLocation)
        {
            return null;
        }
        #region 2022年1月16日17:43:52 作废不使用之前的方式获取适合出库的格子
        ///// <summary>
        ///// 获取绑定信息,通过药品id,看药品在什么位置
        ///// </summary>
        ///// <param name="medicineId"></param>
        ///// <returns></returns>
        //public List<GridMedicineBindingInfo_data> GetFulfillAbleGridBindingsOrderByInstockTimeAsc(long medicineId)
        //{
        //    //SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
        //    //gpr.TableName = TableName_Binding;
        //    //gpr.SetFields("*");
        //    //gpr.WhereEquals.Add("medicineid", medicineId);
        //    //gpr.WhereMoreThan.Add("currentinventory", 0);
        //    //gpr.Sort = "ASC";
        //    //gpr.OrderByColumn = "lastdeliverytime";

        //    string cmd = string.Format("select * from {0} where medicineid={1} and currentinventory>0 order by lastdeliverytime asc", TableName_Binding, medicineId);
        //    Console.ForegroundColor = ConsoleColor.Yellow;
        //    Console.WriteLine(cmd);
        //    Console.ResetColor();
        //    List<GridMedicineBindingInfo_data> bindings = base.GetDatas<GridMedicineBindingInfo_data>(cmd);
        //    Console.WriteLine("获取到的绑定信息数量:{0}", bindings.Count);
        //    return bindings;
        //}
        #endregion
        ///// <summary>
        ///// 根据格子的索引格式 ( stockindex->floorindex->gridindex) 获取格子的最后上药时间
        ///// </summary>
        ///// <param name="gridLocation"></param>
        ///// <returns></returns>
        //public Nullable<DateTime> GetGridLastInstockTime(int stockIndex, int floorIndex, int gridIndex)
        //{
        //    SqlValueGeterParameters gpr = new SqlValueGeterParameters();
        //    gpr.TableName = TableName_InstockRecordDetail;
        //    gpr.WhereEquals.Add("stockindex", stockIndex);
        //    gpr.WhereEquals.Add("floorindex", floorIndex);
        //    gpr.WhereEquals.Add("gridindex", gridIndex);
        //    gpr.Field = "instocktime";

        //    List<DateTime> values = base.GetValues<DateTime>(gpr);
        //    if (values!= null && values.Count == 1)
        //    {
        //        return values[0];
        //    }
        //    return null;
        //}
        #endregion
        #region 药品信息的增删改查
        #region 检查本机是否含有药品数据
        /// <summary>
        /// 检查本机是否含有药品数据,获取含有的药品数量,获取的时候需要根据条件来获取总记录数量
        /// </summary>
        /// <returns></returns>
        public long GetMedicinesKindCount(string tags, string barcode)
        {
            SqlValueGeterParameters gpr = new SqlValueGeterParameters();
            gpr.TableName = TableName_Medicine;
            gpr.Field = "*";
            gpr.IsFuncField = true;
            #region 检索条件
            #region 关键字检索
            if (tags != null && tags.Trim().Length > 0)
            {
                string pyWhereCmd = null;
                if (Utils.IsNumAndEnCh(tags))
                {
                    //所有的要检测的字符串都是字母或者数字,那就用首字母检索的方式.
                    pyWhereCmd = string.Format(
                         @"(
			`{0}` IN (
				SELECT
					CNChars
				FROM
					{1}
				WHERE
					firstpinyin LIKE '%{2}%'
				OR FullPinyin LIKE '%{2}%'
			)
			OR `{0}` LIKE '%{2}%'
		)
                    ",
                      "Name", TableName_PinyinTagDic, tags);
                    gpr.SpecialWhereCommands.Add(pyWhereCmd);
                }
                else
                {
                    gpr.WhereLickParams.Add("name", tags);
                    //使用汉字匹配检索的方式
                }
            }

            #endregion
            if (string.IsNullOrEmpty(barcode) == false && barcode.Trim().Length > 0)
            {
                gpr.WhereLickParams.Add("barcode", barcode);
            }
            #endregion
            return base.GetCount(gpr);
        }
        #endregion
        /// <summary>
        /// 根据药品的id列表 获取药品信息
        /// </summary>
        /// <param name="medicinesIdList"></param>
        /// <returns></returns>
        public List<AMDM_Medicine> GetMedicinesByIdList(List<long> medicinesIdList)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Medicine;
            gpr.WhereInInfos.Add("id", medicinesIdList);
            gpr.SetFields("*");

            return base.GetDatas<AMDM_Medicine>(gpr);
        }
        /// <summary>
        /// 根据药品的id获取药品信息
        /// </summary>
        /// <param name="medicinesIdList"></param>
        /// <returns></returns>
        public AMDM_Medicine GetMedicinesById(long id)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Medicine;
            gpr.WhereEquals.Add("id", id);
            gpr.SetFields("*");

            var rets = base.GetDatas<AMDM_Medicine>(gpr);
            if (rets.Count!=1)
	{
                return null;
	}
            return rets[0];
        }

        /// <summary>
        /// 添加药品信息到数据库,给定一个回调函数,用于接收 amdm_medicie,当前插入序号,总共有多少需要插入,用于在插入了药品以后,返回药品信息
        /// </summary>
        /// <param name="medicines"></param>
        /// <param name="onOneMedicineAdded"></param>
        /// <returns></returns>
        public bool AddMedicines(List<AMDM_Medicine> medicines, Action<AMDM_Medicine,int,int> onOneMedicineAdded)
        {
            for (int i = 0; i < medicines.Count; i++)
            {
                SqlInsertRecordParams ipr = new SqlInsertRecordParams();
                ipr.TableName = TableName_Medicine;
                AMDM_Medicine current = medicines[i];
                ipr.DataObject = current;
                ipr.RemoveField("id");
                current.Id = base.InsertData<AMDM_Medicine>(ipr);
                if (onOneMedicineAdded!= null)
                {
                    onOneMedicineAdded(current, i,medicines.Count);
                }
            }
            return true;
        }

        /// <summary>
        /// 像数据库中添加一个已经定义的药品数据,id添加后自动填充.
        /// </summary>
        /// <param name="medicine"></param>
        /// <returns></returns>
        public bool AddMedicine(AMDM_Medicine medicine)
        {
            SqlInsertRecordParamsV2<AMDM_Medicine> pr = new SqlInsertRecordParamsV2<AMDM_Medicine>(
                TableName_Medicine, medicine, "*", "id", null, null);
            medicine.Id = base.InsertDataV2(pr);

            return medicine.Id > 0;
        }

        /// <summary>
        /// 删除数据库中的所有药品信息,谨慎使用.
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllMedicines()
        {
            return base.MysqlExecute(string.Format("truncate table {0}", TableName_Medicine)) > 0;
        }

        public bool DeleteMedicine(long medicineId)
        {
            SqlDeleteRecordParams dpr = new SqlDeleteRecordParams();
            dpr.TableName = TableName_Medicine;
            dpr.WhereEquals.Add("id", medicineId);
            return base.DeleteData(dpr) >0;
        }

        /// <summary>
        /// 重新设置药品的尺寸信息
        /// </summary>
        /// <param name="medicineId"></param>
        /// <param name="longMM"></param>
        /// <param name="widthMM"></param>
        /// <param name="heightMM"></param>
        /// <returns></returns>
        public bool ResetMedicineSize(long medicineId, float longMM, float widthMM, float heightMM)
        {
            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = TableName_Medicine;
            upr.WhereEquals.Add("id", medicineId);
            upr.UpdateFieldNameAndValues.Add("boxlongmm", longMM);
            upr.UpdateFieldNameAndValues.Add("boxwidthmm", widthMM);
            upr.UpdateFieldNameAndValues.Add("boxheightmm", heightMM);

            return base.UpdateData(upr)>0;
        }
        /// <summary>
        /// 根据药品的条码获取药品
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public AMDM_Medicine GetMedicineByBarcode(string barcode)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Medicine;
            gpr.WhereEquals.Add("barcode", barcode);
            gpr.SetFields("*");

            List<AMDM_Medicine> medicines = base.GetDatas<AMDM_Medicine>(gpr);
            if (medicines!= null)
            {
                if (medicines.Count == 1)
                {
                    return medicines[0];
                }
                else if (medicines.Count > 1)
                {
                    Console.WriteLine("根据同一个条码获取到了多种药品,返回最后一个");
                    return medicines[medicines.Count - 1];
                }
                else
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(barcode);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        sb.Append(Convert.ToString(bytes[i], 16));
                        sb.Append(" ");
                    }
                    Console.WriteLine("GetMedicineByBarcode根据条码没有获取到药品 {0} \r\n {1}---{2}", sb, barcode, barcode.Length);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("GetMedicineByBarcode 根据条码没有获取到药品:{0}", barcode);
                return null;
            }
        }


        /// <summary>
        /// 根据给定的关键字和条码(可以没有)获取药品相关信息,支持分页查询,但是如果想知道是否还有下一页,pagezise比预期值设置大1,就知道还有没有下一页,然后实际返回值的时候给他去掉一条即可.
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="tag"></param>
        /// <param name="barcode"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<AMDM_Medicine> GetMedicines(string fields, string tag,
            string barcode,
            int pageNum, int pageSize, 
            //是否倒叙排列
            bool orderByIdDesc = true)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Medicine;
            gpr.SetFields(fields);
            #region 关键字检索
            if (tag != null && tag.Trim().Length>0)
            {
                string pyWhereCmd  = null;
                if (Utils.IsNumAndEnCh(tag))
                {
                    //所有的要检测的字符串都是字母或者数字,那就用首字母检索的方式.
                   pyWhereCmd = string.Format(
                        @"(
			`{0}` IN (
				SELECT
					CNChars
				FROM
					{1}
				WHERE
					firstpinyin LIKE '%{2}%'
				OR FullPinyin LIKE '%{2}%'
			)
			OR `{0}` LIKE '%{2}%'
		)
                    ",
                     "Name", TableName_PinyinTagDic, tag);
                   gpr.SpecialWhereCommands.Add(pyWhereCmd);
                }
                else
                {
                    gpr.WhereLickParams.Add("name", tag);
                    //使用汉字匹配检索的方式
                }
            }

            #endregion
            if (string.IsNullOrEmpty(barcode) == false && barcode.Trim().Length>0)
            {
                gpr.WhereLickParams.Add("barcode", barcode);
            }
            if (orderByIdDesc)
            {
                gpr.OrderByColumn = "Id";
                gpr.Sort = "DESC";
            }
            gpr.MaxRecordCount = pageSize;
            gpr.StartIndex = pageNum * (pageSize-1);

            return base.GetDatas<AMDM_Medicine>(gpr);
        }

        /// <summary>
        /// 更新药品信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idofhis"></param>
        /// <param name="name"></param>
        /// <param name="barcode"></param>
        /// <param name="company"></param>
        /// <param name="longMM"></param>
        /// <param name="widthMM"></param>
        /// <param name="heightMM"></param>
        /// <returns></returns>
        public bool UpdateMedicine(long id,
            string idofhis,
            string name,
            string barcode,
            string company,
            Nullable<float> boxLongMM,
            Nullable<float> boxWidthMM,
            Nullable<float> boxHeightMM,
            Nullable<int> clmed,
            Nullable<int> slmed,
            Nullable<int> dtoea,
            Nullable<int> ctolia
            )
        {
            if (id <=0)
            {
                return false;
            }
            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = TableName_Medicine;
            upr.WhereEquals.Add("id", id);
            if (idofhis!= null)
            {
                upr.UpdateFieldNameAndValues.Add("idofhis", idofhis);
            }
            if (name != null)
            {
                upr.UpdateFieldNameAndValues.Add("name", name);
            }
            if (barcode != null)
            {
                upr.UpdateFieldNameAndValues.Add("barcode", barcode);
            }
            if (company != null)
            {
                upr.UpdateFieldNameAndValues.Add("company", company);
            }
            if (boxLongMM != null)
            {
                upr.UpdateFieldNameAndValues.Add("boxLongMM", boxLongMM);
            }
            if (boxWidthMM != null)
            {
                upr.UpdateFieldNameAndValues.Add("boxWidthMM", boxWidthMM);
            }
            if (boxHeightMM != null)
            {
                upr.UpdateFieldNameAndValues.Add("boxHeightMM", boxHeightMM);
            }
            if (clmed != null)
            {
                upr.UpdateFieldNameAndValues.Add("clmed", clmed);
            }
            if (slmed != null)
            {
                upr.UpdateFieldNameAndValues.Add("slmed", slmed);
            }
            if (dtoea != null)
            {
                upr.UpdateFieldNameAndValues.Add("dtoea", dtoea);
            }
            if (ctolia != null)
            {
                upr.UpdateFieldNameAndValues.Add("ctolia", ctolia);
            }

            if (upr.UpdateFieldNameAndValues.Count == 0)
            {
                return false;
            }
            return base.UpdateData(upr)  == 1;
        }
        #endregion


        #region 库存相关
        /// <summary>
        /// 增加药品交付记录表记录,不包含明细信息
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool InsertDeliveryRecord(AMDM_DeliveryRecord_data record)
        {
            SqlInsertRecordParams ipr = new SqlInsertRecordParams();
            ipr.TableName = TableName_DeliveryRecord;
            ipr.DataObject = record;
            ipr.RemoveField("details");

            record.Id = base.InsertData<AMDM_DeliveryRecord_data>(ipr);
            return record.Id > 0;
        }

        /// <summary>
        /// 增加药品交付记录明细表中的记录,给定这个detail的时候detail中的parentid应该已经赋值.
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public bool InsertDeliveryRecordDetail(AMDM_DeliveryRecordDetail_data detail)
        {
            if (detail.ParentId <=0)
            {
                Console.WriteLine("需要给定药品交付记录表中的记录的父表的id");
                return false;
            }
            SqlInsertRecordParams ipr = new SqlInsertRecordParams();
            ipr.TableName = TableName_DeliveryRecordDetail;
            ipr.DataObject = detail;

            detail.Id = base.InsertData<AMDM_DeliveryRecordDetail_data>(ipr);
            return detail.Id > 0;
        }

        /// <summary>
        /// 完成单次付药单记录
        /// </summary>
        /// <param name="detailId"></param>
        /// <param name="isError"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool FinishedDeliveryRecordDetail(long detailId, bool isError, string errMsg, DateTime time)
        {
            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = TableName_DeliveryRecordDetail;
            upr.WhereEquals.Add("id", detailId);
            upr.UpdateFieldNameAndValues.Add("iserror", isError);
            upr.UpdateFieldNameAndValues.Add("errmsg", errMsg);
            upr.UpdateFieldNameAndValues.Add("endtime", time);

            return base.UpdateData(upr)>0;
        }

        /// <summary>
        /// 更新付药单记录信息
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="totalKindCount"></param>
        /// <param name="totalMedicineCount"></param>
        /// <param name="finished"></param>
        /// <param name="canceled"></param>
        /// <param name="snapshotImageFilePath"></param>
        /// <param name="endTime"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public bool FinishedDeliveryRecord(long recordId,int totalKindCount, int totalMedicineCount, bool finished, bool canceled, Nullable<DateTime> endTime, string memo)
        {
            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = TableName_DeliveryRecord;
            upr.WhereEquals.Add("id", recordId);
            //upr.UpdateFieldNameAndValues.Add("id", recordId);
            upr.UpdateFieldNameAndValues.Add("totalkindcount", totalKindCount);
            upr.UpdateFieldNameAndValues.Add("totalmedicinecount", totalMedicineCount);
            upr.UpdateFieldNameAndValues.Add("finished", finished);
            upr.UpdateFieldNameAndValues.Add("canceled", canceled);
            //upr.UpdateFieldNameAndValues.Add("snapshotImageFile", snapshotImageFilePath);
            if (endTime != null)
            {
                upr.UpdateFieldNameAndValues.Add("endtime", endTime);
            }
            upr.UpdateFieldNameAndValues.Add("memo", memo);

            return base.UpdateData(upr)>0;
        }
        /// <summary>
        /// 结束了交付记录的截图
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="snapshotImageFilePath"></param>
        /// <returns></returns>
        public bool FinishedDeliveryRecordSnapshotCapture(long recordId, string snapshotImageFilePath)
        {
            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = TableName_DeliveryRecord;
            upr.WhereEquals.Add("id", recordId);
            //upr.UpdateFieldNameAndValues.Add("id", recordId);
            upr.UpdateFieldNameAndValues.Add("snapshotImageFile", snapshotImageFilePath);
            
            return base.UpdateData(upr)>0;
        }

        #region 获取药机的当前库存
        /// <summary>
        /// 获取药机的当前库存,如果参数为空,获取全部药仓的,如果参数不为空,获取指定的仓库的
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <returns></returns>
        public List<AMDM_MedicineInventory>GetMedicinesInventory(Nullable<int> stockIndex)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.AppendFormat(
                @"SELECT
	m.*, count(*) AS Count
FROM
	{0} o
LEFT JOIN {1} AS m ON o.medicineid = m.id
WHERE
	o.OutStockRecordId IS NULL
GROUP BY
	o.medicineid",
                 TableName_MedicineObject,
                 TableName_Medicine,
                 TableName_Binding,
                 stockIndex == null? "": string.Format(" AND o.StockIndex = {0} ", stockIndex)
                 );
            List<AMDM_MedicineInventory> infos = base.GetDatas<AMDM_MedicineInventory>(cmd.ToString());
            return infos;
            //string andCmd = null;
            //if (stockIndex!= null)
            //{
            //    andCmd = string.Format(" and b.StockIndex={0}", stockIndex);
            //}
            //StringBuilder fieldsSB = new StringBuilder("m.*");
            //if (fields!= null)
            //{
            //    string[] fs = fields.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries);
            //    if (fs!= null && fs.Length>0)
            //    {
            //        fieldsSB = new StringBuilder();
            //        foreach (var item in fs)
            //        {
            //            string field = item.Replace(",", "");
            //            if (fieldsSB.Length>0)
            //            {
            //                fieldsSB.Append(",");
            //            }
            //            fieldsSB.AppendFormat("m.{0}", field);
            //        }
            //    }
            //}
            
            //string cmd = string.Format("select {3},b.CurrentInventory as count from {0} b, {1} m where b.MedicineId = m.id{2};",
            //    TableName_Binding,
            //    TableName_Medicine, 
            //    andCmd,
            //    fieldsSB
            //    );
            //List<AMDM_MedicineInventory> infos = base.GetDatas<AMDM_MedicineInventory>(cmd);
            //return infos;
        }
        #endregion

        #region 获取取药记录
        /// <summary>
        /// 根据处方编号获取付药记录
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>
        public List<AMDM_DeliveryRecord_data>GetDeliveryRecordByPrescriptionId(string prescriptionId)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_DeliveryRecord;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("PrescriptionId", prescriptionId);

            List<AMDM_DeliveryRecord_data> ret = base.GetDatas<AMDM_DeliveryRecord_data>(gpr);
            return ret;
        }
        public List<AMDM_DeliveryRecordDetail_data> GetDeliveryRecordDetails(long recordId)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_DeliveryRecordDetail;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("parentid", recordId);

            return base.GetDatas<AMDM_DeliveryRecordDetail_data>(gpr);
        }
        /// <summary>
        /// 获取付药单记录,不包含详情条目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AMDM_DeliveryRecord_data GetDeliveryRecordById(long id)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_DeliveryRecord;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("id", id);

            List<AMDM_DeliveryRecord_data> ret = base.GetDatas<AMDM_DeliveryRecord_data>(gpr);
            if (ret == null || ret.Count  == 0)
            {
                Utils.LogWarnning("根据id没有获取到付药记录");
                return null;
            }
            else if(ret.Count>1)
            {
                Utils.LogBug("根据id获取到了多个付药记录", ret);
                return null;
            }
            else
            {
                return ret[0];
            }
        }
        ///// <summary>
        ///// 根据付药单的id获取他下面的条目集合
        ///// </summary>
        ///// <param name="parentId"></param>
        ///// <returns></returns>
        //public List<AMDM_DeliveryRecordDetail> GetDeliveryRecordDetails(long parentId)
        //{
        //    SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
        //    gpr.TableName = TableName_DeliveryRecordDetail;
        //    gpr.WhereEquals.Add("parentid", parentId);
        //    gpr.SetFields("*");

        //    return base.GetDatas<AMDM_DeliveryRecordDetail>(gpr);
        //}


        /// <summary>
        /// 获取取药单记录,如果指定了fields是 * 或者是all 或者是包含 details 就会获取具体的详细的条目信息
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="startCreate"></param>
        /// <param name="endCraete"></param>
        /// <param name="canceledStatus"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<AMDM_DeliveryRecord> GetDeliveryRecords(string fields,
            string prescriptionId,
            string patientName,
            Nullable<DateTime> startCreate,
            Nullable<DateTime> endCraete,
            Nullable<bool> canceledStatus,
            Nullable<bool> finishStatus,
            int pageNum, int pageSize, bool orderByIdDesc = true)
        {
            if (string.IsNullOrEmpty(fields) == true)
            {
                Utils.LogError("在尝试从数据库获取取药记录时错误,未知的目标字段信息");
                return null;
            }
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_DeliveryRecord;
            gpr.SetFields(fields);
            //如果指定了根据处方编号获取
            if (string.IsNullOrEmpty(prescriptionId) == false)
            {
                gpr.WhereEquals.Add("prescriptionid", prescriptionId);
            }
            if (string.IsNullOrEmpty(patientName) == false)
            {
                gpr.WhereLickParams.Add("patientName", patientName);
            }
            if (startCreate != null && endCraete != null)
            {
                gpr.WhereMoreThan.Add("starttime", startCreate);
                gpr.WhereLessThan.Add("starttime", endCraete);
            }
            if (canceledStatus != null)
            {
                gpr.WhereEquals.Add("canceled", canceledStatus);
            }
            if (finishStatus!= null)
            {
                gpr.WhereEquals.Add("finished", finishStatus);
            }
            if (orderByIdDesc)
            {
                gpr.Sort = "DESC";
                gpr.OrderByColumn = "Id";
            }
            gpr.MaxRecordCount = pageSize;
            gpr.StartIndex = pageNum * (pageSize-1);

            List<AMDM_DeliveryRecord> records = base.GetDatas<AMDM_DeliveryRecord>(gpr);
            if (records != null && records.Count > 0 &&
                (fields.Contains("*") || fields.ToLower() == "all" || fields.Contains("details"))
                )
            {
                //获取取药单的详单信息
                foreach (var item in records)
                {
                    SqlObjectGeterParameters dgpr = new SqlObjectGeterParameters();
                    dgpr.TableName = TableName_DeliveryRecordDetail;
                    dgpr.WhereEquals.Add("parentid", item.Id);
                    dgpr.SetFields("*");
                    List<AMDM_DeliveryRecordDetail_data> details = base.GetDatas<AMDM_DeliveryRecordDetail_data>(dgpr);
                    item.Details = details;
                }
            }

            return records;
        }

        /// <summary>
        /// 获取符合指定条件 或者不指定条件直接查询表 的记录的条数
        /// </summary>
        /// <param name="startCraete"></param>
        /// <param name="endCraete"></param>
        /// <param name="cancelStatus"></param>
        /// <returns></returns>
        public long GetDeliveryRecordsCount(string prescriptionId ,string patientName, Nullable<DateTime> startCraete = null, Nullable<DateTime> endCraete = null, Nullable<bool> cancelStatus = null)
        {
            SqlValueGeterParameters gpr = new SqlValueGeterParameters();
            gpr.TableName = TableName_DeliveryRecord;
            gpr.IsFuncField = true;
            if (string.IsNullOrEmpty(prescriptionId) == false)
            {
                gpr.WhereEquals.Add("prescriptionId", prescriptionId);
            }
            if (string.IsNullOrEmpty(patientName) == false)
            {
                gpr.WhereLickParams.Add("patientName", patientName);
            }
            if (startCraete != null && endCraete != null)
            {
                gpr.WhereMoreThan.Add("starttime", startCraete);
                gpr.WhereLessThan.Add("starttime", endCraete);
            }
            if (cancelStatus != null)
            {
                gpr.WhereEquals.Add("canceled", cancelStatus);
            }
            return base.GetCount(gpr);
        }
        
        #endregion


        #region 上药记录相关
        /// <summary>
        /// 插入入库单,不包含details,插入后record的id赋值
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool InsertInstockRecord(AMDM_InstockRecord_data record)
        {
            SqlInsertRecordParams ipr = new SqlInsertRecordParams();
            ipr.TableName = TableName_InstockRecord;
            ipr.DataObject = record;
            ipr.RemoveField("details");

            record.Id = base.InsertData<AMDM_InstockRecord_data>(ipr);
            return record.Id > 0;
        }
        /// <summary>
        /// 插入入库单明细,插入后detail的id赋值
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public bool InsertInstockRecordDetail(AMDM_InstockRecordDetail detail)
        {
            if (detail.ParentId <= 0)
            {
                Console.WriteLine("需要给定药品交付记录表中的记录的父表的id");
                return false;
            }
            SqlInsertRecordParams ipr = new SqlInsertRecordParams();
            ipr.TableName = TableName_InstockRecordDetail;
            ipr.DataObject = detail;

            detail.Id = base.InsertData<AMDM_InstockRecordDetail>(ipr);
            return detail.Id > 0;
        }
        /// <summary>
        /// 完成整个上药单
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="totalKindCount"></param>
        /// <param name="totalMedicineCount"></param>
        /// <param name="finished"></param>
        /// <param name="canceled"></param>
        /// <param name="snapshotImageFilePath"></param>
        /// <param name="endTime"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public bool FinishedInstockRecord(long recordId, int entriesCount, int totalMedicineCount, bool canceled, Nullable<DateTime> finishTime, string memo)
        {
            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = TableName_InstockRecord;
            upr.WhereEquals.Add("id", recordId);
            upr.UpdateFieldNameAndValues.Add("entriesCount", entriesCount);
            upr.UpdateFieldNameAndValues.Add("totalmedicinecount", totalMedicineCount);
            upr.UpdateFieldNameAndValues.Add("canceled", canceled);
            if (finishTime != null)
            {
                upr.UpdateFieldNameAndValues.Add("finishtime", finishTime);
            }
            upr.UpdateFieldNameAndValues.Add("memo", memo);

            return base.UpdateData(upr)>0;
        }
        
        /// <summary>
        /// 获取入库单记录,如果指定了fields是 * 或者是all 或者是包含 details 就会获取具体的详细的条目信息
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="startCreate"></param>
        /// <param name="endCraete"></param>
        /// <param name="canceledStatus"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<AMDM_InstockRecord> GetInstockRecords(string fields, Nullable<DateTime> startCreate, 
            Nullable<DateTime> endCraete, 
            Nullable<bool> canceledStatus,
            //是否使用倒叙排列的方式
            int pageNum, int pageSize, bool orderByIdDesc = true)
        {
            if (string.IsNullOrEmpty(fields) == true)
            {
                Utils.LogError("在尝试从数据库获取入库记录时错误,未知的目标字段信息");
                return null;
            }
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_InstockRecord;
            gpr.SetFields(fields);
            if (startCreate != null && endCraete != null)
            {
                gpr.WhereMoreThan.Add("createtime", startCreate);
                gpr.WhereLessThan.Add("createtime", endCraete);
            }
            if (canceledStatus!= null)
            {
                gpr.WhereEquals.Add("canceled", canceledStatus);
            }
            if (orderByIdDesc)
            {
                gpr.Sort = "DESC";
                gpr.OrderByColumn = "Id";
            }
            gpr.MaxRecordCount = pageSize;
            gpr.StartIndex = pageNum * (pageSize-1);

            List<AMDM_InstockRecord> records = base.GetDatas<AMDM_InstockRecord>(gpr);
            if (records!= null && records.Count>0 && 
                (fields.Contains("*") || fields.ToLower() == "all" || fields.Contains("details"))
                )
            {
                //获取入库单的详单信息
                foreach (var item in records)
                {
                    SqlObjectGeterParameters dgpr = new SqlObjectGeterParameters();
                    dgpr.TableName = TableName_InstockRecordDetail;
                    dgpr.WhereEquals.Add("parentid", item.Id);
                    dgpr.SetFields("*");
                    List<AMDM_InstockRecordDetail> details = base.GetDatas<AMDM_InstockRecordDetail>(dgpr);
                    item.Details = details;
                }
            }

            return records;
        }

        /// <summary>
        /// 获取符合指定条件 或者不指定条件直接查询表 的记录的条数
        /// </summary>
        /// <param name="startCraete"></param>
        /// <param name="endCraete"></param>
        /// <param name="cancelStatus"></param>
        /// <returns></returns>
        public long GetInstockRecordsCount(Nullable<DateTime> startCraete =null, Nullable<DateTime> endCraete = null, Nullable<bool> cancelStatus = null)
        {
            SqlValueGeterParameters gpr = new SqlValueGeterParameters();
            gpr.TableName = TableName_InstockRecord;
            gpr.IsFuncField = true;
            if (startCraete != null && endCraete != null)
            {
                gpr.WhereMoreThan.Add("createtime", startCraete);
                gpr.WhereLessThan.Add("createtime", endCraete);
            }
            if (cancelStatus!= null)
            {
                gpr.WhereEquals.Add("canceled", cancelStatus);
            }
            return base.GetCount(gpr);
        }
        #endregion
        #endregion

        #region 用户相关
        /// <summary>
        /// 根据用户名和密码获取护士信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public AMDM_Nurse GetNurse(string user, string pass)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Nurse;
            gpr.WhereEquals.Add("user", user);
            gpr.WhereEquals.Add("pass", pass);
            gpr.SetFields("*");

            List<AMDM_Nurse> nurses = base.GetDatas<AMDM_Nurse>(gpr);
            if (nurses!= null && nurses.Count == 1)
            {
                return nurses[0];
            }
            else if(nurses!= null && nurses.Count>1)
            {
                Utils.LogBug("根据用户名和密码获取到的护士的信息不唯一", nurses);
                return null;
            }
            else
            {
                //没有获取到护士
                return null;
            }
        }
        #endregion

        #region 凭据图片相关
        /// <summary>
        /// 根据截图/拍照/快照信息的id获取快照
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<AMDM_Snapshot_data> GetSnapshots(long id)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Snapshot;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("id", id);

            return base.GetDatas<AMDM_Snapshot_data>(gpr);
        }
        /// <summary>
        /// 根据截图/快照/拍照信息的所属表,和所属表的id 获取快照
        /// </summary>
        /// <param name="parentTable"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<AMDM_Snapshot_data> GetSnapshots(SnapshotParentTypeEnum parentType, long parentId,
            //parentType这个类型是否在表中用int来保存的
            bool parentTypeIsInt=true)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Snapshot;
            gpr.SetFields("*");
            if (parentTypeIsInt)
            {
                gpr.WhereEquals.Add("parentType", (int)parentType);
            }
            else
            {
                gpr.WhereEquals.Add("parentType", parentType.ToString());
            }
            gpr.WhereEquals.Add("parentId", parentId);

            return base.GetDatas<AMDM_Snapshot_data>(gpr);
        }

        /// <summary>
        /// 用基础的数据添加一个快照.注意这只是像数据库中添加了一条记录,在此之前或者之后应该把图片存储到本地硬盘上.
        /// </summary>
        /// <param name="parentTalbe"></param>
        /// <param name="parentId"></param>
        /// <param name="location"></param>
        /// <param name="time"></param>
        /// <param name="because"></param>
        /// <param name="localFilePath">保存文件位置的绝对路径</param>
        /// <returns></returns>
        public AMDM_Snapshot_data AddSnapshot(
            SnapshotParentTypeEnum parentType,
            long parentId,
            SnapshotTimePointEnum timePoint,
            SnapshotLocationEnum location, 
            DateTime time, 
            string because, 
            string localFilePath)
        {
            AMDM_Snapshot_data snap = new AMDM_Snapshot_data()
            {
               TimePoint = timePoint,
                Because = because,
                LocalFilePath = localFilePath,
                Location = location,
                ParentId = parentId,
                ParentType = parentType,
                Time = time
            };
            SqlInsertRecordParamsV2<AMDM_Snapshot_data> pr = new SqlInsertRecordParamsV2<AMDM_Snapshot_data>(TableName_Snapshot, snap, "*", "id", "parentType", "location,timepoint");
            snap.Id = base.InsertDataV2(pr);
            if (snap.Id >0)
            {
                return snap;
            }
            else
            {
                Utils.LogError("在数据操作中,AddSnapshot失败:", snap);
                return null;
            }
        }

        #region 以进出库记录为主导,附带凭证图且凭证图为打开数据的结构获取
        public List<AMDM_ClipInOutRecordSnap> GetClipInOutRecordSnap(Nullable<int> stockIndex,
            Nullable<int> floorIndex,
            Nullable<int> gridIndex,
            Nullable<DateTime> start,
            Nullable<DateTime> end,
            int pageSize,
            int pageNum,
            Nullable<bool> orderByTimeDescMode,
            out bool hasNext
            )
        {
            #region 对于时间短的参数构建
            StringBuilder timeCMD = new StringBuilder();
            if (start!= null & end!= null)
            {
                timeCMD.AppendFormat(@" WHERE
	inandoutrecord.OrderByTime >= '{0}' AND inandoutrecord.OrderByTime<={1}", start.Value.ToString("yyyy-MM-dd HH:mm:ss"), end.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            #endregion
            #region whwer对于位置的构建
            StringBuilder whereInCMD = new StringBuilder();
            StringBuilder whereOutCMD = new StringBuilder();
            if (stockIndex!= null)
            {
                whereInCMD.AppendFormat(" WHERE ind.stockindex={0}", stockIndex);
                whereOutCMD.AppendFormat(" WHERE outd.stockindex={0}", stockIndex);
                if (floorIndex != null)
                {
                    whereInCMD.AppendFormat(" AND ind.floorindex={0}", floorIndex);
                    whereOutCMD.AppendFormat(" AND outd.floorindex={0}", floorIndex);
                    if (gridIndex != null)
                    {
                        whereInCMD.AppendFormat(" AND ind.gridindex={0}", gridIndex);
                        whereOutCMD.AppendFormat(" AND outd.gridindex={0}", gridIndex);
                    }
                }
            }


            #endregion
            #region cmd构建

            StringBuilder cmd = new StringBuilder(
                string.Format(@"
SELECT
	*
FROM
	(
		SELECT
			'In' AS InOrOut,
			ind.InstockTime AS OrderByTime,
			inp.id AS InRecordId,
			ind.id AS InRecordDetailId,
			ind.MedicineName AS InDetailMedicineName,
			ind.Count AS InDetailCount,
			ind.InstockTime AS InDetailTime,
			ind.Memo AS InDetailMemo,
			inp.NurseId AS InAccountId,
			'护士名字信息' AS InAccountName,
			inp.Memo AS InMemo,
			inp.Type AS InType,
			inp.Canceled AS InCanceled,
			NULL AS `OutRecordId`,
			NULL AS `OutRecordDetailId`,
			NULL AS `OutDetailMedicineName`,
			NULL AS `OutDetailCount`,
			NULL AS `OutObjectId`,
			NULL AS `OutStartTime`,
			NULL AS `OutEndTime`,
			NULL AS `OutIsError`,
			NULL AS `OutErrMsg`,
			NULL AS `OutPrescriptionId`,
			NULL AS `OutCanceled`,
			NULL AS `OutPatientName`,
			NULL AS `OutMemo`
		FROM
			{0} AS ind
		LEFT JOIN {1} AS inp ON ind.ParentId = inp.Id
		{7} 
		UNION
			SELECT
				'Out',
				outd.StartTime,
				NULL,
				NULL,
				NULL,
				NULL,
				NULL,
				NULL,
				NULL,
				NULL,
				NULL,
				NULL,
				NULL,
				`outp`.`Id`,
				`outd`.`Id`,
				`outd`.`MedicineName`,
				`outd`.`Count`,
				`outd`.`MedicineObjectId`,
				`outd`.`StartTime`,
				`outd`.`EndTime`,
				`outd`.`IsError`,
				`outd`.`ErrMsg`,
				`outp`.`PrescriptionId`,
				`outp`.`Canceled`,
				`outp`.`PatientName`,
				`outp`.`Memo`
			FROM
				`{2}` `outd`
			LEFT JOIN `{3}` `outp` ON `outd`.`ParentId` = `outp`.`Id`
			{8} 
	) AS inandoutrecord
LEFT JOIN {4} AS s ON (
	(
		inandoutrecord.InRecordDetailId = s.ParentId
		AND s.ParentType = 4
	)
	OR (
		inandoutrecord.OutRecordDetailId = s.ParentId
		AND s.ParentType = 2
	)
)
{9} 
ORDER BY
	inandoutrecord.OrderByTime {10}
LIMIT {5},{6}
",
 TableName_InstockRecordDetail,
 TableName_InstockRecord,
 TableName_DeliveryRecordDetail,
 TableName_DeliveryRecord,
 TableName_Snapshot,
 pageNum*pageSize,
 pageSize+1,
 whereInCMD,
 whereOutCMD,
 timeCMD,
 (orderByTimeDescMode != null && orderByTimeDescMode.Value == true)? "DESC" : "ASC"
               ) );

            #endregion
            List<AMDM_ClipInOutRecordSnap> records = base.GetDatas<AMDM_ClipInOutRecordSnap>(cmd.ToString());
            if (records.Count>pageSize)
            {
                hasNext = true;
                records.RemoveAt(records.Count - 1);
            }
            else
            {
                hasNext = false;
            }
            return records;
        }
        #endregion
        #endregion

        #region 2022年1月15日22:10:23 实物装载的药品信息的增删改查,方便管理每一个药品实例的相关信息 如生产日期
        /// <summary>
        /// 实物装载的药品信息的增删改查,方便管理每一个药品实例的相关信息 如生产日期,或者是检索药品已经装进来多久了需要处理了之类的
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <param name="floorIndex"></param>
        /// <param name="gridIndex"></param>
        /// <param name="medicineId"></param>
        /// <param name="instockTime"></param>
        /// <param name="inStockRecordId"></param>
        /// <param name="productionDate"></param>
        /// <param name="expirationDate"></param>
        /// <returns></returns>
        public AMDM_MedicineObject_data AddMedicineObject(int stockIndex, int floorIndex, int gridIndex,
            long medicineId, DateTime instockTime, long inStockRecordId,
            Nullable<DateTime> productionDate, Nullable<DateTime> expirationDate)
        {
            AMDM_MedicineObject_data ret = new AMDM_MedicineObject_data()
            {
                StockIndex = stockIndex,
                FloorIndex = floorIndex,
                GridIndex = gridIndex,
                ExpirationDate = expirationDate,
                InStockRecordId = inStockRecordId,
                InStockTime = instockTime,
                MedicineId = medicineId,
                ProductionDate = productionDate
            };
            SqlInsertRecordParamsV2<AMDM_MedicineObject_data> pr = new SqlInsertRecordParamsV2<AMDM_MedicineObject_data>(
                TableName_MedicineObject, ret, "*", "id", null, null);
           ret.Id = base.InsertDataV2(pr);
           if (ret.Id>0)
           {
               return ret;
           }
           return null;
        }

        /// <summary>
        /// 删除药品实体,药品被取出仓时,是使用update来更新,而药品一旦出仓超过设定的天数,就需要删除这条记录,防止数据冗余
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteMedicineObject(long id)
        {
            SqlDeleteRecordParams dpr = new SqlDeleteRecordParams();
            dpr.TableName = TableName_MedicineObject;
            dpr.WhereEquals.Add("id", id);
            return base.DeleteData(dpr) > 0;
        }
        /// <summary>
        /// 更新药品实体,药品实体就像上膛的子弹,一旦入仓就不能修改了,这个更新只是让这一发子弹 发射出去的意思
        /// </summary>
        /// <param name="id"></param>
        /// <param name="outStockRecordId"></param>
        /// <param name="outStockTime"></param>
        /// <returns></returns>
        public bool UpdateMedicineObject(long id, long outStockRecordId, DateTime outStockTime)
        {
            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = TableName_MedicineObject;
            upr.WhereEquals.Add("id", id);
            upr.UpdateFieldNameAndValues.Add("outstockrecordid", outStockRecordId);
            upr.UpdateFieldNameAndValues.Add("outstocktime", outStockTime);

            return base.UpdateData(upr) > 0;
        }
        /// <summary>
        /// 获取指定药槽位置上的所有药品集合(可以取的,outstockrecordid为null的)
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <param name="floorIndex"></param>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        public List<AMDM_MedicineObject_data> GetMedicinesObject(int stockIndex, int floorIndex, int gridIndex)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_MedicineObject;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("stockindex", stockIndex);
            gpr.WhereEquals.Add("floorindex", floorIndex);
            gpr.WhereEquals.Add("gridindex", gridIndex);
            gpr.SpecialWhereCommands.Add("outstockrecordid is null");
            gpr.OrderByColumn = "id";
            gpr.Sort = "ASC";

            List<AMDM_MedicineObject_data> ret = base.GetDatas<AMDM_MedicineObject_data>(gpr);
            return ret;
        }
        //2022年1月16日10:45:13还需要在入库的时候提示 是否有日期比当前选择的日期要快过期的 所以这里要整理一下逻辑
        /// <summary>
        /// 获取指定药品的合集,也就是可以取的药品的集合,根据入库日期排序,也就是根据id排序 
        /// 2022年2月16日14:39:27 修改名字上增加了CanDelivery,说明是可以取的药品.也就是没卡槽也没有被交付的
        /// </summary>
        /// <param name="medicineId"></param>
        /// <param name="sortMode"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<AMDM_MedicineObject_data> GetCanDeliveryMedicinesObject(long medicineId,GetMedicinesObjectSortModeEnum sortMode, Nullable<int> count)
        {
            #region 2022年2月16日14:25:50 从即刻起,已经卡药的药槽(弹夹)不能再次取药,只有当强制清空药槽以后才可以继续使用
            //SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            //gpr.TableName = TableName_MedicineObject;
            //gpr.SetFields("*");
            //gpr.WhereEquals.Add("medicineid", medicineId);
            //gpr.SpecialWhereCommands.Add("outstockrecordid is null");
            //gpr.MaxRecordCount = count;
            //gpr.StartIndex = 0;
            //switch (sortMode)
            //{
            //    case GetMedicinesObjectSortModeEnum.ExpirationDateAsc:
            //        gpr.OrderByColumn = "ExpirationDate";
            //        gpr.Sort = "ASC";
            //        break;
            //    case GetMedicinesObjectSortModeEnum.ExpirationDateDesc:
            //        gpr.OrderByColumn = "ExpirationDate";
            //        gpr.Sort = "DESC";
            //        break;
            //    case GetMedicinesObjectSortModeEnum.ObjectIdAsc:
            //        gpr.OrderByColumn = "id";
            //        gpr.Sort = "ASC";
            //        break;
            //    case GetMedicinesObjectSortModeEnum.ObjectIdDesc:
            //        gpr.OrderByColumn = "id";
            //        gpr.Sort = "DESC";
            //        break;
            //    default:
            //        break;
            //}


            //List<AMDM_MedicineObject_data> ret = base.GetDatas<AMDM_MedicineObject_data>(gpr);
            //return ret;
            #endregion
            #region 新版,支持获取弹夹是否卡药

            //如果使用到期日期排序的话,需要先根据id排序,因为如果没有指定生产日期排序条件的时候,要保持先进先出(先进的id小)
            string orderByIdMode = null;
            //生产日期排序的where字符串
            string orderByExpDateWhereStr = null;
            switch (sortMode)
            {
                case GetMedicinesObjectSortModeEnum.ExpirationDateAsc:
                    orderByIdMode = "ASC";
                    orderByExpDateWhereStr = ",ExpirationDate ASC";
                    break;
                case GetMedicinesObjectSortModeEnum.ExpirationDateDesc:
                    orderByIdMode = "ASC";
                    orderByExpDateWhereStr = ",ExpirationDate DESC";
                    break;
                case GetMedicinesObjectSortModeEnum.ObjectIdAsc:
                    orderByIdMode = "ASC";
                    break;
                case GetMedicinesObjectSortModeEnum.ObjectIdDesc:
                    orderByIdMode = "DESC";
                    break;
                default:
                    break;
            }
            StringBuilder cmd = new StringBuilder();
            cmd.AppendFormat(

                @"
SELECT
	m.*, b.stuck
FROM
	{0} m
LEFT JOIN {1} AS b ON (
	m.StockIndex = b.StockIndex
	AND m.FloorIndex = b.FloorIndex
	AND m.GridIndex = b.GridIndex
)
WHERE
	m.MedicineId = {2}
AND Stuck=0
AND OutStockRecordId IS NULL
ORDER BY
    m.Id {3} 
	{4}
",
    TableName_MedicineObject, 
    TableName_Binding, 
    medicineId, 
    orderByIdMode,
    orderByExpDateWhereStr
    );
            if (count!= null)
            {
                cmd.AppendFormat(" LIMIT 0, {0}", count);
            }
            
            List<AMDM_MedicineObject_data> ret = base.GetDatas<AMDM_MedicineObject_data>(cmd.ToString());
            return ret;
            #endregion
        }

        public List<AMDM_MedicineObject__Grid>GetMedicinesObjectWithGrid(long medicineId, GetMedicinesObjectSortModeEnum sortMode, Nullable<int> count)
        {
            StringBuilder cmd = new StringBuilder(
                );
            cmd.AppendFormat(
               @"SELECT
	*
FROM
	`{0}` o
LEFT JOIN {1} AS g ON (
	o.StockIndex = g.stockindex
	AND o.FloorIndex = g.FloorIndex
	AND o.GridIndex = g.IndexOfFloor
)
WHERE
	`medicineid` = {2}
AND outstockrecordid IS NULL",TableName_MedicineObject,TableName_Grid, medicineId);
            switch (sortMode)
            {
                case GetMedicinesObjectSortModeEnum.ExpirationDateAsc:
                    cmd.Append(" ORDER BY o.Id ASC, ExpirationDate ASC ");
                    break;
                case GetMedicinesObjectSortModeEnum.ExpirationDateDesc:
                    cmd.Append(" ORDER BY o.Id ASC, ExpirationDate DESC ");
                    break;
                case GetMedicinesObjectSortModeEnum.ObjectIdAsc:
                    cmd.Append(" ORDER BY id ASC ");
                    break;
                case GetMedicinesObjectSortModeEnum.ObjectIdDesc:
                    cmd.Append(" ORDER BY id DESC ");
                    break;
                default:
                    break;
            }

            if (count != null)
            {
                cmd.AppendFormat(" LIMIT 0,{1}", count);
            }

            return base.GetDatas<AMDM_MedicineObject__Grid>(cmd.ToString());
        }

        /// <summary>
        /// 2022年1月18日11:26:09  获取药品实体信息,包含格子基本信息和药品基本信息
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <returns></returns>
        public List<AMDM_MedicineObject__Grid__Medicine>GetMedicinesObjectWithGridAndMedicine(Nullable<int> stockIndex)
        {
            StringBuilder cmd = new StringBuilder(
                );
            cmd.AppendFormat(
               @"
SELECT
	g.StockIndex,
	g.FloorIndex,
	g.IndexOfFloor AS GridIndex,
    g.IndexOfStock,
	b.MedicineId,
	m. NAME,
	m.Barcode,
	m.Company,
	FLOOR(f.depthmm / m.boxlongMM) AS Max,
	o.*
FROM
	{0} g
LEFT JOIN {1} AS b ON (
	g.StockIndex = b.stockindex
	AND g.floorindex = b.floorindex
	AND g.IndexOfFloor = b.GridIndex
)
LEFT JOIN {2} AS m ON b.MedicineId = m.Id
LEFT JOIN {3} AS o ON (
	g.stockindex = o.stockindex
	AND g.floorindex = o.FloorIndex
	AND g.IndexOfFloor = o.GridIndex
    AND o.OutStockRecordId IS NULL
)
LEFT JOIN {4} AS f ON g.FloorId = f.id

{5}

ORDER by g.StockIndex, g.FloorIndex DESC, g.IndexOfFloor
;

", TableName_Grid,TableName_Binding,TableName_Medicine, TableName_MedicineObject,
 TableName_Floor,
 stockIndex == null? "":string.Format(" WHERE g.StockIndex={0} ",stockIndex));
            return base.GetDatas<AMDM_MedicineObject__Grid__Medicine>(cmd.ToString());
        }
        /// <summary>
        /// 2022年3月14日12:46:53  模拟测试器在测试的时候需要获取到药槽是否已经卡住的信息.所以又加上了clip字段
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <returns></returns>
        public List<AMDM_MedicineObject__Clip__Grid__Medicine> GetMedicinesObjectWithClipStuckAndGridAndMedicine(Nullable<int> stockIndex)
        {
            StringBuilder cmd = new StringBuilder(
                );
            cmd.AppendFormat(
               @"
SELECT
	g.StockIndex,
	g.FloorIndex,
	g.IndexOfFloor AS GridIndex,
    g.IndexOfStock,
	b.MedicineId,
    b.Stuck,
    m.IdOfHIS,
	m. NAME,
	m.Barcode,
	m.Company,
	FLOOR(f.depthmm / m.boxlongMM) AS Max,
	o.*
FROM
	{0} g
LEFT JOIN {1} AS b ON (
	g.StockIndex = b.stockindex
	AND g.floorindex = b.floorindex
	AND g.IndexOfFloor = b.GridIndex
)
LEFT JOIN {2} AS m ON b.MedicineId = m.Id
LEFT JOIN {3} AS o ON (
	g.stockindex = o.stockindex
	AND g.floorindex = o.FloorIndex
	AND g.IndexOfFloor = o.GridIndex
    AND o.OutStockRecordId IS NULL
)
LEFT JOIN {4} AS f ON g.FloorId = f.id
{5}

ORDER by g.StockIndex, g.FloorIndex DESC, g.IndexOfFloor
;

", TableName_Grid, 
 TableName_Binding, 
 TableName_Medicine, 
 TableName_MedicineObject,
 TableName_Floor,
 stockIndex == null ? "" : string.Format(" WHERE g.StockIndex={0} ", stockIndex)
 );
            return base.GetDatas<AMDM_MedicineObject__Clip__Grid__Medicine>(cmd.ToString());
        }
        #endregion

        #region 2022年3月29日12:47:00 根据药品的时间获取药品对象

        /// <summary>
        /// 根据有效期信息获取所有没出库的将要过期的药品,如果药品信息中没有保存剩余多少天提醒,使用给定的默认值进行计算
        /// </summary>
        /// <param name="defaultMinExpirationDays">如果药品本身没有设定有效期剩余多少天的时候提醒,使用这个默认的天数来进行计算.符合条件的返回</param>
        /// <returns></returns>
        public List<AMDM_MedicineObject__Grid__Medicine> GetMedicineObjectsByExpirationLimit(int defaultMinExpirationDays)
        {
            string cmd = string.Format(@"
SELECT
	o.*,g.*,m.*
FROM
	{0} AS o
LEFT JOIN {1} AS g ON o.StockIndex = g.StockIndex AND o.FloorIndex = g.FloorIndex AND o.GridIndex = g.IndexOfFloor
LEFT JOIN {2} AS m ON o.MedicineId = m.Id
WHERE
	OutStockRecordId IS NULL
AND DATEDIFF(ExpirationDate, now()) < IFNULL(m.DTOEA, {3});
",
 TableName_MedicineObject,
 TableName_Grid,
 TableName_Medicine,
 defaultMinExpirationDays
 );
            List<AMDM_MedicineObject__Grid__Medicine> ret = base.GetDatas<AMDM_MedicineObject__Grid__Medicine>(cmd);
            return ret;
        }
        #endregion

        #region 2022年3月29日19:11:18 获取低库存的药品信息
        /// <summary>
        /// 获取低库存药品,如果药品本身有库存设置阈值,根据阈值计算,如果没有阈值,阈值设置成给定的默认阈值
        /// </summary>
        /// <param name="defaultMinCountThredhold"></param>
        /// <returns></returns>
        public List<AMDM_MedicineInventory> GetLowInventoryMedicines(int defaultMinCountThredhold)
        {
            string cmd = string.Format(
                @"
SELECT
	*
FROM
	(
		SELECT
			count(*) AS Count,
			m.*
		FROM
			{0} AS o
		LEFT JOIN {1} AS c ON o.StockIndex = c.StockIndex
		AND o.FloorIndex = c.FloorIndex
		AND o.GridIndex = c.GridIndex
		LEFT JOIN {2} AS m ON o.MedicineId = m.Id
		WHERE
			o.OutStockRecordId IS NULL
		AND c.Stuck = 0
		GROUP BY
			m.id
	) AS ret
WHERE
	ret.Count < IFNULL(ret.CTOLIA, {3})
", TableName_MedicineObject, TableName_Binding, TableName_Medicine,defaultMinCountThredhold
                );
            List<AMDM_MedicineInventory> ret = base.GetDatas<AMDM_MedicineInventory>(cmd);
            return ret;
        }
        #endregion

        #region 日志查询
        /// <summary>
        /// 获取符合条件的日志信息的总条数
        /// </summary>
        /// <param name="level"></param>
        /// <param name="titleTag"></param>
        /// <param name="messageTag"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public long GetLogsTotalResultCount(Nullable<int> level, string titleTag, string messageTag, Nullable<DateTime> start, Nullable<DateTime> end)
        {
            if (level < 0 || level > 4)
            {
                return 0;
            }
            string spanCmd = "";
            string titleTagCmd = "";
            string levelStr = "";
            bool hasWhere = false;

            if (level != null)
            {
                levelStr = string.Format(" LEVEL = {0} ", level);
                hasWhere = true;
            }
            if (start != null && end != null)
            {
                spanCmd = string.Format(" {0} time>='{1}' AND time<='{2}'",hasWhere?"AND":null, start.Value.ToString("yyyy:MM:dd HH:mm:ss"), end.Value.ToString("yyyy:MM:dd HH:mm:ss"));
                hasWhere = true;
            }
            if (string.IsNullOrEmpty(titleTag) == false)
            {
                titleTagCmd = string.Format(" {0} title like '%{1}%'",hasWhere?"AND":null, titleTag);
                hasWhere = true;
            }
            string cmd = string.Format(
                @"SELECT count(*) FROM `{0}`.`{1}` {5} {2} {3} {4}", "amdm_log_server", "logrecord", levelStr, spanCmd, titleTagCmd, hasWhere?" WHERE ": null
                );
            var longs = base.GetValues<long>(cmd);
            if (longs!= null && longs.Count == 1)
            {
                return longs[0];
            }
            return 0;
        }
        /// <summary>
        /// 获取日志信息,返回的结果会比给定的单页大小多一条(如果有下一页的话)
        /// </summary>
        /// <param name="level"></param>
        /// <param name="titleTag"></param>
        /// <param name="messageTag"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetLogs(Nullable<int> level, string titleTag, string messageTag, Nullable<DateTime> start, Nullable<DateTime> end, Nullable<int> pageIndex, Nullable<int> pageSize)
        {
            if (level<0 || level>4)
            {
                return new List<Dictionary<string, object>>();
            }
            if (pageIndex == null)
            {
                pageIndex = 0;
            }
            if(pageSize == null)
            {
                pageSize = 10;
            }
            if (pageIndex<0)
            {
                return new List<Dictionary<string, object>>();
            }
            if (pageSize<0 || pageSize>100)
            {
                pageSize = 100;
            }
            string spanCmd = "";
            string titleTagCmd = "";
            string levelCmd = "";
            bool hasWhere = false;
            if (level != null)
            {
                levelCmd = string.Format(" LEVEL = {0} ", level);
                hasWhere = true;
            }
            if (start!= null && end != null)
            {
                spanCmd = string.Format(" {0} time>='{1}' AND time<='{2}'",hasWhere?"AND":null, start.Value.ToString("yyyy:MM:dd HH:mm:ss"), end.Value.ToString("yyyy:MM:dd HH:mm:ss"));
                hasWhere = true;
            }
            if (string.IsNullOrEmpty(titleTag) == false)
            {
                titleTagCmd = string.Format(" {0} title like '%{1}%'",hasWhere?"AND":null, titleTag);
                hasWhere = true;
            }
            
            string cmd = string.Format(
                @"
SELECT
	id,
	`level`,
	time,
	title,
	message
FROM
	`{0}`.`{1}`
{7}
{2}
{3} 
{4} 
ORDER BY ID DESC
LIMIT {5},
 {6}
", "amdm_log_server", "logrecord", levelCmd, spanCmd, titleTagCmd, pageIndex * pageSize, pageSize + 1, hasWhere ? " WHERE " : null
                );
            List<Dictionary<string,object>> ret = base.GetDatas(cmd);
            return ret;
        }
        #endregion

        #region 获取今天的已取药处方和已取药数量数据
        public long GetTodayPrescriptionCount()
        {
            string cmd = string.Format(
                @"
SELECT
	count(*) AS TotayPrescriptionId
FROM
	(
		SELECT
			*
		FROM
			{0}
		WHERE
			StartTime >= '{1}'
		AND StartTime <= '{2}'
		GROUP BY
			PrescriptionId
	) AS t;
",
  TableName_DeliveryRecord  ,DateTime.Today.ToString("yyyy-MM-dd"), DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));
            var vals = base.GetValues<long>(cmd);
            if (vals!= null && vals.Count == 1)
            {
                return vals[0];
            }
            return 0;
        }
        public long GetTodayMedicineCount()
        {
            string cmd = string.Format(
                @"
SELECT
	SUM(t.count) AS TotayMedicineId
FROM
	(
		SELECT
			id,
			count
		FROM
			{0}
		WHERE
			StartTime >= '{1}'
		AND StartTime <= '{2}'
	) AS t;
",
  TableName_DeliveryRecordDetail, DateTime.Today.ToString("yyyy-MM-dd"), DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));
            var vals = base.GetValues<int>(cmd);
            if (vals != null && vals.Count == 1)
            {
                return vals[0];
            }
            return 0;
        }
        #endregion
    }
}
