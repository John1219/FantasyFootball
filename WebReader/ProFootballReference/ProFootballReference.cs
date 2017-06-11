using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyToolbox.ProFootballReference
{
    public class ProFootballReference
    {
        public event Action PlayerListChanged = delegate { };
        public event Action PlayerInfoChanged = delegate { };

        private List<Player> all_players_list;
        public List<Player> AllPlayers
        {
            get { return all_players_list; }
        }

        public ProFootballReference()
        {
            xml_manager = new XmlManager();
            all_players_list = new List<Player>();
        }

        public void LoadPlayers()
        {
            if (File.Exists(string.Format("{0}{1}", kPlayers, kXml)))
            {
                all_players_list = (List<Player>)xml_manager.LoadFromXml(all_players_list.GetType(), string.Format("{0}{1}", kPlayers, kXml));
            }
            else
            {
                GetAllPlayersList();
                Save();
            } 
            PlayerListChanged();
        }

        public void LoadPlayerData(Player player)
        {
            if (player.Link != null)
            {

                GetAddtionalPlayerData(player);
                GetPlayerStats(player);
                player.LastUpdated = DateTime.Now;
                Save();
            }
        }

        private const string kPlayers = "Players";
        private const string kXml = ".xml";

        private static string url_root = "http://www.pro-football-reference.com";
        private static string url_players = url_root + "/players/";

        private XmlManager xml_manager;

        private void GetAllPlayersList()
        {
            all_players_list = new List<Player>();

            foreach (var letter in Strings.ALPHABET)
            {
                string url = string.Format(@"{0}{1}/", url_players, letter);

                all_players_list.AddRange(GetPlayers(url));

                PlayerListChanged();
            }
        }

        private IEnumerable<Player> GetPlayers(string url)
        {
            List<Player> players_list = new List<Player>();
            WebPage web_page = new WebPage();
            web_page.LoadPage(url);

            HtmlNode player_div = web_page.GetSingleNode("div", "id", "div_players");
            IEnumerable<HtmlNode> player_paragraphs = web_page.GetNodes(player_div, "p");

            players_list = player_paragraphs.Select(p => new Player()
            {
                Name = web_page.GetSingleNode(p, "a").InnerHtml.Trim(),
                Link = web_page.GetSingleNode(p, "a").GetAttributeValue("href", "").Trim(),
                IsActive = (p.InnerHtml.Contains("<b>")),
                Position = p.InnerHtml.Replace(web_page.GetSingleNode(p, "a").OuterHtml, "").Replace("<b>", "").Replace(@"</b>", "").Split('(', ')')[1].Trim(),
                Years = p.InnerHtml.Replace(web_page.GetSingleNode(p, "a").OuterHtml, "").Replace("<b>", "").Replace(@"</b>", "").Split(')')[1].Trim(),

            }).ToList();

            return players_list;
        }

        private void GetAddtionalPlayerData(Player player)
        {
            WebPage web_page = new WebPage();
            web_page.LoadPage(url_root + player.Link);

            HtmlNode root_node = web_page.GetRootNode;

            string height = web_page.GetSingleNode("span", "itemprop", "height")?.InnerText;
            string weight = web_page.GetSingleNode("span", "itemprop", "weight")?.InnerText;

            string birth_date = web_page.GetSingleNode("span", "itemprop", "birthDate")?.GetAttributeValue("data-birth", "");
            string birth_place = web_page.GetSingleNode("span", "itemprop", "birthPlace")?.InnerText.Replace("in&nbsp", "").Replace("&nbsp", " ").Replace(";", "").Trim();
            string death_date = web_page.GetSingleNode("span", "itemprop", "birthDate")?.GetAttributeValue("data-death", "");

            IEnumerable <HtmlNode> college_strongs = root_node.Descendants("strong").Where(n => n.InnerText == "College");
            string college = (college_strongs.Count() == 1) ? college_strongs.Single().NextSibling.NextSibling.InnerText : "";
            IEnumerable<HtmlNode> high_school_strongs = root_node.Descendants("strong").Where(n => n.InnerText == "High School");
            string high_school = (high_school_strongs.Count() == 1) ? high_school_strongs.Single().NextSibling.NextSibling.InnerText : "";
            string high_school_state = (high_school_strongs.Count() == 1) ? high_school_strongs.Single().NextSibling.NextSibling.NextSibling.NextSibling.InnerText : "";

            HtmlNode affiliation_span = web_page.GetSingleNode("span", "itemprop", "affiliation");
            string team = affiliation_span != null ? affiliation_span.InnerText : "";
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
        }

        private void GetPlayerStats(Player player)
        {
            player.GameLogs = new List<GameLog>();
            GameLog log = new GameLog();
            WebPage web_page = new WebPage();
            web_page.LoadPage(url_root + player.GameLogsLink);

            HtmlNode stats_table = web_page.GetSingleNode("table", "id", "stats");
            if (stats_table != null)
            {
                IEnumerable<HtmlNode> stats_table_rows = web_page.GetNodes(web_page.GetSingleNode(stats_table, "tbody"), "tr");

                int id = -1;
                player.GameLogs.AddRange(stats_table_rows.Where(tr => Int32.TryParse(web_page.GetSingleNode(tr, "th", "data-stat", "ranker").InnerText, out id)).Select(tr => new GameLog()
                {
                    Id = Int32.TryParse(web_page.GetSingleNode(tr, "th", "data-stat", "ranker").InnerText, out id) ? id : 0,
                    GameDate = GetDataStatValue(tr, "game_date"),
                    GameNumber = GetDataStatValue(tr, "game_num"),
                    Team = GetDataStatValue(tr, "team"),
                    GameLocation = GetDataStatValue(tr, "game_location"),
                    Opponent = GetDataStatValue(tr, "opp"),
                    GameResult = GetDataStatValue(tr, "game_result"),
                    Targets = GetDataStatValue(tr, "targets"),
                    RushingAttempts = GetDataStatValue(tr, "rush_att"),
                    RushingYards = GetDataStatValue(tr, "rush_yds"),
                    RushingTouchdowns = GetDataStatValue(tr, "rush_td"),
                    Receptions = GetDataStatValue(tr, "rec"),
                    ReceivingYards = GetDataStatValue(tr, "rec_yds"),
                    ReceivingTouchdowns = GetDataStatValue(tr, "rec_td"),
                    TwoPointsMade = GetDataStatValue(tr, "two_pt_md"),
                    AllTouchdowns = GetDataStatValue(tr, "all_td"),
                    Points = GetDataStatValue(tr, "scoring"),
                    Sacks = GetDataStatValue(tr, "sacks"),
                    SoloTackles = GetDataStatValue(tr, "tackles_solo"),
                    Assists = GetDataStatValue(tr, "tackles_assists"),
                    XtraPointsMade = GetDataStatValue(tr, "xpm"),
                    XtraPointsAttempted = GetDataStatValue(tr, "xpa"),
                    FieldGoalsMade = GetDataStatValue(tr, "fgm"),
                    FieldGoalsAttempted = GetDataStatValue(tr, "fga"),
                    KickReturns = GetDataStatValue(tr, "kick_ret"),
                    KickReturnYards = GetDataStatValue(tr, "kick_ret_yds"),
                    KickReturnTouchdowns = GetDataStatValue(tr, "kick_ret_td"),
                    Interceptions = GetDataStatValue(tr, "def_int"),
                    InterceptionYards = GetDataStatValue(tr, "def_int_yds"),
                    InterceptionTouchdowns = GetDataStatValue(tr, "def_int_td"),
                }));
            }
        }

        private string GetDataStatValue(HtmlNode tr, string attr_value)
        {
            var td = tr.Descendants("td").Where(n => n.GetAttributeValue("data-stat", "").Equals(attr_value));
            return (td.Count() == 1) ? td.Single().InnerText : "";
        }

        private void Save()
        {
            if (all_players_list.Count() >= 1)
            {
                xml_manager.SaveToXml(all_players_list, string.Format("{0}{1}", kPlayers, kXml));
            }
        }
    }
}
