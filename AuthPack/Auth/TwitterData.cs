using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AuthPack
{
    public class TwitterData
    {
    }
    [DataContract]
    public class TwitterUser
    {
        [DataMember]
        public string id;
        [DataMember]
        public string screen_name;
        [DataMember]
        public string name;
        [DataMember]
        public string profile_image_url;
    }
}