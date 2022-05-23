using  System;
using System.Collections.Generic;
using System.Text;
using Norman;
using Newtonsoft.Json;
using AMDM_Domain;
/*
 * 2021年11月5日13:03:47 付药机到HIS系统的连接器.
 * 用于在布药环节,获取到所有的药品数据到付药机,增量获取his系统的新增和修改药品到付药机.从HIS系统获取付药单,推送付药单到his系统等.
 * 
 * 该类中的全局变量可以根据不同的医院配置不同的变量,比如连接方式(mysql,http,ws,tcp等), 比如连接参数(ip,port,username,password,sign等等) 还可以在此类中扩展一些别的方法 比如计算签名等.
  
  该类对象应该继承自接口IHISServerConnector,必须是先的接口有 初始化,获取付药单,获取全部药品,获取增量药品,推送付药结果信息
 * 
 * 后续还可以再增加接口比如 推送给HIS系统需要补药, 推送给HIS系统每日库存信息 等等
 */

    /// <summary>
    /// 付药机到HIS系统的连接器
    /// </summary>
public class FakeHISServerConnector : IHISServerConnector
{

    /// <summary>
    /// 用于检测HIS系统的当前连接状况,通常是在空闲时间检测HIS系统有没有维护等,如果在维护了.将同步锁定药机的使用.因为那时候就读取不出来处方了,扫了也没有用.如果可以了的时候再根据需要自动恢复
    /// 另外每次取药之前也要先检测一下HIS系统可否连接,如果不能连接,直接给用户提示.并且把药机至于维护等状态
    /// </summary>
    /// <returns></returns>
    public bool CheckConnect()
    {
        GetMedicineOrderRequest req = new GetMedicineOrderRequest();
        req.OrderId = 0;
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("apiName", "medicineorder.get");
        string rspJson = wu.DoPost(url, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req)), "application/json;charset=utf-8", headers);
        GetMedicineOrderResponse rsp = null;
        try
        {
            rsp = JsonConvert.DeserializeObject<GetMedicineOrderResponse>(rspJson);
            if (rsp!= null)
            {
                return true;
            }
            return false;
        }
        catch (Exception err)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("HIS系统连接失败:{0}", err.Message);
            return false;
        }
    }

    //public class Config
    //{
    //    public string Url { get; set; }
    //    public string ContentType { get; set; }
    //    public string Medhod { get; set; }
    //}
    #region 全局变量 用于保存连接时候的参数
    string url;
    WebUtils wu = new WebUtils();
    //string configFilePath;
    #endregion
    #region 初始化付药机到his系统的连接器
    /// <summary>
    /// 初始化付药机到his系统的连接器
    /// </summary>
    /// <returns></returns>
    public bool Init()
    {
        //configFilePath = Environment.CurrentDirectory + "\\HISServerConnector.cfg";
        //#region 从文件读取初始化数据然后放到url等数据中,以便后续能够改变ip地址,不然的话需要重新编写代码以后才能从不同的服务器上获取数据
        //if (System.IO.File.Exists(configFilePath) == false)
        //{
        //    System.IO.File.Create(configFilePath).Close();

        //}
        //#endregion
        //url = "http://localhost:9000";
        url = "http://192.168.2.122:9000";
        wu.Timeout = 5000;
        return true;
    }
    #endregion

    #region 从his系统获取付药单信息
    /// <summary>
    /// 从HIS系统根据处方编号获取付药单订单,每个处方都只会有一个付药单
    /// </summary>
    /// <returns></returns>
    public AMDM_MedicineOrder GetMedicineOrderByPrescriptionId(string prescriptionId)
    {
        Console.WriteLine("使用处方编号获取付药单:{0} url:{1}", prescriptionId, url);
        long longPrescriptionId = 0;
        if (long.TryParse(prescriptionId, out longPrescriptionId) == false)
        {
            Console.WriteLine("无效的付药单编号:{0}", prescriptionId);
            return null;
        }
        GetMedicineOrderRequest req = new GetMedicineOrderRequest();
        req.OrderId = longPrescriptionId;
        Dictionary<string,string> headers = new Dictionary<string,string>();
        headers.Add("apiName", "medicineorder.get");
        string rspJson = wu.DoPost(url, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req)), "application/json;charset=utf-8", headers);
        Console.WriteLine("获取药单返回结果rsp:{0}", rspJson);
        GetMedicineOrderResponse rsp = null;
        try
        {
            rsp = JsonConvert.DeserializeObject<GetMedicineOrderResponse>(rspJson);
        }
        catch (Exception)
        {
            throw;
        }
        if (rsp.Order!= null && rsp.Order.Id>0)
        {
            #region 转换成付药单
            AMDM_MedicineOrder ret = new AMDM_MedicineOrder()
            {
                Balance = rsp.Order.Balance,
                BalanceTime = rsp.Order.BalanceTime,
                Canceled = rsp.Order.Canceled,
                CreateTime = rsp.Order.CreateTime,
                CreatorId = rsp.Order.CreatorId,
                Details = new List<AMDM_MedicineOrderDetail>(),
                EntriesCount = rsp.Order.EntriesCount,
                Fulfilled = rsp.Order.Fulfilled,
                FulfilledTime = rsp.Order.FulfilledTime,
                FulfillmentNurseId = rsp.Order.FulfillmentNurseId,
                ModifiedTime = rsp.Order.ModifiedTime,
                Id = rsp.Order.Id,
                PatientId = string.Format("{0}",rsp.Order.PatientId),
                PharmacyId = rsp.Order.PharmacyId,
                TotalCount = rsp.Order.TotalCount
            };
            for (int i = 0; i < rsp.Order.Details.Count; i++)
            {
                var detailSrc = rsp.Order.Details[i];
                AMDM_MedicineOrderDetail detail = new AMDM_MedicineOrderDetail()
                {
                    Barcode = detailSrc.Barcode,
                    Canceled = detailSrc.Canceled,
                    Count = detailSrc.Count,
                    Fulfilled = detailSrc.Fulfilled,
                    FulfilledNurseId = detailSrc.FulfilledNurseId,
                    FulfilledPharmacyId = detailSrc.FulfilledPharmacyId,
                    FulfilledTime = detailSrc.FulfilledTime,
                    Id = detailSrc.Id,
                    MedicineId = detailSrc.MedicineId,
                    Name = detailSrc.Name,
                    OrderId = detailSrc.OrderId
                };

                ret.Details.Add(detail);
            }
            #endregion
            return ret;
        }
        return null;
    }
    /// <summary>
    /// 从HIS系统根据id卡号或者是患者的编号获取付药单
    /// </summary>
    /// <param name="patientId"></param>
    /// <returns></returns>
    public List<AMDM_MedicineOrder> GetMedicineOrderByPatientId(string patientId)
    {
        return null;
    }
    #endregion

    #region 向HIS系统推送付药单已经完成信息
    /// <summary>
    /// 向HIS系统推送消息,告知his系统某个处方(付药单)已经完成付药
    /// </summary>
    /// <param name="prescriptionId"></param>
    /// <returns></returns>
    public bool PutMedicineOrderFinished(AMDM_MedicineOrder order, string repoter)
    {
        HISPutMedicineOrderDeliveryFinishedRequest req = new HISPutMedicineOrderDeliveryFinishedRequest();
        req.OrderId = Convert.ToInt64(order.Id);
        req.PharmacyInfo = repoter;
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("apiName", "medicineorder.finish");
        string rspJson = wu.DoPost(url, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req)), "application/json;charset=utf-8", headers);
        HISPutMedicineOrderDeliveryFinishedResponse rsp = JsonConvert.DeserializeObject<HISPutMedicineOrderDeliveryFinishedResponse>(rspJson);
        if (rsp != null && rsp.IsError == false && rsp.Success == true)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region 向his系统推送药品库存不足的消息
    public bool NoticeInsufficientInventory(List<AMDM_MedicineInventory> inventories, string reporerInfo, string message, List<string> receiverMobileList)
    {
        HISNoticeInsufficientInventoryRequest req = new HISNoticeInsufficientInventoryRequest();
        req.Message = message;
        req.Repoter = reporerInfo;
        #region 遍历药品的数量做成dic
        Dictionary<long, int> dic = new Dictionary<long, int>();
        foreach (var item in inventories)
        {
            dic.Add(item.Id, item.Count);
        }
        req.MedicinesIdAndInventoryDicJson = JsonConvert.SerializeObject(dic);
        #endregion
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("apiName", "medicineinventory.insufficient");
        string rspJson = wu.DoPost(url, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req)), "application/json;charset=utf-8", headers);
        HISNoticeInsufficientInventoryResponse rsp = JsonConvert.DeserializeObject<HISNoticeInsufficientInventoryResponse>(rspJson);
        if (rsp != null && rsp.IsError == false && rsp.Success == true)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region 告知药机有效期预警信息
    public bool NoticeMedicinesExpirationDateAlert(List<AMDM_MedicineObject__Grid__Medicine> medicineObjects, string reporterInfo, string message, List<string> receiverMobileList)
    {
        HISPutMedicinesExpirationAlertRequest req = new HISPutMedicinesExpirationAlertRequest();
        req.Message = message;
        req.Repoter = reporterInfo;
        #region 遍历药品的数量做成dic
        req.MedicinesExpirationInfoJson = JsonConvert.SerializeObject(medicineObjects);
        #endregion
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("apiName", "medicine.expirationalert");
        string rspJson = wu.DoPost(url, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req)), "application/json;charset=utf-8", headers);
        HISPutMedicinesExpirationAlertResponse rsp = JsonConvert.DeserializeObject<HISPutMedicinesExpirationAlertResponse>(rspJson);
        if (rsp != null && rsp.IsError == false && rsp.Success == true)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region 从his系统获取所有的药品信息
    /// <summary>
    /// 从his系统获取所有的药品信息
    /// </summary>
    /// <returns></returns>
    public List<AMDM_Medicine> GetAllMedicines(OnGetedMedicineInfoFromHisServerEventHandler onGetedMedicineInfoFromHisServer)
    {
        List<AMDM_Medicine> allMedicines = new List<AMDM_Medicine>();
        int pageSize = 100;
        int totalPage = 0;
        Dictionary<string, string> param = new Dictionary<string, string>();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("apiName", "medicines.kind.count.get");
        //headers.Add("apiName", "medicines.modified.get");
        headers.Add("Content-Type", "application/json;charset=utf-8");
        string ret = wu.DoPost(url, param, headers);
        GetMedicinesTotalKindCountResponse rsp  = JsonConvert.DeserializeObject<GetMedicinesTotalKindCountResponse>(ret);
        if (rsp!= null && rsp.IsError == false)
        {
            int totalKindCount = rsp.TotalKindCount;
            totalPage = totalKindCount / pageSize;
            if (totalKindCount%pageSize>0)
            {
                totalPage += 1;
            }
            for (int i = 0; i < totalPage; i++)
            {
                ///////////////////////////测试 只取1000条数据  要不然数据拉取时间太长了调试等不了.
                //if (i > 4)
                //{
                //    break;
                //}
                #region 获取每一页的药品信息
                GetMedicinesRequest getMedicinesReq = new GetMedicinesRequest();
                getMedicinesReq.PageNum = i;
                getMedicinesReq.PageSize = pageSize;
                Dictionary<string, string> getMedicinesHeaders = new Dictionary<string, string>();
                getMedicinesHeaders.Add("apiName", "medicines.get");
                string getPageRetJson = wu.DoPost(url, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(getMedicinesReq)), "application/json;charset=utf-8", getMedicinesHeaders);
                //GetMedicinesRequest getMedicinesReq = new GetMedicinesRequest();
                //getMedicinesReq.PageNum = i;
                //getMedicinesReq.PageSize = pageSize;
                GetMedicinesResponse getMedicinesRsp = null;
                try
                {
                    getMedicinesRsp = JsonConvert.DeserializeObject<GetMedicinesResponse>(getPageRetJson);
                }
                catch (Exception)
                {
                    throw;
                }
                if (getMedicinesRsp ==null)
                {
                    throw new NotImplementedException("获取药品数量成功,获取药品信息列表失败");
                }
                List<object> medicines = new List<object>();
                if (getMedicinesRsp.Medicines!= null)
                {
                    for (int j = 0; j < getMedicinesRsp.Medicines.Count; j++)
                    {
                        FakeHISMedicineInfo currentMedicine = getMedicinesRsp.Medicines[j];
                        medicines.Add(currentMedicine);
                        ///转换HIS系统过来的药品信息到付药机的药品信息对象
                        allMedicines.Add(Convert2AMDM_Medicine(currentMedicine));
                    }
                    if (onGetedMedicineInfoFromHisServer != null)
                    {
                        onGetedMedicineInfoFromHisServer(medicines, i, totalPage);
                    }
                }
                #endregion
            }
        }
        return allMedicines;
    }
    #endregion

    #region 转换HIS系统的药品信息到自动付药机的药品信息
    AMDM_Medicine Convert2AMDM_Medicine(FakeHISMedicineInfo medicine)
    {
        AMDM_Medicine amdmMedicine = new AMDM_Medicine();
        amdmMedicine.Name = medicine.medicine_name;
        amdmMedicine.IdOfHIS = string.Format("{0}", medicine.id);
        amdmMedicine.Company = medicine.medicine_company;
        amdmMedicine.Barcode = medicine.medicine_barcode;
        return amdmMedicine;
    }
    #endregion

    #region 从his系统获取增量药品信息
    /// <summary>
    /// 从his系统获取增量药品信息
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public List<AMDM_Medicine> GetModifiedMedicines(DateTime start, DateTime end, OnGetedMedicineInfoFromHisServerEventHandler onGetedMedicineInfoFromHisServer)
    {
        return null;
    }

    /// <summary>
    /// 是否支持增量获取药品的形式
    /// </summary>
    /// <returns></returns>
    public bool GetIsSupportModifiedMedicines()
    {
        return false;
    }
    #endregion

    #region 发送给his系统,报告付药机系统故障信号
    /// <summary>
    /// 发送给his系统报告付药机的错误信息,由his系统通过短信的方式告知我方工作人员机械发生了故障需要维护.返回的是his那边返回来的内容的json序列化
    /// </summary>
    /// <param name="msgContent"></param>
    /// <returns></returns>
    public bool PushMachineErrorMsg(string msgContent, string repoter, List<string> receiverMobileList)
    {
        int timeOutMS = 3000;

        HISNoticeAMDMErrorRequest req = new HISNoticeAMDMErrorRequest();
        req.Message = msgContent;
        req.Repoter = repoter;

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("apiName", "amdm.machine.error");
        wu.Timeout = timeOutMS;
        string rspJson = null;
        try
        {
            rspJson = wu.DoPost(url, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req)), "application/json;charset=utf-8", headers);
            HISNoticeAMDMErrorResponse rsp = JsonConvert.DeserializeObject<HISNoticeAMDMErrorResponse>(rspJson);

            if (rsp != null && rsp.IsError == false && rsp.Success == true)
            {
                return true;
            }
        }
        catch (Exception err)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("报送取药机发生故障信息时 发生故障:");
            Console.WriteLine(err.Message);
            Console.ResetColor();
            return false;
        }
        return false;
    }
    #endregion

    #region 推送给HIS系统,报告当前的库存信息
    public bool NoticeMedicinesInventory(List<AMDM_MedicineInventory> medicinesInventory, string repoter, List<long> receiverMobileList)
    {
        List<FakeHISMedicineInventory> infos = new List<FakeHISMedicineInventory>();
        for (int i = 0; i < medicinesInventory.Count; i++)
        {
            AMDM_MedicineInventory current = medicinesInventory[i];
            FakeHISMedicineInventory hisInventoryCurrent = ConvertAMDMMedicineInventory2HISMedicineInventory(current);
            infos.Add(hisInventoryCurrent);
        }
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(infos);
        HISPutMedicinesInventoryRequest req = new HISPutMedicinesInventoryRequest();
        req.MedicinesInventoryInfoJson = json;
        req.Repoter = repoter;

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("apiName", "amdm.machine.error");
        string rspJson = wu.DoPost(url, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req)), "application/json;charset=utf-8", headers);
        HISPutMedicinesInventoryResponse rsp = JsonConvert.DeserializeObject<HISPutMedicinesInventoryResponse>(rspJson);
        if (rsp != null && rsp.IsError == false && rsp.Success == true)
        {
            return true;
        }
        return false;
    }
    FakeHISMedicineInventory ConvertAMDMMedicineInventory2HISMedicineInventory(AMDM_MedicineInventory amdmMedicineInventory)
    {
        FakeHISMedicineInventory inventory = new FakeHISMedicineInventory()
        {
            id = Convert.ToInt64(amdmMedicineInventory.IdOfHIS),
            medicine_barcode = amdmMedicineInventory.Barcode,
            medicine_name = amdmMedicineInventory.Name,
            Count = amdmMedicineInventory.Count
        };
        return inventory;
    }
    #endregion
}

#region 医院那边给过来的sdk,获取数据用的,或者是自己根据医院那边给过来的json生成的req和rsp的类对象
#region 付药机->HIS 根据扫码信息获取给药单详细信息Request & Response
/// <summary>
/// 获取付药单请求
/// </summary>
public class GetMedicineOrderRequest
{
    //public string ApiName { get; set; }
    /// <summary>
    /// 付药单编号
    /// </summary>
    public Nullable<long> OrderId { get; set; }
}
/// <summary>
/// 获取付药单返回
/// </summary>
public class GetMedicineOrderResponse
{
    /// <summary>
    /// 付药单信息,包含明细
    /// </summary>
    public MedicineOrder Order { get; set; }
    /// <summary>
    /// 回传信息,通常用于提醒等备用字段.可不传
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 是否调用接口错误
    /// </summary>
    public bool IsError { get; set; }

    /// <summary>
    /// 调用接口错误时的错误信息.
    /// </summary>
    public string ErrMsg { get; set; }
}
#endregion

#region 付药机->HIS 获取所有药品的数量 Request & Response
/// <summary>
/// 获取总共有多少种药品请求
/// </summary>
public class GetMedicinesTotalKindContRequest
{
}
/// <summary>
/// 获取总的药品种类数量信息返回
/// </summary>
public class GetMedicinesTotalKindCountResponse
{
    /// <summary>
    /// 返回总的药品种类数量信息
    /// </summary>
    public int TotalKindCount { get; set; }
    /// <summary>
    /// 回传信息,通常用于提醒等备用字段.可不传
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 是否调用接口错误
    /// </summary>
    public bool IsError { get; set; }

    /// <summary>
    /// 调用接口错误时的错误信息.
    /// </summary>
    public string ErrMsg { get; set; }
}
#endregion

#region 付药机->HIS 获取药品信息 根据分页
/// <summary>
/// 获取药品信息,根据分页信息
/// </summary>
public class GetMedicinesRequest
{
    public Nullable<int> PageNum { get; set; }
    public Nullable<int> PageSize { get; set; }
}
/// <summary>
/// 获取药品信息,根据分页信息的返回
/// </summary>
public class GetMedicinesResponse
{
    public List<FakeHISMedicineInfo> Medicines { get; set; }
    /// <summary>
    /// 回传信息,通常用于提醒等备用字段.可不传
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 是否调用接口错误
    /// </summary>
    public bool IsError { get; set; }

    /// <summary>
    /// 调用接口错误时的错误信息.
    /// </summary>
    public string ErrMsg { get; set; }
}
#endregion

#region 付药机->HIS 推送处方已经完成付药的request和response 2021年11月17日22:15:48
/// <summary>
/// /付药机推送给his系统消息,告诉某个付药单已经完成付药请求类
/// </summary>
public class HISPutMedicineOrderDeliveryFinishedRequest
{
    /// <summary>
    /// 要推送的单号
    /// </summary>
    public Nullable<long> OrderId { get; set; }

    /// <summary>
    /// 付药的药房的信息
    /// </summary>
    public string PharmacyInfo { get; set; }
}
/// <summary>
/// 付药机推送给his系统消息,告诉某个付药单已经完成付药的返回值
/// </summary>
public class HISPutMedicineOrderDeliveryFinishedResponse
{
    /// <summary>
    /// 是否推送成功
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// 回传信息,通常用于提醒等备用字段.可不传
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 是否调用接口错误
    /// </summary>
    public bool IsError { get; set; }

    /// <summary>
    /// 调用接口错误时的错误信息.
    /// </summary>
    public string ErrMsg { get; set; }
}
#endregion

#region 付药机->HIS 推送药品的库存已经不足的消息的request 和response 2021年11月17日22:53:54
/// <summary>
/// 付药机->HIS 推送药品的库存已经不足的消息的request
/// </summary>
public class HISNoticeInsufficientInventoryRequest
{
    /// <summary>
    /// 携带的消息
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// 报送者信息
    /// </summary>
    public string Repoter { get; set; }
    /// <summary>
    /// 所有药房或者药机系统要求按照 键值对对应的方式序列化json 赋值此字段  C#可以直接使用Dictionary long int 的字典序列化
    /// </summary>
    public string MedicinesIdAndInventoryDicJson { get; set; }
}
/// <summary>
/// 付药机->HIS 推送药品的库存已经不足的消息的response
/// </summary>
public class HISNoticeInsufficientInventoryResponse
{
    /// <summary>
    /// 是否已经推送成功
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// 回传信息,通常用于提醒等备用字段.可不传
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 是否调用接口错误
    /// </summary>
    public bool IsError { get; set; }

    /// <summary>
    /// 调用接口错误时的错误信息.
    /// </summary>
    public string ErrMsg { get; set; }
}
#endregion.

#region 付药机->HIS 推送付药机发生故障的消息
/// <summary>
/// 付药机->HIS 推送付药机发生故障的消息request
/// </summary>
public class HISNoticeAMDMErrorRequest
{
    /// <summary>
    /// 携带的消息
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// 报送者信息
    /// </summary>
    public string Repoter { get; set; }
}
/// <summary>
/// 付药机->HIS 推送付药机发生故障的消息response
/// </summary>
public class HISNoticeAMDMErrorResponse
{
    /// <summary>
    /// 是否已经推送成功
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// 回传信息,通常用于提醒等备用字段.可不传
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 是否调用接口错误
    /// </summary>
    public bool IsError { get; set; }

    /// <summary>
    /// 调用接口错误时的错误信息.
    /// </summary>
    public string ErrMsg { get; set; }
}
#endregion

#region 付药机->HIS 主动推送库存信息到his系统
/// <summary>
/// 获取药品信息,根据分页信息
/// </summary>
public class HISPutMedicinesInventoryRequest
{
    /// <summary>
    /// 将FakeHISMedicineInventory的list序列化的json,由于是在网络中传送,所以不能直接传送对象,而是把对象转换成json传递过来
    /// 2021年11月19日14:59:30 这显示器有水波纹啊,而且放不稳当 晃来晃去的晕了...
    /// </summary>
    public string MedicinesInventoryInfoJson { get; set; }
    /// <summary>
    /// 哪个药机推送过来的,报告信息人是谁
    /// </summary>
    public string Repoter { get; set; }
}
/// <summary>
/// 获取药品信息,根据分页信息的返回
/// </summary>
public class HISPutMedicinesInventoryResponse
{
    /// <summary>
    /// 本次推送是否成功
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// 回传信息,通常用于提醒等备用字段.可不传
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 是否调用接口错误
    /// </summary>
    public bool IsError { get; set; }

    /// <summary>
    /// 调用接口错误时的错误信息.
    /// </summary>
    public string ErrMsg { get; set; }
}
#endregion

#region 付药机->HIS 主动推送药品有效期提醒信息
/// <summary>
/// 获取药品信息,根据分页信息
/// </summary>
public class HISPutMedicinesExpirationAlertRequest
{
    /// <summary>
    /// 将FakeHISMedicineInventory的list序列化的json,由于是在网络中传送,所以不能直接传送对象,而是把对象转换成json传递过来
    /// 2021年11月19日14:59:30 这显示器有水波纹啊,而且放不稳当 晃来晃去的晕了...
    /// </summary>
    public string MedicinesExpirationInfoJson { get; set; }
    /// <summary>
    /// 哪个药机推送过来的,报告信息人是谁
    /// </summary>
    public string Repoter { get; set; }

    public string Message { get; set; }
}
/// <summary>
/// 获取药品信息,根据分页信息的返回
/// </summary>
public class HISPutMedicinesExpirationAlertResponse
{
    /// <summary>
    /// 本次推送是否成功
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// 回传信息,通常用于提醒等备用字段.可不传
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 是否调用接口错误
    /// </summary>
    public bool IsError { get; set; }

    /// <summary>
    /// 调用接口错误时的错误信息.
    /// </summary>
    public string ErrMsg { get; set; }
}

#endregion

#region 数据类型定义
public class MedicineOrder_data
{
    Int64 _id = 0;
    public Int64 Id { get { return _id; } set { _id = value; } }

    long _creatorid = 0;
    public long CreatorId { get { return _creatorid; } set { _creatorid = value; } }

    Int64 _patientid = 0;
    public Int64 PatientId { get { return _patientid; } set { _patientid = value; } }

    Int32 _pharmacyid = 0;
    public Int32 PharmacyId { get { return _pharmacyid; } set { _pharmacyid = value; } }

    Nullable<DateTime> _createtime = null;
    public Nullable<DateTime> CreateTime { get { return _createtime; } set { _createtime = value; } }

    Nullable<DateTime> _modifiedtime = null;
    public Nullable<DateTime> ModifiedTime { get { return _modifiedtime; } set { _modifiedtime = value; } }

    Nullable<DateTime> _balancetime = null;
    public Nullable<DateTime> BalanceTime { get { return _balancetime; } set { _balancetime = value; } }

    Nullable<DateTime> _fulfilledtime = null;
    public Nullable<DateTime> FulfilledTime { get { return _fulfilledtime; } set { _fulfilledtime = value; } }

    Int32 _entriescount = 0;
    public Int32 EntriesCount { get { return _entriescount; } set { _entriescount = value; } }

    Int32 _totalcount = 0;
    public Int32 TotalCount { get { return _totalcount; } set { _totalcount = value; } }

    bool _balance = false;
    public bool Balance { get { return _balance; } set { _balance = value; } }

    bool _fulfilled = false;
    public bool Fulfilled { get { return _fulfilled; } set { _fulfilled = value; } }

    Int64 _fulfillmentnurseid = 0;
    public Int64 FulfillmentNurseId { get { return _fulfillmentnurseid; } set { _fulfillmentnurseid = value; } }

    bool _canceled = false;
    public bool Canceled { get { return _canceled; } set { _canceled = value; } }
}
public class FakeHISMedicineInventory : FakeHISMedicineInfo
{
    /// <summary>
    /// his系统内保存的当前药品的库存,或者是远端推送过来的符合his系统的格式的库存信息
    /// </summary>
    public int Count { get; set; }
}
public class MedicineOrder : MedicineOrder_data
{
    public List<MedicineOrderDetail> Details { get; set; }
}
public class MedicineOrderDetail
{
    Int64 _id = 0;
    public Int64 Id { get { return _id; } set { _id = value; } }

    Int64 _orderid = 0;
    public Int64 OrderId { get { return _orderid; } set { _orderid = value; } }

    Int64 _drugid = 0;
    public Int64 MedicineId { get { return _drugid; } set { _drugid = value; } }

    String _name = null;
    public String Name { get { return _name; } set { _name = value; } }

    String _barcode = null;
    public String Barcode { get { return _barcode; } set { _barcode = value; } }

    int _count = 0;
    public int Count { get { return _count; } set { _count = value; } }

    bool _fulfilled = false;
    public bool Fulfilled { get { return _fulfilled; } set { _fulfilled = value; } }

    Nullable<DateTime> _fulfilledtime = null;
    public Nullable<DateTime> FulfilledTime { get { return _fulfilledtime; } set { _fulfilledtime = value; } }

    Nullable<long> _fulfilledpharmacyid = null;
    public Nullable<long> FulfilledPharmacyId { get { return _fulfilledpharmacyid; } set { _fulfilledpharmacyid = value; } }

    Nullable<long> _fulfillednurseid = null;
    public Nullable<long> FulfilledNurseId { get { return _fulfillednurseid; } set { _fulfillednurseid = value; } }

    bool _canceled = false;
    public bool Canceled { get { return _canceled; } set { _canceled = value; } }
}
#endregion
#endregion
