using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Runtime.Serialization;

namespace AuthPack
{
    public static class GoogleAuth
    {
        /// <summary>
        /// Used by Google oAuth process to retrieve tokens.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="refresh_token"></param>
        /// <param name="returl"></param>
        /// <returns></returns>
        public static GoogleTokens GoogleTokensGet(string code, string refresh_token, string returl)
        {
            string grant_type = "authorization_code";
            string code_or_refresh_token = "code=" + System.Web.HttpUtility.UrlEncode(code);

            if (refresh_token != null)
            {
                grant_type = "refresh_token";
                code_or_refresh_token = "refresh_token=" + System.Web.HttpUtility.UrlEncode(refresh_token);
            }

            string json = AuthUtilities.WebRequest(AuthUtilities.Method.POST, "https://accounts.google.com/o/oauth2/token",
                code_or_refresh_token
                + "&client_id=" + ConfigurationManager.AppSettings["google_clientid"].ToString()
                + "&client_secret=" + ConfigurationManager.AppSettings["google_clientsecret"].ToString()
                + "&redirect_uri=" + System.Web.HttpUtility.UrlEncode(returl)
                + "&grant_type=" + grant_type
                );
            return Json.Deserialise<GoogleTokens>(json);
        }
    }

    public class GoogleTokens
    {
        [DataMember]
        public string access_token;
        [DataMember]
        public int expires_in;
        [DataMember]
        public string token_type;
        [DataMember]
        public string refresh_token;
    }

    public class GoogleData
    {
        [DataMember]
        public Data data;
    }
    public class Data
    {
        [DataMember]
        public string email;
    }


}