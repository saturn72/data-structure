﻿using Admin.Backend.Domain;
using EasyCaching.Core;

namespace Admin.Backend.Services.Endpoint
{
    public class EndpointValidator : IValidator<CreateContext<EndpointDomainModel>>
    {
        private readonly IEndpointStore _endpoints;
        private readonly IEasyCachingProvider _cache;

        public EndpointValidator(
            IEndpointStore endpoints,
            IEasyCachingProvider cache)
        {
            _endpoints = endpoints;
            _cache = cache;
        }
        public async Task<bool> IsValidFor(CreateContext<EndpointDomainModel> context)
        {
            var key = EndpointCaching.BuildGetPathKey(context?.Model?.Path);
            var ep = await _cache.GetAsync(key,
                () => _endpoints.GetByPath(context.Model.Path),
                EndpointCaching.Expiration);

            if (ep?.HasValue == true)
            {
                context?.SetErrors($"Endpoint already exist: {context.Model}",
                    "Endpoint already exist");
                return false;
            }

            return true;
        }
    }

    public interface IValidator<TContext> where TContext : ContextBase
    {
        Task<bool> IsValidFor(TContext context);
    }
}
