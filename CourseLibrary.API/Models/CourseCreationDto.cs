using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    public class CourseCreationDto : IValidatableObject
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(1500)]
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == Description)
            {
                yield return new ValidationResult("The Description must not be the same with Title.",
                    new[] { nameof(CourseCreationDto) });
            }
        }
    }
}