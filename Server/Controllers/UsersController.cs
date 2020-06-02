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
    public class UsersController : ControllerBase
    {
        private readonly UserManager userMgr;

        public UsersController(IMemoryCache _cache, IConfiguration _config)
        {
            userMgr = new UserManager(_cache, _config);
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
        public async Task<IActionResult> Get(string userName)
        {
            var result = await userMgr.GetOneByName(userName);
            return Ok(result);
        }

        [HttpGet]
        [Route("Exists")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> Exists(string userName)
        {
            var result = await userMgr.GetOneByName(userName).ConfigureAwait(false);
            return Ok(result != null);
        }

        [HttpPost("{user}")]
        [Route("Save")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Save( User user)
        {    
            user.Active = true;  
            var result = await userMgr.Save(user); 
            return Ok(result);            
        }

        [HttpGet]
        [Route("Validate")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Validate(string username, string password)
        {
            var result = await userMgr.Validate(username, password).ConfigureAwait(false);
            return Ok(result);           
        }
    }
}