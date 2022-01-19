using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Ink;
using System.Drawing.Text;

namespace gInk
{
	public class TestMessageFilter : IMessageFilter
	{
		public Root Root;

		public TestMessageFilter(Root root)
		{
			Root = root;
		}

		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == 0x0312)
			{
				//Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
				//int modifier = (int)m.LParam & 0xFFFF;       // The modifier of the hotkey that was pressed.
				//int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

				if (Root.FormCollection == null && Root.FormDisplay == null)
					Root.StartInk();
				else if (Root.PointerMode)
				{
					//Root.UnPointer();
					Root.SelectPen(Root.LastPen);
				}
				else
				{
					//Root.Pointer();
					Root.SelectPen(-2);
				}
			}
			return false;
		}
	}

	public class Root
	{
		//klasa Local kontrolująca tekst wyświetlany w programie, przypisuje do każdej kontrolki tekst w odpowiednim języku (domyślnie angielski)
		public Local Local = new Local();
		public const int MaxPenCount = 10;

		// opcje programu
		public bool[] PenEnabled = new bool[MaxPenCount];
		public bool EraserEnabled = true;
		public bool PointerEnabled = true;
		public bool PenWidthEnabled = false;
		public bool SnapEnabled = true;
		public bool UndoEnabled = true;
		public bool ClearEnabled = true;
		public bool PanEnabled = true;
		public bool InkVisibleEnabled = true;
		public DrawingAttributes[] PenAttr = new DrawingAttributes[MaxPenCount];
		public bool AutoScroll;
		public bool WhiteTrayIcon;
		public string SnapshotBasePath;
		public int CanvasCursor = 0;
		public int CursorSize = 3;
		public bool AllowDraggingToolbar = true;
		public bool AllowHotkeyInPointerMode = true;
		public int gpButtonsLeft, gpButtonsTop;

		public bool LineEnabled = true;
		public bool RectEnabled = true;
		public bool EllipseEnabled = true;
		public bool ArrowEnabled = true;
		public bool TextEnabled = true;
		public bool HandDrawnEnabled = true;


		// opcje zaawansowane

		public string CloseOnSnap = "blankonly";
		public bool AlwaysHideToolbar = false;

		//zmienna kontroluje, jaką część ekranu zajmują przyciski
		public float ToolbarHeight = 0.06f;

		// opcje skrótów klawiszowych
		// specjalnie dla programu zaimplementowana klasa Hotkey kontrolująca skróty klawiszowe
		public Hotkey Hotkey_Global = new Hotkey();
		public Hotkey[] Hotkey_Pens = new Hotkey[10];
		public Hotkey Hotkey_Eraser = new Hotkey();
		public Hotkey Hotkey_InkVisible = new Hotkey();
		public Hotkey Hotkey_Pointer = new Hotkey();
		public Hotkey Hotkey_Pan = new Hotkey();
		public Hotkey Hotkey_Undo = new Hotkey();
		public Hotkey Hotkey_Redo = new Hotkey();
		public Hotkey Hotkey_Snap = new Hotkey();
		public Hotkey Hotkey_Clear = new Hotkey();

		public Hotkey Hotkey_Draw = new Hotkey();
		public Hotkey Hotkey_Line = new Hotkey();
		public Hotkey Hotkey_Arrow = new Hotkey();
		public Hotkey Hotkey_Rect = new Hotkey();
		public Hotkey Hotkey_Ellipse = new Hotkey();
		public Hotkey Hotkey_Text = new Hotkey();

		//kolejne opcje
		public bool EraserMode = false;
		public bool Docked = false;
		public bool PointerMode = false;
		public bool FingerInAction = false;  // true when mouse down, either drawing or snapping or whatever
		public int Snapping = 0;  // <=0: not snapping, 1: waiting finger, 2:dragging
		public int SnappingX = -1, SnappingY = -1;
		public Rectangle SnappingRect;
		public int UponButtonsUpdate = 0;
		public bool UponTakingSnap = false;
		public bool UponBalloonSnap = false;
		public bool UponSubPanelUpdate = false;
		public bool UponAllDrawingUpdate = false;
		public bool MouseMovedUnderSnapshotDragging = false; // used to pause re-drawing when mouse is not moving during dragging to take a screenshot

		public bool PanMode = false;
		public bool InkVisible = true;
		public int CurrentFontIndex = 4;
		public int FontSize = 20;

		public bool FitFontToRect = true;
		public bool FixedArrowLength = true;
		public int ArrowLength = 10;

		public InstalledFontCollection IFC = new InstalledFontCollection();

		public enum DrawingMode
        {
			Normal,
			Line,
			Rectangle,
			Arrow,
			Ellipse,
			Text,
        };

		public DrawingMode currentDrawingMode = DrawingMode.Normal;

		public Guid TextGuid;
		public Guid FontGuid;
		public Guid FontSizeGuid;

		//zmienne do przechowywania wsp. punktu rozpoczęcia rysowania (do rysowania nowych grafik)
		public Line DrawnLine;
		public Point RectStart;
		public Rectangle DrawnRect;

		public Ink[] UndoStrokes;
		//public Ink UponUndoStrokes;
		public int UndoP;
		public int UndoDepth, RedoDepth;

		//ikonka powiadomienia, która wyświetla się w pasku zadań
		public NotifyIcon trayIcon;

		//menu wyświetlające się po kliknięciu PPM na ikonkę w pasku zadań
		public ContextMenu trayMenu;

		//Formsy kontrolujące rysowanie i jego opcje
		public FormCollection FormCollection;
		public FormDisplay FormDisplay;
		public FormButtonHitter FormButtonHitter;

		//Forms z checkboxami i ComboBoxami pozwalającymi na zmianę opcji programu
		public FormOptions FormOptions;

		//domyślny pisak
		public int CurrentPen = 1; 
		public int LastPen = 1;

		public int PreviousPen = 1;
		public int GlobalPenWidth = 80;

		//widoczność panelu do wybierania grubości pisaka
		public bool gpPenWidthVisible = false;

		public bool textInputPanelVisible = false;

		//
		public string SnapshotFileFullPath = ""; // used to record the last snapshot file name, to select it when the balloon is clicked

		public Root()
		{
			//rejestracja skrótów klawiszowych do zmiany pisaka (domyślnie 10 pisaków do zmiany klawiszami 0-9)
			for (int p = 0; p < MaxPenCount; p++)
				Hotkey_Pens[p] = new Hotkey();

			//inicjalizacja i dodanie opcji w ContextMenu, czyli tego pojawiającego się po kliknięciu PPM na ikonkę w pasku zadań
			trayMenu = new ContextMenu();
			trayMenu.MenuItems.Add(Local.MenuEntryAbout + "...", OnAbout); //wyświetlanie okna z informacjami na temat wersji programu i autorów
			trayMenu.MenuItems.Add(Local.MenuEntryOptions + "...", OnOptions); //wyświetlanie okna z konfiguracją programu
			trayMenu.MenuItems.Add("-"); //linia oddzielająca "About" i "Options" z "Exit"
			trayMenu.MenuItems.Add(Local.MenuEntryExit, OnExit); //zamykanie okna

			//ustawienie domyślnych kolorów, przezroczystości i szerokości pisaków, czytane z kodu funkcji SetDefaultPens
			SetDefaultPens();
			
			//ustawienie niektórych domyślnych opcji, jak Global Hotkey (do przełączania między kursorem a rysowaniem) czy kolor ikonki
			SetDefaultConfig();

			//czytanie opcji dotyczących pisaków, zwykłych opcji i skrótów klawiszowych zawartych w plikach *.ini
			//w oknie Options mamy trzy TabPage odpowiadające tym trzem częściom konfiguracji: General, Pens oraz Hotkeys
			ReadOptions("pens.ini");
			ReadOptions("config.ini");
			ReadOptions("hotkeys.ini");			

			Size size = SystemInformation.SmallIconSize;
			
			//inicjalizacji ikonki w pasku zadań
			trayIcon = new NotifyIcon();

			trayIcon.Text = "gInk"; //tekst po najechaniu myszką na ikonkę
			trayIcon.ContextMenu = trayMenu; //przypisanie stworzonego wcześniej ContextMenu do ikonki
			trayIcon.Visible = true; //ustawienie widoczności ikonki
			trayIcon.MouseClick += TrayIcon_Click; //dodanie funkcji wykonującej się przy kliknięciu LPM na ikonkę
			trayIcon.BalloonTipText = Local.NotificationSnapshot; //tekst wyświetlany przy pojawieniu się powiadomienia o zrzucie ekranu
			trayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked; //funkcja wykonująca się przy kliknięciu w powiadomienie - otwarcie folderu z 
			
			//funkcja ustawiające kolor ikonki (w konfiguracji istnieje opcja zmiany między czerwoną a białą)
			SetTrayIconColor();

			//rejestrowanie ustawionych skrótów klawiszowych
			SetHotkey();
			
			// ???
			TestMessageFilter mf = new TestMessageFilter(this);
			Application.AddMessageFilter(mf);

			TextGuid = Guid.NewGuid();
			FontGuid = Guid.NewGuid();

			//FormCollection i FromDisplay są elementami programu ściśle związanymi z procesem rysowania
			FormCollection = null;
			FormDisplay = null;
		}

		private void TrayIcon_BalloonTipClicked(object sender, EventArgs e)
		{
			//string snapbasepath = SnapshotBasePath;
			//snapbasepath = Environment.ExpandEnvironmentVariables(snapbasepath);
			//System.Diagnostics.Process.Start(snapbasepath);

			//czytanie ścieżki do folderu z zrzutami ekranu i otworzenie folderu za pomocą uruchomienia procesu explorer.exe z tą ścieżką
			string fullpath = System.IO.Path.GetFullPath(SnapshotFileFullPath);
			System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", fullpath));
		}

		private void TrayIcon_Click(object sender, MouseEventArgs e)
		{
			//funkcja uruchamiana przy kliknięciu LPM na ikonkę w pasku zadań
			if (e.Button == MouseButtons.Left)
			{

				//zapobieganie wykonywania, gdy FormDisplay i FormCollection posiadają referencję
				//w ten sposób tylko raz inicjuje się mechanizm rysowania
				if (FormDisplay == null && FormCollection == null)
				{
					//czytanie opcji ogólnych, dotyczących pisaków i dotyczących skrótów klawiszowych
					ReadOptions("pens.ini");
					ReadOptions("config.ini");
					ReadOptions("hotkeys.ini");

					//funkcja StartInk uruchamia mechanizm rysowania i wyświetla przyciski
					StartInk();
				}

				else if (Docked)
					UnDock();
			}
		}

		public void StartInk()
		{
			//funkcja nie jest wykonywana, gdy mechanizm rysowania jest już uruchomiony
			if (FormDisplay != null || FormCollection != null)
				return;

			//Docked = false;

			//konstruktory trzech najważniejszych klas odpowiadających za mechanizm rysowania

			FormDisplay = new FormDisplay(this);
			FormCollection = new FormCollection(this);

			//ButtonHitter służy do trybu programu, w którym działa normalny kursor i można nim działać zwyczajnie na system
			//podczas gdy nadal widoczne są rysunki
			FormButtonHitter = new FormButtonHitter(this);

			//sprawdzenie, czy któryś z pisaków jest aktywny

			//gdy nie ma żadnego aktywnego w menu, zmienna CurrentPen jest ustawiana na wartość -2, która oznacza
			//Pointer Mode (zwyczajna obsługa systemu kursorem). 
			if (CurrentPen < 0)
				CurrentPen = 0;
			if (!PenEnabled[CurrentPen])
			{
				CurrentPen = 0;
				while (CurrentPen < MaxPenCount && !PenEnabled[CurrentPen])
					CurrentPen++;
				if (CurrentPen == MaxPenCount)
					CurrentPen = -2;
			}

			//wybieranie obecnego pisaka, metoda woła metodę o tej samej nazwie z klasy FormCollection
			SelectPen(CurrentPen);

			//funkcja do ukrywania i odkrywania rysunków z ekranu
			SetInkVisible(true);

			FormCollection.ButtonsEntering = 1;
			FormDisplay.Show();
			FormCollection.Show();
			FormDisplay.DrawButtons(true);

			//inicjalizacja tablicy Strokes'ów (pojedynczych rysunków bez puszczania myszki) służąca do przechowywania
			//cofniętych rysunków do ich późniejszego odzyskania
			if (UndoStrokes == null)
			{
				UndoStrokes = new Ink[8];
				UndoStrokes[0] = FormCollection.IC.Ink.Clone();
				UndoDepth = 0;
				UndoP = 0;
			}

			//UponUndoStrokes = FormCollection.IC.Ink.Clone();
		}
		public void StopInk()
		{
			//zamykanie trybu rysowania i chowanie przycisków 

			//
			FormCollection.Close();
			FormDisplay.Close();
			FormButtonHitter.Close();
			//FormCollection.Dispose();
			//FormDisplay.Dispose();
			GC.Collect();
			FormCollection = null;
			FormDisplay = null;

			//jeśli wykonany został screenshot w trakcie pracy z rysowaniem,
			//to po zamknięciu trybu rysowania pokaże się "Balloon Snapshot" (powiadomienie w Win10) z informacją o zapisaniu
			//screenshota w domyślnym(bądź zmienionym) folderze
			if (UponBalloonSnap)
			{
				ShowBalloonSnapshot();

				//ustawienie na false, aby przy kolejnych zamknięciach trybu pracy z rysowaniem nie wyświetlała się informacja
				//o starym screenshocie
				UponBalloonSnap = false;
			}
		}

		private void UpdateDisplay(bool clearing)
		{
			FormDisplay.ClearCanvus();

			if (!clearing)
				FormDisplay.DrawStrokes();

			FormDisplay.DrawButtons(true);
			FormDisplay.UpdateFormDisplay(true);
		}

		public void ClearInk()
		{
			//funkcja czyszcząca ekran ze wszystkich rysunków po kliknięciu na przycisk ze śmietnikiem

			FormCollection.IC.Ink.DeleteStrokes();
			UpdateDisplay(true);
		}

		public void ShowBalloonSnapshot()
		{
			trayIcon.ShowBalloonTip(3000);
		}

		public void UndoInk()
        {
            //UndoDepth kontroluje ile można wykonać cofnięć za pomocą przycisku bądź skrótku kl. CTRL + Z
            //jeśli jest mniejsze lub równe 0, funkcja się oczywiście nie wykonuje
            if (UndoDepth <= 0)
                return;

            UndoP--;
            if (UndoP < 0)
                UndoP = UndoStrokes.GetLength(0) - 1;

            UndoDepth--;

            //RedoDepth kontroluje ile można wykonać operacji (Redo), czyli przywrócenia cofniętego rysunku
            RedoDepth++;

            FormCollection.IC.Ink.DeleteStrokes();
            if (UndoStrokes[UndoP].Strokes.Count > 0)
                FormCollection.IC.Ink.AddStrokesAtRectangle(UndoStrokes[UndoP].Strokes, UndoStrokes[UndoP].Strokes.GetBoundingBox());

            UpdateDisplay(false);
        }


        public void Pan(int x, int y)
		{
			//funkcja Pan służąca do przenoszenia rysunków za pomocą myszki po kliknięciu na przycisk z obrazkiem kursora
			if (x == 0 && y == 0)
				return;

			//przenoszenie wszystkich rysunków
			FormCollection.IC.Ink.Strokes.Move(x, y);

			UpdateDisplay(false);
		}

		public void SetInkVisible(bool visible)
		{
			InkVisible = visible;

			//warunek kontrolujący wyświetlaną grafikę na przycisku od wyłączania widoczności rysunków
			if (visible)
				FormCollection.btInkVisible.Image = FormCollection.image_visible;
			else
				FormCollection.btInkVisible.Image = FormCollection.image_visible_not;

			UpdateDisplay(false);
		}

		

		public void RedoInk()
		{
			//analogiczna metoda do UndoInk(), tylko dotycząca operacji Redo (przywracania cofniętych zmian)
			if (RedoDepth <= 0)
				return;

			UndoDepth++;
			RedoDepth--;
			UndoP++;
			if (UndoP >= UndoStrokes.GetLength(0))
				UndoP = 0;
			FormCollection.IC.Ink.DeleteStrokes();
			if (UndoStrokes[UndoP].Strokes.Count > 0)
				FormCollection.IC.Ink.AddStrokesAtRectangle(UndoStrokes[UndoP].Strokes, UndoStrokes[UndoP].Strokes.GetBoundingBox());

			UpdateDisplay(false);
		}

		public void Dock()
		{
			//dockowanie sprawia, że przyciski dot. wyboru pisaka, ukrywania, zmiany trybu itd. są ukrywane 
			//i jedynym widocznym przyciskiem jest strzałka do rozwinięcia reszty


			if (FormDisplay == null || FormCollection == null)
				return;

			Docked = true;
			gpPenWidthVisible = false;

			//zmiana grafiki na przycisku od dockowania 
			FormCollection.btDock.Image = FormCollection.image_dockback;
			FormCollection.ButtonsEntering = -1;
			UponButtonsUpdate |= 0x2;
		}

		public void UnDock()
		{
			//analogiczna metoda do Dock() ale zajmująca się ponownym rozwijaniem menu z przyciskami
			
			if (FormDisplay == null || FormCollection == null)
				return;

			Docked = false;
			FormCollection.btDock.Image = FormCollection.image_dock;
			FormCollection.ButtonsEntering = 1;
			UponButtonsUpdate |= 0x2;
		}

		public void Pointer()
		{
			//zmiana trybu na Pointer, czyli interakcja myszką z systemem zamiast rysowania
			if (PointerMode == true)
				return;

			PointerMode = true;
			FormCollection.ToThrough();

			//FormButtonHitter odpowiada wyłącznie za interakcję z przyciskami
			FormButtonHitter.Show();
		}

		public void UnPointer()
		{
			//analogiczna metoda do Pointer(), ale kończącą tryb obsługi systemu zwyczajnym kursorem
			if (PointerMode == false)
				return;

			PointerMode = false;
			FormCollection.ToUnThrough();
			FormCollection.ToTopMost();
			FormCollection.Activate();

			FormButtonHitter.Hide();
		}

		public void SelectPen(int pen)
		{
			FormCollection.SelectPen(pen);
		}

		public void SetDefaultPens()
		{
			//ustawienie domyślnych danych dla pisaków


			PenEnabled[0] = false;
			PenAttr[0] = new DrawingAttributes();
			PenAttr[0].Color = Color.FromArgb(80, 80, 80);
			PenAttr[0].Width = 80;
			PenAttr[0].Transparency = 0;

			PenEnabled[1] = true;
			PenAttr[1] = new DrawingAttributes();
			PenAttr[1].Color = Color.FromArgb(225, 60, 60);
			PenAttr[1].Width = 80;
			PenAttr[1].Transparency = 0;

			PenEnabled[2] = true;
			PenAttr[2] = new DrawingAttributes();
			PenAttr[2].Color = Color.FromArgb(30, 110, 200);
			PenAttr[2].Width = 80;
			PenAttr[2].Transparency = 0;

			PenEnabled[3] = true;
			PenAttr[3] = new DrawingAttributes();
			PenAttr[3].Color = Color.FromArgb(235, 180, 55);
			PenAttr[3].Width = 80;
			PenAttr[3].Transparency = 0;

			PenEnabled[4] = true;
			PenAttr[4] = new DrawingAttributes();
			PenAttr[4].Color = Color.FromArgb(120, 175, 70);
			PenAttr[4].Width = 80;
			PenAttr[4].Transparency = 0;

			PenEnabled[5] = true;
			PenAttr[5] = new DrawingAttributes();
			PenAttr[5].Color = Color.FromArgb(235, 125, 15);
			PenAttr[5].Width = 500;
			PenAttr[5].Transparency = 175;

			PenAttr[6] = new DrawingAttributes();
			PenAttr[6].Color = Color.FromArgb(230, 230, 230);
			PenAttr[6].Width = 80;
			PenAttr[6].Transparency = 0;

			PenAttr[7] = new DrawingAttributes();
			PenAttr[7].Color = Color.FromArgb(250, 140, 200);
			PenAttr[7].Width = 80;
			PenAttr[7].Transparency = 0;

			PenAttr[8] = new DrawingAttributes();
			PenAttr[8].Color = Color.FromArgb(25, 180, 175);
			PenAttr[8].Width = 80;
			PenAttr[8].Transparency = 0;

			PenAttr[9] = new DrawingAttributes();
			PenAttr[9].Color = Color.FromArgb(145, 70, 160);
			PenAttr[9].Width = 500;
			PenAttr[9].Transparency = 175;
		}

		public void SetDefaultConfig()
		{
			Hotkey_Global.Control = true;
			Hotkey_Global.Alt = true;
			Hotkey_Global.Shift = false;
			Hotkey_Global.Win = false;
			Hotkey_Global.Key = 'G';

			AutoScroll = false;
			WhiteTrayIcon = false;
			SnapshotBasePath = "%USERPROFILE%/Pictures/gInk/";
		}

		public void SetTrayIconColor()
		{
			//jeśli w opcjach mamy zaznaczoną białą ikonkę, to tutaj odbywa się zamiana tej ikonki

			if (WhiteTrayIcon)
			{
				if (File.Exists("icon_white.ico"))
                {
					trayIcon.Icon = new Icon("icon_white.ico");
                }
				else
                {
					trayIcon.Icon = global::gInk.Properties.Resources.icon_white;

                }
			}
			else
			{
				if (File.Exists("icon_red.ico"))
					trayIcon.Icon = new Icon("icon_red.ico");
				else
					trayIcon.Icon = global::gInk.Properties.Resources.icon_red;
			}


		}

		public void ReadOptions(string file)
		{

			//czytanie z plików opcji ogólnych, pisaków i skrótów klawiszowych za pomocą jednej, tej samej funkcji
			//podobny mechanizm do czytania lokalizacji w klasie Local, przy czym, aby ujednolicić tekst, linie są
			//konwertowane na wielkie litery (funkcja toUpper) i porównywane z tekstem pisanym wielkimi literami

			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

			if (!File.Exists(file))
				file = AppDomain.CurrentDomain.BaseDirectory + file;
			if (!File.Exists(file))
				return;


			FileStream fini = new FileStream(file, FileMode.Open);
			StreamReader srini = new StreamReader(fini);
			string sLine = "";
			string sName = "", sPara = "";
			while (sLine != null)
			{
				sLine = srini.ReadLine();
				if
				(
					sLine != null &&
					sLine != "" &&
					sLine.Substring(0, 1) != "-" &&
					sLine.Substring(0, 1) != "%" &&
					sLine.Substring(0, 1) != "'" &&
					sLine.Substring(0, 1) != "/" &&
					sLine.Substring(0, 1) != "!" &&
					sLine.Substring(0, 1) != "[" &&
					sLine.Substring(0, 1) != "#" &&
					sLine.Contains("=") &&
					!sLine.Substring(sLine.IndexOf("=") + 1).Contains("=")
				)
				{
					sName = sLine.Substring(0, sLine.IndexOf("="));
					sName = sName.Trim();
					sName = sName.ToUpper();
					sPara = sLine.Substring(sLine.IndexOf("=") + 1);
					sPara = sPara.Trim();

					if (sName.StartsWith("PEN"))
					{
						int penid = 0;
						if (int.TryParse(sName.Substring(3, 1), out penid))
						{
							if (sName.EndsWith("_ENABLED"))
							{
								if (sPara.ToUpper() == "TRUE" || sPara == "1" || sPara.ToUpper() == "ON")
									PenEnabled[penid] = true;
								else if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
									PenEnabled[penid] = false;
							}

							int penc = 0;
							if (int.TryParse(sPara, out penc))
							{
								if (sName.EndsWith("_RED") && penc >= 0 && penc <= 255)
								{
									PenAttr[penid].Color = Color.FromArgb(penc, PenAttr[penid].Color.G, PenAttr[penid].Color.B);
								}
								else if (sName.EndsWith("_GREEN") && penc >= 0 && penc <= 255)
								{
									PenAttr[penid].Color = Color.FromArgb(PenAttr[penid].Color.R, penc, PenAttr[penid].Color.B);
								}
								else if (sName.EndsWith("_BLUE") && penc >= 0 && penc <= 255)
								{
									PenAttr[penid].Color = Color.FromArgb(PenAttr[penid].Color.R, PenAttr[penid].Color.G, penc);
								}
								else if (sName.EndsWith("_ALPHA") && penc >= 0 && penc <= 255)
								{
									PenAttr[penid].Transparency = (byte)(255 - penc);
								}
								else if (sName.EndsWith("_WIDTH") && penc >= 30 && penc <= 3000)
								{
									PenAttr[penid].Width = penc;
								}
							}

							if (sName.EndsWith("_HOTKEY"))
							{
								Hotkey_Pens[penid].Parse(sPara);
							}
						}

					}

					int tempi = 0;
					float tempf = 0;
					switch (sName)
					{
						case "LANGUAGE_FILE":
							ChangeLanguage(sPara);
							break;
						case "HOTKEY_GLOBAL":
							Hotkey_Global.Parse(sPara);
							break;
						case "HOTKEY_ERASER":
							Hotkey_Eraser.Parse(sPara);
							break;
						case "HOTKEY_INKVISIBLE":
							Hotkey_InkVisible.Parse(sPara);
							break;
						case "HOTKEY_POINTER":
							Hotkey_Pointer.Parse(sPara);
							break;
						case "HOTKEY_PAN":
							Hotkey_Pan.Parse(sPara);
							break;
						case "HOTKEY_UNDO":
							Hotkey_Undo.Parse(sPara);
							break;
						case "HOTKEY_REDO":
							Hotkey_Redo.Parse(sPara);
							break;
						case "HOTKEY_SNAPSHOT":
							Hotkey_Snap.Parse(sPara);
							break;
						case "HOTKEY_CLEAR":
							Hotkey_Clear.Parse(sPara);
							break;
						case "HOTKEY_DRAW":
							Hotkey_Draw.Parse(sPara);
							break;
						case "HOTKEY_LINE":
							Hotkey_Line.Parse(sPara);
							break;
						case "HOTKEY_ARROW":
							Hotkey_Arrow.Parse(sPara);
							break;
						case "HOTKEY_RECT":
							Hotkey_Rect.Parse(sPara);
							break;
						case "HOTKEY_ELLIPSE":
							Hotkey_Ellipse.Parse(sPara);
							break;
						case "HOTKEY_TEXT":
							Hotkey_Text.Parse(sPara);
							break;

						case "WHITE_TRAY_ICON":
							if (sPara.ToUpper() == "TRUE" || sPara == "1" || sPara.ToUpper() == "ON")
								WhiteTrayIcon = true;
							else
								WhiteTrayIcon = false;
							break;
						case "SNAPSHOT_PATH":
							SnapshotBasePath = sPara;
							if (!SnapshotBasePath.EndsWith("/") && !SnapshotBasePath.EndsWith("\\"))
								SnapshotBasePath += "/";
							break;
						case "ERASER_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								EraserEnabled = false;
							break;
						case "POINTER_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								PointerEnabled = false;
							break;
						case "PEN_WIDTH_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								PenWidthEnabled = false;
							else if (sPara.ToUpper() == "TRUE" || sPara == "1" || sPara.ToUpper() == "ON")
								PenWidthEnabled = true;
							break;
						case "SNAPSHOT_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								SnapEnabled = false;
							break;
						case "CLOSE_ON_SNAP":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								CloseOnSnap = "false";
							else if (sPara.ToUpper() == "TRUE" || sPara == "1" || sPara.ToUpper() == "ON")
								CloseOnSnap = "true";
							else if (sPara.ToUpper() == "BLANKONLY")
								CloseOnSnap = "blankonly";
							break;
						case "ALWAYS_HIDE_TOOLBAR":
							if (sPara.ToUpper() == "TRUE" || sPara == "1" || sPara.ToUpper() == "ON")
								AlwaysHideToolbar = true;
							break;
						case "UNDO_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								UndoEnabled = false;
							break;
						case "CLEAR_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								ClearEnabled = false;
							break;
						case "PAN_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								PanEnabled = false;
							break;
						case "INKVISIBLE_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								InkVisibleEnabled = false;
							break;
						case "ALLOW_DRAGGING_TOOLBAR":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								AllowDraggingToolbar = false;
							break;
						case "ALLOW_HOTKEY_IN_POINTER_MODE":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								AllowHotkeyInPointerMode = false;
							break;
						case "TOOLBAR_LEFT":
							if (int.TryParse(sPara, out tempi))
								gpButtonsLeft = tempi;
							break;
						case "TOOLBAR_TOP":
							if (int.TryParse(sPara, out tempi))
								gpButtonsTop = tempi;
							break;
						case "TOOLBAR_HEIGHT":
							if (float.TryParse(sPara, out tempf))
								ToolbarHeight = tempf;
							break;
						case "CANVAS_CURSOR":
							if (sPara == "0")
								CanvasCursor = 0;
							else if (sPara == "1")
								CanvasCursor = 1;
							break;
						case "CURSOR_SIZE":
							if (int.TryParse(sPara, out tempi))
								CursorSize = tempi;
							break;
						case "FONT_INDEX":
							if (int.TryParse(sPara, out tempi))
								CurrentFontIndex = tempi;
							break;
						case "FONT_SIZE":
							if (int.TryParse(sPara, out tempi))
								FontSize = tempi;
							break;
						case "ARROW_LENGTH":
							if (int.TryParse(sPara, out tempi))
								ArrowLength = tempi;
							break;
						case "TEXT_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								TextEnabled = false;
							break;
						case "ELLIPSE_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								EllipseEnabled = false;
							break;
						case "RECTANGLE_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								RectEnabled = false;
							break;
						case "ARROW_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								ArrowEnabled = false;
							break;
						case "LINE_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								LineEnabled = false;
							break;
						case "HANDDRAWN_ICON":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								HandDrawnEnabled = false;
							break;
						case "FONT_FIT":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								FitFontToRect = false;
							break;
						case "FIXED_ARROW":
							if (sPara.ToUpper() == "FALSE" || sPara == "0" || sPara.ToUpper() == "OFF")
								FixedArrowLength = false;
							break;
					}
				}
			}
			fini.Close();
		}

		public void SaveOptions(string file)
		{
			if (!File.Exists(file))
				file = AppDomain.CurrentDomain.BaseDirectory + file;
			if (!File.Exists(file))
				return;

			FileStream fini = new FileStream(file, FileMode.Open);
			StreamReader srini = new StreamReader(fini);
			string sLine = "";
			string sNameO = "";
			string sName = "", sPara = "";

			List<string> writelines = new List<string>();

			while (sLine != null)
			{
				sPara = "";
				sLine = srini.ReadLine();
				if
				(
					sLine != null &&
					sLine != "" &&
					sLine.Substring(0, 1) != "-" &&
					sLine.Substring(0, 1) != "%" &&
					sLine.Substring(0, 1) != "'" &&
					sLine.Substring(0, 1) != "/" &&
					sLine.Substring(0, 1) != "!" &&
					sLine.Substring(0, 1) != "[" &&
					sLine.Substring(0, 1) != "#" &&
					sLine.Contains("=") &&
					!sLine.Substring(sLine.IndexOf("=") + 1).Contains("=")
				)
				{
					sNameO = sLine.Substring(0, sLine.IndexOf("="));
					sName = sNameO.Trim().ToUpper();

					if (sName.StartsWith("PEN"))
					{
						int penid = 0;
						if (int.TryParse(sName.Substring(3, 1), out penid) && penid >= 0 && penid < MaxPenCount)
						{
							if (sName.EndsWith("_ENABLED"))
							{
								if (PenEnabled[penid])
									sPara = "True";
								else
									sPara = "False";
							}
							else if (sName.EndsWith("_RED"))
							{
								sPara = PenAttr[penid].Color.R.ToString();
							}
							else if (sName.EndsWith("_GREEN"))
							{
								sPara = PenAttr[penid].Color.G.ToString();
							}
							else if (sName.EndsWith("_BLUE"))
							{
								sPara = PenAttr[penid].Color.B.ToString();
							}
							else if (sName.EndsWith("_ALPHA"))
							{
								sPara = (255 - PenAttr[penid].Transparency).ToString();
							}
							else if (sName.EndsWith("_WIDTH"))
							{
								sPara = ((int)PenAttr[penid].Width).ToString();
							}
							else if (sName.EndsWith("_HOTKEY"))
							{
								sPara = Hotkey_Pens[penid].ToString();
							}
						}

					}

					switch (sName)
					{
						case "LANGUAGE_FILE":
							sPara = Local.CurrentLanguageFile;
							break;
						case "HOTKEY_GLOBAL":
							sPara = Hotkey_Global.ToString();
							break;
						case "HOTKEY_ERASER":
							sPara = Hotkey_Eraser.ToString();
							break;
						case "HOTKEY_INKVISIBLE":
							sPara = Hotkey_InkVisible.ToString();
							break;
						case "HOTKEY_POINTER":
							sPara = Hotkey_Pointer.ToString();
							break;
						case "HOTKEY_PAN":
							sPara = Hotkey_Pan.ToString();
							break;
						case "HOTKEY_UNDO":
							sPara = Hotkey_Undo.ToString();
							break;
						case "HOTKEY_REDO":
							sPara = Hotkey_Redo.ToString();
							break;
						case "HOTKEY_SNAPSHOT":
							sPara = Hotkey_Snap.ToString();
							break;
						case "HOTKEY_CLEAR":
							sPara = Hotkey_Clear.ToString();
							break;
						case "HOTKEY_DRAW":
							sPara = Hotkey_Draw.ToString();
							break;
						case "HOTKEY_LINE":
							sPara = Hotkey_Line.ToString();
							break;
						case "HOTKEY_ARROW":
							sPara = Hotkey_Arrow.ToString();
							break;
						case "HOTKEY_RECT":
							sPara = Hotkey_Rect.ToString();
							break;
						case "HOTKEY_ELLIPSE":
							sPara = Hotkey_Ellipse.ToString();
							break;
						case "HOTKEY_TEXT":
							sPara = Hotkey_Text.ToString();
							break;

						case "WHITE_TRAY_ICON":
							if (WhiteTrayIcon)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "SNAPSHOT_PATH":
							sPara = SnapshotBasePath;
							break;
						case "ERASER_ICON":
							if (EraserEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "POINTER_ICON":
							if (PointerEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "PEN_WIDTH_ICON":
							if (PenWidthEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "SNAPSHOT_ICON":
							if (SnapEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "CLOSE_ON_SNAP":
							if (CloseOnSnap == "true")
								sPara = "True";
							else if (CloseOnSnap == "false")
								sPara = "False";
							else
								sPara = "BlankOnly";
							break;
						case "ALWAYS_HIDE_TOOLBAR":
							if (AlwaysHideToolbar)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "UNDO_ICON":
							if (UndoEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "CLEAR_ICON":
							if (ClearEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "PAN_ICON":
							if (PanEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "INKVISIBLE_ICON":
							if (PanEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "ALLOW_DRAGGING_TOOLBAR":
							if (AllowDraggingToolbar)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "ALLOW_HOTKEY_IN_POINTER_MODE":
							if (AllowHotkeyInPointerMode)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "TOOLBAR_LEFT":
							sPara = gpButtonsLeft.ToString();
							break;
						case "TOOLBAR_TOP":
							sPara = gpButtonsTop.ToString();
							break;
						case "TOOLBAR_HEIGHT":
							sPara = ToolbarHeight.ToString();
							break;
						case "CANVAS_CURSOR":
							sPara = CanvasCursor.ToString();
							break;
						case "CURSOR_SIZE":
							sPara = CursorSize.ToString();
							break;
						case "FONT_INDEX":
							sPara = CurrentFontIndex.ToString();
							break;
						case "FONT_SIZE":
							sPara = FontSize.ToString();
							break;
						case "ARROW_LENGTH":
							sPara = ArrowLength.ToString();
							break;
						case "TEXT_ICON":
							if (TextEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "ELLIPSE_ICON":
							if (EllipseEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "RECTANGLE_ICON":
							if (RectEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "ARROW_ICON":
							if (ArrowEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "LINE_ICON":
							if (LineEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "HANDDRRAWN_ICON":
							if (HandDrawnEnabled)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "FONT_FIT":
							if (FitFontToRect)
								sPara = "True";
							else
								sPara = "False";
							break;
						case "FIXED_ARROW":
							if (FixedArrowLength)
								sPara = "True";
							else
								sPara = "False";
							break;
					}
				}
				if (sPara != "")
					writelines.Add(sNameO + "= " + sPara);
				else if (sLine != null)
					writelines.Add(sLine);
			}
			fini.Close();

			FileStream frini = new FileStream(file, FileMode.Create);
			StreamWriter swini = new StreamWriter(frini);
			swini.AutoFlush = true;
			foreach (string line in writelines)
				swini.WriteLine(line);
			frini.Close();
		}

		private void OnAbout(object sender, EventArgs e)
		{
			//metoda wykonywująca się przy kliknięciu na "About" w ContextMenu
			//uruchamia się osobne okienko definiowane w klasie FormAbout
			FormAbout FormAbout = new FormAbout();
			FormAbout.Show();
		}
		/*
		private void OnPenSetting(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("notepad.exe", "pens.ini");
		}
		*/
		private void OnOptions(object sender, EventArgs e)
		{
			//metoda wykonywująca się przy kliknięciu na "Options" w ContextMenu
			//wyświetla okno z konfiguracją definiowane w klasie FormOptions

			//okno Options uruchamia się tylko pojedynczy raz
			if (FormOptions != null)
				return;

			//okno Options nie uruchamia się podczas aktywnego mechanizmu rysowania
			if (FormDisplay != null || FormCollection != null)
				return;

			//czytanie opcji z plików
			ReadOptions("pens.ini");
			ReadOptions("config.ini");
			ReadOptions("hotkeys.ini");
			FormOptions = new FormOptions(this);
			FormOptions.Show();
		}

		public void SetHotkey()
		{
			//ustawienie skrótu klawiszowego

			int modifier = 0;
			if (Hotkey_Global.Control) modifier |= 0x2;
			if (Hotkey_Global.Alt) modifier |= 0x1;
			if (Hotkey_Global.Shift) modifier |= 0x4;
			if (Hotkey_Global.Win) modifier |= 0x8;
			if (modifier != 0)
				//RegisterHotKey jest metodą importowaną z biblioteki user32.dll
				//wysyłany jest wskaźnik zerowy (czyli skrót nie dotyczy danego okna tylko jest aktywny globalnie)
				RegisterHotKey(IntPtr.Zero, 0, modifier, Hotkey_Global.Key);
		}

		public void UnsetHotkey()
		{
			//analogiczna metoda do RegisterHotkey()

			int modifier = 0;
			if (Hotkey_Global.Control) modifier |= 0x2;
			if (Hotkey_Global.Alt) modifier |= 0x1;
			if (Hotkey_Global.Shift) modifier |= 0x4;
			if (Hotkey_Global.Win) modifier |= 0x8;
			if (modifier != 0)
				UnregisterHotKey(IntPtr.Zero, 0);
		}

		public void ChangeLanguage(string filename)
		{
			//zmiana języka 

			//czytanie tekstu w danym języku
			Local.LoadLocalFile(filename);

			//tworzenie od nowa ContextMenu, aby pojawiły się opcje menu w nowym języku
			trayMenu.MenuItems.Clear();
			trayMenu.MenuItems.Add(Local.MenuEntryAbout + "...", OnAbout);
			trayMenu.MenuItems.Add(Local.MenuEntryOptions + "...", OnOptions);
			trayMenu.MenuItems.Add("-");
			trayMenu.MenuItems.Add(Local.MenuEntryExit, OnExit);
		}

		private void OnExit(object sender, EventArgs e)
		{
			//konieczność odrejestrowania skrótów klawiszowych, które działają globalnie
			UnsetHotkey();

			//zamykanie aplikacji
			trayIcon.Dispose();
			Application.Exit();
		}


		//import funkcji rejestrujących skróty klawiszowe z biblioteki user32.dll
		[DllImport("user32.dll")]
		private static extern int RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, int vk);


		[DllImport("user32.dll")]
		private static extern int UnregisterHotKey(IntPtr hwnd, int id);
	}
}

