using AMDM;
using AMDM_Domain;
using GridAutoLayouter;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 自动付药机客户端部署工具
{
    public partial class 自动付药机客户端部署工具 : Form
    {
        AMDMClientConfiggerProjectFile file = new AMDMClientConfiggerProjectFile();
        string currentFilePath = null;
        string amdmInstallPackagePath = null;

        public 自动付药机客户端部署工具()
        {
            InitializeComponent();
            this.amdmInstallPackagePath = Application.StartupPath + "\\amdm.zip";
            file.AMDMAppInstallPath = "D:\\AMDM";
            this.amdmAPPInstallPathTextbox.Text = file.AMDMAppInstallPath;
        }

        private void deleteStockBtn_Click(object sender, EventArgs e)
        {

        }

        private void createMachineSNBtn_Click(object sender, EventArgs e)
        {
            if (this.file.Machine == null)
            {
                this.file.Machine = new AMDM_Domain.AMDM_Machine();
                this.file.Machine.Stocks = new List<AMDM_Domain.AMDM_Stock>();
            }
            bool needBackup = false;
            if (string.IsNullOrEmpty(this.file.Machine.SerialNumber) == false)
            {
                var ret = MessageBox.Show(this, string.Format("该药机已有序列号  [{0}]  \r\n如更新序列号可能会导致资产信息管理错乱\r\n仍要重新指定序列号吗?", this.file.Machine.SerialNumber),"非常规操作",
                     MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                if (ret == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
                else
                {
                    needBackup = true;
                }
            }
            this.file.Machine.SerialNumber = Utils.GetRandomSN(4, 4);
            this.machineSNLabel.Text = this.file.Machine.SerialNumber;

            if (this.file.Machine.ProductionTime == null)
            {
                this.file.Machine.ProductionTime = DateTime.Now;
                this.machineProductionTimeTextbox.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (needBackup)
            {
                saveBackupFile();
            }
            else
            {
                saveFile(false,true);
            }
        }

        private void loadProjectFromFile_Click(object sender, EventArgs e)
        {
            this.loadFile();
            if (this.file == null || this.file.Machine == null)
            {
                return;
            }
            this.file2Show(this.file);
        }

        private void saveProject2FileBtn_Click(object sender, EventArgs e)
        {
            if (this.file.Machine == null )
            {
                MessageBox.Show("空的工程无需保存");
                return;
            }

            saveFile(false,true);
        }

        void saveBackupFile()
        {
            if (string.IsNullOrEmpty(this.currentFilePath) == true)
            {
                saveFile(false,false);
            }
            else
            {
                saveFile(true,false);
            }
        }

        void saveFile(bool isBackup, bool msgBox)
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(file, Newtonsoft.Json.Formatting.Indented);
                var thisTimeSavePath = this.currentFilePath;
                //如果当前文件没保存,就要打开保存文件的对话框看保存位置
                if (string.IsNullOrEmpty(this.currentFilePath) == true)
                {
                    var dest = string.Format("自动付药机客户端部署工程文件{0}.accfg", DateTime.Now.ToString("yyyy-MM-dd-HHmmss"));
                    string fileType = "自动付药机客户端部署工程文件(*.accfg)|*.accfg";
                    SaveFileDialog sdlg = new SaveFileDialog();
                    sdlg.InitialDirectory = Application.StartupPath;
                    sdlg.Filter = fileType;
                    sdlg.FileName = currentFilePath;
                    var ret = sdlg.ShowDialog(this);
                    if (ret == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                    this.currentFilePath = sdlg.FileName;
                    thisTimeSavePath = this.currentFilePath;
                }
                else
                {
                    if (isBackup)
                    {
                        string fileShortName = System.IO.Path.GetFileNameWithoutExtension(currentFilePath);
                        string dir = System.IO.Path.GetDirectoryName(currentFilePath);
                        string backDir = string.Format("{0}\\{1}_bak", dir, fileShortName);
                        if (System.IO.Directory.Exists(backDir) == false)
                        {
                            System.IO.Directory.CreateDirectory(backDir);
                        }
                        string backFileName = string.Format("{0}\\{1}.{2}",backDir, Guid.NewGuid().ToString("N") , "accfg");
                        thisTimeSavePath = backFileName;
                    }
                }
                string path = System.IO.Path.GetDirectoryName(thisTimeSavePath);
                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                System.IO.File.WriteAllText(thisTimeSavePath, json);
                string msg = null;
                if (!isBackup)
                {
                    msg = string.Format("工程文件已保存到\r\n{0} \r\n [{1}]", this.currentFilePath,DateTime.Now); 
                }
                else
                {
                     
                    msg = string.Format("工程文件的备份文件已保存到\r\n{0} \r\n{1}", thisTimeSavePath,DateTime.Now);
                }
                if (msgBox)
                {
                    MessageBox.Show(this,msg);
                }
                else
                {
                    this.Text = msg;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("保存文件失败\r\n" + err.Message);
            }
        }
        void loadFile()
        {
            try
            {

                var dest = string.Format("自动付药机客户端部署工程文件{0}.accfg", DateTime.Now.ToString("yyyy-MM-dd-HHmmss"));
                string fileType = "自动付药机客户端部署工程文件(*.accfg)|*.accfg";
                OpenFileDialog sdlg = new OpenFileDialog();
                sdlg.InitialDirectory = Application.StartupPath;
                sdlg.Filter = fileType;
                sdlg.FileName = currentFilePath;
                var ret = sdlg.ShowDialog(this);
                if (ret == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                this.currentFilePath = sdlg.FileName;
                var json = System.IO.File.ReadAllText(this.currentFilePath);
                this.file = Newtonsoft.Json.JsonConvert.DeserializeObject<AMDMClientConfiggerProjectFile>(json);
            }
            catch (Exception err)
            {
                MessageBox.Show("保存文件失败\r\n" + err.Message);
            }

        }
        void file2Show(AMDMClientConfiggerProjectFile file)
        {
            this.stocksDGV.Rows.Clear();
            this.machineSNLabel.Text = file.Machine.SerialNumber;
            this.machineProductionTimeTextbox.Text = file.Machine.ProductionTime == null? "": file.Machine.ProductionTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            this.publicKeyTextbox.Text = file.Machine.SoftwarePublickKey;
            if (this.file.Machine.Stocks!= null)
            {
                foreach (var s in this.file.Machine.Stocks)
                {
                    addStock2Show(s);
                }
            }
            else
            {
                this.file.Machine.Stocks = new List<AMDM_Domain.AMDM_Stock>();
            }
        }

        void addStock2Show(AMDM_Domain.AMDM_Stock s)
        {
            int index = this.stocksDGV.Rows.Add();
            DataGridViewRow row = this.stocksDGV.Rows[index];
            row.Cells["columnStockIndex"].Value = s.IndexOfMachine;
            row.Cells["columnStockSN"].Value = s.SerialNumber;
            row.Cells["columnStockCreateTime"].Value = s.FirstLayoutTime;
        }

        private void createPublickKeyBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.machineSNLabel.Text) == true)
            {
                return;
            }
            bool needBackup = false;
            if (string.IsNullOrEmpty(this.file.Machine.SoftwarePublickKey)  == false)
            {
                var mbret = MessageBox.Show(this, "当前已经设定了软件的公钥信息\r\n如软件已部署,重新生成公钥信息将导致此前公钥的不可用\r\n仍要覆盖之前的公钥信息吗?", "危险操作", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                if (mbret == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
                else
                {
                    needBackup = true;
                }
            }
            string keyDir = string.Format("{0}\\{1}",System.IO.Path.GetDirectoryName(this.currentFilePath), "公钥私钥文件");
            var f = new Keygen.Main(this.machineSNLabel.Text, keyDir);
            var ret = f.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            this.publicKeyTextbox.Text = f.PublicKey;
            this.file.Machine.SoftwarePublickKey = f.PublicKey;
            if (needBackup)
            {
                this.saveBackupFile();
            }
            else
            {
                saveFile(false,false);
            }
        }

        private void chooseAMDMAPPInstallPathBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            var ret = fd.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string path = fd.SelectedPath;
            if (path.ToString().EndsWith("amdm") || path.ToString().EndsWith("amdm\\"))
            {
                
            }
            else
            {
                path = string.Format("{0}\\AMDM", path);
            }
            this.amdmAPPInstallPathTextbox.Text = path;
            this.saveFile(false,false);
        }

        private void installAMDMAPPBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (System.IO.Directory.Exists(this.amdmAPPInstallPathTextbox.Text) == false)
                {
                    System.IO.Directory.CreateDirectory(this.amdmAPPInstallPathTextbox.Text);
                }
                //把付药机程序拷贝到目标目录
                if (System.IO.File.Exists(this.amdmInstallPackagePath) == false)
                {
                    MessageBox.Show("未找到安装包:\r\n" + this.amdmInstallPackagePath);
                    return;
                }
                var evs = new FastZipEvents();
                evs.CompletedFile = (s,ev)=>
                    {
                    };
                evs.DirectoryFailure = (s, ev) =>
                    { };
                evs.FileFailure = (s, ev) =>
                    {

                    };
                evs.Progress = (s, ev) =>
                    {

                    };
                FastZip fz = new FastZip(evs);
                fz.ExtractZip(this.amdmInstallPackagePath, this.amdmAPPInstallPathTextbox.Text+"\\", null);
            }
            catch (Exception installErr)
            {
                MessageBox.Show("安装付药机程序失败:"+installErr.Message);
            }
        }

        private void createStockByGuideBtn_Click(object sender, EventArgs e)
        {
            if (file.Machine == null)
            {
                MessageBox.Show(this,"请先创建药机(从创建序列号开始)");
                return;
            }
            App.Init(null,null,null,false,false,false,false);
            var machine = file.Machine;
            GridAutoLayouter.AMDMHardwareInfoManager hm= new GridAutoLayouter.AMDMHardwareInfoManager(App.sqlClient);
            var newStock = hm.CreateStock(machine.Id,
                App.Setting.HardwareSetting.Stock.CenterDistanceBetweenTwoGrabbers,
                this.stocksDGV.Rows.Count,
                (int)App.Setting.HardwareSetting.Stock.MaxFloorsHeightMM, 
                (int)App.Setting.HardwareSetting.Stock.MaxFloorWidthMM,
                Utils.GetRandomSN(6,6), 
                App.Setting.HardwareSetting.Stock.XOffsetFromStartPointMM, 
                App.Setting.HardwareSetting.Stock.YOffsetFromStartPointMM);
            newStock.Floors = new Dictionary<int, AMDM_Domain.AMDM_Floor>();

            file.Machine.Stocks.Add(newStock);
            addStock2Show(newStock);
            GridAutoLayouter.GridAutoLayouter lform = new GridAutoLayouter.GridAutoLayouter(newStock, this.getStockFileName(newStock, StockFileTypeEnum.galp));
            lform.ShowDialog();
            this.saveFile(false,false);
        }

        private void 自动付药机客户端部署工具_Load(object sender, EventArgs e)
        {

        }

        private void editStockByGuideBtn_Click(object sender, EventArgs e)
        {
            var destStock = getSelectedStock();
            if (destStock == null)
            {
                return;
            }
            GridAutoLayouter.GridAutoLayouter lform = new GridAutoLayouter.GridAutoLayouter(destStock, this.getStockFileName(destStock, StockFileTypeEnum.galp));
            lform.ShowDialog();
            this.saveFile(false,false);
        }

        private void editStockByMunalBtn_Click(object sender, EventArgs e)
        {

        }
        AMDM_Stock getSelectedStock()
        {
            if (this.stocksDGV.SelectedRows.Count != 1)
            {
                return null;
            }
            string sn = string.Format("{0}",this.stocksDGV.SelectedRows[0].Cells["columnStockSN"].Value);
            if (file.Machine == null || file.Machine.Stocks == null || file.Machine.Stocks.Count<1)
            {
                return null;
            }
            foreach (var s in file.Machine.Stocks)
            {
                if (s.SerialNumber == sn)
                {
                    return s;
                }
            }
            return null;
        }
        public enum StockFileTypeEnum {
            /// <summary>
            /// 自动化的,向导式的布局文件 Grid Auto Layout Project
            /// </summary>
            galp,
            /// <summary>
            /// 手动布局文件 Grid Manual Layout Project
            /// </summary>
            gmlp,
        }
        string getStockFileName(AMDM_Domain.AMDM_Stock stock, StockFileTypeEnum fileType)
        {
            if (fileType == StockFileTypeEnum.galp)
            {
                return string.Format("{0}\\{1}-({2}).galp", Application.StartupPath, stock.IndexOfMachine, stock.SerialNumber);
            }
            else
            {
                return null;
            }
        }

        void mountStock(AMDM_Stock destStock, GridAutoLayoutSLN file)
        {
           
        }
        private void mountStockBtn_Click(object sender, EventArgs e)
        {
            var destStock = getSelectedStock();
            if (destStock== null)
            {
                return;
            }
            var fileName = getStockFileName(destStock, StockFileTypeEnum.galp);
            GridAutoLayoutSLN file = Newtonsoft.Json.JsonConvert.DeserializeObject<GridAutoLayoutSLN>(System.IO.File.ReadAllText(fileName));
            mountStock(destStock, file);
        }
    }
}
