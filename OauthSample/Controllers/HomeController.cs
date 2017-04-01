using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MailChimp.Net;
using MailChimp.Net.Interfaces;
using Newtonsoft.Json;
using OauthSample.Model;
using RestSharp;
using RestSharp.Authenticators;

namespace OauthSample.Controllers
{
    public class HomeController : Controller
    {
        private MailChimpSettings currentMailChimpSettings;
        public HomeController()
        {
            currentMailChimpSettings = new MailChimpSettings();
        }

        public ActionResult Index()
        {
            return View();
        }

        public RedirectResult RedirectToMailChimp()
        {
            var tokenUri = $"{currentMailChimpSettings.authorize_uri}?response_type=code&client_id={currentMailChimpSettings.ClientId}&redirect_uri={HttpUtility.UrlEncode(currentMailChimpSettings.redirect_uri)}";

            return Redirect(tokenUri);
        }

        public async Task<ContentResult> ObtainOauthToken(string code)
        {
            #region Get Acess Token

            string redirectUrl = currentMailChimpSettings.redirect_uri;

            var mailChimpOauthToken = await GetAccessToken(code, redirectUrl);

            if (string.IsNullOrWhiteSpace(mailChimpOauthToken))
                return new ContentResult() { Content = "mailChimp token Empty!" };

            #endregion

            #region Get Api Metadata

            var metadata = await GetMetadata(mailChimpOauthToken);
            if (metadata == null)
                return new ContentResult() { Content = "mailChimp metadata Empty!" };

            #endregion

            //store metadata on db or wherever you want

            //just a test call
            IMailChimpManager mailChimpManager = new MailChimpManager(new MailChimpOauthConfiguration
            {
                DataCenter = metadata.DataCenter,
                OauthToken = mailChimpOauthToken,
            });
            var campaigns = await mailChimpManager.Campaigns.GetAll();

            return new ContentResult() {Content = "OK!"};
        }

        #region Common Methods

        [NonAction]
        public async Task<string> GetAccessToken(string requestCode, string redirectUri)
        {
            var client = new RestClient()
            {
                BaseUrl = new Uri(currentMailChimpSettings.access_token_uri),
            };
            var request = new RestRequest();
            request.Method = Method.POST;
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("client_id", currentMailChimpSettings.ClientId);
            request.AddParameter("client_secret", currentMailChimpSettings.ClientSectet);
            request.AddParameter("redirect_uri", redirectUri);
            request.AddParameter("code", requestCode);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            var response = await client.ExecuteTaskAsync(request);
            if (response.StatusCode != HttpStatusCode.OK ||
                response.ResponseStatus != ResponseStatus.Completed
                || response.ErrorException != null)
            {
                //process error here
                return null;
            }
            return JsonConvert.DeserializeObject<dynamic>(response.Content).access_token;
        }

        [NonAction]
        public async Task<MailChimpApiMetadata> GetMetadata(string mailChimpToken)
        {
            var client = new RestClient()
            {
                BaseUrl = new Uri(currentMailChimpSettings.metadata_uri),
                Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(mailChimpToken),
            };
            var request = new RestRequest();
            var response = await client.ExecuteTaskAsync(request);
            if (response.StatusCode != HttpStatusCode.OK ||
                response.ResponseStatus != ResponseStatus.Completed
                || response.ErrorException != null)
            {
                //process error here
                return null;
            }
            return JsonConvert.DeserializeObject<MailChimpApiMetadata>(response.Content);
        }

        #endregion
    }
}