using System;
using DDS.GSphere.ClaimsProxy;
using System.Net;

namespace TestApp
{
    public class SpCookie
    {
        public string GetCookie(String spSiteUrl, SPServiceRequestor requestor)
        {
            string spCookieRaw = "";
            spCookieRaw = requestor.GetCookie();

            var cookieExpiry = DateTime.UtcNow.AddHours(1);

            var spCookie = new Cookie("FedAuth", spCookieRaw)
            {
                Expires = cookieExpiry,
                Path = "/",
                Secure = true,
                HttpOnly = true,
                Domain = (new Uri(spSiteUrl)).Host
            };

            return spCookie.ToString();
        }
    }
}