using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQRobot
{
    class UiShower
    {
        public Form1 mainForm;
        public Label statusLabel;
        public Label countDownLabel;
        public TextBox takeInfo;
        public Button readConfigBtn;
        public Button startBtn;
        public Button stopBtn;
        public Button doBtn;
        public Label sendCountLabel;

        public UiShower(Form1 f)
        {
            mainForm = f;
        }

        public void showCountDown(string text)
        {
            string[] args = new string[1];
            args[0] = text;
            mainForm.Invoke(new CountDown(countDown), args);
        }

        public void showResult(string label,string text)
        {
            string[] args = new string[2];
            args[0] = label;
            args[1] = text;
            mainForm.Invoke(new SetStatus(setStatus), args);
        }

        public void showStart()
        {
            mainForm.Invoke(new OnStart(onStart));
        }

        public void showStop()
        {
            mainForm.Invoke(new Stop(stop));
        }

        public void showException(Exception e)
        {
            Exception[] args = new Exception[1];
            args[0] = e;
            mainForm.Invoke(new OnException(onException), args);
        }

        public void showCount(string count)
        {
            string[] args = new string[1];
            args[0] = count;
            mainForm.Invoke(new OnSendCount(onSendCount), args);
        }

        public delegate void SetStatus(string label, string text);
        private void setStatus(string label, string text)
        {
            statusLabel.Text = label;
            takeInfo.Text = text;
        }

        public delegate void CountDown(string text);
        private void countDown(string text)
        {
            countDownLabel.Text = text;
        }

        public delegate void Stop();
        private void stop()
        {
            //statusLabel.Text = "已停止";
            countDownLabel.Text = "停止";
            readConfigBtn.Enabled = true;
            startBtn.Enabled = true;
            stopBtn.Enabled = false;
        }

        public delegate void OnException(Exception e);
        private void onException(Exception e)
        {
            countDownLabel.Text = "抓取异常";
            takeInfo.Text = e.Message + "\r\n" + e.Source;
        }

        public delegate void OnStart();
        private void onStart()
        {
            countDownLabel.Text = "抓取中";
        }

        public delegate void OnSendCount(string countText);
        private void onSendCount(string countText)
        {
            sendCountLabel.Text = countText;
        }
    }
}
