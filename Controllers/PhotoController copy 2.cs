// using FluentValidation;
// using Microsoft.AspNetCore.Mvc;
// using PhotoAlbumApi.Models;
// using PhotoAlbumApi.Validators;
// using PhotoAlbumApi.DTOs;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Caching.Memory;
// using AutoMapper;
// using PhotoAlbumApi.Services;
// using Microsoft.AspNetCore.Authorization;
// using System.Security.Claims;

// namespace PhotoAlbumApi.Controllers
// {
//     [ApiController]
//     [Route("api/photos")]
//     [Authorize]
//     public class PhotoController : ControllerBase
//     {
//         private readonly IPhotoAlbumService _service;
//         private readonly IMapper _mapper;
//         private readonly LoggingService _loggingService;
//         private readonly IMemoryCache _cache;

//         public PhotoController(IPhotoAlbumService service, IMapper mapper, LoggingService loggingService, IMemoryCache cache)
//         {
//             _service = service;
//             _mapper = mapper;
//             _loggingService = loggingService;
//             _cache = cache;
//         }

//         private int GetUserId()
//         {
//             var userIdString = User.FindFirstValue("UserId");
//             if (string.IsNullOrEmpty(userIdString))
//             {
//                 _loggingService.LogError("User ID claim is missing.");
//                 throw new UnauthorizedAccessException("Invalid user ID. User is not signed in or using an invalid ID.");
//             }

//             if (int.TryParse(userIdString, out int userId))
//             {
//                 return userId;
//             }

//             _loggingService.LogError($"Failed to parse user ID: {userIdString}");
//             throw new UnauthorizedAccessException("Invalid user ID. User is not signed in or using an invalid ID.");
//         }

//         [HttpGet]
//         public async Task<IActionResult> GetPhotos()
//         {
//             try
//             {
//                 var userId = GetUserId();
//                 _loggingService.LogInformation($"Fetching all photos for user {userId}");

//                 var cacheKey = $"GetPhotos_{userId}";
//                 if (!_cache.TryGetValue(cacheKey, out IEnumerable<PhotoDisplayDto> photoDtos))
//                 {
//                     var photos = await _service.GetPhotosAsync(userId);
//                     if (photos == null)
//                     {
//                         _loggingService.LogWarning("No photos found");
//                         return StatusCode(StatusCodes.Status404NotFound);
//                     }
//                     photoDtos = _mapper.Map<IEnumerable<PhotoDisplayDto>>(photos);

//                     var cacheEntryOptions = new MemoryCacheEntryOptions()
//                         .SetSlidingExpiration(TimeSpan.FromMinutes(5));

//                     _cache.Set(cacheKey, photoDtos, cacheEntryOptions);
//                 }

//                 _loggingService.LogInformation($"Successfully fetched all photos for user {userId}");
//                 return StatusCode(StatusCodes.Status200OK, photoDtos);
//             }
//             catch (UnauthorizedAccessException ex)
//             {
//                 _loggingService.LogError(ex.Message);
//                 return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
//             }
//         }

//         [HttpGet("{id}")]
//         public async Task<ActionResult> GetPhoto(int id)
//         {
//             try
//             {
//                 var userId = GetUserId();
//                 _loggingService.LogInformation($"Fetching photo with ID: {id} for user {userId}");
//                 var cacheKey = $"GetPhoto_{userId}_{id}";
//                 if (!_cache.TryGetValue(cacheKey, out PhotoDisplayDto photoDto))
//                 {
//                     var photo = await _service.GetPhotoAsync(id, userId);
//                     if (photo == null)
//                     {
//                         _loggingService.LogWarning($"Photo with ID: {id} not found for user {userId}");
//                         return StatusCode(StatusCodes.Status404NotFound);
//                     }
//                     photoDto = _mapper.Map<PhotoDisplayDto>(photo);

//                     var cacheEntryOptions = new MemoryCacheEntryOptions()
//                         .SetSlidingExpiration(TimeSpan.FromMinutes(5));

//                     _cache.Set(cacheKey, photoDto, cacheEntryOptions);
//                     _loggingService.LogInformation($"Successfully fetched photo with ID: {id} for user {userId}");
//                 }
//                 return StatusCode(StatusCodes.Status200OK, photoDto);
//             }
//             catch (UnauthorizedAccessException ex)
//             {
//                 _loggingService.LogError(ex.Message);
//                 return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
//             }
//         }

//         [HttpPost]
//         public async Task<ActionResult> AddPhoto(PhotoDto photoDto)
//         {
//             try
//             {
//                 var userId = GetUserId();
//                 _loggingService.LogInformation($"Adding a new photo for user {userId}");

//                 var photo = _mapper.Map<Photo>(photoDto);
//                 photo.UserId = userId;

//                 var addedPhoto = await _service.AddPhotoAsync(photo);
//                 _loggingService.LogInformation($"Successfully added a new photo with ID: {addedPhoto.Id} for user {userId}");

//                 var photoDisplayDto = _mapper.Map<PhotoDisplayDto>(addedPhoto);
//                 return StatusCode(StatusCodes.Status201Created, photoDisplayDto);
//             }
//             catch (InvalidOperationException ex)
//             {
//                 _loggingService.LogError(ex, "Error adding photo");
//                 return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message });
//             }
//             catch (UnauthorizedAccessException ex)
//             {
//                 _loggingService.LogError(ex.Message);
//                 return StatusCode(StatusCodes.Status401Unauthorized, new { message = ex.Message });
//             }
//             catch (Exception ex)
//             {
//                 _loggingService.LogError(ex, "An unexpected error occurred while adding the photo");
//                 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred. Please try again later." });
//             }
//         }

//         [HttpPut("{id}")]
//         public async Task<IActionResult> UpdatePhoto(int id, PhotoDto photoDto)
//         {
//             try
//             {
//                 var userId = GetUserId();
//                 _loggingService.LogInformation($"Updating photo with ID: {id} for user {userId}");

//                 if (photoDto == null)
//                 {
//                     _loggingService.LogWarning("Photo data is missing");
//                     return BadRequest(new { message = "Photo data is missing" });
//                 }

//                 // Retrieve the existing photo from the service/repository
//                 var existingPhoto = await _service.GetPhotoAsync(userId, id);
//                 if (existingPhoto == null)
//                 {
//                     _loggingService.LogWarning($"Photo with ID: {id} not found for user {userId}");
//                     return NotFound(new { message = "Photo not found", photoId = id });
//                 }

//                 // Assign updated properties to the existing photo object
//                 existingPhoto.AlbumId = photoDto.AlbumId;
//                 existingPhoto.Title = photoDto.Title;
//                 existingPhoto.Description = photoDto.Description;
//                 existingPhoto.Url = photoDto.Url;

//                 // Pass the updated photo object to the repository for processing
//                 var updatedPhoto = await _service.UpdatePhotoAsync(existingPhoto);

//                 // Check if the update was successful
//                 if (updatedPhoto == null)
//                 {
//                     _loggingService.LogError($"Failed to update photo with ID: {id} for user {userId}");
//                     return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the photo" });
//                 }

//                 // Invalidate the cache if necessary
//                 _cache.Remove($"GetPhoto_{userId}_{id}");
//                 _loggingService.LogInformation($"Cache invalidated for photo ID: {id} for user {userId}");

//                 // Return the updated photo data
//                 var photoDisplayDto = _mapper.Map<PhotoDisplayDto>(updatedPhoto);
//                 return Ok(photoDisplayDto);
//             }
//             catch (UnauthorizedAccessException ex)
//             {
//                 _loggingService.LogError(ex.Message);
//                 return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
//             }
//         }

//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeletePhoto(int id)
//         {
//             try
//             {
//                 var userId = GetUserId();
//                 _loggingService.LogInformation($"Attempting to delete photo with ID: {id} for user {userId}");

//                 var existingPhoto = await _service.GetPhotoAsync(id, userId);
//                 if (existingPhoto == null)
//                 {
//                     _loggingService.LogWarning($"Photo with ID: {id} not found for user {userId}");
//                     return NotFound(new { message = "Photo not found", photoId = id });
//                 }

//                 await _service.DeletePhotoAsync(id, userId);
//                 _loggingService.LogInformation($"Successfully deleted photo with ID: {id} for user {userId}");

//                 // Invalidate the cache if necessary
//                 _cache.Remove($"GetPhoto_{userId}_{id}");
//                 _loggingService.LogInformation($"Cache invalidated for photo ID: {id} for user {userId}");

//                 return NoContent();
//             }
//             catch (UnauthorizedAccessException ex)
//             {
//                 _loggingService.LogError(ex.Message);
//                 return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
//             }
//         }

//         [HttpPost("{id}/undo-delete")]
//         public async Task<IActionResult> UndoDeletePhoto(int id)
//         {
//             try
//             {
//                 var userId = GetUserId();
//                 _loggingService.LogInformation($"Attempting to undo delete for photo with ID: {id} for user {userId}");

//                 var photo = await _service.UndoDeletePhotoAsync(id, userId);
//                 if (photo != null)
//                 {
//                     _loggingService.LogInformation($"Successfully restored photo with ID: {id} for user {userId}");

//                     _cache.Remove($"GetPhoto_{userId}_{id}");
//                     _loggingService.LogInformation($"Cache invalidated for photo ID: {id} for user {userId}");

//                     var photoDisplayDto = _mapper.Map<PhotoDisplayDto>(photo);
//                     return StatusCode(StatusCodes.Status200OK, photoDisplayDto);
//                 }
//                 else
//                 {
//                     _loggingService.LogWarning($"Photo with ID: {id} not found or not deleted for user {userId}");
//                     return StatusCode(StatusCodes.Status404NotFound, new { message = "Photo not found or not deleted", photoId = id });
//                 }
//             }
//             catch (UnauthorizedAccessException ex)
//             {
//                 _loggingService.LogError(ex.Message);
//                 return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
//             }
//         }

//     }
// }