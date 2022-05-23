using AMDM_Domain;
using DomainBase;
using System;
using System.Collections.Generic;
using System.Text;

public class LogsGetResponse: NetResponse
{
    /// <summary>
    /// 符合条件的日志信息
    /// </summary>
    public List<Dictionary<string, object>> Logs { get; set; }
    /// <summary>
    /// 是否还有下一页
    /// </summary>
    public bool HasNext { get; set; }
    /// <summary>
    /// 总的符合条件的日志的条数
    /// </summary>
    public long TotalResultCount { get; set; }
}
/// <summary>
/// 获取截图
/// </summary>
public class LogsGetRequest : BaseRequest<LogsGetResponse>
{
    /// <summary>
    /// 日志类型,文字转成PipiMessage里面的类型 包含  Info Warning Error Bug,设置为All为获取所有类型的日志
    /// </summary>
    public string Level { get; set; }
    /// <summary>
    /// 日志标题关键字
    /// </summary>
    public string TitleTag { get; set; }
    /// <summary>
    /// 日志内容关键字,不建议使用可能影响效率
    /// </summary>
    public string MessageTag { get; set; }

    public Nullable<DateTime> StartTime { get; set; }

    public Nullable<DateTime> EndTime { get; set; }

    public Nullable<int> PageNum { get; set; }

    public Nullable<int> PageSize { get; set; }

    public override string GetApiName()
    {
        return "logs.get";
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
        if (string.IsNullOrEmpty(Level) == false)
        {
            var l = Level.ToLower();
            if (l != "all" && l != "error" && l != "warning" && l != "info" && l != "bug")
            {
                throw new ArgumentException("无效的日志类型信息");
            }
        }
        if (string.IsNullOrEmpty(Level) == true)
        {
            throw new ArgumentException("需要指定日志的类型,如需获取全部类型,请指定All");
        }
        if(PageNum<0 || PageSize<1)
        {
            throw new ArgumentException("无效的页码或单页数据数量信息");
        }
    }
}