using System;
using System.Web.UI;
using Sparkle.LinkedInNET;

namespace LinkedInApiTest
{
    public partial class UserLandings : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // get the APIs client
            var api = new LinkedInApiTest(LinkedInApiConfiguration.FromAppSettings("MyTest.LinkedInConnect"));
            try
            {
                var url = api.GetAuthorizationUrl(WebFormLinkedInTest.WebConfigurationSettings, WebFormLinkedInTest.RedirectURL);
                Response.Redirect(url.OriginalString);
            }
            catch (Exception ex)
            {
                conditionalLabel.Text = ex.Message;
            }
        }
    }
}