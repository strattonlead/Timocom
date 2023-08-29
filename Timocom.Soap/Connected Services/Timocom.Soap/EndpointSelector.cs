using Microsoft.Extensions.DependencyInjection;
using System;
using Timocom.Soap.DependencyInjection;

namespace Timocom.Soap
{
    public partial class TCCClientPortTypeClient
    {
        protected TimocomSoapClientOptions Options;
        public TCCClientPortTypeClient(TimocomSoapClientOptions options) :
                 base(_getDefaultBinding(), new System.ServiceModel.EndpointAddress(options.RemoteAddress))
        {
            Options = options;
            this.Endpoint.Name = EndpointConfiguration.TCCClientSoap12.ToString();
            _configureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        private static System.ServiceModel.Channels.Binding _getDefaultBinding()
        {
            System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
            System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
            textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
            result.Elements.Add(textBindingElement);
            System.ServiceModel.Channels.HttpTransportBindingElement httpBindingElement = new System.ServiceModel.Channels.HttpsTransportBindingElement();
            httpBindingElement.AllowCookies = true;
            httpBindingElement.MaxBufferSize = int.MaxValue;
            httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
            result.Elements.Add(httpBindingElement);
            return result;
        }

        private void _configureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials)
        {
            if (Options.UseCredentials)
            {
                if (string.IsNullOrWhiteSpace(Options.PrincipalName))
                {
                    throw new ArgumentException($"The parameter {nameof(Options.PrincipalName)} must not be null or whitespace");
                }

                if (string.IsNullOrWhiteSpace(Options.Password))
                {
                    throw new ArgumentException($"The parameter {nameof(Options.Password)} must not be null or whitespace");
                }

                clientCredentials.UserName.UserName = Options.PrincipalName;
                clientCredentials.UserName.Password = Options.Password;
            }
        }
    }

    public class TimocomClient : TCCClientPortTypeClient
    {
        protected readonly IServiceProvider ServiceProvider;
        public TimocomClient(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<TimocomSoapClientOptions>())
        {
            ServiceProvider = serviceProvider;
        }
    }
}
