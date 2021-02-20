using System;
using MercuryBus.Consumer.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityControl.API.Data
{
    public class MercuryExecutionStrategyMessageHandlerDecorator : IMessageHandlerDecorator, IOrdered
    {
        public Action<SubscriberIdAndMessage, IServiceProvider, IMessageHandlerDecoratorChain> Accept =>
            async (subscriberIdAndMessage, serviceProvider, chain) =>
            {
                var dbContext = serviceProvider.GetService<IdentityContext>();
                IExecutionStrategy executionStrategy = dbContext.Database.CreateExecutionStrategy();
                executionStrategy.Execute(() => { chain.InvokeNext(subscriberIdAndMessage, serviceProvider); });
            };

        public int Order => BuiltInMessageHandlerDecoratorOrder.DuplicateDetectingMessageHandlerDecorator - 1;
    }
}