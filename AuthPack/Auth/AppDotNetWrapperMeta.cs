using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AuthPack
{
    [DataContract]
    public class AppDotNetWrapperMeta
    {
        [DataMember]
        public string code { get; set; }
        [DataMember(Name = "invalid-token")]
        public string invalidToken { get; set; }
        [DataMember(Name = "not-authorized")]
        public string notAuthorized { get; set; }
        [DataMember(Name = "token-expired")]
        public string tokenExpired { get; set; }
        [DataMember(Name = "code-used")]
        public string codeUsed { get; set; }
        [DataMember(Name = "redirect-uri-required")]
        public string redirectUriRequired { get; set; }
    }
}