using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
using System.ServiceModel;
using Microsoft.IdentityModel.Protocols.WSTrust;
using System.ServiceModel.Security;
using Microsoft.IdentityModel.SecurityTokenService;

namespace DDS.GSphere.ClaimsProxy
{
    public static class AdfsTokenRequestor
    {
        public static SecurityToken GetToken(SecurityToken dobstsToken, string endpointUri, string spRealm)
        {
            // WSTrust call over SSL with credentails sent in the message.
            var binding = new IssuedTokenWSTrustBinding();
            binding.SecurityMode = SecurityMode.TransportWithMessageCredential;

            var factory = new WSTrustChannelFactory(
                binding,
                endpointUri);
            factory.TrustVersion = TrustVersion.WSTrust13;
            factory.Credentials.SupportInteractive = false;

            // Request Bearer Token so no keys or encryption required.
            var rst = new RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                AppliesTo = new EndpointAddress(spRealm),
                KeyType = KeyTypes.Bearer
            };

            // Make the request with the DobstsToken.
            factory.ConfigureChannelFactory();
            var channel = factory.CreateChannelWithIssuedToken(dobstsToken);
            return channel.Issue(rst) as GenericXmlSecurityToken;
        }
    }
}
