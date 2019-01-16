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

namespace ContosoAirlines.Models
{
    public class GraphService : HttpHelpers
    {
        public async Task<string> CreateTeam(Flight flight)
        {
            var ownerUpns = new List<string>();
            ownerUpns.Add(flight.captain);
            ownerUpns.Add(flight.admin);
            var ownerIds = await GetUserIds(ownerUpns.ToArray());

            var memberIds = await GetUserIds(flight.crew);
            memberIds.Add(await GetUserId(flight.admin));
            memberIds.Add(await GetUserId(flight.captain));

            // Create the modern group for the team
            Group group = 
                (await HttpPost($"/groups",
                            new Group()
                            {
                                DisplayName = "Flight " + flight.number,
                                MailNickname = "flight" + GetTimestamp(),
                                Description = "Everything about flight " + flight.number,
                                Visibility = "Private",
                                Owners = ownerIds,
                                Members = memberIds,
                                GroupTypes = new string[] { "Unified" }, // same for all teams
                                MailEnabled = true,                      // same for all teams
                                SecurityEnabled = false,                 // same for all teams
                            }))
                .Deserialize<Group>();

            // Create the team
            await HttpPut($"/groups/{group.Id}/team",
                new Team()
                {
                    GuestSettings = new TeamGuestSettings()
                    {
                        AllowCreateUpdateChannels = false,
                        AllowDeleteChannels = false
                    },
                    MemberSettings = new TeamMemberSettings() { },
                    MessagingSettings = new TeamMessagingSettings() { },
                    FunSettings = new TeamFunSettings() { },
                },
                retries: 3, retryDelay: 10);
            string teamId = group.Id; // always the same

            // Create a new channel for pilot talk
            Channel channel = (await HttpPost(
                $"/teams/{teamId}/channels",
                new Channel()
                {
                    DisplayName = "Pilots",
                    Description = "Discussion about flightpath, weather, etc."
                }
                )).Deserialize<Channel>();

            // Add a map tab to the created channel
            await HttpPost($"/teams/{teamId}/channels/{channel.Id}/tabs",
                new TeamsTab()
                {
                    DisplayName = "Map",
                    TeamsApp = $"{graphV1Endpoint}/appCatalogs/teamsApps/com.microsoft.teamspace.tab.web", // Website tab
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
            var list = (await HttpPost($"/sites/{teamSite.Id}/lists",
                new SharePointList
                {
                    DisplayName = "Challenging Passengers",
                    Columns = new List<ColumnDefinition>()
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
                }))
                .Deserialize<SharePointList>();

            await HttpPost($"/sites/{teamSite.Id}/lists/{list.Id}/items",
                challengingPassenger
                );

            // Add the list as a team tab
            await HttpPost($"/teams/{teamId}/channels/{channel.Id}/tabs",
                new TeamsTab
                {
                    DisplayName = "Challenging Passengers",
                    TeamsApp = $"{graphV1Endpoint}/appCatalogs/teamsApps/com.microsoft.teamspace.tab.web", // Website tab
                    // It's serialized as "teamsApp@odata.bind" : "{graphV1Endpoint}/appCatalogs/teamsApps/com.microsoft.teamspace.tab.web"
                    Configuration = new TeamsTabConfiguration
                    {
                        ContentUrl = list.WebUrl,
                        WebsiteUrl = list.WebUrl
                    }
                });

            return group.Id;
        }

        public async Task InstallAppToAllTeams()
        {
            if (HomeController.useAppPermissions)
                throw new Exception("Application permissions not currently supported");

            string appid = "0fd925a0-357f-4d25-8456-b3022aaa41a9"; // SurveyMonkey
            var teams = (await GetAllTeams()).Where(t => t.DisplayName.StartsWith("Flight 157"))
                .ToArray();
            foreach (var team in teams)
            {
                var t = await HttpGet<Team>($"/teams/{team.Id}");
                if (!t.IsArchived)
                {
                    // See if it's already installed
                    var apps = await HttpGetList<TeamsAppInstallation>($"/teams/{team.Id}/installedApps?$expand=teamsAppDefinition");
                    if (apps.Where(app => app.TeamsAppDefinition.Id == appid).Count() == 0)
                    {
                        await HttpPost($"/teams/{team.Id}/installedApps",
                            "{ \"teamsApp@odata.bind\" : \"" + graphV1Endpoint + "/appCatalogs/teamsApps/" + appid + "\" }");
                    }
                }
            }
        }

        public async Task<string> CreateTeamUsingClone(Flight flight)
        {
            var response = await HttpPostWithHeaders($"/teams/{flight.prototypeTeamId}/clone",
                new Clone()
                {
                    displayName = "Flight 4" + flight.number,
                    mailNickName = "flight" + GetTimestamp(),
                    description = "Everything about flight " + flight.number,
                    teamVisibilityType = "Private",
                    partsToClone = "apps,settings,channels"
                });
            string operationUrl = response.Headers.Location.ToString();

            string teamId = null;
            for (; ; )
            {
                TeamsAsyncOperation operation = await HttpGet<TeamsAsyncOperation>(operationUrl);

                if (operation.Status == AsyncOperationStatus.Failed)
                    throw new Exception();

                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    teamId = operation.targetResourceId;
                    break;
                }

                Thread.Sleep(10000); // wait 10 seconds between polls
            }

            // Add the crew to the team
            foreach (string upn in flight.crew)
            {
                string payload = $"{{ '@odata.id': '{graphV1Endpoint}/users/{upn}' }}";
                await HttpPost($"/groups/{teamId}/members/$ref", payload);
                if (upn == flight.captain)
                    await HttpPost($"/groups/{teamId}/owners/$ref", payload);
            }

            // get the webUrl
            Team team = await HttpGet<Team>($"/teams/{teamId}");
            string link = team.WebUrl;

            return link;
        }

        public async Task ArchiveAllTeams()
        {
            var teams = (await GetAllTeams()).Where(
                t => t.DisplayName.StartsWith("Flight 157") 
                || t.DisplayName.StartsWith("Flight 4157"))
                .ToArray();
            foreach (var team in teams)
            {
                var t = await HttpGet<Team>($"/teams/{team.Id}");
                if (!t.IsArchived)
                {
                    await ArchiveTeam(team.Id);
                }
            }
        }

        public async Task<string> ArchiveTeam(string teamId)
        {
            HttpResponse response = await HttpPostWithHeaders($"/teams/{teamId}/archive", "{}");
            string operationUrl = response.Headers.Location.ToString();

            for (; ; )
            {
                var operation = await HttpGet<TeamsAsyncOperation>(operationUrl);

                if (operation.Status == AsyncOperationStatus.Failed)
                    throw new Exception();

                if (operation.Status == AsyncOperationStatus.Succeeded)
                    break;

                Thread.Sleep(10000); // wait 10 seconds between polls
            }

            return "success";
        }

        // Get all the teams you have access to. For user delegated, look at the joinedTeams.
        // For application permissions, get all teams in the tenant.
        private async Task<Team[]> GetAllTeams()
        {
            Team[] result;
            if (HomeController.useAppPermissions)
            {
                Group[] groups = await HttpGetList<Group>(
                    $"/groups?$select=id,resourceProvisioningOptions,displayName");
                result = groups
                    .Where(g => g.ResourceProvisioningOptions.Contains("Team"))
                    .Select(g => new Team() {
                    DisplayName = g.DisplayName, Id = g.Id
                }).ToArray();
            }
            else
            {
                result = await HttpGetList<Team>($"/me/joinedTeams");
            }
            return result;
        }

        public async Task<List<string>> GetUserIds(string[] userUpns)
        {
            var userIds = new List<string>();

            // Look up each user to get their Id property
            foreach (var upn in userUpns)
            {
                String userId = (await HttpGet<User>($"/users/{upn}")).Id;
                userIds.Add($"{graphV1Endpoint}/users/{userId}");
            }

            return userIds;
        }

        public async Task<string> GetUserId(string userUpn)
            => (await GetUserIds(new string[] { userUpn })).First();


#region
        private async Task CreatePreflightPlan(string groupId, string channelId, DateTimeOffset departureTime, Flight flight)
        {
            // Create Planner plan and tasks
            //await CreatePreflightPlan(teamId, channel.Id, DateTimeOffset.Now, flight);

            // Create a "Pre-flight checklist" plan
            var plan = (await HttpPost($"/planner/plans",
                new Plan
                {
                    Title = "Pre-flight Checklist",
                    Owner = groupId
                }, retries: 5, retryDelay: 10))
                .Deserialize<Plan>();

            // Create buckets
            var toDoBucket = (await HttpPost($"/planner/buckets",
                new Bucket
                {
                    Name = "To Do",
                    PlanId = plan.Id
                }))
            .Deserialize<Bucket>();

            var completedBucket = (await HttpPost($"/planner/buckets",
                new Bucket
                {
                    Name = "Completed",
                    PlanId = plan.Id
                }))
            .Deserialize<Bucket>();

            // Create tasks in to-do bucket
            await HttpPost($"/planner/tasks",
                new PlannerTask
                {
                    Title = "Perform pre-flight inspection of aircraft",
                    PlanId = plan.Id,
                    BucketId = toDoBucket.Id,
                    DueDateTime = departureTime.ToUniversalTime()
                });

            await HttpPost($"/planner/tasks",
                new PlannerTask
                {
                    Title = "Ensure food and beverages are fully stocked",
                    PlanId = plan.Id,
                    BucketId = toDoBucket.Id,
                    DueDateTime = departureTime.ToUniversalTime()
                });

            // Add planner tab to Pilots channel
            await HttpPost($"/teams/{groupId}/channels/{channelId}/tabs",
                new TeamsTab
                {
                    Name = "Pre-flight Checklist",
                    TeamsAppId = "com.microsoft.teamspace.tab.planner",
                    Configuration = new TeamsTabConfiguration
                    {
                        EntityId = plan.Id,
                        ContentUrl = $"https://tasks.office.com/{flight.tenantName}/Home/PlannerFrame?page=7&planId={plan.Id}&auth_pvr=Orgid&auth_upn={{upn}}&mkt={{locale}}",
                        RemoveUrl = $"https://tasks.office.com/{flight.tenantName}/Home/PlannerFrame?page=13&planId={plan.Id}&auth_pvr=Orgid&auth_upn={{upn}}&mkt={{locale}}",
                        WebsiteUrl = $"https://tasks.office.com/{flight.tenantName}/Home/PlanViews/{plan.Id}"
                    }
                });
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

        public async Task<string> GetChannelText(string teamId, string channelId)
        {
            ChatMessage[] messages = await HttpGetList<ChatMessage>($"/teams/{teamId}/channels/{channelId}/messages");
            string[] texts = messages.Select(m => m.Body.Content).ToArray();
            string all = String.Join(" ", texts);
            return all;
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


        public async Task<SharePointList> CreateSharePointListAsync(string siteId, SharePointList list)
        {
            var response = await HttpPost($"/sites/{siteId}/lists", list);
            return JsonConvert.DeserializeObject<SharePointList>(response);
        }

        public async Task<string> GetAdminUpn()
        {
            // Figure out the members and owners
            var me = await HttpGet<User>($"/me");
            return me.userPrincipalName;
        }


        public async Task BulkDelete()
        {
            Team[] teams = await HttpGetList<Team>($"/me/joinedTeams");
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