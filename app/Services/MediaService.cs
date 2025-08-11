using SparkCheck.Data;
using SparkCheck.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Forms;

namespace SparkCheck.Services
{
    public class MediaService
    {
        private readonly AppDbContext _context;

        private const long MaxFileSize = 1024 * 1024 * 100; // 100 MB
        private const int MaxPhotoCount = 1; // Maximum number of photos allowed

        public MediaService(AppDbContext context, UserSessionService sessionService)
        {
            _context = context;
        }


        // Uploads or updates the profile photo for the current user
        public async Task UploadOrUpdateProfilePhotoAsync(IBrowserFile file, int intUserID)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            // Read the file into byte[]
            byte[] imageData;
            using (var ms = new MemoryStream())
            {
                await file.OpenReadStream(maxAllowedSize: MaxFileSize).CopyToAsync(ms);
                imageData = ms.ToArray();
            }

            int userId = intUserID;

            // Check if a profile photo already exists
            var existingProfileMedia = await _context.TUserMedia
                .Where(m => m.intUserID == userId && m.blnOnProfile == true)
                .FirstOrDefaultAsync();

            if (existingProfileMedia != null)
            {
                // Update existing photo
                existingProfileMedia.Photo = imageData;
                existingProfileMedia.dtmUploadDate = DateTime.Now;
                Console.WriteLine("[MEDIA] Updated existing profile photo.");
            }
            else
            {
                // Insert new photo record
                var newMedia = new TUserMedia
                {
                    intUserID = userId,
                    Photo = imageData,
                    blnOnProfile = true,
                    blnIsFace = true,
                    blnIsActive = true,
                    dtmUploadDate = DateTime.Now
                };

                _context.TUserMedia.Add(newMedia);
                Console.WriteLine("[MEDIA] Uploaded new profile photo.");
            }

            await _context.SaveChangesAsync();
        }

        // Load all media for a given user
        public async Task<List<TUserMedia>> GetUserMediaAsync(int userId)
        {
            try
            {
                var media = await _context.TUserMedia
                    .Where(m => m.intUserID == userId)
                    .AsNoTracking()
                    .ToListAsync();

                Console.WriteLine("[DEBUG] UserMediaFiles loaded, count: {0}", media.Count);
                return media;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MEDIA LOAD ERROR] {ex.Message}");
                return new List<TUserMedia>(); // Return empty list on failure
            }
        }

        public async Task<bool> UploadPhotoAsync(int userId, IBrowserFile file)
        {
            try
            {
                // Enforce 4-photo limit
                var currentPhotoCount = await _context.TUserMedia
                    .CountAsync(m => m.intUserID == userId && m.blnIsActive);

                if (currentPhotoCount >= MaxPhotoCount)
                {
                    Console.WriteLine("[UPLOAD ERROR] Photo limit reached.");
                    return false; // Or throw exception if you want front-end to display message
                }

                using var stream = new MemoryStream();
                await file.OpenReadStream(maxAllowedSize: 10_000_000).CopyToAsync(stream);
                var imageBytes = stream.ToArray();

                var newPhoto = new TUserMedia
                {
                    intUserID = userId,
                    Photo = imageBytes,
                    blnOnProfile = false,
                    blnIsFace = false,
                    blnIsActive = true,
                    dtmUploadDate = DateTime.Now
                };

                _context.TUserMedia.Add(newPhoto);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UPLOAD ERROR] {ex.Message}");
                return false;
            }
        }

    }
}