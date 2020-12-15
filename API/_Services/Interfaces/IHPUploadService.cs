using System.Threading.Tasks;
using Bottom_API.Models;

namespace Bottom_API._Services.Interfaces
{
    public interface IHPUploadService
    {
        Task<HP_Upload_Time_ie27_1_log> HPUpload();
    }
}