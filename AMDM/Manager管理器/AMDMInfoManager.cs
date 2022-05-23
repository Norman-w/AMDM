using AMDM.Manager;
using AMDM_Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


    #region 类型定义
    public enum MoveDirectionEnum { LeftWall2Left, RightWall2Right, LeftWall2Right, RightWall2Left, RemoveLeftWall, RemoveWriteWall };
    #endregion
    /// <summary>
    /// 药机实体库自身信息管理器
    /// </summary>
    public class AMDMStockLoader
    {
        public AMDMStockLoader(SQLDataTransmitter sqlClient)
        {
            this.client = sqlClient;
        }
        

        #region 从数据库加载Stock信息,并且加载全部的floors和grids信息
        /// <summary>
        /// 读取指定stock的层和药槽信息.
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <returns></returns>
        public AMDM_Stock LoadStock(int stockIndex)
        {
            AMDM_Stock loadedStock = new AMDM_Stock();
            AMDM_Stock_data stock_data = client.GetStock(stockIndex);
            if (stock_data == null)
            {
                return null;
            }
            string stockJson = JsonConvert.SerializeObject(stock_data);
            JsonConvert.PopulateObject(stockJson, loadedStock);
            loadedStock.Floors = new Dictionary<int, AMDM_Floor>();
            //读取层信息
            List<AMDM_Floor_data> floorsData = client.GetFloors(loadedStock.Id);
            string floorsJson = JsonConvert.SerializeObject(floorsData);
            List<AMDM_Floor> floors = new List<AMDM_Floor>();
            JsonConvert.PopulateObject(floorsJson, floors);
            //读取格子信息
            foreach (var floor in floors)
            {
                int floorIndex = floor.IndexOfStock;
                loadedStock.Floors.Add(floorIndex, floor);
                int currentFloorId = floor.Id;
                List<AMDM_Grid_data> gridsData = client.GetGrids(currentFloorId);
                string gridsJson = JsonConvert.SerializeObject(gridsData);
                floor.Grids = new List<AMDM_Grid>();
                JsonConvert.PopulateObject(gridsJson, floor.Grids);
            }
            return loadedStock;
        }
        /// <summary>
        /// 读取数据库中保存的所有药仓
        /// </summary>
        /// <returns></returns>
        public List<AMDM_Stock> LoadStocks()
        {
            var stocks = client.GetALLStocks();
            List<AMDM_Stock> ret = new List<AMDM_Stock>();
            foreach (var item in stocks)
            {
                AMDM_Stock loadedStock = new AMDM_Stock();
                string stockJson = JsonConvert.SerializeObject(item);
                JsonConvert.PopulateObject(stockJson, loadedStock);
                loadedStock.Floors = new Dictionary<int, AMDM_Floor>();
                //读取层信息
                List<AMDM_Floor_data> floorsData = client.GetFloors(loadedStock.Id);
                string floorsJson = JsonConvert.SerializeObject(floorsData);
                List<AMDM_Floor> floors = new List<AMDM_Floor>();
                JsonConvert.PopulateObject(floorsJson, floors);
                //读取格子信息
                foreach (var floor in floors)
                {
                    int floorIndex = floor.IndexOfStock;
                    loadedStock.Floors.Add(floorIndex, floor);
                    int currentFloorId = floor.Id;
                    List<AMDM_Grid_data> gridsData = client.GetGrids(currentFloorId);
                    string gridsJson = JsonConvert.SerializeObject(gridsData);
                    floor.Grids = new List<AMDM_Grid>();
                    JsonConvert.PopulateObject(gridsJson, floor.Grids);
                }
                ret.Add(loadedStock);
            }
            return ret;
        }
        #endregion

        #region 数据交互器
        protected SQLDataTransmitter client = null;
        #endregion
    }
