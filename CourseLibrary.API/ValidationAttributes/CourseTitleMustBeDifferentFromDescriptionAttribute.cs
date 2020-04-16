using CourseLibrary.API.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class CourseTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CourseManipulateDto)validationContext.ObjectInstance;

            if (course.Title == course.Description)
            {
                return new ValidationResult(ErrorMessage,
                    new[] { nameof(CourseManipulateDto) });
            }

            return ValidationResult.Success;
        }
    }
}