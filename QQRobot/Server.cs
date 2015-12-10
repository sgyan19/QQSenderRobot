using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QQRobot
{
    class Server : Object
    {
        private Server() { }
        private static Server singleton;
        private static Object lockObj = new Object();

        public WeiboTakeEvent Callback;
        private int flag = 0;
        public static Server getInstance()
        {
            if (singleton == null)
            {
                lock (lockObj)
                {
                    if(singleton == null)
                    {
                        singleton = new Server();
                    }
                }
            }
            return singleton;
        }
        private Thread thread;
        private BaseTaker taker;
        public void Start(BaseTaker taker)
        {
            flag = 1;
            this.taker = taker;
            countdown = taker.Interval;
            if (thread != null)
            {
                try
                {
                    thread.Abort();
                }catch(Exception )
                {
                    
                }
                thread = null;
            }

            thread = new Thread(new ThreadStart(run));
            thread.Start();
        }

        public void StartOnce(BaseTaker taker)
        {
            if(this.taker == null)
            {
                this.taker = taker;
            }
            new Thread(new ThreadStart(runOnce)).Start() ;
        }

        public void Stop()
        {
            flag = 0;
        }

        public void AbortStop()
        {
            flag = 0;
            if (thread != null)
            {
                try
                {
                    thread.Abort();
                }
                catch (Exception)
                {

                }
                thread = null;
            }
        }

        private int countdown;

        private void run()
        {
            if(flag == 1)
            {
                if (Callback != null)
                {
                    Callback.OnStart();
                }
                try
                {
                    runOnce();
                }catch(Exception e)
                {
                    if (Callback != null)
                    {
                        Callback.OnException(e);
                    }
                }
            }
            while (flag == 1)
            {
                if (Callback != null)
                {
                    Callback.OnCountDown(countdown);
                }
                if (countdown == 0)
                {
                    try
                    {
                        runOnce();
                    }
                    catch(Exception e)
                    {
                        if (Callback != null)
                        {
                            Callback.OnException(e);
                        }
                    }
                    countdown = taker.Interval;
                }
                Thread.Sleep(1000);
                countdown--;
            }
            if (Callback != null)
            {
                Callback.OnStop();
            }
        }

        private void runOnce()
        {
            string html = taker.takePage();
            Weibo[] newObjs = taker.paser(html);

            if (newObjs != null && newObjs.Length > 0)
            {
                if (Callback != null)
                {
                    Callback.TakeWeiboes(newObjs);
                }
            }

            Weibo[] weibos = taker.checkNew(newObjs);
            if (weibos != null && weibos.Length > 0)
            {
                if (Callback != null)
                {
                    Callback.NewWeibos(weibos, newObjs);
                }
            }
        }
    }
}
