using RestSharp;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace HRbackend.Lib
{
    public class Notifications
    {
        private readonly IConfiguration _configuration;
        private readonly string SenderName = "Sendchamp";
        private readonly string Route = "dnd";
        private readonly HttpClientService _httpClientService;

        public Notifications(IConfiguration configuration, HttpClientService httpClientService)
        {
            _configuration = configuration;
            _httpClientService = httpClientService;
        }

        // Make the method public so it can be called from other classes/files
        public async Task SendSmsNotificationAsync(string[] to, string messageContent)
        {
            try
            {
                // Get API key from configuration
                string authorizationToken = _configuration["SmsSettings:SendChampApiKey"];
                string SendSmsUrI = _configuration["SmsSettings:SendSmsUrI"];

                var requestBody = new
                {
                    to = to,
                    message = messageContent
                };
            

                var response = await _httpClientService.PostAsync(SendSmsUrI, requestBody, authorizationToken);

                    if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or throw it to the caller
                throw new InvalidOperationException("Failed to send SMS notification", ex);
            }
        }
    }
}
