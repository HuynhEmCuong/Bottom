using System;

namespace Bottom_API.DTO
{
    public class POListT3_Dto
    {
        public string  Subcon_Name { get; set; }
        public string Process_Code { get; set; }
        public  string ToolID { get; set; }
            public  string ToolType { get; set; }
        public string  PO { get; set; }
        public string  Batch { get; set; }

        public string MTCode { get; set; }
        public string  MaterinalName { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }
        public string  T3Supplier { get; set; }

        public string T3SupplierName { get; set; }

        public string  ASLine { get; set; }

        public DateTime? STFStartDate { get; set; }
        public decimal? TTL { get; set; }
        public int? POQuantity { get; set; }

        public string T3SupplierAndName
        {
            get
            {
                return T3Supplier + " " + T3SupplierName;
            }
        }
        public string Tooling
        {
            get
            {
                return ToolType + "-" + ToolID;
            }
        }
        public string PoBatch
        {
            get
            {
                return PO + Batch;
            }
        }
        public string Treatment
        {
            get
            {
                return Process_Code + Subcon_Name;
            }
        }
    }
}