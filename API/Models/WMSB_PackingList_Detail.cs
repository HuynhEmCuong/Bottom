using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bottom_API.Models
{
    public class WMSB_PackingList_Detail
    {
        [Key]
        public long SID { get; set; }
        public string Receive_No {get;set;}
        public string Order_Size { get; set; }
        public string Model_Size { get; set; }
        public string Tool_Size {get;set;}
        public string Spec_Size { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal? MO_Qty {get;set;}
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Purchase_Qty { get; set; }
        [Column(TypeName = "decimal(9,2)")]
        public decimal? Received_Qty {get;set;}
        public DateTime? Updated_Time {get;set;}
        public string Updated_By {get;set;}
    }
}