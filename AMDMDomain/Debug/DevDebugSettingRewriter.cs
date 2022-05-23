using System;
using System.Collections.Generic;
using System.Text;

    public static class DevDebugSettingRewriter
    {
        public static DevSetting Setting { get; set; }
        public static void GetSetting(string[] args)
        {
            if (args!= null)
            {
                Dictionary<string, string> argsDic = new Dictionary<string, string>();
                #region 转换成字典
                foreach (var argLine in args)
                {
                    string[] kv = argLine.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (kv != null && kv.Length == 2)
                    {
                        string k = kv[0].ToLower();
                        string v = kv[1];
                        if (argsDic.ContainsKey(k) == false)
                        {
                            argsDic.Add(k, v);
                        }
                        else
                        {
                            argsDic[k] = v;
                        }
                    }
                }
                #endregion
                if (argsDic.ContainsKey("pc"))
                {
                    string pc = argsDic["pc"];
                    switch (pc.ToLower())
                    {
                        case "chaokapc":
                            Setting = new DevSetting()
                            { 
                                AMDMClientIP = "192.168.2.191",
                                PlcPort = 20108,
                                PlcIP = "192.168.2.192",
                                 HISServerIP = "192.168.2.222",
                                HISServerSQLPort = 3306,
                                AMDMServerIP = "192.168.2.191",
                                AMDMServerSQLPort = 10000,
                            };
                            break;
                        case "homemac":
                            Setting = new DevSetting()
                            {
                                AMDMClientIP = "10.10.10.17",
                                PlcPort = 20108,
                                PlcIP = "10.10.10.17",
                                 HISServerIP = "10.10.10.17",
                                HISServerSQLPort = 3306, AMDMServerIP = "10.10.10.17", AMDMServerSQLPort = 10000,
                            };
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    public class DevSetting
    {
        public string PlcIP { get; set; }
        public int PlcPort { get; set; }
        public string HISServerIP { get; set; }
        public int HISServerSQLPort { get; set; }
        public string AMDMClientIP { get; set; }
        public string AMDMServerIP { get; set; }
        public int AMDMServerSQLPort { get; set; }
    }