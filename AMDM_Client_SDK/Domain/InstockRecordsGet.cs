using DomainBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMDMClientSDK.Domain
{
    #region response
    /// <summary>
    /// 获取入库记录信息
    /// </summary>
    public class InstockRecordsGetResponse : NetResponse
    {
        /// <summary>
        /// 总共有多少条数据
        /// </summary>
        public long TotalRecordCount { get; set; }
        /// <summary>
        /// 是否还有下一页
        /// </summary>
        public bool HasNext { get; set; }
        /// <summary>
        /// 获取到的入库记录的集合
        /// </summary>
        public List<AMDM_Domain.AMDM_InstockRecord> InstockRecords { get; set; }
    }
    #endregion

    #region request
    /// <summary>
    /// 获取入库记录信息
    /// </summary>
    public class InstockRecordsGetRequest : BaseRequest<InstockRecordsGetResponse>
    {
        public InstockRecordsGetRequest()
        {
            this.Fields = "*";
            this.PageNum = 0;
            this.PageSize = 100;
            this.GetTotalRecordCount = true;
        }
        /// <summary>
        /// 必须指定要获取的字段,默认是*
        /// </summary>
        public string Fields { get; set; }
        /// <summary>
        /// 创建时间的起始时间
        /// </summary>
        public Nullable<DateTime> StartCreate { get; set; }
        /// <summary>
        /// 创建时间的结束时间
        /// </summary>
        public Nullable<DateTime> EndCreate { get; set; }

        /// <summary>
        /// 如果要指定只获取已经关闭的或者是没有被关闭的  指定这个参数 否则留空,都会获取.
        /// </summary>
        public Nullable<bool> CancelStatus { get; set; }
        /// <summary>
        /// 是否获取全部结果的数量,只有当 当前页是0的时候有效
        /// </summary>
        public Nullable<bool> GetTotalRecordCount { get; set; }
        ///// <summary>
        ///// 要获取的字段,默认是* 或者是all
        ///// </summary>
        //public string Fields { get; set; }
        /// <summary>
        /// 第几页,从0开始
        /// </summary>
        public Nullable<int> PageNum { get; set; }
        /// <summary>
        /// 单页大小,默认100,最小1,最大500(性能)
        /// </summary>
        public Nullable<int> PageSize { get; set; }
        public override string GetApiName()
        {
            return "instockrecords.get";
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
            if (string.IsNullOrEmpty(Fields) || Fields.Trim().Length<1 || 
                PageSize > 500 || PageSize < 1 || PageNum < 0)
            {
                throw new ArgumentException("无效的参数,必须给定要获取的字段参数,单页获取数量1~500之间,当前页码需>=0");
            }
            //if (this.PageNum != null && this.PageNum != 0 && this.GetTotalRecordCount == true)
            //{
            //    throw new ArgumentException("仅当获取起始页时获取总记录数有效");
            //}
            if ((StartCreate == null)^(EndCreate == null))
            {
                throw new ArgumentException("如果指定按时间搜索,则必须同时指定起始和结束时间");
            }
        }
    }
    #endregion
}
