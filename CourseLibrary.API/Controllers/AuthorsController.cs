using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
        }

        [HttpGet(Name = nameof(GetAuthors))]
        [HttpHead]
        public IActionResult GetAuthors(
            [FromQuery] AuthorsResourceParameters resourceParameters)
        {
            try
            {
                if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(resourceParameters.OrderBy))
                {
                    return BadRequest();
                }

                var authorsEntity = _courseLibraryRepository.GetAuthors(resourceParameters);

                var previousPageLink = authorsEntity.HasPrevious ?
                    CreateAuthorsResourceUri(resourceParameters, ResourceUriType.PreviousPage) : null;

                var nextPageLink = authorsEntity.HasNext ?
                    CreateAuthorsResourceUri(resourceParameters, ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = authorsEntity.TotalCount,
                    pageSize = authorsEntity.PageSize,
                    currentPage = authorsEntity.CurrentPage,
                    totalPages = authorsEntity.Totalpages,
                    previousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

                return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsEntity).ShapeData(resourceParameters.Fields));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
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
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        [HttpDelete("{authorId}")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            _courseLibraryRepository.DeleteAuthor(authorFromRepo);
            _courseLibraryRepository.Save();

            return NoContent();
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();

            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        private string CreateAuthorsResourceUri(AuthorsResourceParameters resourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link(nameof(GetAuthors),
                        new
                        {
                            fields = resourceParameters.Fields,
                            orderBy = resourceParameters.OrderBy,
                            pageNumber = resourceParameters.PageNumber - 1,
                            pageSize = resourceParameters.PageSize,
                            mainCategory = resourceParameters.MainCategory,
                            searchQuery = resourceParameters.SearchQuery
                        });

                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetAuthors),
                        new
                        {
                            fields = resourceParameters.Fields,
                            orderBy = resourceParameters.OrderBy,
                            pageNumber = resourceParameters.PageNumber + 1,
                            pageSize = resourceParameters.PageSize,
                            mainCategory = resourceParameters.MainCategory,
                            searchQuery = resourceParameters.SearchQuery
                        });

                default:
                    return Url.Link(nameof(GetAuthors),
                        new
                        {
                            fields = resourceParameters.Fields,
                            orderBy = resourceParameters.OrderBy,
                            pageNumber = resourceParameters.PageNumber,
                            pageSize = resourceParameters.PageSize,
                            mainCategory = resourceParameters.MainCategory,
                            searchQuery = resourceParameters.SearchQuery
                        });
            }
        }
    }
}