using AMDM_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDM
{
    public class MedicineMonitor
    {
        OnMedicineObjectExpirationAlertEventHandler onExpirationAlert;
        OnMedicineCountAlertEventHandler onCountAlert;
        public MedicineMonitor(OnMedicineObjectExpirationAlertEventHandler onExpirationAlert, OnMedicineCountAlertEventHandler onCountAlert)
        {
            this.ExpirationAlertMedicineObjects = new Dictionary<long, AMDM_MedicineObject__Grid__Medicine>();
            this.onExpirationAlert = onExpirationAlert;
            this.onCountAlert = onCountAlert;
        }
        /// <summary>
        /// 将到有效期预警的药品集合(包含达到提醒值和已经过期的)
        /// </summary>
        public Dictionary<long, AMDM_MedicineObject__Grid__Medicine> ExpirationAlertMedicineObjects { get; set; }
        /// <summary>
        /// 库存达到预警阈值的药品集合.key是药品的id,value是药品(带有库存信息)
        /// </summary>
        public Dictionary<long, AMDM_MedicineInventory> CountAlertAMDM_MedicinesInventory { get; set; }
        /// <summary>
        /// 检查药品的有效期
        /// </summary>
        public void CheckMedicinesExpiration()
        {
            this.ExpirationAlertMedicineObjects.Clear();
            List<AMDM_MedicineObject__Grid__Medicine> objects = App.medicineManager.GetMedicineObjectsByExpirationLimit(App.Setting.ExpirationStrictControlSetting.DefaultDaysThresholdOfExpirationAlert);
            if (objects.Count>0)
            {
                Utils.LogInfo("检测到将要进行提醒的药品数量一共有:", objects.Count);
                int timeOutObjsCount = 0;
                foreach (var item in objects)
                {
                    if (item.ExpirationDate!= null && item.ExpirationDate<DateTime.Now)
                    {
                        timeOutObjsCount++;
                    }
                    try
                    {
                        if (this.ExpirationAlertMedicineObjects.ContainsKey(item.Id) == false)
                        {
                            this.ExpirationAlertMedicineObjects.Add(item.Id, item);
                        }
                    }
                    catch (Exception err)
                    {
                        Utils.LogError("检测到有效期达到预警值的药品,但向ExpiredMedicineObjects中添加数据错误", err.Message);
                    }
                }
                if (timeOutObjsCount > 0)
                {
                    Utils.LogInfo("检测到已经过了有效期的药品有:", timeOutObjsCount);
                }
                //进行提醒报送
                if (onExpirationAlert != null)
                {
                    try
                    {
                        onExpirationAlert(this.ExpirationAlertMedicineObjects);
                    }
                    catch (Exception err)
                    {
                        Utils.LogError("在药品检查器中检查药品有效期后发送报送消息错误:", err.Message);
                    }

                }
            }
        }
        /// <summary>
        /// 检查药品的库存.
        /// </summary>
        public void CheckMedicinesInventory()
        {
            this.CountAlertAMDM_MedicinesInventory.Clear();
            List<AMDM_MedicineInventory> lowInventoryMedicines = App.medicineManager.GetLowInventoryMedicines(App.Setting.DefaultCounttThredholdOfLowInventoryAlert);
            if (lowInventoryMedicines.Count>0)
            {
                foreach (var item in lowInventoryMedicines)
                {
                    if (this.CountAlertAMDM_MedicinesInventory.ContainsKey(item.Id) == false)
                    {
                        this.CountAlertAMDM_MedicinesInventory.Add(item.Id, item);
                    }
                }
                Utils.LogInfo("检测到低库存的商品种类数:", lowInventoryMedicines.Count);
                if (onCountAlert != null)
                {
                    try
                    {
                        onCountAlert(this.CountAlertAMDM_MedicinesInventory);
                    }
                    catch (Exception err)
                    {
                        Utils.LogError("在药品检查器中检查药品库存后发送报送消息错误:", err.Message);
                    }
                }
            }
        }
    }
}
