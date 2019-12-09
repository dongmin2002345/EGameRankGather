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
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;

            btnStop.Enabled = true;

            var registry = new Registry();

            //每隔5分钟,在线热门榜
            registry.Schedule(() => RankTask()).NonReentrant().ToRunEvery(5).Minutes();

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
        }

        private static string getWeb(string url)
        {
            GZipWebClient webClient = new GZipWebClient();

            webClient.Encoding = System.Text.Encoding.UTF8;

            return webClient.DownloadString(url);
        }
    }
}
