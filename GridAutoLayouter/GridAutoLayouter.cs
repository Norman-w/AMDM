using AMDM;
using AMDM_Domain;
using MyCode.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GridAutoLayouter
{
    /// <summary>
    /// 层自动布局器
    /// </summary>
    public partial class GridAutoLayouter : Form
    {
        #region Modules
        #region 构造比较器
        public class WaitJoinMedicinesComparerDESC : IComparer<WaitJoinMedicine>
        {
            public int Compare(WaitJoinMedicine a, WaitJoinMedicine b)
            {
                if (a.TooHigh && b.TooHigh == false)
                {
                    return -1;
                }
                else if(!a.TooHigh && b.TooHigh)
                {
                    return 1;
                }
                if (a.RightGridWidth > b.RightGridWidth)
                {
                    return -1;
                }
                else if (a.RightGridWidth < b.RightGridWidth)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public class WaitJoinMedicinesComparerByNameDESC : IComparer<WaitJoinMedicine>
        {
            public int Compare(WaitJoinMedicine a, WaitJoinMedicine b)
            {
                string an = string.Format("{0}",a.Name);
                string bn = string.Format("{0}",b.Name);
                if (an == bn)
                {
                    return 0;
                }
                return an.CompareTo(bn);
            }
        }
        public class WaitJoinMedicinesComparerASC : IComparer<WaitJoinMedicine>
        {
            public int Compare(WaitJoinMedicine a, WaitJoinMedicine b)
            {
                if (a.RightGridWidth > b.RightGridWidth)
                {
                    return 1;
                }
                else if (a.RightGridWidth < b.RightGridWidth)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 根据药品的高度信息进行降序排列
        /// </summary>
        public class WaitJoinMedicinesComparerByHeightDESC : IComparer<WaitJoinMedicine>
        {
            public int Compare(WaitJoinMedicine a, WaitJoinMedicine b)
            {
                if (a.BoxHeightMM > b.BoxHeightMM)
                {
                    return -1;
                }
                else if (a.BoxHeightMM < b.BoxHeightMM)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public class FloorSorterDESC : IComparer<Floor>
        {
            public int Compare(Floor a, Floor b)
            {
                if (a.RemaindWidth > b.RemaindWidth)
                {
                    return 1;
                }
                else if (a.RemaindWidth < b.RemaindWidth)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public class FloorSorterByFirstGridDESC : IComparer<Floor>
        {
            public int Compare(Floor a, Floor b)
            {
                if (a.Medicines[0].RightGridWidth > b.Medicines[0].RightGridWidth)
                {
                    return 1;
                }
                else if (a.Medicines[0].RightGridWidth < b.Medicines[0].RightGridWidth)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public class FloorSorterASC : IComparer<Floor>
        {
            public int Compare(Floor a, Floor b)
            {
                if (a.RemaindWidth > b.RemaindWidth)
                {
                    return -1;
                }
                else if (a.RemaindWidth < b.RemaindWidth)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion
        
        #region 全局变量
        GridAutoLayoutSLN file = null;
        AMDM_Stock destStock = null;
        string fileName = null;
        #endregion
        #endregion

        #region View
        #region 构造函数和初始化
        public GridAutoLayouter(AMDM_Stock destStock, string fileName)
        {
            InitializeComponent();
            if (destStock == null)
            {
                MessageBox.Show("必须指定目标药仓");
                this.Close();
                return;

            }
            this.destStock = destStock;
            App.Init(null, null, null, false, false, false, false);
            this.fileName = fileName;
            var name = fileName;
            if (System.IO.File.Exists(name) == false)
            {
                this.file = new GridAutoLayoutSLN();
                projectSetup(false);
                save(name);
            }
            else
            {
                //如果文件已经存在的话直接读取文件.
                string json = System.IO.File.ReadAllText(name);
                this.file = Newtonsoft.Json.JsonConvert.DeserializeObject<GridAutoLayoutSLN>(json);
                if (file != null)
                {
                    showFile2View(file);
                }
            }
        }

        private void GridAutoLayouter_Load(object sender, EventArgs e)
        {
            
        }

        #endregion

        #region 页面交互
        private void 随机添加按钮_Click(object sender, EventArgs e)
        {
            NumberInputForm nform = new NumberInputForm("请输入随机添加的药品数量", "随机添加生成的药品信息", 120);
            if (nform.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            file.Medicines.Clear();
            file.Floors.Clear();
            this.药品信息列表.Rows.Clear();
            int count = (int)nform.InputValue;
            for (int i = 0; i < count; i++)
            {
                //var medicine = getMedicine();
                var medicine = new WaitJoinMedicine()
                {
                    Name = "M" +  i.ToString()+"M" ,
                    Id = i,
                };
                //录入药盒尺寸信息
                simulateSetMedicineSize(ref medicine, 50, 115);
                getRightBoxSize(medicine,this.file.floorMedicineBoxMaxHeightM);
                addMedicine2DGV(medicine);
                file.Medicines.Add(medicine);
            }
            this.panel1.Invalidate();
        }
        List<WaitJoinMedicine> sortByName(List<WaitJoinMedicine> medicines)
        {
            Dictionary<string, List<WaitJoinMedicine>> nameAndListDic = new Dictionary<string, List<WaitJoinMedicine>>();
            foreach (var m in medicines)
            {
                if (m.Name == null)
                {
                    m.Name = "";
                }
                if (nameAndListDic.ContainsKey(m.Name) == false)
                {
                    nameAndListDic.Add(m.Name, new List<WaitJoinMedicine>());
                }
                nameAndListDic[m.Name].Add(m);
            }
            List<WaitJoinMedicine> ret = new List<WaitJoinMedicine>();
            foreach (var item in nameAndListDic)
            {
                ret.AddRange(new List<WaitJoinMedicine>(item.Value));
            }
            return ret;
        }
        void insertSameNameMedicine2Front(WaitJoinMedicine current, List<WaitJoinMedicine> medicines)
        {
            int index = medicines.IndexOf(current);
            if (index <0)
            {
                return;
            }
            List<WaitJoinMedicine> sameNameMedicine = new List<WaitJoinMedicine>();
            foreach (var m in medicines)
            {
                if (m.Name == current.Name && m != current)
                {
                    sameNameMedicine.Add(m);
                }
            }
            foreach (var m in sameNameMedicine)
            {
                medicines.Remove(m);
            }
            medicines.InsertRange(0, sameNameMedicine);
        }
        private void 粗略计算按钮_Click(object sender, EventArgs e)
        {
            if (file.Medicines.Count < 1)
            {
                return;
            }
            file.Floors.Clear();
            List<WaitJoinMedicine> copy = new List<WaitJoinMedicine>();
            copy.AddRange(file.Medicines);
            #region 对所有要加入的元素进行排序
            WaitJoinMedicinesComparerDESC cp = new WaitJoinMedicinesComparerDESC();
            file.Medicines.Sort(cp);
            file.Medicines = sortByName(file.Medicines);
            foreach (var item in file.Medicines)
            {
                Console.WriteLine(item.RightGridWidth);
            }
            #endregion
            #region 按照从大到小,依次把他们装入列表中
            Floor currentFloor = new Floor((int)(this.destStock.MaxFloorWidthMM -  App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM), 0);
            file.Floors.Add(currentFloor);
            WaitJoinMedicine lastJoinedMedicine = null;
            while (file.Medicines.Count > 0)
            {
                var current = file.Medicines[0];
                #region 如果当前的药品跟上次插入的一样的话 不进行其他的检测 直接挨着插入到当前层,如果层不行,新一层
                if (lastJoinedMedicine != null && current.Name == lastJoinedMedicine.Name)
                {
                    if (joinMedicine(currentFloor, current) == false)
                    {
                        currentFloor = new Floor((int)(this.destStock.MaxFloorWidthMM - App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM), 0);
                        file.Floors.Add(currentFloor);
                    }
                    else
                    {
                        lastJoinedMedicine = current;
                        file.Medicines.Remove(current);
                    }
                    continue;
                }
                #endregion
                bool joinRet = joinMedicine(currentFloor, current);
                //如果可以加入 继续
                if (joinRet)
                {
                    lastJoinedMedicine = current;
                    file.Medicines.Remove(current);
                    continue;
                }
                else
                {
                    #region 如果没有了 退出
                    if (file.Medicines.Count < 1)
                    {
                        return;
                    }
                    #endregion

                    bool join2AllLineRetSuccess = joinMedicine(current);
                    if (join2AllLineRetSuccess == false)
                    {
                        currentFloor = new Floor((int)(this.destStock.MaxFloorWidthMM-  App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM),0);
                        file.Floors.Add(currentFloor);
                        if (joinMedicine(currentFloor, current))
                        {
                            lastJoinedMedicine = current;
                            file.Medicines.Remove(current);
                        }
                    }
                    else
                    {
                        file.Medicines.Remove(current);
                        lastJoinedMedicine = current;
                        continue;
                    }
                    //}
                    //如果没有刚好这个尺寸的,看比这个尺寸大一点的跟现在剩余的空间差多少
                }
            }
            #endregion
            file.Medicines.Clear();
            file.Medicines.AddRange(copy);
            //设置药槽位置的编号
            setWaitJoinMedicineGridNumber(file.Floors);
            addFloors2Dgv(file.Floors);
            checkFloorCount();
            file.LayoutMode = LayoutModeEnum.Method1;
            panel1.Invalidate();
        }
        bool checkFloorCount()
        {
            int realCount = 0;
            int tooHeightFloorCount = 0;
            foreach (var f in file.Floors)
            {
                if (f.IsTooHighFloor)
                {
                    tooHeightFloorCount += 1;
                }
            }
            realCount = file.Floors.Count;
            if (tooHeightFloorCount == 1)
            {

            }
            else if(tooHeightFloorCount>1)
            {
                realCount += tooHeightFloorCount - 1;
            }
            if (realCount>file.maxFloorCount)
            {
                MessageBox.Show(string.Format("已超出{0}行", realCount - file.maxFloorCount));
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取需要拆掉层隔板的层数
        /// </summary>
        /// <param name="floors"></param>
        /// <returns></returns>
        int getNeedCombainFloorsCount(List<Floor> floors)
        {
            int c = 0;
            int tooHeightFloorCount = 0;
            foreach (var f in floors)
            {
                if (f.IsTooHighFloor)
                {
                    tooHeightFloorCount += 1;
                }
            }
            c = 0;
            if (tooHeightFloorCount == 1)
            {

            }
            else if (tooHeightFloorCount > 1)
            {
                c += tooHeightFloorCount - 1;
            }
            return c;
        }
        private void 按剩余空间排序按钮_Click(object sender, EventArgs e)
        {
            FloorSorterDESC fs = new FloorSorterDESC();
            file.Floors.Sort(fs);
            addFloors2Dgv(file.Floors);
            checkFloorCount();
            this.panel1.Invalidate();
        }
        private void 尝试计算所有的可能性按钮_Click(object sender, EventArgs e)
        {
            if (file.Medicines.Count == 0)
            {
                return;
            }
            #region 如果已经执行了第一种方案,尝试使用最优解之前,看一下已经将要满了的行能不能加入最小的一个元素
            //if (floors!=null && floors.Count>0)
            //{
            //    List<WaitJoinMedicine> copyASC = new List<WaitJoinMedicine>();
            //    copyASC.AddRange(file.Medicines);
            //    copyASC.Sort(new WaitJoinMedicinesComparerASC());
            //    var min = copyASC[0];

            //    //使用所有小元素获取一个最短行
            //    Floor minFloor = new Floor();
            //    Floor minCountFloor = getWhiteSpaceMaxFloor(file.Medicines, file.Floors, true);
            //    Floor whiteSpaceMaxFloor = getWhiteSpaceMaxFloor(file.Medicines, file.Floors, true);
            //    for (int i = 0; i < whiteSpaceMaxFloor.Medicines.Count; i++)
            //    {
            //        joinMedicine(minFloor, copyASC[i + 1]);
            //    }
            //    int minCountFloorMedicinesCount = minCountFloor.Medicines.Count;
            //    minCountFloor.Medicines.Clear();
            //    minCountFloor.CurrentUsedWidth = 0;
            //    for (int i = 0; i < minCountFloorMedicinesCount; i++)
            //    {
            //        //把数量最少的已经够满了的行 换成小元素
            //        joinMedicine(minCountFloor, copyASC[i + 1]);
            //    }
            //    //已经得到了最小行,但是不能加入一个最小的元素 那这个行就是已经可以了.
            //    if (joinMedicine(minFloor, min) == false)
            //    {
            //        //
            //        MessageBox.Show("剩余空间最大的即将满的行里面不能加入最小的元素");
            //        return;
            //    }
            //    if (joinMedicine(minCountFloor, min) == false)
            //    {
            //        MessageBox.Show("元素最少的即将满的行里面不能加入最小的元素");
            //        return;
            //    }
            //}

            #endregion

            ///是否包含有超高的药品,如果是的话,需要把最上面一层安排成超高的药品,不管空间剩余如何,然后其他的在后面排起来
            List<WaitJoinMedicine> tooHighMedicines = new List<WaitJoinMedicine>();
            #region 检查超高药品的总宽度是否超出层宽度 
            //使用超高的药品进行层的制作,制作以后看需要多少层.然后再说.如果就一层,那就搞这一层,如果要多个层,那就需要每一个超高层由两个普通层变化而来
            List<WaitJoinMedicine> mcopy2 = new List<WaitJoinMedicine>();
            mcopy2.AddRange(file.Medicines);
            mcopy2.Sort(new WaitJoinMedicinesComparerByHeightDESC());
            float tooHighMedicinesTotalWidth = 0;
            foreach (var m in mcopy2)
            {
                if (m.BoxHeightMM> file.floorMedicineBoxMaxHeightM)
                {
                    tooHighMedicinesTotalWidth += m.BoxHeightMM;
                    tooHighMedicines.Add(m);
                }
            }

            //排序超高的药品
            tooHighMedicines.Sort(new WaitJoinMedicinesComparerByHeightDESC());
            List<Floor> rightTooHighFloors = MakeFloors(ref tooHighMedicines, null);
            
            if (rightTooHighFloors.Count>1)
            {
                MessageBox.Show(this,
                    string.Format("超高 (限高{0}mm) 药品数量为{1}\r\n总宽度{2}mm,除最上层层板外,还需要{3}层超高层板\r\n如只使用最上层层板,请移除部分药品后再尝试",
                    file.floorMedicineBoxMaxHeightM,
                    tooHighMedicines.Count,
                    tooHighMedicinesTotalWidth, 

                    rightTooHighFloors.Count-1
                    ),
                    "超高药品过多");
            }
            #endregion
            foreach (var f in rightTooHighFloors)
            {
                f.IsTooHighFloor = true;
                foreach (var m in f.Medicines)
                {
                    //原始要排序的药品表中去掉超高的药品
                    mcopy2.Remove(m);
                }
            }

            //把没有填满的超高行用普通的药品填满
            rightTooHighFloors = FillUnFullTooHighFloors(ref mcopy2, rightTooHighFloors);


            #region 正式执行方案2
            file.Floors.Clear();
            List<Floor> rightFloors = new List<Floor>();

            file.Medicines.Sort(new WaitJoinMedicinesComparerDESC());

            rightFloors = MakeFloors(ref mcopy2,null);
            #region 由于需要超高的和不超高的分两次自动排布 所以把之前的代码改写成一个makefloors函数
            //bool nowIsFullWidthFloor = false;
            //List<Floor> floorsBuffer = new List<Floor>();
            //while (true)
            //{
            //    //var canUse = getCurrentCanUseMedicines(file.Medicines);
            //    var canUse = file.Medicines;
            //    foreach (var item in canUse)
            //    {
            //        #region 如果没有层信息 就是第一层 直接新建
            //        Floor f = new Floor((int)App.Setting.HardwareSetting.Floor.FloorWidthMM, 0);
            //        joinMedicine(f, item);
            //        file.Floors.Add(f);
            //        #endregion
            //        ConnectAndAutoConnectSub(ref nowIsFullWidthFloor ,ref floorsBuffer, f, item, file.Medicines, canUse);
            //        if (nowIsFullWidthFloor)
            //        {
            //            break;
            //        }
            //    }
            //    //减掉最长的那一层
            //    if (file.Floors.Count == 0)
            //    {
            //        break;
            //    }
            //    if (nowIsFullWidthFloor == false)
            //    {

            //    }
            //    file.Floors.Sort(new FloorSorterDESC());
            //    var last = file.Floors[0];
            //    rightFloors.Add(last);
            //    file.Floors.Clear();
            //    nowIsFullWidthFloor = false;

            //    foreach (var m in last.Medicines)
            //    {
            //        file.Medicines.Remove(m);
            //    }
            //}
            #endregion
            #endregion



            int floorIndex = 0;
            #region 把floor进行排序
            FloorSorterDESC fs = new FloorSorterDESC();
            rightFloors.Sort(fs);

            
            #endregion

            #region 如果按照首层大小排序层
            if (按首格大小排序层选择框.Checked)
            {
                rightFloors.Sort(new FloorSorterByFirstGridDESC());
            }
            #endregion

            file.Floors = new List<Floor>();
            file.Floors.AddRange(rightTooHighFloors);
            file.Floors.AddRange(rightFloors);

            #region 排序层里面的格子
            foreach (var floor in file.Floors)
            {
                //重新排序格子
                if (按从小到大顺序排列格子选择框.Checked)
                {
                    if (isRemixFloor(floor))
                    {
                        if (包含超高和正常的混合层选择框.Checked)
                        {
                            floor.Medicines.Sort(new WaitJoinMedicinesComparerDESC());
                        }
                    }
                    else
                    {
                        floor.Medicines.Sort(new WaitJoinMedicinesComparerDESC());
                    }
                }
                floorIndex++;
                Console.ForegroundColor = floor.IsTooHighFloor?ConsoleColor.Red: ConsoleColor.DarkGreen;
                Console.WriteLine("{0}: 总已使用:{1}   元素个数:{2}  元素集合:", floorIndex, floor.CurrentUsedWidth, floor.Medicines.Count);
                Console.ResetColor();
                foreach (var g in floor.Medicines)
                {
                    Console.Write(g.Name);
                    Console.Write("(");
                    Console.Write(g.RightGridWidth);
                    Console.Write("mm)");
                    Console.Write(", ");
                }
                Console.WriteLine("\r\n");
            }
            #endregion

            setWaitJoinMedicineGridNumber(file.Floors);
            addFloors2Dgv(file.Floors);
            checkFloorCount();
            file.LayoutMode = LayoutModeEnum.Method2;
            panel1.Invalidate();
        }
        #endregion

        /// <summary>
        /// 该行是否为混合行,也就是层中是否有超高和非超高的再一起
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        bool isRemixFloor(Floor floor)
        {
            if (floor.IsTooHighFloor == false)
            {
                return false;
            }
            foreach (var m in floor.Medicines)
            {
                //如果超高行里面有非超高的 就是混合
                if (m.TooHigh == false)
                {
                    return true;
                }
            }
            return false;
        }

        #region 渲染函数
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Brushes.Black), new Rectangle(0, 0, this.panel1.ClientRectangle.Width - 1, this.panel1.ClientRectangle.Height - 1));
            e.Graphics.FillRectangle(Brushes.LightGray, new Rectangle(0, this.panel1.ClientRectangle.Bottom - 50, this.panel1.ClientRectangle.Width, 50));
            if (file == null)
            {
                return;
            }
            if (file.Floors.Count <1)
            {
                return;
            }
            float perFloorHeight = (this.panel1.ClientRectangle.Height -50)  / file.maxFloorCount;
            int realWidthFull = this.panel1.ClientRectangle.Width - 10;
            int realStartFullX = 5;
            int floorIndex = 0;

            int allIndex = 1;
            float startY = perFloorHeight;
            foreach (var item in file.Floors)
            {
                if (floorIndex >0 && item.IsTooHighFloor)
                {
                    //如果这是第二个或者是更多的超高行,绘制的时候直接跨度为2行
                    startY += perFloorHeight; 
                }
                int currentX = 0 + realStartFullX;
                foreach (var m in item.Medicines)
                {
                    float gridPrecent = m.RightGridWidth / item.MaxWidth;
                    int widthGridPixel = (int)Math.Round(gridPrecent * realWidthFull);
                    Pen heightPen = m.TooHigh ? new Pen(Brushes.Red) : new Pen(Brushes.Blue);
                    //绘制药盒高度
                    e.Graphics.DrawLine(heightPen, new Point(currentX + widthGridPixel, (int)startY), new Point(currentX + widthGridPixel, (int)(startY - (m.BoxHeightMM / file.floorMedicineBoxMaxHeightM) * perFloorHeight)));
                    if (m.TooHigh)
                    {
                        e.Graphics.FillRectangle(Brushes.IndianRed, new Rectangle(currentX+1, (int)(startY - perFloorHeight) +1 , widthGridPixel -2, (int)perFloorHeight -2));
                    }
                    ///绘制文字信息
                    e.Graphics.DrawString(string.Format("[{0}]{1}({2})", allIndex, m.Name, m.RightGridWidth - file.gridWallWidthMM), new Font("Gulim", 8), Brushes.Green, new PointF(currentX, startY - perFloorHeight + 3));
                    currentX += widthGridPixel;
                    allIndex++;
                }

                float percent = item.CurrentUsedWidth / item.MaxWidth;
                int widthPixel = (int)Math.Round(percent * realWidthFull);
                e.Graphics.DrawLine(new Pen( item.IsTooHighFloor? Brushes.Red : Brushes.Blue), new Point(0 + realStartFullX, (int)startY), new Point(widthPixel, (int)startY));
                floorIndex++;
                startY += perFloorHeight;
            }

        }
        #endregion
        #endregion

        #region Control
        #region 数据联通方法
        /// <summary>
        /// 通过条码获取药品
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        WaitJoinMedicine getMedicine(string barcode)
        {
            var medicine = App.medicineManager.GetMedicineByBarcode(barcode);
            var medicineFull = Newtonsoft.Json.JsonConvert.DeserializeObject<WaitJoinMedicine>(Newtonsoft.Json.JsonConvert.SerializeObject(medicine));
            return medicineFull;
        }

        /// <summary>
        /// 设置药品的尺寸信息.如果是使用模拟模式,不更新数据库
        /// </summary>
        /// <param name="medicine"></param>
        /// <param name="boxLongMM"></param>
        /// <param name="boxWidthMM"></param>
        /// <param name="boxHeightMM"></param>
        /// <param name="simulation"></param>
        /// <returns></returns>
        bool setMedicineSize(ref WaitJoinMedicine medicine, float boxLongMM, float boxWidthMM, float boxHeightMM, bool simulation = true)
        {
            AMDM_Medicine m = Newtonsoft.Json.JsonConvert.DeserializeObject<AMDM_Medicine>(Newtonsoft.Json.JsonConvert.SerializeObject(medicine));
            bool ret = false;


            if (simulation)
            {
                ret = true;
                m.BoxLongMM = boxLongMM;
                m.BoxWidthMM = boxWidthMM;
                m.BoxHeightMM = boxHeightMM;
            }
            else
            {
                ret = App.medicineManager.ResetMedicineBoxSize(ref m, boxLongMM, boxWidthMM, boxHeightMM);
            }
            medicine.BoxWidthMM = m.BoxWidthMM;
            medicine.BoxHeightMM = m.BoxHeightMM;
            medicine.BoxLongMM = m.BoxLongMM;
            return ret;
        }

        #endregion

        #region 数据层和逻辑层互通
        void addMedicine2DGV(AMDM_Medicine medicine)
        {
            DataGridViewRow row = null;
            int index = this.药品信息列表.Rows.Add();
            row = this.药品信息列表.Rows[index];
            row.Cells["columnMedicineId"].Value = medicine.Id;
            row.Cells["columnIndex"].Value = index + 1;
            row.Cells["columnBarcode"].Value = medicine.Barcode;
            row.Cells["columnName"].Value = medicine.Name;
            row.Cells["columnLong"].Value = medicine.BoxLongMM;
            row.Cells["columnWidth"].Value = medicine.BoxWidthMM;
            row.Cells["columnHeight"].Value = medicine.BoxHeightMM;
        }
        void addFloors2Dgv(List<Floor> floors)
        {
            this.floorsDGV.Rows.Clear();
            int floorIndex = floors.Count;
            foreach (var f in floors)
            {
                int index = this.floorsDGV.Rows.Add();
                DataGridViewRow row = null;
                row = this.floorsDGV.Rows[index];
                row.Cells["columnFloorIndex"].Value = floorIndex--;
                row.Cells["columnFloorWidth"].Value = f.MaxWidth + App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM;
                row.Cells["columnMedicineCount"].Value = f.Medicines.Count;
                row.Cells["columnFloorRemaining"].Value = f.RemaindWidth;
            }
        }
        #endregion
       
        #region 获取格子的最合适的宽度
        float getRightBoxSize(WaitJoinMedicine medicine, float floorMedicineBoxMaxHeightMM)
        {
            float ret = 0;
            //固定给药槽增加一定的最小间隙
            ret = medicine.BoxWidthMM + App.Setting.HardwareSetting.Grid.MinGridPaddingWidthMM;
            //算出这个间隙一共移动了多少个小格子(当前是每5mm一个格子)
            double stepCount = ret / App.Setting.HardwareSetting.Grid.PerGridMoveStepWidthMM;
            //这个格子因为只能取5mm一个,所以要向上取整.比如药盒100毫米,加上间隙以后102毫米,然后102毫米不能放在100毫米的格子上,只能向上取整用21个格子也就是105mm
            int stepCountInt = (int)Math.Ceiling(stepCount);
            //格子数量21*5就是最后要用105毫米的格子
            ret = stepCountInt * App.Setting.HardwareSetting.Grid.PerGridMoveStepWidthMM + App.Setting.HardwareSetting.Grid.GridWallWidthMM;
            medicine.RightGridWidth = ret;
            //大于最大限度的话,就属于超高
            medicine.TooHigh = medicine.BoxHeightMM > floorMedicineBoxMaxHeightMM;
            return ret;
        }
        #endregion
        
        #region 往层中加入药品,如果能加入进来就成功,如果加入不进来就失败了.
        bool joinMedicine(Floor floor, WaitJoinMedicine medicine)
        {
            if (floor.CurrentUsedWidth >= floor.MaxWidth)
            {
                return false;
            }
            else
            {
                var newWidth = floor.CurrentUsedWidth + medicine.RightGridWidth;
                if (newWidth > floor.MaxWidth)
                {
                    return false;
                }
                else
                {
                    floor.CurrentUsedWidth += medicine.RightGridWidth;
                    floor.Medicines.Add(medicine);
                    return true;
                }
            }
        }
        #endregion

        #region 计算每个格子的序号
        void setWaitJoinMedicineGridNumber(List<Floor> floors)
        {
            int index = 0;
            foreach (var f in floors)
            {
                foreach (var m in f.Medicines)
                {
                    m.IndexOfStock = ++index;
                }
            }
        }
        #endregion

        #region 主要逻辑递归
        public List<Floor> FillUnFullTooHighFloors(ref List<WaitJoinMedicine> allMedicines, List<Floor> unFullFloors)
        {
            List<Floor> rightFloors = new List<Floor>();
            foreach (var f in unFullFloors)
            {
                List<Floor> allFloorsBuffer = new List<Floor>();
                if (f.RemaindWidth<10)
                {
                    rightFloors.Add(f);
                    continue;
                }
                List<Floor> fs = MakeFloors(ref allMedicines, f);
                if (fs.Count == 0)
                {
                    rightFloors.Add(f);
                    continue;
                }
                allFloorsBuffer.AddRange(fs);
                allFloorsBuffer.Sort(new FloorSorterDESC());
                if (allFloorsBuffer.Count>0)
                {
                    rightFloors.Add(allFloorsBuffer[0]);
                }
            }
            foreach (Floor f in rightFloors)
            {
                foreach (var m in f.Medicines)
                {
                    allMedicines.Remove(m);
                }
            }
           
            return rightFloors;
            //MakeFloors(ref allMedicines, un)
        }

        public List<Floor> MakeFloors(ref List<WaitJoinMedicine> allMedicines, Floor unFullFloor)
        {
            bool nowIsFullWidthFloor = false;
            List<Floor> rightFloors = new List<Floor>();
            List<Floor> allFloorsBuffer = new List<Floor>();
            List<WaitJoinMedicine> mcopy = new List<WaitJoinMedicine>();
            mcopy.AddRange(allMedicines);
            while (true)
            {
                var canUse = mcopy;
                foreach (var item in canUse)
                {
                    #region 如果没有层信息 就是第一层 直接新建
                    Floor f = unFullFloor != null ? copyFloor(unFullFloor): new Floor((int)(this.destStock.MaxFloorWidthMM -  App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM), 0);
                    if(joinMedicine(f, item) == false)
                    {
                        continue;
                    }

                    allFloorsBuffer.Add(f);
                    #endregion
                    ConnectAndAutoConnectSub(ref nowIsFullWidthFloor, ref allFloorsBuffer, f, item, mcopy, canUse);
                    if (nowIsFullWidthFloor)
                    {
                        break;
                    }
                }
                allFloorsBuffer.Sort(new FloorSorterDESC());
                if (allFloorsBuffer.Count<1)
                {
                    break;
                }
                var last = allFloorsBuffer[0];
                rightFloors.Add(last);
                allFloorsBuffer.Clear();
                nowIsFullWidthFloor = false;

                foreach (var m in last.Medicines)
                {
                    mcopy.Remove(m);
                }
            }
            return rightFloors;
        }

        /// <summary>
        /// 全局的,每一次递归完成时检测或设置,如果设置到了有满连的链条,那就逐级跳出递归,如果当前拼接正好能拼满,那就设置为true,供父级回溯时检查
        /// </summary>
        //bool nowIsFullWidthFloor = false;
        /// <summary>
        /// 主要函数,递归处理拼接方式
        /// </summary>
        /// <param name="parentFloor">父级的层,也就相当于当前操作应该把链子挂载到什么样的链条上</param>
        /// <param name="parent">父级的元素,也就是相当于当前操作应该把链子挂载到哪一个上一个链子上</param>
        /// <param name="canUseList">可以使用的没往上挂的链子的集合.因为每一次挂成功了满长度的一条链子以后,都会去掉之前已经用掉的,所以就只拼接后面的就够了.大大的减少了操作次数</param>
        void ConnectAndAutoConnectSub(ref bool nowIsFullWidthFloor,ref List<Floor> allFloorsBuffer, Floor parentFloor, WaitJoinMedicine parent,List<WaitJoinMedicine> allMedicines, List<WaitJoinMedicine> canUseList)
        {
            if (nowIsFullWidthFloor)
            {
                return;
            }
            currentDepth++;
            int parentIndex = canUseList.IndexOf(parent);
            if (parentIndex + 1 >= canUseList.Count)
            {
                return;
            }
            int start = parentIndex + 1;
            int end = canUseList.Count;
            float lastSize = 0;
            for (int i = start; i < end; i++)
            {
                var current = canUseList[i];
                if (current.RightGridWidth == lastSize)
                {
                    continue;
                }
                var 当前格 = current.Name;
                var 主层信息 = getFloorInfo(parentFloor);
                var 当前层编号 = parentFloor.FloorName;
                var 当前深度 = currentDepth;
                Console.WriteLine("层数量:{4}  正在尝试使用: {0}({1}mm)   +    {2}({3}mm)", 主层信息, parentFloor.CurrentUsedWidth, 当前格, current.RightGridWidth, allFloorsBuffer.Count);
                var joinRet = joinMedicine(parentFloor, current);
                if (joinRet)
                {
                    if (allFloorsBuffer.Contains(parentFloor) == false)
                    {
                        allFloorsBuffer.Add(parentFloor);
                    }
                    if (parentFloor.MaxWidth == parentFloor.CurrentUsedWidth)
                    {
                        nowIsFullWidthFloor = true;
                        return;
                    }
                    Floor newFloor = copyFloor(parentFloor);
                    ConnectAndAutoConnectSub(ref nowIsFullWidthFloor,ref allFloorsBuffer, newFloor, current,allMedicines, canUseList);// joinMedicine(currentFloor, current);
                    if (!containsAll(parentFloor))
                    {
                        Floor copy = copyFloor(parentFloor);
                        allFloorsBuffer.Add(copy);
                        removeLast(parentFloor);
                    }
                    else
                    {
                        nowIsFullWidthFloor = true;
                        return;
                    }
                }
                else
                {
                    //continue;
                }
                lastSize = current.RightGridWidth;
            }
            

            currentDepth--;
        }

        #endregion

        #region 层操作方法
        Floor copyFloor(Floor floor)
        {
            Floor newFloor = new Floor((int)floor.MaxWidth, (int)file.maxClipDepth);
            newFloor.FloorName = "copy floor " + currentFloorIndex++.ToString();
            newFloor.MaxWidth = floor.MaxWidth;
            newFloor.IsTooHighFloor = floor.IsTooHighFloor;
            foreach (var m in floor.Medicines)
            {
                newFloor.Medicines.Add(m);
                newFloor.CurrentUsedWidth += m.RightGridWidth;
            }
            return newFloor;
        }
        void removeLast(Floor floor)
        {
            floor.CurrentUsedWidth -= floor.Medicines[floor.Medicines.Count - 1].RightGridWidth;
            floor.Medicines.RemoveAt(floor.Medicines.Count - 1);
        }
        bool containsAll(Floor floor)
        {
            if (floor.Medicines.Count != file.Medicines.Count)
            {
                return false;
            }
            else
            {
                return floor.Medicines.Except(file.Medicines).Count() == 0;
            }
        }
        /// <summary>
        /// 获取空白空间最大的层
        /// </summary>
        /// <param name="allMedicines"></param>
        /// <param name="floors"></param>
        /// <param name="ignoreNotFullFloor"></param>
        /// <returns></returns>
        Floor getWhiteSpaceMaxFloor(List<WaitJoinMedicine> allMedicines, List<Floor> floors, bool ignoreNotFullFloor = true)
        {
            List<WaitJoinMedicine> copyASC = new List<WaitJoinMedicine>();
            copyASC.AddRange(allMedicines);
            copyASC.Sort(new WaitJoinMedicinesComparerASC());
            var min = copyASC[0];


            Floor whiteSpaceMaxFloor = null;
            List<Floor> floorsASC = new List<Floor>();

            floorsASC.AddRange(file.Floors);
            floorsASC.Sort(new FloorSorterASC());
            foreach (var floor in floorsASC)
            {
                var f = copyFloor(floor);
                if (joinMedicine(f, min) == false)
                {
                    whiteSpaceMaxFloor = f;
                    break;
                }
                else
                {
                    if (ignoreNotFullFloor)
                    {
                        return f;
                    }
                    //还能加入就不是最满的
                }
            }

            return whiteSpaceMaxFloor;
        }
        /// <summary>
        /// 获取元素最少的层
        /// </summary>
        /// <param name="allMedicines"></param>
        /// <param name="floors"></param>
        /// <param name="ignoreNotFullFloor"></param>
        /// <returns></returns>
        Floor getMinCountFloor(List<WaitJoinMedicine> allMedicines, List<Floor> floors, bool ignoreNotFullFloor = true)
        {
            List<WaitJoinMedicine> copyASC = new List<WaitJoinMedicine>();
            copyASC.AddRange(allMedicines);
            copyASC.Sort(new WaitJoinMedicinesComparerASC());
            var min = copyASC[0];

            List<Floor> floorsASC = new List<Floor>();

            floorsASC.AddRange(file.Floors);
            floorsASC.Sort(new FloorSorterASC());
            foreach (var floor in floorsASC)
            {
                var f = copyFloor(floor);
                if (joinMedicine(f, min) == false)
                {
                    return f;
                }
                else
                {
                    if (ignoreNotFullFloor)
                    {
                        return f;
                    }
                    //还能加入就不是最满的
                }
            }

            return null;
        }
        bool joinMedicine(WaitJoinMedicine medicine)
        {
            foreach (var floor in file.Floors)
            {
                var ret = joinMedicine(floor, medicine);
                if (ret)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #endregion

        #region DebugAndTest
        #region 测试方法
        /// <summary>
        /// 测试,用于设置药品的尺寸,使用随机数
        /// </summary>
        /// <param name="medicine"></param>
        /// <param name="randomWidthMin"></param>
        /// <param name="randomWidthMax"></param>
        /// <returns></returns>
        bool simulateSetMedicineSize(ref WaitJoinMedicine medicine, int randomWidthMin, int randomWidthMax)
        {
            float randomLong = new Random(Guid.NewGuid().GetHashCode()).Next(150, 220);
            //float randomWidth = new Random(Guid.NewGuid().GetHashCode()).Next(120, 125);
            float randomWidth = new Random(Guid.NewGuid().GetHashCode()).Next(randomWidthMin, randomWidthMax);
            float randomHeight = new Random(Guid.NewGuid().GetHashCode()).Next(5, 50);

            return setMedicineSize(ref medicine, randomLong, randomWidth, randomHeight);
        }
        /// <summary>
        /// 测试,模拟获取随机药品,从数据库中
        /// </summary>
        /// <returns></returns>
        WaitJoinMedicine simulateGetRandomMedicine()
        {
            var medicine = App.medicineManager.GetRandomMedicine();
            WaitJoinMedicine m = Newtonsoft.Json.JsonConvert.DeserializeObject<WaitJoinMedicine>(Newtonsoft.Json.JsonConvert.SerializeObject(medicine));
            return m;
        }
        #endregion
        #region 调试层
        /// <summary>
        /// 当前的递归深度
        /// </summary>
        int currentDepth = 0;
        /// <summary>
        /// 当前层的索引,用于创建层的名称
        /// </summary>
        int currentFloorIndex = 0;

        string getFloorInfo(Floor floor)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var m in floor.Medicines)
            {
                sb.Append(m.Name);
                sb.Append(",");
            }
            return sb.ToString();
        }

        void logFLoor(Floor floor)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("LogFloor层信息,总长度:{0}", floor.CurrentUsedWidth);
            Console.ResetColor();
            Console.Write("层元素集合:");
            for (int i = 0; i < floor.Medicines.Count; i++)
            {
                Console.Write("{0}({1}mm), ", floor.Medicines[i].Name, floor.Medicines[i].RightGridWidth);
            }
            Console.WriteLine("\r\n");
        }
        #endregion

        
        #endregion

        private void 批量设置宽度按钮_Click(object sender, EventArgs e)
        {
            scanAndInputWidth(false);
        }
        /// <summary>
        /// 扫描并且输入宽度(连续)
        /// </summary>
        /// <param name="simulating">是否测试,如果是测试,自动获取药品库的药品</param>
        void scanAndInputWidth(bool simulating)
        {
            

            while (true)
            {
                string scanedBarcode = null;
                //扫描条码或者等待条码
                if (simulating)
                {
                   scanedBarcode = simulateWaitBarcode(); 
                }
                else
                {
                    scanedBarcode = waitBarcode();
                    if (scanedBarcode == null)
                    {
                        return;
                    }
                }
                //根据条码获取商品  


                var medicine = getMedicine(scanedBarcode);

                bool canContinueSetWidth = true;
                if (medicine == null)
                {
                    MessageBox.Show(string.Format("根据条码 {0} 未能获取到药品信息", scanedBarcode));
                    return;
                }
                #region 如果每次扫描的时候都检测一下是否重新设置尺寸 执行这其中的代码

                else if (medicine.BoxWidthMM != 0 && medicine.BoxHeightMM != 0 && medicine.BoxLongMM != 0)
                {
                    canContinueSetWidth = false;
                    //var ret = MessageBox.Show(this, string.Format("该药品已设定宽度为{0}mm\r\n确认重新设定尺寸吗?", medicine.BoxWidthMM), "是否重设宽度", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    //if (ret == System.Windows.Forms.DialogResult.No)
                    //{
                    //    canContinueSetWidth = false;
                    //}
                    //else
                    //{

                    //}
                }

                #endregion
                #region 设置药盒尺寸信息
                if (canContinueSetWidth)
                {
                    this.inputMedicineSize(medicine,false, simulating);
                }
                else
                {
                    this.getRightBoxSize(medicine, file.floorMedicineBoxMaxHeightM);
                }
                #endregion
                

                bool canContanueJoin = true;
                var hasThisIdMedicines = this.getMedicinesFromWaitJoinList(file.Medicines, medicine.Id);
                if (hasThisIdMedicines.Count>0)
                {
                    var ret = MessageBox.Show(this, string.Format("{0}\r\n已经有{1}个药槽即将部署该药品\r\n\r\n仍要为此药品安排新的药槽吗?", medicine.Name, hasThisIdMedicines.Count),
                        "该药品已扫描", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    if (ret == System.Windows.Forms.DialogResult.No)
                    {
                        canContanueJoin = false;
                    }
                }
                if (canContanueJoin)
                {
                    file.Medicines.Add(medicine);
                    this.addMedicine2DGV(medicine);
                }
            }
        }
        void inputMedicineSize(WaitJoinMedicine medicine,bool ignoreHeightChecking, bool simulating)
        {
            ///药槽内的最大可用空间
            float canUseMax = App.Setting.HardwareSetting.Grid.PerGridMaxWidthMM - App.Setting.HardwareSetting.Grid.GridWallWidthMM;
            ///药槽内的最小可用空间
            float canUseMin = App.Setting.HardwareSetting.Grid.PerGridMinWidthMM - App.Setting.HardwareSetting.Grid.GridWallWidthMM;

            float canUseLongMax = App.Setting.HardwareSetting.Stock.MaxPerMedicineDepthMM;
            float canUseLongMin = App.Setting.HardwareSetting.Stock.MinPerMedicineDepthMM;

            float canUseHeightMax = file.floorMedicineBoxMaxHeightM;
            float canUseHeightMin = App.Setting.HardwareSetting.Stock.MinPerMedicineHeightMM;

            int longMM = 0;
            #region 输入长度信息 也就是进深
            //始终检测药盒尺寸信息
            while (true)
            {
                BoxWidthInputForm bform = new BoxWidthInputForm(string.Format("请输入 {0} 的药盒长度(进深) 毫米", medicine.Name), file.Medicines.Count);
                var boxSizeInputRet = bform.ShowDialog();
                if (boxSizeInputRet == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                if (isDigitString(bform.InputWidth) == false)
                {
                    MessageBox.Show("无效的数字输入,请输入整数毫米");
                    continue;
                }
                longMM = Int32.Parse(bform.InputWidth);

                if (longMM > canUseLongMax || longMM < canUseLongMin)
                {
                    MessageBox.Show(string.Format("尺寸无效,设备可使用的药品 长度(进深)\r\n需要介于{0}~{1}毫米之间", canUseLongMin, canUseLongMax));
                    continue;
                }
                break;
            }
            #endregion
            medicine.BoxLongMM = longMM;
            this.updateMedicineLong(medicine.Id, longMM);
            int widthMM = 0;
            #region 输入宽度信息 也就是药槽占的宽度相关参数
            //始终检测药盒尺寸信息
            while (true)
            {
                BoxWidthInputForm bform = new BoxWidthInputForm(string.Format("请输入 {0} 的药盒宽度 毫米", medicine.Name), file.Medicines.Count);
                var boxSizeInputRet = bform.ShowDialog();
                if (boxSizeInputRet == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                if (isDigitString(bform.InputWidth) == false)
                {
                    MessageBox.Show("无效的数字输入,请输入整数毫米");
                    continue;
                }
                widthMM = Int32.Parse(bform.InputWidth);

                if (widthMM > canUseMax || widthMM < canUseMin)
                {
                    MessageBox.Show(string.Format("尺寸无效,设备可使用的药品宽度\r\n需要介于{0}~{1}毫米之间", canUseMin, canUseMax));
                    continue;
                }
                break;
            }
            #endregion

            medicine.BoxWidthMM = widthMM;
            this.updateMedicineWidth(medicine.Id, widthMM);

            int heightMM = 0;
            #region 输入高度信息 也就是厚度
            while (true)
            {
                BoxWidthInputForm bform = new BoxWidthInputForm(string.Format("请输入 {0} 的药盒高度(厚度) 毫米", medicine.Name), file.Medicines.Count);
                var boxSizeInputRet = bform.ShowDialog();
                if (boxSizeInputRet == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                if (isDigitString(bform.InputWidth) == false)
                {
                    MessageBox.Show("无效的数字输入,请输入整数毫米");
                    continue;
                }
                heightMM = Int32.Parse(bform.InputWidth);

                if (heightMM > canUseHeightMax || heightMM < canUseHeightMin)
                {
                    if (ignoreHeightChecking)
                    {
                        MessageBox.Show(string.Format("尺寸无效,设备可使用的药品 高度(厚度)\r\n需要介于{0}~{1}毫米之间", canUseHeightMin, canUseHeightMax));
                        continue;
                    }
                    else
                    {

                    }
                }
                break;
            }
            #endregion
            medicine.BoxHeightMM = heightMM;
            this.updateMedicineHeight(medicine.Id, heightMM);

            this.setMedicineSize(ref medicine, medicine.BoxLongMM, widthMM, medicine.BoxHeightMM, simulating);
        }
        void updateMedicineWidth(long medicineId, int newWidthMM)
        {
            var ms = this.getMedicinesFromWaitJoinList(file.Medicines, medicineId);
            foreach (var m in ms)
            {
                m.BoxWidthMM = newWidthMM;
                getRightBoxSize(m,this.file.floorMedicineBoxMaxHeightM);
            }
            updateMedicineWidth2DGV(medicineId, newWidthMM);
        }
        void updateMedicineLong(long medicineId, int newLongMM)
        {
            var ms = this.getMedicinesFromWaitJoinList(file.Medicines, medicineId);
            foreach (var m in ms)
            {
                m.BoxLongMM = newLongMM;
                //getRightBoxSize(m);
            }
            updateMedicineLong2DGV(medicineId, newLongMM);
        }
        void updateMedicineHeight(long medicineId, int newHeightMM)
        {
            var ms = this.getMedicinesFromWaitJoinList(file.Medicines, medicineId);
            foreach (var m in ms)
            {
                m.BoxHeightMM = newHeightMM;
                //getRightBoxSize(m);

                //大于最大限度的话,就属于超高
                m.TooHigh = m.BoxHeightMM > file.floorMedicineBoxMaxHeightM;
            }
            updateMedicineHeight2DGV(medicineId, newHeightMM);
        }
        void updateMedicineWidth2DGV(long medicineId, int newWidthMM)
        {
            foreach (DataGridViewRow row in this.药品信息列表.Rows)
            {
                if((long)row.Cells["columnMedicineId"].Value == medicineId)
                {
                    row.Cells["columnWidth"].Value = newWidthMM;
                }
            }
        }
        void updateMedicineLong2DGV(long medicineId, int newLongMM)
        {
            foreach (DataGridViewRow row in this.药品信息列表.Rows)
            {
                if ((long)row.Cells["columnMedicineId"].Value == medicineId)
                {
                    row.Cells["columnLong"].Value = newLongMM;
                }
            }
        }
        void updateMedicineHeight2DGV(long medicineId, int newHeightMM)
        {
            foreach (DataGridViewRow row in this.药品信息列表.Rows)
            {
                if ((long)row.Cells["columnMedicineId"].Value == medicineId)
                {
                    row.Cells["columnHeight"].Value = newHeightMM;
                }
            }
        }
        /// <summary>
        /// 在列表中获取同id的药品
        /// </summary>
        /// <param name="waitJoinMedicines"></param>
        /// <param name="medicineId"></param>
        /// <returns></returns>
        List<WaitJoinMedicine> getMedicinesFromWaitJoinList(List<WaitJoinMedicine> waitJoinMedicines, long medicineId)
        {
            List<WaitJoinMedicine> ret = new List<WaitJoinMedicine>();
            foreach (var m in waitJoinMedicines)
            {
                if (m.Id == medicineId)
                {
                    ret.Add(m);
                }
            }
            return ret;
        }
        /// <summary>
        /// 模拟等待条码,并没有真正等待,而是直接从药品库获取药品返回条码
        /// </summary>
        /// <returns></returns>
        string simulateWaitBarcode()
        {
            var medicine = simulateGetRandomMedicine();
            return medicine.Barcode;
        }
        string waitBarcode()
        {
            string scanedBarcode = null;
            BarcodeInputForm iform = new BarcodeInputForm("请输入或扫描药品条码");
            var ret = iform.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
            {
                return null;
            }
            else
            {
                scanedBarcode = iform.Barcode;
            }
            return scanedBarcode;
        }
        bool isDigitString(string str)
        {
            if (str == null || str.Length<1)
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if ("0123456789".IndexOf(c)<0)
                {
                    return false;
                }
            }
            return true;
        }

        private void 打印选中行贴纸按钮_Click(object sender, EventArgs e)
        {
            if (floorsDGV.SelectedRows.Count != 1)
            {
                return;
            }
            var floorIndex = file.Floors.Count - (int)floorsDGV.SelectedRows[0].Cells[0].Value;
            var selectedFloor = file.Floors[floorIndex];

            App.Setting.DevicesSetting.Printer58MMSetting.PrinterName = "GP-58MI";
            if (file.Floors == null || file.Floors.Count<1)
            {
                MessageBox.Show("目前没有任何行可被选择");
                return;
            }

            printFloor(selectedFloor, floorIndex, App.Setting.DevicesSetting.Printer58MMSetting.PrinterName, file.printerScaleY);
        }

        private void 打印所有行的贴纸按钮_Click(object sender, EventArgs e)
        {
            App.Setting.DevicesSetting.Printer58MMSetting.PrinterName = "GP-58MI";
            foreach (var f in file.Floors)
            {
                int index = file.Floors.IndexOf(f) +1;
                printFloor(f,index, App.Setting.DevicesSetting.Printer58MMSetting.PrinterName,file.printerScaleY);
            }
        }

        void printFloor(Floor floor, int index, string printerName, double scaleY)
        {
            //获取选中的行
            //var selectedFloor = file.Floors[file.Floors.Count - 2];
            //生成选中行的纸
            ImageCompose2 paper = new MyCode.Forms.ImageCompose2(58, floor.MaxWidth + App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM);
            Tag rootTag = new Tag();
            paper.LoadRootTag(rootTag);
            rootTag.width = 1;
            //rootTag.height = scaleY * selectedFloor.MaxWidth / 58;
            //rootTag.height = 1;
            rootTag.border.width = 1;
            rootTag.border.color = Color.Red;
            rootTag.border.dashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            rootTag.flexDirection = FlexDirectionEnum.col;
            rootTag.alignItems = AlignItemsEnum.flexStart;
            rootTag.id = "根元素";
            rootTag.justifyContent = JustifyContentEnum.center;

            //添加打印头信息
            addInfoTag(floor,index, rootTag, scaleY);
            //添加头部的外格子
            addStartOrEndLine(rootTag, false);
            //先加上一个隔板
            addWall(rootTag, scaleY);

            foreach (var m in floor.Medicines)
            {
                Tag line = new MyCode.Forms.Tag();
                line.sizeFixed = true;
                line.flexDirection = FlexDirectionEnum.row;
                line.width = 1;
                line.height = scaleY * (m.RightGridWidth - App.Setting.HardwareSetting.Grid.GridWallWidthMM) / 58;// *rootTag.height;
                //左侧
                Tag left = new MyCode.Forms.Tag();
                left.width = 0.2;
                left.height = line.height;
                left.flexDirection = FlexDirectionEnum.col;
                left.justifyContent = JustifyContentEnum.spaceBetween;

                Tag topDot = new MyCode.Forms.Tag();
                topDot.width = left.width;
                topDot.height = App.Setting.HardwareSetting.Grid.GridWallWidthMM / 58;
                topDot.border.right.width = 1;
                topDot.border.right.color = Color.Black;
                topDot.border.bottom.width = 1;
                topDot.border.bottom.color = Color.Black;

                Tag bottomDot = new MyCode.Forms.Tag();
                bottomDot.width = left.width;
                bottomDot.height = topDot.height;
                bottomDot.border.right.width = 1;
                bottomDot.border.right.color = Color.Black;
                bottomDot.border.top.width = 1;
                bottomDot.border.top.color = Color.Black;

                left.AddChirld(topDot);
                left.AddChirld(bottomDot);
                line.AddChirld(left);
                //右侧


                Text mTag = new Text();
                mTag.width = 0.8;
                mTag.value = string.Format("药品名称:\r\n{0}\r\n\r\n药槽内部宽度:{1}mm\r\n\r\n药槽编号:{2}", m.Name, m.RightGridWidth - App.Setting.HardwareSetting.Grid.GridWallWidthMM,
                    m.IndexOfStock.ToString().PadLeft(3, '0')
                    );
                mTag.height = line.height;
                //mTag.border.color = Color.Blue;
                //mTag.border.width = 2;
                //mTag.border.dashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                //mTag.border.left = null;
                mTag.font = new Font("隶书", 14);
                mTag.fontColor = Color.Black;
                mTag.format = new StringFormat() { Alignment = StringAlignment.Center };
                mTag.sizeFixed = true;
                //mTag.format.FormatFlags |= StringFormatFlags.DirectionVertical;
                mTag.format.LineAlignment = StringAlignment.Center;
                mTag.rotate = 90;

                line.AddChirld(mTag);

                rootTag.AddChirld(line);

                addWall(rootTag, scaleY);

            }
            addStartOrEndLine(rootTag, true);
            addInfoTag(floor,index, rootTag, scaleY);

            paper.PrintToPrinter(printerName, string.Format("布局辅助条 (层名称[{0}])", floor.FloorName));
        }
        void addStartOrEndLine(Tag root, bool isLast)
        {
            Tag startLine = new Tag();
            startLine.width = 1;
            startLine.height = App.Setting.HardwareSetting.Grid.GridWallWidthMM / 58;
            startLine.flexDirection = FlexDirectionEnum.row;
            startLine.justifyContent = JustifyContentEnum.flexStart;

            Tag small = new Tag();
            small.width = 0.2;
            small.border.right.color = Color.Black;
            small.border.right.width = 1;
            small.border.right.dashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            small.height = App.Setting.HardwareSetting.Grid.GridWallWidthMM / 58;
            if (isLast)
            {
                small.border.bottom.width = 5;
                small.border.bottom.color = Color.Red;
            }
            else
            {
                small.border.top.width = 5;
                small.border.top.color = Color.Red;
            }

            Text title = new MyCode.Forms.Text();
            title.sizeFixed = true;
            title.width = 0.8;
            title.value = isLast ?"↓右侧边缘↓" :"↑左侧边缘↑";
            if (isLast)
            {
                title.border.bottom.width = 5;
                title.border.bottom.color = Color.Black;
                title.border.dashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                title.format.LineAlignment = StringAlignment.Far;
            }
            else
            {
                title.border.top.width = 5;
                title.border.top.color = Color.Black;
                title.border.dashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            }
            title.height = small.height;
            title.font = new Font("隶书", 12);
            title.fontColor = Color.Black;
            title.format = new StringFormat() { Alignment = StringAlignment.Center };

            startLine.AddChirld(small);
            startLine.AddChirld(title);

            root.AddChirld(startLine);
        }
        void addWall(Tag root, double scaleY)
        {
            Text firstWall = new Text();
            firstWall.width = 1;
            firstWall.value = "档条安装位置";
            firstWall.height = scaleY * App.Setting.HardwareSetting.Grid.GridWallWidthMM / 58;
            firstWall.border.color = Color.Blue;
            firstWall.border.width = 2;
            firstWall.border.dashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            firstWall.font = new Font("@隶书", 10);
            firstWall.fontColor = Color.Black;
            firstWall.format = new StringFormat() { Alignment = StringAlignment.Center };
            firstWall.sizeFixed = true;
            //firstWall.format.FormatFlags |= StringFormatFlags.DirectionVertical;
            firstWall.format.LineAlignment = StringAlignment.Center;

            root.AddChirld(firstWall);
        }

        void addInfoTag(Floor floor,int index, Tag root, double scaleY)
        {
            Text tag = new Text();
            tag.width = 1;
            tag.value = string.Format("当前层号(由上至下){0}\r\n当前层名称:{1}\r\n药槽数量:{2}\r\n剩余空间:{3}mm\r\n打印时间:{4}\r\n打印机缩放比例:{5}\r\n层板总宽:{5}mm",
                index + 1,
                floor.FloorName, 
                floor.Medicines.Count, 
                floor.RemaindWidth,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                scaleY,
                floor.MaxWidth + -App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM
                );
            //tag.border.color = Color.Blue;
            //tag.border.width = 2;
            //tag.border.dashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            tag.font = new Font("@隶书", 12);
            tag.fontColor = Color.Black;
            tag.format = new StringFormat() { Alignment = StringAlignment.Center };
            tag.format.LineAlignment = StringAlignment.Center;

            root.AddChirld(tag);
        }

        private void 随机添加药品库药品按钮_Click(object sender, EventArgs e)
        {
            NumberInputForm nform = new NumberInputForm("请输入随机添加的药品数量", "随机添加药品库中的药品信息", 120);
            if (nform.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            file.Medicines.Clear();
            file.Floors.Clear();
            this.药品信息列表.Rows.Clear();
            int count = (int)nform.InputValue;
            for (int i = 0; i < count; i++)
            {
                var medicine = simulateGetRandomMedicine();
                //录入药盒尺寸信息
                simulateSetMedicineSize(ref medicine, 50, 115);
                getRightBoxSize(medicine,this.file.floorMedicineBoxMaxHeightM);
                addMedicine2DGV(medicine);
                file.Medicines.Add(medicine);
            }
            this.panel1.Invalidate();
        }

        private void 打印第一行贴纸按钮_Click(object sender, EventArgs e)
        {
            App.Setting.DevicesSetting.Printer58MMSetting.PrinterName = "GP-58MI";
            if (file.Floors == null || file.Floors.Count < 1)
            {
                MessageBox.Show("目前没有任何行可被选择");
                return;
            }

            printFloor(file.Floors[0],0, App.Setting.DevicesSetting.Printer58MMSetting.PrinterName, file.printerScaleY);
        }

        private void floorsDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void 开始扫码并设置药盒宽度按钮_Click(object sender, EventArgs e)
        {
            //projectSetup(file.Floors.Count > 0);
            scanAndInputWidth(false);
        }


        void projectSetup(bool resetting)
        {
            if (resetting)
            {
                string msg = string.Format("重新设定层板宽度,固定件宽度或格栅宽度后,将会重新排列层信息\r\n确认进行修改吗?");
                var mbret = MessageBox.Show(this, msg, "可能改层布局", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (mbret == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            #region 层板的总宽度
            //输入层板总宽度
            NumberInputForm nform = new NumberInputForm("请输入层板的总宽度(毫米)", "设置",
                (int)(App.Setting.HardwareSetting.Floor.FloorWidthMM));
            var ret = nform.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            this.destStock.MaxFloorWidthMM = (float)nform.InputValue;
            #endregion
            
            #region 层板总高度
            //输入层板总宽度
            nform = new NumberInputForm("请输入药仓可放置的层板总高度(毫米)", "设置",
                (int)(App.Setting.HardwareSetting.Stock.MaxFloorsHeightMM));
            ret = nform.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            this.destStock.MaxFloorsHeightMM = (int)nform.InputValue;
            #endregion

            #region 输入药槽的可用长度
            //输入药槽的可用长度
            nform = new NumberInputForm("请输入药槽的可用长度(毫米)", "设置",
                (int)(App.Setting.HardwareSetting.Floor.UpPartFloorDepthMM));
            ret = nform.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            file.maxClipDepth = (float)nform.InputValue;
            #endregion
               //通过坡度自动计算层中可容纳的药盒尺寸的最大高度
            var cos27x80 = Math.Cos(App.Setting.HardwareSetting.Floor.FloorSlopeAngle * Math.PI / 180) * App.Setting.HardwareSetting.Floor.FloorHeightMM;
            file.floorMedicineBoxMaxHeightM =
                (float)(
               cos27x80 - App.Setting.HardwareSetting.Floor.FloorPanelHeightMM - App.Setting.HardwareSetting.Stock.FloorFixingsHeightMM - App.Setting.HardwareSetting.Floor.MinGridPaddingHeighMM)
                ;
            
            file.gridWallWidthMM = App.Setting.HardwareSetting.Grid.GridWallWidthMM;
            file.maxClipDepth = App.Setting.HardwareSetting.Floor.UpPartFloorDepthMM;
            file.maxFloorCount = this.destStock.MaxFloorsHeightMM / (int)App.Setting.HardwareSetting.Floor.FloorHeightMM;
            if (resetting)
            {
                if (file.LayoutMode == LayoutModeEnum.Method1)
                {
                    粗略计算按钮_Click(null,null);
                }
                else if (file.LayoutMode == LayoutModeEnum.Method2)
                {
                    尝试计算所有的可能性按钮_Click(null,null);
                }
                MessageBox.Show("已执行重新计算");
            }
        }
        private void 设置按钮_Click(object sender, EventArgs e)
        {
            projectSetup(file.Floors.Count>0);
        }

        private void 显示模拟功能按钮_CheckedChanged(object sender, EventArgs e)
        {
            this.随机添加按钮.Enabled = this.显示模拟功能按钮.Checked;
            this.随机添加药品库药品按钮.Enabled = this.显示模拟功能按钮.Checked;
            this.自动随机添加药品库商品并批量设置宽度按钮.Enabled = this.显示模拟功能按钮.Checked;
            this.打印首行贴纸按钮.Enabled = this.显示模拟功能按钮.Checked;
        }

        private void 清空药品列表按钮_Click(object sender, EventArgs e)
        {
            var ret =MessageBox.Show(this,"清空药品列表后,所有的药品信息需要重新录入,确认吗?", "将清空药品列表", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (ret == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            file.Medicines.Clear();
            file.Floors.Clear();
            this.药品信息列表.Rows.Clear();
            this.floorsDGV.Rows.Clear();
            this.panel1.Invalidate();
        }

        private void 移除选定药品按钮_Click(object sender, EventArgs e)
        {
            if (this.药品信息列表.SelectedRows.Count >0)
            {
                var ret = MessageBox.Show(this, string.Format("确认移除选中的{0}条药品信息吗?", this.药品信息列表.SelectedRows.Count), "将从药品列表中移除药品", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (ret == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                List<DataGridViewRow> rows = new List<DataGridViewRow>();
                List<WaitJoinMedicine> ms = new List<WaitJoinMedicine>();
                foreach (DataGridViewRow row in this.药品信息列表.SelectedRows)
                {
                    rows.Add(row);
                    var mm = this.getMedicinesFromWaitJoinList(file.Medicines, (long)row.Cells["columnMedicineId"].Value);
                    if (mm.Count>0)
                    {
                        ms.AddRange(mm);
                    }
                }
                foreach (var row in rows)
                {
                    this.药品信息列表.Rows.Remove(row);
                }
                foreach (var m in ms)
                {
                    file.Medicines.Remove(m);
                }
            }
        }

        private void 保存当前工程按钮_Click(object sender, EventArgs e)
        {
            if (file.Floors.Count ==0 && file.Medicines.Count == 0)
            {
                MessageBox.Show("空的工程无需保存");
                return;
            }
            save(this.fileName);
        }
        //public string GetSaveFileName()
        //{
        //    return string.Format("{0}\\{1}-({2}).galp", Application.StartupPath, this.destStock.IndexOfMachine, this.destStock.SerialNumber);
        //}
        void save(string name)
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(file);
                if (string.IsNullOrEmpty(name))
                {
                    SaveFileDialog sdlg = new SaveFileDialog();
                    sdlg.InitialDirectory = Application.StartupPath;
                    string fileName = string.Format("药槽自动布局工程{0}.galp", DateTime.Now.ToString("yyyy-MM-dd-HHmmss"));
                    string fileType = "药槽自动布局文件(*.galp)|*.galp";
                    sdlg.Filter = fileType;
                    sdlg.FileName = fileName;
                    var ret = sdlg.ShowDialog(this);
                    if (ret == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                    string path = System.IO.Path.GetDirectoryName(sdlg.FileName);
                    if (System.IO.Directory.Exists(path) == false)
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    System.IO.File.WriteAllText(sdlg.FileName, json);
                    MessageBox.Show(this, string.Format("工程文件已保存到\r\n{0}", sdlg.FileName));
                }
                else
                {
                    System.IO.File.WriteAllText(name, json);
                    this.Text = string.Format("自动保存工程文件到:{0}", System.IO.Path.GetFileName(name));
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("保存文件失败\r\n" + err.Message);
            }
        }

        private void 从文件读取工程按钮_Click(object sender, EventArgs e)
        {
            string fileType = "药槽自动布局文件(*.galp)|*.galp";
            OpenFileDialog odlg = new OpenFileDialog();
            odlg.Filter = fileType;
            odlg.InitialDirectory = Application.StartupPath;
            var ret = odlg.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            try
            {
                string file = odlg.FileName;
                string json = System.IO.File.ReadAllText(file);
                this.file = Newtonsoft.Json.JsonConvert.DeserializeObject<GridAutoLayoutSLN>(json);
                if (this.file != null)
                {

                    this.showFile2View(this.file);
                }
                else
                {
                    MessageBox.Show("读取工程文件错误,未解析工程");
                    return;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("尝试读取工程文件失败\r\n"+err.Message);
            }
        }

        void showFile2View(GridAutoLayoutSLN file)
        {
            foreach (var m in file.Medicines)
            {
                this.addMedicine2DGV(m);
            }
            //if (this.file.LayoutMode == LayoutModeEnum.Method1)
            //{
            //    粗略计算按钮_Click(sender, e);
            //}
            //else if(this.file.LayoutMode == LayoutModeEnum.Method2)
            //{
            //    尝试计算所有的可能性按钮_Click(sender, e);
            //}
            this.addFloors2Dgv(file.Floors);
            this.printerYScaleRateTextbox.Text = file.printerScaleY.ToString();
            this.panel1.Invalidate();
        }

        

        private void 应用药槽和药品绑定信息按钮_Click(object sender, EventArgs e)
        {
            #region 检查是否超出最大行数
            if (checkFloorCount() == false)
            {
                MessageBox.Show(this, "已经超出药仓载量,请移除部分药品再重新尝试");
                return;
            }
            #endregion
            #region 如果层数不够的话 自动添加空层
            int needRemoveFloorCount = getNeedCombainFloorsCount(file.Floors);
            int needAddEmptyFloorCount = this.file.maxFloorCount - file.Floors.Count - needRemoveFloorCount;
            if (needAddEmptyFloorCount < 0)
            {
                needAddEmptyFloorCount = 0;
            }
            for (int i = 0; i < needAddEmptyFloorCount; i++)
            {
                file.Floors.Add(new Floor((int)(this.destStock.MaxFloorWidthMM  -App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM), (int)file.maxClipDepth));
            }
            #endregion
            AMDMHardwareInfoManager hm = new AMDMHardwareInfoManager(App.sqlClient);
            var clips = App.bindingManager.GetStockAllBindedMedicineWithMedicineObject(this.destStock.IndexOfMachine);
            bool needClearMedicinesObject = false;
            bool needClearClipBind = false;
            if (clips.Count>0)
            {
                int medicineCount = 0;
                foreach (var c in clips)
                {
                    medicineCount += c.MedicineObjects == null ? 0 : c.MedicineObjects.Count;
                }
                if (medicineCount >0)
                {
                    var mbret = MessageBox.Show(this,string.Format("当前药仓中仍有{0}个药品\r\n要清空这些药品并清除药槽绑定信息吗?",medicineCount),"将清空未出仓药品数据并清空药槽绑定!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (mbret == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                    else
                    {
                        needClearMedicinesObject = true;
                        needClearClipBind = true;
                    }
                }
                var ret = MessageBox.Show(this,string.Format("当前药仓中已有{0}个药槽被绑定\r\n要覆盖这些绑定信息并继续吗?",clips.Count),"将清空绑定信息", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (ret == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
                else
                {
                    needClearClipBind = true;
                }
            }
            
            //如果需要清空药品,要把所有的没出库的药品都出库掉
            if (needClearMedicinesObject)
            {
                foreach (var c in clips)
                {
                    App.bindingManager.ZeroMedicineCount(new AMDM_Grid() { StockIndex = c.StockIndex, FloorIndex = c.FloorIndex, IndexOfFloor = c.GridIndex, }, 0);
                }
            }
            //如果需要清空药槽绑定,需要把所有的药槽绑定数据都清除掉
            if (needClearClipBind)
            {
                foreach (var c in clips)
                {
                    App.bindingManager.UnBindMedicine(c.StockIndex, c.FloorIndex, c.GridIndex);
                }
            }
            //清空所有stock的层和格
            AMDM_Stock oldStock = hm.LoadStock(this.destStock.IndexOfMachine);
            if (oldStock!= null)
            {
                hm.ClearStockFloorAndGirds(oldStock);
                hm.RemoveStock(oldStock.Id);
            }
            
            hm.JoinStock(null, this.destStock);
            this.destStock.Floors.Clear();
            
            //删除stock在数据库中的信息
            //创建stock并添加层,药槽到stock
            //要反向插入
            for (int i = file.Floors.Count-1; i >=0; i--)
            {
                var f = file.Floors[i];
                var newFloor = hm.CreateAndJoinNewUpPartFloor(this.destStock, App.Setting.HardwareSetting.Floor.FloorHeightMM, this.destStock.MaxFloorWidthMM, App.Setting.HardwareSetting.Floor.UpPartFloorDepthMM);
                float gridStartX = App.Setting.HardwareSetting.Grid.GridWallFixtureFullWidthMM / 2;
                foreach (var m in f.Medicines)
                {
                    AMDM_Grid newGrid = hm.CraeteAndJoinNewGrid(newFloor, null, gridStartX, m.RightGridWidth, m.IndexOfStock);
                    gridStartX += m.RightGridWidth;
                    App.bindingManager.BindMedicine2Grid(m, newGrid);
                }
            }
            //添加绑定信息
            MessageBox.Show("完成");
        }

        private void 按从小到大顺序排列格子选择框_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void GridAutoLayouter_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.save(this.fileName);
        }

        private void 修改尺寸按钮_Click(object sender, EventArgs e)
        {
            if (this.药品信息列表.SelectedRows.Count != 1)
            {
                return;
            }
            DataGridViewRow row = this.药品信息列表.SelectedRows[0];
            long id = (long)row.Cells["columnMedicineId"].Value;
            List<WaitJoinMedicine> ms = this.getMedicinesFromWaitJoinList(file.Medicines, id);
            if (ms!= null && ms.Count>0)
            {
                this.inputMedicineSize(ms[0],false,false);
            }
        }

        private void changePrinterYScaleRateBtn_Click(object sender, EventArgs e)
        {
            NumberInputForm iform = new NumberInputForm("请输入打印机缩放系数", "缩放系数", file.printerScaleY, true);
            if (iform.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            this.file.printerScaleY = iform.InputValue;
            this.printerYScaleRateTextbox.Text = this.file.printerScaleY.ToString();
        }
    }
    public enum LayoutModeEnum { NONE, Method1, Method2 };
    /// <summary>
    /// 用于保存的工程文件信息
    /// </summary>
    public class GridAutoLayoutSLN
    {
        public GridAutoLayoutSLN()
        {
            this.Floors = new List<Floor>();
            this.Medicines = new List<WaitJoinMedicine>();
        }
        public List<Floor> Floors { get; set; }
        public List<WaitJoinMedicine> Medicines { get; set; }
        public LayoutModeEnum LayoutMode { get; set; }
        /// <summary>
        /// 显示时的最高行数,也可以检测药品是否可以放到一个药柜中
        /// </summary>
        public int maxFloorCount = 0;

        public double printerScaleY = 1.1948;

        public float maxClipDepth = 0;

        //public float gridWallFixtureFullWidthMM = 0;

        public float gridWallWidthMM = 0;

        public float floorMedicineBoxMaxHeightM = 0;
    }
    #region 扩展AMDM药机的药品信息,只扩展了一个字段就是正合适的药槽的尺寸
    public class WaitJoinMedicine : AMDM_Medicine
    {
        /// <summary>
        /// 该药品要使用的药槽的最合适宽度是多少.要加上格子隔板宽度信息
        /// </summary>
        public float RightGridWidth { get; set; }

        public int IndexOfStock { get; set; }

        /// <summary>
        /// 表示该药品是否超高
        /// </summary>
        public bool TooHigh { get; set; }
    }
    #endregion
    #region 类定义

    public class Floor
    {
        public Floor(int widthMM, int depthMM)
        {
            this.Medicines = new List<WaitJoinMedicine>();
            this.MaxWidth = widthMM;
        }
        /// <summary>
        /// 这个层的最大宽度是多少
        /// </summary>
        public float MaxWidth { get; set; }
        /// <summary>
        /// 当前已经使用了多少宽度
        /// </summary>
        public float CurrentUsedWidth { get; set; }
        /// <summary>
        /// 当前已经加入的药品信息集合
        /// </summary>
        public List<WaitJoinMedicine> Medicines { get; set; }

        /// <summary>
        /// 还剩余多少空间
        /// </summary>
        public float RemaindWidth { get { return this.MaxWidth - CurrentUsedWidth; } }

        public string FloorName { get; set; }

        /// <summary>
        /// 是否为超高层
        /// </summary>
        public bool IsTooHighFloor { get; set; }
    }

    #endregion
}
