using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebReader.ProFootballReference
{
    public class ProFootballReference
    {
        
        public static event Action LocalPlayerListChanged = delegate { };
        public static event Action LocalPlayerInfoLoaded = delegate { };
        public event Action PlayerListChanged = delegate { };
        public event Action PlayerInfoLoaded = delegate { };

        private static List<Player> all_player_list;
        public List<Player> Allplayers
        {
            get { return all_player_list.OrderBy(p => p.Link).ToList(); }
        }

        public ProFootballReference()
        {
            LocalPlayerListChanged += UpdateAllPlayers;
            LocalPlayerInfoLoaded += UpdatePlayerData;
            all_player_list = new List<Player>();
        }

        public void LoadPlayers()
        {
            if (File.Exists(string.Format("{0}{1}", kPlayers, kXml)))
            {

                LoadFromXml();
                PlayerListChanged();
            }
            else
            {
                LoadAllPlayers();
            }
        }

        public void LoadPlayerData(Player player)
        {
            if (player.Link != null)
            {
                LoadAdditionalPlayerData(player);
            }
        }


        private const string kXml = ".xml";
        private const string kPlayers = "Players";

        private static string url_root = "http://www.pro-football-reference.com";
        private static string url_players = url_root + "/players/";

        private static bool is_update_complete;
        private static bool is_addtional_info_complete;

        private static void LoadAllPlayers()
        {
            is_update_complete = false;
            int loop_index = 0;
            int loop_total = (Strings.ALPHABET.ToCharArray().ToList().Count);
            foreach (char c in Strings.ALPHABET.ToCharArray())
            {

                string letter = c.ToString();
                string url = string.Format(@"{0}{1}/", url_players, letter);
                Task<IEnumerable<Player>> player_list = GetPlayersAsync(url);
                player_list.ContinueWith(t =>
                {
                    loop_index++;
                    if (loop_index == loop_total)
                    {
                        is_update_complete = true;
                    }
                    all_player_list.AddRange(player_list.Result);
                    LocalPlayerListChanged();
                    if (is_update_complete)
                    {
                        is_update_complete = false;
                        //LoadAdditionalData();
                    }
                });
            }

        }

        private static void LoadAdditionalData()
        {
            is_addtional_info_complete = false;
            int loop_index = 0;
            int loop_total = (all_player_list.Count);
            foreach (Player p in all_player_list)
            {
                Task additionalInfo = GetAdditionalPlayerInfoAsync(p);
                additionalInfo.ContinueWith(t =>
                {
                    Thread.Sleep(500);
                    loop_index++;
                    if (loop_index == loop_total)
                    {
                        is_addtional_info_complete = true;
                    }
                    LocalPlayerListChanged();
                });
            }
        }

        private static void LoadAdditionalPlayerData(Player player)
        {
            Task additionalInfo = GetAdditionalPlayerInfoAsync(player);
            additionalInfo.ContinueWith(t =>
            {
                LocalPlayerInfoLoaded();
                if (!string.IsNullOrWhiteSpace(player.GameLogsLink))
                {
                    LoadPlayerStats(player);
                }
            });
        }

        private static void LoadPlayerStats(Player player)
        {
            Task stats = GetPlayerStatsAsync(player);
            stats.ContinueWith(t =>
            {
                LocalPlayerInfoLoaded();
            });
        }

        private void UpdateAllPlayers()
        {
            PlayerListChanged();
            if (is_update_complete)
            {
                SaveToXml(); 
            }
            if(is_addtional_info_complete)
            {
                SaveToXml();
            }
        }

        private void UpdatePlayerData()
        {
            PlayerInfoLoaded();
            SaveToXml();
        }

        private static IEnumerable<Player> GetPlayers(string url)
        {
            WebPage web_page = new WebPage(url);
            string xpath = "//div[@id='div_players']//p";
            List <Player> player_list = new List<Player>(); 
            foreach (HtmlNode item in web_page.GetNodes(xpath))
            {

                string position_year = item.InnerHtml.Replace(item.SelectNodes(".//a").FirstOrDefault().OuterHtml, "").Replace("<b>", "").Replace(@"</b>", "");
                Player p = new Player()
                {
                    Name = item.SelectNodes(".//a").FirstOrDefault().InnerHtml.Trim(),
                    Position = position_year.Split('(', ')')[1].Trim(),
                    Link = item.SelectNodes(".//a").FirstOrDefault().Attributes["href"].Value.Trim(),
                    Years = position_year.Split(')')[1].Trim(),
                    IsActive = (item.InnerHtml.Contains("<b>")),
                };
                player_list.Add(p);
            }
            return player_list.Where(p => p != null);
        }

        private static Task<IEnumerable<Player>> GetPlayersAsync(string url)
        {
            return Task.Run<IEnumerable<Player>>(() =>
            {
                return GetPlayers(url);
            });
        }

        private static bool GetAdditionalPlayerInfo(Player player)
        {
            bool result = false;

            try
            {
                WebPage web_page = new WebPage();
                HtmlNode root_node = web_page.GetRootNode(url_root + player.Link);

                IEnumerable<HtmlNode> height_spans = root_node.Descendants("span").Where(n => n.GetAttributeValue("itemprop", "").Equals("height"));
                string height = (height_spans.Count() == 1) ? height_spans.Single().InnerText : "";
                IEnumerable<HtmlNode> weight_spans = root_node.Descendants("span").Where(n => n.GetAttributeValue("itemprop", "").Equals("weight"));
                string weight = (weight_spans.Count() == 1) ? weight_spans.Single().InnerText : "";
                IEnumerable<HtmlNode> birth_date_spans = root_node.Descendants("span").Where(n => n.GetAttributeValue("itemprop", "").Equals("birthDate"));
                string birth_date = (birth_date_spans.Count() == 1) ? birth_date_spans.Single().GetAttributeValue("data-birth", "") : "";
                IEnumerable<HtmlNode> birth_place_spans = root_node.Descendants("span").Where(n => n.GetAttributeValue("itemprop", "").Equals("birthPlace"));
                string birth_place = (birth_place_spans.Count() == 1) ? birth_place_spans.Single().InnerText.Replace("in&nbsp", "").Replace("&nbsp", " ").Replace(";", "").Trim() : "";
                IEnumerable<HtmlNode> death_date_spans = root_node.Descendants("span").Where(n => n.GetAttributeValue("itemprop", "").Equals("deathDate"));
                string death_date = (death_date_spans.Count() == 1) ? death_date_spans.Single().GetAttributeValue("data-death", "") : "";
                IEnumerable<HtmlNode> college_strongs = root_node.Descendants("strong").Where(n => n.InnerText == "College");
                string college = (college_strongs.Count() == 1) ? college_strongs.Single().NextSibling.NextSibling.InnerText : "";
                IEnumerable<HtmlNode> high_school_strongs = root_node.Descendants("strong").Where(n => n.InnerText == "High School");
                string high_school = (high_school_strongs.Count() == 1) ? high_school_strongs.Single().NextSibling.NextSibling.InnerText : "";
                string high_school_state = (high_school_strongs.Count() == 1) ? high_school_strongs.Single().NextSibling.NextSibling.NextSibling.NextSibling.InnerText : "";
                IEnumerable<HtmlNode> affiliation_spans = root_node.Descendants("span").Where(n => n.GetAttributeValue("itemprop", "").Equals("affiliation"));
                string team = (affiliation_spans.Count() == 1) ? affiliation_spans.Single().InnerText : "";
                IEnumerable<HtmlNode> draft_strongs = root_node.Descendants("strong").Where(n => n.InnerText == "Draft");
                string draft_team = (draft_strongs.Count() == 1) ? draft_strongs.Single().NextSibling.NextSibling.InnerText : "";
                string draft_position = (draft_strongs.Count() == 1) ? draft_strongs.Single().NextSibling.NextSibling.NextSibling.InnerText : "";
                string draft_year = (draft_strongs.Count() == 1) ? draft_strongs.Single().NextSibling.NextSibling.NextSibling.NextSibling.InnerText : "";
                IEnumerable<HtmlNode> twitter_strongs = root_node.Descendants("strong").Where(n => n.InnerText == "Twitter:");
                string twitter = (twitter_strongs.Count() == 1) ? twitter_strongs.Single().NextSibling.NextSibling.InnerText : "";
                IEnumerable<HtmlNode> images = root_node.Descendants("img").Where(n => n.GetAttributeValue("itemscope", "").Equals("image"));
                string image_path = (images.Count() == 1) ? images.Single().GetAttributeValue("src", "") : "";
                IEnumerable<HtmlNode> bottom_nav_container = root_node.Descendants("div").Where(n => n.GetAttributeValue("id", "").Equals("bottom_nav_container"));
                IEnumerable<HtmlNode> career_items = (bottom_nav_container.Count() == 1) ? bottom_nav_container.Single().Descendants("li").Where(n => n.InnerText == "Career").Where(g => g.InnerHtml.Contains("gamelog")) : bottom_nav_container;
                string game_logs_link = (career_items.Count() == 1) ? career_items.Single().Descendants("a").Single().GetAttributeValue("href", "") : "";

                player.Height = string.IsNullOrEmpty(height) ? "" : height;
                player.Weight = string.IsNullOrEmpty(weight) ? "" : weight;
                player.BirthDate = string.IsNullOrEmpty(birth_date) ? "" : birth_date;
                player.BirthPlace = string.IsNullOrEmpty(birth_place) ? "" : birth_place;
                player.DeathDate = string.IsNullOrEmpty(death_date) ? "" : death_date;
                player.College = string.IsNullOrEmpty(college) ? "" : college;
                player.HighSchool = string.IsNullOrEmpty(high_school) ? "" : string.Format("{0} ({1})", high_school, high_school_state);
                player.Team = string.IsNullOrEmpty(team) ? "" : team;
                player.DraftedTeam = string.IsNullOrEmpty(draft_team) ? "" : draft_team;
                player.DraftedPosition = string.IsNullOrEmpty(draft_position) ? "" : draft_position;
                player.DraftedYear = string.IsNullOrEmpty(draft_year) ? "" : draft_year;
                player.Twitter = string.IsNullOrEmpty(twitter) ? "" : twitter;
                player.HeadShotPath = string.IsNullOrEmpty(image_path) ? "" : image_path;
                player.GameLogsLink = string.IsNullOrEmpty(game_logs_link) ? "" : game_logs_link;

                result = true;

            }
            catch(Exception ex)
            {
                Console.WriteLine(player.Name);
                Console.WriteLine(ex);
            }
 

            return result;
        }

        private static Task GetAdditionalPlayerInfoAsync(Player player)
        {
            return Task.Run(() =>
            {
                return GetAdditionalPlayerInfo(player);
            });
        }

        private static bool GetPlayerStats(Player player)
        {
            bool result = false;

            try
            {
                WebPage web_page = new WebPage();
                HtmlNode root_node = web_page.GetRootNode(url_root + player.GameLogsLink);

                IEnumerable<HtmlNode> stats_tables = root_node.Descendants("table").Where(n => n.GetAttributeValue("id", "").Equals("stats"));
                IEnumerable<HtmlNode> stats_table_rows = stats_tables.Single().Descendants("tbody").Single().Descendants("tr");
                int count = stats_table_rows.Count();
                int loop_count = 0;
                int add_count = 0;
                player.GameLogs.Clear();
                foreach(HtmlNode tr in stats_table_rows)
                {
                    GameLog log = new GameLog();
                    int id = -1;
                    var th = tr.Descendants("th").Where(n => n.GetAttributeValue("data-stat", "").Equals("ranker"));
                    string rank = (th.Count() == 1) ? th.Single().InnerText : "";
                    log.Id = Int32.TryParse(rank, out id) ? id : 0;
                    log.GameDate = GetDataStatValue(tr, "game_date");
                    log.GameNumber = GetDataStatValue(tr, "game_num");
                    log.Team = GetDataStatValue(tr, "team");
                    log.GameLocation = GetDataStatValue(tr, "game_location");
                    log.Opponent = GetDataStatValue(tr, "opp");
                    log.GameResult = GetDataStatValue(tr, "game_result");
                    log.Targets = GetDataStatValue(tr, "targets");
                    log.Receptions = GetDataStatValue(tr, "rec");
                    log.ReceivingYards = GetDataStatValue(tr, "rec_yds");
                    log.ReceivingTouchdowns = GetDataStatValue(tr, "rec_td");
                    log.TwoPointsMade = GetDataStatValue(tr, "two_pt_md");
                    log.AllTouchdowns = GetDataStatValue(tr, "all_td");
                    log.Points = GetDataStatValue(tr, "scoring");
                    log.Sacks = GetDataStatValue(tr, "sacks");
                    log.SoloTackles = GetDataStatValue(tr, "tackles_solo");
                    log.Assists = GetDataStatValue(tr, "tackles_assists");

                    if (log.Id != 0)
                    {
                        player.GameLogs.Add(log);
                        add_count++;
                    }
                    loop_count++;

                }
                Console.WriteLine(string.Format("{0} loops : {1} adds", loop_count, add_count));
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(player.Name);
                Console.WriteLine(ex);
            }

            return result;
        }

        private static Task GetPlayerStatsAsync(Player player)
        {
            return Task.Run(() =>
            {
                return GetPlayerStats(player);
            });
        }

        private static string GetDataStatValue(HtmlNode tr, string attr_value)
        {
            var td = tr.Descendants("td").Where(n => n.GetAttributeValue("data-stat", "").Equals(attr_value));
            return (td.Count() == 1) ? td.Single().InnerText : "";
        }

        private void LoadFromXml()
        {
            all_player_list = new List<Player>();
            XmlSerializer deserializer = new XmlSerializer(all_player_list.GetType());
            TextReader reader = new StreamReader(string.Format("{0}{1}", kPlayers, kXml));
            object obj = deserializer.Deserialize(reader);
            all_player_list = (List<Player>)obj;
            reader.Close();
        }

        private void SaveToXml()
        {
            XmlSerializer x = new XmlSerializer(all_player_list.GetType());

            using (
                
                TextWriter writer = new StreamWriter(string.Format("{0}{1}", kPlayers, kXml)))
            {
                x.Serialize(writer, all_player_list);
            }
        }
    }
}
