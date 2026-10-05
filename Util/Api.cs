using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Discord;

namespace Util;

public class Api
{
    private string _apiUrl = "https://ftc-api.firstinspires.org/v2.0/";
    private string _username = string.Empty;
    private string _token = string.Empty;

    private HttpClient _http = new HttpClient();

    public Api()
    {
        _username = Environment.GetEnvironmentVariable("FTC_API_USERNAME") ?? string.Empty;
        _token = Environment.GetEnvironmentVariable("FTC_API_TOKEN") ?? string.Empty;

        if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_token))
        {
            throw new InvalidOperationException("Environment variables for FTC_API_USERNAME and FTC_API_TOKEN must be set.");
        }

        string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_token}"));
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
    }

    public async Task<Team[]?> GetTeams()
    {
        string url = _apiUrl + $"{Bot.Season}/teams?state=GA";
        using HttpResponseMessage response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            string errorText = await response.Content.ReadAsStringAsync();
            await Logger.Log(new LogMessage(LogSeverity.Error, "Api", $"Failed to get teams: {(int)response.StatusCode} - {errorText}"));
            return null;
        }

        TeamListResponse? data = await response.Content.ReadFromJsonAsync<TeamListResponse>();
        if (data == null)
        {
            await Logger.Log(new LogMessage(LogSeverity.Error, "Api", "Failed to get teams from API (response is null)."));
            return null;
        }

        // Filter out non-USA Georgia tagged teams
        return data.Teams.Where(team => team.Country == "USA").ToArray();
    }
}