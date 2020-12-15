using System.Collections.Generic;
using Bottom_API.ViewModel;

namespace Bottom_API.DTO
{
    public class OutputPrintQrCode_Dto
    {
        public QRCodeMainViewModel QrCodeModel { get; set; }
        public List<PackingListDetailViewModel> PackingListDetail { get; set; }
        public string RackLocation { get; set; }
    }
}