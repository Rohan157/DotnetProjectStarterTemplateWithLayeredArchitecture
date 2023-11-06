using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.BusinessLogic.Services.Interface;
using Server.Domain.DTOs;
using Server.Domain.Exceptions;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IDataProcessorService _dataProcessorService;
        private readonly ILogger<HotelController> _logger;
        public HotelController(IDataProcessorService dataProcessorService, ILogger<HotelController> logger)
        {
            _dataProcessorService = dataProcessorService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("CheckStatus")]
        public async Task<IActionResult> CheckStatusAsync(string hotelId)
        {
            if (string.IsNullOrEmpty(hotelId))
            {
                return BadRequest(new ResponseModel("HotelId is required.",false));
            }
            try
            {
                var response = await _dataProcessorService.GetCertificationStatusAsync(hotelId);
                return StatusCode(200, new ResponseModel("Success", response));

            }catch (CustomException ex)
            {
                return StatusCode(400, new ResponseModel(ex.Message, false));
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckStatusAsync: An unexpected error occurred.");
                return StatusCode(500, new ResponseModel("An unexpected error occurred.", false));
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("excel")]
        public async Task<IActionResult> AddFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ResponseModel("File is required.", false));
            }
            try
            {
                var response = await _dataProcessorService.SaveFileAsync(file);
                return StatusCode(200, new ResponseModel("Success", response));
            }
            catch (CustomException ex)
            {
                return StatusCode(400, new ResponseModel(ex.Message, false));
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddFileAsync: An unexpected error occurred.");
                return StatusCode(500, new ResponseModel("An unexpected error occurred.", false));
            }
        }
    }
}
