using DomainBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMDMClientSDK.Domain
{
    #region response
    /// <summary>
    /// 获取交付记录信息
    /// </summary>
    public class DeliveryRecordDeleteResponse : NetResponse
    {
        
    }
    #endregion

    #region request
    /// <summary>
    /// 获取交付记录信息
    /// </summary>
    public class DeliveryRecordDeleteRequest : BaseRequest<DeliveryRecordDeleteResponse>
    {
        public DeliveryRecordDeleteRequest()
        {
        }
        public Nullable<long> Id { get; set; }
        public override string GetApiName()
        {
            return "deliveryrecord.delete";
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
            if (this.Id == null)
            {
                throw new ArgumentException("必须指定要删除的付药单ID");
            }
        }
    }
    #endregion
}
