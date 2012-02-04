using System;
using System.Linq;
using System.Text;
using System.Net;
using System.IdentityModel.Tokens;
using System.Web;

namespace DDS.GSphere.ClaimsProxy
{
    public static class SharepointCookieRequestor
    {
        public static string GetCookie(SecurityToken adfsToken, string siteUrl, string realm)
        {
            var xmlToken = adfsToken as GenericXmlSecurityToken;

            // build the relying party information to build the tokens for.
            var sharePointInformation = new
            {
                Wctx = siteUrl + "_layouts/Authenticate.aspx?Source=%2F",
                Wtrealm = realm,
                Wreply = siteUrl + "_trust/"
            };

            var validFrom = String.Format("{0:s}", adfsToken.ValidFrom) + "Z";
            var validTo = String.Format("{0:s}", adfsToken.ValidTo) + "Z";

            // had to add in some extra XML wrapping here to get it working.
            var adfsTokenXml = String.Format(
                "<t:RequestSecurityTokenResponse xmlns:t=\"http://schemas.xmlsoap.org/ws/2005/02/trust\"><t:Lifetime><wsu:Created xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">{0}</wsu:Created><wsu:Expires xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">{1}</wsu:Expires></t:Lifetime><wsp:AppliesTo xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\"><wsa:EndpointReference xmlns:wsa=\"http://www.w3.org/2005/08/addressing\"><wsa:Address>{2}</wsa:Address></wsa:EndpointReference></wsp:AppliesTo><t:RequestedSecurityToken>{3}</t:RequestedSecurityToken><t:TokenType>urn:oasis:names:tc:SAML:1.0:assertion</t:TokenType><t:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</t:RequestType><t:KeyType>http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey</t:KeyType></t:RequestSecurityTokenResponse>",
                validFrom, validTo, realm, xmlToken.TokenXml.OuterXml);

            // need to post the AD FS token to the SharePoint STS Server.
            var sharepointRequest = WebRequest.Create(sharePointInformation.Wreply) as HttpWebRequest;
            if (sharepointRequest != null)
            {
                // configure the web request for the post to the SharePoint STS
                sharepointRequest.Method = "POST";
                sharepointRequest.ContentType = "application/x-www-form-urlencoded";
                sharepointRequest.CookieContainer = new CookieContainer();
                sharepointRequest.AllowAutoRedirect = false; // This is important

                // build a reference to the request stream to submit the information on.
                var newStream = sharepointRequest.GetRequestStream();

                // format the information to submit to the SharePoint STS.
                var loginInformation = String.Format("wa=wsignin1.0&wctx={0}&wresult={1}",
                    HttpUtility.UrlEncode(sharePointInformation.Wctx),
                    HttpUtility.UrlEncode(adfsTokenXml));

                // convert the login information to bytes for submittion on the request stream.
                var loginInformationBytes = Encoding.UTF8.GetBytes(loginInformation);

                // write the bytes to the request stream.
                newStream.Write(loginInformationBytes, 0, loginInformationBytes.Length);
                newStream.Close();

                // retrieve the response from the SharePoint STS.
                var webResponse = sharepointRequest.GetResponse() as HttpWebResponse;

                // inspect the response for the FedAuth cookie and return its contents.
                if (webResponse != null)
                {
                    // ensure there were cookies received.
                    if (webResponse.Cookies != null && webResponse.Cookies.Count > 0)
                    {
                        // find the FedAuth cook and return it.
                        // TODO: cookie contents may be spread across multiple FedAuthN cookies.
                        foreach (var cookie in
                            webResponse.Cookies.Cast<Cookie>().Where(cookie => cookie.Name == "FedAuth"))
                        {
                            return cookie.Value;
                        }
                    }
                }
            }

            // unable to find the FedAuth cookie, return an empty string to be handled by the calling method.
            return string.Empty;
        }
    }
}
