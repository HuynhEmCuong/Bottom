using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.Helpers;
using Bottom_API.ViewModel;

namespace Bottom_API._Services.Interfaces
{
    public interface IReceivingService
    {
        Task<PagedList<MaterialMainViewModel>> SearchByModel(PaginationParams param,FilterMaterialParam filterParam);
        Task<object> MaterialMerging(MaterialMainViewModel model);
        Task<List<ReceiveNoMain>> UpdateMaterial(List<OrderSizeByBatch> data, string updateBy);
        Task<bool> UpdateStatusMaterial(string purchaseNo,string mo_No, string mOSeq, string missingNo);
        Task<bool> UpdateStatusMaterialSubmit(string purchaseNo,string mo_No, string mOSeq, string missingNo);
        Task<List<ReceiveNoDetail>> ReceiveNoDetails(string receive_No);
        Task<List<ReceiveNoMain>> ReceiveNoMain(MaterialMainViewModel model);
        Task<bool> ClosePurchase(MaterialMainViewModel model);
        Task<string> StatusPurchase (MaterialMainViewModel model);

        // Show dữ liệu ra bảng edit
        Task<List<MaterialEditModel>> EditMaterial(ReceiveNoMain model);

        // Tiến hàng edit lại dữ liệu.
        Task<bool> EditDetail(List<MaterialEditModel> data);
    }
}