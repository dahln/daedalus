using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using daedalus.Shared;
using Blazored.Toast.Services;
using BlazorSpinner;
using daedalus.Shared.Model;

namespace daedalus.Client.Services
{
    public class API
    {
        private HttpClient _httpClient { get; set; }
        private NavigationManager _navigationManger { get; set; }
        private IToastService _toastService { get; set; }
        private SpinnerService _spinnerService { get; set; }
        public API(HttpClient httpClient, NavigationManager navigationManager, IToastService toastService, SpinnerService spinnerService)
        {
            _httpClient = httpClient;

            _navigationManger = navigationManager;
            _toastService = toastService;
            _spinnerService = spinnerService;
        }
        
        async public Task<ConditionSearchResponse> SearchConditions(long start, long end, int page, int size)
        {
            return await GetAsync<ConditionSearchResponse>($"api/v1/condition/search/{start}/{end}/{page}/{size}");
        }

        #region HTTP Methods
        private async Task GetAsync(string path)
        {
            await Send(HttpMethod.Get, path);
        }
        private async Task<T> GetAsync<T>(string path)
        {
            var response = await Send(HttpMethod.Get, path);
            T result = await ParseResponseObject<T>(response);
            return result;
        }
        private async Task PostAsync(string path, object content)
        {
            await Send(HttpMethod.Post, path, content);
        }
        private async Task<T> PostAsync<T>(string path, object content)
        {
            var response = await Send(HttpMethod.Post, path, content);
            return await ParseResponseObject<T>(response);
        }
        private async Task PutAsync(string path, object content)
        {
            await Send(HttpMethod.Put, path, content);
        }
        private async Task<T> PutAsync<T>(string path, object content)
        {
            var response = await Send(HttpMethod.Put, path, content);
            return await ParseResponseObject<T>(response);
        }
        private async Task PutAsync(string path)
        {
            await Send(HttpMethod.Put, path);
        }
        private async Task DeleteAsync(string path)
        {
            await Send(HttpMethod.Delete, path);
        }
        private async Task DeleteAsync(string path, object content)
        {
            await Send(HttpMethod.Delete, path, content);
        }
        
        
        private async Task<HttpResponseMessage> Send(HttpMethod method, string path, object content = null)
        {
            _spinnerService.Show();

            var httpWebRequest = new HttpRequestMessage(method, path);

            if (content != null)
            {
                string json = JsonConvert.SerializeObject(content);
                StringContent postContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                httpWebRequest.Content = postContent;
            }

            HttpResponseMessage response = new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            response = await _httpClient.SendAsync(httpWebRequest);

            if (response.IsSuccessStatusCode == false)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                if(!string.IsNullOrEmpty(responseContent))
                {
                    _toastService.ShowError(responseContent);
                }                    
            }

            _spinnerService.Hide();
            return response;
        }

        private async Task<T> ParseResponseObject<T>(HttpResponseMessage response)
        {
            if (response != null && response.IsSuccessStatusCode && response.Content != null)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                //Can't deseriazlize a string unless it starts with a "
                if (typeof(T) == typeof(string))
                    responseContent = $"\"{responseContent}\"";

                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            else
            {
                return default(T);
            }
        }
        #endregion
    }
}
