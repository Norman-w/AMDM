using AMDM_Domain;
using DomainBase;
using System;
using System.Collections.Generic;
using System.Web;

public class SnapshotsGetResponse : NetResponse
{
    List<AMDM_Snapshot> _snapshots = new List<AMDM_Snapshot>();
    public List<AMDM_Snapshot> Snapshots { get { return _snapshots; } set { _snapshots = value; } }
}
/// <summary>
/// 获取截图
/// </summary>
public class SnapshotsGetRequest : BaseRequest<SnapshotsGetResponse>
{
    public string Fields { get; set; }

    /// <summary>
    /// 截图所属
    /// </summary>
    public Nullable<SnapshotParentTypeEnum> ParentType { get; set; }
    /// <summary>
    /// 截图所属对象的id 比如获取DeliveryRecord时候的图像,ParentType为 DeliveryRecord, ParentId 为 record的ID
    /// </summary>
    public Nullable<long> ParentId { get; set; }
    public override string GetApiName()
    {
        return "snapshots.get";
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
        if (ParentType == null || ParentId == null)
        {
            throw new ArgumentException("没有指定有效的场景类型和该场景下记录的ID");
        }
    }
}