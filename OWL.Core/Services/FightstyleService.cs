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
    public class FightstyleService
    {

        private readonly IFightstyleRepository _fightstyleRepo;

        public FightstyleService(IFightstyleRepository fightstyleRepository)
        {
            this._fightstyleRepo = fightstyleRepository;
        }

        public Fightstyle GetFightstyleById(int styleId)
        {
            if (string.IsNullOrEmpty(styleId.ToString()))
            {
                throw new IdNotFoundException("Fightstyle ID has not been found", styleId);
            }
            else
            {
                FightstyleDto styleDto = _fightstyleRepo.GetFightstyleDtoById(styleId);
                return new Fightstyle(styleDto);
            }
        }

        public List<Fightstyle> GetAllFightstyles()
        {
            try
            {
                List<FightstyleDto> styleDtos = _fightstyleRepo.GetAllFightstyles();
                return Fightstyle.MapToFightstyles(styleDtos);
            }
            catch (Exception ex)
            {
                OwlLogger.LogError("Error getting all fight styles", ex);
                throw;
            }
        }

        public void AddFightstyle(Fightstyle fightstyle)
        {
            try
            {
                if (string.IsNullOrEmpty(fightstyle.Name))
                {
                    throw new ArgumentException("Fightstyle name cannot be null or empty");
                }

                FightstyleDto styleDto = new FightstyleDto(fightstyle);
                _fightstyleRepo.AddFightstyleDto(styleDto);
            }
            catch (Exception ex)
            {
                OwlLogger.LogError($"Error adding fightstyle {fightstyle.Name}", ex);
                throw;
            }
        }
    }
}
