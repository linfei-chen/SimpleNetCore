﻿using Autofac;
using CLF.Common.Caching;
using CLF.Common.Configuration;
using CLF.Common.Infrastructure;
using CLF.Common.Infrastructure.TypeFinder;
using CLF.Common.Redis;
using CLF.DataAccess.Account;
using CLF.DataAccess.Account.Repository;
using CLF.Domain.Core.EFRepository;
using CLF.Domain.Core.IRepository;
using CLF.Model.Account;
using CLF.Model.Core;
using CLF.Service.Account;
using CLF.Service.Core.Events;
using CLF.Service.Core.Messages;
using CLF.Web.Framework.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLF.Web.Framework.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder,AppConfig config)
        {
            #region UnitOfWorkContext

            builder.RegisterType<AccountUnitOfWorkContext>()
                .As<EFUnitOfWorkContextBase>()
                .Named<EFUnitOfWorkContextBase>("AccountUnitOfWorkContext").InstancePerLifetimeScope();

            #endregion

            #region repositories

            builder.Register(context => new PermissionRepository(context.ResolveNamed<EFUnitOfWorkContextBase>("AccountUnitOfWorkContext")))
                .AsSelf().InstancePerLifetimeScope();


            #endregion

            #region UserManager，RoleManager，SignInManager
            builder.RegisterType<UserManager<AspNetUsers>>().As<UserManager<AspNetUsers>>().InstancePerLifetimeScope();
            builder.RegisterType<RoleManager<AspNetRoles>>().As<RoleManager<AspNetRoles>>().InstancePerLifetimeScope();
            builder.RegisterType<PasswordHasher<AspNetUsers>>().As<IPasswordHasher<AspNetUsers>>().InstancePerLifetimeScope();
            builder.RegisterType<PasswordValidator<AspNetUsers>>().As<IPasswordValidator<AspNetUsers>>().InstancePerLifetimeScope();
            builder.RegisterType<UserValidator<AspNetUsers>>().As<IUserValidator<AspNetUsers>>().InstancePerLifetimeScope();
            builder.RegisterType<RoleValidator<AspNetRoles>>().As<IRoleValidator<AspNetRoles>>().InstancePerLifetimeScope();
            builder.RegisterType<UpperInvariantLookupNormalizer>().As<ILookupNormalizer>().InstancePerLifetimeScope();
            builder.RegisterType<IdentityErrorDescriber>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<ApplicationSignInManager>().AsSelf().InstancePerLifetimeScope();

            #endregion

            #region services

            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();

            #endregion
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerLifetimeScope();

            #region 缓存
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();

            //redis
            if (config.RedisEnabled)
            {
                builder.RegisterType<RedisConnectionWrapper>()
                    .As<ILocker>()
                    .As<IRedisConnectionWrapper>()
                    .SingleInstance();
            }

            if (config.RedisEnabled && config.UseRedisForCaching)
            {
                builder.RegisterType<RedisCacheManager>().As<IStaticCacheManager>().InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<MemoryCacheManager>()
                    .As<ILocker>()
                    .As<IStaticCacheManager>()
                    .SingleInstance();
            }

            #endregion

            #region 注册清缓存的对象
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                //某对象同时实现多个泛型接口注册接口的办法
                builder.RegisterType(consumer)
                    .As(consumer.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>)))
                    .InstancePerLifetimeScope();
            }
            #endregion
        }
    }
}
