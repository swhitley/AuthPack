using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Security.Principal;
using System.Web.Security;

namespace AuthPack
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IPrincipal User = HttpContext.Current.User;

            if (User.Identity.IsAuthenticated)
            {
                //Display a profile picture for Twitter or Facebook users.
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                UserData userData = Json.Deserialise<UserData>(authTicket.UserData);
                Image profileImage = (Image) HeadLoginView.FindControl("profileImage");
                if (userData.serviceType == "twitter")
                {
                    profileImage.ImageUrl = "http://api.twitter.com/1/users/profile_image/" + userData.username;
                    profileImage.AlternateText = userData.name;
                    profileImage.Visible = true;
                }
                if (userData.serviceType == "facebook")
                {
                    profileImage.ImageUrl = "http://graph.facebook.com/" + userData.id + "/picture/";
                    profileImage.AlternateText = userData.name;
                    profileImage.Visible = true;
                }
            }
        }
    }
}
