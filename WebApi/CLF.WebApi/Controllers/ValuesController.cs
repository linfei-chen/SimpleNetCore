using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CLF.WebApi.Controllers
{
    //[Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// 根据id获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public string GetUserById(string userId)
        {
            return userId;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete]
        public void Delete(int id)
        {

        }
    }
}
