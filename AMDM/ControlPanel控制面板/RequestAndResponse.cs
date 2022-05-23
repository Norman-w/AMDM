using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region 获取药剂的状态

/// <summary>
/// 获取付药机的状态 主要是外设的状态
/// </summary>
public class AMDMStatusGetRequest
{
    /// <summary>
    /// 要获取的字段信息,默认为* 全部 (与 all) 意思相同
    /// </summary>
    public string Fields { get; set; }

    /// <summary>
    /// 使用缓存里面的数据还是强制刷新,如果是强制刷新,重新从plc获取数据
    /// </summary>
    public bool ForceRefresh { get; set; }
}
public class AMDMStatusGetResponse
{
    public bool IsError { get; set; }
    public string ErrMsg { get; set; }
    public AMDMPeripheralsStatus PeripheralsStatus { get; set; }
    /// <summary>
    /// 全局状态
    /// </summary>
    public MaintenanceStatusEnum MaintenanceStatus { get; set; }

    /// <summary>
    /// 当日已经处理的处方数量
    /// </summary>
    public long TodayPrescriptionCount { get; set; }

    /// <summary>
    /// 当日已经取药的数量
    /// </summary>
    public long TodayMedicineCount { get; set; }
}

#endregion
#region 设置药机的空调

/// <summary>
/// 开启或者关闭指定药仓的空调
/// </summary>
public class AMDMPeripheralsStatusACSetRequest
{
    /// <summary>
    /// 要开启或者关闭的药仓的索引编号.
    /// </summary>
    public Nullable<int> StockIndex { get; set; }

    /// <summary>
    /// 设置状态是否为开启(正常是下位机来控制的,直接设置这个字段相对于手动控制)
    /// </summary>
    public Nullable<bool> IsWorking { get; set; }

    /// <summary>
    /// 目标温度是多少
    /// </summary>
    public Nullable<float> DestTemperature { get; set; }
}
/// <summary>
/// 开启或者关闭指定药仓的空调的返回结果
/// </summary>
public class AMDMPeripheralsStatusACSetResponse
{
    public bool IsError { get; set; }
    public string ErrMsg { get; set; }
    /// <summary>
    /// 现在的状态是开还是关
    /// </summary>
    public bool IsWorking { get; set; }
    /// <summary>
    /// 当前的温度
    /// </summary>
    public float CurrentTemperature { get; set; }

    /// <summary>
    /// 设定的目标温度
    /// </summary>
    public float DestTemperature { get; set; }
}
#endregion
#region 设置药剂的紫外线灯是否打开或者关闭(手动模式)

/// <summary>
/// 开启或关闭紫外线灯的请求
/// </summary>
public class AMDMPeripheralsStatusUVLampTurnOnOffRequest
{
    /// <summary>
    /// 设置状态是否为开启
    /// </summary>
    public Nullable<bool> On { get; set; }
    /// <summary>
    /// 如果设置为开启状态,打开多久后自动关闭.
    /// </summary>
    public Nullable<double> AutoTurnOffDelayMM { get; set; }
}
/// <summary>
/// 开启或关闭紫外线灯的结果
/// </summary>
public class AMDMPeripheralsStatusUVLampTurnOnOffResponse
{
    public bool IsError { get; set; }
    public string ErrMsg { get; set; }
    public bool On { get; set; }
}

#endregion


#region 药机的当前运行状态获取 比如 取药中 上药中的各个状态

/// <summary>
/// 获取工作状态
/// </summary>
public class AMDMWorkingStatusGetRequest
{
}

public class AMDMWorkingStatusGetResponse
{
    public bool IsError { get; set; }
    public string ErrMsg { get; set; }
    /// <summary>
    /// 是否正在工作
    /// </summary>
    public bool IsWorking { get; set; }
    /// <summary>
    /// 当前工作内容的名字
    /// </summary>
    public string StatusName { get; set; }
}
#endregion

#region 获取药机的设置

public class AMDMSettingGetRequst
{

}
public class AMDMSettingGetResponse
{
    /// <summary>
    /// 获取到的自动付药机的设置
    /// </summary>
    public Setting AMDMSetting { get; set; }
    /// <summary>
    /// 是否发生错误
    /// </summary>
    public bool IsError { get; set; }
    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrMsg { get; set; }
    /// <summary>
    /// 是否正在工作
    /// </summary>
    public bool IsWorking { get; set; }
}
#endregion
#region 更新药机设置
public class AMDMSettingUpdateRequst
{
    public string Field { get; set; }
    public object Value { get; set; }
}
public class AMDMSettingUpdateResponse
{
    /// <summary>
    /// 是否发生错误
    /// </summary>
    public bool IsError { get; set; }
    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrMsg { get; set; }

    public object NewValue { get; set; }
}
#endregion
#region 紫外线灯自动打开和关闭的设置
/// <summary>
/// 设置紫外线灯的自动开关时间
/// </summary>
public class AMDMPeripheralsSetUVLampOnOffTimeRequest
{
    /// <summary>
    /// 设置紫外线灯的自动开时间,时间格式为  HH:mm  时 分 格式
    /// </summary>
    public string UVLampOnTime { get; set; }
    /// <summary>
    /// 设置紫外线灯的自动关闭时间,时间格式为: HH:mm 时 分 格式
    /// </summary>
    public string UVLampOffTime { get; set; }
}
/// <summary>
/// 设置紫外线灯的自动开始或关闭时间点的结果
/// </summary>
public class AMDMPeripheralsSetUVLampOnOffTimeResponse
{
    public bool IsError { get; set; }
    public string ErrMsg { get; set; }
    public string UVLampOnTime { get; set; }
    public string UVLampOffTime { get; set; }
}
#endregion

#region 补打详单
/// <summary>
/// 重新打印付药单
/// </summary>
public class AMDMRePrintDeliveryRecordPaperResponse
{
    public bool Success { get; set; }
}

public class AMDMRePrintDeliveryRecordPaperRequest
{
    /// <summary>
    /// 将要打印的详单的id
    /// </summary>
    public Nullable<long> DeliveryRecordId { get; set; }
}
#endregion

#region 清空错误状态
public class AMDMClearErrorStatusRequest
{
}
public class AMDMClearErrorStatusResponse : AMDMStatusGetResponse
{
}
#endregion

