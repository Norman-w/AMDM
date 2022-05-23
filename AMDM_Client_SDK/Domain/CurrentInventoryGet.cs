using AMDM_Domain;
using DomainBase;
using System;
using System.Collections.Generic;
using System.Web;

public class GetCurrentAllInventoryResponse: NetResponse
{
    List<AMDM_MedicineInventory> inventory = new List<AMDM_MedicineInventory>();
    public List<AMDM_MedicineInventory> Inventory { get { return inventory; } set { inventory = value; } }
}
public class GetCurrentAllInventoryRequest:BaseRequest<GetCurrentAllInventoryResponse>
{
    //public string Fields { get; set; }
    public Nullable<int> StockIndex { get; set; }
    public override string GetApiName()
    {
        return "inventory.all.get";
    }
    public override bool GetIsSessionRequired()
    {
        return true;
    }
    public override IDictionary<string, string> GetParameters()
    {
        QPNetDictionary param = new QPNetDictionary();
        //param.Add("fields", this.Fields);
        param.Add("stockindex", this.StockIndex);
        param.AddAll(this.GetOtherParameters());
        return param;
    }
    public override void Validate()
    {
        //if (StartTime == null || EndTime == null || UserId == null)
        //{
        //    throw new ArgumentException("无效的参数,需要制定开始,结束时间和用户id");
        //}
    }
}