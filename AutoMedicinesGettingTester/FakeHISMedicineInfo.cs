using System;
using System.Collections.Generic;
using System.Text;

namespace FakeHISServer.Domain
{
    public class FakeHISMedicineInfo
    {
        Int32 _id = 0;
        public Int32 id { get { return _id; } set { _id = value; } }

        String _one_type = null;
        /// <summary>
        /// 一级类别
        /// </summary>
        public String one_type { get { return _one_type; } set { _one_type = value; } }

        String _statistics_type = null;
        /// <summary>
        /// 统计类别
        /// </summary>
        public String statistics_type { get { return _statistics_type; } set { _statistics_type = value; } }

        String _pay_rate = null;
        /// <summary>
        /// 自负比例
        /// </summary>
        public String pay_rate { get { return _pay_rate; } set { _pay_rate = value; } }

        String _pay_type = null;
        /// <summary>
        /// 支付类型
        /// </summary>
        public String pay_type { get { return _pay_type; } set { _pay_type = value; } }

        String _two_type = null;
        /// <summary>
        /// 二级类别
        /// </summary>
        public String two_type { get { return _two_type; } set { _two_type = value; } }

        String _three_type = null;
        /// <summary>
        /// 三级类别
        /// </summary>
        public String three_type { get { return _three_type; } set { _three_type = value; } }

        String _four_type = null;
        /// <summary>
        /// 四级类别
        /// </summary>
        public String four_type { get { return _four_type; } set { _four_type = value; } }

        String _five_type = null;
        /// <summary>
        /// 五级类别
        /// </summary>
        public String five_type { get { return _five_type; } set { _five_type = value; } }

        String _catalog_no = null;
        /// <summary>
        /// 目录书编号
        /// </summary>
        public String catalog_no { get { return _catalog_no; } set { _catalog_no = value; } }

        String _catalog_frug_form = null;
        /// <summary>
        /// 目录剂型
        /// </summary>
        public String catalog_frug_form { get { return _catalog_frug_form; } set { _catalog_frug_form = value; } }

        String _register_name = null;
        /// <summary>
        /// 药品注册名称
        /// </summary>
        public String register_name { get { return _register_name; } set { _register_name = value; } }

        String _register_frug_form = null;
        /// <summary>
        /// 药品注册剂型
        /// </summary>
        public String register_frug_form { get { return _register_frug_form; } set { _register_frug_form = value; } }

        String _limitation = null;
        /// <summary>
        /// 限制范围
        /// </summary>
        public String limitation { get { return _limitation; } set { _limitation = value; } }

        String _english_name = null;
        /// <summary>
        /// 英文名称
        /// </summary>
        public String english_name { get { return _english_name; } set { _english_name = value; } }

        String _approval_date = null;
        /// <summary>
        /// 批准日期
        /// </summary>
        public String approval_date { get { return _approval_date; } set { _approval_date = value; } }

        String _frug_form = null;
        /// <summary>
        /// 剂型
        /// </summary>
        public String frug_form { get { return _frug_form; } set { _frug_form = value; } }

        String _ab_flag = null;
        /// <summary>
        /// 甲乙标识
        /// </summary>
        public String ab_flag { get { return _ab_flag; } set { _ab_flag = value; } }

        String _medical_catalog = null;
        /// <summary>
        /// 医疗目录
        /// </summary>
        public String medical_catalog { get { return _medical_catalog; } set { _medical_catalog = value; } }

        String _injury_catalog = null;
        /// <summary>
        /// 工伤补充目录
        /// </summary>
        public String injury_catalog { get { return _injury_catalog; } set { _injury_catalog = value; } }

        String _birth_catalog = null;
        /// <summary>
        /// 生育补充目录
        /// </summary>
        public String birth_catalog { get { return _birth_catalog; } set { _birth_catalog = value; } }

        String _outpatient_catalog = null;
        /// <summary>
        /// 门诊统筹目录
        /// </summary>
        public String outpatient_catalog { get { return _outpatient_catalog; } set { _outpatient_catalog = value; } }

        String _medicine_no = null;
        /// <summary>
        /// 药品通用编码
        /// </summary>
        public String medicine_no { get { return _medicine_no; } set { _medicine_no = value; } }

        String _medicine_type = null;
        /// <summary>
        /// 药品类别
        /// </summary>
        public String medicine_type { get { return _medicine_type; } set { _medicine_type = value; } }

        String _medicine_name = null;
        /// <summary>
        /// 药品通用名称
        /// </summary>
        public String medicine_name { get { return _medicine_name; } set { _medicine_name = value; } }

        String _medicine_pinyin = null;
        /// <summary>
        /// 拼音
        /// </summary>
        public String medicine_pinyin { get { return _medicine_pinyin; } set { _medicine_pinyin = value; } }

        String _medicine_alias = null;
        /// <summary>
        /// 助记名
        /// </summary>
        public String medicine_alias { get { return _medicine_alias; } set { _medicine_alias = value; } }

        String _medicine_goods_name = null;
        /// <summary>
        /// 药品商品名称
        /// </summary>
        public String medicine_goods_name { get { return _medicine_goods_name; } set { _medicine_goods_name = value; } }

        String _medicine_address = null;
        /// <summary>
        /// 地址
        /// </summary>
        public String medicine_address { get { return _medicine_address; } set { _medicine_address = value; } }

        String _medicine_barcode = null;
        /// <summary>
        /// 药品条形码
        /// </summary>
        public String medicine_barcode { get { return _medicine_barcode; } set { _medicine_barcode = value; } }

        String _medicine_classification = null;
        /// <summary>
        /// 药物品类
        /// </summary>
        public String medicine_classification { get { return _medicine_classification; } set { _medicine_classification = value; } }

        String _medicine_component = null;
        /// <summary>
        /// 成分
        /// </summary>
        public String medicine_component { get { return _medicine_component; } set { _medicine_component = value; } }

        String _medicine_dose = null;
        /// <summary>
        /// 剂量
        /// </summary>
        public String medicine_dose { get { return _medicine_dose; } set { _medicine_dose = value; } }

        String _medicine_indication = null;
        /// <summary>
        /// 适应症
        /// </summary>
        public String medicine_indication { get { return _medicine_indication; } set { _medicine_indication = value; } }

        String _medicine_avoid = null;
        /// <summary>
        /// 用药禁忌
        /// </summary>
        public String medicine_avoid { get { return _medicine_avoid; } set { _medicine_avoid = value; } }

        String _medicine_response = null;
        /// <summary>
        /// 不良反应
        /// </summary>
        public String medicine_response { get { return _medicine_response; } set { _medicine_response = value; } }

        String _medicine_toxicology = null;
        /// <summary>
        /// 药理毒理
        /// </summary>
        public String medicine_toxicology { get { return _medicine_toxicology; } set { _medicine_toxicology = value; } }

        String _medicine_unit = null;
        /// <summary>
        /// 包装单位
        /// </summary>
        public String medicine_unit { get { return _medicine_unit; } set { _medicine_unit = value; } }

        String _medicine_unit_form = null;
        /// <summary>
        /// 计量单位
        /// </summary>
        public String medicine_unit_form { get { return _medicine_unit_form; } set { _medicine_unit_form = value; } }

        String _medicine_unit_mini = null;
        /// <summary>
        /// 最小单位
        /// </summary>
        public String medicine_unit_mini { get { return _medicine_unit_mini; } set { _medicine_unit_mini = value; } }

        String _medicine_usage_remark = null;
        /// <summary>
        /// 用法用量
        /// </summary>
        public String medicine_usage_remark { get { return _medicine_usage_remark; } set { _medicine_usage_remark = value; } }

        String _medicine_notes = null;
        /// <summary>
        /// 注意事项
        /// </summary>
        public String medicine_notes { get { return _medicine_notes; } set { _medicine_notes = value; } }

        String _medicine_preg = null;
        /// <summary>
        /// 哺乳用药
        /// </summary>
        public String medicine_preg { get { return _medicine_preg; } set { _medicine_preg = value; } }

        String _medicine_child = null;
        /// <summary>
        /// 儿童用药
        /// </summary>
        public String medicine_child { get { return _medicine_child; } set { _medicine_child = value; } }

        String _medicine_oldman = null;
        /// <summary>
        /// 老人用药
        /// </summary>
        public String medicine_oldman { get { return _medicine_oldman; } set { _medicine_oldman = value; } }

        String _medicine_interaction = null;
        /// <summary>
        /// 药物相互作用
        /// </summary>
        public String medicine_interaction { get { return _medicine_interaction; } set { _medicine_interaction = value; } }

        String _medicine_excessive = null;
        /// <summary>
        /// 药物过量
        /// </summary>
        public String medicine_excessive { get { return _medicine_excessive; } set { _medicine_excessive = value; } }

        String _medicine_otc = null;
        /// <summary>
        /// 是否处方药0否
        /// </summary>
        public String medicine_otc { get { return _medicine_otc; } set { _medicine_otc = value; } }

        String _medicine_base = null;
        /// <summary>
        /// 是否基药0否
        /// </summary>
        public String medicine_base { get { return _medicine_base; } set { _medicine_base = value; } }

        String _medicine_place = null;
        /// <summary>
        /// 国内外0国内1国外
        /// </summary>
        public String medicine_place { get { return _medicine_place; } set { _medicine_place = value; } }

        String _medicine_rates = null;
        /// <summary>
        /// 规格转化率
        /// </summary>
        public String medicine_rates { get { return _medicine_rates; } set { _medicine_rates = value; } }

        String _medicine_specifications = null;
        /// <summary>
        /// 注册规格
        /// </summary>
        public String medicine_specifications { get { return _medicine_specifications; } set { _medicine_specifications = value; } }

        String _medicine_approval_no = null;
        /// <summary>
        /// 批准文号
        /// </summary>
        public String medicine_approval_no { get { return _medicine_approval_no; } set { _medicine_approval_no = value; } }

        String _medicine_company = null;
        /// <summary>
        /// 生产企业
        /// </summary>
        public String medicine_company { get { return _medicine_company; } set { _medicine_company = value; } }

        String _medicine_spc = null;
        /// <summary>
        /// 规格
        /// </summary>
        public String medicine_spc { get { return _medicine_spc; } set { _medicine_spc = value; } }

        String _medicine_instruction = null;
        /// <summary>
        /// 说明书
        /// </summary>
        public String medicine_instruction { get { return _medicine_instruction; } set { _medicine_instruction = value; } }
    }
    public class FakeHISMedicineInventory:FakeHISMedicineInfo
    {
        /// <summary>
        /// his系统内保存的当前药品的库存,或者是远端推送过来的符合his系统的格式的库存信息
        /// </summary>
        public int Count { get; set; }
    }
}
