using System;
using System.Collections.Generic;
using System.Text;

namespace AMDM_Domain
{
    public class AMDM_MedicineOrderDetail
    {
	Int64 _id = 0;
	public Int64 Id {get {return _id;} set{ _id = value;} }

	Int64 _orderid = 0;
	public Int64 OrderId {get {return _orderid;} set{ _orderid = value;} }

	Int64 _drugid = 0;
	public Int64 MedicineId {get {return _drugid;} set{ _drugid = value;} }

	String _name = null;
	public String Name {get {return _name;} set{ _name = value;} }

	String _barcode = null;
	public String Barcode {get {return _barcode;} set{ _barcode = value;} }

	int _count = 0;
    public int Count { get { return _count; } set { _count = value; } }

    bool _fulfilled = false;
    public bool Fulfilled { get { return _fulfilled; } set { _fulfilled = value; } }

	Nullable<DateTime> _fulfilledtime = null;
    public Nullable<DateTime> FulfilledTime { get { return _fulfilledtime; } set { _fulfilledtime = value; } }

    Nullable<long> _fulfilledpharmacyid = null;
    public Nullable<long> FulfilledPharmacyId { get { return _fulfilledpharmacyid; } set { _fulfilledpharmacyid = value; } }

	Nullable<long> _fulfillednurseid = null;
    public Nullable<long> FulfilledNurseId { get { return _fulfillednurseid; } set { _fulfillednurseid = value; } }

    bool _canceled = false;
    public bool Canceled { get { return _canceled; } set { _canceled = value; } }
}
}
