using System;
using System.Collections.Generic;
using System.Text;

namespace FakeHISServer.Domain
{
    public class HISMedicineOrder_data
    {
        Int64 _id = 0;
        public Int64 Id { get { return _id; } set { _id = value; } }

        long _creatorid = 0;
        public long CreatorId { get { return _creatorid; } set { _creatorid = value; } }

        Int64 _patientid = 0;
        public Int64 PatientId { get { return _patientid; } set { _patientid = value; } }

        Int32 _pharmacyid = 0;
        public Int32 PharmacyId { get { return _pharmacyid; } set { _pharmacyid = value; } }

        Nullable<DateTime> _createtime = null;
        public Nullable<DateTime> CreateTime { get { return _createtime; } set { _createtime = value; } }

        Nullable<DateTime> _modifiedtime = null;
        public Nullable<DateTime> ModifiedTime { get { return _modifiedtime; } set { _modifiedtime = value; } }

        Nullable<DateTime> _balancetime = null;
        public Nullable<DateTime> BalanceTime { get { return _balancetime; } set { _balancetime = value; } }

        Nullable<DateTime> _fulfilledtime = null;
        public Nullable<DateTime> FulfilledTime { get { return _fulfilledtime; } set { _fulfilledtime = value; } }

        Int32 _entriescount = 0;
        public Int32 EntriesCount { get { return _entriescount; } set { _entriescount = value; } }

        Int32 _totalcount = 0;
        public Int32 TotalCount { get { return _totalcount; } set { _totalcount = value; } }

        bool _balance = false;
        public bool Balance { get { return _balance; } set { _balance = value; } }

        bool _fulfilled = false;
        public bool Fulfilled { get { return _fulfilled; } set { _fulfilled = value; } }

        Int64 _fulfillmentnurseid = 0;
        public Int64 FulfillmentNurseId { get { return _fulfillmentnurseid; } set { _fulfillmentnurseid = value; } }

        bool _canceled = false;
        public bool Canceled { get { return _canceled; } set { _canceled = value; } }
    }

    public class HISMedicineOrder:HISMedicineOrder_data
    {
        public List<HISMedicineOrderDetail> Details { get; set; }
    }
}
