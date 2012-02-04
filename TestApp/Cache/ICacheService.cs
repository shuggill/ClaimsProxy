using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DDS.GSphere.ClaimsProxy;

namespace TestApp
{
    public interface ICacheService
    {
        string GetSPCookie(string username, string spSiteUrl, SPServiceRequestor requestor);
    }
}