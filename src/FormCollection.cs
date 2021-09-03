using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
//using System.Windows.Input;
using Microsoft.Ink;

namespace gInk
{
	public partial class FormCollection : Form
	{
		
		public Root Root;
		public InkOverlay IC;

		//przyciski odpowiadające aktywnym pisakom do wyboru
		public Button[] btPen;

		//zmienne i tablica bitmap służące do przechowywania grafik nanoszonych na przyciski w menu
		public Bitmap image_exit, image_clear, image_undo, image_snap, image_penwidth;
		public Bitmap image_dock, image_dockback;
		public Bitmap image_pencil, image_highlighter, image_pencil_act, image_highlighter_act;
		public Bitmap image_pointer, image_pointer_act;
		public Bitmap[] image_pen;
		public Bitmap[] image_pen_act;
		public Bitmap image_eraser_act, image_eraser;
		public Bitmap image_pan_act, image_pan;
		public Bitmap image_visible_not, image_visible;

		//zmienne kontrolujące podmianę kursora myszki (na czerwony przy rysowaniu, bądź na krzyżyk przy trybie zrzutu ekranu)
		public System.Windows.Forms.Cursor cursorred, cursorsnap;
		public System.Windows.Forms.Cursor cursortip;

		public int ButtonsEntering = 0;  // -1 = exiting

		public const double defaultButtonHeight = 0.88;
		public const double defaultButtonWidth = 1.0;
		public const double defaultButtonTop = 0.07;
		public const double defaultGapBetweenButtons = 0.1;

		public int gpButtonsLeft, gpButtonsTop, gpButtonsWidth, gpButtonsHeight; // the default location, fixed

		public bool gpPenWidth_MouseOn = false;

		public int PrimaryLeft, PrimaryTop;

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

		public FormCollection(Root root)
		{
			Root = root;
			InitializeComponent();

			//zmienne PrimaryLeft i PrimaryTop przechowują koordynaty lewego górnego rogu całego ekranu służącego do rysunku
			//zazwyczaj gdy używamy jednego ekranu bez ekranów wirtualnych to obie zmienne mają oczekiwaną wartość 0
			PrimaryLeft = Screen.PrimaryScreen.Bounds.Left - SystemInformation.VirtualScreen.Left;
			PrimaryTop = Screen.PrimaryScreen.Bounds.Top - SystemInformation.VirtualScreen.Top;

			//ustawienie wysokości menu
			gpButtons.Height = (int)(Screen.PrimaryScreen.Bounds.Height * Root.ToolbarHeight);

			//przypisanie wartości wysokości, szerokości i pozycji wertykalnej przycisków
			//autorska funkcja niwelująca powtarzalny kod
			InitButtonPropertySetup(btClear);
			InitButtonPropertySetup(btClear,defaultButtonHeight, 0.75,defaultButtonTop);
			InitButtonPropertySetup(btEraser);
			InitButtonPropertySetup(btInkVisible);
			InitButtonPropertySetup(btPan);
			InitButtonPropertySetup(btPointer);
			InitButtonPropertySetup(btSnap);
			InitButtonPropertySetup(btStop);
			InitButtonPropertySetup(btUndo);

			//inicjalizacja tablicy przycisków widocznych w zależności od aktywnych pisaków do wyboru
			btPen = new Button[Root.MaxPenCount];

			int cumulatedleft = (int)(btStop.Width * 1.2);
			for (int b = 0; b < Root.MaxPenCount; b++)
			{
				//pętla inicjalizująca przyciski z pisakami i ustawiająca im odpowiednie parametry i własności
				btPen[b] = new Button();
				btPen[b].Width = (int)(gpButtons.Height * 0.88);
				btPen[b].Height = (int)(gpButtons.Height * 0.88);
				btPen[b].Top = (int)(gpButtons.Height * 0.08);
				btPen[b].FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
				btPen[b].FlatAppearance.BorderSize = 3;
				btPen[b].FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(250, 50, 50);
				btPen[b].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
				btPen[b].ForeColor = System.Drawing.Color.Transparent;
				//btPen[b].Name = "btPen" + b.ToString();
				btPen[b].UseVisualStyleBackColor = false;
				btPen[b].Click += new System.EventHandler(this.btColor_Click);
				btPen[b].BackColor = Root.PenAttr[b].Color;
				btPen[b].FlatAppearance.MouseDownBackColor = Root.PenAttr[b].Color;
				btPen[b].FlatAppearance.MouseOverBackColor = Root.PenAttr[b].Color;

				this.toolTip.SetToolTip(this.btPen[b], Root.Local.ButtonNamePen[b] + " (" + Root.Hotkey_Pens[b].ToString() + ")");

				//metody służące do obsługi zmiany pozycji toolbara za pomocą myszki (w obecnej wersji gInk funkcja eksperymentalna)
				btPen[b].MouseDown += gpButtons_MouseDown;
				btPen[b].MouseMove += gpButtons_MouseMove;
				btPen[b].MouseUp += gpButtons_MouseUp;

				gpButtons.Controls.Add(btPen[b]);

				//pokazywanie tylko takich przycisków z pisakami, które są zaznaczone jako aktywne w opcjach
				InitButtonPos(btPen[b], ref cumulatedleft, Root.PenEnabled[b]);
			}

			//dłuższa przerwa między przyciskami
			cumulatedleft += (int)(btStop.Width * 0.8);

			InitButtonPos(btEraser, ref cumulatedleft, Root.EraserEnabled);
			InitButtonPos(btPan, ref cumulatedleft, Root.PanEnabled);
			InitButtonPos(btPointer, ref cumulatedleft, Root.PointerEnabled);

			//dłuższa przerwa między przyciskami
			cumulatedleft += (int)(btStop.Width * 0.8);

			InitButtonPos(btPenWidth, ref cumulatedleft, Root.PenWidthEnabled);
			InitButtonPos(btInkVisible, ref cumulatedleft, Root.InkVisibleEnabled);
			InitButtonPos(btSnap, ref cumulatedleft, Root.SnapEnabled);
			InitButtonPos(btUndo, ref cumulatedleft, Root.UndoEnabled);
			InitButtonPos(btClear, ref cumulatedleft, Root.ClearEnabled);

			//dłuższa przerwa między przyciskami
			cumulatedleft += (int)(btStop.Width * 0.8);

			btStop.Left = cumulatedleft;
			gpButtons.Width = btStop.Right + (int)(btStop.Width * 0.5);
			

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

			//unikanie drgania obrazu poprzez podwójne buforowanie
			this.DoubleBuffered = true;

			gpButtonsWidth = gpButtons.Width;
			gpButtonsHeight = gpButtons.Height;
			if (true || Root.AllowDraggingToolbar)
			{
				gpButtonsLeft = Root.gpButtonsLeft;
				gpButtonsTop = Root.gpButtonsTop;
				if
				(
					!(IsInsideVisibleScreen(gpButtonsLeft, gpButtonsTop) &&
					IsInsideVisibleScreen(gpButtonsLeft + gpButtonsWidth, gpButtonsTop) &&
					IsInsideVisibleScreen(gpButtonsLeft, gpButtonsTop + gpButtonsHeight) &&
					IsInsideVisibleScreen(gpButtonsLeft + gpButtonsWidth, gpButtonsTop + gpButtonsHeight))
					||
					(gpButtonsLeft == 0 && gpButtonsTop == 0)
				)
				{
					gpButtonsLeft = Screen.PrimaryScreen.WorkingArea.Right - gpButtons.Width + PrimaryLeft;
					gpButtonsTop = Screen.PrimaryScreen.WorkingArea.Bottom - gpButtons.Height - 15 + PrimaryTop;
				}
			}
			else
			{
				gpButtonsLeft = Screen.PrimaryScreen.WorkingArea.Right - gpButtons.Width + PrimaryLeft;
				gpButtonsTop = Screen.PrimaryScreen.WorkingArea.Bottom - gpButtons.Height - 15 + PrimaryTop;
			}

			gpButtons.Left = gpButtonsLeft + gpButtons.Width;
			gpButtons.Top = gpButtonsTop;
			gpPenWidth.Left = gpButtonsLeft + btPenWidth.Left - gpPenWidth.Width / 2 + btPenWidth.Width / 2;
			gpPenWidth.Top = gpButtonsTop - gpPenWidth.Height - 10;

			pboxPenWidthIndicator.Top = 0;
			pboxPenWidthIndicator.Left = (int)Math.Sqrt(Root.GlobalPenWidth * 30);
			gpPenWidth.Controls.Add(pboxPenWidthIndicator);

			//informacje z https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-3.5/ms552322(v=vs.90)

			IC = new InkOverlay(this.Handle);
			IC.CollectionMode = CollectionMode.InkOnly;

			//AutoRedraw kontroluje, czy po zminimalizowaniu i późniejszym zmaksymalizowaniu okna, rysunki zostają narysowane ponownie
			IC.AutoRedraw = false;

			//musi być oznaczone jako false, aby rysunki były renderowane w czasie rzeczywistym przy AutoRedraw=false
			IC.DynamicRendering = false;

			//ustawienie trybu usuwania rysunków jako usuwanie całego stroke'a, a nie pojedynczego punktu (StrokeErase zamiast PointErase)
			IC.EraserMode = InkOverlayEraserMode.StrokeErase;

			IC.CursorInRange += IC_CursorInRange;
			IC.MouseDown += IC_MouseDown;
			IC.MouseMove += IC_MouseMove;
			IC.MouseUp += IC_MouseUp;
			IC.CursorDown += IC_CursorDown;
			IC.Stroke += IC_Stroke;
			IC.DefaultDrawingAttributes.Width = 80;
			IC.DefaultDrawingAttributes.Transparency = 30;
			IC.DefaultDrawingAttributes.AntiAliased = true;

			//cursorred = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorred.Handle);
			//IC.Cursor = cursorred;
			IC.Enabled = true;

			image_exit = new Bitmap(btStop.Width, btStop.Height);
			Graphics g = Graphics.FromImage(image_exit);


			//załadowanie obrazków dla przycisków, autorskie metody dla zniwelowania powtarzalnego kodu
			InitButtonBitmap(btStop,ref image_exit, gInk.Properties.Resources.exit,  g, true);
			InitButtonBitmap(btClear, ref image_clear,gInk.Properties.Resources.garbage ,g, true);
			InitButtonBitmap(btUndo, ref image_undo, gInk.Properties.Resources.undo, g, true);

			InitButtonBitmap(btEraser, ref image_eraser_act,gInk.Properties.Resources.eraser_act, g, false);
			InitButtonBitmap(btEraser, ref image_eraser,gInk.Properties.Resources.eraser, g, true);

			InitButtonBitmap(btPan, ref image_pan_act,gInk.Properties.Resources.pan_act ,g, false);
			InitButtonBitmap(btPan, ref image_pan, gInk.Properties.Resources.pan,g, true);

			InitButtonBitmap(btInkVisible, ref image_visible_not, gInk.Properties.Resources.visible_not,g, false);
			InitButtonBitmap(btInkVisible, ref image_visible,gInk.Properties.Resources.visible, g, true);

			InitButtonBitmap(btSnap, ref image_snap,gInk.Properties.Resources.snap, g, true);
			InitButtonBitmap(btPenWidth, ref image_penwidth, gInk.Properties.Resources.penwidth, g, true);

			InitButtonBitmap(btDock, ref image_dock, gInk.Properties.Resources.dock, g, false);
			InitButtonBitmap(btDock, ref image_dockback, gInk.Properties.Resources.dockback, g, false);
			
			//sprawdzenie, czy toolbar jest schowany i odpowiednie dostosowanie obrazku na przycisku "Dock"
			if (Root.Docked)
				btDock.Image = image_dockback;
			else
				btDock.Image = image_dock;

			InitButtonBitmap(btPen[2], ref image_pencil, gInk.Properties.Resources.pencil, g, false);
			InitButtonBitmap(btPen[2], ref image_highlighter, gInk.Properties.Resources.highlighter, g, false);
			InitButtonBitmap(btPen[2], ref image_pencil_act, gInk.Properties.Resources.pencil_act, g, false);
			InitButtonBitmap(btPen[2], ref image_highlighter_act, gInk.Properties.Resources.highlighter_act, g, false);
			InitButtonBitmap(btPointer, ref image_pointer, gInk.Properties.Resources.pointer, g, false);
			InitButtonBitmap(btPointer, ref image_pointer_act, gInk.Properties.Resources.pointer_act, g, false);
			
			//inicjalizacja bitmap dla przycisków pisaków i załadowanie obrazków dla aktywnych pisaków
			image_pen = new Bitmap[Root.MaxPenCount];
			image_pen_act = new Bitmap[Root.MaxPenCount];
			for (int b = 0; b < Root.MaxPenCount; b++)
			{
				if (Root.PenAttr[b].Transparency >= 100)
				{
					image_pen[b] = new Bitmap(btPen[b].Width, btPen[b].Height);
					image_pen[b] = image_highlighter;
					image_pen_act[b] = new Bitmap(btPen[b].Width, btPen[b].Height);
					image_pen_act[b] = image_highlighter_act;
				}
				else
				{
					image_pen[b] = new Bitmap(btPen[b].Width, btPen[b].Height);
					image_pen[b] = image_pencil;
					image_pen_act[b] = new Bitmap(btPen[b].Width, btPen[b].Height);
					image_pen_act[b] = image_pencil_act;
				}
			}

			LastTickTime = DateTime.Parse("1987-01-01");
			tiSlide.Enabled = true;

			ToTransparent();
			ToTopMost();

			//ustawienie tooltipów dla przycisków
			this.toolTip.SetToolTip(this.btDock, Root.Local.ButtonNameDock);
			this.toolTip.SetToolTip(this.btPenWidth, Root.Local.ButtonNamePenwidth);
			this.toolTip.SetToolTip(this.btEraser, Root.Local.ButtonNameErasor + " (" + Root.Hotkey_Eraser.ToString() + ")");
			this.toolTip.SetToolTip(this.btPan, Root.Local.ButtonNamePan + " (" + Root.Hotkey_Pan.ToString() + ")");
			this.toolTip.SetToolTip(this.btPointer, Root.Local.ButtonNameMousePointer + " (" + Root.Hotkey_Pointer.ToString() + ")");
			this.toolTip.SetToolTip(this.btInkVisible, Root.Local.ButtonNameInkVisible + " (" + Root.Hotkey_InkVisible.ToString() + ")");
			this.toolTip.SetToolTip(this.btSnap, Root.Local.ButtonNameSnapshot + " (" + Root.Hotkey_Snap.ToString() + ")");
			this.toolTip.SetToolTip(this.btUndo, Root.Local.ButtonNameUndo + " (" + Root.Hotkey_Undo.ToString() + ")");
			this.toolTip.SetToolTip(this.btClear, Root.Local.ButtonNameClear + " (" + Root.Hotkey_Clear.ToString() + ")");
			this.toolTip.SetToolTip(this.btStop, Root.Local.ButtonNameExit + " (ESC)");
		}

		private void InitButtonPropertySetup(Button button, double height=defaultButtonHeight, double width=defaultButtonWidth, double top=defaultButtonTop)
        {
			button.Height = (int)(gpButtons.Height * height);
			button.Width = (int)(button.Height * width);
			button.Top = (int)(gpButtons.Height * top);
		}

		private void InitButtonPos(Button button, ref int cumulatedLeft, bool isEnabled, double gap = defaultGapBetweenButtons)
        {
			if (isEnabled)
			{
				button.Visible = true;
				button.Left = cumulatedLeft;
				cumulatedLeft += (int)(button.Width * (1.0 + defaultGapBetweenButtons));
			}
			else
			{
				button.Visible = false;
			}
		}

		private void InitButtonBitmap(Button button, ref Bitmap bitmap, Bitmap resource,Graphics g, bool overwriteImage)
        {
			bitmap = new Bitmap(button.Width, button.Height);
			g = Graphics.FromImage(bitmap);
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			g.DrawImage(resource, 0, 0, button.Width, button.Height);
			if(overwriteImage)
				button.Image = bitmap;
		}

		private void IC_Stroke(object sender, InkCollectorStrokeEventArgs e)
		{
			//przy zakończeniu jednego rysunku uruchamia się procedura obsługi cofania (undo)
			SaveUndoStrokes();
		}

		private void SaveUndoStrokes()
		{
			//gdy dorysowany jest nowy rysunek, Redo Depth zostaje ustawiony na 0.
			//Nie ma możliwości operacji Redo po dodaniu nowego rysunku
			Root.RedoDepth = 0;
			if (Root.UndoDepth < Root.UndoStrokes.GetLength(0) - 1)
				Root.UndoDepth++;

			Root.UndoP++;
			if (Root.UndoP >= Root.UndoStrokes.GetLength(0))
				Root.UndoP = 0;

			if (Root.UndoStrokes[Root.UndoP] == null)
				Root.UndoStrokes[Root.UndoP] = new Ink();
			Root.UndoStrokes[Root.UndoP].DeleteStrokes();
			if (IC.Ink.Strokes.Count > 0)
				Root.UndoStrokes[Root.UndoP].AddStrokesAtRectangle(IC.Ink.Strokes, IC.Ink.Strokes.GetBoundingBox());
		}

		private void IC_CursorDown(object sender, InkCollectorCursorDownEventArgs e)
		{
			if (!Root.InkVisible && Root.Snapping <= 0)
			{
				Root.SetInkVisible(true);
			}

			Root.FormDisplay.ClearCanvus(Root.FormDisplay.gOneStrokeCanvus);
			Root.FormDisplay.DrawStrokes(Root.FormDisplay.gOneStrokeCanvus);
			Root.FormDisplay.DrawButtons(Root.FormDisplay.gOneStrokeCanvus, false);
		}

		private void IC_MouseDown(object sender, CancelMouseEventArgs e)
		{
			if (Root.gpPenWidthVisible)
			{
				Root.gpPenWidthVisible = false;
				Root.UponSubPanelUpdate = true;
			}

			Root.FingerInAction = true;
			if (Root.Snapping == 1)
			{
				Root.SnappingX = e.X;
				Root.SnappingY = e.Y;
				Root.SnappingRect = new Rectangle(e.X, e.Y, 0, 0);
				Root.Snapping = 2;
			}

			if (!Root.InkVisible && Root.Snapping <= 0)
			{
				Root.SetInkVisible(true);
			}

			LasteXY.X = e.X;
			LasteXY.Y = e.Y;
			IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref LasteXY);
		}

		public Point LasteXY;
		private void IC_MouseMove(object sender, CancelMouseEventArgs e)
		{
			if (LasteXY.X == 0 && LasteXY.Y == 0)
			{
				LasteXY.X = e.X;
				LasteXY.Y = e.Y;
				IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref LasteXY);
			}
			Point currentxy = new Point(e.X, e.Y);
			IC.Renderer.PixelToInkSpace(Root.FormDisplay.gOneStrokeCanvus, ref currentxy);

			if (Root.Snapping == 2)
			{
				int left = Math.Min(Root.SnappingX, e.X);
				int top = Math.Min(Root.SnappingY, e.Y);
				int width = Math.Abs(Root.SnappingX - e.X);
				int height = Math.Abs(Root.SnappingY - e.Y);
				Root.SnappingRect = new Rectangle(left, top, width, height);

				if (LasteXY != currentxy)
					Root.MouseMovedUnderSnapshotDragging = true;
			}
			else if (Root.PanMode && Root.FingerInAction)
			{
				Root.Pan(currentxy.X - LasteXY.X, currentxy.Y - LasteXY.Y);			
			}

			LasteXY = currentxy;
		}

		private void IC_MouseUp(object sender, CancelMouseEventArgs e)
		{
			Root.FingerInAction = false;
			if (Root.Snapping == 2)
			{
				int left = Math.Min(Root.SnappingX, e.X);
				int top = Math.Min(Root.SnappingY, e.Y);
				int width = Math.Abs(Root.SnappingX - e.X);
				int height = Math.Abs(Root.SnappingY - e.Y);
				if (width < 5 || height < 5)
				{
					left = 0;
					top = 0;
					width = this.Width;
					height = this.Height;
				}
				Root.SnappingRect = new Rectangle(left + this.Left, top + this.Top, width, height);
				Root.UponTakingSnap = true;
				ExitSnapping();
			}
			else if (Root.PanMode)
			{
				SaveUndoStrokes();
			}
			else
			{
				Root.UponAllDrawingUpdate = true;
			}
		}

		private void IC_CursorInRange(object sender, InkCollectorCursorInRangeEventArgs e)
		{
			if (e.Cursor.Inverted && Root.CurrentPen != -1)
			{
				EnterEraserMode(true);
				/*
				// temperary eraser icon light
				if (btEraser.Image == image_eraser)
				{
					btEraser.Image = image_eraser_act;
					Root.FormDisplay.DrawButtons(true);
					Root.FormDisplay.UpdateFormDisplay();
				}
				*/
			}
			else if (!e.Cursor.Inverted && Root.CurrentPen != -1)
			{
				EnterEraserMode(false);
				/*
				if (btEraser.Image == image_eraser_act)
				{
					btEraser.Image = image_eraser;
					Root.FormDisplay.DrawButtons(true);
					Root.FormDisplay.UpdateFormDisplay();
				}
				*/
			}
		}

		public void ToTransparent()
		{
			//ustawienie całego Forma jako przezroczystego

			UInt32 dwExStyle = GetWindowLong(this.Handle, -20);
			SetWindowLong(this.Handle, -20, dwExStyle | 0x00080000);
			SetLayeredWindowAttributes(this.Handle, 0x00FFFFFF, 1, 0x2);
		}

		public void ToTopMost()
		{
			//ustawienie FormCollection jako Forma znajdującego się najwyżej (przykrywa wszystkie inne aktywnego okna w systemie)

			SetWindowPos(this.Handle, (IntPtr)(-1), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0020);
		}

		public void ToThrough()
		{
			UInt32 dwExStyle = GetWindowLong(this.Handle, -20);
			//SetWindowLong(this.Handle, -20, dwExStyle | 0x00080000);
			//SetWindowPos(this.Handle, (IntPtr)0, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0004 | 0x0010 | 0x0020);
			//SetLayeredWindowAttributes(this.Handle, 0x00FFFFFF, 1, 0x2);
			SetWindowLong(this.Handle, -20, dwExStyle | 0x00080000 | 0x00000020);
			//SetWindowPos(this.Handle, (IntPtr)(1), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0010 | 0x0020);
		}

		public void ToUnThrough()
		{
			UInt32 dwExStyle = GetWindowLong(this.Handle, -20);
			//SetWindowLong(this.Handle, -20, (uint)(dwExStyle & ~0x00080000 & ~0x0020));
			SetWindowLong(this.Handle, -20, (uint)(dwExStyle & ~0x0020));
			//SetWindowPos(this.Handle, (IntPtr)(-2), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0010 | 0x0020);

			//dwExStyle = GetWindowLong(this.Handle, -20);
			//SetWindowLong(this.Handle, -20, dwExStyle | 0x00080000);
			//SetLayeredWindowAttributes(this.Handle, 0x00FFFFFF, 1, 0x2);
			//SetWindowPos(this.Handle, (IntPtr)(-1), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0020);
		}

		public void EnterEraserMode(bool enter)
		{
			int exceptiontick = 0;
			bool exc;
			do
			{
				exceptiontick++;
				exc = false;
				try
				{
					if (enter)
					{
						IC.EditingMode = InkOverlayEditingMode.Delete;
						Root.EraserMode = true;
					}
					else
					{
						IC.EditingMode = InkOverlayEditingMode.Ink;
						Root.EraserMode = false;
					}
				}
				catch
				{
					Thread.Sleep(50);
					exc = true;
				}
			}
			while (exc && exceptiontick < 3);
		}

		public void SelectPen(int pen)
		{
			// -3=pan, -2=pointer, -1=erasor, 0+=pens
			if (pen == -3)
			{
				//uruchomienie Pan Mode, czyli przenoszenie całości rysunków 
				for (int b = 0; b < Root.MaxPenCount; b++)
					btPen[b].Image = image_pen[b];
				btEraser.Image = image_eraser;
				btPointer.Image = image_pointer;
				btPan.Image = image_pan_act;
				EnterEraserMode(false);
				Root.UnPointer();
				Root.PanMode = true;

				try
				{
					IC.SetWindowInputRectangle(new Rectangle(0, 0, 1, 1));
				}
				catch
				{
					Thread.Sleep(1);
					IC.SetWindowInputRectangle(new Rectangle(0, 0, 1, 1));
				}
			}
			else if (pen == -2)
			{
				//uruchomienie Pointer Mode - działanie kursorem na resztę systemu

				for (int b = 0; b < Root.MaxPenCount; b++)
					btPen[b].Image = image_pen[b];
				btEraser.Image = image_eraser;
				btPointer.Image = image_pointer_act;
				btPan.Image = image_pan;
				EnterEraserMode(false);
				Root.Pointer();
				Root.PanMode = false;
			}
			else if (pen == -1)
			{
				//uruchomienie trybu gumki, usuwanie konkretnych rysunków
				if (this.Cursor != System.Windows.Forms.Cursors.Default)
					this.Cursor = System.Windows.Forms.Cursors.Default;

				for (int b = 0; b < Root.MaxPenCount; b++)
					btPen[b].Image = image_pen[b];
				btEraser.Image = image_eraser_act;
				btPointer.Image = image_pointer;
				btPan.Image = image_pan;
				EnterEraserMode(true);
				Root.UnPointer();
				Root.PanMode = false;

				if (Root.CanvasCursor == 0)
				{
					//ustawienie specjalnego kursora, switch dobierający odpowiedni rozmiar w zależności od trackbara "Cursor size"
					var size = Root.CursorSize;
					switch(size)
                    {
						case 0:
							cursorred = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorred0.Handle);
							break;
						case 1:
							cursorred = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorred1.Handle);
							break;
						case 2:
							cursorred = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorred2.Handle);
							break;
						case 3:
							cursorred = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorred3.Handle);
							break;
						case 4:
							cursorred = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorred4.Handle);
							break;
					}
					IC.Cursor = cursorred;
				}
				else if (Root.CanvasCursor == 1)
					//metoda zastępująca kursor kropką o kolorze i średnicy odpowiadającym kolorze aktywnego pisaka i jego grubości
					SetPenTipCursor();

				try
				{
					IC.SetWindowInputRectangle(new Rectangle(0, 0, this.Width, this.Height));
				}
				catch
				{
					Thread.Sleep(1);
					IC.SetWindowInputRectangle(new Rectangle(0, 0, this.Width, this.Height));
				}
			}
			else if (pen >= 0)
			{
				//ustawienie pisaka
				if (this.Cursor != System.Windows.Forms.Cursors.Default)
					this.Cursor = System.Windows.Forms.Cursors.Default;

				IC.DefaultDrawingAttributes = Root.PenAttr[pen].Clone();

				//wymuszanie konkretnej grubości pisaka w zależności od grubości ustawionej w panelu
				if (Root.PenWidthEnabled)
				{
					IC.DefaultDrawingAttributes.Width = Root.GlobalPenWidth;
				}

				//ustawienie obrazków na przyciskach od nieaktywnych trybów na ich "czarną" wersję
				//tylko aktywny pisak (którego indeks jest przechowywany w zmiennej pen) dostaje obrazek oznaczający aktywny (żółty)
				for (int b = 0; b < Root.MaxPenCount; b++)
					btPen[b].Image = image_pen[b];
				btPen[pen].Image = image_pen_act[pen];
				btEraser.Image = image_eraser;
				btPointer.Image = image_pointer;
				btPan.Image = image_pan;
				EnterEraserMode(false);
				Root.UnPointer();
				Root.PanMode = false;

				if (Root.CanvasCursor == 0)
				{
					//ustawienie specjalnego kursora, switch dobierający odpowiedni rozmiar w zależności od trackbara "Cursor size"
					var size = Root.CursorSize;
					switch (size)
					{
						case 0:
							cursorred = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorred0.Handle);
							break;
						case 1:
							cursorred = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorred1.Handle);
							break;
						case 2:
							cursorred = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorred2.Handle);
							break;
						case 3:
							cursorred = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorred3.Handle);
							break;
						case 4:
							cursorred = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorred4.Handle);
							break;
					}
					IC.Cursor = cursorred;
				}
				else if (Root.CanvasCursor == 1)
					SetPenTipCursor();

				try
				{
					IC.SetWindowInputRectangle(new Rectangle(0, 0, this.Width, this.Height));
				}
				catch
				{
					Thread.Sleep(1);
					IC.SetWindowInputRectangle(new Rectangle(0, 0, this.Width, this.Height));
				}
			}
			Root.CurrentPen = pen;

			//zamknięcie panelu z doborem grubości pisaka po kliknięciu innego przycisku
			if (Root.gpPenWidthVisible)
			{
				Root.gpPenWidthVisible = false;
				Root.UponSubPanelUpdate = true;
			}
			else
				Root.UponButtonsUpdate |= 0x2;

			if (pen != -2)
				Root.LastPen = pen;
		}

		public void RetreatAndExit()
		{
			//metoda wywoływana przy końcu działania i zamykaniu FormCollection
			
			ToThrough();

			Root.ClearInk();
			SaveUndoStrokes();
			Root.SaveOptions("config.ini");
			Root.gpPenWidthVisible = false;

			LastTickTime = DateTime.Now;
			ButtonsEntering = -9;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
		}

		public void btDock_Click(object sender, EventArgs e)
		{
			//metoda wywołująca się po kliknięciu na przycisk od dockowania, najbardziej wychylonego w lewo

			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			LastTickTime = DateTime.Now;
			if (!Root.Docked)
			{
				Root.Dock();
			}
			else
			{
				Root.UnDock();
			}
		}

		public void btPointer_Click(object sender, EventArgs e)
		{
			//metoda wywołująca się przy kliknięciu przycisku z kursorem (Pointer Mode)

			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			//inne tryby działania z aplikacją, jak Pointer Mode lub tryb wymazywania są traktowane jako pisaki o indeksach mniejszych niż 0
			//tryby te są uruchamiane wewnątrz metody SelectPen
			SelectPen(-2);
		}


		private void btPenWidth_Click(object sender, EventArgs e)
		{
			//metoda wywołująca się przy kliknięciu przycisku od ustawienia grubości pisaka (domyślnie jest on ukryty i do ujawnienia w opcjach)
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			if (Root.PointerMode)
				return;

			Root.gpPenWidthVisible = !Root.gpPenWidthVisible;
			if (Root.gpPenWidthVisible)
				Root.UponButtonsUpdate |= 0x2;
			else
				Root.UponSubPanelUpdate = true;
		}

		public void btSnap_Click(object sender, EventArgs e)
		{
			//metoda przy kliknięciu przycisku z aparatem (tryb tworzenie zrzutu ekranu)
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			if (Root.Snapping > 0)
				return;

			//zmiana kursora na krzyżyk występujący podczas trybu tworzenia screenshota
			var size = Root.CursorSize;
			switch (size)
			{
				case 0:
					cursorsnap = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorsnap0.Handle);
					break;
				case 1:
					cursorsnap = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorsnap1.Handle);
					break;
				case 2:
					cursorsnap = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorsnap2.Handle);
					break;
				case 3:
					cursorsnap = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorsnap3.Handle);
					break;
				case 4:
					cursorsnap = new System.Windows.Forms.Cursor(gInk.Properties.Resources.cursorsnap4.Handle);
					break;
			}
			this.Cursor = cursorsnap;

			Root.gpPenWidthVisible = false;

			try
			{
				IC.SetWindowInputRectangle(new Rectangle(0, 0, 1, 1));
			}
			catch
			{
				Thread.Sleep(1);
				IC.SetWindowInputRectangle(new Rectangle(0, 0, 1, 1));
			}
			Root.SnappingX = -1;
			Root.SnappingY = -1;
			Root.SnappingRect = new Rectangle(0, 0, 0, 0);
			Root.Snapping = 1;
			ButtonsEntering = -2;
			Root.UnPointer();
		}

		public void ExitSnapping()
		{
			//wyjście z trybu tworzenia screenshota
			try
			{
				IC.SetWindowInputRectangle(new Rectangle(0, 0, this.Width, this.Height));
			}
			catch
			{
				Thread.Sleep(1);
				IC.SetWindowInputRectangle(new Rectangle(0, 0, this.Width, this.Height));
			}
			Root.SnappingX = -1;
			Root.SnappingY = -1;
			Root.Snapping = -60;
			ButtonsEntering = 1;
			Root.SelectPen(Root.CurrentPen);

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}

		public void btStop_Click(object sender, EventArgs e)
		{
			//metoda wywoływana po kliknięciu przycisku "X", zamykająca aplikacje
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			RetreatAndExit();
		}

		DateTime LastTickTime;
		bool[] LastPenStatus = new bool[10];
		bool LastEraserStatus = false;
		bool LastVisibleStatus = false;
		bool LastPointerStatus = false;
		bool LastPanStatus = false;
		bool LastUndoStatus = false;
		bool LastRedoStatus = false;
		bool LastSnapStatus = false;
		bool LastClearStatus = false;

		private void gpPenWidth_MouseDown(object sender, MouseEventArgs e)
		{
			gpPenWidth_MouseOn = true;
		}

		private void gpPenWidth_MouseMove(object sender, MouseEventArgs e)
		{
			if (gpPenWidth_MouseOn)
			{
				if (e.X < 10 || gpPenWidth.Width - e.X < 10)
					return;

				Root.GlobalPenWidth = e.X * e.X / 30;
				pboxPenWidthIndicator.Left = e.X - pboxPenWidthIndicator.Width / 2;
				IC.DefaultDrawingAttributes.Width = Root.GlobalPenWidth;
				Root.UponButtonsUpdate |= 0x2;
			}
		}

		private void gpPenWidth_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.X >= 10 && gpPenWidth.Width - e.X >= 10)
			{
				Root.GlobalPenWidth = e.X * e.X / 30;
				pboxPenWidthIndicator.Left = e.X - pboxPenWidthIndicator.Width / 2;
				IC.DefaultDrawingAttributes.Width = Root.GlobalPenWidth;
			}

			if (Root.CanvasCursor == 1)
				SetPenTipCursor();

			Root.gpPenWidthVisible = false;
			Root.UponSubPanelUpdate = true;
			gpPenWidth_MouseOn = false;
		}

		private void pboxPenWidthIndicator_MouseDown(object sender, MouseEventArgs e)
		{
			gpPenWidth_MouseOn = true;
		}

		private void pboxPenWidthIndicator_MouseMove(object sender, MouseEventArgs e)
		{
			if (gpPenWidth_MouseOn)
			{
				int x = e.X + pboxPenWidthIndicator.Left;
				if (x < 10 || gpPenWidth.Width - x < 10)
					return;

				Root.GlobalPenWidth = x * x / 30;
				pboxPenWidthIndicator.Left = x - pboxPenWidthIndicator.Width / 2;
				IC.DefaultDrawingAttributes.Width = Root.GlobalPenWidth;
				Root.UponButtonsUpdate |= 0x2;
			}
		}

		private void pboxPenWidthIndicator_MouseUp(object sender, MouseEventArgs e)
		{
			if (Root.CanvasCursor == 1)
				SetPenTipCursor();

			Root.gpPenWidthVisible = false;
			Root.UponSubPanelUpdate = true;
			gpPenWidth_MouseOn = false;
		}

		private void SetPenTipCursor()
		{
			//ustawienie kursora jako kropki w kolorze i grubości aktywnego pisaka

			Bitmap bitmaptip = (Bitmap)(gInk.Properties.Resources._null).Clone();
			Graphics g = Graphics.FromImage(bitmaptip);
			DrawingAttributes dda = IC.DefaultDrawingAttributes;
			Brush cbrush;
			Point widt;
			if (!Root.EraserMode)
			{
				cbrush = new SolidBrush(IC.DefaultDrawingAttributes.Color);
				//Brush cbrush = new SolidBrush(Color.FromArgb(255 - dda.Transparency, dda.Color.R, dda.Color.G, dda.Color.B));
				widt = new Point((int)IC.DefaultDrawingAttributes.Width, 0);
			}
			else
			{
				cbrush = new SolidBrush(Color.Black);
				widt = new Point(60, 0);
			}
			IC.Renderer.InkSpaceToPixel(IC.Handle, ref widt);

			IntPtr screenDc = GetDC(IntPtr.Zero);
			const int VERTRES = 10;
			const int DESKTOPVERTRES = 117;
			int LogicalScreenHeight = GetDeviceCaps(screenDc, VERTRES);
			int PhysicalScreenHeight = GetDeviceCaps(screenDc, DESKTOPVERTRES);
			float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
			ReleaseDC(IntPtr.Zero, screenDc);

			int dia = Math.Max((int)(widt.X * ScreenScalingFactor), 2);
			g.FillEllipse(cbrush, 64 - dia / 2, 64 - dia / 2, dia, dia);
			if (dia <= 5)
			{
				Pen cpen = new Pen(Color.FromArgb(50, 128, 128, 128), 2);
				dia += 6;
				g.DrawEllipse(cpen, 64 - dia / 2, 64 - dia / 2, dia, dia);
			}
			IC.Cursor = new System.Windows.Forms.Cursor(bitmaptip.GetHicon());
			
		}

		short LastESCStatus = 0;
		private void tiSlide_Tick(object sender, EventArgs e)
		{
			// ignore the first tick
			if (LastTickTime.Year == 1987)
			{
				LastTickTime = DateTime.Now;
				return;
			}

			int aimedleft = gpButtonsLeft;
			if (ButtonsEntering == -9)
			{
				aimedleft = gpButtonsLeft + gpButtonsWidth;
			}
			else if (ButtonsEntering < 0)
			{
				if (Root.Snapping > 0)
					aimedleft = gpButtonsLeft + gpButtonsWidth + 0;
				else if (Root.Docked)
					aimedleft = gpButtonsLeft + gpButtonsWidth - btDock.Right;
			}
			else if (ButtonsEntering > 0)
			{
				if (Root.Docked)
					aimedleft = gpButtonsLeft + gpButtonsWidth - btDock.Right;
				else
					aimedleft = gpButtonsLeft;
			}
			else if (ButtonsEntering == 0)
			{
				aimedleft = gpButtons.Left; // stay at current location
			}

			if (gpButtons.Left > aimedleft)
			{
				float dleft = gpButtons.Left - aimedleft;
				dleft /= 70;
				if (dleft > 8) dleft = 8;
				dleft *= (float)(DateTime.Now - LastTickTime).TotalMilliseconds;
				if (dleft > 120) dleft = 230;
				if (dleft < 1) dleft = 1;
				gpButtons.Left -= (int)dleft;
				LastTickTime = DateTime.Now;
				if (gpButtons.Left < aimedleft)
				{
					gpButtons.Left = aimedleft;
				}
				gpButtons.Width = Math.Max(gpButtonsWidth - (gpButtons.Left - gpButtonsLeft), btDock.Width);
				Root.UponButtonsUpdate |= 0x1;
			}
			else if (gpButtons.Left < aimedleft)
			{
				float dleft = aimedleft - gpButtons.Left;
				dleft /= 70;
				if (dleft > 8) dleft = 8;
				// fast exiting when not docked
				if (ButtonsEntering == -9 && !Root.Docked)
					dleft = 8;
				dleft *= (float)(DateTime.Now - LastTickTime).TotalMilliseconds;
				if (dleft > 120) dleft = 120;
				if (dleft < 1) dleft = 1;
				// fast exiting when docked
				if (ButtonsEntering == -9 && dleft == 1)
					dleft = 2;
				gpButtons.Left += (int)dleft;
				LastTickTime = DateTime.Now;
				if (gpButtons.Left > aimedleft)
				{
					gpButtons.Left = aimedleft;
				}
				gpButtons.Width = Math.Max(gpButtonsWidth - (gpButtons.Left - gpButtonsLeft), btDock.Width);
				Root.UponButtonsUpdate |= 0x1;
				Root.UponButtonsUpdate |= 0x4;
			}

			if (ButtonsEntering == -9 && gpButtons.Left == aimedleft)
			{
				tiSlide.Enabled = false;
				Root.StopInk();
				return;
			}
			else if (ButtonsEntering < 0)
			{
				Root.UponAllDrawingUpdate = true;
				Root.UponButtonsUpdate = 0;
			}
			if (gpButtons.Left == aimedleft)
			{
				ButtonsEntering = 0;
			}



			if (!Root.PointerMode && !this.TopMost)
				ToTopMost();

			// gpPenWidth status

			if (Root.gpPenWidthVisible != gpPenWidth.Visible)
				gpPenWidth.Visible = Root.gpPenWidthVisible;

			// hotkeys

			const int VK_LCONTROL = 0xA2;
			const int VK_RCONTROL = 0xA3;
			const int VK_LSHIFT = 0xA0;
			const int VK_RSHIFT = 0xA1;
			const int VK_LMENU = 0xA4;
			const int VK_RMENU = 0xA5;
			const int VK_LWIN = 0x5B;
			const int VK_RWIN = 0x5C;
			bool pressed;

			if (!Root.PointerMode)
			{
				// ESC key : Exit
				short retVal;
				retVal = GetKeyState(27);
				if ((retVal & 0x8000) == 0x8000 && (LastESCStatus & 0x8000) == 0x0000)
				{
					if (Root.Snapping > 0)
					{
						ExitSnapping();
					}
					else if (Root.gpPenWidthVisible)
					{
						Root.gpPenWidthVisible = false;
						Root.UponSubPanelUpdate = true;
					}
					else if (Root.Snapping == 0)
						RetreatAndExit();
				}
				LastESCStatus = retVal;
			}


			if (!Root.FingerInAction && (!Root.PointerMode || Root.AllowHotkeyInPointerMode) && Root.Snapping <= 0)
			{
				bool control = ((short)(GetKeyState(VK_LCONTROL) | GetKeyState(VK_RCONTROL)) & 0x8000) == 0x8000;
				bool alt = ((short)(GetKeyState(VK_LMENU) | GetKeyState(VK_RMENU)) & 0x8000) == 0x8000;
				bool shift = ((short)(GetKeyState(VK_LSHIFT) | GetKeyState(VK_RSHIFT)) & 0x8000) == 0x8000;
				bool win = ((short)(GetKeyState(VK_LWIN) | GetKeyState(VK_RWIN)) & 0x8000) == 0x8000;

				for (int p = 0; p < Root.MaxPenCount; p++)
				{
					pressed = (GetKeyState(Root.Hotkey_Pens[p].Key) & 0x8000) == 0x8000;
					if(pressed && !LastPenStatus[p] && Root.Hotkey_Pens[p].ModifierMatch(control, alt, shift, win))
					{
						SelectPen(p);
					}
					LastPenStatus[p] = pressed;
				}

				pressed = (GetKeyState(Root.Hotkey_Eraser.Key) & 0x8000) == 0x8000;
				if (pressed && !LastEraserStatus && Root.Hotkey_Eraser.ModifierMatch(control, alt, shift, win))
				{
					SelectPen(-1);
				}
				LastEraserStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_InkVisible.Key) & 0x8000) == 0x8000;
				if (pressed && !LastVisibleStatus && Root.Hotkey_InkVisible.ModifierMatch(control, alt, shift, win))
				{
					btInkVisible_Click(null, null);
				}
				LastVisibleStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_Undo.Key) & 0x8000) == 0x8000;
				if (pressed && !LastUndoStatus && Root.Hotkey_Undo.ModifierMatch(control, alt, shift, win))
				{
					if (!Root.InkVisible)
						Root.SetInkVisible(true);

					Root.UndoInk();
				}
				LastUndoStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_Redo.Key) & 0x8000) == 0x8000;
				if (pressed && !LastRedoStatus && Root.Hotkey_Redo.ModifierMatch(control, alt, shift, win))
				{
					Root.RedoInk();
				}
				LastRedoStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_Pointer.Key) & 0x8000) == 0x8000;
				if (pressed && !LastPointerStatus && Root.Hotkey_Pointer.ModifierMatch(control, alt, shift, win))
				{
					SelectPen(-2);
				}
				LastPointerStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_Pan.Key) & 0x8000) == 0x8000;
				if (pressed && !LastPanStatus && Root.Hotkey_Pan.ModifierMatch(control, alt, shift, win))
				{
					SelectPen(-3);
				}
				LastPanStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_Clear.Key) & 0x8000) == 0x8000;
				if (pressed && !LastClearStatus && Root.Hotkey_Clear.ModifierMatch(control, alt, shift, win))
				{
					btClear_Click(null, null);
				}
				LastClearStatus = pressed;

				pressed = (GetKeyState(Root.Hotkey_Snap.Key) & 0x8000) == 0x8000;
				if (pressed && !LastSnapStatus && Root.Hotkey_Snap.ModifierMatch(control, alt, shift, win))
				{
					btSnap_Click(null, null);
				}
				LastSnapStatus = pressed;
			}

			if (Root.Snapping < 0)
				Root.Snapping++;
		}

		private bool IsInsideVisibleScreen(int x, int y)
		{
			x -= PrimaryLeft;
			y -= PrimaryTop;
			//foreach (Screen s in Screen.AllScreens)
			//	Console.WriteLine(s.Bounds);
			//Console.WriteLine(x.ToString() + ", " + y.ToString());

			foreach (Screen s in Screen.AllScreens)
				if (s.Bounds.Contains(x, y))
					return true;
			return false;
		}

		int IsMovingToolbar = 0;
		Point HitMovingToolbareXY = new Point();
		bool ToolbarMoved = false;
		private void gpButtons_MouseDown(object sender, MouseEventArgs e)
		{
			if (!Root.AllowDraggingToolbar)
				return;
			if (ButtonsEntering != 0)
				return;

			ToolbarMoved = false;
			IsMovingToolbar = 1;
			HitMovingToolbareXY.X = e.X;
			HitMovingToolbareXY.Y = e.Y;
		}

		private void gpButtons_MouseMove(object sender, MouseEventArgs e)
		{
			if (IsMovingToolbar == 1)
			{
				if (Math.Abs(e.X - HitMovingToolbareXY.X) > 20 || Math.Abs(e.Y - HitMovingToolbareXY.Y) > 20)
					IsMovingToolbar = 2;
			}
			if (IsMovingToolbar == 2)
			{
				if (e.X != HitMovingToolbareXY.X || e.Y != HitMovingToolbareXY.Y)
				{
					/*
					gpButtonsLeft += e.X - HitMovingToolbareXY.X;
					gpButtonsTop += e.Y - HitMovingToolbareXY.Y;
					
					if (gpButtonsLeft + gpButtonsWidth > SystemInformation.VirtualScreen.Right)
						gpButtonsLeft = SystemInformation.VirtualScreen.Right - gpButtonsWidth;
					if (gpButtonsLeft < SystemInformation.VirtualScreen.Left)
						gpButtonsLeft = SystemInformation.VirtualScreen.Left;
					if (gpButtonsTop + gpButtonsHeight > SystemInformation.VirtualScreen.Bottom)
						gpButtonsTop = SystemInformation.VirtualScreen.Bottom - gpButtonsHeight;
					if (gpButtonsTop < SystemInformation.VirtualScreen.Top)
						gpButtonsTop = SystemInformation.VirtualScreen.Top;
					*/
					int newleft = gpButtonsLeft + e.X - HitMovingToolbareXY.X;
					int newtop = gpButtonsTop + e.Y - HitMovingToolbareXY.Y;

					bool continuemoving;
					bool toolbarmovedthisframe = false;
					int dleft = 0, dtop = 0;
					if
					(
						IsInsideVisibleScreen(newleft, newtop) &&
						IsInsideVisibleScreen(newleft + gpButtonsWidth, newtop) &&
						IsInsideVisibleScreen(newleft, newtop + gpButtonsHeight) &&
						IsInsideVisibleScreen(newleft + gpButtonsWidth, newtop + gpButtonsHeight)
					)
					{
						continuemoving = true;
						ToolbarMoved = true;
						toolbarmovedthisframe = true;
						dleft = newleft - gpButtonsLeft;
						dtop = newtop - gpButtonsTop;
					}
					else
					{
						do
						{
							if (dleft != newleft - gpButtonsLeft)
								dleft += Math.Sign(newleft - gpButtonsLeft);
							else
								break;
							if
							(
								IsInsideVisibleScreen(gpButtonsLeft + dleft, gpButtonsTop + dtop) &&
								IsInsideVisibleScreen(gpButtonsLeft + gpButtonsWidth + dleft, gpButtonsTop + dtop) &&
								IsInsideVisibleScreen(gpButtonsLeft + dleft, gpButtonsTop + gpButtonsHeight + dtop) &&
								IsInsideVisibleScreen(gpButtonsLeft + gpButtonsWidth + dleft, gpButtonsTop + gpButtonsHeight + dtop)
							)
							{
								continuemoving = true;
								ToolbarMoved = true;
								toolbarmovedthisframe = true;
							}
							else
							{
								continuemoving = false;
								dleft -= Math.Sign(newleft - gpButtonsLeft);
							}
						}
						while (continuemoving);
						do
						{
							if (dtop != newtop - gpButtonsTop)
								dtop += Math.Sign(newtop - gpButtonsTop);
							else
								break;
							if
							(
								IsInsideVisibleScreen(gpButtonsLeft + dleft, gpButtonsTop + dtop) &&
								IsInsideVisibleScreen(gpButtonsLeft + gpButtonsWidth + dleft, gpButtonsTop + dtop) &&
								IsInsideVisibleScreen(gpButtonsLeft + dleft, gpButtonsTop + gpButtonsHeight + dtop) &&
								IsInsideVisibleScreen(gpButtonsLeft + gpButtonsWidth + dleft, gpButtonsTop + gpButtonsHeight + dtop)
							)
							{
								continuemoving = true;
								ToolbarMoved = true;
								toolbarmovedthisframe = true;
							}
							else
							{
								continuemoving = false;
								dtop -= Math.Sign(newtop - gpButtonsTop);
							}
						}
						while (continuemoving);
					}

					if (toolbarmovedthisframe)
					{
						gpButtonsLeft += dleft;
						gpButtonsTop += dtop;
						Root.gpButtonsLeft = gpButtonsLeft;
						Root.gpButtonsTop = gpButtonsTop;
						if (Root.Docked)
							gpButtons.Left = gpButtonsLeft + gpButtonsWidth - btDock.Right;
						else
							gpButtons.Left = gpButtonsLeft;
						gpPenWidth.Left = gpButtonsLeft + btPenWidth.Left - gpPenWidth.Width / 2 + btPenWidth.Width / 2;
						gpPenWidth.Top = gpButtonsTop - gpPenWidth.Height - 10;
						gpButtons.Top = gpButtonsTop;
						Root.UponAllDrawingUpdate = true;
					}
				}
			}
		}

		private void gpButtons_MouseUp(object sender, MouseEventArgs e)
		{
			IsMovingToolbar = 0;
		}

		private void btInkVisible_Click(object sender, EventArgs e)
		{
			//metoda wywoływana przy kliknięciu na przycisk z okiem (ukrywanie/odkrywanie rysunków)
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			Root.SetInkVisible(!Root.InkVisible);
		}

		public void btClear_Click(object sender, EventArgs e)
		{
			//metoda wywoływana przy kliknięciu na przycisk ze śmietnikiem (czyszczenie całego ekranu)
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			Root.ClearInk();
			SaveUndoStrokes();
		}

		private void btUndo_Click(object sender, EventArgs e)
		{
			//metoda wywoływana przy kliknięciu na przycisk z zakręconą strzałką (Tryb cofania)
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			if (!Root.InkVisible)
				Root.SetInkVisible(true);

			Root.UndoInk();
		}

		public void btColor_Click(object sender, EventArgs e)
		{
			//metoda wywoływana przy kliknięciu na przycisk z pisakiem
			//ta sama metoda obsługuje wszystkie przyciski i rzutowanie "sender" na klasę Button pozwala na rozróżnienie pisaków
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			for (int b = 0; b < Root.MaxPenCount; b++)
				if ((Button)sender == btPen[b])
				{
					SelectPen(b);
				}
		}

		public void btEraser_Click(object sender, EventArgs e)
		{
			//metoda wywoływana przy kliknięciu na przycisk z gumką (Tryb usuwania rysunków)
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			SelectPen(-1);
		}


		private void btPan_Click(object sender, EventArgs e)
		{
			//metoda wywoływana przy kliknięciu na przycisk z celownikiem (włączenie trybu przenoszenia rysunków)
			if (ToolbarMoved)
			{
				ToolbarMoved = false;
				return;
			}

			SelectPen(-3);
		}

		short LastF4Status = 0;
		private void FormCollection_FormClosing(object sender, FormClosingEventArgs e)
		{
			// check if F4 key is pressed and we assume it's Alt+F4
			short retVal = GetKeyState(0x73);
			if ((retVal & 0x8000) == 0x8000 && (LastF4Status & 0x8000) == 0x0000)
			{
				e.Cancel = true;

				// the following block is copyed from tiSlide_Tick() where we check whether ESC is pressed
				if (Root.Snapping > 0)
				{
					ExitSnapping();
				}
				else if (Root.gpPenWidthVisible)
				{
					Root.gpPenWidthVisible = false;
					Root.UponSubPanelUpdate = true;
				}
				else if (Root.Snapping == 0)
					RetreatAndExit();
			}

			LastF4Status = retVal;
		}

		[DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		[DllImport("user32.dll", SetLastError = true)]
		static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);
		[DllImport("user32.dll")]
		public extern static bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
		[DllImport("user32.dll", SetLastError = false)]
		static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern short GetKeyState(int keyCode);

		[DllImport("gdi32.dll")]
		static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
		[DllImport("user32.dll")]
		static extern IntPtr GetDC(IntPtr hWnd);
		[DllImport("user32.dll")]
		static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
	}
}
