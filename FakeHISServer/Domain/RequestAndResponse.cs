using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeHISServer.Domain
{
    #region 错误response
    public class HISErrResponse
    {
        public bool IsError { get{return !string.IsNullOrEmpty(ErrMsg);} set{} }
        public string ErrMsg { get; set; }
    }
    #endregion
    #region 付药机->HIS 根据扫码信息获取给药单详细信息Request & Response
    /// <summary>
    /// 获取付药单请求
    /// </summary>
    public class HISGetMedicineOrderRequest
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
    public class HISGetMedicineOrderResponse
    {
        /// <summary>
        /// 付药单信息,包含明细
        /// </summary>
        public HISMedicineOrder Order { get; set; }
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
    #endregion

    

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

    #region 付药机->HIS 获取所有药品的数量 Request & Response
    /// <summary>
    /// 获取总共有多少种药品请求
    /// </summary>
    public class HISGetMedicinesTotalKindContRequest
    {
    }
    /// <summary>
    /// 获取总的药品种类数量信息返回
    /// </summary>
    public class HISGetMedicinesTotalKindCountResponse
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
    public class HISGetMedicinesRequest
    {
        public Nullable<int> PageNum { get; set; }
        public Nullable<int> PageSize { get; set; }
    }
    /// <summary>
    /// 获取药品信息,根据分页信息的返回
    /// </summary>
    public class HISGetMedicinesResponse
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

    #region 获取给药机库存信息Request
    /// <summary>
    /// 从药库/药房获取库存信息
    /// </summary>
    public class HISGetInventoryFromPharmacyReuqest
    {
        /// <summary>
        /// 药房编号
        /// </summary>
        public Nullable<long> PharmacyId { get; set; }
        /// <summary>
        /// 页码,从1开始
        /// </summary>
        public int PageNum { get; set; }
        /// <summary>
        /// 每一页返回的数量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 截止到什么时候的库存信息
        /// </summary>
        public Nullable<DateTime> EndTime { get; set; }

        /// <summary>
        /// 从什么时间开始的库存信息,默认不传,除非获取区间库存时使用.
        /// </summary>
        public Nullable<DateTime> StartTime { get; set; }

        /// <summary>
        /// 用于搜索的药品名称,如不传,获取所有药品
        /// </summary>
        public string NameTag { get; set; }

        /// <summary>
        /// 用于搜索的条码名称,如不传,获取所有药品
        /// </summary>
        public string Barcode { get; set; }
    }
    #endregion
    #region 获取给药机库存信息Response
    /// <summary>
    /// 药品库存信息获取结果
    /// </summary>
    public class HISGetInventoryFromPharmacyResponse
    {
        /// <summary>
        /// 药品的库存信息列表
        /// </summary>
        public List<HISMedicineInventoryInfo> Inventories { get; set; }
        /// <summary>
        /// 是否还有下一页
        /// </summary>
        public bool HasNext { get; set; }
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
    /// <summary>
    /// 药品库存信息
    /// </summary>
    public class HISMedicineInventoryInfo
    {
        /// <summary>
        /// 药品id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 药品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 药品数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 药品条码
        /// </summary>
        public string Barcode { get; set; }
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
}
