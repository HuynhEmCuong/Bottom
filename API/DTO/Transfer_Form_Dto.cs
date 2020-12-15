using System;

namespace Bottom_API.DTO
{
    public class Transfer_Form_Dto
    {
        public string Collect_Trans_No { get; set; }
        public string Transac_No { get; set; }
        public DateTime? Generate_Time { get; set; }
        public string T3_Supplier { get; set; }
        public string Article { get; set; }
        public string Is_Release { get; set; }
        public string Release_By { get; set; }
        public DateTime? Release_Time { get; set; }
        public DateTime Update_Time { get; set; }
        public string Update_By { get; set; }
    }
}