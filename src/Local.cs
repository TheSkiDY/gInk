using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace gInk
{
	public class Local
	{
		//słownik języków w postaci domena : nazwa języka
		Dictionary<string, string> Languages = new Dictionary<string, string>();

		public string CurrentLanguageFile;

		//tablica nazw pisaków - maksymalnie 10
		public string[] ButtonNamePen = new string[10];

		
		//tekst w przyciskach, menu, opcjach oraz powiadomieniach w języku angielskim
		public string ButtonNamePenwidth = "Pen width";
		public string ButtonNameErasor = "Eraser";
		public string ButtonNamePan = "Pan";
		public string ButtonNameMousePointer = "Mouse pointer";
		public string ButtonNameInkVisible = "Ink visible";
		public string ButtonNameSnapshot = "Snapshot";
		public string ButtonNameUndo = "Undo";
		public string ButtonNameRedo = "Redo";
		public string ButtonNameClear = "Clear";
		public string ButtonNameExit = "Exit drawing";
		public string ButtonNameDock = "Dock";

		public string ButtonNameDraw = "Hand drawn";
		public string ButtonNameLine = "Line";
		public string ButtonNameArrow = "Arrow";
		public string ButtonNameRect = "Rectangle";
		public string ButtonNameEllipse = "Ellipse";
		public string ButtonNameText = "Text";


		public string MenuEntryExit = "Exit";
		public string MenuEntryOptions = "Options";
		public string MenuEntryAbout = "About";

		public string OptionsTabGeneral = "General";
		public string OptionsTabPens = "Pens";
		public string OptionsTabHotkeys = "Hotkeys";

		public string OptionsGeneralLanguage = "Language";
		public string OptionsGeneralCanvascursor = "Canvus cursor";
		public string OptionsGeneralCanvascursorArrow = "Arrow";
		public string OptionsGeneralCanvascursorPentip = "Pen tip";
		public string OptionsGeneralCursorsize = "Cursor size";
		public string OptionsGeneralSnapshotsavepath = "Snapshot save path";
		public string OptionsGeneralWhitetrayicon = "Use white tray icon";
		public string OptionsGeneralAllowdragging = "Allow dragging toolbar";
		public string OptionsGeneralNotePenwidth = "Note: pen width panel overides each individual pen width settings";

		public string OptionsGeneralFont = "Font";
		public string OptionsGeneralFontSize = "Font size";
		public string OptionsGeneralAllowDynamicArrowLength = "Fixed arrow length";
		public string OptionsGeneralArrowLength = "Arrow length";
		public string OptionsGeneralFixedFont = "Fit font automatically";

		public string OptionsPensShow = "Show";
		public string OptionsPensColor = "Color";
		public string OptionsPensAlpha = "Alpha";
		public string OptionsPensWidth = "Width";
		public string OptionsPensPencil = "Pencil";
		public string OptionsPensHighlighter = "Highlighter";
		public string OptionsPensThin = "Thin";
		public string OptionsPensNormal = "Normal";
		public string OptionsPensThick = "Thick";

		public string OptionsHotkeysglobal = "Global hotkey (start drawing, switch between mouse pointer and drawing)";
		public string OptionsHotkeysEnableinpointer = "Enable all following hotkeys in mouse pointer mode (may cause a mess)";

		public string NotificationSnapshot = "Snapshot saved. Click here to browse snapshots.";

		public Local()
		{
			//przypisywanie nazwy pisaków w konstruktorze po inicjacji obiektu tablicy
			ButtonNamePen[0] = "Pen 0";
			ButtonNamePen[1] = "Pen 1";
			ButtonNamePen[2] = "Pen 2";
			ButtonNamePen[3] = "Pen 3";
			ButtonNamePen[4] = "Pen 4";
			ButtonNamePen[5] = "Pen 5";
			ButtonNamePen[6] = "Pen 6";
			ButtonNamePen[7] = "Pen 7";
			ButtonNamePen[8] = "Pen 8";
			ButtonNamePen[9] = "Pen 9";

			//funkcja dodająca do słownika możliwe języki
			LoadLocalList();
		}

		public void LoadLocalList()
		{
			//możliwe języki czytane z folderu lang
			DirectoryInfo d = new DirectoryInfo("./lang/");
			if (!d.Exists)
				d = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "lang");
			if (!d.Exists)
				return;

			//przygotowanie do pracy tylko na plikach tekstowych
			FileInfo[] Files = d.GetFiles("*.txt");

			//pętla po wszystkich plikach tekstowych dodająca poszczególne języki do słownika Languages
			foreach (FileInfo file in Files)
			{
				FileStream fini = new FileStream(file.FullName, FileMode.Open);

				//StreamReader czyta obecny plik
				StreamReader srini = new StreamReader(fini);

				//zmienna sLine przechowuje linię otwartego pliku
				string sLine;
				do
				{
					sLine = srini.ReadLine();
				}

				//przeszukiwanie pliku tekstowego w poszukiwaniu informacji o nazwie języka (pod linią z tekstem "LanguageName")
				while (sLine != null && !sLine.StartsWith("LanguageName"));
				if (sLine == null)
					continue;
				
				//zmienna sPara przechowuje wartość z nazwą języka dla klucza w postaci domeny (np. "pl")
				//wartość bierze się z oczekiwanej linii (przykładowo) "LanguageName=polski" w pliku z lokalizacją
				string sPara = sLine.Substring(sLine.IndexOf("=") + 1);

				//usuwanie znaków białych i cudzysłowów
				sPara = sPara.Trim();
				sPara = sPara.Trim('\"');
				string languagename = sPara;

				//dodawanie znalezionego języka do słownika Languages
				Languages.Add(file.Name.Substring(0, file.Name.Length - 4), sPara);

				fini.Close();
			}
		}

		public List<string> GetLanguagenames()
		{
			//funkcja zwraca listę możliwych języków wcześniej wyczytanych z plików
			//do wykorzystania w ComboBoxie z wyborem języka programu
			List<string> names = new List<string>();
			foreach (KeyValuePair<string, string> pair in Languages)
				names.Add(pair.Value);

			return names;
		}

		public string GetFilenameByLanguagename(string languagename)
		{
			//zwracanie nazwy pliku, w którym znajduje się lokalizacja dla danego języka (argumentu funkcji)
			foreach (KeyValuePair<string, string> pair in Languages)
				if (pair.Value == languagename)
					return pair.Key;

			return "";
		}

		public string GetLanguagenameByFilename(string filename)
		{
			//funkcja odwrotna do powyższej, zwraca nazwę języka dla danej nazwy pliku jako arguemnt funkcji
			foreach (KeyValuePair<string, string> pair in Languages)
				if (pair.Key == filename)
					return pair.Value;

			return "";
		}

		public void LoadLocalFile(string loname)
		{
			//funkcja ładująca tłumaczenie w danym języku (argument funkcji)

			//otwieranie pliku z danym językiem
			string filename = "./lang/" + loname + ".txt";
			if (!File.Exists(filename))
				filename = AppDomain.CurrentDomain.BaseDirectory + "lang/" + loname + ".txt";
			if (!File.Exists(filename))
				return;

			FileStream fini = new FileStream(filename, FileMode.Open);
			
			//StreamReader srini czyta plik językowy
			StreamReader srini = new StreamReader(fini);
			string sLine = "";
			string sName = "", sPara = "";
			while (sLine != null)
			{
				sLine = srini.ReadLine();
				if
				(
					//funkcja pomija puste linie, bądź te składające się ze znaków białych
					sLine != null &&
					sLine != "" &&

					//funkcja pomija również specjalne znaki pozwalające na wpisanie komentarzy do pliku z tłumaczeniem
					sLine.Substring(0, 1) != "-" &&
					sLine.Substring(0, 1) != "%" &&
					sLine.Substring(0, 1) != "'" &&
					sLine.Substring(0, 1) != "/" &&
					sLine.Substring(0, 1) != "!" &&
					sLine.Substring(0, 1) != "[" &&
					sLine.Substring(0, 1) != "#" &&

					//warunek sprawdzający czy istnieje tylko jeden znak równości w linii
					//pozwala na łatwe przypisanie klucza i wartości po obu stronach znaku równości
					sLine.Contains("=") &&
					!sLine.Substring(sLine.IndexOf("=") + 1).Contains("=")
				)
				{
					//zmienna sName to nazwa zmiennej tekstowej, której wartość powinna się zmienić w ramach tłumaczenia na inny język
					sName = sLine.Substring(0, sLine.IndexOf("="));
					sName = sName.Trim();

					//zmienna sPara to nowy tekst do wpiisania do zmiennej o nazwie przechowywanej w sName
					sPara = sLine.Substring(sLine.IndexOf("=") + 1);
					sPara = sPara.Trim();
					sPara = sPara.Trim('\"');

					if (sName.StartsWith("ButtonNamePen"))
					{
						//specjalne formatowanie do nazw pisaków 
						//program zakłada linię postaci "ButtonNamePenX", gdzie X jest indeksem danego pisaka
						int penid = 0;
						if (int.TryParse(sName.Substring(13, 1), out penid))
						{
							ButtonNamePen[penid] = sPara;
						}
					}

					//mechanizm Reflection zmieniający wartość danej zmiennej o nazwie zawartej w sName na wartość zawartą w sPara
					System.Reflection.FieldInfo fi = typeof(Local).GetField(sName);
					if (fi != null)
						fi.SetValue(this, sPara);
				}
			}
			fini.Close();

			//zmienna tekstowa kontrolująca, który język jest obecnie aktywny
			CurrentLanguageFile = loname;
		}
	}
}
