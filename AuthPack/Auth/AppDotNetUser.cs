using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AuthPack
{
    [DataContract]
    public class AppDotNetUserWrapper
    {
        [DataMember]
        public AppDotNetUser data { get; set; }
        [DataMember]
        public AppDotNetWrapperMeta meta { get; set; }
    }
    [DataContract]
    public class AppDotNetUser
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public Description description { get; set; }
        [DataMember]
        public AvatarImage avatar_image { get; set; }
        [DataMember]
        public bool follows_you { get; set; }
        [DataMember]
        public bool you_follow { get; set; }
        [DataMember]
        public bool you_muted { get; set; }
        [DataMember]
        public Counts counts { get; set; }
        //TODO: Add other properties.
    }
    [DataContract]
    public class Description
    {
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string html { get; set; }
    }
    [DataContract]
    public class AvatarImage
    {
        [DataMember]
        public int height { get; set; }
        [DataMember]
        public int width { get; set; }
        [DataMember]
        public string url { get; set; }
    }
    [DataContract]
    public class Counts
    {
        [DataMember]
        public int followers { get; set; }
        [DataMember]
        public int following { get; set; }
        [DataMember]
        public int posts { get; set; }
    }
}