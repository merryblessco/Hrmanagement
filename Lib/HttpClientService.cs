using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

public class HttpClientService
{
    private readonly HttpClient _httpClient;

    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // GET Request with Bearer Token and QueryString
    public async Task<HttpResponseMessage> GetAsync(string url, string bearerToken = null, string queryString = null)
    {
        try
        {
            // Append query string if provided
            if (!string.IsNullOrEmpty(queryString))
            {
                url = $"{url}?{queryString}";
            }

            // Set bearer token if provided
            if (!string.IsNullOrEmpty(bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            // Send GET request
            return await _httpClient.GetAsync(url);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("GET request failed", ex);
        }
    }

    // POST Request with Bearer Token and JSON Body
    public async Task<HttpResponseMessage> PostAsync(string url, object body, string bearerToken = null)
    {
        try
        {
            // Set bearer token if provided
            if (!string.IsNullOrEmpty(bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            // Convert body to JSON
            var jsonBody = JsonConvert.SerializeObject(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Send POST request
            return await _httpClient.PostAsync(url, content);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("POST request failed", ex);
        }
    }

    // PUT Request with Bearer Token and JSON Body
    public async Task<HttpResponseMessage> PutAsync(string url, object body, string bearerToken = null)
    {
        try
        {
            // Set bearer token if provided
            if (!string.IsNullOrEmpty(bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            // Convert body to JSON
            var jsonBody = JsonConvert.SerializeObject(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Send PUT request
            return await _httpClient.PutAsync(url, content);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("PUT request failed", ex);
        }
    }

    // DELETE Request with Bearer Token
    public async Task<HttpResponseMessage> DeleteAsync(string url, string bearerToken = null)
    {
        try
        {
            // Set bearer token if provided
            if (!string.IsNullOrEmpty(bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            // Send DELETE request
            return await _httpClient.DeleteAsync(url);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("DELETE request failed", ex);
        }
    }
}
