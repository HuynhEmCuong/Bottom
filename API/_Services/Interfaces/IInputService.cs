using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO;
using Bottom_API.Helpers;
using Bottom_API.Models;
using Bottom_API.ViewModel;

namespace Bottom_API._Services.Interfaces
{
    public interface IInputService 
    {
        Task<Transaction_Dto> GetByQRCodeID(object qrCodeID);
        Task<Transaction_Detail_Dto> GetDetailByQRCodeID(object qrCodeID);
        Task<bool> CreateInput(Transaction_Detail_Dto model, string updateBy);
        Task<bool> SubmitInput(InputSubmitModel data, string updateBy);
        Task<MissingPrint_Dto> GetMaterialPrint(string missingNo);
        Task<PagedList<QrCodeAgain_Dto>> FilterQrCodeAgain(PaginationParams param, FilterQrCodeAgainParam filterParam);
        Task<PagedList<Transaction_Main_Dto>> FilterMissingPrint(PaginationParams param, FilterMissingParam filterParam);
        Task<string> FindMaterialName(string materialID);
        Task<string> FindMissingByQrCode(string qrCodeID);
        Task<bool> CheckQrCodeInV696(string qrCodeID);
        Task<bool> CheckRackLocation(string rackLocation);
        Task<PagedList<IntegrationInputModel>> SearchIntegrationInput(PaginationParams param,FilterPackingListParam filterparam);
        Task<bool> IntegrationInputSubmit(List<IntegrationInputModel> data, string user);
        Task<WMSB_Transaction_Main> FindQrCodeInput(string qrCodeId);
        Task<string> CheckEnterRackInputIntergration(string racklocation, string receiveNo);
    }
}