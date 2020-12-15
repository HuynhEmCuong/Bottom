using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Bottom_API._Services.Interfaces;
using Bottom_API.DTO;
using Bottom_API.Helpers;
using Bottom_API.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Bottom_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InputController : ControllerBase
    {
        
        private readonly IInputService _service;
        public InputController(IInputService service)
        {
            _service = service;
        }

        [HttpGet("{qrCodeID}", Name="GetByQrCodeID")]
        public async Task<IActionResult> GetByQrCodeID(string qrCodeID)
        {
            var model =  await _service.GetByQRCodeID(qrCodeID);
            if(model.QrCode_Id != null)
                return Ok(model);
            else return NoContent();
        }

        [HttpGet("detail/{qrCode}", Name="GetDetail")]
        public async Task<IActionResult> GetDetailByQrCodeID(string qrCode)
        {
            var model = await _service.GetDetailByQRCodeID(qrCode);
            if (model != null)
                return Ok(model);
            else return NoContent();
        }

        [HttpPost("create", Name = "CreateInput")]
        public async Task<IActionResult> CreateInput(Transaction_Detail_Dto model)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            if (await _service.CreateInput(model, updateBy))
            {
                return Ok();
            }

            throw new Exception("Creating the rack location failed on save");
        }

        [HttpPost("submit", Name = "SubmitInput")]
        public async Task<IActionResult> SubmitInput([FromBody]InputSubmitModel data)
        {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            if (await _service.SubmitInput(data, updateBy))
            {
                return Ok();
            }

            throw new Exception("Submit failed on save");
        }

        [HttpGet("printmissing/{missingNo}", Name="PrintMissing")]
        public async Task<IActionResult> PrintMissing(string missingNo)
        {
            var model = await _service.GetMaterialPrint(missingNo);
            if (model != null)
                return Ok(model);
            else return NoContent();
        }

        [HttpPost("filterQrCodeAgain")]
        public async Task<IActionResult> FilterQrCodeAgain([FromQuery]PaginationParams param, FilterQrCodeAgainParam filterParam) 
        {
            var data = await _service.FilterQrCodeAgain(param,filterParam);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }
        
        [HttpPost("filterMissingPrint")]
        public async Task<IActionResult> FilterMissingPrint([FromQuery]PaginationParams param, FilterMissingParam filterParam) {
            var data = await _service.FilterMissingPrint(param,filterParam);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }

        [HttpGet("findMaterialName/{materialID}")]
        public async Task<IActionResult> FindMaterialName (string materialID) 
        {
            var materialName = await _service.FindMaterialName(materialID);
            return Ok(new {materialName = materialName});
        }

        [HttpGet("findMiss/{qrCodeId}")]
        public async Task<IActionResult> FindMissingByQrCode (string qrCodeId) {
            var missingNo = await _service.FindMissingByQrCode(qrCodeId);
            return Ok(new {missingNo = missingNo});
        }


        [HttpGet("checkQrCodeInV696/{qrCodeId}")]
        public async Task<IActionResult> CheckQrCodeInV696(string qrCodeId) {
            var data = await _service.CheckQrCodeInV696(qrCodeId);
            return Ok(new {result = data});
        }
        [HttpGet("checkRacLocation/{rackLocation}")]
        public async Task<IActionResult> CheckRacklocation(string rackLocation) {
            var data = await _service.CheckRackLocation(rackLocation);
            return Ok(new {result = data});
        }

        [HttpPost("integrations")]
        public async Task<IActionResult> SearchIntegrationInput([FromQuery]PaginationParams param,FilterPackingListParam filterparam) {
            var result = await _service.SearchIntegrationInput(param, filterparam);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpPost("intergationSubmit")]
        public async Task<IActionResult> IntergationSubmit([FromBody] List<IntegrationInputModel> data) {
            var updateBy = User.FindFirst(ClaimTypes.Name).Value;
            var result = await _service.IntegrationInputSubmit(data, updateBy);
            return Ok(new {result = result});
        }

        [HttpGet("findQrCodeInput/{qrCodeId}")]
        public async Task<IActionResult> FindQrCodeInput(string qrCodeId) {
            var data = await _service.FindQrCodeInput(qrCodeId);
            return Ok(data);
        }   

        [HttpGet("checkEnterRackInputIntergration")]
        public async Task<IActionResult> CheckEnterRackInputIntergration(string racklocation, string receiveNo) {
            var data = await _service.CheckEnterRackInputIntergration(racklocation, receiveNo);
            return Ok(new {result = data});
        }
    }
}