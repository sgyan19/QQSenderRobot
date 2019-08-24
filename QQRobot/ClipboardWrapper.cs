using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQRobot
{
    class ClipboardWrapper
    {
        public static void Clear()
        {
        retry:
            try
            {
                Clipboard.Clear();
            } catch
            {
                goto retry;
            }
        } 

        public static void SetText(string text)
        {
        retry:
            try
            {
                Clipboard.SetText(text);
            }
            catch
            {
                goto retry;
            }
        }

        public static void SetImage(Image image)
        {
        retry:
            try
            {
                Clipboard.SetImage(image);
            }
            catch
            {
                goto retry;
            }
        }
    }
}
