using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    class ImageHelper
    {
        public const int WidthPix = 440;

        public static string download(WebClient wb, string url)
        {
            int index = url.LastIndexOf('/');
            string name = url.Substring(index + 1, url.Length - index - 1);
            if (name.Length > 100)
            {
                name = name.Substring(0, 100);
            }
            if (!Directory.Exists("tmp"))
            {
                Directory.CreateDirectory("tmp");
            }
            string path = "tmp\\" + name;
            if (!File.Exists(path))
            {
                try
                {
                    wb.DownloadFile(url, path);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            return path;
        }
        
        public static Image download(WebClient wb, string url, int times)
        {
            int index = url.LastIndexOf('/');
            string name = url.Substring(index + 1, url.Length - index - 1);
            if (name.Length > 100)
            {
                name = name.Substring(0, 100);
            }
            if (!Directory.Exists("tmp"))
            {
                Directory.CreateDirectory("tmp");
            }
            string path = "tmp\\" + name;
        download:
            if (!File.Exists(path))
            {
                try
                {
                    wb.DownloadFile(url, path);
                }
                catch (Exception e)
                {
                    if (times > 0)
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        times--;
                        goto download;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            Image image = null;
            try
            {
                image = Image.FromFile(path);
            }
            catch (Exception)
            {
                if (times > 0)
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    times--;
                    goto download;
                }
            }
            return image;
        }

        public static Image longImageMake(Image[] src)
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

        public static string save(Image img)
        {
            if(img == null)
            {
                return null;
            }
            string path ;
            if (!Directory.Exists("tmp"))
            {
                Directory.CreateDirectory("tmp");
            }
            path = "tmp\\" + DateTime.Now.Ticks.ToString();
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }catch(Exception e)
                {

                }
            }
            img.Save(path);
            return path;
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
