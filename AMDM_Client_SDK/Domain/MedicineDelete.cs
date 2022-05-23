using DomainBase;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AMDMClientSDK.Domain
{
    public class MedicineDeleteResponse: NetResponse
    {
    }
    public class MedicineDeleteRequest : BaseRequest<MedicineDeleteResponse>
    {
        Nullable<Int64> _id = 0;
        /// <summary>
        /// 药品id,流水号,自生成
        /// </summary>
        public Nullable<Int64> Id { get { return _id; } set { _id = value; } }

        public override string GetApiName()
        {
            return "medicine.delete";
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
                throw new ArgumentNullException("Id");
            }
        }
    }
}
