using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddictDemo.Data;
using OpenIddictDemo.Models;
using OpenIddictDemo.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();

builder.Services.AddDbContext<SampleDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SampleDb"));
    options.UseOpenIddict();
});

builder.Services.AddIdentity<User, Role>()
       .AddEntityFrameworkStores<SampleDbContext>()
       .AddDefaultTokenProviders();

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
                      .UseDbContext<SampleDbContext>();
    })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("connect/token");
        options.AllowPasswordFlow()
        .AllowRefreshTokenFlow();

        // Uncomment in order to ignore CLientId
        //options.AcceptAnonymousClients();

        //// For Development
        // options.AddDevelopmentEncryptionCertificate()
        //             .AddDevelopmentSigningCertificate();

        /* For Production
         * Use GenerateCertificate application.
         * Import Generated Certificates in the Windows
         * Use certmgr app (search Manage User Certificates from start menu)
         * Find your certificates thumbnails
         * In my case Certificates -> Personal -> Certificates -> (Certificate Name you gave) -> Double Click -> Details-> Thumbprint
         *
         https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html
         */
        options.AddEncryptionCertificate(builder.Configuration.GetSection("EncryptionCertificate").Value)
         .AddSigningCertificate(builder.Configuration.GetSection("SigningCertificate").Value);



        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        // Import the configuration from the local OpenIddict server instance.
        options.UseLocalServer();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
