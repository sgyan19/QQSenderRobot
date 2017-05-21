using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    /// <summary>
    /// 预留推特数据结构
    /// </summary>
    public class Twitter : BaseData
    {
        public string Id { set; get; }
        public TwitterUser  User{ set; get; }
        public string ReplyId { set; get; }
        public TwitterUser ReplyUser { set; get; }
        public Twitter Reply { set; get; }
        public string Truncated { set; get; }

        public string IsQuoteStatus { set; get; }

        public string QuotedStatusId { set; get; }

        public Twitter QuotedStatus { set; get; }

        public bool EverUsed { set; get; }

        public string[] ExpandedUrls { set; get; }
    }
}
