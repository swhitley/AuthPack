using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AuthPack
{
    public class oAuthLinkedIn:oAuthTwitter
    {
        public override string REQUEST_TOKEN
        {
            get
            {
                return "https://api.linkedin.com/uas/oauth/requestToken";
            }
        }
        public override string AUTHORIZE
        {
            get
            {
                return "https://api.linkedin.com/uas/oauth/authorize";
            }
        }
        public override string ACCESS_TOKEN
        {
            get
            {
                return "https://api.linkedin.com/uas/oauth/accessToken";
            }
        }
        public override string Service
        {
            get
            {
                return "linkedin";
            }
        }
    }

    [DataContract]
    public class LinkedIn_oAuth_Cookie
    {
        [DataMember]
        public string signature_version;
        [DataMember]
        public string signature_method;
        [DataMember]
        public string[] signature_order;
        [DataMember]
        public string access_token;
        [DataMember]
        public string signature;
        [DataMember]
        public string member_id;
    }
}