using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JWT.Algorithms;
using JWT.Builder;

namespace HelloWorld
{
    class Program
    {
        static async Task Main()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string endpoint = "https://mortgage-external-0.sofitest.com/mort-loan-service/v1/documents/affiliated-business-disclosure-35392";

                    long issuedAt = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
                    long tokenTTL = 60;
                    long expiration = issuedAt + tokenTTL;

                    var token = new JwtBuilder()
                        .WithAlgorithm(new HMACSHA256Algorithm())
                        .WithSecret("N,Kj?*YGR3MeJ2i28+s7vAwKer$8R93CHdAha^4}QUrP%qnpLqn2$HZZA,uxRAcC")
                        .AddClaim("aud", $"GET {endpoint}")
                        .AddClaim("jti", Guid.NewGuid().ToString())
                        .AddClaim("iat", issuedAt)
                        .AddClaim("exp", expiration)
                        .Build();

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage response = await client.GetAsync(endpoint);

                    using (Stream abadStream = await response.Content.ReadAsStreamAsync())
                    {
                        string abadPath = "/tmp/abad-test.pdf";
                        using (Stream outputStream = File.Open(abadPath, FileMode.Create))
                        {
                            await abadStream.CopyToAsync(outputStream);
                        }

                        response.Content = null;
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message: {0}", e.Message);
                }
            }
        }
    }
}
