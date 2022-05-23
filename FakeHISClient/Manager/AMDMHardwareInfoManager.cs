using AMDM.Manager;
using AMDM_Domain;
using FakeHISClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MyCode;

    #region 类型定义
    public enum MoveDirectionEnum { LeftWall2Left, RightWall2Right, LeftWall2Right, RightWall2Left, RemoveLeftWall, RemoveWriteWall };
    #endregion
    /// <summary>
    /// 药机实体库自身信息管理器
    /// </summary>
    public class AMDMHardwareInfoManager : AMDMStockLoader
    {
        public AMDMHardwareInfoManager(SQLDataTransmitter sqlClient)
            : base(sqlClient)
        {
        }
        #region 初始化一个药仓并且加入到机器中
        ///// <summary>
        ///// 初始化药库
        ///// </summary>
        ///// <param name="floorCount">总层数信息</param>
        ///// <param name="gridCountPerFloor">每一层有多少个格子(所有层都一样)</param>
        ///// <returns></returns>
        //public AMDM_Stock CreateAndJoinStock(AMDM_Machine machine, int floorCount, int gridCountPerFloor, float floorWidth, float floorDepthMM)
        //{
        //    #region 如果机器没有药仓列表 创建
        //    if (machine.Stocks == null)
        //    {
        //        machine.Stocks = new List<AMDM_Stock>();
        //    }
        //    #endregion
        //    AMDM_Stock newStock = new AMDM_Stock();
        //    InitStock(ref newStock, floorCount, gridCountPerFloor, floorWidth, floorDepthMM);
        //    machine.Stocks.Add(newStock);

        //    return newStock;
        //}

        ///// <summary>
        ///// 初始化药库,根据已经包含格子信息的层信息集合
        ///// </summary>
        ///// <param name="floors">层信息集合</param>
        ///// <returns></returns>
        //public AMDM_Stock CreateAndJoinStock(List<AMDM_Floor> floors)
        //{
        //    AMDM_Stock_data newStock = null;
        //    return newStock;
        //}
        /// <summary>
        /// 初始化药库,根据包含层信息和格子信息的 药仓信息 进行初始化
        /// </summary>
        /// <param name="stock">药仓信息</param>
        /// <returns></returns>
        public AMDM_Stock_data CreateAndJoinStock(ref AMDM_Machine machine,
            float CenterDistanceBetweenTwoGrabbers,
            int IndexOfMachine,
            int MaxFloorsHeightMM,
            int MaxFloorWidthMM,
            string SerialNumber,
            float XOffsetFromStartPointMM,
            float YOffsetFromStartPointMM
               )
        {
            AMDM_Stock newStock = new AMDM_Stock()
            {
                CenterDistanceBetweenTwoGrabbers = CenterDistanceBetweenTwoGrabbers,
                FirstLayoutTime = DateTime.Now,
                IndexOfMachine = IndexOfMachine,
                MachineId = machine.Id,
                MaxFloorsHeightMM = MaxFloorsHeightMM,
                MaxFloorWidthMM = MaxFloorWidthMM,
                SerialNumber = SerialNumber == null ? Guid.NewGuid().ToString("N") : SerialNumber,
                XOffsetFromStartPointMM = XOffsetFromStartPointMM,
                YOffsetFromStartPointMM = YOffsetFromStartPointMM
            };
            if (client.AddStock(newStock))
            {
                if (machine.Stocks == null)
                {
                    machine.Stocks = new List<AMDM_Stock>();
                }
                machine.Stocks.Add(newStock);
            }
            else
            {
                return null;
            }
            return newStock;
        }
        #endregion

        #region 从数据库加载Stock信息,并且加载全部的floors和grids信息
        /// <summary>
        /// 读取指定stock的层和药槽信息.
        /// </summary>
        /// <param name="stockIndex"></param>
        /// <returns></returns>
        public AMDM_Stock LoadStock(int stockIndex)
        {
            AMDM_Stock loadedStock = new AMDM_Stock();
            AMDM_Stock_data stock_data = client.GetStock(stockIndex);
            if (stock_data == null)
            {
                return null;
            }
            string stockJson = JsonConvert.SerializeObject(stock_data);
            JsonConvert.PopulateObject(stockJson, loadedStock);
            //loadedStock.Floors = new List<AMDM_Floor>();
            loadedStock.Floors = new Dictionary<int, AMDM_Floor>();
            //读取层信息
            List<AMDM_Floor_data> floorsData = client.GetFloors(loadedStock.Id);
            string floorsJson = JsonConvert.SerializeObject(floorsData);
            List<AMDM_Floor> floors = new List<AMDM_Floor>();
            JsonConvert.PopulateObject(floorsJson, floors);
            //读取格子信息
            foreach (var floor in floors)
            {
                int floorIndex = floor.IndexOfStock;
                loadedStock.Floors.Add(floorIndex, floor);

                int currentFloorId = floor.Id;
                List<AMDM_Grid_data> gridsData = client.GetGrids(currentFloorId);
                string gridsJson = JsonConvert.SerializeObject(gridsData);
                floor.Grids = new List<AMDM_Grid>();
                JsonConvert.PopulateObject(gridsJson, floor.Grids);
            }
            return loadedStock;
        }
        #endregion

        #region 重新初始化药仓,清空全部的药槽和层
        /// <summary>
        /// 把指定的药仓内的格子和层板都清空
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public bool ClearStockFloorAndGirds(AMDM_Stock stock)
        {
            if (stock.Floors == null)
            {
                return false;
            }
            foreach (var item in stock.Floors)
            {
                client.ClearFloorById(item.Value.Id);
                item.Value.Grids.Clear();
            }
            client.ClearStockByStockId(stock.Id);
            stock.Floors.Clear();
            return true;
        }
        /// <summary>
        /// 初始化一个药仓,如果给定的药仓里面的格子被绑定了,那么就提示不能初始化该药仓
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="floorCount"></param>
        /// <param name="gridCountPerFloor"></param>
        /// <param name="floorWidth"></param>
        /// <returns></returns>
        public AMDM_Stock InitStock(ref AMDM_Stock stock,
            int upPartFloorCount,
            int downPartFloorCount,
            int gridCountPerUpPartFloor,
            int gridCountPerDownPartFloor,
            float upPartFloorWidth,
            float downPartFloorWidth,
            float upPartFloorDepthMM,
            float downPartFloorDepthMM
            )
        {
            if (stock != null && (stock.Id > 0 || stock.IndexOfMachine >= 0))
            {
                //如果药仓不是空的,检查这个药仓的所有的格子是否有绑定的药品
                List<AMDM_Clip_data> bindingInfos = client.GetStockBindingInfo(stock.IndexOfMachine);
                if (bindingInfos.Count > 0)
                {
                    throw new NotImplementedException(string.Format("该药仓中有{0}种药品尚未解绑", bindingInfos.Count));
                }
            }
            //stock = new AMDM_Stock();
            stock.MaxFloorWidthMM = upPartFloorWidth;
            //client.AddStock(stock as AMDM_Stock_data);
            stock.Floors = new Dictionary<int, AMDM_Floor>();
            #region 初始化上半部分的层板
            for (int i = 0; i < upPartFloorCount; i++)
            {
                AMDM_Floor newFloor = this.CreateAndJoinNewUpPartFloor(stock, stock.MaxFloorsHeightMM * 1f / upPartFloorCount, upPartFloorWidth, upPartFloorDepthMM);
                if (newFloor == null)
                {
                    continue;
                }
                else
                {
                    for (int j = 0; j < gridCountPerUpPartFloor; j++)
                    {
                        float width = upPartFloorWidth / gridCountPerUpPartFloor;
                        float left = width * j;
                        AMDM_Grid newGrid = this.CraeteAndJoinNewGrid(newFloor, null, left, width);
                    }
                }
            }
            #endregion
            #region 初始化下半部分的层板
            for (int i = 0; i < downPartFloorCount; i++)
            {
                AMDM_Floor newFloor = this.CreateAndJoinNewDownPartFloor(stock, downPartFloorWidth, downPartFloorDepthMM);
                if (newFloor == null)
                {
                    continue;
                }
                else
                {
                    for (int j = 0; j < gridCountPerDownPartFloor; j++)
                    {
                        float width = downPartFloorWidth / gridCountPerDownPartFloor;
                        float left = width * j;
                        AMDM_Grid newGrid = this.CraeteAndJoinNewGrid(newFloor, null, left, width);
                    }
                }
            }
            #endregion

            return stock;
        }
        #endregion

        #region 删除层和格子
        /// <summary>
        /// 在药仓中移除层和层内的格子集合,在数据库中移除后再移除对象中的数据
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public bool RemoveFloorAndGrids(ref AMDM_Stock stockRef, int floorId)
        {
            //bool removeGridsRet = 
            client.RemoveGrids(floorId);
            //if (removeGridsRet)
            //{
            AMDM_Floor destFloor = null;
            foreach (var item in stockRef.Floors)
            {
                if (item.Value.Id == floorId)
                {
                    destFloor = item.Value;
                    break;
                }
            }
            destFloor.Grids.Clear();
            bool removeFloorRet = client.RemoveFloor(floorId);
            if (removeFloorRet)
            {
                stockRef.Floors.Remove(destFloor.IndexOfStock);
            }
            //}
            return true;
        }
        #endregion

        #region 移除格子的墙板
        public bool RemoveGridWall(AMDM_Grid currentGrid, ref AMDM_Floor currentFloor, MoveDirectionEnum type)
        {
            //bool ret = MoveGridWall(currentGrid, type);
            //if (ret)
            //{
            int removedGridIndex = currentFloor.Grids.IndexOf(currentGrid);
            if (!client.RemoveGrid(currentGrid.Id))
            {
                return false;
            }
            currentFloor.Grids.Remove(currentGrid);
            if (type == MoveDirectionEnum.RemoveLeftWall)
            {
                //左边的格子的大小要扩大,
                AMDM_Grid preGrid = currentFloor.Grids[removedGridIndex - 1];
                preGrid.RightMM = currentGrid.RightMM;
                client.UpdateGrid(preGrid);
            }
            else if (type == MoveDirectionEnum.RemoveWriteWall)
            {
                AMDM_Grid nextGrid = currentFloor.Grids[removedGridIndex];
                nextGrid.LeftMM = currentGrid.LeftMM;
                client.UpdateGrid(nextGrid);
            }
            #region 移除了格子以后,被移除的格子的所有的右面的索引号都减少了1
            for (int i = removedGridIndex; i < currentFloor.Grids.Count; i++)
            {
                AMDM_Grid currentNeedModifiedIndexGrid = currentFloor.Grids[i];
                currentNeedModifiedIndexGrid.IndexOfFloor -= 1;
                if (!client.UpdateGrid(currentNeedModifiedIndexGrid))
                {
                    currentNeedModifiedIndexGrid.IndexOfFloor += 1;
                }
            }
            #endregion
            //}

            return true;
        }
        #endregion

        #region 获取所有的层信息,获取所有的格子信息
        /// <summary>
        /// 获取所有的层信息
        /// </summary>
        /// <param name="containsGridInfo">是否包含层内的格子信息</param>
        /// <returns></returns>
        public List<AMDM_Floor> GetAllFloorsInfo(bool containsGridInfo)
        {
            List<AMDM_Floor> floors = null;
            return floors;
        }
        /// <summary>
        /// 获取所有的格子信息,根据给定的层编号.如果不给定层编号,则默认获取所有的格子信息
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public List<AMDM_Grid> GetAllGridsInfo(Nullable<int> floorId)
        {
            List<AMDM_Grid> grids = null;
            return grids;
        }
        #endregion

        #region 更新格子的信息
        public bool MoveGridWall(MoveGridParam param)
        {
            if (param.OffsetMM < 0)
            {
                //向左移动
                if (param.Dest != null)
                {
                    param.Dest.RightMM += param.OffsetMM;
                    if (client.UpdateGrid(param.Dest) == false)
                    {
                        param.Dest.RightMM -= param.OffsetMM;
                    }
                }
                param.Src.LeftMM += param.OffsetMM;
                if (client.UpdateGrid(param.Src) == false)
                {
                    param.Src.LeftMM -= param.OffsetMM;
                }
            }
            else if (param.OffsetMM > 0)
            {
                //向右移动
                if (param.Dest != null)
                {
                    param.Dest.LeftMM += param.OffsetMM;
                    if (client.UpdateGrid(param.Dest) == false)
                    {
                        param.Dest.LeftMM -= param.OffsetMM;
                    }
                }
                param.Src.RightMM += param.OffsetMM;
                if (client.UpdateGrid(param.Src) == false)
                {
                    param.Src.RightMM -= param.OffsetMM;
                }
            }
            return true;
        }
        #endregion

        #region 逻辑交互部分,先操作数据库更新然后更新逻辑.
        /// <summary>
        /// 修改格子的宽度毫米,根据给定的纵横坐标
        /// </summary>
        /// <param name="yxLocation">格子的yx坐标</param>
        /// <param name="newWidthMM">新的格子的宽度</param>
        /// <returns></returns>
        public bool EditGridWIdthMM(Point yxLocation, double newWidthMM)
        {
            return false;
        }
        /// <summary>
        /// 在指定的药仓中添加层
        /// </summary>
        /// <param name="stock">原始药仓</param>
        /// <param name="indexOfStock">所在药仓中的层号,如果不指定,直接在原来的仓中向上增加一层</param>
        /// <returns></returns>
        public AMDM_Floor CreateAndJoinNewUpPartFloor(AMDM_Stock stock, float heightMM, float widthMM, float depthMM
            //, Nullable<int> indexOfStock
            )
        {
            //if (indexOfStock != null &&( indexOfStock.Value<0 || indexOfStock.Value>100))
            //{
            //    Console.WriteLine("无效的层索引信息");
            //    return null;
            //}
            AMDM_Floor lastFloor = null;
            int upPartFloorCurrentCount = this.getUpPartFloorCount(stock);
            if (upPartFloorCurrentCount > 0)
            {
                lastFloor = stock.Floors[upPartFloorCurrentCount - 1];
            }
            AMDM_Floor newFloor = new AMDM_Floor();
            newFloor.StockId = stock.Id;
            newFloor.IndexOfStock = upPartFloorCurrentCount == 0 ? 0 : upPartFloorCurrentCount;
            newFloor.BottomMM = lastFloor == null ? 0 : lastFloor.TopMM;
            newFloor.TopMM = newFloor.BottomMM + heightMM;
            newFloor.WidthMM = widthMM;
            newFloor.DepthMM = depthMM;
            newFloor.Grids = new List<AMDM_Grid>();
            this.client.AddFloor(newFloor);
            stock.Floors.Add(newFloor.IndexOfStock, newFloor);
            return newFloor;
        }
        /// <summary>
        /// 早下面部分添加层
        /// </summary>
        /// <param name="stock">原始药仓</param>
        /// <param name="indexOfStock">所在药仓中的层号,如果不指定,直接在原来的仓中向上增加一层</param>
        /// <returns></returns>
        public AMDM_Floor CreateAndJoinNewDownPartFloor(AMDM_Stock stock, float widthMM, float depthMM
            //, Nullable<int> indexOfStock
            )
        {
            //if (indexOfStock != null && (indexOfStock.Value > 0 || indexOfStock.Value < -10))
            //{
            //    Console.WriteLine("无效的层索引信息");
            //    return null;
            //}
            AMDM_Floor topestFloor = null;
            int downPartCurrentFloorCount = this.getDownPartFloorCount(stock);
            if (downPartCurrentFloorCount > 0)
            {
                topestFloor = stock.Floors[0 - downPartCurrentFloorCount];
            }
            AMDM_Floor newFloor = new AMDM_Floor();
            newFloor.StockId = stock.Id;
            newFloor.IndexOfStock = downPartCurrentFloorCount == 0 ? -1 : (0 - downPartCurrentFloorCount - 1);
            //newFloor.BottomMM = topestFloor == null ? 0 : topestFloor.TopMM;
            //newFloor.TopMM = newFloor.BottomMM + heightMM;
            newFloor.WidthMM = widthMM;
            newFloor.DepthMM = depthMM;
            newFloor.Grids = new List<AMDM_Grid>();
            this.client.AddFloor(newFloor);
            stock.Floors.Add(newFloor.IndexOfStock, newFloor);
            return newFloor;
        }
        /// <summary>
        /// 获取上面仓中的最上边的那个层.
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        int getUpPartFloorCount(AMDM_Stock stock)
        {
            int count = 0;
            if (stock.Floors == null || stock.Floors.Count == 0)
            {
                return 0;
            }
            foreach (var item in stock.Floors)
            {
                if (item.Key >= 0)
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// 获取下面部分的层板的最上面一层,最高层是-1,如果获取到0了,那就是下面没有层板
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        int getDownPartFloorCount(AMDM_Stock stock)
        {
            int count = 0;
            if (stock.Floors == null || stock.Floors.Count == 0)
            {
                return 0;
            }
            foreach (var item in stock.Floors)
            {
                if (item.Key < 0)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 像目标stock中添加floor和grid并保存到数据库,通常适用于把现有的json中的floor和grid数据添加到新建的stock中.
        /// </summary>
        /// <param name="destStock"></param>
        /// <param name="floors"></param>
        /// <returns></returns>
        public bool JoinFloorsAndGrids(ref AMDM_Stock destStock, Dictionary<int, AMDM_Floor> floorsDic)
        {
            if (destStock.Floors == null)
            {
                destStock.Floors = new Dictionary<int, AMDM_Floor>();
            }
            foreach (var item in floorsDic)
            {
                AMDM_Floor currentFloor = item.Value;
                //改变之前的floor的stockid为当前的stock的id
                currentFloor.StockId = destStock.Id;
                if (client.AddFloor(currentFloor) == false)
                {
                    Console.WriteLine("JoinFloorsAndGrids添加层信息错误");
                    return false;
                }
                else
                {
                    AMDM_Floor_data floor = JsonConvert.DeserializeObject<AMDM_Floor_data>(JsonConvert.SerializeObject(currentFloor));
                    AMDM_Floor newFloor = new AMDM_Floor();
                    JsonConvert.PopulateObject(JsonConvert.SerializeObject(floor), newFloor);
                    destStock.Floors.Add(newFloor.IndexOfStock, newFloor);
                    newFloor.Grids = new List<AMDM_Grid>();
                }
                for (int j = 0; j < currentFloor.Grids.Count; j++)
                {
                    var currentGrid = currentFloor.Grids[j];
                    currentGrid.FloorId = currentFloor.Id;
                    currentGrid.StockId = destStock.Id;
                    currentGrid.StockIndex = destStock.IndexOfMachine;
                    if (client.AddGrid(currentGrid) == false)
                    {
                        Console.WriteLine("JoinFloorsAndGrids添加药槽信息错误");
                        return false;
                    }
                    else
                    {
                        AMDM_Grid_data grid = JsonConvert.DeserializeObject<AMDM_Grid_data>(JsonConvert.SerializeObject(currentGrid));
                        AMDM_Grid newGrid = new AMDM_Grid();
                        JsonConvert.PopulateObject(JsonConvert.SerializeObject(grid), newGrid);

                        destStock.Floors[item.Key].Grids.Add(newGrid);
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 在指定的层中创建一个格子
        /// </summary>
        /// <param name="floor">层信息</param>
        /// <param name="indexOfFloor">新格子所在层中的索引位置,如果不自定,直接在原来的层中向后添加一个格子</param>
        /// <returns></returns>
        public AMDM_Grid CraeteAndJoinNewGrid(AMDM_Floor floor, Nullable<int> indexOfFloor, float leftMM, float widthMM)
        {
            if (indexOfFloor != null && (indexOfFloor < 0 || indexOfFloor > 100))
            {
                Console.WriteLine("无效的格子索引");
                return null;
            }
            AMDM_Grid newGrid = new AMDM_Grid();
            newGrid.FloorId = floor.Id;
            newGrid.StockId = floor.StockId;
            newGrid.CreateTime = DateTime.Now;
            //newGrid.FloorId = floor.Id;
            newGrid.FloorIndex = floor.IndexOfStock;
            newGrid.IndexOfFloor = indexOfFloor == null ? floor.Grids.Count : indexOfFloor.Value;
            newGrid.LeftMM = leftMM;
            newGrid.RightMM = leftMM + widthMM;
            newGrid.TopMM = floor.TopMM;
            newGrid.BottomMM = floor.BottomMM;
            this.client.AddGrid(newGrid);
            floor.Grids.Insert(newGrid.IndexOfFloor, newGrid);
            return newGrid;
        }
        /// <summary>
        /// 在指定的层中删除格子
        /// </summary>
        /// <param name="floor">层数据</param>
        /// <param name="indexOfFloor">要删除的格子所层中的索引位置</param>
        /// <returns></returns>
        public bool RemoveGrid(AMDM_Floor floor, int indexOfFloor)
        {
            return false;
        }
        #endregion

        #region 校验层内的数据是否合法
        /// <summary>
        /// 校验层内的宽度信息是否合法.如果层内的信息总宽度超过层的宽度,不合法,如果是层内的格子 第0个格子的结束是100毫米,第1个格子的起点位置是90毫米 就错了.
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        public bool CheckFloorInfo(AMDM_Floor floor)
        {
            return false;
        }
        #endregion

        #region 更新呢库的基本信息
        public bool UpdateStock(ref AMDM_Stock stock, float MaxFloorWidthMM, float XOffsetFromStartPointMM,
            float YOffsetFromStartPointMM, float CenterDistanceBetweenTwoGrabbers
            )
        {
            AMDM_Stock temp = JsonConvert.DeserializeObject<AMDM_Stock>(JsonConvert.SerializeObject(stock));
            temp.MaxFloorWidthMM = MaxFloorWidthMM;
            temp.XOffsetFromStartPointMM = XOffsetFromStartPointMM;
            temp.YOffsetFromStartPointMM = YOffsetFromStartPointMM;
            temp.CenterDistanceBetweenTwoGrabbers = CenterDistanceBetweenTwoGrabbers;
            if (client.UpdateStock(temp))
            {
                stock.MaxFloorWidthMM = MaxFloorWidthMM;
                stock.XOffsetFromStartPointMM = XOffsetFromStartPointMM;
                stock.YOffsetFromStartPointMM = YOffsetFromStartPointMM;
                stock.CenterDistanceBetweenTwoGrabbers = CenterDistanceBetweenTwoGrabbers;
                return true;
            }
            return false;
        }
        #endregion

        #region 更新层信息,更新完了数据库以后更新实体的信息
        public bool UpdateFloor(ref AMDM_Floor floor, float widhtMM, float topMM, float bottomMM)
        {
            AMDM_Floor temp = JsonConvert.DeserializeObject<AMDM_Floor>(JsonConvert.SerializeObject(floor));
            temp.WidthMM = widhtMM;
            temp.TopMM = topMM;
            temp.BottomMM = bottomMM;
            if (client.UpdateFloor(temp))
            {
                floor.WidthMM = widhtMM;
                floor.TopMM = topMM;
                floor.BottomMM = bottomMM;
                return true;
            }
            return false;
        }
        #endregion

        #region 2022年2月18日10:34:38 更新格子信息
        public bool UpdateGridIndexOfStock(ref AMDM_Grid grid, int indexOfStock, Nullable<long> lastModifiedUserId)
        {
            AMDM_Grid temp = JsonConvert.DeserializeObject<AMDM_Grid>(JsonConvert.SerializeObject(grid));
            if (lastModifiedUserId!= null)
            {
                temp.LastModifiedUserId = lastModifiedUserId.Value;
            }
            if (client.UpdateGrid(temp))
            {
                grid.IndexOfStock = indexOfStock;
                return true;
            }
            return false;
        }
        #endregion

        #region 更新格子
        /// <summary>
        /// 修改格子的宽度毫米
        /// </summary>
        /// <param name="girdId">格子id</param>
        /// <param name="newWidthMM">新的宽度mm</param>
        /// <returns></returns>
        public bool EditGridWidthMM(int girdId, double newWidthMM)
        {
            return false;
        }
        /// <summary>
        /// 修改格子的宽度毫米,根据给定的层id和在层中格子的位置信息
        /// </summary>
        /// <param name="floorId"></param>
        /// <param name="index"></param>
        /// <param name="newWidthMM"></param>
        /// <returns></returns>
        public bool EditGridWidthMM(int floorId, int indexOfFloor, double newWidthMM)
        {
            return false;
        }
        #endregion

        #region 删除库
        public bool RemoveStock(ref AMDM_Machine machine, int stockId)
        {
            if (client.RemoveStock(stockId))
            {
                AMDM_Stock needRemoveStock = null;
                for (int i = 0; i < machine.Stocks.Count; i++)
                {
                    if (machine.Stocks[i].Id == stockId)
                    {
                        needRemoveStock = machine.Stocks[i];
                        break;
                    }
                }
                if (needRemoveStock != null)
                {
                    machine.Stocks.Remove(needRemoveStock);
                }
                return true;
            }
            return false;
        }
        #endregion
    }
