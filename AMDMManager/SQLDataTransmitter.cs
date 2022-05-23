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
        #endregion

        #endregion
        #region 药品和格子信息的增删改查
        /// <summary>
        /// 增加绑定信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool AddBindingInfo(GridMedicineBindingInfo_data info)
        {
            SqlInsertRecordParams ipr = new SqlInsertRecordParams();
            ipr.TableName = TableName_Binding;
            ipr.DataObject = info;
            info.Id = base.InsertData<GridMedicineBindingInfo_data>(ipr);
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
        public bool UpdateBindingInfo(GridMedicineBindingInfo_data info)
        {
            return false;
        }
        /// <summary>
        /// 更新药品绑定表中存储的药品的库存信息
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <param name="floorIndex"></param>
        /// <param name="gridIndex"></param>
        /// <param name="inOutCount"></param>
        /// <returns></returns>
        public bool UpdateBindingInfoSavedInventoryInfoField(int stockIndex, int floorIndex, int gridIndex, int inOutCount)
        {
            //根据出入的多少 自动判断是更新最后入库时间字段还是最后出库时间字段
            string inOutSetCMD = null;
            if (inOutCount>0)
            {
                inOutSetCMD = string.Format(",lastinstocktime='{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else if(inOutCount<0)
            {
                inOutSetCMD = string.Format(",lastdeliverytime='{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            string cmd = string.Format("update {0} set currentinventory=currentinventory+{4}{5} where stockindex={1} and floorindex={2} and gridindex={3}",
                TableName_Binding, stockIndex, floorIndex, gridIndex, inOutCount,
                inOutSetCMD
                );
            return base.MysqlExecute(cmd) > 0;
        }
        /// <summary>
        /// 获取在绑定信息表中存储的库存信息,如果获取失败会返回null
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <param name="floorIndex"></param>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        public Nullable<int> GetBindingInfoSavedInventoryCount(int stockIndex, int floorIndex, int gridIndex)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.WhereEquals.Add("stockindex", stockIndex);
            gpr.WhereEquals.Add("floorindex", floorIndex);
            gpr.WhereEquals.Add("gridindex", gridIndex);
            gpr.SetFields("*");

            List<GridMedicineBindingInfo_data> bindings = base.GetDatas<GridMedicineBindingInfo_data>(gpr);
            if (bindings.Count == 1)
            {
                return bindings[0].CurrentInventory;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 更新呢药品绑定表中存储的药品的库存信息,并设置为0 这个操作是危险的 通常是初始化药库的时候使用
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <param name="floorIndex"></param>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        public bool UpdateBindingInfoSavedInventoryInfoFieldSetZero(int stockIndex, int floorIndex, int gridIndex)
        {
            string cmd = string.Format("update {0} set currentinventory=0 where stockindex={1} and floorindex={2} and gridindex={3}", TableName_Binding, stockIndex, floorIndex, gridIndex);
            return base.MysqlExecute(cmd) > 0;
        }
        /// <summary>
        /// 获取绑定信息
        /// </summary>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public GridMedicineBindingInfo_data GetBindingInfo(int infoId)
        {
            return null;
        }

        /// <summary>
        /// 根据药品的id获取已经绑定的该药品的所有药槽
        /// </summary>
        /// <param name="medicineId"></param>
        /// <returns></returns>
        public List<GridMedicineBindingInfo_data> GetBindingsInfoList(long medicineId)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.WhereEquals.Add("medicineid", medicineId);
            gpr.SetFields("*");

            return base.GetDatas<GridMedicineBindingInfo_data>(gpr);
        }
        /// <summary>
        /// 根据药品的id集合,获取已经绑定的这些药品的所有药槽
        /// </summary>
        /// <param name="medicineIdsList"></param>
        /// <returns></returns>
        public List<GridMedicineBindingInfo_data> GetBindingsInfoList(List<long> medicineIdsList)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.SetFields("*");
            gpr.WhereInInfos.Add("medicineid", medicineIdsList);

            return base.GetDatas<GridMedicineBindingInfo_data>(gpr);
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
        public GridMedicineBindingInfo_data GetBindingInfo(int stockIndex, int floorIndex, int gridIndex)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("stockindex", stockIndex);
            gpr.WhereEquals.Add("floorindex", floorIndex);
            gpr.WhereEquals.Add("gridIndex", gridIndex);

            List<GridMedicineBindingInfo_data> bindings = base.GetDatas<GridMedicineBindingInfo_data>(gpr);
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
        public List<GridMedicineBindingInfo_data> GetBindingInfos4Stock(int stockIndex)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.WhereEquals.Add("stockindex", stockIndex);
            gpr.SetFields("*");

            return base.GetDatas<GridMedicineBindingInfo_data>(gpr);
        }
        /// <summary>
        /// 获取整个药机的所有的药品绑定信息
        /// </summary>
        /// <returns></returns>
        public List<GridMedicineBindingInfo_data> GetBindingInfos4WholeMachine()
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.SetFields("*");

            return base.GetDatas<GridMedicineBindingInfo_data>(gpr);
        }
        /// <summary>
        /// 获取药仓中绑定的药品信息,如果药仓需要重新初始化,必须要检查是否有药品绑定到此药仓.
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <returns></returns>
        public List<GridMedicineBindingInfo_data> GetStockBindingInfo(int stockIndex)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_Binding;
            gpr.WhereEquals.Add("stockindex", stockIndex);
            gpr.SetFields("*");

            List<GridMedicineBindingInfo_data> infos = base.GetDatas<GridMedicineBindingInfo_data>(gpr);
            return infos;
        }
        /// <summary>
        /// 获取绑定信息,通过药品的条码
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public GridMedicineBindingInfo_data GetBindingInfoByMedicine(string barcode)
        {
            return null;
        }
        /// <summary>
        /// 获取绑定信息,通过层格位置的索引
        /// </summary>
        /// <param name="yxLocation"></param>
        /// <returns></returns>
        public GridMedicineBindingInfo_data GetBindingInfo(Point yxLocation)
        {
            return null;
        }
        /// <summary>
        /// 获取绑定信息,通过药品id,看药品在什么位置
        /// </summary>
        /// <param name="medicineId"></param>
        /// <returns></returns>
        public List<GridMedicineBindingInfo_data> GetFulfillAbleGridBindingsOrderByInstockTimeAsc(long medicineId)
        {
            //SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            //gpr.TableName = TableName_Binding;
            //gpr.SetFields("*");
            //gpr.WhereEquals.Add("medicineid", medicineId);
            //gpr.WhereMoreThan.Add("currentinventory", 0);
            //gpr.Sort = "ASC";
            //gpr.OrderByColumn = "lastdeliverytime";

            string cmd = string.Format("select * from {0} where medicineid={1} and currentinventory>0 order by lastdeliverytime asc", TableName_Binding, medicineId);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(cmd);
            Console.ResetColor();
            List<GridMedicineBindingInfo_data> bindings = base.GetDatas<GridMedicineBindingInfo_data>(cmd);
            Console.WriteLine("获取到的绑定信息数量:{0}", bindings.Count);
            return bindings;
        }
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
        /// 检查本机是否含有药品数据,获取含有的药品数量
        /// </summary>
        /// <returns></returns>
        public long GetMedicinesKindCount()
        {
            SqlValueGeterParameters gpr = new SqlValueGeterParameters();
            gpr.TableName = TableName_Medicine;
            gpr.Field = "*";
            gpr.IsFuncField = true;
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
        /// 删除数据库中的所有药品信息,谨慎使用.
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllMedicines()
        {
            return base.MysqlExecute(string.Format("truncate table {0}", TableName_Medicine)) > 0;
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
        public bool InsertDeliveryRecordDetail(AMDM_DeliveryRecordDetail detail)
        {
            if (detail.ParentId <=0)
            {
                Console.WriteLine("需要给定药品交付记录表中的记录的父表的id");
                return false;
            }
            SqlInsertRecordParams ipr = new SqlInsertRecordParams();
            ipr.TableName = TableName_DeliveryRecordDetail;
            ipr.DataObject = detail;

            detail.Id = base.InsertData<AMDM_DeliveryRecordDetail>(ipr);
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
        public List<AMDM_MedicineInventory>GetMedicinesInventory(Nullable<int> stockIndex, string fields)
        {
            string andCmd = null;
            if (stockIndex!= null)
            {
                andCmd = string.Format(" and b.StockIndex={0}", stockIndex);
            }
            StringBuilder fieldsSB = new StringBuilder("m.*");
            if (fields!= null)
            {
                string[] fs = fields.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries);
                if (fs!= null && fs.Length>0)
                {
                    fieldsSB = new StringBuilder();
                    foreach (var item in fs)
                    {
                        string field = item.Replace(",", "");
                        if (fieldsSB.Length>0)
                        {
                            fieldsSB.Append(",");
                        }
                        fieldsSB.AppendFormat("m.{0}", field);
                    }
                }
            }
            
            string cmd = string.Format("select {3},b.CurrentInventory as count from {0} b, {1} m where b.MedicineId = m.id{2};",
                TableName_Binding,
                TableName_Medicine, 
                andCmd,
                fieldsSB
                );
            List<AMDM_MedicineInventory> infos = base.GetDatas<AMDM_MedicineInventory>(cmd);
            return infos;
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
        public List<AMDM_DeliveryRecordDetail> GetDeliveryRecordDetails(long recordId)
        {
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = TableName_DeliveryRecordDetail;
            gpr.SetFields("*");
            gpr.WhereEquals.Add("parentid", recordId);

            return base.GetDatas<AMDM_DeliveryRecordDetail>(gpr);
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
    }
}
