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
        public IEnumerable<string> TImgUrls { set; get; }
    }
}
