using DomainBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMDMClientSDK.Domain
{
    #region response
    /// <summary>
    /// 获取药品信息
    /// </summary>
    public class GridsGetResponse : NetResponse
    {
        public List<AMDM_Domain.AMDM_Grid__Clip__Medicine> Grids { get; set; }
    }
    #endregion

    #region request
    /// <summary>
    /// 获取药品信息
    /// </summary>
    public class GridsGetRequest : BaseRequest<GridsGetResponse>
    {
        public GridsGetRequest()
        {
            //this.Fields = "*";
        }
        public Nullable<int> StockIndex { get; set; }
        public Nullable<int> FloorIndex { get; set; }
        public Nullable<int> GridIndex { get; set; }
        /// <summary>
        /// 要获取的字段,默认是* 或者是all
        /// </summary>
        //public string Fields { get; set; }
        
        public override string GetApiName()
        {
            return "grids.get";
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
            if (this.StockIndex == null)
            {
                throw new ArgumentNullException("药仓索引");
            }
        }
    }
    #endregion
}
