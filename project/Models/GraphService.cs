/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using ContosoAirlines.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Graph;

namespace ContosoAirlines.Models
{
    public class GraphService : HttpHelpers
    {
        public async Task<User> GetUserFromUpn(string upn)
        {
            var graph = GetAuthenticatedClient();
            var user = await graph.Users[upn].Request().GetAsync();
            return user;
        }

        public async Task<Tuple<string, string>> CreateTeam(Flight flight)
        {
            var graph = GetAuthenticatedClient();

            var owners = new User[] {
                await GetUserFromUpn(flight.captain),
                await GetUserFromUpn(flight.admin)
            };

            var crewTasks = flight.crew.Select(async upn => await GetUserFromUpn(upn)).ToArray();
            var crew = await Task.WhenAll(crewTasks);
            var members = owners.Concat(crew).ToArray();


            var groupDef = new Group()
            {
                DisplayName = "Flight " + flight.number,
                MailNickname = "flight" + GetTimestamp(),
                Description = "Everything about flight " + flight.number,
                Visibility = "Private",
                GroupTypes = new string[] { "Unified" }, // same for all teams
                MailEnabled = true,                      // same for all teams
                SecurityEnabled = false,                 // same for all teams

                AdditionalData = new Dictionary<string, object>()
                {
                    ["owners@odata.bind"] = owners.Select(o => $"{graphV1Endpoint}/users/{o.Id}").ToArray(),
                    ["members@odata.bind"] = members.Select(o => $"{graphV1Endpoint}/users/{o.Id}").ToArray(),
                }
            };

            // Create the modern group for the team
            Group group = await graph.Groups.Request().AddAsync(groupDef);

            // Create the team
            await graph.Groups[group.Id].Team.Request().WithMaxRetry(3).PutAsync(
                new Team()
                {
                    GuestSettings = new TeamGuestSettings()
                    {
                        AllowCreateUpdateChannels = false,
                        AllowDeleteChannels = false
                    },
                }
                );

            string teamId = group.Id; // always the same

            // Create a new channel for pilot talk
            Channel channel = await graph.Teams[teamId].Channels.Request().AddAsync(
                new Channel()
                {
                    DisplayName = "Pilots",
                    Description = "Discussion about flightpath, weather, etc."
                });

            // Add a map tab to the created channel
            await graph.Teams[teamId].Channels[channel.Id].Tabs.Request().AddAsync(
                new TeamsTab()
                {
                    DisplayName = "Map",
                    AdditionalData = new Dictionary<string, object>() { ["teamsApp@odata.bind"] = $"{graphV1Endpoint}/appCatalogs/teamsApps/com.microsoft.teamspace.tab.web" },
                    // It's serialized as "teamsApp@odata.bind" : "{graphV1Endpoint}/appCatalogs/teamsApps/com.microsoft.teamspace.tab.web"
                    Configuration = new TeamsTabConfiguration()
                    {
                        EntityId = null,
                        ContentUrl = "https://www.bing.com/maps/embed?h=800&w=800&cp=47.640016~-122.13088799999998&lvl=16&typ=s&sty=r&src=SHELL&FORM=MBEDV8",
                        WebsiteUrl = "https://binged.it/2xjBS1R",
                        RemoveUrl = null,
                    }
                });

            // Now create a SharePoint list of challenging passengers

            // Get the team site
            var teamSite = await HttpGet<Site>($"/groups/{teamId}/sites/root",
                retries: 3, retryDelay: 30);

            // Create the list
            var list = await graph.Sites[teamSite.Id].Lists.Request().AddAsync(
                                new Microsoft.Graph.List
                                {
                                    DisplayName = "Challenging Passengers",
                                    Columns = new ListColumnsCollectionPage()
                                    {
                                        new ColumnDefinition
                                        {
                                            Name = "Name",
                                            Text = new TextColumn()
                                        },
                                        new ColumnDefinition
                                        {
                                            Name = "SeatNumber",
                                            Text = new TextColumn()
                                        },
                                        new ColumnDefinition
                                        {
                                            Name = "Notes",
                                            Text = new TextColumn()
                                        }
                                    }
                                }
                                );

            await HttpPost($"/sites/{teamSite.Id}/lists/{list.Id}/items",
                challengingPassenger
                );

            // Add the list as a team tab
            await graph.Teams[teamId].Channels[channel.Id].Tabs.Request().AddAsync(
                new TeamsTab
                {
                    DisplayName = "Challenging Passengers",
                    AdditionalData = new Dictionary<string, object>() { ["teamsApp@odata.bind"] = $"{graphV1Endpoint}/appCatalogs/teamsApps/com.microsoft.teamspace.tab.web" }, // Website tab
                    // It's serialized as "teamsApp@odata.bind" : "{graphV1Endpoint}/appCatalogs/teamsApps/com.microsoft.teamspace.tab.web"
                    Configuration = new TeamsTabConfiguration
                    {
                        ContentUrl = list.WebUrl,
                        WebsiteUrl = list.WebUrl
                    }
                });

            return new Tuple<string, string>(group.Id, "");// webUrl);
        }

        public async Task InstallAppToAllTeams()
        {
            var graph = GetAuthenticatedClient();
            string appid = "0fd925a0-357f-4d25-8456-b3022aaa41a9"; // SurveyMonkey
            var teams = (await GetAllTeams()).Where(t => t.DisplayName.StartsWith("Flight 157"))
                .ToArray();
            foreach (var team in teams)
            {
                var t = await graph.Teams[team.Id].Request().GetAsync();
                if (t.IsArchived == false)
                {
                    // See if it's already installed
                    var apps = await graph.Teams[team.Id].InstalledApps.Request().Expand("teamsAppDefinition").GetAsync();
                    if (apps.Where(app => app.TeamsAppDefinition.Id == appid).Count() == 0)
                    {
                        await graph.Teams[team.Id].InstalledApps.Request().AddAsync(
                        new TeamsAppInstallation()
                        {
                            AdditionalData = new Dictionary<string, object>() { ["teamsApp@odata.bind"] = $"{graphV1Endpoint}/appCatalogs/teamsApps/{appid}" }
                        });
                    }
                }
            }
        }

        public async Task<string> CreateTeamUsingClone(Flight flight)
        {
            var response = await HttpPostWithHeaders($"/teams/{flight.prototypeTeamId}/clone",
                new TeamCloneRequestBody()
                {
                    DisplayName = "Flight 4" + flight.number,
                    MailNickname = "flight" + GetTimestamp(),
                    Description = "Everything about flight " + flight.number,
                    Visibility = TeamVisibilityType.Private,
                    PartsToClone = ClonableTeamParts.Apps | ClonableTeamParts.Settings | ClonableTeamParts.Channels,
                });

            string operationUrl = response.Headers.Location.ToString();

            string teamId = null;
            for (; ; )
            {
                TeamsAsyncOperation operation = await HttpGet<TeamsAsyncOperation>(operationUrl);

                if (operation.Status == TeamsAsyncOperationStatus.Failed)
                    throw new Exception();

                if (operation.Status == TeamsAsyncOperationStatus.Succeeded)
                {
                    teamId = operation.TargetResourceId;
                    break;
                }

                Thread.Sleep(10000); // wait 10 seconds between polls
            }

            var graph = GetAuthenticatedClient();

            // Add the crew to the team
            foreach (string upn in flight.crew)
            {
                var user = await GetUserFromUpn(upn);
                await graph.Groups[teamId].Members.References.Request().AddAsync(
                    user);

                if (upn == flight.captain)
                    await graph.Groups[teamId].Owners.References.Request().AddAsync(
                        user);
            }

            // get the webUrl
            Team team = await graph.Teams[teamId].Request().GetAsync();
            string link = team.WebUrl;

            return link;
        }
        
        public async Task ArchiveAllTeams()
        {
            var graph = GetAuthenticatedClient();
            var teams = (await GetAllTeams()).Where(
                t => t.DisplayName.StartsWith("Flight 157") 
                || t.DisplayName.StartsWith("Flight 4157"))
                .ToArray();
            foreach (var team in teams)
            {
                var t = await graph.Teams[team.Id].Request().GetAsync();

                if (!t.IsArchived == true)
                {
                    await ArchiveTeam(team.Id);
                }
            }
        }

        public async Task<string> ArchiveTeam(string teamId)
        {
            var graph = GetAuthenticatedClient();
            HttpResponse response = await HttpPostWithHeaders($"/teams/{teamId}/archive", "{}");
            string operationUrl = response.Headers.Location.ToString();

            for (; ; )
            {
                var operation = await HttpGet<TeamsAsyncOperation>(operationUrl);

                if (operation.Status == TeamsAsyncOperationStatus.Failed)
                    throw new Exception();

                if (operation.Status == TeamsAsyncOperationStatus.Succeeded)
                    break;

                Thread.Sleep(10000); // wait 10 seconds between polls
            }

            return "success";
        }

        // Get all the teams you have access to. For user delegated, look at the joinedTeams.
        // For application permissions, get all teams in the tenant.
        private async Task<Group[]> GetAllTeams()
        {
            var graph = GetAuthenticatedClient();
            Group[] result;
            if (HomeController.useAppPermissions)
            {
                var groups = await graph.Groups.Request().Select("id,resourceProvisioningOptions,displayName").GetAsync();
                result = groups
                    .Where(g => true) // g.AdditionalData["ResourceProvisioningOptions"].Contains("Team")) // beta; different API available in v1.0
                    .ToArray();
            }
            else
            {
                var teams = await graph.Me.JoinedTeams.Request().GetAsync();
                result = teams.Select(t => new Group() { Id = t.Id, DisplayName = t.DisplayName }).ToArray();
            }
            return result;
        }

        public async Task<User[]> GetUserIds(string[] userUpns)
        {
            var graph = GetAuthenticatedClient();
            var users = new List<User>();

            // Look up each user to get their Id property
            foreach (var upn in userUpns)
            {
                var user = await graph.Users[upn].Request().GetAsync();
                users.Add(user);
            }

            return users.ToArray();
        }

        public async Task<User> GetUserId(string userUpn)
            => (await GetUserIds(new string[] { userUpn })).First();


#region
        private async Task CreatePreflightPlan(string groupId, string channelId, DateTimeOffset departureTime, Flight flight)
        {
            //// Create Planner plan and tasks
            ////await CreatePreflightPlan(teamId, channel.Id, DateTimeOffset.Now, flight);

            //// Create a "Pre-flight checklist" plan
            //var plan = (await HttpPost($"/planner/plans",
            //    new PlannerPlan
            //    {
            //        Title = "Pre-flight Checklist",
            //        Owner = groupId
            //    }, retries: 5, retryDelay: 10))
            //    .Deserialize<PlannerPlan>();

            //// Create buckets
            //var toDoBucket = (await HttpPost($"/planner/buckets",
            //    new PlannerBucket
            //    {
            //        Name = "To Do",
            //        PlanId = plan.Id
            //    }))
            //.Deserialize<PlannerBucket>();

            //var completedBucket = (await HttpPost($"/planner/buckets",
            //    new PlannerBucket
            //    {
            //        Name = "Completed",
            //        PlanId = plan.Id
            //    }))
            //.Deserialize<PlannerBucket>();

            //// Create tasks in to-do bucket
            //await HttpPost($"/planner/tasks",
            //    new PlannerTask
            //    {
            //        Title = "Perform pre-flight inspection of aircraft",
            //        PlanId = plan.Id,
            //        BucketId = toDoBucket.Id,
            //        DueDateTime = departureTime.ToUniversalTime()
            //    });

            //await HttpPost($"/planner/tasks",
            //    new PlannerTask
            //    {
            //        Title = "Ensure food and beverages are fully stocked",
            //        PlanId = plan.Id,
            //        BucketId = toDoBucket.Id,
            //        DueDateTime = departureTime.ToUniversalTime()
            //    });

            //// Add planner tab to Pilots channel
            //await HttpPost($"/teams/{groupId}/channels/{channelId}/tabs",
            //    new TeamsTab
            //    {
            //        DisplayName = "Pre-flight Checklist",
            //        TeamsAppId = "com.microsoft.teamspace.tab.planner",

            //        Configuration = new TeamsTabConfiguration
            //        {
            //            EntityId = plan.Id,
            //            ContentUrl = $"https://tasks.office.com/{flight.tenantName}/Home/PlannerFrame?page=7&planId={plan.Id}&auth_pvr=Orgid&auth_upn={{upn}}&mkt={{locale}}",
            //            RemoveUrl = $"https://tasks.office.com/{flight.tenantName}/Home/PlannerFrame?page=13&planId={plan.Id}&auth_pvr=Orgid&auth_upn={{upn}}&mkt={{locale}}",
            //            WebsiteUrl = $"https://tasks.office.com/{flight.tenantName}/Home/PlanViews/{plan.Id}"
            //        }
            //    });
        }

        public async Task CreateChannel(string teamId, string channelName)
        {
            await HttpPost(
                $"/teams/{teamId}/channels",
                new Channel()
                {
                    DisplayName = channelName,
                    Description = ""
                });
        }

        public async Task DeleteAllChannel(string teamId)
        {
            Channel[] channels = await HttpGetList<Channel>($"/teams/{teamId}/channels");
            foreach (Channel c in channels)
            {
                if (c.DisplayName != "General")
                {
                    // Rename the channel so we don't run into problems later with soft delete
                    c.DisplayName = c.DisplayName + GetTimestamp();
                    await HttpPatch($"/teams/{teamId}/channels/{c.Id}", c);

                    // Delete it now that it has a name that won't conflict with future channels
                    await HttpDelete($"/teams/{teamId}/channels/{c.Id}");
                }
            }
        }

        //private async Task CopyFlightLogToTeamFilesAsync(GraphService graphClient, string groupId)
        //{
        //    // Upload flight log to team files
        //    // Get root site to determine SP host name
        //    var rootSite = await HttpGet<Site>($"/sites/root");

        //    // Get flight admin site
        //    var adminSite = await HttpGet<Site>($"{rootSite.SiteCollection.Hostname}:/sites/{flightAdminSite}");

        //    // Get the flight log document
        //    var flightLog = await HttpGet<DriveItem>($"/sites/{adminSite.Id}/drive/root:/{flightLogFile}");

        //    // Get the files folder in the team OneDrive
        //    var teamDrive = await HttpGet<DriveItem>($"/groups/{groupId}/drive/root:/General", retries: 4);
        //    // Retry this call if it fails
        //    // There seems to be a delay between creating a Team and the drives being
        //    // fully created/enabled

        //    // Copy the file from SharePoint to team drive
        //    var teamDriveReference = new ItemReference
        //    {
        //        DriveId = teamDrive.ParentReference.DriveId,
        //        Id = teamDrive.Id
        //    };

        //    await graphClient.CopySharePointFileAsync(adminSite.Id, flightLog.Id, teamDriveReference);
        //}

        //public async Task CopySharePointFileAsync(string siteId, string itemId, ItemReference target)
        //{
        //    var copyPayload = new DriveItem
        //    {
        //        ParentReference = target
        //    };

        //    var response = await HttpPost($"/sites/{siteId}/drive/items/{itemId}/copy",
        //        copyPayload);
        //}

        // Posting messages is supported in beta but will not GA in 2018.
        // Post a message to that channel
        //await HttpPost($"/teams/{teamId}/channels/{channel.Id}/chatThreads",
        //        new PostMessage()
        //{
        //    rootMessage = new RootMessage()
        //    {
        //        body = new MessageBody()
        //        {
        //            content = $"Welcome to Flight {flight.number}!"
        //        }
        //    }
        //        });


        public async Task<Microsoft.Graph.List> CreateSharePointListAsync(string siteId, Microsoft.Graph.List list)
        {
            var response = await HttpPost($"/sites/{siteId}/lists", list);
            return JsonConvert.DeserializeObject<Microsoft.Graph.List>(response);
        }

        public async Task<string> GetAdminUpn()
        {
            // Figure out the members and owners
            var me = await HttpGet<User>($"/me");
            return me.UserPrincipalName;
        }

        public async Task BulkDelete()
        {
            Group[] teams = await HttpGetList<Group>($"/me/joinedTeams");
            var relevant = teams.Where(t => t.DisplayName.StartsWith("Flight 158")).ToArray();
            foreach (var t in relevant)
            {
                await HttpDelete($"/groups/{t.Id}");
            }
        }

        private string challengingPassenger = "{ \"fields\": {\"Name\":\"Nick Kramer\",\"SeatNumber\":\"4C\",\"Notes\":\"Snores loudly\"} }";

        string GetTimestamp()
        {
            DateTime now = DateTime.Now;
            string timestamp = $"{now.Hour}{now.Minute}{now.Second}";
            return timestamp;
        }

        #endregion
    }
}