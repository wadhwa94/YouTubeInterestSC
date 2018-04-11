using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;

namespace YoutubeInterestSC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> YouTubeInterest( int videoCount, CancellationToken cancellationToken)
        {
            var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
                AuthorizeAsync(cancellationToken);

            if (result.Credential != null)
            {
                var ytService = new YouTubeService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = result.Credential,
                    ApplicationName = "ASP.NET MVC Sample"
                });

                // YOUR CODE SHOULD BE HERE..
                // SAMPLE CODE:
                //var playlistRequest = ytservice.Playlists.List("snippet");
                //playlistRequest.Mine = true;
                //var response = await playlistRequest.ExecuteAsync();

                Dictionary<string, int> VidCategory_Array = new Dictionary<string, int>();
                string message = "";
                try
                {
                    var channelsRequest = ytService.Channels.List("contentDetails");
                    channelsRequest.Mine = true;
                    var likePlaylistIIdResponse = channelsRequest.Execute();
                    Google.Apis.YouTube.v3.Data.PlaylistItemListResponse videosResponse = null;
                    int currentVideoCount = 0;
                    if (likePlaylistIIdResponse.Items.Count > 0)
                    {
                        string likePlayListId = likePlaylistIIdResponse.Items[0].ContentDetails.RelatedPlaylists.Likes;
                        string nextPagetoken = null;
                        do
                        {
                            var videosRequest = ytService.PlaylistItems.List("snippet");
                            videosRequest.PlaylistId = likePlayListId;
                            if (videoCount != -1)
                                videosRequest.MaxResults = Math.Min(videoCount, 50);
                            else
                                videosRequest.MaxResults = 50;
                            if (nextPagetoken != null)
                            {
                                if (videosResponse.NextPageToken != null)
                                {
                                    videosRequest.PageToken = videosResponse.NextPageToken;
                                }
                            }
                            videosResponse = videosRequest.Execute();
                            currentVideoCount += videosResponse.Items.Count;
                            for (int i = 0; i < videosResponse.Items.Count; i++)
                            {

                                var videoId = videosResponse.Items[i].Snippet.ResourceId.VideoId; //itearate for all the videos

                                var videoDetails = ytService.Videos.List("snippet");
                                videoDetails.Id = videoId;
                                var videoDetailsResponse = videoDetails.Execute();
                                if (videoDetailsResponse.Items.Count > 0)
                                {
                                    var videoCategoryId = videoDetailsResponse.Items[0].Snippet.CategoryId;
                                    var videoCategoryNameRequest = ytService.VideoCategories.List("snippet");
                                    videoCategoryNameRequest.Id = videoCategoryId;
                                    var videoCategoryNameResponse = videoCategoryNameRequest.Execute();
                                    var videoCategoryName = videoCategoryNameResponse.Items[0].Snippet.Title;

                                    if (VidCategory_Array.ContainsKey(videoCategoryName))
                                    {
                                        VidCategory_Array[videoCategoryName] += 1;
                                    }
                                    else
                                    {
                                        VidCategory_Array.Add(videoCategoryName, 1);
                                    }
                                }

                            }

                            if (videosResponse.NextPageToken != null)
                                nextPagetoken = videosResponse.NextPageToken;
                        } while (videosResponse.NextPageToken != null && videoCount == -1);
                    }
                    
                    ViewBag.Message = "response IS: " + VidCategory_Array.ToString();
                    //                    return VidCategory_Array;
                }
                catch (Exception e)
                {
                    ViewBag.Message = "response IS: " + VidCategory_Array.ToString();
                }
                return View();
            }
            //var list = await service.Files.List().ExecuteAsync();



            else
            {
                return new RedirectResult(result.RedirectUri);
            }
        }
    }
}