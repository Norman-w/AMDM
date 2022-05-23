using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using FakeHISServer.Domain;
using AMDM_Domain;
using AMDMClientSDK.Domain;

/*2021年12月8日17:29:46 自动取药测试器,现在第一步开发的功能是,每一次只取一个药,连续把整个药仓的药品取空.
 * 每次取药之前,获取一下所有的药品库存,然后在里面开一盒药品.然后开始自动扫码,自动点击开始取药,然后等取完了药品以后自动开始下一个任务
 * 目的是为了测试整个流程中有没有出现错误.
 *
 *
 * 流程和函数
 *  获取库存,自动创建单子保存,返回单号
 *  触发扫码函数
 *  直接点击确认取药
 *  等待都完成了以后直接再进行一次该流程,直到没有药了
 */

public class AutoMedicinesGettingTester
{
    /// <summary>
    /// 是否正在工作中,只要执行过一次create new order 就算是正在工作了
    /// </summary>
    public bool Working { get; set; }
    /// <summary>
    /// 获取现有的库存,然后创建一个只取一盒药品的付药单,返回为付药单的编号字符串
    /// </summary>
    /// <returns></returns>
    public string CreateNewOrder()
    {
        //string medicinesGettingOrderId = null;
        AMDMApiProcessor p = new AMDMApiProcessor();
        if (System.Diagnostics.Debugger.IsAttached)
        {
            p.SetClient(new AMDM.Manager.SQLDataTransmitter("192.168.2.191", "root", "", "amdm_local", 10000));
        }
        GridInventoryGetRequest r = new GridInventoryGetRequest();
        r.StockIndex = 0;
        //GetCurrentAllInventoryRequest req = new GetCurrentAllInventoryRequest();
        //req.StockIndex = 0;
        GridInventoryGetResponse rsp = p.DoGridInventoryGetRequest(r, "");
        if (rsp!= null && rsp.Machine!= null && rsp.Machine.Stocks!= null && rsp.Machine.Stocks.Count>0 && rsp.Machine.Stocks[0].Floors!= null && rsp.Machine.Stocks[0].Floors.Count>0)
        {
            AMDM_GridInventory first = null;
            foreach (var f in rsp.Machine.Stocks[0].Floors)
            {
                foreach (var g in f.Grids)
                {
                    if (g.Stuck || g.Count == 0)
                    {
                        continue;
                    }
                    first = g;
                    break;
                }
                if (first!= null)
                {
                    break;
                }
            }
            if (first != null)
            {
                //创建一个付药单
                #region 创建付药单
                string hisServerIp = "192.168.2.122";
                MysqlClient sqlClient = new MysqlClient(hisServerIp, "root", "", "his_server", 9999);
                //根据给药单随机创建数量.
                HISMedicineOrder order = new HISMedicineOrder();
                order.Balance = true;
                order.CreateTime = DateTime.Now.AddHours(-5);
                order.ModifiedTime = DateTime.Now.AddHours(-3);
                order.FulfilledTime = order.Fulfilled ? (Nullable<DateTime>)DateTime.Now : null;
                order.Canceled = false;
                //先创建一个付药单,然后创建好了详情以后 更新给药单
                SqlInsertRecordParams ipr = new SqlInsertRecordParams();
                ipr.TableName = "medicine_order";
                ipr.DataObject = order;
                Utils.LogInfo("将插入付药单");
                order.Id = sqlClient.InsertData<FakeHISMedicineInfo>(ipr);
                Utils.LogFinished("插入付药单完成");
                List<object> details = new List<object>();

                    HISMedicineOrderDetail detail = new HISMedicineOrderDetail()
                    {
                        Canceled = false,
                        Barcode = first.Barcode,
                        OrderId = order.Id,
                        Fulfilled = order.Fulfilled,
                        FulfilledTime = order.FulfilledTime,
                        FulfilledNurseId = order.Fulfilled ? (Nullable<long>)order.FulfillmentNurseId : null,
                        FulfilledPharmacyId = order.Fulfilled ? (Nullable<long>)order.PharmacyId : null,
                        MedicineId = Convert.ToInt64(first.IdOfHIS),
                        Name = first.Name,
                        //Count = first.Count
                        Count = 1
                    };


                    SqlInsertRecordParams detailIPR = new SqlInsertRecordParams();
                    detailIPR.TableName = "medicine_order_detail";
                    detailIPR.DataObject = detail;
                    Utils.LogStarted("准备插入付药单详情");
                    detail.Id = sqlClient.InsertData<HISMedicineOrderDetail>(detailIPR);
                    Utils.LogFinished("插入付药单详情完成");

                    if (order.Details == null)
                    {
                        order.Details = new List<HISMedicineOrderDetail>();
                    }
                    order.Details.Add(detail);

                    order.TotalCount += detail.Count;
                    order.EntriesCount++;



                SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
                upr.TableName = "medicine_order";
                upr.WhereEquals.Add("id", order.Id);
                upr.UpdateFieldNameAndValues.Add("entriesCount", order.EntriesCount);
                upr.UpdateFieldNameAndValues.Add("totalCount", order.TotalCount);
                bool updateCountRet = sqlClient.UpdateData(upr)>0;
                if (!updateCountRet)
                {
                    string msg = "插入测试付药单失败";
                    Console.WriteLine(msg);
                    //MessageBox.Show(msg);
                    return null;
                }


                this.Working = true;
                return order.Id.ToString();
                //string json = JsonConvert.SerializeObject(order, jsonSetting);
                //MessageBox.Show(json);
                #endregion
            }
        }
        return null;
    }
}

