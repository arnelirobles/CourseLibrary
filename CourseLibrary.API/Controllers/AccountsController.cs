using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class AccountsController: ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public AccountsController(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository ??
                throw new ArgumentNullException(nameof(accountRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{userId}",Name = nameof(GetUserAsync))]
        public  ActionResult<UserDto> GetUserAsync(string userId)
        {
            var userEntity = _accountRepository.GetUserAsync(userId).Result;
            var user = _mapper.Map<UserDto>(userEntity);
            return Ok(user);
        }

        [HttpPost(Name = nameof(AddUserAsync))]
        public async Task<IActionResult> AddUserAsync([FromBody] UserCreateDto user)
        {
           
            if (!TryValidateModel(user))
            {
                return ValidationProblem(ModelState);
            }

            var userEntity = _mapper.Map<IdentityUser>(user);

            var result = await _accountRepository.AddUserAsync(userEntity, user.Password);

            if (!TryValidateModel(userEntity))
            {
                return ValidationProblem(ModelState);
            }

            if (result.Succeeded)
            {
                var userToReturn = _mapper.Map<UserDto>(userEntity);
                return CreatedAtRoute(nameof(GetUserAsync), new { userId = userToReturn.Id }, userToReturn);
            }
            else
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code,error.Description);
                }
                return ValidationProblem(ModelState);
            }
         
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();

            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }



    }
}
