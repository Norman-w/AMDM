using DomainBase;
using System;
using System.Collections.Generic;
using System.Text;
using AMDM_Domain;

namespace AMDMClientSDK.Domain
{
    public class MedicineAddResponse : NetResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 新添加的药品
        /// </summary>
        public AMDM_Medicine NewMedicine { get; set; }
    }

    /// <summary>
    /// 添加药品
    /// </summary>
    public class MedicineAddRequest: BaseRequest<MedicineAddResponse>
    {
        /// <summary>
        /// 要添加的药品的json序列化后的值
        /// </summary>
        public string MedicineInfoJson { get; set; }

        public override string GetApiName()
        {
            return "medicine.add";
        }
        public override bool GetIsSessionRequired()
        {
            return true;
        }
        public override IDictionary<string, string> GetParameters()
        {
            QPNetDictionary param = new QPNetDictionary();
            param.Add("medicineinfojson", this.MedicineInfoJson);
            
            param.AddAll(this.GetOtherParameters());
            return param;
        }
        public override void Validate()
        {
            if (string.IsNullOrEmpty(MedicineInfoJson) == true)
            {
                throw new ArgumentNullException("MedicineInfoJson");
            }
        }
    }
}
