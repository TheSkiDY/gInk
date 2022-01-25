﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Ink;

namespace gInk
{
    public partial class FormDisplay : Form
    {
        public Root Root;
        IntPtr Canvus;
        IntPtr canvusDc;
        IntPtr OneStrokeCanvus;
        IntPtr onestrokeDc;
        IntPtr BlankCanvus;
        IntPtr blankcanvusDc;
        public Graphics gCanvus;
        public Graphics gOneStrokeCanvus;
        //Bitmap ScreenBitmap;
        IntPtr hScreenBitmap;
        IntPtr memscreenDc;

        Bitmap gpButtonsImage;
        Bitmap gpPenWidthImage;

        Bitmap textInputPanelImage;

        SolidBrush TransparentBrush;
        SolidBrush SemiTransparentBrush;

        byte[] screenbits;
        byte[] lastscreenbits;

        // http://www.csharp411.com/hide-form-from-alttab/
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        public FormDisplay(Root root)
        {
            Root = root;
            InitializeComponent();

            //dla obecnego wirtualnego ekranu ustanawia się pozycja i wymiary FormDisplay
            this.Left = SystemInformation.VirtualScreen.Left;
            this.Top = SystemInformation.VirtualScreen.Top;
            //int targetbottom = 0;
            //foreach (Screen screen in Screen.AllScreens)
            //{
            //	if (screen.WorkingArea.Bottom > targetbottom)
            //		targetbottom = screen.WorkingArea.Bottom;
            //}
            //int virwidth = SystemInformation.VirtualScreen.Width;
            //this.Width = virwidth;
            //this.Height = targetbottom - this.Top;
            this.Width = SystemInformation.VirtualScreen.Width;
            this.Height = SystemInformation.VirtualScreen.Height - 2;

            //inicjacja uchwytów do pełnego płótna (Canvus) i płótna dla jednego rysunku (OneStrokeCanvus)
            //przypisowane są uchwyty dla map bitowych GDI o wymiarach równych jednemu całemu ekranowi
            Bitmap InitCanvus = new Bitmap(this.Width, this.Height);
            Canvus = InitCanvus.GetHbitmap(Color.FromArgb(0));
            OneStrokeCanvus = InitCanvus.GetHbitmap(Color.FromArgb(0));
            //BlankCanvus = InitCanvus.GetHbitmap(Color.FromArgb(0));

            //uchwyt dla całego ekranu
            IntPtr screenDc = GetDC(IntPtr.Zero);

            //tworzenie Device Contextów dla naszego ekranu (CreateCompatibleDC)
            //oraz przypisywanie uchwytów do konkretnych DC (SelectObject)
            canvusDc = CreateCompatibleDC(screenDc);
            SelectObject(canvusDc, Canvus);
            onestrokeDc = CreateCompatibleDC(screenDc);
            SelectObject(onestrokeDc, OneStrokeCanvus);

            //blankcanvusDc = CreateCompatibleDC(screenDc);
            //SelectObject(blankcanvusDc, BlankCanvus);

            //inicjacja klasy Graphics dla pełnego płótna za pomocą stworzonego wcześniej Device Context'u
            gCanvus = Graphics.FromHdc(canvusDc);
            //tryb kompozycji SourceCopy, aby naniesiony kolor po prostu przykrywał kolor pod nim
            gCanvus.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            //analogiczna inicjacja dla płótna pojedynczego
            gOneStrokeCanvus = Graphics.FromHdc(onestrokeDc);
            //gOneStrokeCanvus.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;


            if (Root.AutoScroll)
            {
                hScreenBitmap = InitCanvus.GetHbitmap(Color.FromArgb(0));
                memscreenDc = CreateCompatibleDC(screenDc);
                SelectObject(memscreenDc, hScreenBitmap);
                screenbits = new byte[50000000];
                lastscreenbits = new byte[50000000];
            }

            //zwolnienie DeviceContextu ekranu
            ReleaseDC(IntPtr.Zero, screenDc);
            InitCanvus.Dispose();

            //this.DoubleBuffered = true;

            //inicjalizacja bitmap dla toolbara i dla panelu wyboru grubości pisaka
            int gpheight = (int)(Screen.PrimaryScreen.Bounds.Height * Root.ToolbarHeight);
            gpButtonsImage = new Bitmap(2400, gpheight);
            gpPenWidthImage = new Bitmap(200, gpheight);
            textInputPanelImage = new Bitmap(400, Root.FormCollection.textInputPanel.Height);

            TransparentBrush = new SolidBrush(Color.Transparent);

            //SemiTransparentBrush używany w trybie tworzenie screenshota,
            //jest to biały kolor ekranu z odpowiednią przezroczystością
            SemiTransparentBrush = new SolidBrush(Color.FromArgb(120, 255, 255, 255));


            ToTopMostThrough();
        }

        public void ToTopMostThrough()
        {
            //ustawienie FormDisplay na pierwszą warstwę, nad resztą otwartych aplikacji

            UInt32 dwExStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, dwExStyle | 0x00080000);
            SetWindowPos(this.Handle, (IntPtr)0, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0004 | 0x0010 | 0x0020);
            //SetLayeredWindowAttributes(this.Handle, 0x00FFFFFF, 1, 0x2);
            SetWindowLong(this.Handle, -20, dwExStyle | 0x00080000 | 0x00000020);
            SetWindowPos(this.Handle, (IntPtr)(-1), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0010 | 0x0020);
        }

        public void ClearCanvus()
        {
            //metoda czyszcząca płótno za pomocą metody Clear() z klasy Graphics

            gCanvus.Clear(Color.Transparent);
        }
        public void ClearCanvus(Graphics g)
        {
            //przeciążenie metody ClearCanvus() dla konkretnej instacji klasy Graphics (domyśłnie jest dla pełnego płótna)

            g.Clear(Color.Transparent);
        }

        public void DrawSnapping(Rectangle rect)
        {
            //CompositingMode zmieniony na SourceOver, czyli zmywanie się z kolorem pod spodem
            gCanvus.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            if (rect.Width > 0 && rect.Height > 0)
            {
                //gdy trzymamy LPM wypełniają się 4 prostokąty stykające się z poszczególnymi bokami naszego prostokąta wyznaczającego wymiary screenshota, ale będące poza nim
                //otrzymany jest efekt, że wewnątrz naszego prostokąta mamy po prostu przezroczystość - widzimy co zrzucamy
                gCanvus.FillRectangle(SemiTransparentBrush, new Rectangle(0, 0, rect.Left, this.Height));
                gCanvus.FillRectangle(SemiTransparentBrush, new Rectangle(rect.Right, 0, this.Width - rect.Right, this.Height));
                gCanvus.FillRectangle(SemiTransparentBrush, new Rectangle(rect.Left, 0, rect.Width, rect.Top));
                gCanvus.FillRectangle(SemiTransparentBrush, new Rectangle(rect.Left, rect.Bottom, rect.Width, this.Height - rect.Bottom));

                //rysowanie prostokąta oznaczającego wymiary zrzutu ekranu (bardziej ciemno-szara ramka)
                Pen pen = new Pen(Color.FromArgb(200, 80, 80, 80));
                pen.Width = 3;
                gCanvus.DrawRectangle(pen, rect);
            }
            else
            {
                //gdy nie trzymamy LPM cały ekran jest pokryty SemiTransparentBrush (pół-przezroczyste białe tło)
                gCanvus.FillRectangle(SemiTransparentBrush, new Rectangle(0, 0, this.Width, this.Height));
            }

            //powrót do normalnego CompositingMode
            gCanvus.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        }

        public void DrawButtons(bool redrawbuttons, bool exiting = false)
        {
            //pokazanie na ekranie pełnej ramki toolbara (nie konkretnych przycisków)
            if (Root.AlwaysHideToolbar)
                return;

            int top, height, left, width;
            int fullwidth;
            int gpbl;
            int drawwidth;

            top = Root.FormCollection.gpButtons.Top;
            height = Root.FormCollection.gpButtons.Height;
            left = Root.FormCollection.gpButtons.Left;
            width = Root.FormCollection.gpButtons.Width;
            fullwidth = Root.FormCollection.gpButtonsWidth;
            drawwidth = width;
            gpbl = Root.FormCollection.gpButtonsLeft;
            if (left + width > gpbl + fullwidth)
                drawwidth = gpbl + fullwidth - left;

            if (redrawbuttons)
                Root.FormCollection.gpButtons.DrawToBitmap(gpButtonsImage, new Rectangle(0, 0, width, height));

            if (exiting)
            {
                int clearleft = Math.Max(left - 120, gpbl);
                //gCanvus.FillRectangle(TransparentBrush, clearleft, top, fullwidth * 2, height);
                gCanvus.FillRectangle(TransparentBrush, clearleft, top, drawwidth, height);
            }
            gCanvus.DrawImage(gpButtonsImage, left, top, new Rectangle(0, 0, drawwidth, height), GraphicsUnit.Pixel);

            //pokazanie panelu z wyborem grubości, jeśli ma być widoczny
            if (Root.gpPenWidthVisible)
            {
                top = Root.FormCollection.gpPenWidth.Top;
                height = Root.FormCollection.gpPenWidth.Height;
                left = Root.FormCollection.gpPenWidth.Left;
                width = Root.FormCollection.gpPenWidth.Width;
                if (redrawbuttons)
                    Root.FormCollection.gpPenWidth.DrawToBitmap(gpPenWidthImage, new Rectangle(0, 0, width, height));

                gCanvus.DrawImage(gpPenWidthImage, left, top);
            }

            if (Root.textInputPanelVisible)
            {
                top = Root.FormCollection.textInputPanel.Top;
                height = Root.FormCollection.textInputPanel.Height;
                left = Root.FormCollection.textInputPanel.Left;
                width = Root.FormCollection.textInputPanel.Width;
                Root.FormCollection.textInputPanel.DrawToBitmap(textInputPanelImage, new Rectangle(0, 0, width, height));

                gCanvus.DrawImage(textInputPanelImage, left, top);
            }
        }
        public void DrawButtons(Graphics g, bool redrawbuttons, bool exiting = false)
        {
            //przeciążenie metody DrawButtons() która działa na konkretnej instacji klasy Graphics
            //zawiera ten sam kod, co domyślna metoda DrawButtons()
            int top, height, left, width;
            int fullwidth;
            int gpbl;
            int drawwidth;

            top = Root.FormCollection.gpButtons.Top;
            height = Root.FormCollection.gpButtons.Height;
            left = Root.FormCollection.gpButtons.Left;
            width = Root.FormCollection.gpButtons.Width;
            fullwidth = Root.FormCollection.gpButtonsWidth;
            drawwidth = width;
            gpbl = Root.FormCollection.gpButtonsLeft;
            if (left + width > gpbl + fullwidth)
                drawwidth = gpbl + fullwidth - left;

            if (redrawbuttons)
                Root.FormCollection.gpButtons.DrawToBitmap(gpButtonsImage, new Rectangle(0, 0, width, height));

            if (exiting)
            {
                int clearleft = Math.Max(left - 120, gpbl);
                //g.FillRectangle(TransparentBrush, clearleft, top, width + 80, height);
                g.FillRectangle(TransparentBrush, clearleft, top, drawwidth, height);
            }
            g.DrawImage(gpButtonsImage, left, top);

            if (Root.gpPenWidthVisible)
            {
                top = Root.FormCollection.gpPenWidth.Top;
                height = Root.FormCollection.gpPenWidth.Height;
                left = Root.FormCollection.gpPenWidth.Left;
                width = Root.FormCollection.gpPenWidth.Width;
                if (redrawbuttons)
                    Root.FormCollection.gpPenWidth.DrawToBitmap(gpPenWidthImage, new Rectangle(0, 0, width, height));

                g.DrawImage(gpPenWidthImage, left, top);
            }

            if (Root.textInputPanelVisible)
            {
                top = Root.FormCollection.textInputPanel.Top;
                left = Root.FormCollection.textInputPanel.Left;
                height = Root.FormCollection.textInputPanel.Height;
                width = Root.FormCollection.textInputPanel.Width;
                if (redrawbuttons)
                    Root.FormCollection.textInputPanel.DrawToBitmap(textInputPanelImage, new Rectangle(0, 0, width, height));

                g.DrawImage(textInputPanelImage, left, top);
            }
        }


        public void DrawStrokes()
        {
            //nanoszenie na ekran wszystkich rysunków (podczas, gdy nie jest aktywne ukrycie ich poprzez przycisk)
            DrawStrokes(gCanvus);
        }


        public void DrawStrokes(Graphics g)
        {
            //przeciążenie DrawStrokes() dla konkretnej instancji klasy Graphics
            if (Root.InkVisible)
            {
                Root.FormCollection.IC.Renderer.Draw(g, Root.FormCollection.IC.Ink.Strokes);
                foreach (Stroke stroke in Root.FormCollection.IC.Ink.Strokes)
                {
                    if (stroke.ExtendedProperties.Contains(Root.TextGuid))
                    {
                        gCanvus.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                        string text = (string)stroke.ExtendedProperties[Root.TextGuid].Data;
                        int fontIndex = (int)stroke.ExtendedProperties[Root.FontGuid].Data;
                        int fontSize = (int)stroke.ExtendedProperties[Root.FontSizeGuid].Data;
                        Font testFont = new Font(Root.IFC.Families[fontIndex].Name, fontSize);

                        Point point = stroke.GetPoint(0);
                        Root.FormCollection.IC.Renderer.InkSpaceToPixel(g, ref point);

                        SolidBrush solidBrush = new SolidBrush(stroke.DrawingAttributes.Color);
                        g.DrawString(text, testFont, solidBrush, point.X, point.Y);
                        gCanvus.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    }
                }
            }
        }

        public void MoveStrokes(int dy)
        {
            //metoda używana przy AutoScroll, by rysunki przenosiły się wraz ze scrollowaniem myszką
            //funkcja AutoScroll nie ma jednak opcji uruchomienia z poziomu użytkownika
            Point pt1 = new Point(0, 0);
            Point pt2 = new Point(0, 100);
            Root.FormCollection.IC.Renderer.PixelToInkSpace(gCanvus, ref pt1);
            Root.FormCollection.IC.Renderer.PixelToInkSpace(gCanvus, ref pt2);
            float unitperpixel = (pt2.Y - pt1.Y) / 100.0f;
            float shouldmove = dy * unitperpixel;
            foreach (Stroke stroke in Root.FormCollection.IC.Ink.Strokes)
                if (!stroke.Deleted)
                    stroke.Move(0, shouldmove);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            UpdateFormDisplay(true);
        }


        public uint N1(int i, int j)
        {
            //return BitConverter.ToUInt32(screenbits, (this.Width * j + i) * 4);
            Nlastp1 = (this.Width * j + i) * 4 + 1;
            return screenbits[Nlastp1];
        }
        public uint N2(int i, int j)
        {
            //return BitConverter.ToUInt32(screenbits, (this.Width * j + i) * 4);
            Nlastp2 = (this.Width * j + i) * 4 + 1;
            return screenbits[Nlastp2];
        }
        public uint L(int i, int j)
        {
            //return BitConverter.ToUInt32(lastscreenbits, (this.Width * j + i) * 4);
            Llastp = (this.Width * j + i) * 4 + 1;
            return lastscreenbits[Llastp];
        }
        int Nlastp1, Nlastp2, Llastp;
        public uint Nnext1()
        {
            Nlastp1 += 40;
            return screenbits[Nlastp1];
        }
        public uint Nnext2()
        {
            Nlastp2 += 40;
            return screenbits[Nlastp2];
        }
        public uint Lnext()
        {
            Llastp += 40;
            return lastscreenbits[Llastp];
        }

        public void SnapShot(Rectangle rect)
        {
            string snapbasepath = Root.SnapshotBasePath;
            snapbasepath = Environment.ExpandEnvironmentVariables(snapbasepath);
            if (Root.SnapshotBasePath == "%USERPROFILE%/Pictures/gInk/")
                if (!System.IO.Directory.Exists(snapbasepath))
                    System.IO.Directory.CreateDirectory(snapbasepath);

            if (System.IO.Directory.Exists(snapbasepath))
            {
                IntPtr screenDc = GetDC(IntPtr.Zero);

                const int VERTRES = 10;
                const int DESKTOPVERTRES = 117;
                int LogicalScreenHeight = GetDeviceCaps(screenDc, VERTRES);
                int PhysicalScreenHeight = GetDeviceCaps(screenDc, DESKTOPVERTRES);
                float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

                rect.X = (int)(rect.X * ScreenScalingFactor);
                rect.Y = (int)(rect.Y * ScreenScalingFactor);
                rect.Width = (int)(rect.Width * ScreenScalingFactor);
                rect.Height = (int)(rect.Height * ScreenScalingFactor);


                Bitmap tempbmp = new Bitmap(rect.Width, rect.Height);
                Graphics g = Graphics.FromImage(tempbmp);
                g.Clear(Color.Red);

                IntPtr hDest = CreateCompatibleDC(screenDc);
                IntPtr hBmp = tempbmp.GetHbitmap();
                SelectObject(hDest, hBmp);
                bool b = BitBlt(hDest, 0, 0, rect.Width, rect.Height, screenDc, rect.Left, rect.Top, (uint)(CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt));
                tempbmp = Bitmap.FromHbitmap(hBmp);

                if (!b)
                {
                    g = Graphics.FromImage(tempbmp);
                    g.Clear(Color.Blue);
                    g.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(rect.Width, rect.Height));
                }

                Clipboard.SetImage(tempbmp);
                DateTime now = DateTime.Now;
                string nowstr = now.Year.ToString() + "-" + now.Month.ToString("D2") + "-" + now.Day.ToString("D2") + " " + now.Hour.ToString("D2") + "-" + now.Minute.ToString("D2") + "-" + now.Second.ToString("D2");
                string savefilename = nowstr + ".png";
                Root.SnapshotFileFullPath = snapbasepath + savefilename;

                tempbmp.Save(Root.SnapshotFileFullPath, System.Drawing.Imaging.ImageFormat.Png);

                tempbmp.Dispose();
                DeleteObject(hBmp);
                ReleaseDC(IntPtr.Zero, screenDc);
                DeleteDC(hDest);

                Root.UponBalloonSnap = true;
            }
        }

        public int Test()
        {
            IntPtr screenDc = GetDC(IntPtr.Zero);

            // big time consuming, but not CPU consuming
            BitBlt(memscreenDc, Width / 4, 0, Width / 2, this.Height, screenDc, Width / 4, 0, 0x00CC0020);
            // <1% CPU
            GetBitmapBits(hScreenBitmap, this.Width * this.Height * 4, screenbits);

            int dj;
            int maxidpixels = 0;
            float maxidchdrio = 0;
            int maxdj = 0;


            // 25% CPU with 1x10x10 sample rate?
            int istart = Width / 2 - Width / 4;
            int iend = Width / 2 + Width / 4;
            for (dj = -Height * 3 / 8 + 1; dj < Height * 3 / 8 - 1; dj++)
            {
                int chdpixels = 0, idpixels = 0;
                for (int j = Height / 2 - Height / 8; j < Height / 2 + Height / 8; j += 10)
                {
                    L(istart - 10, j);
                    N1(istart - 10, j);
                    N2(istart - 10, j + dj);
                    for (int i = istart; i < iend; i += 10)
                    {
                        //uint l = Lnext();
                        //uint n1 = Nnext1();
                        //uint n2 = Nnext2();
                        //if (l != n1)
                        //{
                        //	chdpixels++;
                        //	if (l == n2)
                        //		idpixels++;
                        //}


                        if (Lnext() == Nnext2())
                            idpixels++;
                    }
                }

                //float idchdrio = (float)idpixels / chdpixels;
                if (idpixels > maxidpixels)
                //if (idchdrio > maxidchdrio)
                {
                    //maxidchdrio = idchdrio;
                    maxidpixels = idpixels;
                    maxdj = dj;
                }
            }

            //if (maxidchdrio < 0.1 || maxidpixels < 30)
            if (maxidpixels < 100)
                maxdj = 0;


            // 2% CPU
            IntPtr pscreenbits = Marshal.UnsafeAddrOfPinnedArrayElement(screenbits, (int)(this.Width * this.Height * 4 * 0.375));
            IntPtr plastscreenbits = Marshal.UnsafeAddrOfPinnedArrayElement(lastscreenbits, (int)(this.Width * this.Height * 4 * 0.375));
            memcpy(plastscreenbits, pscreenbits, this.Width * this.Height * 4 / 4);

            ReleaseDC(IntPtr.Zero, screenDc);
            return maxdj;
        }

        public void UpdateFormDisplay(bool draw)
        {
            IntPtr screenDc = GetDC(IntPtr.Zero);

            //Display-rectangle
            Size size = new Size(this.Width, this.Height);
            Point pointSource = new Point(0, 0);
            Point topPos = new Point(this.Left, this.Top);

            //Set up blending options
            BLENDFUNCTION blend = new BLENDFUNCTION();
            blend.BlendOp = AC_SRC_OVER;
            blend.BlendFlags = 0;
            blend.SourceConstantAlpha = 255;  // additional alpha multiplier to the whole image. value 255 means multiply with 1.
            blend.AlphaFormat = AC_SRC_ALPHA;

            if (draw)
                UpdateLayeredWindow(this.Handle, screenDc, ref topPos, ref size, canvusDc, ref pointSource, 0, ref blend, ULW_ALPHA);
            else
                UpdateLayeredWindow(this.Handle, screenDc, ref topPos, ref size, blankcanvusDc, ref pointSource, 0, ref blend, ULW_ALPHA);

            //Clean-up
            ReleaseDC(IntPtr.Zero, screenDc);
        }

        public void DrawOtherShapePreview(Graphics g)
        {
            

            Color color = Color.FromArgb(255 - Root.PenAttr[Root.CurrentPen].Transparency, Root.PenAttr[Root.CurrentPen].Color);
            Pen pen;

            //konwersja z himetric na piksele
            if (Root.PenWidthEnabled)
                pen = new Pen(color, (float)((double)Root.GlobalPenWidth / 26.45833));
            else
                pen = new Pen(color, (float)((double)Root.PenAttr[Root.CurrentPen].Width / 26.45833));
               
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            switch (Root.currentDrawingMode)
            {
                case Root.DrawingMode.Line:
                    if (Root.DrawnLine.BeginPoint.X == 0 && Root.DrawnLine.BeginPoint.Y == 0)
                        return;
                    g.DrawLine(pen, Root.DrawnLine.BeginPoint, Root.DrawnLine.EndPoint);
                    break;
                case Root.DrawingMode.Arrow:
                    if (Root.DrawnLine.BeginPoint.X == 0 && Root.DrawnLine.BeginPoint.Y == 0)
                        return;
                    double angle = Math.PI / 12.0;

                    double x1, x2, x3, x4;
                    double y1, y2, y3, y4;

                    x1 = Root.DrawnLine.BeginPoint.X;
                    x2 = Root.DrawnLine.EndPoint.X;
                    y1 = Root.DrawnLine.BeginPoint.Y;
                    y2 = Root.DrawnLine.EndPoint.Y;

                    double l1 = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
                    double l2 = l1 / 4;

                    g.DrawLine(pen, Root.DrawnLine.BeginPoint, Root.DrawnLine.EndPoint);
                    if (l1 > 4)
                    {
                        if(Root.FixedArrowLength)
                        {
                            if (l2 >= Root.ArrowLength)
                                l2 = Root.ArrowLength;
                        }

                        x3 = x2 + (l2 / l1) * ((x1 - x2) * Math.Cos(angle) + (y1 - y2) * Math.Sin(angle));
                        y3 = y2 + (l2 / l1) * ((y1 - y2) * Math.Cos(angle) - (x1 - x2) * Math.Sin(angle));
                        x4 = x2 + (l2 / l1) * ((x1 - x2) * Math.Cos(angle) - (y1 - y2) * Math.Sin(angle));
                        y4 = y2 + (l2 / l1) * ((y1 - y2) * Math.Cos(angle) + (x1 - x2) * Math.Sin(angle));

                        g.DrawLine(pen, (int)Math.Round(x2), (int)Math.Round(y2), (int)Math.Round(x3), (int)Math.Round(y3));
                        g.DrawLine(pen, (int)Math.Round(x2), (int)Math.Round(y2), (int)Math.Round(x4), (int)Math.Round(y4));
                    }


                    break;
                case Root.DrawingMode.Rectangle:
                    if (Root.DrawnRect.X == 0 && Root.DrawnRect.Y == 0)
                        return;
                    g.DrawRectangle(pen, Root.DrawnRect);
                    break;
                case Root.DrawingMode.Ellipse:
                    if (Root.DrawnRect.X == 0 && Root.DrawnRect.Y == 0)
                        return;
                    g.DrawEllipse(pen, Root.DrawnRect);
                    break;
                case Root.DrawingMode.Text:
                    if (Root.DrawnRect.X == 0 && Root.DrawnRect.Y == 0)
                        return;
                    if (Root.FitFontToRect)
                    {
                        Pen tempPen = new Pen(Color.Black, 1);
                        g.DrawRectangle(tempPen, Root.DrawnRect);
                    }
                    break;
            }
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        }

        int stackmove = 0;
        int Tick = 0;
        DateTime TickStartTime;
        private void timer1_Tick(object sender, EventArgs e)
        {
            Tick++;

            /*
			if (Tick == 1)
				TickStartTime = DateTime.Now;
			else if (Tick % 60 == 0)
			{
				Console.WriteLine(60 / (DateTime.Now - TickStartTime).TotalMilliseconds * 1000);
				TickStartTime = DateTime.Now;
			}
			*/

            if (Root.UponAllDrawingUpdate)
            {
                ClearCanvus();
                DrawStrokes();
                DrawButtons(true);
                if (Root.Snapping > 0)
                    DrawSnapping(Root.SnappingRect);
                UpdateFormDisplay(true);
                Root.UponAllDrawingUpdate = false;
            }

            else if (Root.UponTakingSnap)
            {
                if (Root.SnappingRect.Width == this.Width && Root.SnappingRect.Height == this.Height)
                    System.Threading.Thread.Sleep(200);
                ClearCanvus();
                DrawStrokes();
                //DrawButtons(false);
                UpdateFormDisplay(true);
                SnapShot(Root.SnappingRect);
                Root.UponTakingSnap = false;
                if (Root.CloseOnSnap == "true")
                {
                    Root.FormCollection.RetreatAndExit();
                }
                else if (Root.CloseOnSnap == "blankonly")
                {
                    if ((Root.FormCollection.IC.Ink.Strokes.Count == 0))
                        Root.FormCollection.RetreatAndExit();
                }
            }

            else if (Root.Snapping == 2)
            {
                if (Root.MouseMovedUnderSnapshotDragging)
                {
                    ClearCanvus();
                    DrawStrokes();
                    DrawButtons(false);
                    DrawSnapping(Root.SnappingRect);
                    UpdateFormDisplay(true);
                    Root.MouseMovedUnderSnapshotDragging = false;
                }
            }

            else if (Root.FormCollection.IC.CollectingInk && Root.EraserMode == false && Root.InkVisible)
            {
                //ClearCanvus();
                //DrawStrokes();
                //DrawButtons(false);
                //UpdateFormDisplay();
                //
                if (Root.FormCollection.IC.Ink.Strokes.Count > 0)
                {
                    Stroke stroke = Root.FormCollection.IC.Ink.Strokes[Root.FormCollection.IC.Ink.Strokes.Count - 1];
                    if (!stroke.Deleted)
                    {
                        //Rectangle box = stroke.GetBoundingBox();
                        Point lt = new Point(this.Left, this.Top);
                        Point rb = new Point(this.Right + 1, this.Bottom + 1);
                        //Root.FormCollection.IC.Renderer.InkSpaceToPixel(gCanvus, ref lt);
                        //Root.FormCollection.IC.Renderer.InkSpaceToPixel(gCanvus, ref rb);
                        BitBlt(canvusDc, lt.X, lt.Y, rb.X - lt.X, rb.Y - lt.Y, onestrokeDc, lt.X, lt.Y, (uint)CopyPixelOperation.SourceCopy);
                        if (Root.currentDrawingMode == Root.DrawingMode.Normal)
                            Root.FormCollection.IC.Renderer.Draw(gCanvus, stroke, Root.FormCollection.IC.DefaultDrawingAttributes);
                        else
                            DrawOtherShapePreview(gCanvus);
                    }
                    UpdateFormDisplay(true);
                }
            }

            else if (Root.FormCollection.IC.CollectingInk && Root.EraserMode == true)
            {
                ClearCanvus();
                DrawStrokes();
                DrawButtons(false);
                UpdateFormDisplay(true);
            }

            else if (Root.Snapping < -58)
            {
                ClearCanvus();
                DrawStrokes();
                DrawButtons(false);
                UpdateFormDisplay(true);
            }

            else if (Root.UponButtonsUpdate > 0)
            {
                if ((Root.UponButtonsUpdate & 0x2) > 0)
                    DrawButtons(true, (Root.UponButtonsUpdate & 0x4) > 0);
                else if ((Root.UponButtonsUpdate & 0x1) > 0)
                    DrawButtons(false, (Root.UponButtonsUpdate & 0x4) > 0);
                UpdateFormDisplay(true);
                Root.UponButtonsUpdate = 0;
            }

            else if (Root.UponSubPanelUpdate)
            {
                ClearCanvus();
                DrawStrokes();
                DrawButtons(false);
                UpdateFormDisplay(true);
                Root.UponSubPanelUpdate = false;
            }

            if (Root.AutoScroll && Root.PointerMode)
            {
                int moved = Test();
                stackmove += moved;

                if (stackmove != 0 && Tick % 10 == 1)
                {
                    MoveStrokes(stackmove);
                    ClearCanvus();
                    DrawStrokes();
                    DrawButtons(false);
                    UpdateFormDisplay(true);
                    stackmove = 0;
                }
            }
        }

        private void FormDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            DeleteObject(Canvus);
            //DeleteObject(BlankCanvus);
            DeleteDC(canvusDc);
            if (Root.AutoScroll)
            {
                DeleteObject(hScreenBitmap);
                DeleteDC(memscreenDc);
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern bool DeleteDC([In] IntPtr hdc);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
        static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);
        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc, uint crKey, [In] ref BLENDFUNCTION pblend, uint dwFlags);
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
        [DllImport("gdi32.dll")]
        public static extern bool StretchBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int nWidthSrc, int nHeightSrc, long dwRop);


        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;

            public BLENDFUNCTION(byte op, byte flags, byte alpha, byte format)
            {
                BlendOp = op;
                BlendFlags = flags;
                SourceConstantAlpha = alpha;
                AlphaFormat = format;
            }
        }

        const int ULW_ALPHA = 2;
        const int AC_SRC_OVER = 0x00;
        const int AC_SRC_ALPHA = 0x01;


        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll", SetLastError = true)]
        static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);
        [DllImport("user32.dll")]
        public extern static bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("gdi32.dll")]
        static extern int GetBitmapBits(IntPtr hbmp, int cbBuffer, [Out] byte[] lpvBits);
        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);

    }
}
