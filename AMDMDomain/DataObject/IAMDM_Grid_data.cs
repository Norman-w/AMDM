using System;
namespace AMDM_Domain
{
    interface IAMDM_Grid_data
    {
        float BottomMM { get; set; }
        DateTime CreateTime { get; set; }
        int FloorId { get; set; }
        int FloorIndex { get; set; }
        int IndexOfFloor { get; set; }
        int IndexOfStock { get; set; }
        long LastModifiedUserId { get; set; }
        float LeftMM { get; set; }
        DateTime ModifiedTime { get; set; }
        float RightMM { get; set; }
        int StockId { get; set; }
        int StockIndex { get; set; }
        float TopMM { get; set; }
    }
}
