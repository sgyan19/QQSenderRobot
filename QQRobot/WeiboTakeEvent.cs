using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    /// <summary>
    /// server中回调的抓取结果事件
    /// </summary>
    interface WeiboTakeEvent
    {
        void TakeWeiboes(Weibo[] takeWeibos);
        void NewWeibos(Weibo[] newWeibos,Weibo[] newAll);
        void OnCountDown(int secounds);
        void OnStart();
        void OnStop();
        void OnException(Exception e);
    }
}
