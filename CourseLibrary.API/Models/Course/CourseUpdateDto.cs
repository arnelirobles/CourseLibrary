using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    public class CourseUpdateDto : CourseManipulateDto
    {
        [Required(ErrorMessage = "Kindly fill out Description.")]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}