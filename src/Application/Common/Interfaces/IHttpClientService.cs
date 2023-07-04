using CleanArchitecture.Domain.Enums;
using JetBrains.Annotations;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IHttpClientService
{
    Task<HttpResponseMessage> SendRequestAsync(HttpPurpose purpose, HttpVerb verb, string requestUri, [CanBeNull] object requestModel);
}

