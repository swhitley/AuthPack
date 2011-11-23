using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AuthPack
{
    public class FacebookData
    {

    }
    [DataContract]
    public class FacebookMe
    {
        [DataMember]
        public string id;
        [DataMember]
        public string username;
        [DataMember]
        public string name;
    }
}