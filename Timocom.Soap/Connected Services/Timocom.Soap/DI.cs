using Microsoft.Extensions.DependencyInjection;
using System;

namespace Timocom.Soap.DependencyInjection
{
    public static class TCCClientPortTypeClientExt
    {
        public static void AddTimocomSoapClient(this IServiceCollection services, Action<TimocomSoapClientOptionsBuilder> builderAction)
        {
            TimocomSoapClientOptions options = new TimocomSoapClientOptions();
            if (builderAction != null)
            {
                var builder = new TimocomSoapClientOptionsBuilder();
                builderAction(builder);
                options = builder._options;
            }

            services.AddSingleton(options);
            services.AddScoped(sp => new TimocomClient(sp));
        }
    }

    public class TimocomSoapClientOptions
    {
        public bool? UseProduction { get; set; }
        public Func<bool> OnUseProduction { get; set; }

        public bool UseCredentials { get; set; }
        public string PrincipalName { get; set; }
        public string Password { get; set; }

        public string RemoteAddress
        {
            get
            {
                if (OnUseProduction?.Invoke() ?? false)
                {
                    return PRODUCTION_ENDPOINT;
                }

                if (UseProduction ?? false)
                {
                    return PRODUCTION_ENDPOINT;
                }

                return TESTING_ENDPOINT;
            }
        }

        private const string TESTING_ENDPOINT = "https://ws-test.timocom.com/tcconnect/ws_v2/soap1_2/";
        private const string PRODUCTION_ENDPOINT = "https://webservice.timocom.com/tcconnect/ws_v2/soap1_2/";
        protected static System.ServiceModel.EndpointAddress GetTestingEndpointAddress()
           => new System.ServiceModel.EndpointAddress("https://ws-test.timocom.com/tcconnect/ws_v2/soap1_2/");

        protected static System.ServiceModel.EndpointAddress GetProductionEndpointAddress()
            => new System.ServiceModel.EndpointAddress("https://webservice.timocom.com/tcconnect/ws_v2/soap1_2/");
    }

    public class TimocomSoapClientOptionsBuilder
    {
        internal TimocomSoapClientOptions _options = new TimocomSoapClientOptions();

        /// <summary>
        /// Use Credentials call
        /// </summary>
        /// <param name="principalName">The Timocom principal name</param>
        /// <param name="password">The Timocom password</param>
        public TimocomSoapClientOptionsBuilder UseCredentials(string principalName, string password)
        {
            _options.UseCredentials = true;
            _options.PrincipalName = principalName;
            _options.Password = password;
            return this;
        }

        /// <summary>
        /// Use the Production Endpoint
        /// </summary>
        public TimocomSoapClientOptionsBuilder UseProduction()
        {
            _options.UseProduction = true;
            return this;
        }

        /// <summary>
        /// Function to determine if the production mode is active
        /// </summary>
        public TimocomSoapClientOptionsBuilder UseProduction(Func<bool> func)
        {
            _options.OnUseProduction = func;
            return this;
        }
    }
}
