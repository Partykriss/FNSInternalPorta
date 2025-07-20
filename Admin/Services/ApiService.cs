using FNS.Admin.Model;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace FNS.Admin.ViewModel
{
    public class ApiService
    {
        public async Task<List<T>> GetAsync<T>(string endpoint)
        {
            using (var clientHandler = new HttpClientHandler())
            {
                clientHandler.UseDefaultCredentials = true;
                using (var client = new HttpClient(clientHandler))
                {
                    try
                    {
                        var response = await client.GetAsync(GlobalSettings.ApiBaseUrl + endpoint);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            return JsonConvert.DeserializeObject<List<T>>(json);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message);
                    }
                    return new List<T>();
                }
            }
        }

        public async Task<bool> DeleteAsync(string endpoint, int id)
        {
            using (var clientHandler = new HttpClientHandler())
            {
                clientHandler.UseDefaultCredentials = true;
                using (var client = new HttpClient(clientHandler))
                {
                    try
                    {
                        var response = await client.DeleteAsync($"{GlobalSettings.ApiBaseUrl}{endpoint}{id}");
                        var test = response.StatusCode;
                        return response.IsSuccessStatusCode;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public async Task<bool> PostAsync<T>(string endpoint, T item)
        {
            using (var clientHandler = new HttpClientHandler())
            {
                clientHandler.UseDefaultCredentials = true;
                using (var client = new HttpClient(clientHandler))
                {
                    var json = JsonConvert.SerializeObject(item);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var test = data;
                    try
                    {
                        var response = await client.PostAsync(GlobalSettings.ApiBaseUrl + endpoint, data);
                        return response.IsSuccessStatusCode;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public async Task<bool> PutAsync<T>(string endpoint, int id, T item)
        {
            using (var clientHandler = new HttpClientHandler())
            {
                clientHandler.UseDefaultCredentials = true;
                using (var client = new HttpClient(clientHandler))
                {
                    var json = JsonConvert.SerializeObject(item);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    try
                    {
                        var response = await client.PutAsync(GlobalSettings.ApiBaseUrl + endpoint + id, data);
                        return response.IsSuccessStatusCode;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public async Task<bool> DeactivateVotingAsync(string endpoint, int id)
        {
            using (var clientHandler = new HttpClientHandler())
            {
                clientHandler.UseDefaultCredentials = true;
                using (var client = new HttpClient(clientHandler))
                {
                    try
                    {
                        var response = await client.PutAsync(GlobalSettings.ApiBaseUrl + endpoint + $"{id}/deactivate", null);
                        return response.IsSuccessStatusCode;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
                        return false;
                    }
                }
            }
        }
    }
}
