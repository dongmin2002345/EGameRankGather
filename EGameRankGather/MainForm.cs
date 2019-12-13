using FluentScheduler;
using MySql.Data.MySqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace EGameRankGather
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        /*
         全部      https://egame.qq.com/livelist?layoutid=hot
         梦工厂    https://egame.qq.com/livelist?layoutid=2000000157
         */

        private void MainForm_Load(object sender, EventArgs e)
        {
            //RankTask();

            //PercentTask("梦工厂", 100000);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;

            btnStop.Enabled = true;

            var registry = new Registry();

            //每隔5分钟,在线热门榜
            registry.Schedule(() => RankTask()).NonReentrant().ToRunEvery(5).Minutes();

            //每天1:00点执行
            registry.Schedule(() => PercentTask("梦工厂", 100000, 50000)).NonReentrant().ToRunEvery(1).Days().At(1, 0);

            JobManager.Initialize(registry);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;

            btnStart.Enabled = true;

            JobManager.StopAndBlock();
        }

        private static void RankTask()
        {
            //所有 3 页
            LiveList liveList = getAppId("hot", 1);

            if (liveList.data.key.retBody.data.total > 120)
            {
                getAppId("hot", 2);
            }

            if (liveList.data.key.retBody.data.total > 240)
            {
                getAppId("hot", 3);
            }

            //梦工厂 2 页
            liveList = getAppId("2000000157", 1);

            if (liveList.data.key.retBody.data.total > 120)
            {
                getAppId("2000000157", 2);
            }
        }

        public static LiveList getAppId(string appid, int page)
        {
            string url = "https://share.egame.qq.com/cgi-bin/pgg_live_async_fcgi?param={\"key\":{\"module\":\"pgg_live_read_ifc_mt_svr\",\"method\":\"get_pc_live_list\",\"param\":{\"appid\":\"" + appid + "\",\"page_num\":" + page + ",\"page_size\":120,\"tag_id\":0,\"tag_id_str\":\"\"}}}&app_info={\"platform\":4,\"terminal_type\":2,\"egame_id\":\"egame_official\",\"imei\":\"\",\"version_code\":\"9.9.9.9\",\"version_name\":\"9.9.9.9\"}";

            string text = getWeb(url);

            LiveList liveList = JsonHelper.DeSerialize<LiveList>(text);

            InsertData(liveList);

            return liveList;
        }

        private static void InsertData(LiveList liveList)
        {
            var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            var db = new QueryFactory(connection, new MySqlCompiler());

            foreach (var item in liveList.data.key.retBody.data.live_data.live_list)
            {
                var query = db.Query("RankInfo").Insert(new
                {
                    UserId = item.anchor_id,
                    UserNick = item.anchor_name,
                    UserFans = item.fans_count,
                    RoomOnline = item.online,
                    RoomTitle = item.title,
                    AppName = item.appname
                });
            }

            connection.Close();
        }

        /// <summary>
        /// 计算当前分类所占比例
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="online"></param>
        private static void PercentTask(string appName, int online, int fans)
        {
            var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            var db = new QueryFactory(connection, new MySqlCompiler());

            db.Logger = compiled =>
            {
                Console.WriteLine(compiled.ToString());
            };

            var rooms = db.Query("RankInfo")
                .Select("UserId")
                .SelectRaw("MAX(`RoomOnline`) as RoomOnline")
                .Where("AppName", "=", appName)
                .GroupBy("UserId")
                .HavingRaw($"MAX(`RoomOnline`) >= {online} and MAX(`UserFans`) >= {fans}")
                .OrderByRaw("MAX(`RoomOnline`) DESC")
                .Get<Room>();

            foreach (var room in rooms)
            {
                try
                {
                    //查询分类占百分比
                    int percent = GetPercent(room.UserId, appName);

                    //更新百分比
                    UpdatePercent(room.UserId, appName, percent);
                }
                catch
                {


                }
            }

            connection.Close();
        }

        private static int GetPercent(int userId, string appName)
        {
            int count = 0;

            int total = 0;

            var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            var db = new QueryFactory(connection, new MySqlCompiler());

            db.Logger = compiled =>
            {
                Console.WriteLine(compiled.ToString());
            };

            var roomCount = db.Query("RankInfo")
                .Select("AppName")
                .SelectRaw("count(*) as Count")
                .Where("UserId", "=", userId)
                .GroupBy("AppName")
                .Get<RoomCount>();

            foreach (var room in roomCount)
            {
                total += room.Count;

                if (room.AppName == appName)
                {
                    count = room.Count;
                }
            }

            connection.Close();

            int percent = (int)(((double)count / total) * 100);

            return percent;
        }

        private static void UpdatePercent(int userId, string appName, int percent)
        {
            var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            var db = new QueryFactory(connection, new MySqlCompiler());

            db.Logger = compiled =>
            {
                Console.WriteLine(compiled.ToString());
            };

            int affected = db.Query("RankInfo")
                .Where("UserId", "=", userId)
                .Where("AppName", "=", appName)
                .Where("AppPercent", "=", 0)
                .Update(new { AppPercent = percent });

            connection.Close();
        }

        private static string getWeb(string url)
        {
            GZipWebClient webClient = new GZipWebClient();

            webClient.Encoding = System.Text.Encoding.UTF8;

            return webClient.DownloadString(url);
        }
    }
}
