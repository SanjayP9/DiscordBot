using System;
using System.Collections.Generic;
using System.Web.Script;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using Discord;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace DiscordBot.Modules
{
    class PageScrape
    {
        private static ChromeDriver driver;

        public static async Task Initialise()
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            //options.AddArgument("load-extension=" + path + @"\3.27.0_0");

            driver = new ChromeDriver(path, options);

            Actions actionObj = new Actions(driver);
            actionObj.KeyDown(Keys.Control)
                     .SendKeys("w")
                     .KeyUp(Keys.Control)
                     .Perform();

            await Task.CompletedTask;
        }

        public static string SmartTranslate(string phrase)
        {
            string[] temp = phrase.Split(new string[] { " " }, StringSplitOptions.None);
            string result = String.Empty;

            //for (int i = 0; i < temp.Length; i++)
            //{
            //    result += GetBestSynonym(temp[i]) + " ";
            //}

            result = GetBestSynonym(temp[0]);

            return result;
        }

        // change to private later
        private static string GetBestSynonym(string word)
        {
            using (var driver = new ChromeDriver(@"\DiscordBot"))
            {
                driver.Navigate().GoToUrl("http://thesaurus.altervista.org/thesaurus/v1?word=" + word + "&language=en_US&output=json&key=" + APIKeys.AlterVistaAPIKey + "&callback=process");

                var jsonString = (driver.FindElementByTagName("pre").Text);

                dynamic json = JValue.Parse(jsonString);

                //string

                //var JsonObject = new JavaScriptSerializer().Deserialize<SynonymObject>(driver.FindElementByTagName("pre").Text);               

                //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                //var dict = oSerializer.Deserialize<Dictionary<string, object>>(responseFromServer);

                //string JSONObj = new JavaScriptSerializer 

            }


            return "";
        }

        public async static Task<Rank> GetRankInfo(string summonerName)
        {
            Rank result = new Rank();

            using (driver)
            {
                driver.Navigate().GoToUrl("https://na.op.gg/summoner/userName=" + summonerName);
                //driver.FindElementById("SummonerRefreshButton").Click();
                driver.FindElementByCssSelector(".Buttons .Button.SemiRound.LightGreen").Click();

                //result.Name = driver.FindElementByCssSelector(".Profile .Information .Name:last()").Text;
                result.LadderRank = driver.FindElementByCssSelector(".LadderRank").Text;
                result.ProfileImg = driver.FindElementByCssSelector(".ProfileIcon img").GetAttribute("src");
                Console.WriteLine(result.Name + "\n" + result.LadderRank + "\n" + result.ProfileImg + "Heading Scrape done");

                result.SoloRank = driver.FindElementByCssSelector(".TierRank .tierRank").Text;
                result.SoloMMR = driver.FindElementByCssSelector(".TierRankBOX .MMR").Text + " (" + driver.FindElementByCssSelector(".TierRankBOX .TierRankString").Text + ")";
                result.SoloRankImg = driver.FindElementByCssSelector(".SummonerRatingMedium .Image").GetAttribute("src");
                result.SoloRankInfo = driver.FindElementByCssSelector(".SummonerRatingMedium .TierRankInfo").Text;
                result.SoloRankName = driver.FindElementByCssSelector(".TierRankInfo .LeagueName").Text;
                Console.WriteLine(result.SoloRank + "\n" + result.SoloMMR + "\n" + result.SoloRankImg + "\n" + result.SoloRankInfo + "\n" + result.SoloRankName + "Solo Scrape done");

                result.FlexRank = driver.FindElementByCssSelector(".SummonerRatingLine .TierRank .TierRank").Text;
                result.FlexLP = driver.FindElementByCssSelector(".SummonerRatingLine .TierRank .leaguePoints").Text;
                result.FlexRankName = driver.FindElementByCssSelector(".SummonerRatingLine .TeamName").Text;
                result.FlexWinLoss = driver.FindElementByCssSelector(".SummonerRatingLine .WinLose .wins").Text + " / " + driver.FindElementByCssSelector(".SummonerRatingLine .WinLose .losses").Text;
                result.FlexWinPercent = "Win Ratio " + driver.FindElementByCssSelector(".SummonerRatingLine .winratio").Text;
                Console.WriteLine(result.FlexRank + "\n" + result.FlexLP + "\n" + result.FlexRankName + "\n" + result.FlexWinLoss + "\n" + result.FlexWinPercent + "\n" + "Flex Scrape done");

                /*result.Add(driver.FindElementByCssSelector(".ProfileIcon img").GetAttribute("src"));
                result.Add(driver.FindElementByCssSelector(".Profile").Text);
                result.Add(driver.FindElementByCssSelector(".SummonerRatingMedium").Text);
                result.Add(driver.FindElementByCssSelector(".SummonerRatingMedium .Image").GetAttribute("src"));
                result.Add(driver.FindElementByCssSelector(".TierBox.Box .SummonerRatingLine").Text);
                result.Add(driver.FindElementByCssSelector(".TierBox.Box .SummonerRatingLine .Image").GetAttribute("src"));
                result.Add(driver.FindElementByCssSelector(".Profile .Name").Text);*/
            }

            return result;
        }
    }

    class Rank
    {
        public string Name = "Unavailable";
        public string LadderRank = "Unavailable";
        public string ProfileImg = "https://vignette.wikia.nocookie.net/pixarcars/images/6/6c/Question-mark.png/revision/latest";

        public string SoloRank = "Unavailable";
        public string SoloMMR = "Unavailable";
        public string SoloRankImg = "Unavailable";
        public string SoloRankName = "Unavailable";
        public string SoloRankInfo = "Unavailable";

        public string FlexRank = "Unavailable";
        public string FlexRankName = "Unavailable";
        public string FlexLP = "Unavailable";
        public string FlexWinPercent = "Unavailable";
        public string FlexWinLoss = "Unavailable";

    }
}
