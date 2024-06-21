using Microsoft.AspNetCore.Http;
using OWL.Core.CustomExceptions;
using OWL.Core.DTO;
using OWL.Core.Interfaces;
using OWL.Core.Logger;
using OWL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Services
{
    public class MoveService
    {
        private readonly IMoveRepository _moveRepo;

        public MoveService(IMoveRepository moveRepository)
        {
            this._moveRepo = moveRepository;
        }

        public List<Move> GetAllUniversalMoves()
        {
            try
            {
                List<MoveDto> moveDtos = _moveRepo.GetAllUniversalMoves();
                return Move.MapToMoves(moveDtos);
            }
            catch (Exception ex)
            {
                OwlLogger.LogError("Error getting all moves", ex);
                throw;
            }
        }

        public void AddMoveImage(Move move, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Validate file extension
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    throw new InvalidImageExtensionException("Invalid image file type.");
                }

                // Validate MIME type
                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                if (!allowedMimeTypes.Contains(imageFile.ContentType))
                {
                    throw new InvalidImageTypeException("Invalid image MIME type.");
                }

                // Generate a unique file name
                var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";

                // Set the path to store the file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Move", uniqueFileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                // Save the path in the model
                move.Image = $"/Images/Move/{uniqueFileName}";
            }
            else
            {
                move.Image = $"/Images/Move/Question.png";
            }
        }

        public void RemoveMoveImage(Move move)
        {
            // Delete associated image file if it exists
            if (!string.IsNullOrEmpty(move.Image))
            {
                if (move.Image != "/Images/Move/Question.png")
                {
                    try
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", move.Image.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    catch (ImageNotDeletedException ex)
                    {
                        Console.WriteLine($"Error deleting image file: {ex.Message}");
                    }
                }

            }
        }

        public void AddMove(Move moveToAdd, int charId)
        {

            if (string.IsNullOrEmpty(moveToAdd.Name))
            {
                throw new NameRequiredException("Move name is required.");
            }

            if (_moveRepo.CheckNameExists(new MoveDto
            {
                Name = moveToAdd.Name,
                Id = moveToAdd.Id
            }))
            {
                throw new NameExistsException("A move with this name already exists.", moveToAdd.Name);
            }          

            MoveDto moveDto = new MoveDto(moveToAdd);
            _moveRepo.AddMoveDto(moveDto,charId);
        }
    }
}
