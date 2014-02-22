using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace AuthPack
{
    public static class AuthUtilities
    {
        public enum Method { GET, POST, PUT, DELETE };

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public static string WebRequest(Method method, string url, string postData)
        {
            return WebRequest(method, url, postData, null);
        }


        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <param name="headers">Additional Header Data</param>
        /// <returns>The web server response.</returns>
        public static string WebRequest(Method method, string url, string postData, List<KeyValuePair<string,string>> headers)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            if(headers != null)
            {
                foreach(KeyValuePair<string,string> header in headers)
                {
                    webRequest.Headers.Add(header.Key, header.Value);
                }
            }
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            //webRequest.UserAgent  = "Identify your application please.";
            //webRequest.Timeout = 20000;

            if (method == Method.POST || method == Method.DELETE)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";

                //POST the data.
                using (requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(postData);
                    requestWriter.Close();
                }
            }

            responseData = WebResponseGet(webRequest);

            webRequest = null;

            return responseData;

        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        public static string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    responseReader = new StreamReader(ex.Response.GetResponseStream());
                    //Read the response.
                    string innerResponseData = responseReader.ReadToEnd();
                    if (innerResponseData.Trim().Length > 0)
                    {
                        responseData = innerResponseData;
                    }
                }

                if (responseReader != null)
                {
                    responseReader.Close();
                }
                if (webRequest != null)
                {
                    try
                    {
                        webRequest.GetResponse().Close();
                    }
                    catch { }
                }

                throw new Exception(responseData);
            }
            catch (Exception ex)
            {
                //TODO: Improve error handling
                responseData = ex.Message;

                if (responseReader != null)
                {
                    responseReader.Close();
                }
                if (webRequest != null)
                {
                    try
                    {
                        webRequest.GetResponse().Close();
                    }
                    catch { }
                }
            }

            //Release variables.
            responseReader = null;
            webRequest = null;

            return responseData;
        }
    }

    //Generic UserData for Forms Auth Cookie
    [DataContract]
    public class UserData
    {
        [DataMember]
        public string id;
        [DataMember]
        public string username;
        [DataMember]
        public string name;
        [DataMember]
        public string serviceType;
        [DataMember]
        public string imageUrl;
    }
}