using System;
using DDS.GSphere.ClaimsProxy;
using System.ServiceModel;
using System.Xml.Linq;
using System.Net;
using System.Collections;
using System.Xml;
using System.Web;
using System.Web.Caching;
using System.Diagnostics;

namespace TestApp
{
    public partial class _Default : System.Web.UI.Page
    {
        // ought to refactor this to use DI but it's only a test app...
        private ICacheService _cacheService = new CacheService();

        protected void Button1_Click(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            txtDebug.Text = "";
            txtOutput.Text = "";

            // configure our SPServiceRequestor.
            var requestor = new SPServiceRequestor
            {
                DobstsEndpoint = txtStarterStsEndpoint.Text,
                DobstsUsername = txtStarterStsUsername.Text,
                DobstsPassword = txtStarterStsPassword.Text,
                DobstsAdfsRealm = txtStarterStsADFSRealm.Text,
                AdfsEndpoint = txtADFSEndpoint.Text,
                SharepointRealm = txtSPRealm.Text,
                SharepointSiteUrl = txtSPSite.Text,
                IgnoreSslValidation = chkIgnoreSSL.Checked,
                DebugMode = true,
                DebugEventCallback = (data) =>
                {
                    _debug(data);
                }
            };
            
            string spCookie;

            // attempt to get a cookie.
            try
            {
                _debug("Requesting SharePoint Cookie...");

                spCookie = _cacheService.GetSPCookie(txtStarterStsUsername.Text, txtSPSite.Text, requestor);

                _debug(String.Format("Got SharePoint Cookie = {0}", spCookie));
            }
            catch (Exception ex)
            {
                _debug(String.Format("Exception! {0}", ex.Message));
                return;
            }

            _debug("Making web service call...");

            var error = "";
            var groups = getGroupCollection(txtSPSite.Text, spCookie.ToString(), out error);

            if (error == "")
            {
                _debug(String.Format("No errors reported, total groups returned = {0}", groups.Count.ToString()));
                foreach (DictionaryEntry group in groups)
                {
                    _output(String.Format("{0}\n", group.Key));
                }
            }
            else
            {
                _debug(String.Format("{0}", error));
            }

            sw.Stop();

            _debug(String.Format("Total Execution Time (ms) = {0}", sw.ElapsedMilliseconds));
        }

        #region SharePoint Service Calls

        public static SortedList getGroupCollection(string webPath, string spCookie, out string error)
        {
            error = "";
            SortedList groups = new SortedList();
            try
            {
                SharepointUserGroupsWCF.UserGroupSoapClient client = new SharepointUserGroupsWCF.UserGroupSoapClient();
                //client.ClientCredentials.Windows.ClientCredential = (NetworkCredential)credentials;
                //client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                client.Endpoint.Address = new EndpointAddress(webPath + "/_vti_bin/usergroup.asmx");

                // -- start new code --
                // add a Cookie Behaviour to inject a cookie into the underlying httpRequest
                CookieBehavior cookieBehaviour = new CookieBehavior(spCookie);
                client.Endpoint.Behaviors.Add(cookieBehaviour);
                // -- end new code --

                XElement groupXml;
                groupXml = client.GetGroupCollectionFromSite();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(groupXml.ToString());
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("sp", "http://schemas.microsoft.com/sharepoint/soap/directory/");
                foreach (XmlNode group in doc.SelectNodes("//sp:Groups/sp:Group", nsmgr))
                {
                    groups.Add(group.Attributes["Name"].Value.ToUpper(), "");
                }
            }
            catch (Exception e)
            {
                error = "ERROR: " + e.Message;
                return groups;
            }

            return groups;
        }

        #endregion

        #region Helpers

        private void _output(string a)
        {
            txtOutput.Text += string.Format("{0}\n", a);
        }

        private void _debug(string a)
        {
            txtDebug.Text += String.Format("{0}\n", a);
        }

        #endregion

    }
}
