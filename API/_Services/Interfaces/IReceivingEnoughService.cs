using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.Helpers;
using Bottom_API.ViewModel;

namespace Bottom_API._Services.Interfaces
{
    public interface IRecevingEnoughService
    {
        Task<PagedList<ReceivingMaterialMainModel>> SearchByModel(PaginationParams param,FilterReceivingMateParam filterParam);
        Task<List<ReceiveAfterSubmit>> SubmitData(List<ReceivingMaterialMainModel> data, string updateBy);
    }
}