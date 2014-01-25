using System;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Users;
using Abp.Utils.Extensions;
using Abp.Utils.Extensions.Collections;
using Castle.DynamicProxy;

namespace Abp.Modules.Core.Data.Repositories.Interceptors
{
    public class AuditInterceptor : IInterceptor
    {
        private readonly IUserRepository _userRepository;

        public AuditInterceptor(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.MethodInvocationTarget.Name == "Insert")
            {
                if (!invocation.Arguments.IsNullOrEmpty())
                {
                    if (invocation.Arguments[0] is ICreationAudited)
                    {
                        invocation.Arguments[0].As<ICreationAudited>().CreationTime = DateTime.Now;
                        if (invocation.Arguments[0].As<ICreationAudited>().CreatorUser == null)
                        {
                            invocation.Arguments[0].As<ICreationAudited>().CreatorUser = _userRepository.Load(User.CurrentUserId);
                        }
                    }
                }

                invocation.Proceed();
            }
            else if (invocation.MethodInvocationTarget.Name == "Update")
            {
                if (!invocation.Arguments.IsNullOrEmpty())
                {
                    if (invocation.Arguments[0] is IModificationAudited)
                    {
                        invocation.Arguments[0].As<IModificationAudited>().LastModificationTime = DateTime.Now;
                        if (invocation.Arguments[0].As<IModificationAudited>().LastModifierUser == null)
                        {
                            invocation.Arguments[0].As<IModificationAudited>().LastModifierUser = _userRepository.Load(User.CurrentUserId);
                        }
                    }
                }

                invocation.Proceed();
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}