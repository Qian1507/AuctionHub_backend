using AuctionHub_backend.Core.Interfaces;
using AuctionHub_backend.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuctionHub_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        public AuctionController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string? searchTerm, [FromQuery] bool? isOpen)
        {
            var auctions = await _auctionService.SearchAsync(searchTerm, isOpen);
            return Ok(auctions);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id, [FromQuery] bool includeHistory = true)
        {
            var auction = await _auctionService.GetByIdAsync(id, includeHistory);
            if (auction == null) return NotFound("Auction not found or disabled.");
            return Ok(auction);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AuctionCreateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _auctionService.CreateAsync(userId.Value, dto);
            return success ? Ok(new { message = "Auction created" }) : BadRequest("Invalid data or dates.");
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] AuctionUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _auctionService.UpdateAsync(userId.Value, id, dto);
            return success 
                ? Ok(new { message = "Update successful" })
                : BadRequest("Update failed (unauthorized or auction is closed).");
        }

      
        [HttpPost("{id}/bid")]
        [Authorize]
        public async Task<IActionResult> PlaceBid(int id, [FromBody] BidCreateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _auctionService.PlaceBidAsync(userId.Value, id, dto);
            return success ? Ok(new { message = "Bid placed" }) : BadRequest("Bid too low, auction expired, or you are the creator.");
        }

        [HttpDelete("{id}/bid")]
        [Authorize]
        public async Task<IActionResult> CancelBid(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _auctionService.CancelLastBidAsync(userId.Value, id);
            return success ? NoContent() : BadRequest("Cannot cancel this bid.");
        }

        [HttpPatch("{id}/disable")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Disable(int id)
        {
            var success = await _auctionService.DisableAuctionAsync(id);
            return success ? Ok(new { message = "Auction disabled by admin" }) : NotFound();
        }

        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return (idClaim != null && int.TryParse(idClaim.Value, out int id)) ? id : null;
        }
    }
}
