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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Web.Security;
using System.Collections.Specialized;
using System.Xml;

namespace AuthPack
{
    public partial class Auth : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Twitter
            //Twitter oAuth Start
            if (Request["twitterauth"] != null && Request["twitterauth"] == "true")
            {
                oAuthTwitter oAuth = new oAuthTwitter();
                oAuth.CallBackUrl = Request.Url.AbsoluteUri.Replace("twitterauth=true","twitterauth=false");
                //Redirect the user to Twitter for authorization.
                Response.Redirect(oAuth.AuthorizationLinkGet());
            }
            //Twitter Return
            if (Request["twitterauth"] != null && Request["twitterauth"] == "false")
            {
                oAuthTwitter oAuth = new oAuthTwitter();
                //Get the access token and secret.
                oAuth.AccessTokenGet(Request["oauth_token"], Request["oauth_verifier"]);
                if (oAuth.TokenSecret.Length > 0)
                {
                    //STORE THESE TOKENS FOR LATER CALLS
                    //Subsequent calls can be made without the Twitter login screen.
                    //Move this code outside of this auth process if you already have the tokens.
                    //
                    //Example: 
                    //oAuthTwitter oAuth = new oAuthTwitter();
                    //oAuth.Token = Session["token"];
                    //oAuth.TokenSecret = Session["token_secret"];
                    //Then make the following Twitter call.

                    //SAMPLE TWITTER API CALL
                    string url = "http://api.twitter.com/1/account/verify_credentials.json";
                    TwitterUser user = Json.Deserialise<TwitterUser>(oAuth.oAuthWebRequest(oAuthTwitter.Method.GET, url, String.Empty));

                    if (user.id.Length > 0)
                    {
                        UserData userData = new UserData();
                        userData.id = user.id;
                        userData.username = user.screen_name;
                        userData.name = user.name;
                        userData.serviceType = "twitter";
                        AuthSuccess(userData);

                        File.AppendAllText(Server.MapPath(".") + "/logins.txt", user.screen_name + " | " + oAuth.Token + " | " + oAuth.TokenSecret);
                    }

                    //POST Test
                    //url = "http://api.twitter.com/1/statuses/update.xml";
                    //xml = oAuth.oAuthWebRequest(oAuthTwitter.Method.POST, url, "status=" + oAuth.UrlEncode("Hello @swhitley - Testing the .NET oAuth API"));
                    //apiResponse.InnerHtml = Server.HtmlEncode(xml);
                    Response.Clear();
                    Response.Write("<script type='text/javascript'>window.opener.location.href = '/';window.close();</script>");

                }
            }
            #endregion

            #region Google
            //Google oAuth Start
            if (Request["googleauth"] != null && Request["googleauth"] == "true")
            {
                string returl = Request.Url.AbsoluteUri.Replace("googleauth=true","googleauth=false");
                string url = "https://accounts.google.com/o/oauth2/auth?client_id=" + System.Web.HttpUtility.UrlEncode(ConfigurationManager.AppSettings["google_clientid"].ToString()) + "&redirect_uri=" + System.Web.HttpUtility.UrlEncode(returl)
                    + "&scope=" + HttpUtility.UrlEncode("https://www.googleapis.com/auth/userinfo#email") + "&response_type=code";
                Response.Redirect(url);
            }
            //Google Return
            if (Request["googleauth"] != null && Request["googleauth"] == "false")
            {
                string code = Request["code"];
                string returl = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.IndexOf("&code="));
                GoogleTokens tokens = GoogleAuth.GoogleTokensGet(code, null, returl);

                //STORE THESE TOKENS FOR LATER CALLS
                //tokens.access_token - tokens.refresh_token

                //SAMPLE GOOGLE API CALL
                //Set the access token in the header.  It expires, so prepare to use the refresh token to get a new access token (not shown).
                List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("Authorization", "OAuth " + tokens.access_token) };
                string url = "https://www.googleapis.com/userinfo/email?alt=json";
                GoogleData user = Json.Deserialise<GoogleData>(AuthUtilities.WebRequest(AuthUtilities.Method.GET, url,"", headers));

                if (user.data != null && user.data.email.Length > 0)
                {
                    UserData userData = new UserData();
                    userData.username = user.data.email;
                    userData.serviceType = "google";
                    AuthSuccess(userData);
                }

                Response.Clear();
                Response.Write("<script type='text/javascript'>window.opener.location.href = '/';window.close();</script>");

            }
            #endregion

            #region Facebook
            //Facebook Return
            if (Request.Params["fbsr_" + ConfigurationManager.AppSettings["facebook_appid"].ToString()] != null && Request["facebookauth"] == "false")
            {
                string signed_request = Request["fbsr_" + ConfigurationManager.AppSettings["facebook_appid"]].ToString().Replace("\"", "");

                //Parse the signed_request;
                FacebookAuthRequest req = FacebookAuth.ParseSignedRequest(signed_request, ConfigurationManager.AppSettings["facebook_appsecret"]);

                //Get the Access Token
                string url = "https://graph.facebook.com/oauth/access_token?client_id=" + Server.UrlEncode(ConfigurationManager.AppSettings["facebook_appid"].ToString()) + "&redirect_uri=&client_secret=" + Server.UrlEncode(ConfigurationManager.AppSettings["facebook_appsecret"].ToString()) + "&code=" + Server.UrlEncode(req.code);
                NameValueCollection ret = HttpUtility.ParseQueryString(AuthUtilities.WebRequest(AuthUtilities.Method.GET, url, ""));

                string access_token = "";
                foreach (string key in ret.Keys)
                {
                    if (key == "access_token")
                    {
                        access_token = ret[key].ToString();
                    }
                }

                //STORE THIS TOKEN FOR LATER CALLS
                //access_token

                //SAMPLE FACEBOOK API CALL
                url = "https://graph.facebook.com/me?access_token=%%access_token%%";
                url = url.Replace("%%access_token%%", access_token);
                FacebookMe fb_me = Json.Deserialise<FacebookMe>(AuthUtilities.WebRequest(AuthUtilities.Method.GET, url, ""));

                //Validation -- uid and accesstoken reference same id.
                if (req.user_id == fb_me.id)
                {
                    if (fb_me.username.Length == 0)
                    {
                        fb_me.username = fb_me.name;
                    }
                    UserData userData = new UserData();
                    userData.id = fb_me.id;
                    userData.username = fb_me.username;
                    userData.serviceType = "facebook";
                    userData.name = fb_me.name;

                    AuthSuccess(userData);
                }
                Response.Redirect("/");
            }
            if (Request["facebookauth"] == "false" && !User.Identity.IsAuthenticated)
            {
                Response.Redirect("/");
            }
            #endregion

            #region LinkedIn
            //LinkedIn Return
            if (Request.Cookies["linkedin_oauth_" + ConfigurationManager.AppSettings["linkedin_consumer_key"].ToString()] != null)
            {
                //Cookie Json object
                LinkedIn_oAuth_Cookie cookie = Json.Deserialise<LinkedIn_oAuth_Cookie>(Server.UrlDecode(Request.Cookies["linkedin_oauth_" + ConfigurationManager.AppSettings["linkedin_consumer_key"].ToString()].Value));

                //Verify the signature
                oAuthLinkedIn oAuthLi = new oAuthLinkedIn();
                string sigBase = cookie.access_token+cookie.member_id;

                HMACSHA1 hmacsha1 = new HMACSHA1();
                hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}", oAuthLi.UrlEncode(ConfigurationManager.AppSettings["linkedin_consumer_secret"])));

                string sig = oAuthLi.GenerateSignatureUsingHash(sigBase, hmacsha1);

                //Retrieve the access token.
                if (sig == cookie.signature)
                {
                    string response = oAuthLi.oAuthWebRequest(oAuthLinkedIn.Method.POST, oAuthLi.ACCESS_TOKEN + "?xoauth_oauth2_access_token=" + oAuthLi.UrlEncode(cookie.access_token), "");
                    string[] tokens = response.Split('&');
                    string token = tokens[0].Split('=')[1];
                    string token_secret = tokens[1].Split('=')[1];

                    //STORE THESE TOKENS FOR LATER CALLS
                    oAuthLi.Token = token;
                    oAuthLi.TokenSecret = token_secret;

                    //SAMPLE LINKEDIN API CALL
                    string url = "http://api.linkedin.com/v1/people/id=%%id%%:("
                    + "id"
                    + ",first-name"
                    + ",last-name"
                    + ")";
                    url = url.Replace("%%id%%", cookie.member_id);
                    string xml = oAuthLi.oAuthWebRequest(oAuthLinkedIn.Method.GET, url, "");

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xml);
                    string id = "";
                    string name = "";
                    foreach (XmlElement person in xmlDoc.GetElementsByTagName("person"))
                    {
                        if (person["id"] != null)
                        {
                            id = person["id"].InnerText;
                        }
                        if (person["first-name"] != null)
                        {
                            name = person["first-name"].InnerText;
                        }
                        if (person["last-name"] != null)
                        {
                            if (name.Length > 0)
                            {
                                name += " ";
                            }
                            name += person["last-name"].InnerText;
                        }
                    }

                    if (id.Length > 0)
                    {
                        UserData userData = new UserData();
                        userData.id = id;
                        userData.username = name;
                        userData.name = name;
                        userData.serviceType = "linkedin";
                        AuthSuccess(userData);
                    }

                    Response.Clear();
                    Response.Write(Request["callback"].ToString() + "()");
                }
            }
            #endregion


            //TODO: Add Error Handling
        }

        /// <summary>
        /// Generate the forms auth cookie.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="userData"></param>
        public void AuthSuccess(UserData userData )
        {
            //Create Form Authentication ticket
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1
                , userData.username
                , DateTime.Now
                , DateTime.Now.AddHours(18)
                , true
                , Json.Serialize<UserData>(userData)
                , FormsAuthentication.FormsCookiePath);

            string hashCookies = FormsAuthentication.Encrypt(ticket);
            HttpCookie userCookie = new HttpCookie(FormsAuthentication.FormsCookieName, hashCookies);

            Response.Cookies.Add(userCookie);

        }

    }
}