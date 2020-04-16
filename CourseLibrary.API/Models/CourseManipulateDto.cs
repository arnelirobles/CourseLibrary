using CourseLibrary.API.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    [CourseTitleMustBeDifferentFromDescription(ErrorMessage = "The Description must not be the same with Title.")]
    public abstract class CourseManipulateDto
    {
        [Required(ErrorMessage = "Kindly fill out title.")]
        [MaxLength(100, ErrorMessage = "Title must be not be more than 100 chars.")]
        public string Title { get; set; }

        [MaxLength(1500, ErrorMessage = "Description must be not be more than 1500 chars.")]
        public virtual string Description { get; set; }
    }
}