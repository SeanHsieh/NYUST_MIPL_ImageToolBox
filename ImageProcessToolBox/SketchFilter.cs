﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessToolBox
{
    class SketchFilter : IImageProcess
    {
        //https://www.kancloud.cn/trent/hotoimagefilter/102806
        private Bitmap _SourceImage;

        public SketchFilter(Bitmap bitmap)
        {
            _SourceImage = new Grayscale(bitmap).Process();
        }

        public System.Drawing.Bitmap Process()
        {
            return SketchFilterProcess(_SourceImage, 1);
        }

        private Bitmap SketchFilterProcess(Bitmap src, int edgeIntensity)
        {   
            Bitmap dst = new Bitmap(src);
            int w = dst.Width;
            int h = dst.Height;

            BitmapData dstData = dst.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData edgeData = src.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* pGauss = (byte*)edgeData.Scan0;
                byte* pDst = (byte*)dstData.Scan0;
                int offset = dstData.Stride - w * 4;
                int gray, graySrc, grayGauss;
                for (int j = 0; j < h; j++)
                {
                    for (int i = 0; i < w; i++)
                    {
                        graySrc = (pDst[0] + pDst[1] + pDst[2]) / 3;
                        grayGauss = 255 - (pGauss[0] + pGauss[1] + pGauss[2]) / 3;
                        gray = graySrc + (graySrc * grayGauss) / (256 - grayGauss);
                        gray = Math.Min(255, Math.Max(0, gray));
                        pDst[0] = (byte)gray;
                        pDst[1] = (byte)gray;
                        pDst[2] = (byte)gray;
                        pDst[3] = (byte)255;
                        pGauss += 4;
                        pDst += 4;
                    }
                    pGauss += offset;
                    pDst += offset;
                }
            }
            
            dst.UnlockBits(dstData);
            src.UnlockBits(edgeData);
            return dst;
        }
    }
}
