using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OauthSample.Model
{
    public class MailChimpSettings
    {
        //get urls from http://developer.mailchimp.com/documentation/mailchimp/guides/how-to-use-oauth2/
        public string authorize_uri { get; set; } = "https://login.mailchimp.com/oauth2/authorize";
        public string access_token_uri { get; set; } = "https://login.mailchimp.com/oauth2/token";
        public string metadata_uri { get; set; } = "https://login.mailchimp.com/oauth2/metadata";


        //get data from yours mailchimp registered application
        public string ClientId { get; set; } = "YOUR_CLIENTID";
        public string ClientSectet { get; set; } = "YOUR_CLIENTSECRET";
        public string redirect_uri { get; set; } = "http://127.0.0.1/OauthSample/Home/ObtainOauthToken";
    }
}