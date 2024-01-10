A) I added ClientId restriction. But you can cancel it.
   Follow these steps
  A.1) OpenIddictDemo.Server/Program.cs
  // Uncomment in order to ignore CLientId
  options.AcceptAnonymousClients();

  A.2) OpenIddictDemo.Server/Worker.cs
  Delete code below
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
  A.3) OpenIddictDemo.Client/Program.cs
   Delete ClientId and ClientSecret definitions
   ClientId = "12758AFD-8CA0-4786-B5F5-AB718A84BF4B",
   ClientSecret = "996Aa571D6CCAn47x841BCB34664A29D64E09xAAA3B468yy27BCD8BDBB7Dmm"
   
B) For development use AddCertificates
OpenIddictDemo.Server/Program.cs
        //// For Development Uncomment it
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
