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


        //Auction
        //GET /api/auction?searchTerm=&isOpen=
        [HttpGet("Search")]
        [AllowAnonymous]
        [EndpointSummary("Search auctions by title and open/closed status")]
        public async Task<IActionResult> Search([FromQuery] string? searchTerm, [FromQuery] bool? isOpen)
        {
            var auctions = await _auctionService.SearchAsync(searchTerm, isOpen);
            return Ok(auctions);
        }


        [HttpGet("MyAuctions")]
        [Authorize]
        public async Task<IActionResult> GetMyAuctions()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var result = await _auctionService.GetAuctionsByUserIdAsync(userId.Value);
            return Ok(result);
        }



        //GET /api/auction/{id}?includeHistory=true
        [HttpGet("GetById/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id, [FromQuery] bool includeHistory = true)
        {
            var auction = await _auctionService.GetByIdAsync(id, includeHistory);
            if (auction == null) return NotFound("Auction not found or disabled.");
            return Ok(auction);
        }


        //POST /api/auction
        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AuctionCreateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _auctionService.CreateAsync(userId.Value, dto);
            return success ? Ok(new { message = "Auction created" }) : BadRequest("Invalid data or dates.");
        }


        //PUT /api/auction/{id}
        [HttpPut("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] AuctionUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var success = await _auctionService.UpdateAsync(userId.Value, id, dto);
                return Ok(new { message = "Update successful" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred while updating the auction.");
            }
        }


        //Bid
        // POST /api/auction/{id}/bid
        [HttpPost("PlaceBid/{id}")]
        [Authorize]
        [EndpointSummary("Place a bid on an auction (must be higher than current highest)")]
        public async Task<IActionResult> PlaceBid(int id, [FromBody] BidCreateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var success = await _auctionService.PlaceBidAsync(userId.Value, id, dto);
                return Ok(new { message = "Bid placed successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        //DELETE /api/auction/{id}/bid
        [HttpDelete("CancelBid/{id}")]
        [Authorize]
        [EndpointSummary("Cancel the latest bid of current user if auction is open")]
        public async Task<IActionResult> CancelBid(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _auctionService.CancelLastBidAsync(userId.Value, id);
            return success ? NoContent() : BadRequest("Cannot cancel this bid.");
        }


        //Admin
        //PATCH /api/auction/{id}/disable
        [HttpPatch("Disable/{id}")]
        [Authorize(Roles = "Admin")]
        [Tags("Admin Operations")]
        [EndpointSummary("Disable an auction")]
        public async Task<IActionResult> Disable(int id)
        {
            var success = await _auctionService.DisableAuctionAsync(id);
            return success ? Ok(new { message = "Auction disabled by admin" }) : NotFound();
        }

        // GET: /api/auction/admin?searchTerm=xxx
        [HttpGet("GetAllAuctions")]
        [Authorize(Roles = "Admin")]
        [Tags("Admin Operations")]
        [EndpointSummary("Admin: get all auctions")]
        public async Task<IActionResult> GetAllAuctions([FromQuery] string? searchTerm)
        {
            
            var auctions = await _auctionService.SearchAsync(searchTerm, isOpen: null);
            return Ok(auctions);
        }


        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return (idClaim != null && int.TryParse(idClaim.Value, out int id)) ? id : null;
        }
    }
}
