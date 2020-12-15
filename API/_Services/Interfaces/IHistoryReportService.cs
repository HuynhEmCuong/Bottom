using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO;
using Bottom_API.Helpers;

namespace Bottom_API._Services.Interfaces
{
    public interface IHistoryReportService
    {
        Task<List<HistoryReportInputDB>> HistoryReportInputExcel(HistoryReportParam param);
        Task<List<HistoryReportOutputDB>> HistoryReportOutputExcel(HistoryReportParam param);
    }
}