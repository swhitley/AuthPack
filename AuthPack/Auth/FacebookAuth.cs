using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Cryptography;

namespace AuthPack
{
    public static class FacebookAuth
    {
        public static FacebookAuthRequest ParseSignedRequest(string signed_request, string secret)
        {
            FacebookAuthRequest req = new FacebookAuthRequest();

            string[] parms = signed_request.Split('.');
            var encoded_sig = parms[0];

            if (encoded_sig.Length > 0)
            {
                string sig = Base64UrlDecode(encoded_sig);
                string payload = parms[1];

                using (HMACSHA256 crypto = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
                {
                    string expected_sig = Base64UrlDecode(Convert.ToBase64String(crypto.ComputeHash(Encoding.UTF8.GetBytes(payload))));
                    if (expected_sig == sig)
                    {
                        string json = Encoding.UTF8.GetString(Convert.FromBase64String(Base64UrlDecode(payload)));
                        req = Json.Deserialise<FacebookAuthRequest>(json);
                    }
                }

            }

            return req;
        }

        /// <summary>
        /// Converts the base 64 url encoded string to standard base 64 encoding.
        /// </summary>
        /// <param name="encodedValue">The encoded value.</param>
        /// <returns>The base 64 string.</returns>
        private static string Base64UrlDecode(string encodedValue)
        {
            encodedValue = encodedValue.Replace('+', '-').Replace('/', '_').Trim();
            int pad = encodedValue.Length % 4;
            if (pad > 0)
            {
                pad = 4 - pad;
            }

            encodedValue = encodedValue.PadRight(encodedValue.Length + pad, '=');
            return encodedValue;
        }


    }

    public class FacebookAuthRequest
    {
        [DataMember]
        public FacebookAuthUser user;
        [DataMember]
        public string algorithm;
        [DataMember]
        public string issued_at;
        [DataMember]
        public string user_id;
        [DataMember]
        public string oauth_token;
        [DataMember]
        public string code;
        [DataMember]
        public string expires;
        [DataMember]
        public string app_data;
        [DataMember]
        public FacebookAuthPage page;
        [DataMember]
        public string profile_id;
    }

    public class FacebookAuthUser
    {
        [DataMember]
        public string locale;
        [DataMember]
        public string country;
        [DataMember]
        public FacebookAuthAge age;
    }
    public class FacebookAuthAge
    {
        [DataMember]
        public int min;
        [DataMember]
        public int max;
    }
    public class FacebookAuthPage
    {
        [DataMember]
        public string id;
        [DataMember]
        public bool liked;
        [DataMember]
        public bool admin;
    }
}