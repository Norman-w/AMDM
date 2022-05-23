/*
 * 2022年2月19日18:32:46  写到返回值一共有多少条记录的时候发现,如果是要在后端处理排序进出记录的时候 还必须要在一条sql语句里面直接限定数量  要不然会增加数据的请求次数
 * 而且先请求入库的再请求出库的会造成请求数据的浪费
 * 比如一共药10条记录,那么久需要再入库里面取到10条然后在出库里面取到10条,然后按照时间排序以后把没有用的给废弃掉.所以这个方法应该是不太可取的.
 * 而且在获取每一页10条数据的时候 实际上为了保证   HasNext 字段的数据准确 还需要在多获取一条   也就是获取11条,如果获取到了11条以后 证明获取到的数据还有下一页  然后在返回数据的时候去掉 最后一条记录返回.

 * 2022年2月19日18:35:31  所以现在应该研究一下 每次获取的时候 都直接获取入库和出库的并且排序好.
 */

using AMDM_Domain;
using DomainBase;
using System;
using System.Collections.Generic;
using System.Web;

public class ClipInOutRecordSnapGetResponse : NetResponse
{
    /// <summary>
    /// 是否还包含有下一页数据.
    /// </summary>
    public bool HasNext { get; set; }
    /// <summary>
    /// 总共有多少条记录
    /// </summary>
    public int TotalResult { get; set; }
    /// <summary>
    /// 获取到的所有的截图记录数据
    /// </summary>
    public List<AMDM_ClipInOutRecordSnap> Snaps { get; set; }
}
/// <summary>
/// 获取药槽的使用记录的截图.
/// </summary>
public class ClipInOutRecordSnapGetRequest : BaseRequest<ClipInOutRecordSnapGetResponse>
{
    public  ClipInOutRecordSnapGetRequest()
    {
        this.PageSize = 10;
        this.PageNum = 0;
    }
    /// <summary>
    /// 要获取的药仓的编号
    /// </summary>
    public Nullable<int> StockIndex { get; set; }
    /// <summary>
    /// 要获取的层板的编号
    /// </summary>
    public Nullable<int> FloorIndex { get; set; }
    /// <summary>
    /// 要获取的格子的编号
    /// </summary>
    public Nullable<int> GridIndex { get; set; }

    /// <summary>
    /// 获取记录的起始时间
    /// </summary>
    public Nullable<DateTime> StartTime { get; set; }

    /// <summary>
    /// 获取记录的结束时间
    /// </summary>
    public Nullable<DateTime> EndTime { get; set; }

    /// <summary>
    /// 是否使用时间的倒叙排序
    /// </summary>
    public Nullable<bool> OrderByTimeDescMode { get; set; }

    /// <summary>
    /// 单次获取的页的大小
    /// </summary>
    public int PageSize { get; set; }
    /// <summary>
    /// 当前获取的是哪一页的数据,从0开始.
    /// </summary>
    public int PageNum { get; set; }


    public override string GetApiName()
    {
        return "clip.inoutrecord.snap.get";
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
        if (this.PageSize == null || this.PageSize<0)
        {
            throw new ArgumentException("无效的单页数据量设置");
        }
        else if(this.PageSize>100)
        {
            throw new ArgumentException("单页数据量过大,最大可设置为100");
        }
        if (this.PageNum<0)
        {
            throw new ArgumentException("无效的页码数据,第一页数据为0");
        }
        if (this.StockIndex== null  || this.StockIndex<0)
        {
            throw new ArgumentException("无效的药仓序号");
        }
        if (this.GridIndex < 0)
        {
            throw new ArgumentException("无效的药槽序号");
        }
        if (this.StartTime== null ^ this.EndTime == null)
        {
            throw new ArgumentException("起始时间和结束时间必须同时指定");
        }
    }
}