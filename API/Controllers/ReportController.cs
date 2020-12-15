using System;
using System.IO;
using System.Threading.Tasks;
using Aspose.Cells;
using Bottom_API._Services.Interfaces;
using Bottom_API.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
        public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportController(IReportService reportService, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _reportService = reportService;
        }

        [HttpPost("exportmatrec")]
        public async Task<IActionResult> GetMaterialReceiveExcel(MaterialReceiveParam MaterialReceiveParam)
        {
            var data = await _reportService.GetMaterialReceiveExcel(MaterialReceiveParam);
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources\\Template\\MaterialReceive.xlsx");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            Worksheet ws = designer.Workbook.Worksheets[0];

            designer.SetDataSource("result", data);
            designer.Process();

            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            byte[] result = stream.ToArray();

            return File(result, "application/xlsx", "Excel" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xlsx");
        }

        [HttpPost("getMaterial")]
        public async Task<IActionResult> GetMaterialReceive(MaterialReceiveParam MaterialReceiveParam) {
            var data = await _reportService.GetMaterialReceiveExcel(MaterialReceiveParam);
            return Ok(data);
        }
    }
}