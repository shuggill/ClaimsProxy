using System.IdentityModel.Tokens;
using System.ServiceModel;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
using System.ServiceModel.Security;
using Microsoft.IdentityModel.SecurityTokenService;

namespace DDS.GSphere.ClaimsProxy
{
    public static class DobstsTokenRequestor
    {
        public static SecurityToken GetToken(string endpointUri, string username, string password, string realm)
        {
            // use WSTrust and SSL Transport Mode with security credentials being sent with the message.
            var endpoint = new EndpointAddress(endpointUri);
            var factory = new WSTrustChannelFactory(new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential), endpoint);
            factory.TrustVersion = TrustVersion.WSTrust13;

            factory.Credentials.UserName.UserName = username;
            factory.Credentials.UserName.Password = password;

            // Symmetric key request.
            var rst = new RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                AppliesTo = new EndpointAddress(realm),
                KeyType = KeyTypes.Symmetric
            };

            // Token will be encrypted with ADFS Token Signing cert.
            var channel = factory.CreateChannel();
            return channel.Issue(rst);
        }
    }
}
