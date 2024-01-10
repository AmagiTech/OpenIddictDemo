using OpenIddict.Abstractions;
using OpenIddictDemo.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpenIddictDemo.Server
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            var context = scope.ServiceProvider.GetRequiredService<SampleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("14751805-A4A0-47A6-A27B-198438CC5621") == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "14751805-A4A0-47A6-A27B-198438CC5621",
                    ClientSecret = "996Aa571D6CCAn47xaMncU34664A29D64E09xAAA3B468yy27BCD8BDBB7Dmm",
                    DisplayName = "Amagi",
                    Permissions =
                {
                    Permissions.Endpoints.Token,
                    //Permissions.GrantTypes.ClientCredentials,
                    Permissions.GrantTypes.Password
                }
                });
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
