using System;
namespace AMDM_Domain
{
    /// <summary>
    /// 患者信息,如果付药单上需要保存患者信息的时候就继承这个接口
    /// </summary>
    interface IAMDM_PatientInfo
    {
        /// <summary>
        /// 患者年龄
        /// </summary>
        int PatientAge { get; set; }
        /// <summary>
        /// 患者的id
        /// </summary>
        string PatientId { get; set; }
        /// <summary>
        /// 患者的名字
        /// </summary>
        string PatientName { get; set; }
        /// <summary>
        /// 患者的名字
        /// </summary>
        string PatientSex { get; set; }
    }
}
