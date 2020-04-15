using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] AuthorsResourceParameters resourceParameters)
        {
            var authors = _mapper.Map<IEnumerable<AuthorDto>>(_courseLibraryRepository.GetAuthors(resourceParameters));

            return Ok(authors);
        }

        [HttpGet("{authorId}", Name = nameof(GetAuthor))]
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var author = _mapper.Map<AuthorDto>(_courseLibraryRepository.GetAuthor(authorId));

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor([FromBody] AuthorCreationDto author)
        {
            var authorEntity = _mapper.Map<Author>(author);
            _courseLibraryRepository.AddAuthor(authorEntity);
            _courseLibraryRepository.Save();

            var authorDto = _mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute(nameof(GetAuthor), new { authorId = authorDto.Id }, authorDto);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow","GET,OPTIONS,POST");
            return Ok();
        }
    }
}