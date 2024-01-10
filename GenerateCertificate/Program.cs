using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;


Console.WriteLine("Certificate Name:");
var certificateName = Console.ReadLine();
if (string.IsNullOrWhiteSpace(certificateName))
    Console.WriteLine("Geçersiz sertifika ismi!");
else
{
    certificateName = certificateName.Trim();
    var sb = new StringBuilder();
    foreach (var ch in certificateName.ToLower())
    {
        if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && (ch) <= '9'))
            sb.Append(ch);
    }
    var fileName = sb.ToString();
    GenerateCertificate($"CN={certificateName} Encryption Certificate", X509KeyUsageFlags.KeyEncipherment, $"{fileName}-encryption-certificate.pfx");
    GenerateCertificate($"CN={certificateName} Signing Certificate", X509KeyUsageFlags.DigitalSignature, $"{fileName}-signing-certificate.pfx");
}






static void GenerateCertificate(string distinguishedName, X509KeyUsageFlags x509KeyUsageFlags, string fileName)
{
    using (var algorithm = RSA.Create(keySizeInBits: 2048))
    {
        var subject = new X500DistinguishedName(distinguishedName);
        var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(x509KeyUsageFlags, critical: true));
        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));
        File.WriteAllBytes(fileName, certificate.Export(X509ContentType.Pfx, string.Empty));
        Console.WriteLine($"Generated Certificate: {fileName}");
    }
}