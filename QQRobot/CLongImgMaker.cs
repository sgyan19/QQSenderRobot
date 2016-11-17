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
            int count = 0;
            foreach (Image item in src)
            {
                if(item != null)
                {
                    count++;
                }
            }
            if (count <= 0) return null;
            Image[] usSrc = new Image[count];
            for (int i = 0,j = 0; i < src.Length; i++)
            {
                if(src[i] != null)
                {
                    usSrc[j++] = src[i];
                }
                
            }
            Image[] scaleSrc = new Image[usSrc.Length];
            int[] heights = new int[usSrc.Length];
            int height = 0;

            for (int i = 0; i < usSrc.Length; i++)
            {
                heights[i] = (int)(WidthPix * usSrc[i].Height / (float)usSrc[i].Width + 0.5);
                height += heights[i];
            }

            Bitmap result = new Bitmap(WidthPix, height);
            Graphics g = Graphics.FromImage(result);

            int height_pos = 0;
            for (int i = 0; i < usSrc.Length; i++)
            {
                g.DrawImage(usSrc[i], 0, height_pos, WidthPix, heights[i]);
                height_pos += heights[i];
            }
            g.Dispose();

            return result;
        }

        public static Image changeSize(Image image, int width, int height)
        {
            if (image == null) return null;
            if((image.Width == width &&　image.Height == height) || width <=0 || height <= 0)
            {
                return image;
            }
            Bitmap rel = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(rel);
            g.DrawImage(image, 0, 0, width, height);
            g.Dispose();
            return rel;
        }
    }
}
