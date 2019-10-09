using CLF.Service.DTO.Account;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Xunit;
using Newtonsoft.Json;
using System;

namespace CLF.Test
{
    public   class AccountControllerTest
    {
        private readonly HttpClient _client;

        public AccountControllerTest()
        {
            var builder = new WebHostBuilder()
               .ConfigureAppConfiguration(config =>
               {
                   config.SetBasePath(Directory.GetCurrentDirectory());
                   config.AddJsonFile("appsettings.json");
               })
               .UseStartup<CLF.Web.Mvc.Startup>();

            var testServer = new TestServer(builder);
            _client = testServer.CreateClient();
        }

        /// <summary>
        /// 测试controller记得RegisterDTO参数前加[FromBody]，否则无法自动赋值
        /// 发送注册邮件需要更改appsetting邮件节点配置（username和password）
        /// </summary>
        [Fact]
        public  async void Test_Register()
        {
            try
            {
                string action = "/home/register";
                RegisterDTO model = new RegisterDTO
                {
                    Email = "chenlinfei929@126.com",
                    Password = "qwert123",
                    ConfirmPassword = "qwert123"
                };
                var result = await _client.PostAsJsonAsync(action, model);
                result.EnsureSuccessStatusCode();

                var data = await result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 测试controller记得SignInDTO参数前加[FromBody]，否则无法自动赋值
        /// </summary>
        [Fact]
        public async void Test_Login()
        {
            string action = "/home/login?returnUrl=http://localhost:50995/";
            SignInDTO model = new SignInDTO
            {
                UserName = "chenlinfei929@126.com",
                Password = "qwert123"
            };
            var result = await _client.PostAsJsonAsync(action, model);
            result.EnsureSuccessStatusCode();

            var data = await result.Content.ReadAsStringAsync();
        }
    }
}
