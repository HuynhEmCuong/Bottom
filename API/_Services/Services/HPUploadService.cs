
using System.Linq;
using System.Threading.Tasks;
using Bottom_API._Repositories.Interfaces.DbHpBasic;
using Bottom_API._Services.Interfaces;
using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class HPUploadService : IHPUploadService
    {
        private readonly IHPUploadTimeLogRepository _repo;
        public HPUploadService(IHPUploadTimeLogRepository repo) {
                _repo = repo;
        }

        public async Task<HP_Upload_Time_ie27_1_log> HPUpload()
        {
            var data = await _repo.FindAll().OrderByDescending(x => x.Update_Time).FirstOrDefaultAsync();
            return data;
        }
    }
}