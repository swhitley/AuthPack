using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AuthPack
{
    [DataContract]
    public class AppDotNetPostWrapper
    {
        [DataMember]
        public AppDotNetPost data { get; set; }
        [DataMember]
        public AppDotNetWrapperMeta meta { get; set; }
    }
    [DataContract]
    public class AppDotNetPost
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public DateTime created_at { get; set; }
        [DataMember]
        public AppDotNetUser user { get; set; }
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string html { get; set; }
        [DataMember]
        public Source source { get; set; }
        [DataMember]
        public string reply_to { get; set; }
        [DataMember]
        public string thread_id { get; set; }
        [DataMember]
        public int num_replies { get; set; }
        [DataMember]
        public string deleted { get; set; }
        //TODO: Add other properties.
    }
    [DataContract]
    public class Source
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string link { get; set; }
    }
}