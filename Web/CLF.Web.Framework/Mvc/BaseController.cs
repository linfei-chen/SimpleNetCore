using CLF.Service.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLF.Web.Framework.Mvc
{
    public class BaseController : Controller
    {
        protected JsonResult GetServiceJsonResult<T>(Func<T> func)
        {
            var result = ServiceWrapper.Invoke(
                "CLF.Web.Mvc.Controllers.BaseController",
                "GetServiceJsonResult",
                func);

            return Json(result);
        }

        protected string GetModelStateErrorMessage()
        {
            string message = string.Empty;
            foreach (var value in ModelState.Values.Where(o => o.Errors.Any()))
            {
                if (!string.IsNullOrEmpty(value.Errors.FirstOrDefault().ErrorMessage))
                {
                    message = value.Errors.FirstOrDefault().ErrorMessage;
                    break;
                }
            }
            return message;
        }

        protected virtual JsonResult ThrowJsonMessage(bool success, string message = null)
        {
            return Json(new ServiceResult<bool>(success ? ServiceResultType.Success : ServiceResultType.Error, message));
        }
    }
}
