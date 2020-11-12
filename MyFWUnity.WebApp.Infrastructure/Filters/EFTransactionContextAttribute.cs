using MyFWUnity.Common.Services;
using MyFWUnity.Core.Infrastructure.DatabaseContext;
using MyFWUnity.Core.Model;
using MyFWUnity.WebApp.Infrastructure.Utilities;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MyFWUnity.WebApp.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EFTransactionContextAttribute : ActionFilterAttribute
    {
        private IRepositoryContext m_transactionContext;

        public EFTransactionContextAttribute(Enum database)
        {
            // Singleton
            m_transactionContext = ServiceHelper.GetService(typeof(IRepositoryContext)) as IRepositoryContext;
            if (this.m_transactionContext != null)
            {
                m_transactionContext.Initialize();
            }
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            m_transactionContext.BeginTransaction();
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            if (actionExecutedContext.Exception != null)
            {
                m_transactionContext.Rollback();
            }
            else
            {
                m_transactionContext.Commit();
            }
        }
    }
}
