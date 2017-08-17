using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Sparkle.LinkedInNET;
using Sparkle.LinkedInNET.OAuth2;
using Sparkle.LinkedInNET.Profiles;

namespace LinkedInApiTest
{
    public class LinkedInApiTest
    {
        private UserAuthorization user;
        private AuthorizationAccessToken userToken;

        public LinkedInApi API { get; }

        public Person Person { get; set; }

        public LinkedInApiTest(LinkedInApiConfiguration configeSettings)
        {
            API = new LinkedInApi(configeSettings);
        }

        public Uri GetAuthorizationUrl(NameValueCollection configuSettings, string redirectURL)
        {
            var scope = GetAuthorizationScopeFromConfig(configuSettings);
            var state = Guid.NewGuid().ToString();
            return API.OAuth2.GetAuthorizationUrl(scope, state, redirectURL);
        }

        public void LoadUserProfileData(string accessCode, string redirectURL)
        {
            // keep this token for your API requests
            userToken = API.OAuth2.GetAccessToken(accessCode, redirectURL);
            user = new UserAuthorization(userToken.AccessToken);

            //Fields to retrive
            /*var fields = FieldSelector.For<Person>()
                .WithId()
                .WithFirstName()
                .WithLastName()
                .WithFormattedName()
                .WithEmailAddress()
                .WithHeadline()

                .WithLocationName()
                .WithLocationCountryCode()

                .WithPictureUrl()
                .WithPublicProfileUrl()
                .WithSummary()
                .WithIndustry()

                .WithPositions()
                .WithPositionsSummary()
                .WithThreeCurrentPositions()
                .WithThreePastPositions()

                .WithProposalComments()
                .WithAssociations()
                .WithInterests()
                .WithLanguageId()
                .WithLanguageName()
                .WithLanguageProficiency()
                .WithCertifications()
                .WithEducations()
                .WithFullVolunteer()
                .WithPatents()
                .WithRecommendationsReceived()
                .WithRecommendationsReceivedWithAdditionalRecommenderInfo()

                .WithDateOfBirth()
                .WithPhoneNumbers()
                .WithImAccounts()
                .WithPrimaryTwitterAccount()
                .WithTwitterAccounts()
                .WithSkills();
			*/

            try
            {
                Person = API.Profiles.GetMyProfile(user, GetAcceptLanguages());
            }
            catch (LinkedInApiException ex) // one exception type to handle
            {
                // ex.Message
                // ex.InnerException // WebException
                // ex.Data["ResponseStream"]
                // ex.Data["HttpStatusCode"]
                // ex.Data["Method"]
                // ex.Data["UrlPath"]
                // ex.Data["ResponseText"]
            }
        }

        /// <summary>
        /// Save person fields to csv
        /// </summary>
        public void SaveUserDataAsCVSFile(string directory, string fileName)
        {
            const string sep = ",";

            var sb = new StringBuilder();
            var type = Person.GetType();
            var properties = new List<PropertyInfo>(type.GetProperties());

            //Save data to StringBuilder
            sb.AppendLine(string.Join(sep, properties.Select(p => p.Name)));
            sb.AppendLine(string.Join(sep, properties.Select(p => p.GetValue(Person, null))));

            //Save csv file
            File.WriteAllText(Path.Combine(directory, fileName), sb.ToString());
        }

        /// <summary>
        /// Get AuthorizationScope from config file
        /// </summary>
        private static AuthorizationScope GetAuthorizationScopeFromConfig(NameValueCollection configuSettings)
        {
            var scopes = configuSettings["AuthorizationScope"].Split(new[]
            {
                "|",
                " "
            }, StringSplitOptions.RemoveEmptyEntries);
            if (scopes.Length == 0)
            {
                throw new ArgumentException("Configuration file does not contains AuthorizationScope setting.");
            }
            var authScope = ParseEnum(scopes[0]);
            for (var i = 1; i < scopes.Length; i++)
            {
                authScope = authScope | ParseEnum(scopes[i]);
            }
            return authScope;
        }

        /// <summary>
        /// Parse enum from string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static AuthorizationScope ParseEnum(string value)
        {
            return (AuthorizationScope)Enum.Parse(typeof(AuthorizationScope), value, true);
        }

        /// <summary>
        /// Get list of accepted languages to retrive profile information
        /// </summary>
        /// <returns></returns>
        private static string[] GetAcceptLanguages()
        {
            return new[] { "en-US", "ru-RU" };
        }
    }
}
