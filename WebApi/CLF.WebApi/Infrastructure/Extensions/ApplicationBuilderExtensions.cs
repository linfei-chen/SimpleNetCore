using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLF.WebApi.Infrastructure.Extensions
{
    public static  class ApplicationBuilderExtensions
    {
        public static void UseAppSwagger(this IApplicationBuilder application)
        {
            //启用中间件服务生成Swagger作为JSON终结点
            application.UseSwagger();

            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            application.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                options.RoutePrefix = string.Empty;
            });
        }
    }
}
