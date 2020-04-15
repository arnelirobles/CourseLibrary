using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));

            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courses = _mapper.Map<IEnumerable<CourseDto>>(_courseLibraryRepository.GetCourses(authorId));
            return Ok(courses);
        }

        [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (courseFromRepo == null)
            {
                return NotFound();
            }

            var course = _mapper.Map<CourseDto>(courseFromRepo);
            return Ok(course);
        }

        [HttpPost(Name = "CreateCourseForAuthor")]
        public ActionResult<CourseDto> CreateCourseForAuthor(Guid authorId, CourseCreationDto course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseEntity = _mapper.Map<Course>(course);
            _courseLibraryRepository.AddCourse(authorId, courseEntity);
            _courseLibraryRepository.Save();

            var courseDto = _mapper.Map<CourseDto>(courseEntity);

            return CreatedAtRoute("GetCourseForAuthor", 
                new { authorId = courseDto.AuthorId, courseId = courseDto.Id }, courseDto);
        }
    }
}