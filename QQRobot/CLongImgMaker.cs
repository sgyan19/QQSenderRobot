using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    class CLongImgMaker
    {
        public const int WidthPix = 440;
        public static Image make(Image[] src)
        {
            if(src.Length <= 0)
            {
                return null;
            }
            Image[] scaleSrc = new Image[src.Length];
            int[] heights = new int[src.Length];
            int height = 0;

            for (int i = 0; i < src.Length; i++)
            {
                heights[i] = (int)(WidthPix * src[i].Height / (float)src[i].Width + 0.5);
                height += heights[i];
            }

            Bitmap result = new Bitmap(WidthPix, height);
            Graphics g = Graphics.FromImage(result);

            int height_pos = 0;
            for (int i = 0; i < src.Length; i++)
            {
                g.DrawImage(src[i], 0, height_pos, WidthPix, heights[i]);
                height_pos += heights[i];
            }
            g.Dispose();

            return result;
        }
    }
}
