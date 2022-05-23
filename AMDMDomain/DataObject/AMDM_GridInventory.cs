using System;
using System.Collections.Generic;
using System.Text;
/*
 * 带有数量信息的格子信息
 */
namespace AMDM_Domain
{
    /// <summary>
    /// 药槽库存数量,包含药槽的基础位置数据,药品基础数据和数量
    /// </summary>
    public class AMDM_GridInventory
    {
        public AMDM_GridInventory()
        {
            this.MedicinesObject = new List<AMDM_MedicineObject_data>();
        }
        public string IdOfHIS { get; set; }
        public int StockIndex { get; set; }
        public int FloorIndex { get; set; }
        public int GridIndex { get; set; }
        ///// <summary>
        ///// 格子在药仓中的索引
        ///// </summary>
        //public int IndexOfStock { get; set; }
        public int GridIndexOfStock { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Company { get; set; }
        public int Count { get; set; }

        /// <summary>
        /// 最大装在数量
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// 该药品所在的药槽是不是卡住了
        /// </summary>
        public bool Stuck { get; set; }

        /// <summary>
        /// 2022年1月18日10:48:27  新增的 每个格子里面都赋予他实际的已经装载的药品的信息
        /// </summary>
        public List< AMDM_MedicineObject_data> MedicinesObject { get; set; }
    }
    /// <summary>
    /// 2022年1月15日10:47:18 新增 层的库存统计信息
    /// </summary>
    public class AMDM_FloorInventory
    {
        
        public int StockIndex { get; set; }
        public int FloorIndex { get; set; }
        //public int TotalKind { get; set; }
        public int TotalCounnt { get; set; }
        public List<AMDM_GridInventory> Grids { get; set; }
    }
    //public class AMDM_FloorInventory_forsort : AMDM_FloorInventory
    //{
    //    public AMDM_FloorInventory_forsort()
    //    {
    //        this.GridsInventory = new Dictionary<int, AMDM_GridInventory>();
    //    }
    //    public Dictionary<int, AMDM_GridInventory> GridsInventory { get; set; }
    //}

    public class AMDM_StockInventory
    {
        public int StockIndex { get; set; }
        //public int TotalKind { get; set; }
        public int TotalCount { get; set; }

        public List<AMDM_FloorInventory> Floors { get; set; }
    }
    ///// <summary>
    ///// 2022年1月15日10:47:32  新增 药仓的统计信息
    ///// </summary>
    //public class AMDM_StockInventory_forsort : AMDM_StockInventory
    //{
    //    public AMDM_StockInventory_forsort()
    //    {
    //        this.FloorsInventory = new Dictionary<int, AMDM_FloorInventory_forsort>();
    //    }
    //    public Dictionary<int, AMDM_FloorInventory_forsort> FloorsInventory { get; set; }
    //}

    public class AMDM_MachineInventory
    {
        //public int TotalKind { get; set; }
        public int TotalCount { get; set; }

        public List<AMDM_StockInventory> Stocks { get; set; }
    }
    ///// <summary>
    ///// 2022年1月15日10:47:41 新增 药机的统计信息
    ///// </summary>
    //public class AMDM_MachineInventory_forsort : AMDM_MachineInventory
    //{
    //    public AMDM_MachineInventory_forsort()
    //    {
    //        this.StocksInventory = new Dictionary<int, AMDM_StockInventory_forsort>();
    //    }

    //    public Dictionary<int,AMDM_StockInventory_forsort> StocksInventory { get; set; }
    //}
}
