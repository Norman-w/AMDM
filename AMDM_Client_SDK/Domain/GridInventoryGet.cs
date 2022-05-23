using DomainBase;
using System;
using System.Collections.Generic;
using System.Text;
using AMDM_Domain;

namespace AMDMClientSDK.Domain
{
    /// <summary>
    /// 获取某个药仓或者所有药仓的格子内的库存信息 不支持单独指定格子
    /// </summary>
    public class GridInventoryGetResponse : NetResponse
    {
        /// <summary>
        /// 格子的库存信息
        /// </summary>
        public List<AMDM_GridInventory> GridsInventory { get; set; }

        ///// <summary>
        ///// 使用树形结构保存的格子库存信息
        ///// </summary>
        //public Dictionary<int,Dictionary<int,Dictionary<int,AMDM_GridInventory>>> GridsInventoryByTree { get; set; }

        /// <summary>
        /// 2022年1月15日10:49:03 使用树状结构保存的整个机器的库存树
        /// </summary>
        public AMDM_MachineInventory Machine { get; set; }
    }

    public class GridInventoryGetRequest : BaseRequest<GridInventoryGetResponse>
    {
        public GridInventoryGetRequest()
        {
            Fields = "*";
        }
        #region 字段
        /// <summary>
        /// 药仓索引序列
        /// </summary>
        public Nullable<int> StockIndex { get; set; }

        ///// <summary>
        ///// 返回值的模式,如果使用list模式,返回AMDM_GridInventory的数组,如果使用tree模式,返回dic模式
        ///// </summary>
        //public string ResultMode { get; set; }

        /// <summary>
        /// 要获取的字段
        /// </summary>
        public string Fields { get; set; }
        #endregion
        public override string GetApiName()
        {
            return "gridinventory.all.get";
        }
        public override bool GetIsSessionRequired()
        {
            return true;
        }
        public override IDictionary<string, string> GetParameters()
        {
            return base.GetParametersDicByPublicFieldAuto(this);
        }
        public override void Validate()
        {
            if (string.IsNullOrEmpty(Fields) || Fields.Trim().Length < 1)
            {
                throw new ArgumentNullException("无效的fields字段信息");
            }
        }
    }
}
