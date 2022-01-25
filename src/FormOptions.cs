using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gInk
{
    public partial class FormOptions : Form
    {
        public Root Root;

        Label[] lbPens = new Label[10];
        CheckBox[] cbPens = new CheckBox[10];
        PictureBox[] pboxPens = new PictureBox[10];
        ComboBox[] comboPensAlpha = new ComboBox[10];
        ComboBox[] comboPensWidth = new ComboBox[10];
        Label lbcbPens, lbpboxPens, lbcomboPensAlpha, lbcomboPensWidth;

        Label[] lbHotkeyPens = new Label[10];
        HotkeyInputBox[] hiPens = new HotkeyInputBox[10];

        

        public FormOptions(Root root)
        {
            Root = root;
            InitializeComponent();

            comboFont.SelectedIndexChanged -= new System.EventHandler(this.comboFont_SelectedIndexChanged);
            comboFont.DrawMode = DrawMode.OwnerDrawFixed;
            FontFamily[] families = Root.IFC.Families;
            comboFont.DataSource = families;
            comboFont.DrawItem += comboFont_DrawItem;
            comboFont.SelectedIndex = Root.CurrentFontIndex;
            comboFont.SelectedIndexChanged += new System.EventHandler(this.comboFont_SelectedIndexChanged);

            SetIcon();
        }


        private void comboFont_DrawItem(object sender, DrawItemEventArgs e)
        {
            var cFont = (ComboBox)sender; 
            var fontFamily = (FontFamily)cFont.Items[e.Index];
            var font = new Font(fontFamily, cFont.Font.SizeInPoints);

            e.DrawBackground();
            e.Graphics.DrawString(font.Name, font, Brushes.Black, e.Bounds.X, e.Bounds.Y);
        }

        private void SetIcon()
        {
            //ustawienie ikony w górnym pasku okna Options
            if (Root.WhiteTrayIcon)
                if (File.Exists("icon_white.ico"))
                    Icon = new Icon("icon_white.ico");
                else
                    Icon = Properties.Resources.icon_white;
            else
                if (File.Exists("icon_red.ico"))
                Icon = new Icon("icon_red.ico");
            else
                Icon = Properties.Resources.icon_red;
        }

        private void FormOptions_Load(object sender, EventArgs e)
        {
            Root.UnsetHotkey();

            if (Root.EraserEnabled)
                cbEraserEnabled.Checked = true;
            if (Root.PointerEnabled)
                cbPointerEnabled.Checked = true;
            if (Root.SnapEnabled)
                cbSnapEnabled.Checked = true;
            if (Root.UndoEnabled)
                cbUndoEnabled.Checked = true;
            if (Root.ClearEnabled)
                cbClearEnabled.Checked = true;
            if (Root.PenWidthEnabled)
                cbWidthEnabled.Checked = true;
            if (Root.PanEnabled)
                cbPanEnabled.Checked = true;
            if (Root.InkVisibleEnabled)
                cbInkVisibleEnabled.Checked = true;
            if (Root.HandDrawnEnabled)
                cb_HandDrawnEnabled.Checked = true;
            if (Root.LineEnabled)
                cb_LineEnabled.Checked = true;
            if (Root.ArrowEnabled)
                cb_ArrowEnabled.Checked = true;
            if (Root.RectEnabled)
                cb_RectEnabled.Checked = true;
            if (Root.EllipseEnabled)
                cb_EllipseEnabled.Checked = true;
            if (Root.TextEnabled)
                cb_TextEnabled.Checked = true;

            if (Root.FitFontToRect)
                cbFontFit.Checked = true;
            if (Root.FixedArrowLength)
                cbFixedArrow.Checked = true;

            if (Root.WhiteTrayIcon)
                cbWhiteIcon.Checked = true;
            if (Root.AllowDraggingToolbar)
                cbAllowDragging.Checked = true;
            if (Root.AllowHotkeyInPointerMode)
                cbAllowHotkeyInPointer.Checked = true;

            comboCanvasCursor.SelectedIndex = Root.CanvasCursor;

            tbCursorsize.Value = Root.CursorSize;

            comboFont.SelectedIndex = Root.CurrentFontIndex;

            numFontSize.Value = Root.FontSize;

            numArrowSize.Value = Root.ArrowLength;

            tbSnapPath.Text = Root.SnapshotBasePath;

            lbNote.ForeColor = Color.Black;

            lbcbPens = new Label();
            lbcbPens.Left = (int)(this.Width / 500.0 * 25);
            lbcbPens.Width = 100;
            lbcbPens.Top = 15;

            tabPage2.Controls.Add(lbcbPens);
            lbpboxPens = new Label();
            lbpboxPens.Left = (int)(this.Width / 500.0 * 140);
            lbpboxPens.Width = 60;
            lbpboxPens.Top = 15;

            tabPage2.Controls.Add(lbpboxPens);
            lbcomboPensAlpha = new Label();
            lbcomboPensAlpha.Left = (int)(this.Width / 500.0 * 200);
            lbcomboPensAlpha.Width = 100;
            lbcomboPensAlpha.Top = 15;

            tabPage2.Controls.Add(lbcomboPensAlpha);
            lbcomboPensWidth = new Label();
            lbcomboPensWidth.Left = (int)(this.Width / 500.0 * 325);
            lbcomboPensWidth.Width = 100;
            lbcomboPensWidth.Top = 15;

            tabPage2.Controls.Add(lbcomboPensWidth);

            for (int p = 0; p < Root.MaxPenCount; p++)
            {
                int top = p * (int)(this.Height * 0.075) + (int)(this.Height * 0.09);
                lbPens[p] = new Label();
                lbPens[p].Left = (int)(this.Width / 500.0 * 60);
                lbPens[p].Width = 80;
                lbPens[p].Top = top;

                cbPens[p] = new CheckBox();
                cbPens[p].Left = (int)(this.Width / 500.0 * 30);
                cbPens[p].Width = 25;
                cbPens[p].Top = top - 5;
                cbPens[p].Text = "";
                cbPens[p].Checked = Root.PenEnabled[p];
                cbPens[p].CheckedChanged += cbPens_CheckedChanged;

                pboxPens[p] = new PictureBox();
                pboxPens[p].Left = (int)(this.Width / 500.0 * 145);
                pboxPens[p].Top = top;
                pboxPens[p].Width = 15;
                pboxPens[p].Height = 15;
                pboxPens[p].BackColor = Root.PenAttr[p].Color;
                pboxPens[p].Click += pboxPens_Click;

                comboPensAlpha[p] = new ComboBox();

                comboPensAlpha[p].Left = (int)(this.Width / 500.0 * 205);
                comboPensAlpha[p].Top = top - 2;
                comboPensAlpha[p].Width = 100;
                comboPensAlpha[p].Text = (255 - Root.PenAttr[p].Transparency).ToString();
                comboPensAlpha[p].TextChanged += comboPensAlpha_TextChanged;

                comboPensWidth[p] = new ComboBox();

                comboPensWidth[p].Left = (int)(this.Width / 500.0 * 330);
                comboPensWidth[p].Top = top - 2;
                comboPensWidth[p].Width = 100;
                comboPensWidth[p].Text = ((int)Root.PenAttr[p].Width).ToString();
                comboPensWidth[p].TextChanged += comboPensWidth_TextChanged;

                tabPage2.Controls.Add(lbPens[p]);
                tabPage2.Controls.Add(cbPens[p]);
                tabPage2.Controls.Add(pboxPens[p]);
                tabPage2.Controls.Add(comboPensAlpha[p]);
                tabPage2.Controls.Add(comboPensWidth[p]);
            }

            cbAllowHotkeyInPointer.Top = (int)(this.Height * 0.18);

            for (int p = 0; p < Root.MaxPenCount; p++)
            {
                int top = p * (int)(this.Height * 0.055) + (int)(this.Height * 0.24);
                lbHotkeyPens[p] = new Label();
                lbHotkeyPens[p].Left = 20;
                lbHotkeyPens[p].Width = 80;
                lbHotkeyPens[p].Top = top;

                hiPens[p] = new HotkeyInputBox();
                hiPens[p].Hotkey = Root.Hotkey_Pens[p];
                hiPens[p].Left = 100;
                hiPens[p].Width = 120;
                hiPens[p].Top = top;
                hiPens[p].OnHotkeyChanged += hi_OnHotkeyChanged;

                tabPage3.Controls.Add(lbHotkeyPens[p]);
                tabPage3.Controls.Add(hiPens[p]);
            }

            hiGlobal.Hotkey = Root.Hotkey_Global;
            hiEraser.Hotkey = Root.Hotkey_Eraser;
            hiPan.Hotkey = Root.Hotkey_Pan;
            hiInkVisible.Hotkey = Root.Hotkey_InkVisible;
            hiPointer.Hotkey = Root.Hotkey_Pointer;
            hiSnapshot.Hotkey = Root.Hotkey_Snap;
            hiUndo.Hotkey = Root.Hotkey_Undo;
            hiRedo.Hotkey = Root.Hotkey_Redo;
            hiClear.Hotkey = Root.Hotkey_Clear;

            hiDraw.Hotkey = Root.Hotkey_Draw;
            hiLine.Hotkey = Root.Hotkey_Line;
            hiArrow.Hotkey = Root.Hotkey_Arrow;
            hiRect.Hotkey = Root.Hotkey_Rect;
            hiEllipse.Hotkey = Root.Hotkey_Ellipse;
            hiText.Hotkey = Root.Hotkey_Text;

            FormOptions_LocalReload();
        }

        private void FormOptions_LocalReload()
        {
            this.Text = Root.Local.MenuEntryOptions + " - gInk";
            tabControl1.TabPages[0].Text = Root.Local.OptionsTabGeneral;
            tabControl1.TabPages[1].Text = Root.Local.OptionsTabPens;
            tabControl1.TabPages[2].Text = Root.Local.OptionsTabHotkeys;
            this.lbLanguage.Text = Root.Local.OptionsGeneralLanguage;
            this.lbCanvascursor.Text = Root.Local.OptionsGeneralCanvascursor;
            this.lbCursorsize.Text = Root.Local.OptionsGeneralCursorsize;
            this.lbFont.Text = Root.Local.OptionsGeneralFont;
            this.lbFontSize.Text = Root.Local.OptionsGeneralFontSize;
            this.lbSnapshotsavepath.Text = Root.Local.OptionsGeneralSnapshotsavepath;
            this.cbWhiteIcon.Text = Root.Local.OptionsGeneralWhitetrayicon;
            this.cbAllowDragging.Text = Root.Local.OptionsGeneralAllowdragging;
            this.lbNote.Text = Root.Local.OptionsGeneralNotePenwidth;

            this.lbArrowLength.Text = Root.Local.OptionsGeneralArrowLength;
            this.cbFixedArrow.Text = Root.Local.OptionsGeneralAllowDynamicArrowLength;
            this.cbFontFit.Text = Root.Local.OptionsGeneralFixedFont;
            
            this.lbHkClear.Text = Root.Local.ButtonNameClear;
            this.lbHkEraser.Text = Root.Local.ButtonNameErasor;
            this.lbHkInkVisible.Text = Root.Local.ButtonNameInkVisible;
            this.lbHkPan.Text = Root.Local.ButtonNamePan;
            this.lbHkPointer.Text = Root.Local.ButtonNameMousePointer;
            this.lbHkRedo.Text = Root.Local.ButtonNameRedo;
            this.lbHkSnapshot.Text = Root.Local.ButtonNameSnapshot;
            this.lbHkUndo.Text = Root.Local.ButtonNameUndo;
            this.lbGlobalHotkey.Text = Root.Local.OptionsHotkeysglobal;
            this.cbAllowHotkeyInPointer.Text = Root.Local.OptionsHotkeysEnableinpointer;

            this.lbHkDraw.Text = Root.Local.ButtonNameDraw;
            this.lbHkLine.Text = Root.Local.ButtonNameLine;
            this.lbHkArrow.Text = Root.Local.ButtonNameArrow;
            this.lbHkRect.Text = Root.Local.ButtonNameRect;
            this.lbHkEllipse.Text = Root.Local.ButtonNameEllipse;
            this.lbHkText.Text = Root.Local.ButtonNameText;

            this.comboCanvasCursor.Items[0] = Root.Local.OptionsGeneralCanvascursorArrow;
            this.comboCanvasCursor.Items[1] = Root.Local.OptionsGeneralCanvascursorPentip;


            for (int p = 0; p < Root.MaxPenCount; p++)
            {
                comboPensAlpha[p].Items.Clear();
                comboPensWidth[p].Items.Clear();
                comboPensAlpha[p].Items.AddRange(new object[] { Root.Local.OptionsPensPencil, Root.Local.OptionsPensHighlighter });
                comboPensWidth[p].Items.AddRange(new object[] { Root.Local.OptionsPensThin, Root.Local.OptionsPensNormal, Root.Local.OptionsPensThick });

                lbPens[p].Text = Root.Local.ButtonNamePen[p];
                lbHotkeyPens[p].Text = Root.Local.ButtonNamePen[p];

                lbcbPens.Text = Root.Local.OptionsPensShow;
                lbpboxPens.Text = Root.Local.OptionsPensColor;
                lbcomboPensAlpha.Text = Root.Local.OptionsPensAlpha;
                lbcomboPensWidth.Text = Root.Local.OptionsPensWidth;
            }

            comboLanguage.Items.Clear();
            List<string> langs = Root.Local.GetLanguagenames();
            foreach (string languagename in langs)
            {
                comboLanguage.Items.Add(languagename);
            }

            string ln = Root.Local.GetLanguagenameByFilename(Root.Local.CurrentLanguageFile);
            if (comboLanguage.Items.Contains(ln))
                comboLanguage.SelectedIndex = comboLanguage.Items.IndexOf(ln);

            if (Root.CanvasCursor == 1)
                tbCursorsize.Enabled = false;
            else
                tbCursorsize.Enabled = true;

            if (Root.FixedArrowLength)
                numArrowSize.Enabled = true;
            else
                numArrowSize.Enabled = false;

            if (Root.FitFontToRect)
                numFontSize.Enabled = false;
            else
                numFontSize.Enabled = true;

        }

        private void comboPensAlpha_TextChanged(object sender, EventArgs e)
        {
            for (int p = 0; p < Root.MaxPenCount; p++)
                if ((ComboBox)sender == comboPensAlpha[p])
                {
                    byte o;
                    if (byte.TryParse(comboPensAlpha[p].Text, out o) && o >= 0 && o <= 255)
                    {
                        Root.PenAttr[p].Transparency = (byte)(255 - o);
                        comboPensAlpha[p].BackColor = Color.White;
                    }
                    else
                    {
                        comboPensAlpha[p].BackColor = Color.IndianRed;
                    }
                }
        }

        private void comboPensWidth_TextChanged(object sender, EventArgs e)
        {
            for (int p = 0; p < Root.MaxPenCount; p++)
                if ((ComboBox)sender == comboPensWidth[p])
                {
                    int o;
                    if (int.TryParse(comboPensWidth[p].Text, out o) && o >= 30 && o <= 3000)
                    {
                        Root.PenAttr[p].Width = o;
                        comboPensWidth[p].BackColor = Color.White;
                    }
                    else
                    {
                        comboPensWidth[p].BackColor = Color.IndianRed;
                    }
                }
        }

        private void pboxPens_Click(object sender, EventArgs e)
        {
            for (int p = 0; p < Root.MaxPenCount; p++)
                if ((PictureBox)sender == pboxPens[p])
                {
                    colorDialog1.Color = Root.PenAttr[p].Color;
                    if (colorDialog1.ShowDialog() == DialogResult.OK)
                    {
                        Root.PenAttr[p].Color = colorDialog1.Color;
                        pboxPens[p].BackColor = colorDialog1.Color;
                    }
                }
        }

        private void cbPens_CheckedChanged(object sender, EventArgs e)
        {
            for (int p = 0; p < Root.MaxPenCount; p++)
                if ((CheckBox)sender == cbPens[p])
                    Root.PenEnabled[p] = cbPens[p].Checked;
        }

        private void FormOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            Root.SetHotkey();

            Root.SaveOptions("pens.ini");
            Root.SaveOptions("config.ini");
            Root.SaveOptions("hotkeys.ini");

            Root.FormOptions = null;
        }

        private void cbWidthEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.PenWidthEnabled = cbWidthEnabled.Checked;
            lbNote.ForeColor = Color.Red;
        }

        private void cbEraserEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.EraserEnabled = cbEraserEnabled.Checked;
        }

        private void cbPointerEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.PointerEnabled = cbPointerEnabled.Checked;
        }

        private void cbSnapEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.SnapEnabled = cbSnapEnabled.Checked;
        }

        private void cbUndoEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.UndoEnabled = cbUndoEnabled.Checked;
        }

        private void cbClearEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.ClearEnabled = cbClearEnabled.Checked;
        }

        private void cbPanEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.PanEnabled = cbPanEnabled.Checked;
        }

        private void cbInkVisibleEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.InkVisibleEnabled = cbInkVisibleEnabled.Checked;
        }

        private void cbWhiteIcon_CheckedChanged(object sender, EventArgs e)
        {
            Root.WhiteTrayIcon = cbWhiteIcon.Checked;
            Root.SetTrayIconColor();
            SetIcon();
        }

        private void btSnapPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.SelectedPath = Root.SnapshotBasePath;

            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderBrowserDialog1.SelectedPath))
            {
                tbSnapPath.Text = folderBrowserDialog1.SelectedPath;
                Root.SnapshotBasePath = folderBrowserDialog1.SelectedPath;
            }
        }

        private void tbSnapPath_ModifiedChanged(object sender, EventArgs e)
        {
            Root.SnapshotBasePath = tbSnapPath.Text;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int p = 0; p < Root.MaxPenCount; p++)
            {
                if (comboPensWidth[p].Text == Root.Local.OptionsPensThin)
                {
                    comboPensWidth[p].Text = "30";
                }
                else if (comboPensWidth[p].Text == Root.Local.OptionsPensNormal)
                {
                    comboPensWidth[p].Text = "80";
                }
                else if (comboPensWidth[p].Text == Root.Local.OptionsPensThick)
                {
                    comboPensWidth[p].Text = "500";
                }

                if (comboPensAlpha[p].Text == Root.Local.OptionsPensPencil)
                {
                    comboPensAlpha[p].Text = "255";
                }
                else if (comboPensAlpha[p].Text == Root.Local.OptionsPensHighlighter)
                {
                    comboPensAlpha[p].Text = "80";
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Root.CanvasCursor = comboCanvasCursor.SelectedIndex;
            if (Root.CanvasCursor == 1)
                tbCursorsize.Enabled = false;
            else
                tbCursorsize.Enabled = true;
        }

        private void cbAllowDragging_CheckedChanged(object sender, EventArgs e)
        {
            Root.AllowDraggingToolbar = cbAllowDragging.Checked;
        }

        private void tbCursorsize_ValueChanged(object sender, EventArgs e)
        {
            Root.CursorSize = tbCursorsize.Value;
        }

        private void comboFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            Root.CurrentFontIndex = comboFont.SelectedIndex;

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Root.FontSize = (int)numFontSize.Value;
        }

        private void cb_TextEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.TextEnabled = cb_TextEnabled.Checked;
        }

        private void cb_EllipseEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.EllipseEnabled = cb_EllipseEnabled.Checked;
        }

        private void cb_RectEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.RectEnabled = cb_RectEnabled.Checked;
        }

        private void cb_ArrowEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.ArrowEnabled = cb_ArrowEnabled.Checked;
        }

        private void cb_LineEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.LineEnabled = cb_LineEnabled.Checked;
        }

        private void cb_HandDrawnEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Root.HandDrawnEnabled = cb_HandDrawnEnabled.Checked;
        }

        private void cbFontFit_CheckedChanged(object sender, EventArgs e)
        {
            Root.FitFontToRect = cbFontFit.Checked;

            if (Root.FitFontToRect)
                numFontSize.Enabled = false;
            else
                numFontSize.Enabled = true;

        }

        private void cbFixedArrow_CheckedChanged(object sender, EventArgs e)
        {
            Root.FixedArrowLength = cbFixedArrow.Checked;

            if (Root.FixedArrowLength)
                numArrowSize.Enabled = true;
            else
                numArrowSize.Enabled = false;
        }

        private void numArrowSize_ValueChanged(object sender, EventArgs e)
        {
            Root.ArrowLength = (int)numArrowSize.Value;
        }

        private void cbAllowHotkeyInPointer_CheckedChanged(object sender, EventArgs e)
        {
            Root.AllowHotkeyInPointerMode = cbAllowHotkeyInPointer.Checked;
        }

        private void hi_OnHotkeyChanged(object sender, EventArgs e)
        {
            foreach (Control c in tabPage3.Controls)
            {
                if (c.GetType() != typeof(HotkeyInputBox))
                    continue;
                HotkeyInputBox hi = (HotkeyInputBox)c;

                hi.ExternalConflictFlag = false;
                foreach (Control c2 in tabPage3.Controls)
                {
                    if (c2.GetType() != typeof(HotkeyInputBox))
                        continue;
                    if (c == c2)
                        continue;
                    HotkeyInputBox hi2 = (HotkeyInputBox)c2;

                    if (hi.Hotkey.ConflictWith(hi2.Hotkey))
                    {
                        hi.ExternalConflictFlag = true;
                        break;
                    }
                }
            }
        }

        private void comboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboLanguage.Text != Root.Local.GetLanguagenameByFilename(Root.Local.CurrentLanguageFile))
            {
                Root.ChangeLanguage(Root.Local.GetFilenameByLanguagename(comboLanguage.Text));
                FormOptions_LocalReload();
            }
        }
    }
}
