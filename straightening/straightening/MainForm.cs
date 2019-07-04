/*
 * 由SharpDevelop创建。
 * 用户： 杜亚达
 * 日期: 2019/2/13
 * 时间: 17:23
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace straightening
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		public static Bitmap ToGray(Bitmap bmp)
	    {
	    	for (int i = 0; i < bmp.Width; i++)
	    	{
	      		for (int j = 0; j < bmp.Height; j++)
	      			{
	          			//获取该点的像素的RGB的颜色
	          			Color color = bmp.GetPixel(i, j);
	          			//利用公式计算灰度值
	          			//int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
	          			int gray = (int)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
	          			Color newColor = Color.FromArgb(gray, gray, gray);
	          			bmp.SetPixel(i, j, newColor);
	      			}
	    	}
	    	return bmp;
	    }
		/// <summary>
		/// 打开图片
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Button1Click(object sender, EventArgs e)
		{
			this.openFileDialog1.ShowDialog();
//选择文件后，用openFileDialog1的FileName属性获取文件的绝对路径
			this.textBox1.Text = this.openFileDialog1.FileName;	
			
			pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);

//显示图片
//			if (DialogResult.OK == openFileDialog1.ShowDialog())
//
//            {
//
//                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);

//                foreach (string Path in Directory.GetFiles(System.IO.Path.GetDirectoryName(openFileDialog1.FileName)))
//
//                {
//
//                    ImagePaths.Add(Path);
//
//                }
//
//                if (ImagePaths.Count != 0)
//
//                {
//
//                    ImageCount = ImagePaths.Count;
//
//                }

//            }
			
		}
		/// <summary>
		/// 图像处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Button2Click(object sender, EventArgs e)
		{
//			Bitmap myBitmap = new Bitmap(this.openFileDialog1.FileName);
//			//获取图像
//			pictureBox2.Image = Image.FromFile(openFileDialog1.FileName);
			
			Bitmap img1 = (Bitmap)pictureBox1.Image;
			pictureBox2.Image = GrayByPixels(img1);
			//pictureBox2.Image = ToGray(img1);
			Double d = 0.55;
			Bitmap img2 = (Bitmap)pictureBox2.Image;
			pictureBox3.Image = BitmapToBlack1(img2, d);
			//pictureBox3.Image = BitmapToBlack2(img2);//ConvertTo1Bpp1(img2);//二值化2
			//边缘检测
			Bitmap img3 = (Bitmap)pictureBox3.Image;
//			pictureBox4.Image = sobel(img3);
			pictureBox4.Image = robert(img3);
			
//			Bitmap img4 = (Bitmap)pictureBox4.Image;
//			pictureBox5.Image = HoughLineResult(img4);
			//矫正
			Bitmap bmpIn = (Bitmap)pictureBox4.Image;
			gmseDeskew sk = new gmseDeskew(bmpIn);
			double skewangle = sk.GetSkewAngle();
			pictureBox5.Image = RotateImage(bmpIn, -skewangle);
			//裁剪
			//Bitmap img5 = (Bitmap)pictureBox5.Image;
			//pictureBox6.Image = CutImage(img5);
			//CloneImage(0, 0, 500, 200);
			Bitmap img5 = (Bitmap)pictureBox5.Image;
			pictureBox6.Image = GetResultImage(img5);
			if (pictureBox6 == null)
				MessageBox.Show("没有生成图像");
		}
				
		/// <summary>
	    /// 图像灰度化
	    /// </summary>
	    /// <param name="bmp"></param>
	    /// <returns></returns>
	    
		/// <summary>
		/// 图像灰度化
		/// </summary>
		/// <param name="bmpcode"></param>
		/// <returns></returns>
		public Bitmap GrayByPixels(Bitmap bmpcode)
        {
        	bmpcode = new Bitmap(bmpcode);
        	for (int i = 0; i < bmpcode.Height; i++)
        	{
        		for (int j = 0; j < bmpcode.Width; j++)
        		{
        			int tmpValue = GetGrayNumColor(bmpcode.GetPixel(j, i));
        			bmpcode.SetPixel(j, i, Color.FromArgb(tmpValue, tmpValue, tmpValue));
        		}
        	}
        	return bmpcode;
        }
		/// <summary>
		/// 根据RGB，计算灰度值
		/// </summary>
		/// <param name="codecolor"></param>
		/// <returns></returns>
        private int GetGrayNumColor(System.Drawing.Color codecolor)
        {
        	return (codecolor.R * 19595 + codecolor.G * 38469 + codecolor.B * 7472) >> 16;
        }
        
        /// <summary>
        /// 二值化1
        /// </summary>
        /// <param name="img"></param>
        /// <param name="hsb"></param>
        /// <returns></returns>
        public static Bitmap BitmapToBlack1(Bitmap img, Double hsb)
        {
        	int w = img.Width;
        	int h = img.Height;
        	Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
        	BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);//将 Bitmap 锁定到系统内存中
        	for (int y = 0; y < h; y++)
        	{
        		byte[] scan = new byte[(w + 7) / 8];
        		for (int x = 0; x < w; x++)
        		{
        			Color c = img.GetPixel(x, y);
        
        			if (c.GetBrightness() >= hsb) scan[x / 8] |= (byte)(0x80 >> (x % 8));//亮度值和原来比较，二值化处理
        		}
        		Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);
        	}
        	bmp.UnlockBits(data);//将 Bitmap 锁定到系统内存中
        	return bmp;
        }
        
        ///二值化2
        public static Bitmap BitmapToBlack2(Bitmap bmp)//ConvertTo1Bpp1(Bitmap bmp)
        {
            int average = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    average += color.B;                    
                }
            }
            average = (int)average / (bmp.Width * bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    int value = 255 - color.B;
                    Color newColor = value > average ? Color.FromArgb(0, 0, 0): Color.FromArgb(255,255, 255);                   
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }
		///图像边缘化
		//定义roberts算子函数
		private static Bitmap robert(Bitmap a)
		{
			int w = a.Width;
			int h = a.Height;
			try
			{
				Bitmap dstBitmap = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				System.Drawing.Imaging.BitmapData srcData = a.LockBits(new Rectangle
				                                                       (0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				System.Drawing.Imaging.BitmapData dstData = dstBitmap.LockBits(new Rectangle
				                                                               (0, 0, w, h), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				unsafe
				{
					byte* pIn = (byte*)srcData.Scan0.ToPointer();
					byte* pOut = (byte*)dstData.Scan0.ToPointer();
					byte* p;
					int stride = srcData.Stride;
					for (int y = 0; y < h; y++)
					{
						for (int x = 0; x < w; x++)
						{
							//边缘八个点像素不变
							if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
							{
								pOut[0] = pIn[0];
								pOut[1] = pIn[1];
								pOut[2] = pIn[2];
							}
							else
							{
								int r0, r5, r6, r7;
								int g5, g6, g7, g0;
								int b5, b6, b7, b0;
								double vR, vG, vB;
								//右
								p = pIn + 3;
								r5 = p[2];
								g5 = p[1];
								b5 = p[0];
								//左下
								p = pIn + stride - 3;
								r6 = p[2];
								g6 = p[1];
								b6 = p[0];
								//正下
								p = pIn + stride;
								r7 = p[2];
								g7 = p[1];
								b7 = p[0];
								//中心点
								p = pIn;
								r0 = p[2];
								g0 = p[1];
								b0 = p[0];
								vR = (double)(Math .Abs (r0-r5)+Math .Abs ( r5-r7));
								vG = (double)(Math.Abs(g0 - g5) + Math.Abs(g5 - g7));
								vB = (double)(Math.Abs(b0 - b5) + Math.Abs(b5 - b7));
								if (vR > 0)
								{
									vR = Math.Min(255, vR);
								}
								else
								{
									vR = Math.Max(0, vR);
								}
								if (vG > 0)
								{
									vG = Math.Min(255, vG);
								}
								else
								{
									vG = Math.Max(0, vG);
								}
								if (vB > 0)
								{
									vB = Math.Min(255, vB);
								}
								else
								{
									vB = Math.Max(0, vB);
								}
								pOut[0] = (byte)vB;
								pOut[1] = (byte)vG;
								pOut[2] = (byte)vR;
							}
							pIn += 3;
							pOut += 3;
						}
						pIn += srcData.Stride - w * 3;
						pOut += srcData.Stride - w * 3;
					}
				}
				a.UnlockBits(srcData);
				dstBitmap.UnlockBits(dstData);
				return dstBitmap;
			}
			catch
			{
				return null;
			}
		}
		//定义smoothed算子边缘检测函数
//		private static Bitmap smoothed(Bitmap a)
//		{
//			int w = a.Width;
//			int h = a.Height;
//			try
//			{
//				Bitmap dstBitmap = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
//				System.Drawing.Imaging.BitmapData srcData = a.LockBits(new Rectangle
//				                                                       (0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
//				System.Drawing.Imaging.BitmapData dstData = dstBitmap.LockBits(new Rectangle
//				                                                               (0, 0, w, h), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
//				unsafe
//				{
//					byte* pIn = (byte*)srcData.Scan0.ToPointer();
//					byte* pOut = (byte*)dstData.Scan0.ToPointer();
//					byte* p;
//					int stride = srcData.Stride;
//					for (int y = 0; y < h; y++)
//					{
//						for (int x = 0; x < w; x++)
//						{
//							//边缘八个点像素不变
//							if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
//							{
//								pOut[0] = pIn[0];
//								pOut[1] = pIn[1];
//								pOut[2] = pIn[2];
//							}
//							else
//							{
//								int r0, r1, r2, r3, r4, r5, r6, r7, r8;
//								int g1, g2, g3, g4, g5, g6, g7, g8, g0;
//								int b1, b2, b3, b4, b5, b6, b7, b8, b0;
//								double vR, vG, vB;
//								//左上
//								p = pIn - stride - 3;
//								r1 = p[2];
//								g1 = p[1];
//								b1 = p[0];
//								//正上
//								p = pIn - stride;
//								r2 = p[2];
//								g2 = p[1];
//								b2 = p[0];
//								//右上
//								p = pIn - stride + 3;
//								r3 = p[2];
//								g3 = p[1];
//								b3 = p[0];
//								//左
//								p = pIn - 3;
//								r4 = p[2];
//								g4 = p[1];
//								b4 = p[0];
//								//右
//								p = pIn + 3;
//								r5 = p[2];
//								g5 = p[1];
//								b5 = p[0];
//								//左下
//								p = pIn + stride - 3;
//								r6 = p[2];
//								g6 = p[1];
//								b6 = p[0];
//								//正下
//								p = pIn + stride;
//								r7 = p[2];
//								g7 = p[1];
//								b7 = p[0];
//								// 右下
//								p = pIn + stride + 3;
//								r8 = p[2];
//								g8 = p[1];
//								b8 = p[0];
//								//中心点
//								p = pIn;
//								r0 = p[2];
//								g0 = p[1];
//								b0 = p[0];
//								//使用模板
//								vR = (double)(Math.Abs(r3+r5+r8-r1-r4-r6) + Math .Abs (r1+r2+r3-r6-r7-r8));
//								vG = (double)(Math.Abs(g3+g5+g8-g1-g4-g6) + Math .Abs (g1+g2+g3-g6-g7-g8));
//								vB = (double)(Math.Abs(b3+b5+b8-b1-b4-b6) + Math. Abs (b1+b2+b3-b6-b7-b8));
//								if (vR > 0)
//								{
//									vR = Math.Min(255, vR);
//								}
//								else
//								{
//									vR = Math.Max(0, vR);
//								}
//								if (vG > 0)
//								{
//									vG = Math.Min(255, vG);
//								}
//								else
//								{
//									vG = Math.Max(0, vG);
//								}
//								if (vB > 0)
//								{
//									vB = Math.Min(255, vB);
//								}
//								else
//								{
//									vB = Math.Max(0, vB);
//								}
//								pOut[0] = (byte)vB;
//								pOut[1] = (byte)vG;
//								pOut[2] = (byte)vR;
//							}
//							pIn += 3;
//							pOut += 3;
//						}
//						pIn += srcData.Stride - w * 3;
//						pOut += srcData.Stride - w * 3;
//					}
//				}
//				a.UnlockBits(srcData);
//				dstBitmap.UnlockBits(dstData);
//				return dstBitmap;
//			}
//			catch
//			{
//				return null;
//			}
//		}
		//定义sobel算子函数
		private static Bitmap sobel(Bitmap a)
		{
			int w = a.Width;
			int h = a.Height;
			try
			{
				Bitmap dstBitmap = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				System.Drawing.Imaging.BitmapData srcData = a.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				System.Drawing.Imaging.BitmapData dstData = dstBitmap.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				unsafe
				{
					byte* pIn = (byte*)srcData.Scan0.ToPointer();
					byte* pOut = (byte*)dstData.Scan0.ToPointer();
					byte* p;
					int stride = srcData.Stride;
					for (int y = 0; y < h; y++)
					{
						for (int x = 0; x < w; x++)
						{
							//边缘八个点像素不变
							if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
							{
								pOut[0] = pIn[0];
								pOut[1] = pIn[1];
								pOut[2] = pIn[2];
							}
							else
							{
								int r0, r1, r2, r3, r4, r5, r6, r7, r8;
								int g1, g2, g3, g4, g5, g6, g7, g8, g0;
								int b1, b2, b3, b4, b5, b6, b7, b8, b0;
								double vR, vG, vB;
								//左上
								p = pIn - stride - 3;
								r1 = p[2];
								g1 = p[1];
								b1 = p[0];
								//正上
								p = pIn - stride;
								r2 = p[2];
								g2 = p[1];
								b2 = p[0];
								//右上
								p = pIn - stride + 3;
								r3 = p[2];
								g3 = p[1];
								b3 = p[0];
								//左
								p = pIn - 3;
								r4 = p[2];
								g4 = p[1];
								b4 = p[0];
								//右
								p = pIn + 3;
								r5 = p[2];
								g5 = p[1];
								b5 = p[0];
								//左下
								p = pIn + stride - 3;
								r6 = p[2];
								g6 = p[1];
								b6 = p[0];
								//正下
								p = pIn + stride;
								r7 = p[2];
								g7 = p[1];
								b7 = p[0];
								// 右下
								p = pIn + stride + 3;
								r8 = p[2];
								g8 = p[1];
								b8 = p[0];
								//中心点
								p = pIn;
								r0 = p[2];
								g0 = p[1];
								b0 = p[0];
								//使用模板
								vR = (double)(Math .Abs (r1+2*r4+r6-r3-2*r5-r8)+Math .Abs (r1+2*r2+r3-r6-2*r7-r8));
								vG = (double)(Math .Abs (g1+2*g4+g6-g3-2*g5-g8)+Math .Abs (g1+2*g2+g3-g6-2*g7-g8));
								vB = (double)(Math .Abs (b1+2*b4+b6-b3-2*b5-b8)+Math .Abs (b1+2*b2+b3-b6-2*b7-b8));
								if (vR > 0)
								{
									vR = Math.Min(255, vR);
								}
								else
								{
									vR = Math.Max(0, vR);
								}
								if (vG > 0)
								{
									vG = Math.Min(255, vG);
								}
								else
								{
									vG = Math.Max(0, vG);
								}
								if (vB > 0)
								{
									vB = Math.Min(255, vB);
								}
								else
								{
									vB = Math.Max(0, vB);
								}
								pOut[0] = (byte)vB;
								pOut[1] = (byte)vG;
								pOut[2] = (byte)vR;
							}
							pIn += 3;
							pOut += 3;
						}
						pIn += srcData.Stride - w * 3;
						pOut += srcData.Stride - w * 3;
					}
				}
				a.UnlockBits(srcData);
				dstBitmap.UnlockBits(dstData);
				return dstBitmap;
			}
			catch
			{
				return null;
			}
		}
		
		///hough变换
		private Bitmap RotateImage(Bitmap bmp, double angle)//旋转图像
		{
			Graphics g = null;
			Bitmap tmp = new Bitmap(bmp.Width+200, bmp.Height+100, PixelFormat.Format32bppRgb);
			tmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
			g = Graphics.FromImage(tmp);
			try
			{
				g.FillRectangle(Brushes.Black/*White*/, 0, 0, bmp.Width+100, bmp.Height+100);//背景颜色
				g.RotateTransform((float)angle);
				g.DrawImage(bmp, 100, 50);
			}
			finally
			{
				g.Dispose();
			}
			return tmp;
		}

		#region 算法处理类

		public class gmseDeskew
		{
			public class HougLine
			{
				// Count of points in the line.
				public int Count;
				// Index in Matrix.
				public int Index;
				// The line is represented as all x,y that solve y*cos(alpha)-x*sin(alpha)=d
				public double Alpha;
				public double d;
			}
			Bitmap cBmp;
			double cAlphaStart = -20;
			double cAlphaStep = 0.2;
			int cSteps = 40 * 5;
			double[] cSinA;
			double[] cCosA;
			double cDMin;
			double cDStep = 1;
			int cDCount;
			// Count of points that fit in a line.
			int[] cHMatrix;
			public double GetSkewAngle()
			{
				gmseDeskew.HougLine[] hl = null;
				int i = 0;
				double sum = 0;
				int count = 0;
				// Hough Transformation

				Calc();
				// Top 20 of the detected lines in the image.
				hl = GetTop(20);
				// Average angle of the lines
				//for (i = 0; i <= 19; i++)
				for (i = 19; i >= 0; i--)
				{
					sum += hl[i].Alpha;
					count += 1;
				}
				return sum / count;
			}
			private HougLine[] GetTop(int Count)//计算直线
			{
				HougLine[] hl = null;
				int i = 0;
				int j = 0;
				HougLine tmp = null;
				int AlphaIndex = 0;
				int dIndex = 0;
				hl = new HougLine[Count + 1];
				for (i = 0; i <= Count - 1; i++)
				{
					hl[i] = new HougLine();
				}
				for (i = 0; i <= cHMatrix.Length - 1; i++)
				{
					if (cHMatrix[i] > hl[Count - 1].Count)
					{
						hl[Count - 1].Count = cHMatrix[i];
						hl[Count - 1].Index = i;
						j = Count - 1;
						while (j > 0 && hl[j].Count > hl[j - 1].Count)
						{
							tmp = hl[j];
							hl[j] = hl[j - 1];
							hl[j - 1] = tmp; j -= 1;
						}
					}
				}
				for (i = 0; i <= Count - 1; i++)
				{
					dIndex = hl[i].Index / cSteps;
					AlphaIndex = hl[i].Index - dIndex * cSteps;
					hl[i].Alpha = GetAlpha(AlphaIndex);
					hl[i].d = dIndex + cDMin;
				}
				return hl;
			}
			public gmseDeskew(Bitmap bmp)
			{
				cBmp = bmp;
			}
			private void Calc()
			{
				int x = 0;
				int y = 0;
				int hMin = cBmp.Height / 4;
				int hMax = cBmp.Height * 3 / 4;
				Init();
				//for (y = hMin; y <= hMax; y++)
				for (y = hMax; y >= hMin; y--)
				{
					for (x = 1; x <= cBmp.Width - 2; x++)
					{    // Only lower edges are considered.
						if (IsBlack(x, y))
						{
							if (!IsBlack(x, y + 1))
							{
								Calc(x, y);
							}
						}
					}
				}
			}
			private void Calc(int x, int y)
			{
				int alpha = 0;
				double d = 0;
				int dIndex = 0;
				int Index = 0;
				for (alpha = 0; alpha <= cSteps - 1; alpha++)
				{
					d = y * cCosA[alpha] - x * cSinA[alpha];
					dIndex = (int)CalcDIndex(d);
					Index = dIndex * cSteps + alpha;
					try
					{
						cHMatrix[Index] += 1;
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex.ToString());
					}
				}
			}
			private double CalcDIndex(double d)
			{
				return Convert.ToInt32(d - cDMin);
			}
			private bool IsBlack(int x, int y)//图像灰度化
			{
				Color c = default(Color);
				double luminance = 0;
				c = cBmp.GetPixel(x, y);
				luminance = (c.R * 0.299) + (c.G * 0.587) + (c.B * 0.114);
				return luminance < 140;
			}
			private void Init()
			{
				int i = 0;
				double angle = 0;
				// Precalculation of sin and cos.
				cSinA = new double[cSteps];
				cCosA = new double[cSteps];
				for (i = 0; i <= cSteps - 1; i++)
				{
					angle = GetAlpha(i) * Math.PI / 180.0;
					cSinA[i] = Math.Sin(angle);
					cCosA[i] = Math.Cos(angle);
				}  // Range of d:
				cDMin = -cBmp.Width;
				cDCount = (int)(2 * (cBmp.Width + cBmp.Height) / cDStep);
				cHMatrix = new int[cDCount * cSteps + 1];
			}
			public double GetAlpha(int Index)
			{
				return cAlphaStart + Index * cAlphaStep;
			}
		}

		#endregion 类结束
		///裁剪
		private Bitmap GetResultImage(Image img) 
		{
			try {
				using (Bitmap bmp=new Bitmap(img)) 
				{
					Point p_min=new Point(0,0), p_max=new Point(0,0);
					int min = 0, max = 0;
					for (int x=0;x<bmp.Width;x++) 
					{
						for (int y=0;y<bmp.Height;y++) 
						{
							Color c = bmp.GetPixel(x, y);
							if (Convert.ToInt32(c.R) > 200 && Convert.ToInt32(c.G) > 200 && Convert.ToInt32(c.B) > 200)
							{
								if (min == 0 || max == 0)
								{
									p_min = new Point(x, y);
									p_max = new Point(x, y);
									min = x + y;
									max = x + y;
								}
								else {
									if (x + y < min) {
										min = x + y;
										p_min = new Point(x, y);
									} else if (x+y>max) {
										max = x + y;
										p_max = new Point(x,y);
									}
								}
							}
						}
					}
					Bitmap bmpResult = new Bitmap(bmp.Width, bmp.Height);
					TextureBrush brush = new TextureBrush(img);
					GraphicsPath path = new GraphicsPath();
					//path.AddPolygon(new[] { p_max/*p_min*/, new Point(p_max.X,p_min.Y), p_min/*p_max*/, new Point(p_min.X,p_max.Y) });
					path.AddPolygon(new[] { new Point(p_min.X-40,p_min.Y-10), new Point(p_max.X+40,p_min.Y-10), new Point(p_max.X+40,p_max.Y), new Point(p_min.X-40,p_max.Y) });
					Graphics g = Graphics.FromImage(bmpResult);
					g.Clear(Color.Transparent);
					g.FillPath(brush, path);
					return bmpResult;

				}
			} 
			catch 
			{
				return null;
			}
		}

		
		void Button3Click(object sender, EventArgs e)
		{
			if (pictureBox5.Image != null)
			{
				Bitmap box = new Bitmap(pictureBox5.Image);
				SaveFileDialog sv1 = new SaveFileDialog();
				sv1.Filter = "png文件|*.png";
				sv1.ShowDialog();
				string str = sv1.FileName;
				box.Save(str, System.Drawing.Imaging.ImageFormat.Bmp);
				//pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
			}
			else MessageBox.Show("没有生成图像");
		}
		//裁剪
		void Button4Click(object sender, EventArgs e)
		{	
			Bitmap img5 = (Bitmap)pictureBox5.Image;
			pictureBox6.Image = GetResultImage(img5);
			if (pictureBox6 == null)
				MessageBox.Show("没有生成图像");
		}
		
		void MainFormLoad(object sender, EventArgs e)
		{
			
		}
	}
}