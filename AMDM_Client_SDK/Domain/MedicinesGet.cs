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
    public class MedicinesGetResponse: NetResponse
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
        /// 获取到的药品的集合
        /// </summary>
        public List<AMDM_Domain.AMDM_Medicine> Medicines { get; set; }
    }
    #endregion

    #region request
    /// <summary>
    /// 获取药品信息
    /// </summary>
    public class MedicinesGetRequest:BaseRequest<MedicinesGetResponse>
    {
        public MedicinesGetRequest()
        {
            this.Fields = "*";
            this.PageNum = 0;
            this.PageSize = 100;
            this.GetTotalRecordCount = true;
        }
        /// <summary>
        /// 查询时指定的关键字集合,以空格分隔
        /// </summary>
        public string Tags { get; set; }
        /// <summary>
        /// 查询时指定的条码
        /// </summary>
        public string Barcode { get; set; }
        /// <summary>
        /// 是否获取全部结果的数量,只有当 当前页是0的时候有效
        /// </summary>
        public Nullable<bool> GetTotalRecordCount { get; set; }
        /// <summary>
        /// 要获取的字段,默认是* 或者是all
        /// </summary>
        public string Fields { get; set; }
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
            return "medicines.get";
        }
        public override bool GetIsSessionRequired()
        {
            return true;
        }
        public override IDictionary<string, string> GetParameters()
        {
            QPNetDictionary param = new QPNetDictionary();
            param.Add("fields", this.Fields);    
            param.Add("tags", this.Tags);
            param.Add("barcode", this.Barcode);
            param.Add("gettotalrecordcount", this.GetTotalRecordCount);
            param.Add("pagenum", this.PageNum);
            param.Add("pagesize", this.PageSize);
            param.AddAll(this.GetOtherParameters());
            return param;
        }
        public override void Validate()
        {
            if (Fields == null || PageSize>500 || PageSize <1 || PageNum <0)
            {
                throw new ArgumentException("无效的参数,需指定所需要获取的字段,单页获取数量1~500之间,当前页码需>=0");
            }
            //if (this.PageNum !=null && this.PageNum != 0 && this.GetTotalRecordCount ==  true)
            //{
            //    throw new ArgumentException("仅当获取起始页时获取总记录数有效");
            //}
        }
    }
    #endregion
}
