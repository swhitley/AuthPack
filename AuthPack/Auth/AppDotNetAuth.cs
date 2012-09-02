/*
 * (Copyright (c) 2011, Shannon Whitley <swhitley@whitleymedia.com> http://voiceoftech.com/
 * 
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are permitted 
 * provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of conditions 
 * and the following disclaimer.
 * 
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions
 * and the following disclaimer in the documentation and/or other materials provided with the distribution.

 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR 
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND 
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS 
 * BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Runtime.Serialization;

namespace AuthPack
{
    public class AppDotNetAuth
    {
        public string access_token { get; set; }
        public const string OAUTH_AUTHENTICATE = "https://alpha.app.net/oauth/authenticate";
        public const string OAUTH_ACCESS_TOKEN = "https://alpha.app.net/oauth/access_token";
        public const string USER = "https://alpha-api.app.net/stream/0/users/[user_id]";
        public const string MY_POSTS = "https://alpha-api.app.net/stream/0/users/me/posts";
        public const string MY_STREAM = "https://alpha-api.app.net/stream/0/posts/stream";
        public const string MENTIONS = "https://alpha-api.app.net/stream/0/users/me/mentions";
        public const string WRITE_POST = "https://alpha-api.app.net/stream/0/posts";
        public const string GLOBAL_STREAM = "https://alpha-api.app.net/stream/0/posts/stream/global";
        public const string POST = "https://alpha-api.app.net/stream/0/posts/";
        public const string FOLLOW = "https://alpha-api.app.net/stream/0/users/[user_id]/follow";
        
        //Auth.net Permissions
        [Flags]
        public enum Scope
        {
            stream = 1, write_post = 2, follow = 4, messages=8, export = 16
        }

        //Add the access token to the header of the web call.
        public List<KeyValuePair<string,string>> AuthHeader()
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("Authorization", "Bearer " + this.access_token));

            return headers;
        }

        /// <summary>
        /// Used by App.net oAuth process to retrieve the authenticate link.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="returl"></param>
        /// <returns></returns>
        public static string AuthorizationLinkGet(string scope, string returl)
        {
            return OAUTH_AUTHENTICATE
                + "?client_id=" + HttpUtility.UrlEncode(ConfigurationManager.AppSettings["appdotnet_clientid"].ToString())
                + "&response_type=code"
                + "&redirect_uri=" + HttpUtility.UrlEncode(returl)
                + "&scope=" + HttpUtility.UrlEncode(scope)
                + "&state=" + HttpUtility.UrlEncode(returl);
        }

        /// <summary>
        /// Used by App.net oAuth process to retrieve tokens.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="returl"></param>
        /// <returns></returns>
        public void TokenGet(string code, string returl)
        {
            string url = OAUTH_ACCESS_TOKEN;

            string json = AuthUtilities.WebRequest(AuthUtilities.Method.POST, url,
                  "client_id=" + HttpUtility.UrlEncode(ConfigurationManager.AppSettings["appdotnet_clientid"].ToString())
                + "&client_secret=" + HttpUtility.UrlEncode(ConfigurationManager.AppSettings["appdotnet_clientsecret"].ToString())
                + "&grant_type=authorization_code"
                + "&redirect_uri=" + HttpUtility.UrlEncode(returl)
                + "&code=" + HttpUtility.UrlEncode(code)
                );
            AppDotNetAccessToken token = Json.Deserialise<AppDotNetAccessToken>(json);
            this.access_token = token.access_token;
        }
    }

    [DataContract]
    class AppDotNetAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
    }
}