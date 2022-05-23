using AMDM_Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
/*
 * 控制面板的接口服务器.
 * 提供对机器的状态的获取能力和对机器的相关简单控制能力.
 * 比如查看机器当前的状态,查看每个药仓的当前温度,查看每个药仓的当前风扇开启状态,每个药仓的紫外杀菌状态,每个药仓的各种阈值设置等.
 */
namespace AMDM
{
    /// <summary>
    /// 控制面板接口服务器
    /// </summary>
    public class ControlPanelInterfaceServer
    {
        #region 构造函数和初始化
        
        public ControlPanelInterfaceServer()
        {
            this.httpServer = new HTTPServer();
            this.httpServer.OnReciveMessage += httpServer_OnReciveMessage;
        }

        string createErrRsp(string errmsg)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IsError", true);
            dic.Add("ErrMsg", errmsg);

            string json = JsonConvert.SerializeObject(dic);
            return json;
        }

        
        #endregion

        #region 对消息的处理
        string httpServer_OnReciveMessage(string msg, object requestRef, object responseRef)
        {
            //在这里处理发过来的消息然后返回一个response的序列化后的json

            HttpListenerRequest request = requestRef as HttpListenerRequest;
            if (request.HttpMethod == "GET" && msg == null)
            {
                //get请求的处理
            }
            else
            {

            }
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
            //string log = string.Format("Headers:\r\n{0}\r\n\r\n Content:\r\n{1}", headersSB, msg);
            //this.updTextBox(log, this.receiveMsgTextbox);
            //Utils.LogInfo(log);

            //正式处理请求
            #region 只处理contentType为 application/json的消息
            if (request.ContentType.ToLower().Contains("application/json"))
            {
                Dictionary<string, object> jsonKV = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
                #region 根据header中的apiName决定要把msg序列化成什么格式然后进行处理
                string apiName = request.Headers["apiName"];
                if (string.IsNullOrEmpty(apiName))
                {
                    return createErrRsp("无效的api名称,请在请求的headers中指定apiName");
                }
                switch (apiName.ToLower())
                {
                    #region 正式处理每一个请求.
                    #region 获取状态
                    case "status.get":
                        try
                        {
                            AMDMStatusGetRequest req = new AMDMStatusGetRequest();
                            JsonConvert.PopulateObject(msg, req);
                            AMDMStatusGetResponse rsp = new AMDMStatusGetResponse();
                            if (req.ForceRefresh)
                            {
                                App.ControlPanel.ReloadStatus();
                            }
                            rsp.PeripheralsStatus = App.ControlPanel.PeripheralsStatus;
                            rsp.MaintenanceStatus = App.ControlPanel.MaintenanceStatus;
                            rsp.TodayPrescriptionCount = App.ControlPanel.TodayPrescriptionCount;
                            rsp.TodayMedicineCount = App.ControlPanel.TodayMedicineCount;
                            
                            string rspJson = JsonConvert.SerializeObject(rsp, Formatting.Indented);
                            return rspJson;
                        }
                        catch (Exception err)
                        {
                            string errMsg = string.Format("处理获取状态的请求发生错误:{0}", err.Message);
                            Utils.LogError(errMsg);
                            return createErrRsp(errMsg);
                        }
                    #endregion
                    #region 设置空调的设置
                    case "peripheralsstatus.ac.set":
                        try
                        {
                            AMDMPeripheralsStatusACSetRequest req = new AMDMPeripheralsStatusACSetRequest();
                            JsonConvert.PopulateObject(msg, req);
                            if (req.StockIndex == null || (req.IsWorking == null && req.DestTemperature == null))
                            {
                                return createErrRsp("参数错误,需指定有效的药仓序号和新状态");
                            }
                            #region 目标温度变更
                            if (req.DestTemperature!= null)
                            {
                                if(App.ControlPanel.SetACDestTemperature(req.StockIndex.Value, req.DestTemperature.Value))
                                {
                                    Utils.LogInfo("已经为网络请求重新设置了温度", req);
                                }
                                else
                                {
                                    return createErrRsp("设置空调温度错误");
                                }
                            }
                            #endregion
                            #region 状态变更

                            if (req.IsWorking!= null)
                            {
                                if (req.IsWorking.Value)
                                {
                                    if (App.ControlPanel.TurnOnAC(req.StockIndex.Value))
                                    {

                                    }
                                    else
                                    {
                                        return createErrRsp("开空调操作执行错误");
                                    }
                                }
                                else
                                {
                                    if (App.ControlPanel.TurnOffAC(req.StockIndex.Value))
                                    {

                                    }
                                    else
                                    {
                                        return createErrRsp("关闭空调操作执行错误");
                                    }
                                }
                            }

                            #endregion
                            App.ControlPanel.ReloadStatus();
                            AMDMPeripheralsStatusACSetResponse rsp = new AMDMPeripheralsStatusACSetResponse();
                            foreach (var item in App.ControlPanel.PeripheralsStatus.WarehousesACStatus)
                            {
                                if (item.WarehouseIndexId == req.StockIndex)
                                {
                                    rsp.IsWorking = item.IsACWorking;
                                    rsp.CurrentTemperature = item.CurrentTemperature;
                                    rsp.DestTemperature = item.DestTemperature;
                                    break;
                                }
                            }
                            Utils.LogInfo("新的药仓信息:", rsp);
                            string rspJson = JsonConvert.SerializeObject(rsp, Formatting.Indented);
                            return rspJson;
                        }
                        catch (Exception err)
                        {
                            string errMsg = string.Format("处理获取状态的请求发生错误:{0}", err.Message);
                            Utils.LogError(errMsg);
                            return createErrRsp(errMsg);
                        }
                    #endregion
                    #region 紫外线灯开关
                    case "peripheralsstatus.uvlamp.onoff":
                        try
                        {
                            AMDMPeripheralsStatusUVLampTurnOnOffRequest req = new AMDMPeripheralsStatusUVLampTurnOnOffRequest();
                            JsonConvert.PopulateObject(msg, req);
                            
                            if (req.On == null)
                            {
                                return createErrRsp("输入参数错误,必须指定紫外线灯的新状态");
                            }
                            if (req.On.Value)
                            {
                                if (req.AutoTurnOffDelayMM == null)
                                {
                                    return createErrRsp("为避免伤害,必须设定本次打开紫外线灯的工作时间长度(毫秒)");
                                }
                                App.ControlPanel.TurnOnUVLamp(req.AutoTurnOffDelayMM.Value);
                            }
                            else
                            {
                                App.ControlPanel.TurnOffUVLamp();
                            }
                            App.ControlPanel.ReloadStatus();
                            AMDMPeripheralsStatusUVLampTurnOnOffResponse rsp = new AMDMPeripheralsStatusUVLampTurnOnOffResponse();
                            rsp.On = App.ControlPanel.PeripheralsStatus.UVLampIsWorking;
                            string rspJson = JsonConvert.SerializeObject(rsp, Formatting.Indented);
                            return rspJson;
                        }
                        catch (Exception err)
                        {
                            string errMsg = string.Format("处理紫外线灯的状态切换请求错误:{0}", err.Message);
                            Utils.LogError(errMsg);
                            return createErrRsp(errMsg);
                        }
                    #endregion
                    #region 紫外线灯的自动开关时间设置
                    //UV lamp automatic working time

                    case "peripherals.uvlamp.onofftime.set":
                        //2022年3月20日16:40:50 当前是没有在下位机上设置定时功能,直接在上位机上进行处理.
                        //设定的时候直接更新本类中的变量,在更新了本类中的变量以后,设置app.setting里面的相应字段
                        try
                        {
                            AMDMPeripheralsSetUVLampOnOffTimeRequest req = new AMDMPeripheralsSetUVLampOnOffTimeRequest();
                            JsonConvert.PopulateObject(msg, req);
                            if (req.UVLampOffTime == null && req.UVLampOnTime == null)
                            {
                                return createErrRsp("请输入有效的开启和关闭时间");
                            }
                            if (req.UVLampOnTime.Length != 5 && req.UVLampOffTime.Length != 5)
                            {
                                return createErrRsp("输入的时间格式不正确,应为HH:mm格式");
                            }
                            string startStr = string.Format("1970-01-01 {0}", req.UVLampOnTime);
                            string endStr = string.Format("1970-01-01 {0}", req.UVLampOffTime);
                            DateTime start = DateTime.Parse(startStr);
                            DateTime end = DateTime.Parse(endStr);
                            App.Setting.DevicesSetting.UVLampSetting.UVLampOffTime = end;
                            App.Setting.DevicesSetting.UVLampSetting.UVLampOnTime = start;
                            AMDMPeripheralsSetUVLampOnOffTimeResponse rsp = new AMDMPeripheralsSetUVLampOnOffTimeResponse();
                            rsp.UVLampOffTime = end.ToString("HH:mm");
                            rsp.UVLampOnTime = start.ToString("HH:mm");
                            string rspJson = JsonConvert.SerializeObject(rsp, Formatting.Indented);
                            App.Setting.Save();
                            return rspJson;
                        }
                        catch (Exception err)
                        {
                            string errMsg = string.Format("设置紫外线灯的自动开关时间错误:{0}", err.Message);
                            Utils.LogError(errMsg);
                            return createErrRsp(errMsg);
                        }
                    #endregion
                    #region 获取取药任务工作状态
                    case "workingstatus.get":
                        try
                        {
                            AMDMWorkingStatusGetRequest req = new AMDMWorkingStatusGetRequest();
                            JsonConvert.PopulateObject(msg, req);
                            if (App.ControlPanel.GetShowingPage == null)
                            {
                                return createErrRsp("取药功能尚未就绪");
                            }
                            var currentPage = App.ControlPanel.GetShowingPage();
                            AMDMWorkingStatusGetResponse rsp = new AMDMWorkingStatusGetResponse();
                            rsp.IsWorking = currentPage != ShowingPageEnum.空闲播放广告中;
                            rsp.StatusName = currentPage.ToString();
                            string rspJson = JsonConvert.SerializeObject(rsp, Formatting.Indented);
                            return rspJson;
                        }
                        catch (Exception err)
                        {
                            string errMsg = string.Format("获取当前工作状态发生错误:{0}", err.Message);
                            Utils.LogError(errMsg);
                            return createErrRsp(errMsg);
                        }
                    #endregion
                    #region 获取AMDMSetting 付药机请求
                    case "setting.get":
                        try
                        {
                            AMDMSettingGetRequst req = new AMDMSettingGetRequst();
                            JsonConvert.PopulateObject(msg, req);
                            AMDMSettingGetResponse rsp = new AMDMSettingGetResponse();
                            rsp.AMDMSetting = App.Setting;
                            string rspJson = JsonConvert.SerializeObject(rsp, Formatting.Indented);
                            return rspJson;
                        }
                        catch (Exception err)
                        {
                            string errMsg = string.Format("获取付药机的设置发生错误:{0}", err.Message);
                            Utils.LogError(errMsg);
                            return createErrRsp(errMsg);
                        }
                    #endregion
                    #region 更新设置
                        //当用户扫描了处方二维码但是没有进行点击取药的操作的话 多久自动消失那个提示框 以及全屏提示框多久关闭等 amdmsetting的update操作
                    case "amdmsetting.update":
                        try
                        {
                            AMDMSettingUpdateRequst req = new AMDMSettingUpdateRequst();
                            JsonConvert.PopulateObject(msg, req);
                            AMDMSettingUpdateResponse rsp = new AMDMSettingUpdateResponse();
                            if (req.Field == null)
                            {
                                return createErrRsp("未指定有效的目标字段");
                            }
                            #region ui交互设置

                            if (req.Field.ToLower() == "userinterfacesetting.medicineorderautohidewhennoactionms")
                            {
                                int dest = int.Parse(string.Format("{0}", req.Value));
                                if (dest<10000 || dest >300000)
                                {
                                    return createErrRsp("设置值错误,应设置为10秒~5分钟之间");
                                }
                                App.Setting.UserInterfaceSetting.MedicineOrderAutoHideWhenNoActionMS = dest;
                                rsp.NewValue = App.Setting.UserInterfaceSetting.MedicineOrderAutoHideWhenNoActionMS;
                            }
                            else if(req.Field.ToLower() == "userinterfacesetting.noticeshowerautohidems")
                            {
                                int dest = int.Parse(string.Format("{0}", req.Value));
                                if (dest < 1000 || dest > 60*1000)
                                {
                                    return createErrRsp("设置值错误,应设置为1秒~1分钟之间");
                                }
                                App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS = dest;
                                rsp.NewValue = App.Setting.UserInterfaceSetting.NoticeShowerAutoHideMS;
                            }

                            #endregion
                            #region 药品有效期控制设置

                            else if (req.Field == "ExpirationStrictControlSetting.DefaultCanLoadMinExpirationDays")
                            {
                                int dest = int.Parse(string.Format("{0}", req.Value));
                                if (dest<0 || dest>365)
                                {
                                    return createErrRsp("设置错误,应设置为0~365天之间");
                                }
                                rsp.NewValue = App.Setting.ExpirationStrictControlSetting.DefaultCanLoadMinExpirationDays = dest;
                            }
                            else if (req.Field == "ExpirationStrictControlSetting.DefaultSuggestLoadMinExpirationDays")
                            {
                                int dest = int.Parse(string.Format("{0}", req.Value));
                                if (dest < 0 || dest > 3*365)
                                {
                                    return createErrRsp("设置错误,应设置为0天~3年之间");
                                }
                                if (dest <= App.Setting.ExpirationStrictControlSetting.DefaultCanLoadMinExpirationDays)
                                {
                                    return createErrRsp(string.Format("设置错误,建议有效期必须大于最小有效期(大于{0}天以上)", App.Setting.ExpirationStrictControlSetting.DefaultCanLoadMinExpirationDays));
                                }
                                rsp.NewValue = App.Setting.ExpirationStrictControlSetting.DefaultSuggestLoadMinExpirationDays = dest;
                            }
                            else if (req.Field == "ExpirationStrictControlSetting.DefaultDaysThresholdOfExpirationAlert")
                            {
                                int dest = int.Parse(string.Format("{0}", req.Value));
                                if (dest < 0 || dest > 3*365)
                                {
                                    return createErrRsp("设置错误,应设置为0天~3年之间");
                                }
                                rsp.NewValue = App.Setting.ExpirationStrictControlSetting.DefaultDaysThresholdOfExpirationAlert = dest;
                            }
                            else if (req.Field == "ExpirationStrictControlSetting.Enable")
                            {
                                bool dest = false;
                                if (bool.TryParse(string.Format("{0}", req.Value), out dest) == false)
                                {
                                    return createErrRsp("设置错误,无效的开关信息");
                                }
                                rsp.NewValue = App.Setting.ExpirationStrictControlSetting.Enable = dest;
                            }

                            #endregion
                            #region 故障处置方案设置
                            else if (req.Field == "TroubleshootingPlanSetting.DisableAMDMWhenDeliveryFailed")
                            {
                                bool dest = false;
                                if (bool.TryParse(string.Format("{0}", req.Value), out dest) == false)
                                {
                                    return createErrRsp("设置错误,无效的开关信息");
                                }
                                rsp.NewValue = App.Setting.TroubleshootingPlanSetting.DisableAMDMWhenDeliveryFailed = dest;
                            }
                            else if(req.Field == "TroubleshootingPlanSetting.AlertReceiveUsers")
                            {
                                string listString = string.Format("{0}", req.Value);
                                if (string.IsNullOrEmpty(listString) == true)
                                {
                                    return createErrRsp("设置错误,无效的接收人ID列表");
                                }
                                try
                                {
                                    App.Setting.TroubleshootingPlanSetting.AlertReceiveUsers = JsonConvert.DeserializeObject<List<int>>(listString);
                                }
                                catch (Exception jsonParseErr)
                                {
                                    string err = "无法解析故障接收人列表";
                                    Utils.LogError(err, req, jsonParseErr.Message);
                                    return createErrRsp(err);
                                }
                                rsp.NewValue = req.Value;
                            }
                            #endregion
                            #region 药品预警设置
                            else if(req.Field == "MedicineAlertSetting.LowInventoryAndExpirationAlertReceiveUsers")
                            {
                                string listString = string.Format("{0}", req.Value);
                                if (string.IsNullOrEmpty(listString) == true)
                                {
                                    return createErrRsp("设置错误,无效的药品预警消息接收人ID列表");
                                }
                                try
                                {
                                    App.Setting.MedicineAlertSetting.LowInventoryAndExpirationAlertReceiveUsers = JsonConvert.DeserializeObject<List<int>>(listString);
                                }
                                catch (Exception jsonParseErr)
                                {
                                    string err = "无法解析药品预警消息接收人列表";
                                    Utils.LogError(err, req, jsonParseErr.Message);
                                    return createErrRsp(err);
                                }
                                rsp.NewValue = req.Value;
                            }
                            #endregion
                            //rsp.NewValue = JsonConvert.SerializeObject(req);
                            string rspJson = JsonConvert.SerializeObject(rsp, Formatting.Indented);
                            //设置过后因为文件需要修改所以还需要再保存到文件
                            App.Setting.Save();
                            return rspJson;
                        }
                        catch (Exception err)
                        {
                            string errMsg = string.Format("远程更新药机设置错误:{0}",err.Message);
                            Utils.LogError(errMsg);
                            return createErrRsp(errMsg);
                        }
                    #endregion
                    #region 补充打印详单
                    case "deliveryrecord.reprint":
                        try
                        {
                            AMDMRePrintDeliveryRecordPaperRequest req = new AMDMRePrintDeliveryRecordPaperRequest();
                            JsonConvert.PopulateObject(msg, req);
                            if (req.DeliveryRecordId == null)
                            {
                                return createErrRsp("请指定有效的付药单编号");
                            }
                            AMDMRePrintDeliveryRecordPaperResponse rsp = new AMDMRePrintDeliveryRecordPaperResponse();
                            AMDM_DeliveryRecord record =  App.inventoryManager.GetDeliveryRecordFull(req.DeliveryRecordId.Value);
                            rsp.Success = App.DeliveryRecordPaperPrinter.Print(record);
                            string rspJson = JsonConvert.SerializeObject(rsp, Formatting.Indented);
                            return rspJson;
                        }
                        catch (Exception err)
                        {
                            string errMsg = string.Format("补充打印付药单发生错误:{0}", err.Message);
                            Utils.LogError(errMsg);
                            return createErrRsp(errMsg);
                        }
                    #endregion
                    #region 消除故障码恢复正常使用状态

                    case "status.clearerror":
                        try
                        {
                            AMDMClearErrorStatusRequest req = new AMDMClearErrorStatusRequest();
                            JsonConvert.PopulateObject(msg, req);
                            App.ControlPanel.ClearMaintenanceError();
                            AMDMClearErrorStatusResponse rsp = new AMDMClearErrorStatusResponse();
                            rsp.PeripheralsStatus = App.ControlPanel.PeripheralsStatus;
                            rsp.MaintenanceStatus = App.ControlPanel.MaintenanceStatus;

                            string rspJson = JsonConvert.SerializeObject(rsp, Formatting.Indented);
                            return rspJson;
                        }
                        catch (Exception err)
                        {
                            string errMsg = string.Format("处理清空故障状态信息时发生错误:{0}", err.Message);
                            Utils.LogError(errMsg);
                            return createErrRsp(errMsg);
                        }

                    #endregion
                    #endregion
                    default:
                        break;
                }
                #endregion
                return msg;
            }
            #endregion
            #region 其他类型的消息 是?a=b&c=d 这样的模式 原路返回这个请求的json  不进行处理.
            else
            {
                //如果发过来的不是json的话  结构整个不是json 那就看做是 ?a=b&c=d 这样的模式
                if (msg == null)
                {
                    return createErrRsp("请求无效,内容为空");
                }
                string[] formLine = msg.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                if (formLine != null && formLine.Length > 0)
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
                    string rspStr = string.Format("未处理的请求:{0}", JsonConvert.SerializeObject(dic, jsonSetting));
                    return createErrRsp(rspStr);
                }
            }
            #endregion

            return null;
        }
        #endregion

        #region 全局变量
        HTTPServer httpServer;
        JsonSerializerSettings jsonSetting = new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss" };
        #endregion

        #region 开启和关闭以及销毁
        public bool Start()
        {
            try
            {
                return this.httpServer.Start(App.Setting.ControlPanelInterfaceServerSetting.HttpServerPort
                    //,null
                    //,"*"
                    );
            }
            catch (Exception startHttpServerErr)
            {
                Utils.LogError("启动http服务器失败", startHttpServerErr.Message);
                return false;
            }
        }
        public void Stop()
        {
            try
            {
                if (this.httpServer != null)
                {
                    this.httpServer.Stop();
                }
            }
            catch (Exception stopHttpServerErr)
            {
                Utils.LogError("关闭htt服务器失败:", stopHttpServerErr.Message);
            }
        }
        public void Dispose()
        {
            try
            {
                if (this.httpServer != null)
                {
                    this.httpServer.OnReciveMessage -= this.httpServer_OnReciveMessage;
                    this.httpServer.Stop();
                }
            }
            catch (Exception disposeErr)
            {
                Utils.LogError("销毁控制面板接口服务器中的htt服务器失败", disposeErr.Message);
            }
            
        }
        #endregion
    }
}
