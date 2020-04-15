﻿using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    public class CourseCreationDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(1500)]
        public string Description { get; set; }
    }
}