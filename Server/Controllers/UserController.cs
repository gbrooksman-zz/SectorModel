using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SectorModel.Server.Managers;
using SectorModel.Shared.Entities;

namespace SectorModel.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager userMgr;
        private readonly AppSettings appSettings;

        public UserController(IMemoryCache _cache, IConfiguration _config, AppSettings _appSettings)
        {
            appSettings = _appSettings;

            userMgr = new UserManager(_cache, _config, appSettings);
        }

        [HttpGet]
        [Route("GetByID")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult>GetByID(Guid id)
        {
            var result = await userMgr.GetOneById(id).ConfigureAwait(false);
            return Ok(result);

        }

        [HttpGet]
        [Route("Get")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string email)
        {
            var user = await userMgr.GetOneByEmail(email);
            return Ok(user);
        }

        [HttpGet]
        [Route("Exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Exists(string email)
        {
            var result = await userMgr.GetOneByEmail(email).ConfigureAwait(false);
            return Ok(result != null);
        }

        [HttpPost()]
        [Route("Save")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Save( User user)
        {    
            user.IsActive = true;  
            var result = await userMgr.Save(user); 
            return Ok(result);            
        }

        [HttpPost]
        [Route("Validate")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Validate(User user)
        {
            var result = await userMgr.Validate(user);
			if (result)
			{
            	return Ok(true);   
			} 
			else
			{
				return Unauthorized(false);
			}       
        }              
    }
}