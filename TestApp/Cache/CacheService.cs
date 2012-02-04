using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DDS.GSphere.ClaimsProxy;


namespace TestApp
{
    public class CacheService : ICacheService
    {
        #region Cache Keys

        private const string SpCookieHolder = "SPCookie";

        #endregion

        #region Head

        private readonly IInMemoryCache _inmemorycacheService;
        private readonly SpCookie _spCookie;

        public CacheService()
            : this(new InMemoryCache(), new SpCookie())
        {
        }

        private CacheService(IInMemoryCache inmemorycacheService, SpCookie spCookie)
        {
            _inmemorycacheService = inmemorycacheService;
            _spCookie = spCookie;
        }

        #endregion

        public string GetSPCookie(string username, string spSiteUrl, SPServiceRequestor requestor)
        {
            return _inmemorycacheService.Get(_getUserKey(SpCookieHolder, username), () => _spCookie.GetCookie(spSiteUrl, requestor));
        }

        #region Helpers

        private string _getUserKey(string holder, string username)
        {
            return String.Format("{0}_{1}", username,
                                            holder);
        }

        #endregion
    }
}