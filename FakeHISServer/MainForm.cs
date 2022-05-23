using AMDM_Domain;
using FakeHISServer.Domain;
using MyCode.Forms;
using Newtonsoft.Json;
using Norman;
using QP.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Printing;
using System.Text;
using System.Windows.Forms;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;
using ZXing;
using ZXing.Common;

namespace FakeHISServer
{
    delegate void UpdateTextboxSyncFunc(string message, TextBox ctrl);
    public partial class MainForm : Form
    {
        #region 全局变量
        /// <summary>
        /// HIS服务器的接口访问地址
        /// </summary>
        string HTTPServerAddress = null;
        /// <summary>
        /// 自动给药机的接口地址
        /// </summary>
        string AMDMServerAddress = null;
        MysqlClient sqlClient = null;
        const string TableNames_MedicineOrder = "medicine_order";
        const string TableNames_MedicineOrderDetail = "medicine_order_detail";
        const string TableNames_MedicineInfo = "medicine_info";
        BackgroundWorker httpServerThread = new BackgroundWorker();

        string printerName = null;

        JsonSerializerSettings jsonSetting = null;
        HTTPServer server = new HTTPServer();

        const string startedHTTPServerBtnText = "停止HTTP服务";
        const string stopedHTTPServerBtnText = "开启HTTP服务";
        #endregion
        #region 异步更新函数
        void updTextBox(string msg, TextBox tb)
        {
            if (tb.InvokeRequired)
            {
                UpdateTextboxSyncFunc fc = new UpdateTextboxSyncFunc(updTextBox);
                tb.BeginInvoke(fc, new object[]{msg, tb});
            }
            else
            {
                tb.Text = msg;
            }
        }
        #endregion
        #region 构造函数
        public MainForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            
            this.sendToHTTPServerBtn.Enabled = false;
            this.sendToAMDMBtn.Enabled = false;
            this.requestCombox.SelectedItem = this.requestCombox.Items[this.requestCombox.Items.Count-1];
            jsonSetting = new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss" };

            //GetInventoryFromPharmacyResponse rsp = new GetInventoryFromPharmacyResponse();
            //rsp.Inventories = new List<MedicineInventoryInfo>();
            //rsp.Inventories.Add(new MedicineInventoryInfo());

            //string json = JsonConvert.SerializeObject(rsp);

            this.AMDMServerAddress = this.AMDMAddressTextbox.Text;
            this.HTTPServerAddress = this.httpServerAddressTextbox.Text;
            server.OnReciveMessage += server_OnReciveMessage;


            //httpServerThread.WorkerReportsProgress = true;
            //httpServerThread.DoWork += httpServerThread_DoWork;
            //httpServerThread.ProgressChanged += httpServerThread_ProgressChanged;
            //httpServerThread.RunWorkerAsync();

            #region 获取并设置打印机列表
            List<string> printerList = this.GetPrintList();
            for (int i = 0; i < printerList.Count; i++)
            {
                this.printerListCombox.Items.Add(printerList[i]);
            }
            if (printerList.Count>0)
            {
                this.printerListCombox.SelectedItem = printerList[0];
            }
            #endregion
        }
        #endregion
        #region 异步启动http服务器
        void httpServerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.ProgressPercentage == 0)
            {
                
            }
        }

        void httpServerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            //server.Start(this.HTTPServerAddress);
        }
        #endregion
        #region http服务器接到请求内容时的处理
        string server_OnReciveMessage(string msg, object requestRef, object responseRef)
        {
            
            HttpListenerRequest request = requestRef as HttpListenerRequest;

            StringBuilder headersSB = new StringBuilder();
            int headerCount = request.Headers.Count;
            List<string> headerKeys = new List<string>(request.Headers.AllKeys);
            for (int i = 0; i < headerKeys.Count; i++)
            {
                string key = headerKeys[i];
                string val = request.Headers[key];

                if (headersSB.Length > 0)
                {
                    headersSB.Append("\r\n");
                }
                headersSB.AppendFormat("{0}={1}", key, val);
            }
            string log = string.Format("Headers:\r\n{0}\r\n\r\n Content:\r\n{1}", headersSB, msg);
            this.updTextBox(log, this.receiveMsgTextbox);


            if (request.ContentType.ToLower().Contains("application/json"))
            {
                Dictionary<string, object> jsonKV = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
                #region 根据header中的apiName决定要把msg序列化成什么格式然后进行处理
                string apiName = request.Headers["apiName"];
                if (string.IsNullOrEmpty(apiName))
                {
                    return getErrorResponse("无效的api名称");
                }
                switch (apiName.ToLower())
                {
                    case "medicineorder.get":
                    #region 获取付药单
                        HISGetMedicineOrderRequest req = JsonConvert.DeserializeObject<HISGetMedicineOrderRequest>(msg);
                        if (req!= null && req.OrderId>0)
                        {
                            SqlObjectGeterParameters ogpr = new SqlObjectGeterParameters();
                            ogpr.TableName = TableNames_MedicineOrder;
                            ogpr.SetFields("*");
                            ogpr.WhereEquals.Add("id", req.OrderId);
                            List<HISMedicineOrder> orders = sqlClient.GetDatas<HISMedicineOrder>(ogpr);
                            if (orders!= null && orders.Count ==1)
                            {
                                HISMedicineOrder order = orders[0];
                                SqlObjectGeterParameters dgpr = new SqlObjectGeterParameters();
                                dgpr.TableName = TableNames_MedicineOrderDetail;
                                dgpr.SetFields("*");
                                dgpr.WhereEquals.Add("orderid", order.Id);
                                order.Details = sqlClient.GetDatas<HISMedicineOrderDetail>(dgpr);
                                if (order.Details == null || order.Details.Count == 0)
                                {
                                    return getErrorResponse("该付药单应付药品为空");
                                }
                                HISGetMedicineOrderResponse rsp = new HISGetMedicineOrderResponse();
                                rsp.Order = order;

                                string rspJson = JsonConvert.SerializeObject(rsp, jsonSetting);
                                return rspJson;
                            }
                            else
                            {
                                return getErrorResponse("未获取到有效的付药单"); 
                            }
                        }
                        else
                        {
                            return getErrorResponse("无效的请求主体");
                        }
                        #endregion
                    #region 推送到HIS,已经完成了某个付药单的付药
                    case "medicineorder.finish":
                        HISPutMedicineOrderDeliveryFinishedRequest putFinishReq = JsonConvert.DeserializeObject<HISPutMedicineOrderDeliveryFinishedRequest>(msg);
                        if (putFinishReq != null && putFinishReq.OrderId!= null && putFinishReq.OrderId!= null)
                        {
                            SqlObjectUpdaterParamters putFinishPr = new SqlObjectUpdaterParamters();
                            putFinishPr.TableName = TableNames_MedicineOrder;
                            putFinishPr.WhereEquals.Add("id", putFinishReq.OrderId);
                            putFinishPr.UpdateFieldNameAndValues.Add("fulfilled", true);
                            putFinishPr.UpdateFieldNameAndValues.Add("fulfilledTime", DateTime.Now);

                            bool putFinishedRet = sqlClient.UpdateData(putFinishPr);
                            if (putFinishedRet == false)
                            {
                                return getErrorResponse("未能更新付药单(处方)的完结状态");
                            }
                            HISPutMedicineOrderDeliveryFinishedResponse putFinishRsp = new HISPutMedicineOrderDeliveryFinishedResponse();
                            putFinishRsp.Success = true;

                            string rspJson = JsonConvert.SerializeObject(putFinishRsp, jsonSetting);
                            return rspJson;
                        }
                        else
                        {
                            return getErrorResponse("无效的请求主体或参数错误");
                        }
                    #endregion
                    #region 推送到his 药品库存不足
                    case "medicineinventory.insufficient":
                        HISNoticeInsufficientInventoryRequest inventoryInsufficentReq = JsonConvert.DeserializeObject<HISNoticeInsufficientInventoryRequest>(msg);
                        if (inventoryInsufficentReq != null && inventoryInsufficentReq.MedicinesIdAndInventoryDicJson != null)
                        {
                            #region 处理,发短信
                            Dictionary<long, int> medicinesIdAndInventoryDic = new Dictionary<long, int>();
                            try
                            {
                                StringBuilder mobileMsg = new StringBuilder(string.Format("{0}发来的库存不足通知:\r\n",inventoryInsufficentReq.Repoter));
                                JsonConvert.PopulateObject(inventoryInsufficentReq.MedicinesIdAndInventoryDicJson, medicinesIdAndInventoryDic);
                                foreach (var item in medicinesIdAndInventoryDic)
                                {
                                    long medicineId = item.Key;
                                    long medicineInvenntory = item.Value;
                                    FakeHISMedicineInfo medicine = null;
                                    //读取每个药品的名称
                                    SqlObjectGeterParameters mgpr = new SqlObjectGeterParameters();
                                    mgpr.TableName = TableNames_MedicineInfo;
                                    mgpr.SetFields("*");
                                    
                                    mgpr.WhereEquals.Add("id", medicineId);

                                    List<FakeHISMedicineInfo> medicines = sqlClient.GetDatas<FakeHISMedicineInfo>(mgpr);
                                    if (medicines.Count == 1)
                                    {
                                        medicine = medicines[0];
                                    }
                                    string line = string.Format("[{0}] 库存 {0}\r\n");
                                    mobileMsg.Append(line);
                                }
                                mobileMsg.AppendFormat("附加信息:{0}", (inventoryInsufficentReq.Message == null ? "无" : inventoryInsufficentReq.Message));

                                showLog("这是护士手机上收到的短信", mobileMsg.ToString());
                            }
                            catch (Exception err)
                            {
                                return getErrorResponse(string.Format("解析入参json错误:{0}", err.Message));
                            }
                            #endregion
                            HISNoticeInsufficientInventoryResponse putFinishRsp = new HISNoticeInsufficientInventoryResponse();
                            putFinishRsp.Message = "短信已发送";
                            putFinishRsp.Success = true;

                            string rspJson = JsonConvert.SerializeObject(putFinishRsp, jsonSetting);
                            return rspJson;
                        }
                        else
                        {
                            return getErrorResponse("无效的请求主体或参数错误");
                        }
                    #endregion
                    #region 推送付药机发生故障的消息,应当由his系统推送短信消息到我方工作人员 或者是护士
                    case "amdm.machine.error":
                        HISNoticeAMDMErrorRequest amdmMachineErrorRequest = JsonConvert.DeserializeObject<HISNoticeAMDMErrorRequest>(msg);
                        if (amdmMachineErrorRequest!= null && amdmMachineErrorRequest.Message != null)
                        {
                            showLog("这是护士或者我们工作人员收到的药机故障短信", string.Format("发送者:{0}\r\n{1}",amdmMachineErrorRequest.Repoter, amdmMachineErrorRequest.Message));

                            HISNoticeAMDMErrorResponse amdmMachineErrorResponse = new HISNoticeAMDMErrorResponse();
                            amdmMachineErrorResponse.Message = "短信已发送";
                            amdmMachineErrorResponse.Success = true;

                            string rspJson = JsonConvert.SerializeObject(amdmMachineErrorResponse, jsonSetting);
                            return rspJson;
                        }
                        else
                        {
                            return getErrorResponse("无效的药机故障请求主体,必须指定message");
                        }
                    #endregion
                    #region 推送付药机当前的库存到his系统
                    case "medicineinventory.put":
                        HISPutMedicinesInventoryRequest putinventoryReq = JsonConvert.DeserializeObject<HISPutMedicinesInventoryRequest>(msg);
                        if (putinventoryReq != null && putinventoryReq.MedicinesInventoryInfoJson!= null || putinventoryReq.Repoter == null)
                        {
                            try
                            {
                                List<FakeHISMedicineInventory> infos = JsonConvert.DeserializeObject<List<FakeHISMedicineInventory>>(putinventoryReq.MedicinesInventoryInfoJson);
                                showLog("这是his系统服务器收到的内容", string.Format("已经收到{0}推送过来的付药机当前库存信息", putinventoryReq.Repoter));
                                HISPutMedicinesInventoryResponse rsp = new HISPutMedicinesInventoryResponse();
                                rsp.Success = true;
                                rsp.Message = "感谢推送.我收到库存信息了.谢谢";
                                string rspJson = JsonConvert.SerializeObject(rsp, jsonSetting);
                                return rspJson;
                            }
                            catch (Exception parseParameterErr)
                            {
                                return getErrorResponse(string.Format("推送到HIS系统库存信息错误,无法解析入参json:{0}", parseParameterErr.Message));
                            }
                        }
                        else
                        {
                            return getErrorResponse("推送到HIS系统库存信息错误,入参不正确");
                        }
                    #endregion
                    #region 推送药品有效期预警消息
                    case "medicine.expirationalert":
                        HISPutMedicinesExpirationAlertRequest expirationRet = JsonConvert.DeserializeObject<HISPutMedicinesExpirationAlertRequest>(msg);
                        if (expirationRet != null && expirationRet.MedicinesExpirationInfoJson != null)
                        {
                            try
                            {
                                List<HISPutMedicinesExpirationAlertResponse> infos = JsonConvert.DeserializeObject<List<HISPutMedicinesExpirationAlertResponse>>(expirationRet.MedicinesExpirationInfoJson);
                                showLog("这是his系统服务器收到的有效期提醒消息内容", string.Format("已经收到{0}推送过来的有效期预警消息", expirationRet.Repoter));
                                HISPutMedicinesInventoryResponse rsp = new HISPutMedicinesInventoryResponse();
                                rsp.Success = true;
                                rsp.Message = "感谢推送.我收到预警信息了.谢谢";
                                string rspJson = JsonConvert.SerializeObject(rsp, jsonSetting);
                                return rspJson;
                            }
                            catch (Exception parseParameterErr)
                            {
                                return getErrorResponse(string.Format("推送到HIS系统药品有效期预警消息错误,无法解析入参json:{0}", parseParameterErr.Message));
                            }
                        }
                        else
                        {
                            return getErrorResponse("推送到HIS系统药品有效期预警消息错误,入参不正确");
                        }
                    #endregion
                    #region 获取药品的总分类数量
                    case "medicines.kind.count.get":
                        HISGetMedicinesTotalKindContRequest kindCountGetReq = JsonConvert.DeserializeObject<HISGetMedicinesTotalKindContRequest>(msg);
                        string kindCountGetCmd = string.Format("select count(*) from {0}",TableNames_MedicineInfo);
                        List<long> kindCountValues = sqlClient.GetValues<long>(kindCountGetCmd);
                        if (kindCountValues.Count == 1)
                        {
                            HISGetMedicinesTotalKindCountResponse kindCountGetRsp = new HISGetMedicinesTotalKindCountResponse();
                            kindCountGetRsp.TotalKindCount = (int)kindCountValues[0];

                            string rspJson = JsonConvert.SerializeObject(kindCountGetRsp, jsonSetting);
                            return rspJson;
                        }
                        break;
                    #endregion
                    #region 获取药品,按照分页
                    case "medicines.get":
                        HISGetMedicinesRequest medicinesGetReq = JsonConvert.DeserializeObject<HISGetMedicinesRequest>(msg);
                        if (medicinesGetReq.PageSize == null || medicinesGetReq.PageNum == null)
                        {
                            return "无效的请求参数";
                        }
                        SqlObjectGeterParameters medicineGetPr = new SqlObjectGeterParameters();
                        medicineGetPr.TableName = TableNames_MedicineInfo;
                        medicineGetPr.SetFields("*");
                        medicineGetPr.StartIndex = medicinesGetReq.PageNum * medicinesGetReq.PageSize;
                        medicineGetPr.MaxRecordCount = medicinesGetReq.PageSize;

                        List<FakeHISMedicineInfo> medicinesCurrentPage = sqlClient.GetDatas<FakeHISMedicineInfo>(medicineGetPr);
                        HISGetMedicinesResponse medicinesGetRsp = new HISGetMedicinesResponse();
                        medicinesGetRsp.Medicines = medicinesCurrentPage;
                        string medicinesGetRspJSON = JsonConvert.SerializeObject(medicinesGetRsp, jsonSetting);
                        return medicinesGetRspJSON;
                        break;
                    #endregion
                    default:
                        break;
                }
                #endregion
                return msg;
            }
            else
            {
                //如果发过来的不是json的话  结构整个不是json 那就看做是 ?a=b&c=d 这样的模式
                if (msg == null)
                {
                    return null;
                }
                string[] formLine = msg.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                if (formLine!= null && formLine.Length>0)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    for (int i = 0; i < formLine.Length; i++)
                    {
                        string currentLine = formLine[i];
                        if (currentLine == null || currentLine.Length < 1)
                        {
                            continue;
                        }
                        string[] currentLineKV = currentLine.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        if (currentLineKV == null || currentLineKV.Length != 2)
                        {
                            continue;
                        }
                        string key = currentLineKV[0];
                        string value = currentLineKV[1];
                        if (dic.ContainsKey(key) == false)
                        {
                            dic.Add(key, value);
                        }
                    }
                    return JsonConvert.SerializeObject(dic,jsonSetting);
                }
            }
            return null;
        }
        string getErrorResponse(string errMsg)
        {
            HISErrResponse rsp = new HISErrResponse() { ErrMsg = errMsg };
            return JsonConvert.SerializeObject(rsp,jsonSetting);
        }
        #endregion
        #region 窗体加载/初始化
        private void MainForm_Load(object sender, EventArgs e)
        {
            #region http服务器
            
            #endregion


            this.updateSQLserverSettingAndTryConnectBtn_Click(sender, e);
            
            #region 获取付药单信息列表
            try
            {
                this.reloadDataBtn_Click(sender, e);
            }
            catch (Exception err)
            {
                showLog("获取付药单列表失败", err.Message);
            }
            
            #endregion
        }
        #endregion
        #region 创建付药单
        private void createMedicineOrderBtn_Click(object sender, EventArgs e)
        {
            /*
             * 随机读取一些药品信息,然后创建一个表.存入到数据库.返回数据库中表的id
             */
            List<string> testingHasedMedicinesBarcode = new List<string>();
            #region 测试的时候有的药品的条码集合,开测试付药单的时候从测试付药单的表格中获取
            string hasedIdOfHisFile = Application.StartupPath + "\\idofhis.txt";
            string hasedMedicinesBarcodeFile = Application.StartupPath+"\\medicinesBarcodes.txt";
            string[] hasedMedicinesFileContent = null;
            string[] hasedIdOfHisFileContent = null;
            if (System.IO.File.Exists(hasedIdOfHisFile))
            {
                hasedIdOfHisFileContent = System.IO.File.ReadAllLines(hasedIdOfHisFile);
            }
            if (System.IO.File.Exists(hasedMedicinesBarcodeFile))
            {
                hasedMedicinesFileContent = System.IO.File.ReadAllLines(hasedMedicinesBarcodeFile);
            }
            #endregion
            List<int> medicineIdList = null;
            
            StringBuilder medicinesBarcodes = new StringBuilder();
            #region 是用固定的一些id
            if (hasedIdOfHisFileContent != null && hasedIdOfHisFileContent.Length > 0)
            {
                medicineIdList = new List<int>();
                for (int i = 0; i < hasedIdOfHisFileContent.Length; i++)
                {
                    medicineIdList.Add(Convert.ToInt32(hasedIdOfHisFileContent[i]));
                }
                Console.WriteLine(  "使用固定的药品id:数量:{0}", medicineIdList.Count);
            }
            #endregion
            #region 使用固定的一些barcode
            else if (hasedMedicinesFileContent != null && hasedMedicinesFileContent.Length > 0)
            {
                for (int i = 0; i < hasedMedicinesFileContent.Length; i++)
                {
                    if (medicinesBarcodes.Length > 0)
                    {
                        medicinesBarcodes.Append(",");
                    }
                    medicinesBarcodes.AppendFormat("'{0}'", hasedMedicinesFileContent[i]);
                }
                string cmd = string.Format("select id from medicine_info where medicine_barcode in ({0})", medicinesBarcodes);
                medicineIdList = sqlClient.GetValues<int>(cmd);
                //Console.WriteLine("使用本地条码文件中的药品,获取到药品id数量为:{0}\r\n cmd:{1}", medicineIdList.Count, cmd);
                if (medicineIdList.Count == 0)
                {
                    showLog("", "没有获取到药品");
                    return;
                }
            }
            #endregion
            #region 使用全部id
            else
            {
                medicineIdList = sqlClient.GetValues<int>("select id from medicine_info");
            }
            #endregion
            int randomCount = new Random().Next(5, 9);//随机取5到9种
            List<int> needIdList = new List<int>();
            StringBuilder needIdListSb = new StringBuilder();
            Random rm = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < randomCount; i++)
            {
                int index = rm.Next();
                int realIndex = index % medicineIdList.Count;
                //needIdList.Add(medicineIdList[index%medicineIdList.Count]);
                if (i > 0)
                {
                    needIdListSb.Append(",");
                }
                needIdListSb.Append(medicineIdList[realIndex]);
            }
            List<FakeHISMedicineInfo> medicines = sqlClient.GetDatas<FakeHISMedicineInfo>(string.Format("select * from medicine_info where id in ({0})", needIdListSb));


            //根据给药单随机创建数量.
            HISMedicineOrder order = new HISMedicineOrder();
            bool balance = Convert.ToBoolean(new Random(Guid.NewGuid().GetHashCode()).Next() % 2);
            order.Balance = balance;
            order.BalanceTime = balance ? (Nullable<DateTime>)DateTime.Now.AddHours(-1) : null;
            order.CreateTime = DateTime.Now.AddHours(-5);
            order.CreatorId = new Random().Next(1000, 2000);
            order.ModifiedTime = DateTime.Now.AddHours(-3);
            order.PatientId = new Random().Next(10000000, 100000000);
            order.PharmacyId = new Random().Next(10000, 100000);
            order.Fulfilled = balance ? Convert.ToBoolean(new Random(Guid.NewGuid().GetHashCode()).Next(10000) % 2) : false;
            order.FulfilledTime = order.Fulfilled ? (Nullable<DateTime>)DateTime.Now : null;
            order.Canceled = false;
            //先创建一个付药单,然后创建好了详情以后 更新给药单
            SqlInsertRecordParams ipr = new SqlInsertRecordParams();
            ipr.TableName = "medicine_order";
            ipr.DataObject = order;
            order.Id = sqlClient.InsertData<FakeHISMedicineInfo>(ipr);

            List<object> details = new List<object>();
            for (int i = 0; i < medicines.Count; i++)
            {
                FakeHISMedicineInfo current = medicines[i];
                int count = new Random().Next(1, 4);
                HISMedicineOrderDetail detail = new HISMedicineOrderDetail()
                {
                    Canceled = false,
                    Barcode = current.medicine_barcode,
                    OrderId = order.Id,
                    Fulfilled = order.Fulfilled,
                    FulfilledTime = order.FulfilledTime,
                    FulfilledNurseId = order.Fulfilled ? (Nullable<long>)order.FulfillmentNurseId : null,
                    FulfilledPharmacyId = order.Fulfilled ? (Nullable<long>)order.PharmacyId : null,
                    MedicineId = current.id,
                    Name = current.medicine_name,
                    Count = count,
                };


                SqlInsertRecordParams detailIPR = new SqlInsertRecordParams();
                detailIPR.TableName = "medicine_order_detail";
                detailIPR.DataObject = detail;
                detail.Id = sqlClient.InsertData<HISMedicineOrderDetail>(detailIPR);

                if (order.Details == null)
                {
                    order.Details = new List<HISMedicineOrderDetail>();
                }
                order.Details.Add(detail);

                order.TotalCount += detail.Count;
                order.EntriesCount++;
            }


            SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
            upr.TableName = "medicine_order";
            upr.WhereEquals.Add("id", order.Id);
            upr.UpdateFieldNameAndValues.Add("entriesCount", order.EntriesCount);
            upr.UpdateFieldNameAndValues.Add("totalCount", order.TotalCount);
            bool updateCountRet = sqlClient.UpdateData(upr);
            if (!updateCountRet)
            {
                showLog("", "插入测试付药单失败");
                return;
            }
            string json = JsonConvert.SerializeObject(order,jsonSetting);
            showLog("",json);
        }
        #endregion
        #region 打印付药单
        private void printMedicineOrderBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1)
            {
                return;
            }
            string idStr = string.Format("{0}", dataGridView1.SelectedRows[0].Cells["columnId"].Value);
            string content = string.Format("MedicineOrderID:{0}", dataGridView1.SelectedRows[0].Cells["columnId"].Value);
            ZXing.QrCode.QRCodeWriter qrw = new ZXing.QrCode.QRCodeWriter();
            BitMatrix bitM = qrw.encode(content, ZXing.BarcodeFormat.QR_CODE, pictureBox1.Width, pictureBox1.Width);

            BarcodeWriter wr = new BarcodeWriter();
            Bitmap bit = wr.Write(bitM);
            

            ImageCompose2 im2 = new ImageCompose2(58, 58);
            Tag root = new Tag();
            root.width = 1;
            //root.height = 1;
            root.flexDirection = FlexDirectionEnum.col;
            root.width = 1;
            root.id = "root";
            root.justifyContent = JustifyContentEnum.flexStart;
            root.alignItems = AlignItemsEnum.center;
            root.name = "主元素";
            root.border.color = Color.Red;
            root.border.width = 1;
            root.border.dashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

            Img imgTag = new Img();
            imgTag.width = 1;
            imgTag.height = 1;
            ZXing.QrCode.QRCodeWriter qrwFull = new ZXing.QrCode.QRCodeWriter();
            BitMatrix bitMFull = qrwFull.encode(content, ZXing.BarcodeFormat.QR_CODE, 800, 800);
            BarcodeWriter wrFull = new BarcodeWriter();
            Bitmap bitFull = wr.Write(bitMFull);

            imgTag.image = bitFull;
            imgTag.flexDirection = FlexDirectionEnum.row;
            imgTag.justifyContent = JustifyContentEnum.center;
            imgTag.alignItems = AlignItemsEnum.center;
            

            Text textTag = new MyCode.Forms.Text();
            textTag.width = 1;
            //textTag.height = 1;
            textTag.font = new System.Drawing.Font("隶书", 18);
            textTag.fontColor = Color.Black;
            textTag.value = "付药单编号:" + idStr;
            textTag.flexDirection = FlexDirectionEnum.row;
            textTag.justifyContent = JustifyContentEnum.center;
            textTag.alignItems = AlignItemsEnum.center;
            textTag.format = new StringFormat() { Alignment = StringAlignment.Center };

            Text infoTag = new MyCode.Forms.Text();
            infoTag.width = 1;
            //textTag.height = 1;
            infoTag.font = new System.Drawing.Font("黑体", 18);
            infoTag.fontColor = Color.Black;
            infoTag.value = "FakeHIS";
            infoTag.flexDirection = FlexDirectionEnum.row;
            infoTag.justifyContent = JustifyContentEnum.center;
            infoTag.alignItems = AlignItemsEnum.center;
            infoTag.format = new StringFormat() { Alignment = StringAlignment.Center };

            root.AddChirld(textTag);

            root.AddChirld(imgTag);

            root.AddChirld(infoTag);
            
            im2.LoadRootTag(root);
            Image img = new Bitmap(pictureBox1.Width, pictureBox1.Width);
            try
            {
                im2.PrintToPrinter(printerName, "付药单" + idStr);
                im2.PrintToImage(Graphics.FromImage(img));
            }
            catch (Exception err)
            {
                showLog("", "打印失败");
                return;
            }
            pictureBox1.Image = bit;

            Graphics g = Graphics.FromImage(bit);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            g.DrawString(content, new Font("黑体", 9), new SolidBrush(Color.Red), new PointF(pictureBox1.Width / 2, 0), sf);

            //this.pictureBox1.Image = img;

            string log = string.Format("生成付药单二维码完成,已使用:{0}\r\n打印出.请使用给药机进行扫码付药", printerName);
            updTextBox(log, logTextbox);
        }
        #endregion
        #region 重新加载付药单列表
        private void reloadDataBtn_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            SqlObjectGeterParameters gpr = new SqlObjectGeterParameters();
            gpr.TableName = "medicine_order";
            gpr.SetFields("*");
            //gpr.WhereEquals.Add()
            try
            {
                List<HISMedicineOrder_data> orders = sqlClient.GetDatas<HISMedicineOrder_data>(gpr);
                for (int i = 0; i < orders.Count; i++)
                {
                    HISMedicineOrder_data current = orders[i];
                    dataGridView1.Rows.Add(new object[] { current.Id, current.Balance ? "已结清" : "", current.Fulfilled ? "已交付" : "" });
                }
            }
            catch (Exception err )
            {
                showLog("", "刷新付药单列表失败");
            }
           
        }
        #endregion
        #region 修改已结清状态
        private void changeBalanceStatusBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count ==1)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                string value = string.Format("{0}", row.Cells["columnBalance"].Value);
                long id = Convert.ToInt64(row.Cells["columnId"].Value);
                SqlValueUpdaterParamters upr = new SqlValueUpdaterParamters();
                upr.TableName = TableNames_MedicineOrder;
                upr.WhereEquals.Add("id", id);
                upr.Field = "balance";
                if (string.IsNullOrEmpty(value))
                {
                    upr.Value = true;
                }
                else
                {
                    upr.Value = false;
                }
                sqlClient.UpdateValue(upr);
                row.Cells["columnBalance"].Value = Convert.ToBoolean(upr.Value)? "已结清":"";
            }
        }
        #endregion
        #region 修改已交付状态
        private void changeFulfillStatusBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                string value = string.Format("{0}", row.Cells["columnFulfill"].Value);
                long id = Convert.ToInt64(row.Cells["columnId"].Value);
                SqlValueUpdaterParamters upr = new SqlValueUpdaterParamters();
                upr.TableName = TableNames_MedicineOrder;
                upr.WhereEquals.Add("id", id);
                upr.Field = "fulfilled";
                if (string.IsNullOrEmpty(value))
                {
                    upr.Value = true;
                }
                else
                {
                    upr.Value = false;
                }
                sqlClient.UpdateValue(upr);
                row.Cells["columnFulfill"].Value = Convert.ToBoolean(upr.Value) ? "已交付" : "";
            }
        }
        #endregion
        #region 发送测试内容到http服务器
        private void sendToHTTPServerBtn_Click(object sender, EventArgs e)
        {
            WebUtils wu = new WebUtils();
            //Dictionary<string,string> param = new Dictionary<string,string>();
            //param.Add("RequestId", Guid.NewGuid().ToString("N"));
            //param.Add("From", "AMDM");
            //param.Add("Content", "{\"Type\":\"Heart beat,Hello world!\"}");

            //string requestJson = JsonConvert.SerializeObject(param);
            //Dictionary<string, object> dic = new Dictionary<string, object>();
            //dic.Add("Type", "Heartbeat");
            //dic.Add("T", DateTime.Now.Ticks);
            ////string requestJson = "{\"Type\":\"Heart beat,Hello world!\"}";
            //string requestJson = JsonConvert.SerializeObject(dic);

            //this.sendMsgTextBox.Text = requestJson;
            //string rsp = wu.DoPost(HTTPServerAddress, param);
            //string rsp = wu.DoPost(HTTPServerAddress, Encoding.UTF8.GetBytes(requestJson),"application/json", new Dictionary<string,string>());
            if ((string)this.requestCombox.SelectedItem == "√自定义信息(上方输入框内容)")
            {
                try
                {
                    string rsp = wu.DoPost(HTTPServerAddress, Encoding.UTF8.GetBytes(this.sendMsgTextBox.Text), "application/json", new Dictionary<string, string>());
                    string log = string.Format("本机HTTP服务器返回信息:{0}", rsp);
                    this.updTextBox(log, logTextbox);
                }
                catch (Exception doPostErr)
                {
                    string log = string.Format("发送post请求到本机http服务器错误:{0}", doPostErr.Message);
                    this.updTextBox(log, logTextbox);
                }
            }
            else
            {
                string rsp = this.GetRequestJsonBySelectedRequestName(string.Format("{0}", this.requestCombox.SelectedItem));
                //string log = string.Format("执行Request返回值:{0}", rsp);
                //this.updTextBox(log, logTextbox);
            }
            
            
            
        }
        /// <summary>
        /// 通过选择的request名字 获取相应的request样本
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetRequestJsonBySelectedRequestName(string name)
        {
            long testOrderId = 222;
            string ret = null;
            #region √付药机->HIS 根据扫码信息获取给药单详细信息
            if (name == "√付药机->HIS 根据扫码信息获取给药单详细信息")
            {
                HISGetMedicineOrderRequest req = new HISGetMedicineOrderRequest();
                req.OrderId = testOrderId;
                WebUtils wu = new WebUtils();

                
                string requestJson = JsonConvert.SerializeObject(req,jsonSetting);

                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("apiName", "medicineorder.get");


                this.sendMsgTextBox.Text = requestJson;// buildTextboxText(headers, requestJson);
                try
                {
                    string rsp = wu.DoPost(HTTPServerAddress, Encoding.UTF8.GetBytes(requestJson), "application/json", headers);
                    string log = string.Format("付药机->HIS 根据扫码信息获取给药单详细信息\r\n返回值:{0}", rsp);
                    this.updTextBox(log, logTextbox);
                }
                catch (Exception doPostErr)
                {
                    string log = string.Format("付药机->HIS 根据扫码信息获取给药单详细信息 请求错误:{0}\r\n", doPostErr.Message);
                    this.updTextBox(log, logTextbox);
                }
            }
            #endregion
            #region √HIS->付药机 获取付药机当前的库存信息
            else if(name == "√HIS->付药机 获取付药机当前的库存信息")
            {
                WebUtils wu = new WebUtils();
                wu.Timeout = 2000;
                #region 2021年11月30日11:27:43  先前his系统内直接定义的request不能直接用,实际应当使用付药机的request

               
                //HISGetInventoryFromPharmacyReuqest req = new HISGetInventoryFromPharmacyReuqest();
                //req.StartTime = DateTime.Now.AddMonths(-1);
                //req.EndTime = DateTime.Now;
                //req.PageNum = 1;
                //req.PageSize = 100;



                //string requestJson = JsonConvert.SerializeObject(req, jsonSetting);

                //Dictionary<string, string> headers = new Dictionary<string, string>();
                //headers.Add("apiName", "inventory.get");


                //this.sendMsgTextBox.Text = requestJson;// buildTextboxText(headers, requestJson);
                //try
                //{
                //    string rsp = wu.DoPost(AMDMServerAddress, Encoding.UTF8.GetBytes(requestJson), "application/json", headers);
                //    string log = string.Format("√HIS->付药机 获取付药机当前的库存信息\r\n返回值:{0}", rsp);
                //    this.updTextBox(log, logTextbox);
                //}
                //catch (Exception doPostErr)
                //{
                //    string log = string.Format("√HIS->付药机 获取付药机当前的库存信息 请求错误:{0}\r\n", doPostErr.Message);
                //    this.updTextBox(log, logTextbox);
                //}
                #endregion
                #region 2021年11月30日11:28:08  付药机那边给的sdk
                GetCurrentAllInventoryRequest req = new GetCurrentAllInventoryRequest();
                //req.Fields = "id,name,barcode";
                req.StockIndex = 0;
                req.AddOtherParameter("method", req.GetApiName());
                try
                {
                    string retJson = wu.DoGet(this.AMDMServerAddress, req.GetParameters());
                    return retJson;
                }
                catch (Exception err)
                {
                    return err.Message;
                }
                #endregion
            }
            #endregion
            return ret;
        }

        string buildTextboxText(Dictionary<string,string> headers, string requestJson)
        {
            StringBuilder headersSB = new StringBuilder();
            foreach (var item in headers)
            {
                if (headersSB.Length > 0)
                {
                    headersSB.Append("\r\n");
                }
                headersSB.AppendFormat("{0}={1}", item.Key, item.Value);
            }

            return string.Format("Headers:\r\n{0}\r\n\r\nContent:\r\n{1}", headersSB, requestJson);
        }
        
        #endregion
        #region 发送测试内容到已连接的给药机(广播)
        private void sendToAMDMBtn_Click(object sender, EventArgs e)
        {
            WebUtils wu = new WebUtils();

            if ((string)this.requestCombox.SelectedItem == "√自定义信息(上方输入框内容)")
            {
                try
                {
                    string requestJson = sendMsgTextBox.Text;

                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    headers.Add("apiName", "unknow/customMsg");

                    this.sendMsgTextBox.Text = requestJson;// requestJson;/// buildTextboxText(headers, requestJson);
                    try
                    {
                        wu.Timeout = 500;
                        string rsp = wu.DoPost(AMDMServerAddress, Encoding.UTF8.GetBytes(requestJson), "application/json", headers);
                        string log = string.Format("√自定义信息(上方输入框内容)\r\n返回值:{0}", rsp);
                        this.updTextBox(log, logTextbox);
                    }
                    catch (Exception doPostErr)
                    {
                        string log = string.Format("√自定义信息(上方输入框内容) 请求错误:{0}\r\n", doPostErr.Message);
                        this.updTextBox(log, logTextbox);
                    }
                }
                catch (Exception doPostErr)
                {
                    string log = string.Format("发送post请求到本机http服务器错误:{0}", doPostErr.Message);
                    this.updTextBox(log, logTextbox);
                }
            }
            else
            {
                string rsp = this.GetRequestJsonBySelectedRequestName(string.Format("{0}", this.requestCombox.SelectedItem));
                this.updTextBox(rsp, this.receiveMsgTextbox);
            }


            
        }
        #endregion


        

        private void requestCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedItem = requestCombox.SelectedItem;
            string selectedItemStr = string.Format("{0}", selectedItem);
            this.sendToHTTPServerBtn.Enabled = false;
            this.sendToAMDMBtn.Enabled = false;
            if (selectedItemStr.Contains("自定义信息(上方输入框内容)"))
            {
                this.sendToHTTPServerBtn.Enabled = true;
                this.sendToAMDMBtn.Enabled = true;
            }
            else if (selectedItemStr.Contains("√") && selectedItemStr.Contains("付药机->HIS"))
            {
                this.sendToHTTPServerBtn.Enabled = true;
            }
            else if(selectedItemStr.Contains("√") && selectedItemStr.Contains("HIS->付药机"))
            {
                this.sendToAMDMBtn.Enabled = true;
            }
        }

        private void AMDMAddressTextbox_TextChanged(object sender, EventArgs e)
        {
            this.AMDMServerAddress = AMDMAddressTextbox.Text;
        }

        private void httpServerAddressTextbox_TextChanged(object sender, EventArgs e)
        {
            this.HTTPServerAddress = httpServerAddressTextbox.Text;
        }

        private void button1_TextChanged(object sender, EventArgs e)
        {

        }

        private void reStartHpptServerBtn_Click(object sender, EventArgs e)
        {
            if (this.reStartHpptServerBtn.Text == startedHTTPServerBtnText)
            {
                this.server.Stop();
                //停止
                this.reStartHpptServerBtn.Text = stopedHTTPServerBtnText;
            }
            else
            {
                //this.server.Stop();
                this.server.Start(this.HTTPServerAddress);
                this.reStartHpptServerBtn.Text = startedHTTPServerBtnText;
            }
        }

        private void printerListCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.printerName = string.Format("{0}", this.printerListCombox.SelectedItem);
        }

        #region 工具函数
        /// <summary>
        /// 获取打印机列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetPrintList()
        {
            List<string> lt = new List<string>();
            LocalPrintServer printServer = new LocalPrintServer();
            PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local });
            foreach (PrintQueue printer in printQueuesOnLocalServer)
                lt.Add(printer.Name);
            return lt;
        }
        #endregion

        #region 连接和断开sql
        private void updateSQLserverSettingAndTryConnectBtn_Click(object sender, EventArgs e)
        {
            #region 连接his的mysql 服务器
            int ret = -1;
            string ip, user, pass, database; int port;
            if (string.IsNullOrEmpty(this.sqlServerAddressTextbox.Text) || string.IsNullOrEmpty(sqlServerDatabaseTextbox.Text))
            {
                showLog("", "sql服务器设置错误");
                return;
            }
            if (string.IsNullOrEmpty(this.sqlServerUserTextbox.Text) || string.IsNullOrEmpty(this.sqlServerPassTextbox.Text))
            {
                showLog("", "用户名或密码填写错误");
                return;
            }
            if (int.TryParse(this.sqlServerPortTextbox.Text, out port) == false || port > 65535 || port < 1)
            {
                showLog("", "sql服务器端口号设置错误");
                return;
            }
            ip = this.sqlServerAddressTextbox.Text.Trim();
            user = this.sqlServerUserTextbox.Text.Trim();
            pass = this.sqlServerPassTextbox.Text.Trim();
            database = this.sqlServerDatabaseTextbox.Text.Trim();

            sqlClient = new MysqlClient(ip, user, pass, database, port);
            try
            {
                ret = sqlClient.MysqlExecute("use his_server");
            }
            catch (Exception err)
            {
                showLog("", string.Format("数据库通讯错误:\r\n{0}", err.Message));
                //Application.Exit();
                //return;
            }

            if (ret == 0)
            {
                this.Text = "HIS服务器-数据库已连接";
            }
            else
            {
                this.Text = "HIS服务器-尚未连接数据库...";
                showLog("", "服务器数据库连接失败!!!");
                //Application.Exit();
            }

            #endregion
        }
        #endregion

        #region 创建取空当前药仓的付药单
        private void createAllCurrentInventoryMedicineOrderBtn_Click(object sender, EventArgs e)
        {
            WebUtils wu = new WebUtils();
            GetCurrentAllInventoryRequest req = new GetCurrentAllInventoryRequest();
            //req.Fields = "id,name,barcode,idofhis";
            req.StockIndex = 0;
            req.AddOtherParameter("method", req.GetApiName());
            try
            {
                //获取所有的当前药品库存
                string retJson = wu.DoGet(this.AMDMServerAddress, req.GetParameters());                
                //解析药品库存获取的rspjson为对象
                GetCurrentAllInventoryResponse rsp = Newtonsoft.Json.JsonConvert.DeserializeObject<GetCurrentAllInventoryResponse>(retJson);
                #region 创建付药单
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
                order.Id = sqlClient.InsertData<FakeHISMedicineInfo>(ipr);

                List<object> details = new List<object>();
                for (int i = 0; i < rsp.Inventory.Count; i++)
                {
                    AMDM_MedicineInventory current = rsp.Inventory[i];                    
                    HISMedicineOrderDetail detail = new HISMedicineOrderDetail()
                    {
                        Canceled = false,
                        Barcode = current.Barcode,
                        OrderId = order.Id,
                        Fulfilled = order.Fulfilled,
                        FulfilledTime = order.FulfilledTime,
                        FulfilledNurseId = order.Fulfilled ? (Nullable<long>)order.FulfillmentNurseId : null,
                        FulfilledPharmacyId = order.Fulfilled ? (Nullable<long>)order.PharmacyId : null,
                        MedicineId  = Convert.ToInt64(current.IdOfHIS),
                        Name = current.Name,
                        Count = current.Count
                    };


                    SqlInsertRecordParams detailIPR = new SqlInsertRecordParams();
                    detailIPR.TableName = "medicine_order_detail";
                    detailIPR.DataObject = detail;
                    detail.Id = sqlClient.InsertData<HISMedicineOrderDetail>(detailIPR);

                    if (order.Details == null)
                    {
                        order.Details = new List<HISMedicineOrderDetail>();
                    }
                    order.Details.Add(detail);

                    order.TotalCount += detail.Count;
                    order.EntriesCount++;
                }


                SqlObjectUpdaterParamters upr = new SqlObjectUpdaterParamters();
                upr.TableName = "medicine_order";
                upr.WhereEquals.Add("id", order.Id);
                upr.UpdateFieldNameAndValues.Add("entriesCount", order.EntriesCount);
                upr.UpdateFieldNameAndValues.Add("totalCount", order.TotalCount);
                bool updateCountRet = sqlClient.UpdateData(upr);
                if (!updateCountRet)
                {
                    showLog("", "插入测试付药单失败");
                    return;
                }
                string json = JsonConvert.SerializeObject(order, jsonSetting);
                showLog("", json);
                #endregion
            }
            catch (Exception err)
            {
                showLog(err.Message, err.StackTrace);
            }
        }
        #endregion

        #region 显示日志信息
        delegate void showLogFunc(string title, string msg, ConsoleColor color = ConsoleColor.White);
        void showLog(string title, string msg, ConsoleColor color = ConsoleColor.White)
        {
            if (this.logTextbox.InvokeRequired)
            {
                showLogFunc fc = new showLogFunc(showLog);
                this.logTextbox.BeginInvoke(fc, title, msg, color);
                return;
            }
            string line = string.Format("{0}\r\n标题:{1}消息:{2}\r\n", DateTime.Now.ToString("HH:mm:ss"), title, msg);
            this.logTextbox.AppendText(line);
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ResetColor();
        }
        #endregion
    }
}
