﻿using System.ComponentModel.DataAnnotations;
using Whip.Common.Validation;
using Whip.ViewModels.TabViewModels.EditTrack;

namespace Whip.ViewModels.Validation
{
    public class DiscNoAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int discNo;

            if (!int.TryParse(value.ToString(), out discNo))
            {
                return new ValidationResult(GetNumberRangeErrorMessage(validationContext));
            }

            if (discNo < TrackValidation.MinDiscCount || discNo > TrackValidation.MaxDiscCount)
            {
                return new ValidationResult(GetNumberRangeErrorMessage(validationContext));
            }

            var disc = validationContext.ObjectInstance as DiscViewModel;

            int discCount;

            if (int.TryParse(disc.Album.DiscCount, out discCount) && discNo > discCount)
            { 
                return new ValidationResult("Disc No must not be greater than the Disc Count");
            }

            return ValidationResult.Success;
        }

        private string GetNumberRangeErrorMessage(ValidationContext context)
        {
            return string.Format("{0} must be a number between {1} and {2}",
                context.DisplayName,
                TrackValidation.MinDiscCount,
                TrackValidation.MaxDiscCount);
        }
    }
}
