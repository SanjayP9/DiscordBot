using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    /// <summary>
    /// !info - Gets info for user that called command
    /// !info <user_name> - Gets info for mentioned user
    /// 
    /// </summary>
    public class Commmands : ModuleBase<SocketCommandContext>
    {


        private static String[] monthArr = { "",
            "January",
            "Febuary",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"};



        [Command("user")]
        public async Task UserInfoCommand()
        {
            await Context.Guild.DownloadUsersAsync();

            await UserInfoSync(Context.User);
        }

        [Command("user")]
        public async Task UserInfoCommand(IUser user)
        {
            await Context.Guild.DownloadUsersAsync();

            await UserInfoSync(user);
        }

        private async Task UserInfoSync(IUser user)
        {
            // offline users doesnt work lel
            await Context.Guild.DownloadUsersAsync();

            SocketGuildUser userGuild = (SocketGuildUser)user;

            string[] results = (userGuild.JoinedAt.ToString()).Split(new string[] { " " }, StringSplitOptions.None);
            string[] date = results[0].Split(new string[] { "/" }, StringSplitOptions.None);

            date[0] = monthArr[Int32.Parse(date[0])] + " ";

            switch (date[1][date[1].Length - 1])
            {
                case '1':
                    date[1] += "st ";
                    break;
                case '2':
                    date[1] += "nd ";
                    break;
                case '3':
                    date[1] += "rd ";
                    break;
                default:
                    date[1] += "th ";
                    break;
            }

            EmbedBuilder builder = new EmbedBuilder();

            //builder.WithTitle("")
            //    .WithDescription("Testing Purple embed method")
            //    .WithColor(Color.Purple);


            builder.WithTitle($"{user.ToString()}");
            builder.WithThumbnailUrl($"{user.GetAvatarUrl()}");
            builder.AddField($" {Context.Guild.ToString()} Join Date:", date[0] + date[1] + date[2] + " @ " + results[1] + " " + results[2]);

            if (user.Game != null)
            {
                builder.AddField("Game:", $"{user.Game}");
            }

            builder.AddField("Avatar URL:", $"{user.GetAvatarUrl()}");
            builder.AddField("Id:", $"{user.Id}");

            /*if (name == "<@!149320098434383872>")
            {
                builder.AddField("Test", name);
            }*/
            builder.WithColor(Color.Red);


            await ReplyAsync("", false, builder.Build());

            //Context.User;
            //Context.Client;
            //Context.Guild;
            //Context.Message;

            //await ReplyAsync($"{Context.User.Mention} \njoined the server on\n {Context.User.CreatedAt} \n :fire:");

        }

        [Command("serverinfo")]
        public async Task ServerInfoCommand()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle(Context.Guild.Name.ToString());
            builder.WithThumbnailUrl(Context.Guild.IconUrl);
            builder.WithColor(Color.Red);
            builder.AddField("Member Count:", Context.Guild.MemberCount.ToString());

            await ReplyAsync("", false, builder.Build());
        }

        [Command("smart")]
        public async Task SmartTranslateCommand()
        {
            APIKeys.RetrieveAPIKey();



            await ReplyAsync(APIKeys.AlterVistaAPIKey, false);
        }


        [Command("test")]
        public async Task TestSync(IUser user)
        {
            EmbedBuilder builder = new EmbedBuilder();

            SocketGuildUser guildUser = (SocketGuildUser)user;

            builder.WithTitle(guildUser.JoinedAt.ToString());

            await ReplyAsync("", false, builder.Build());

            //Context.Guild.Users()
        }

        [Command("lolrank")]
        public async Task LoLRank(string summonerName)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            EmbedBuilder builder;

            try
            {
                Rank rankData;
                rankData =  PageScrape.GetRankInfo(summonerName).Result;
                builder = new EmbedBuilder();

                builder.WithThumbnailUrl(rankData.ProfileImg);
                builder.AddField(rankData.Name, rankData.LadderRank);
                if (rankData.SoloRank != "Unavailable")
                {
                    builder.AddField("Solo Queue Ranked Stats", rankData.SoloRankInfo + "\n" + rankData.SoloRankName + "\n" + rankData.SoloMMR);
                }
                else
                {
                    builder.AddField("Solo Queue Ranked Stats", rankData.SoloRank);
                }

                if (rankData.FlexRank != "Unavailable")
                {
                    builder.AddField("Flex Queue Ranked Stats", rankData.FlexRank + "\n" + rankData.FlexLP  +" / " +rankData.FlexWinLoss + "\n" + rankData.FlexWinPercent + "\n" + rankData.FlexRankName);
                }
                else
                {
                    builder.AddField("Solo Queue Ranked Stats", rankData.SoloRank);
                }
                builder.WithColor(Color.Red);
            }
            catch (Exception e)
            {
                builder = new EmbedBuilder();
                builder.AddField("Error", e.ToString());
            }


            /*
            String name = "Unavailable";
            try
            {
                builder = new EmbedBuilder();
                List<string> result = PageScrape.GetRankInfo(summonerName).Result;

                builder.WithThumbnailUrl(result[0]);


                string[] profileScrape = result[1].Split(new string[] { "\n" }, StringSplitOptions.None);

                string ladderRank = "Unavailable";
                if (profileScrape[0].Contains("Favorites"))
                {
                    name = result[6];

                }
                if (profileScrape[1].Contains("Ladder"))
                {
                    ladderRank = profileScrape[1];
                }

                builder.AddField(name, ladderRank);

                string[] soloScrape = result[2].Split(new string[] { "\n" }, StringSplitOptions.None);

                if (soloScrape.Length == 4)
                {
                    builder.AddField("Solo Queue Ranked Stats", result[2]);
                }
                builder.WithImageUrl(result[3]);

                string[] flexScrape = result[4].Split(new string[] { "\n" }, StringSplitOptions.None);
                string flexOutput = "Unavailable";
                if (flexScrape.Length == 6)
                {
                    flexOutput = flexScrape[0] + "\n" + flexScrape[1] +  flexScrape[4] + "\nWin Ratio " + flexScrape[5] + "\n" + flexScrape[3];
                }
                builder.AddField("Flex Queue Ranked Stats", flexOutput);
                builder.WithColor(Color.Red);
            }
            catch (Exception e)
            {
                builder = new EmbedBuilder();
                builder.AddField("Error", e.ToString());
            }

            if (name.Contains("Jid Kid"))
            {
                builder = new EmbedBuilder();
                builder.WithImageUrl("http://opgg-static.akamaized.net/images/medals/bronze_5.png");
            }*/

            stopwatch.Stop();

            builder.WithFooter("Execution Time: " + (stopwatch.ElapsedMilliseconds / 1000.0f) + " sec");
            builder.WithCurrentTimestamp();

            await ReplyAsync("", false, builder.Build());
        }
    }
}