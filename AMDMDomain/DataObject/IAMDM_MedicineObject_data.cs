using System;
namespace AMDM_Domain
{
    public interface IAMDM_MedicineObject_data
    {
        DateTime? ExpirationDate { get; set; }
        int FloorIndex { get; set; }
        int GridIndex { get; set; }
        long InStockRecordId { get; set; }
        DateTime InStockTime { get; set; }
        long MedicineId { get; set; }
        long? OutStockRecordId { get; set; }
        DateTime? OutStockTime { get; set; }
        DateTime? ProductionDate { get; set; }
        int StockIndex { get; set; }
    }
}
