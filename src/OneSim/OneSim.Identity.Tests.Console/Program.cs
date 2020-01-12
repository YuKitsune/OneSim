namespace OneSim.Identity.Tests.Console
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using IdentityModel.Client;
    using IdentityModel.OidcClient;

    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     The Program.
    /// </summary>
    public class Program
	{
        /// <summary>
        ///     The Authority (domain name).
        /// </summary>
        static string _authority = "https://demo.identityserver.io";

        /// <summary>
        ///     The <see cref="OidcClient"/>.
        /// </summary>
        static OidcClient _oidcClient;

        /// <summary>
        ///     The <see cref="HttpClient"/> for the API.
        /// </summary>
        static HttpClient _apiClient;

        /// <summary>
        ///     The main application entry point.
        /// </summary>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        /// <summary>
        ///     The asynchronous method for <see cref="Main"/> to run.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        public static async Task MainAsync()
        {
            Console.WriteLine("+-----------------------+");
            Console.WriteLine("|  Sign in with OIDC    |");
            Console.WriteLine("+-----------------------+");
            Console.WriteLine(string.Empty);
            Console.Write("API URL: ");
            _authority = Console.ReadLine();
            HttpClientHandler handler = new HttpClientHandler
                                        {
                                            ServerCertificateCustomValidationCallback = HttpClientHandler
                                               .DangerousAcceptAnyServerCertificateValidator
                                        };
            _apiClient = new HttpClient(handler) { BaseAddress = new Uri(_authority) };
            Console.WriteLine("Press any key to sign in...");
            Console.ReadKey();

            await Login();
        }

        /// <summary>
        ///     Logs in.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        private static async Task Login()
        {
            // create a redirect URI using an available port on the loopback address.
            // requires the OP to allow random ports on 127.0.0.1 - otherwise set a static port
            var browser = new SystemBrowser();
            string redirectUri = string.Format($"http://127.0.0.1:{browser.Port}");

            var options = new OidcClientOptions
            {
                Authority = _authority,
                ClientId = "desktop",
                RedirectUri = redirectUri,
                Scope = "openid profile identity traffic",
                FilterClaims = false,
                Browser = browser,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect
            };

            _oidcClient = new OidcClient(options);
            var result = await _oidcClient.LoginAsync(new LoginRequest());

            ShowResult(result);
            await NextSteps(result);
        }

        private static void ShowResult(LoginResult result)
        {
            if (result.IsError)
            {
                Console.WriteLine("\n\nError:\n{0}", result.Error);
                return;
            }

            Console.WriteLine("\n\nClaims:");
            foreach (var claim in result.User.Claims)
            {
                Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
            }

            Console.WriteLine($"\nidentity token: {result.IdentityToken}");
            Console.WriteLine($"access token:   {result.AccessToken}");
            Console.WriteLine($"refresh token:  {result?.RefreshToken ?? "none"}");
        }

        private static async Task NextSteps(LoginResult result)
        {
            var currentAccessToken = result.AccessToken;
            var currentRefreshToken = result.RefreshToken;

            var menu = "  x...exit  c...call api   ";
            if (currentRefreshToken != null) menu += "r...refresh token   ";

            while (true)
            {
                Console.WriteLine("\n\n");

                Console.Write(menu);
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.X) return;
                if (key.Key == ConsoleKey.C) await CallApi(currentAccessToken);
                if (key.Key == ConsoleKey.R)
                {
                    var refreshResult = await _oidcClient.RefreshTokenAsync(currentRefreshToken);
                    if (result.IsError)
                    {
                        Console.WriteLine($"Error: {refreshResult.Error}");
                    }
                    else
                    {
                        currentRefreshToken = refreshResult.RefreshToken;
                        currentAccessToken = refreshResult.AccessToken;

                        Console.WriteLine("\n\n");
                        Console.WriteLine($"access token:   {result.AccessToken}");
                        Console.WriteLine($"refresh token:  {result?.RefreshToken ?? "none"}");
                    }
                }
            }
        }

        private static async Task CallApi(string currentAccessToken)
        {
            _apiClient.SetBearerToken(currentAccessToken);
            var response = await _apiClient.GetAsync("");

            if (response.IsSuccessStatusCode)
            {
                var json = JArray.Parse(await response.Content.ReadAsStringAsync());
                Console.WriteLine("\n\n");
                Console.WriteLine(json);
            }
            else
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
            }
        }
	}
}