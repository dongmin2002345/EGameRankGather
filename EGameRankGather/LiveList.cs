using System.Collections.Generic;

public class LiveList
{
    public Data data { get; set; }

    public class Data
    {
        public Key key { get; set; }
    }

    public class Key
    {
        public RetBody retBody { get; set; }
    }

    public class RetBody
    {
        public RetData data { get; set; }
    }

    public class RetData
    {
        public int total { get; set; }

        public Live_data live_data { get; set; }
    }

    public class Live_data
    {
        public List<Live_listItem> live_list { get; set; }
    }

    public class Live_listItem
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long anchor_id { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string anchor_name { get; set; }

        /// <summary>
        /// 粉丝/关注人数
        /// </summary>
        public int fans_count { get; set; }

        /// <summary>
        /// 在线观看人数
        /// </summary>
        public int online { get; set; }

        /// <summary>
        /// 房间标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 直播分类
        /// </summary>
        public string appname { get; set; }
    }
}
