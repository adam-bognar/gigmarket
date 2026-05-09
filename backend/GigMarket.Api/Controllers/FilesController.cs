using GigMarket.Application.Features.Files.Commands.DeleteFile;
using GigMarket.Application.Features.Files.Commands.UploadGigMedia;
using GigMarket.Application.Features.Files.Commands.UploadOrderDeliveryFile;
using GigMarket.Application.Features.Files.Commands.UploadProfilePicture;
using GigMarket.Application.Features.Files.Queries.GetFileUrl;
using GigMarket.Application.Features.Files.Queries.ListGigMedia;
using GigMarket.Application.Features.Files.Queries.ListProfilePictures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GigMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(IMediator mediator) : ControllerBase
    {
        [Authorize]
        [HttpPost("upload/profile/me")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file, CancellationToken ct)
        {
            await using var stream = file.OpenReadStream();
            var result = await mediator.Send(new UploadProfilePictureCommand(
                stream,
                file.FileName,
                file.ContentType,
                file.Length), ct);

            return Ok(result);
        }

        [HttpPost("upload/gig/{gigId:guid}")]
        [RequestSizeLimit(80_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 80_000_000)]
        public async Task<IActionResult> UploadGigMedia(Guid gigId, IFormFile file, CancellationToken ct)
        {
            await using var stream = file.OpenReadStream();
            var result = await mediator.Send(new UploadGigMediaCommand(
                gigId,
                stream,
                file.FileName,
                file.ContentType,
                file.Length), ct);

            return Ok(result);
        }

        [HttpGet("url")]
        public async Task<IActionResult> GetUrl([FromQuery] string blobPath, CancellationToken ct)
        {
            var result = await mediator.Send(new GetFileUrlQuery(blobPath), ct);
            return Ok(result);
        }

        [HttpGet("list/gig/{gigId:guid}")]
        public async Task<IActionResult> ListGigMedia(Guid gigId, CancellationToken ct)
        {
            var files = await mediator.Send(new ListGigMediaQuery(gigId), ct);
            return Ok(files);
        }

        [HttpGet("list/profile/{userId:guid}")]
        public async Task<IActionResult> ListProfilePictures(Guid userId, CancellationToken ct)
        {
            var files = await mediator.Send(new ListProfilePicturesQuery(userId), ct);
            return Ok(files);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string blobPath, CancellationToken ct)
        {
            await mediator.Send(new DeleteFileCommand(blobPath), ct);
            return NoContent();
        }
        
        [HttpPost("upload/order/{orderId:guid}/delivery")]
        [Authorize]
        public async Task<IActionResult> UploadOrderDeliveryFile(Guid orderId, IFormFile file, CancellationToken ct)
        {
            await using var stream = file.OpenReadStream();
            var result = await mediator.Send(new UploadOrderDeliveryFileCommand(
                orderId,
                stream,
                file.FileName,
                file.ContentType,
                file.Length), ct);
 
            return Ok(result);
        }
    }
}