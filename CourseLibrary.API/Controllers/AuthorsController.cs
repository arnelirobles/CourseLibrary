using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(CourseLibraryRepository));
        }
        public IActionResult GetAuthors()
        {
            var authors = _courseLibraryRepository.GetAuthors();
            return new JsonResult(authors);
        }
    }
}
