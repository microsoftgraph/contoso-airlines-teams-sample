/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Graph;
using ContosoAirlines.Helpers;
using ContosoAirlines.Models;
using Resources;
using System.Configuration;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ContosoAirlines.Controllers
{
    public class Flight
    {
        public string tenantName = "M365x165177.onmicrosoft.com";
        public string prototypeTeamId = "64b9c746-b155-4042-8d47-941bd381681b";
        public string number = "157";

        public string captain;
        public string[] crew;
        public string admin;

        public Flight(string admin)
        {
            captain = $"debraB@{tenantName}";
            crew = new string[] {
                $"isaiahl@{tenantName}",
                $"leeg@{tenantName}",
            };
            this.admin = admin;
        }
    }

    // Used only for de-serializing JSON
    public class TokenResponse
    {
        public string access_token { get; set; }
    }


    public class HomeController : Controller
    {
        GraphService graphService = new GraphService();
        public static bool useAppPermissions = true; // hack
        private static string adminUpn = null;

        public ActionResult About()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> Index(RootModel rootModel)
        {
            try
            {
                if (adminUpn == null)
                {
                    // always user delegated
                    graphService.accessToken = await SampleAuthProvider.Instance.GetUserAccessTokenAsync();
                    HomeController.adminUpn = await graphService.GetAdminUpn();
                }

                return DefaultView(rootModel);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
                return RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + se.Error.Message });
            }
        }

        private ViewResult DefaultView(RootModel rootModel)
            => View("Graph", rootModel);

        public ActionResult UseUserDelegatedPermissions(RootModel rootModel)
        {
            HomeController.useAppPermissions = false;
            rootModel.UseAppPermissions = false;
            return DefaultView(rootModel);
        }

        public ActionResult UseApplicationPermissions(RootModel rootModel)
        {
            HomeController.useAppPermissions = true;
            rootModel.UseAppPermissions = true;
            return DefaultView(rootModel);
        }

        public static string AdminConsentPromptUrl()
        {
            string appId = ConfigurationManager.AppSettings["ida:AppId"];
            string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];

            string adminConsentPrompt = $"https://login.microsoftonline.com/common/adminconsent?client_id={appId}&state=12345&redirect_uri={redirectUri}";
            return adminConsentPrompt;
        }
        
        private async Task<string> GetToken(RootModel rootModel)
        {
            string token;
            // if (rootModel.UseAppPermissions)
            if (HomeController.useAppPermissions)
            {
                string appId = ConfigurationManager.AppSettings["ida:AppId"];
                string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
                string appSecret = ConfigurationManager.AppSettings["ida:AppSecret"];

                string tenant = "M365x165177.onmicrosoft.com";
                string response = await HttpHelpers.POST($"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token",
                      $"grant_type=client_credentials&client_id={appId}&client_secret={appSecret}"
                      + "&scope=https%3A%2F%2Fgraph.microsoft.com%2F.default");
                token = response.Deserialize<TokenResponse>().access_token;
            }
            else
            {
                token = await SampleAuthProvider.Instance.GetUserAccessTokenAsync();
            }
            graphService.accessToken = token;
            return token;
        }

        [Authorize]
        public async Task<ActionResult> CreateTeam(RootModel rootModel)
        {
            try
            {
                string accessToken = await GetToken(rootModel);
                var tuple = await graphService.CreateTeam(new Flight(adminUpn));
                string groupId = tuple.Item1;
                string link = tuple.Item2;

                lastGroupCreated = groupId;

                // Reset the status to display when the page reloads.
                ViewBag.CreateTeamDone = "Enable";
                ViewBag.Url = link;

                return DefaultView(rootModel);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
                return RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + se.Error.Message });
            }
        }

        [Authorize]
        public async Task<ActionResult> ArchiveAllTeams(RootModel rootModel)
        {
            try
            {
                string accessToken = await GetToken(rootModel);
                await graphService.ArchiveAllTeams();

                // Reset the status to display when the page reloads.
                ViewBag.CreateTeamDone = "Enable";

                return DefaultView(rootModel);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
                return RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + se.Error.Message });
            }
        }

        [Authorize]
        public async Task<ActionResult> InstallAppToAllTeams(RootModel rootModel)
        {
            try
            {
                string accessToken = await GetToken(rootModel);
                await graphService.InstallAppToAllTeams();

                // Reset the status to display when the page reloads.
                ViewBag.CreateTeamDone = "Enable";

                return DefaultView(rootModel);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
                return RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + se.Error.Message });
            }
        }

        [Authorize]
        public async Task<ActionResult> BulkDelete(RootModel rootModel)
        {
            try
            {
                string accessToken = await GetToken(rootModel);
                await graphService.BulkDelete();

                // Reset the status to display when the page reloads.
                ViewBag.CreateTeamDone = "Enable";

                return DefaultView(rootModel);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
                return RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + se.Error.Message });
            }
        }

        [Authorize]
        public async Task<ActionResult> ArchiveTeam(RootModel rootModel)
        {
            try
            {
                string accessToken = await GetToken(rootModel);
                Flight flight = new Flight(adminUpn);
                string groupId = await graphService.ArchiveTeam(lastGroupCreated);

                ViewBag.CreateTeamDone = "Enable";
                return DefaultView(rootModel);
            }
            catch (Exception e)
            {
                if (e.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
                return RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + e.Message });
            }
        }

        [Authorize]
        public async Task<ActionResult> CloneTeam(RootModel rootModel)
        {
            try
            {
                string accessToken = await GetToken(rootModel);
                Flight flight = new Flight(adminUpn);
                string url = await graphService.CreateTeamUsingClone(flight);

                ViewBag.CreateTeamDone = "Enable";
                ViewBag.Url = url;
                return DefaultView(rootModel);
            }
            catch (Exception e)
            {
                if (e.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
                return RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + e.Message });
            }
        }

        // only through CreateTeam -- Clone doesn't count
        public static string lastGroupCreated = null;

        // Application context
        public async Task<ActionResult> StartSyncService(RootModel rootModel)
        {
            try
            {
                string accessToken = await GetToken(rootModel);

                string teamId = "f11e383b-5637-4df7-a470-afae0cc0b98b";
                string[] vipNames = new string[] { "Andrew Bybee", "Larry Jin", "Bill Bliss", "Fred Flintstone", "Homer Simpson", "John Doe" };

                foreach (string vip in vipNames)
                {
                    await graphService.CreateChannel(teamId, vip);
                    Thread.Sleep(15000);
                }

                //// Reset the status to display when the page reloads.
                ViewBag.CreateTeamDone = "Enable";

                return DefaultView(rootModel);
            }
            catch (Exception e)
            {
                if (e.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
                return RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + e.Message });
            }
        }

        public async Task<ActionResult> RemoveVIPs(RootModel rootModel)
        {
            try
            {
                string tenant = "M365x165177.onmicrosoft.com";
                string appId = ConfigurationManager.AppSettings["ida:AppId"];
                string appSecret = ConfigurationManager.AppSettings["ida:AppSecret"];

                string response = await HttpHelpers.POST($"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token",
                      $"grant_type=client_credentials&client_id={appId}&client_secret={appSecret}"
                      + "&scope=https%3A%2F%2Fgraph.microsoft.com%2F.default");
                string accessToken = response.Deserialize<TokenResponse>().access_token;
                graphService.accessToken = accessToken;

                string teamId = "f11e383b-5637-4df7-a470-afae0cc0b98b";
                await graphService.DeleteAllChannel(teamId);

                return DefaultView(rootModel);
            }
            catch (Exception e)
            {
                if (e.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
                return RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + e.Message });
            }
        }

        [Authorize]
        public async Task<ActionResult> Analytics(RootModel rootModel)
        {
            try
            {
                string accessToken = await GetToken(rootModel);
                return View("WordCloud");
            }
            catch (Exception e)
            {
                if (e.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
                return RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + e.Message });
            }
        }

        public async Task<FileContentResult> WordCloudImage(RootModel rootModel)
        {
            //string accessToken = await GetToken(rootModel);
            //string text = await graphService.GetChannelText("4e3455e3-64c6-48f3-b9ac-316c3dceedd3", "19:954c94bd72e4413a80f8011828e31874@thread.skype");
            //text = text.Replace("<div>", "");

            ////string text = System.IO.File.ReadAllText(@"C:\Users\nkramer\source\repos\WordCloudDemo\Messages\1.txt");
            //string[] badwords =
            //    "to the and for a in you your of new with can on or as from now that will be this we is have are if into also an full use center it add by all both help set please view us make youre start only get using out div nbsp nbspdiv"
            //    .Trim().Split(' ');
            ////System.IO.File.ReadAllText(@"C:\Users\nkramer\source\repos\WordCloudDemo\Messages\badwords.txt").Trim().Split(' ');

            //text = text.ToLower();
            //text = new string((from c in text
            //                   where char.IsLetter(c) || char.IsWhiteSpace(c)
            //                   select char.IsWhiteSpace(c) ? ' ' : c).ToArray());
            //var words = text.Split(' ').Where(word => word != "").ToArray();

            //Dictionary<string, int> wordCount = words
            //    .GroupBy(word => word)
            //    .ToDictionary(group => group.Key, group => group.Count());
            //var orderedWords =
            //    (from w in wordCount.Keys
            //     where !badwords.Contains(w)
            //     orderby wordCount[w] descending
            //     select w).Take(50).ToArray();

            //var finalWords = string.Join(" ", orderedWords);
            //var finalWordCount = orderedWords.Select(w => wordCount[w]).ToArray();

            //var img = new WordCloud.WordCloud(600, 600).Draw(orderedWords.ToList(), finalWordCount.ToList());

            //var stream = new MemoryStream();
            //img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            //var bytes = stream.ToArray();
            //return File(bytes, "image/jpeg");
            return null;
        }
    }
}