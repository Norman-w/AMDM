using DomainBase;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AMDMClientSDK.Domain
{
    public class MedicineUpdateResponse: NetResponse
    {
        public AMDM_Domain.AMDM_Medicine UpdatedMedicine { get; set; }
    }
    public class MedicineUpdateRequest: BaseRequest<MedicineUpdateResponse>
    {
        Nullable<Int64> _id = 0;
        /// <summary>
        /// 药品id,流水号,自生成
        /// </summary>
        public Nullable<Int64> Id { get { return _id; } set { _id = value; } }

        #region 2021年12月21日20:36:00  支持的更新字段集合
        string _idofhis = null;
        /// <summary>
        /// 药品在HIS系统中的id编号,从his系统获取到药品的时候要保存这个编号,方便在以后获取库存等信息的时候,定位到药机系统内的编号.但是因为his系统内的id可能不符合本机的逻辑,所以本药品库中有单独的id
        /// </summary>
        public string IdOfHIS { get { return _idofhis; } set { _idofhis = value; } }

        string _name = null;
        /// <summary>
        /// 药品名称
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }

        string _barcode = null;
        /// <summary>
        /// 药品条码
        /// </summary>
        public string Barcode { get { return _barcode; } set { _barcode = value; } }

        string _company = null;
        /// <summary>
        /// 药品所属公司
        /// </summary>
        public string Company { get { return _company; } set { _company = value; } }

        Nullable<float> _boxlongmm = 0;
        /// <summary>
        /// 药盒的长度毫米
        /// </summary>
        public Nullable<float> BoxLongMM { get { return _boxlongmm; } set { _boxlongmm = value; } }

        Nullable<float> _boxwidthmm = 0;
        /// <summary>
        /// 药盒的宽度毫米
        /// </summary>
        public Nullable<float> BoxWidthMM { get { return _boxwidthmm; } set { _boxwidthmm = value; } }

        Nullable<float> _boxheightmm = 0;
        /// <summary>
        /// 药盒的高度毫米
        /// </summary>
        public Nullable<float> BoxHeightMM { get { return _boxheightmm; } set { _boxheightmm = value; } }


        /// <summary>
        /// 可以装入药仓的最小药品有效期
        /// </summary>
        public Nullable<int> CLMED { get; set; }
        /// <summary>
        /// 建议装入药仓的最小有效期
        /// </summary>
        public Nullable<int> SLMED { get; set; }
        /// <summary>
        /// 有效期小于多少天时候提醒
        /// </summary>
        public Nullable<int> DTOEA { get; set; }
        /// <summary>
        /// 数量少于多少个的时候提醒
        /// </summary>
        public Nullable<int> CTOLIA { get; set; }
        #endregion

        public override string GetApiName()
        {
            return "medicine.update";
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
            else if(this.GetParameters().Count<2)
            {
                throw new ArgumentException("除ID参数为必填外,必须指定有效的要更新字段");
            }
        }
    }
}
