using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System;
using System.Web.Mvc;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.YouTube.v3;
using Google.Apis.Util.Store;

namespace YoutubeInterestSC
{
    public class AppFlowMetadata : FlowMetadata
    {
        private static readonly IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "1091996968324-gmpcveo6mc3ptgl4fmthr3n49tb73f81.apps.googleusercontent.com",
                    ClientSecret = "vU6QcV020r9kUfgEYsXeZfJB"
                },
                Scopes = new[] { YouTubeService.Scope.YoutubeReadonly, YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeForceSsl, YouTubeService.Scope.Youtubepartner, YouTubeService.Scope.YoutubeUpload },
                DataStore = new FileDataStore("Drive.Api.Auth.Store")
            });

        public override string GetUserId(Controller controller)
        {
            // In this sample we use the session to store the user identifiers.
            // That's not the best practice, because you should have a logic to identify
            // a user. You might want to use "OpenID Connect".
            // You can read more about the protocol in the following link:
            // https://developers.google.com/accounts/docs/OAuth2Login.
            var user = controller.Session["user"];
            if (user == null)
            {
                user = Guid.NewGuid();
                controller.Session["user"] = user;
            }
            return user.ToString();

        }

        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }
    }
}