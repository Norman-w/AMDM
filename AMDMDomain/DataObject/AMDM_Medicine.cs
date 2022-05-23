using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 给药机使用的药品信息
    /// </summary>
    public class AMDM_Medicine : IAMDM_Medicine
    {
        Int64 _id = 0;
        /// <summary>
        /// 药品id,流水号,自生成
        /// </summary>
        public Int64 Id { get { return _id; } set { _id = value; } }

        String _idofhis = null;
        /// <summary>
        /// 药品在HIS系统中的id编号,从his系统获取到药品的时候要保存这个编号,方便在以后获取库存等信息的时候,定位到药机系统内的编号.但是因为his系统内的id可能不符合本机的逻辑,所以本药品库中有单独的id
        /// </summary>
        public String IdOfHIS { get { return _idofhis; } set { _idofhis = value; } }

        String _name = null;
        /// <summary>
        /// 药品名称
        /// </summary>
        public String Name { get { return _name; } set { _name = value; } }

        String _barcode = null;
        /// <summary>
        /// 药品条码
        /// </summary>
        public String Barcode { get { return _barcode; } set { _barcode = value; } }

        String _company = null;
        /// <summary>
        /// 药品所属公司
        /// </summary>
        public String Company { get { return _company; } set { _company = value; } }

        float _boxlongmm = 0;
        /// <summary>
        /// 药盒的长度毫米
        /// </summary>
        public Single BoxLongMM { get { return _boxlongmm; } set { _boxlongmm = value; } }

        float _boxwidthmm = 0;
        /// <summary>
        /// 药盒的宽度毫米
        /// </summary>
        public float BoxWidthMM { get { return _boxwidthmm; } set { _boxwidthmm = value; } }

        float _boxheightmm = 0;
        /// <summary>
        /// 药盒的高度毫米
        /// </summary>
        public float BoxHeightMM { get { return _boxheightmm; } set { _boxheightmm = value; } }


        #region 新增库存预警阈值的两个可为空的设置字段 2022年1月9日16:17:17
        /// <summary>
        /// CountThresholdOfLowInventoryAlert低库存预警设定值,如果小于或等于这个数值时警报
        /// </summary>
        public Nullable<int> CTOLIA { get; set; }
        /// <summary>
        /// PercentThresholdOfLowInventoryAlert低库存预警设定百分比,如果小于或等于这个数值时警报
        /// </summary>
        public Nullable<float> PTOLIA { get; set; }
        #endregion

        #region 新增库存阈值的两个字段,一个是最少有多少天有效期的可以装载到该药机,一个是装入该药机的最小有效期是多少天
        /// <summary>
        /// CanLoadMinExprationDay该药品需要满足至少多少天的有效期才可以装入药槽
        /// </summary>
        public Nullable<int> CLMED { get; set; }
        /// <summary>
        /// SuggestLoadMinExpirationDay建议装载到药槽中的药品有效期最少多少天
        /// </summary>
        public Nullable<int> SLMED { get; set; }
        #endregion

        /// <summary>
        /// 到期提醒的天数阈值
        /// </summary>
        public Nullable<int> DTOEA { get; set; }
    }
}
