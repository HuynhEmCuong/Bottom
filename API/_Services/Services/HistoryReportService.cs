using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.Data;
using Bottom_API.DTO;
using Bottom_API.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API._Services.Services
{
    public class HistoryReportService : IHistoryReportService
    {
        private readonly DataContext _context;

        public HistoryReportService(DataContext context) {
            _context = context;
        }
        public async Task<List<HistoryReportInputDB>> HistoryReportInputExcel(HistoryReportParam param)
        {
            var data = new List<HistoryReportInputDB>();
                data = await (_context.HistoryReportInputDB.FromSqlRaw("EXEC [dbo].[NSP_TRANSFER_HISTORY] @Transac_Type, @Date_S, @Date_E, @PO, @T2_Supplier_ID ",
                new SqlParameter("Transac_Type", "I"),
                new SqlParameter("Date_S", param.DateStart != "" ? param.DateStart :  (object)DBNull.Value),
                new SqlParameter("Date_E", param.DateEnd != "" ? param.DateEnd :  (object)DBNull.Value),
                new SqlParameter("PO", param.PO != "" ? param.PO : (object)DBNull.Value),
                new SqlParameter("T2_Supplier_ID", param.T2_Supplier_ID != "" ? param.T2_Supplier_ID.Trim() :(object)DBNull.Value)
                    )).ToListAsync();
            return data;
        }
        public async Task<List<HistoryReportOutputDB>> HistoryReportOutputExcel(HistoryReportParam param)
        {
            var data = new List<HistoryReportOutputDB>();
                data = await (_context.HistoryReportOutputDB.FromSqlRaw("EXEC [dbo].[NSP_TRANSFER_HISTORY_O] @Transac_Type, @Date_S, @Date_E, @PO, @T2_Supplier_ID ",
                new SqlParameter("Transac_Type", "O"),
                new SqlParameter("Date_S", param.DateStart != "" ? param.DateStart :  (object)DBNull.Value),
                new SqlParameter("Date_E", param.DateEnd != "" ? param.DateEnd :  (object)DBNull.Value),
                new SqlParameter("PO", param.PO != "" ? param.PO : (object)DBNull.Value),
                new SqlParameter("T2_Supplier_ID", param.T2_Supplier_ID != "" ? param.T2_Supplier_ID.Trim() :(object)DBNull.Value)
                    )).ToListAsync();
            return data;
        }
    }
}