using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    /// <summary>
    /// 保存拍照截图的相关信息.需要拍照的时候,在这个表中记录一条拍照数据,然后拍照在本机保存
    /// </summary>
    public class AMDM_Snapshot_data
    {
        Int64 _id = 0;
        /// <summary>
        /// 自增量ID,这个id在其他的需要用到截图的地方保存,也可以其他地方不保存,直接在本表中保存一个forid
        /// </summary>
        public Int64 Id { get { return _id; } set { _id = value; } }

        SnapshotParentTypeEnum _parenttype =  SnapshotParentTypeEnum.Unknow;
        /// <summary>
        /// 为哪个表拍的照,主子是谁
        /// </summary>
        public SnapshotParentTypeEnum ParentType { get { return _parenttype; } set { _parenttype = value; } }

        Int64 _parentid = 0;
        /// <summary>
        /// 为那个表下面的哪个id拍的照
        /// </summary>
        public Int64 ParentId { get { return _parentid; } set { _parentid = value; } }

        Nullable<SnapshotLocationEnum> _location = null;
        /// <summary>
        /// 拍摄的什么位置的照片 比如 药仓0后背板左侧  出药口 或者用户交互区之类的
        /// </summary>
        public Nullable<SnapshotLocationEnum> Location { get { return _location; } set { _location = value; } }

        DateTime _time = DateTime.Now;
        /// <summary>
        /// 拍摄时间
        /// </summary>
        public DateTime Time { get { return _time; } set { _time = value; } }

        String _because = null;
        /// <summary>
        /// 因为啥拍照,是取药或者是什么联动,或者什么报警之类的
        /// </summary>
        public String Because { get { return _because; } set { _because = value; } }

        String _localfilepath = null;
        /// <summary>
        /// 保存到本地的文件的路径是什么.
        /// </summary>
        public String LocalFilePath { get { return _localfilepath; } set { _localfilepath = value; } }

        /// <summary>
        /// 在什么时候执行的拍照,动作前,动作后还是动作发生正当时
        /// </summary>
        public Nullable<SnapshotTimePointEnum> TimePoint { get; set; }
    }
    public class AMDM_Snapshot : AMDM_Snapshot_data
    {
        /// <summary>
        /// 用于外部访问时候的url
        /// </summary>
        public string FileUrl { get; set; }
    }
}
