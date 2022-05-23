using AMDM_Domain;
using MyCode.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;

/*
 * 2021年11月19日09:30:52 开始构建付药单据打印组件
 * 该组件必须包含打印函数和打印任务完成的回调函数以及打印错误的回调函数.
 * 正常情况,当打印开始,就开始检测打印机的状态和任务,如果打印超过1分钟还存在打印任务或者是打印任务在1分钟内发生了故障信号,则通知外部已经发生故障,发送信息给his系统,让his系统通知护士进行付药机的维护
 */
namespace AMDM.Manager
{
    #region 打印机状态枚举
    public enum PrinterStatusEnum
    {
        其他 = 1,
        未知 = 2,
        空闲 = 3,
        打印 = 4,
        预热 = 5,
        停止打印 = 6,
        脱机 = 7,
    }
    #endregion
    
    /// <summary>
    /// 付药单据打印组件
    /// </summary>
    public class DeliveryRecordPaperPrinter
    {
        #region 构造函数
        public DeliveryRecordPaperPrinter()
        {
            try
            {
                this.PrinterStatus = this.GetPrinterPrinterStatus(App.Setting.DevicesSetting.Printer58MMSetting.PrinterName);
            }
            catch (Exception err)
            {
                Utils.LogError("获取打印机状态时发生错误:", err.Message, err.StackTrace);
            }
        }
        #endregion
        #region 全局变量
        /// <summary>
        /// 当前打印任务是否正在进行中,如果正在打印或者是打印任务表中的记录数大于0,就是正在忙线中.不能打印
        /// </summary>
        public PrinterStatusEnum PrinterStatus { get; set; }
        #endregion
        /// <summary>
        /// 打印付药单
        /// </summary>
        /// <param name="record"></param>
        public bool Print(AMDM_DeliveryRecord record)
        {
            this.PrinterStatus = GetPrinterPrinterStatus(App.Setting.DevicesSetting.Printer58MMSetting.PrinterName);
            
            ImageCompose2 paper = this.CreateDeliveryRecordPaper(record);
            if (this.PrinterStatus != PrinterStatusEnum.空闲)
            {
                Utils.LogFail("打印机不在空闲状态,无法执行打印任务", this.PrinterStatus);
            }
            else
            {
                paper.PrintToPrinter(App.Setting.DevicesSetting.Printer58MMSetting.PrinterName, string.Format("付药单:{0}", record.Id));
            }
            #region 如果指定了还要把图片保存到文件中
            
            
            if (App.Setting.DevicesSetting.Printer58MMSetting.SaveBackupImage)
            {
                Utils.LogStarted("需要把详单保存到图片");
                try
                {
                    if (System.IO.Directory.Exists(App.Setting.DevicesSetting.Printer58MMSetting.SaveBackupImageFileDir) == false)
                    {
                        System.IO.Directory.CreateDirectory(App.Setting.DevicesSetting.Printer58MMSetting.SaveBackupImageFileDir);
                    }
                    string fileExt = "bmp";
                    string realFilePath = string.Format("{0}\\{1}_{2}_{3}.{4}",
                        App.Setting.DevicesSetting.Printer58MMSetting.SaveBackupImageFileDir.TrimEnd('\\'),
                        record.Id,
                        SnapshotLocationEnum.DeliveryRecordPaper.ToString(),
                        DateTime.Now.Ticks,
                        fileExt
                        );
                    using (Bitmap bmp = new Bitmap(paper.Width, paper.Height))
                    {
                        Graphics g = Graphics.FromImage(bmp);
                        g.PageUnit = GraphicsUnit.Millimeter;
                        paper.PrintToImage(Graphics.FromImage(bmp));
                        bmp.Save(realFilePath);
                    }

                    if (App.sqlClient != null)
                    {
                        App.sqlClient.AddSnapshot(SnapshotParentTypeEnum.DeliveryRecord,
                    record.Id,
                    SnapshotTimePointEnum.OnAction,
                    SnapshotLocationEnum.DeliveryRecordPaper,
                    DateTime.Now,
                    "药品出仓打印小票",
                    realFilePath);
                    }
                }
                catch (Exception save2FileErr)
                {
                    Utils.LogError("在DeliveryRecordPaperPrinter中保存图片失败:", save2FileErr.Message);
                }
            }
            #endregion
            return true;
        }
        #region 私有函数,创建一个打印对象
        ImageCompose2 CreateDeliveryRecordPaper(AMDM_DeliveryRecord record)
        {
            ImageCompose2 paper = new ImageCompose2(58);
            Tag root = new Tag();
            root.width = 1;
            root.border = new Tag.BorderBound() { color = Color.Red, dashStyle = System.Drawing.Drawing2D.DashStyle.Dash, width =1 };
            root.flexDirection = FlexDirectionEnum.col;
            //root.height = 1;
            root.id = "根元素";
            root.justifyContent = JustifyContentEnum.center;

            paper.LoadRootTag(root);

            Utils.LogSuccess("已完成根元素的加载");

            #region 添加一个标题行
            Text titleTag = (Text)addTextLine(root, "自动取药机付药单", 16, true);
            titleTag.font = new Font("隶书", 14);
            #endregion
            Utils.LogSuccess("添加完标题行");

            #region 添加付药单的基础信息行
            //开始时间 结束时间 处方编号 流水号 药品总数
            addTextLine(root, string.Format("开始时间:{0}", record.StartTime.ToString("yyyy-MM-dd HH:mm:ss")), 9, true, false);
            addTextLine(root, string.Format("结束时间:{0}", record.EndTime == null? "未完成": record.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss")), 9, true, false);
            addTextLine(root, string.Format("处方编号:{0}", record.PrescriptionId.ToString()), 9, true, false);
            addTextLine(root, string.Format("流 水 号:{0}", record.Id.ToString()), 9, true, false);
            addTextLine(root, string.Format("药品总数:{0}", record.TotalMedicineCount.ToString()), 9, true, false);
            #endregion
            Utils.LogSuccess("添加完基础信息行");
            #region 添加截图行
            if (string.IsNullOrEmpty(record.SnapshotImageFile) == false && System.IO.File.Exists(record.SnapshotImageFile))
            {
                Utils.LogInfo("读取图片", record.SnapshotImageFile);
                Img img = new Img();
                Image bmp = null;
                try
                {
                    Image bmpFull = Bitmap.FromFile(record.SnapshotImageFile);
                    bmp = bmpFull;
                }
                catch (Exception loadBmpErr)
                {
                    Utils.LogFail("读取图片文件错误",loadBmpErr.Message,loadBmpErr.StackTrace, record.SnapshotImageFile);
                }
                img.image = bmp;
                img.width = 1;
                //img.height = 1;
                img.border = new Tag.BorderBound() { width = 1, color = Color.Red, dashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
                root.AddChirld(img);
            }
            else
            {
                Utils.LogWarnning("没有图片信息或图片不存在", record.SnapshotImageFile);
            }
            #endregion
            #region 添加付药单的明细信息行
            //本身记录是每一个药品一次记录的,但是打印的时候直接打印每一个药品的总数即可
            Dictionary<long, AMDM_DeliveryRecordDetail_data> detailsDic = new Dictionary<long, AMDM_DeliveryRecordDetail_data>();
            foreach (var detail in record.Details)
            {
                if (detailsDic.ContainsKey(detail.MedicineId) == false)
                {
                    AMDM_DeliveryRecordDetail_data newD = new AMDM_DeliveryRecordDetail_data();
                    Newtonsoft.Json.JsonConvert.PopulateObject(Newtonsoft.Json.JsonConvert.SerializeObject(detail), newD);
                    newD.Count = 0;
                    detailsDic.Add(newD.MedicineId, newD);
                }
                detailsDic[detail.MedicineId].Count += detail.Count;
            }
            Utils.LogSuccess("付药单明细归纳完成,明细条目数量:", detailsDic.Count);

            #region 明细标题行
            Tag detailsTitleLine = new Tag();
            detailsTitleLine.width = 1;
            detailsTitleLine.flexDirection = FlexDirectionEnum.row;
            Font titleFont = new Font("gulim", 7);
            Text detailsTitleLineIndex = new Text();
            detailsTitleLineIndex.font = titleFont;
            detailsTitleLineIndex.value = "序号";
            detailsTitleLineIndex.width = 0.2f;

            Text detailsTitleLineTitle = new Text();
            detailsTitleLineTitle.font = titleFont;
            detailsTitleLineTitle.value = "药品名称";
            detailsTitleLineTitle.width = 0.6f;

            Text detailsTitleLineCount = new Text();
            detailsTitleLineCount.font = titleFont;
            detailsTitleLineCount.value = "数量";
            detailsTitleLineCount.format = new StringFormat() { Alignment = StringAlignment.Far };
            detailsTitleLineCount.width = 0.2f;

            detailsTitleLine.AddChirld(detailsTitleLineIndex);
            detailsTitleLine.AddChirld(detailsTitleLineTitle);
            detailsTitleLine.AddChirld(detailsTitleLineCount);

            root.AddChirld(detailsTitleLine);
            #endregion
            Utils.LogSuccess("添加明细标题行完成");
            int detailIndex = 1;
            foreach (var detail in detailsDic)
            {
                Tag detailLine = new Tag();
                detailLine.width = 1;
                detailLine.flexDirection = FlexDirectionEnum.row;
                detailLine.border = new Tag.BorderBound() { width = 1, dashStyle = System.Drawing.Drawing2D.DashStyle.Dash, color = Color.Red };

                Text index = new Text();
                index.font = titleFont;
                index.width = 0.15f;
                index.value = detailIndex++;

                Text title = new Text();
                title.font = titleFont;
                title.width = 0.7f;
                title.value = detail.Value.MedicineName;

                Text count = new Text();
                count.format = new StringFormat() { Alignment = StringAlignment.Center };
                count.font = titleFont;
                count.width = 0.15f;
                count.value = detail.Value.Count;


                detailLine.AddChirld(index);
                detailLine.AddChirld(title);
                detailLine.AddChirld(count);

                root.AddChirld(detailLine);
            }
            #endregion
            Utils.LogSuccess("添加所有明细内容行完成");
            #region 添加打印时间行
            addTextLine(root, string.Format("打印时间:{0}",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), 6, true, true);
            //增加一个空白行
            addTextLine(root, "---------------------", 6, true, true);
            #endregion
            #region 添加广告行
            addTextLine(root, "河北潮咖医疗自动付药机", 9, true, true);
            #endregion
            #region 添加提示信息行
            addTextLine(root, "*********给钱的药业提示您*********", 6, true, true);
            addTextLine(root, "*********请核对处方*********", 6, true, true);
            addTextLine(root, "*****付药单以及取到的药品*****", 6, true, true);
            addTextLine(root, "*****一致后方可用药*****", 6, true, true);
            #endregion
            Utils.LogSuccess("添加打印时间 广告 提示语行完成");

            return paper;
        }
        /// <summary>
        /// 向一个元素中添加一行文字
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <param name="isFullLine"></param>
        Tag addTextLine(Tag parent,string text, float fontSize, bool isFullLine, bool center = true)
        {
            Text tag = new Text();
            tag.font = new Font("gulim", fontSize);
            tag.fontColor = Color.Black;
            tag.value = text;
            tag.width = isFullLine ? (Nullable<int>)1 : null;
            tag.format = new StringFormat() { Alignment = center?StringAlignment.Center : StringAlignment.Near};
            parent.AddChirld(tag);
            return tag;
        }
        #endregion
        #region 私有函数 打印机任务和状态的获取


        /// <summary>
        /// 获取打印机的当前状态
        /// </summary>
        /// <param name="PrinterDevice">打印机设备名称</param>
        /// <returns>打印机状态</returns>
        private PrinterStatusEnum GetPrinterPrinterStatus(string PrinterDevice)
        {
            PrinterStatusEnum ret = 0;
            string path = @"win32_printer.DeviceId='" + PrinterDevice + "'";
            ManagementObject printer = new ManagementObject(path);
            try
            {
                printer.Get();
                ret = (PrinterStatusEnum)Convert.ToInt32(printer.Properties["PrinterStatus"].Value);
            }
            catch (Exception err)
            {
                Utils.LogError("在服药记录打印组件中获取打印机状态失败:", err.Message);
            }
            return ret;
        }

        /// <summary>
        /// 检查打印机是否在线状态
        /// </summary>
        /// <param name="BindPrintName"></param>
        /// <returns></returns>
        public bool CheckPrinterIsOnline(string BindPrintName)
        {
            ManagementScope scope = new ManagementScope(@"\root\cimv2");
            scope.Connect();

            // Select Printers from WMI Object Collections
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");

            string printerName = "";
            foreach (ManagementObject printer in searcher.Get())
            {
                printerName = printer["Name"].ToString().ToLower();
                if (printerName.IndexOf(BindPrintName.ToLower()) > -1)
                {
                    //foreach (var property in printer.Properties)
                    //{
                    //    LogUtil.WriteLog(property.Name + ":" + property.Value);
                    //}
                    if (printer["WorkOffline"].ToString().ToLower().Equals("true"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion
    }
}
