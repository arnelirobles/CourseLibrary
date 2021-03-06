﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    public class AuthorCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTimeOffset DateOfBirth { get; set; }

        [Required]
        [MaxLength(50)]
        public string MainCategory { get; set; }

        public ICollection<CourseCreationDto> Courses { get; set; }
            = new List<CourseCreationDto>();
    }
}