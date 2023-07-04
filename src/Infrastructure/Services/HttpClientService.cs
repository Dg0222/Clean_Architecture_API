using Newtonsoft.Json;
using System.Text;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Infrastructure.Services;

public class HttpClientService : IHttpClientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private HttpClient _httpClient = new();

    public HttpClientService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }


    #region Helpers


    private async Task<HttpResponseMessage> PostAsync(HttpPurpose purpose, string requestUri, object requestModel)
    {
        return await GetResponseMessageAsync(requestUri, purpose, HttpVerb.Post, requestModel);
    }

    private async Task<HttpResponseMessage> GetAsync(HttpPurpose purpose, string requestUri)
    {
        return await GetResponseMessageAsync(requestUri, purpose, HttpVerb.Get);
    }

    private async Task<HttpResponseMessage> PutAsync(HttpPurpose purpose, string requestUri, object requestModel)
    {
        return await GetResponseMessageAsync(requestUri, purpose, HttpVerb.Put);
    }

    private async Task<HttpResponseMessage> DeleteAsync(HttpPurpose purpose, string requestUri)
    {
        return await GetResponseMessageAsync(requestUri, purpose, HttpVerb.Delete);
    }

    private async Task<HttpResponseMessage> GetResponseMessageAsync(string requestUri, HttpPurpose purpose, HttpVerb verb, object requestModel = null)
    {
        HttpResponseMessage response = null;
        StringContent content = null;

        switch (purpose)
        {
            case HttpPurpose.TestAPI:
                _httpClient = _httpClientFactory.CreateClient("TestAPI");
                break;
        }


        if (requestModel != null)
        {
            var jsonData = JsonConvert.SerializeObject(requestModel);
            content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        }

        switch (verb)
        {
            case HttpVerb.Get:
                response = await _httpClient.GetAsync(requestUri);
                break;

            case HttpVerb.Post:
                response = await _httpClient.PostAsync(requestUri, content);
                break;

            case HttpVerb.Delete:
                response = await _httpClient.DeleteAsync(requestUri);
                break;

            case HttpVerb.Put:
                response = await _httpClient.PutAsync(requestUri, content);
                break;
        }

        await EnsureSuccessAsync(response);
        return response;
    }

    public async Task<HttpResponseMessage> SendRequestAsync(HttpPurpose purpose, HttpVerb verb, string requestUri, object requestModel)
    {
        switch (verb)
        {
            case HttpVerb.Get:
                return await GetAsync(purpose, requestUri);
            case HttpVerb.Post:
                return await PostAsync(purpose, requestUri, requestModel);
            case HttpVerb.Delete:
                return await DeleteAsync(purpose, requestUri);
            case HttpVerb.Put:
                return await PutAsync(purpose, requestUri, requestModel);
            default:
                return new HttpResponseMessage();
        }
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = response.Content == null ? "NoContent" : await response.Content.ReadAsStringAsync();

            throw new HttpRequestException($"{response.StatusCode} " +
                                           $"Reason: {response.ReasonPhrase} " +
                                           $"Content: {content}");
        }
    }

    #endregion

}

