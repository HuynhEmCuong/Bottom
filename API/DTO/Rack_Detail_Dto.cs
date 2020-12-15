using System;

namespace Bottom_API.DTO
{
    public class Rack_Detail_Dto
    {
        public string Rack_Location {get;set;}
        public string Tool_Type {get;set;}
        public string Plan_No {get;set;}
        public string MO_Seq {get;set;}
        public string Tool_ID {get;set;}
        public string Material_ID {get;set;}
        public string Material_Name {get;set;}
        public string Unit {get;set;}
        public decimal? Transacted_Qty {get;set;}
        public string Supplier_No {get;set;}
        public string Supplier_Name {get;set;}
        public string Line_ASY {get;set;}
        public DateTime? Stockfiting_Date {get;set;}
        public decimal? Transacted_Qty_Sum {get;set;}
        public int PO_Qty {get;set;}
        public string T2Supplier
        {
            get
            {
                return Supplier_No + " " + Supplier_Name;
            }
        }
        public string Tooling
        {
            get
            {
                return Tool_Type + "-" + Tool_ID;
            }
        }
        public string PoBatch
        {
            get
            {
                return Plan_No + MO_Seq;
            }
        }
    }
}