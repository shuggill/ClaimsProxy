using System;
using System.Net;

namespace DDS.GSphere.ClaimsProxy
{
    public class SPServiceRequestor
    {
        public string DobstsEndpoint { get; set; }
        public string DobstsUsername { get; set; }
        public string DobstsPassword { get; set; }
        public string DobstsAdfsRealm { get; set; }
        public string AdfsEndpoint { get; set; }
        public string SharepointRealm { get; set; }
        public string SharepointSiteUrl { get; set; }
        public bool IgnoreSslValidation { get; set; }
        public bool DebugMode { get; set; }

        // our delegate for debugging events.
        public delegate void DebugEvent(string data);
        public DebugEvent DebugEventCallback { get; set; }

        #region Constructors

        public SPServiceRequestor()
        {
        }
        
        public SPServiceRequestor(string dobstsEndpoint, string dobstsUsername, string dobstsPassword, string dobstsAdfsRealm,
                                  string adfsEndpoint, string sharepointRealm, string sharepointSiteUrl, bool ignoreSslValidation,
                                  bool debugMode)
        {
            this.DobstsEndpoint = dobstsEndpoint;
            this.DobstsUsername = dobstsUsername;
            this.DobstsPassword = dobstsPassword;
            this.DobstsAdfsRealm = dobstsAdfsRealm;
            this.AdfsEndpoint = adfsEndpoint;
            this.SharepointRealm = sharepointRealm;
            this.SharepointSiteUrl = sharepointSiteUrl;
            this.IgnoreSslValidation = ignoreSslValidation;
            this.DebugMode = debugMode;
        }

        #endregion

        public string GetCookie()
        {
            _validateConfiguration();
            if (IgnoreSslValidation) _setCertValidator();

            _debug("Requesting DobSts Token...");

            var dobstsToken = DobstsTokenRequestor.GetToken(DobstsEndpoint,
                                                            DobstsUsername,
                                                            DobstsPassword,
                                                            DobstsAdfsRealm);

            _debug("Requesting ADFS Token...");

            var adfsToken = AdfsTokenRequestor.GetToken(dobstsToken,
                                                        AdfsEndpoint,
                                                        SharepointRealm);

            _debug("Requesting SP Cookie...");

            var spCookie = SharepointCookieRequestor.GetCookie(adfsToken,
                                                               SharepointSiteUrl,
                                                               SharepointRealm);

            return spCookie;
        }

        #region Helpers

        private void _debug(string data)
        {
            if (DebugMode)
            {
                DebugEventCallback(String.Format("SPServiceRequestor: {0}",data));
            }
        }

        private void _setCertValidator()
        {
            _debug("Disabling SSL Certificate checks");

            // skip certificate checks.
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => { return true; };
        }

        private void _validateConfiguration()
        {
            if (this.DobstsEndpoint == null) throw new ArgumentNullException("DobstsEndpoint");
            if (this.DobstsEndpoint == "") throw new ArgumentException("DobstsEndpoint is empty");
            if (this.DobstsUsername == null) throw new ArgumentNullException("DobstsUsername");
            if (this.DobstsPassword == null) throw new ArgumentNullException("DobstsPassword");
            if (this.DobstsAdfsRealm == "") throw new ArgumentException("DobstsAdfsRealm is empty");
            if (this.AdfsEndpoint == "") throw new ArgumentException("AdfsEndpoint is empty");
            if (this.SharepointRealm == "") throw new ArgumentException("SharepointRealm is empty");
        }

        #endregion
    }
}
