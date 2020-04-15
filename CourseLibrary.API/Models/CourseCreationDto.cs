using CourseLibrary.API.ValidationAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    [CourseTitleMustBeDifferentFromDescription(ErrorMessage = "The Description must not be the same with Title.")]
    public class CourseCreationDto //: IValidatableObject
    {
        [Required(ErrorMessage = "Kindly fill out title.")]
        [MaxLength(100, ErrorMessage = "Title must be not be more than 100 chars.")]
        public string Title { get; set; }

        [MaxLength(1500, ErrorMessage = "Description must be not be more than 1500 chars.")]
        public string Description { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title == Description)
        //    {
        //        yield return new ValidationResult("The Description must not be the same with Title.",
        //            new[] { nameof(CourseCreationDto) });
        //    }
        //}
    }
}