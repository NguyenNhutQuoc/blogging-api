using System;
using Microsoft.AspNetCore.Authorization;

namespace BloggingSystem.Infrastructure.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permission)
            : base(policy: $"Permission:{permission}")
        {
        }
    }
}