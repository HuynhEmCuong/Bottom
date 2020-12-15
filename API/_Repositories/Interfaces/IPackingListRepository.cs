using System.Threading.Tasks;
using Bottom_API.Models;

namespace Bottom_API._Repositories.Interfaces
{
    public interface IPackingListRepository : IBottomRepository<WMSB_Packing_List>
    {
        bool AddModel(WMSB_Packing_List model);
        Task<WMSB_Packing_List> GetByReceiveNo(object receiveNo);

        WMSB_Packing_List GetPackingList(string Purchase_No, string MO_No, string MO_Seq, string Material_ID);
    }
}