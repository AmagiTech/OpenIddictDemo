using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Client;
using System.Net.Http.Headers;

var services = new ServiceCollection();
services.AddOpenIddict()
    .AddClient(options =>
    {
        options.AllowPasswordFlow();
        options.DisableTokenStorage();
        options.UseSystemNetHttp()
        .SetProductInformation(typeof(Program).Assembly);
        options.AddRegistration(new OpenIddictClientRegistration
        {
            Issuer = new Uri("https://localhost:7027/", UriKind.Absolute),
            ClientId = "12758AFD-8CA0-4786-B5F5-AB718A84BF4B",
            ClientSecret = "996Aa571D6CCAn47x841BCB34664A29D64E09xAAA3B468yy27BCD8BDBB7Dmm"
        });
    });
await using var provider = services.BuildServiceProvider();

var username = "0A2B07D3-7396-49B4-B824-449DF9D3C2EE";
var pass = "5wsc31G#!d2Zuz*yB6Asfg!8UUzH%nMd";

var token = await GetTokenAsync(provider, username, pass);
Console.WriteLine("Access token: {0}", token);
Console.WriteLine();

var resource = await GetResourceAsync(provider, token);
Console.WriteLine("API response: {0}", resource);

Console.ReadLine();

static async Task<string> GetTokenAsync(IServiceProvider provider, string username, string password)
{
    var service = provider.GetRequiredService<OpenIddictClientService>();
    var result = await service.AuthenticateWithPasswordAsync(new()
    {
        Username = username,
        Password = password
    });

    return result.AccessToken;
}

static async Task<string> GetResourceAsync(IServiceProvider provider, string token)
{
    using var client = provider.GetRequiredService<HttpClient>();
    using var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7027/WeatherForecast");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

    using var response = await client.SendAsync(request);
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadAsStringAsync();
}