using DigitalResourcesStore.Models.VoucherDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalResourcesStore.Controllers
{
    [ApiController]
    [Route("voucher")]
        public class VoucherController : Controller
        {
            private readonly IVoucherService _voucherService;

            public VoucherController(IVoucherService voucherService)
            {
                _voucherService = voucherService;
            }

            [HttpGet("{id}")]
            public async Task<ActionResult<VoucherDtos>> GetById(int id)
            {
                var voucher = await _voucherService.GetVoucherById(id);
                if (voucher == null) return NotFound();
                return Ok(voucher);
            }

            [HttpGet]
            public async Task<ActionResult<List<VoucherDtos>>> Get()
            {
                var vouchers = await _voucherService.GetAllVouchers();
                return Ok(vouchers);
            }

            [HttpPost]
            public async Task<ActionResult<bool>> Create([FromBody] CreatedVoucherDtos request)
            {
                var result = await _voucherService.CreateVoucher(request);
                if (!result) return StatusCode(500, "Failed to create voucher.");
                return Ok(result);
            }

            [HttpPut("{id}")]
            public async Task<ActionResult<bool>> Update(int id, [FromBody] UpdateVoucherDtos request)
            {
                var result = await _voucherService.UpdateVoucher(id, request);
                if (!result) return NotFound($"Voucher with ID {id} not found.");
                return Ok(result);
            }

            [HttpDelete("{id}")]
            public async Task<ActionResult<bool>> Delete(int id)
            {
                var result = await _voucherService.DeleteVoucher(id, "admin");
                if (!result) return NotFound($"Voucher with ID {id} not found.");
                return Ok(result);
            }
        }
}
