using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    [Serializable]
    class TwitterMsg : Object
    {
        private string textMessage;

        private Image imageData;

        public string Text
        {
            set
            {
                textMessage = value;
            }
            get
            {
                return textMessage;
            }
        }

        public Image Img
        {
            set
            {
                imageData = value;
            }
            get
            {
                return imageData;
            }
        }
    }
}
