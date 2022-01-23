namespace gInk
{
	partial class FormCollection
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.gpButtons = new System.Windows.Forms.Panel();
            this.btText = new System.Windows.Forms.Button();
            this.btArrow = new System.Windows.Forms.Button();
            this.btEllipse = new System.Windows.Forms.Button();
            this.btDraw = new System.Windows.Forms.Button();
            this.btRect = new System.Windows.Forms.Button();
            this.btLine = new System.Windows.Forms.Button();
            this.btInkVisible = new System.Windows.Forms.Button();
            this.btPan = new System.Windows.Forms.Button();
            this.btDock = new System.Windows.Forms.Button();
            this.btPenWidth = new System.Windows.Forms.Button();
            this.btEraser = new System.Windows.Forms.Button();
            this.btSnap = new System.Windows.Forms.Button();
            this.btPointer = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.btClear = new System.Windows.Forms.Button();
            this.btUndo = new System.Windows.Forms.Button();
            this.tiSlide = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.gpPenWidth = new System.Windows.Forms.Panel();
            this.pboxPenWidthIndicator = new System.Windows.Forms.PictureBox();
            this.textInputPanel = new System.Windows.Forms.Panel();
            this.numFontSizeDynamic = new System.Windows.Forms.NumericUpDown();
            this.textInput = new System.Windows.Forms.TextBox();
            this.gpButtons.SuspendLayout();
            this.gpPenWidth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pboxPenWidthIndicator)).BeginInit();
            this.textInputPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFontSizeDynamic)).BeginInit();
            this.SuspendLayout();
            // 
            // gpButtons
            // 
            this.gpButtons.BackColor = System.Drawing.Color.WhiteSmoke;
            this.gpButtons.Controls.Add(this.btText);
            this.gpButtons.Controls.Add(this.btArrow);
            this.gpButtons.Controls.Add(this.btEllipse);
            this.gpButtons.Controls.Add(this.btDraw);
            this.gpButtons.Controls.Add(this.btRect);
            this.gpButtons.Controls.Add(this.btLine);
            this.gpButtons.Controls.Add(this.btInkVisible);
            this.gpButtons.Controls.Add(this.btPan);
            this.gpButtons.Controls.Add(this.btDock);
            this.gpButtons.Controls.Add(this.btPenWidth);
            this.gpButtons.Controls.Add(this.btEraser);
            this.gpButtons.Controls.Add(this.btSnap);
            this.gpButtons.Controls.Add(this.btPointer);
            this.gpButtons.Controls.Add(this.btStop);
            this.gpButtons.Controls.Add(this.btClear);
            this.gpButtons.Controls.Add(this.btUndo);
            this.gpButtons.Location = new System.Drawing.Point(43, 23);
            this.gpButtons.Margin = new System.Windows.Forms.Padding(2);
            this.gpButtons.Name = "gpButtons";
            this.gpButtons.Size = new System.Drawing.Size(1130, 107);
            this.gpButtons.TabIndex = 3;
            this.gpButtons.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseDown);
            this.gpButtons.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseMove);
            this.gpButtons.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseUp);
            // 
            // btText
            // 
            this.btText.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btText.FlatAppearance.BorderSize = 0;
            this.btText.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btText.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btText.ForeColor = System.Drawing.Color.Transparent;
            this.btText.Image = global::gInk.Properties.Resources.text;
            this.btText.Location = new System.Drawing.Point(32, 2);
            this.btText.Margin = new System.Windows.Forms.Padding(2);
            this.btText.Name = "btText";
            this.btText.Size = new System.Drawing.Size(57, 57);
            this.btText.TabIndex = 9;
            this.toolTip.SetToolTip(this.btText, "Pen width");
            this.btText.UseVisualStyleBackColor = true;
            this.btText.Click += new System.EventHandler(this.btText_Click);
            // 
            // btArrow
            // 
            this.btArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btArrow.FlatAppearance.BorderSize = 0;
            this.btArrow.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btArrow.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btArrow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btArrow.ForeColor = System.Drawing.Color.Transparent;
            this.btArrow.Image = global::gInk.Properties.Resources.arrow;
            this.btArrow.Location = new System.Drawing.Point(93, 3);
            this.btArrow.Margin = new System.Windows.Forms.Padding(2);
            this.btArrow.Name = "btArrow";
            this.btArrow.Size = new System.Drawing.Size(57, 57);
            this.btArrow.TabIndex = 8;
            this.toolTip.SetToolTip(this.btArrow, "Pen width");
            this.btArrow.UseVisualStyleBackColor = true;
            this.btArrow.Click += new System.EventHandler(this.btArrow_Click);
            // 
            // btEllipse
            // 
            this.btEllipse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btEllipse.FlatAppearance.BorderSize = 0;
            this.btEllipse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btEllipse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btEllipse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btEllipse.ForeColor = System.Drawing.Color.Transparent;
            this.btEllipse.Image = global::gInk.Properties.Resources.ellipse;
            this.btEllipse.Location = new System.Drawing.Point(154, 3);
            this.btEllipse.Margin = new System.Windows.Forms.Padding(2);
            this.btEllipse.Name = "btEllipse";
            this.btEllipse.Size = new System.Drawing.Size(57, 57);
            this.btEllipse.TabIndex = 7;
            this.toolTip.SetToolTip(this.btEllipse, "Pen width");
            this.btEllipse.UseVisualStyleBackColor = true;
            this.btEllipse.Click += new System.EventHandler(this.btEllipse_Click);
            // 
            // btDraw
            // 
            this.btDraw.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btDraw.FlatAppearance.BorderSize = 0;
            this.btDraw.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btDraw.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btDraw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btDraw.ForeColor = System.Drawing.Color.Transparent;
            this.btDraw.Image = global::gInk.Properties.Resources.normal_draw;
            this.btDraw.Location = new System.Drawing.Point(223, 3);
            this.btDraw.Margin = new System.Windows.Forms.Padding(2);
            this.btDraw.Name = "btDraw";
            this.btDraw.Size = new System.Drawing.Size(57, 57);
            this.btDraw.TabIndex = 6;
            this.toolTip.SetToolTip(this.btDraw, "Pen width");
            this.btDraw.UseVisualStyleBackColor = true;
            this.btDraw.Click += new System.EventHandler(this.btDraw_Click);
            // 
            // btRect
            // 
            this.btRect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btRect.FlatAppearance.BorderSize = 0;
            this.btRect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btRect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btRect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btRect.ForeColor = System.Drawing.Color.Transparent;
            this.btRect.Image = global::gInk.Properties.Resources.rect;
            this.btRect.Location = new System.Drawing.Point(297, 3);
            this.btRect.Margin = new System.Windows.Forms.Padding(2);
            this.btRect.Name = "btRect";
            this.btRect.Size = new System.Drawing.Size(57, 57);
            this.btRect.TabIndex = 5;
            this.toolTip.SetToolTip(this.btRect, "Pen width");
            this.btRect.UseVisualStyleBackColor = true;
            this.btRect.Click += new System.EventHandler(this.btRect_Click);
            // 
            // btLine
            // 
            this.btLine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btLine.FlatAppearance.BorderSize = 0;
            this.btLine.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btLine.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btLine.ForeColor = System.Drawing.Color.Transparent;
            this.btLine.Image = global::gInk.Properties.Resources.lines;
            this.btLine.Location = new System.Drawing.Point(368, 3);
            this.btLine.Margin = new System.Windows.Forms.Padding(2);
            this.btLine.Name = "btLine";
            this.btLine.Size = new System.Drawing.Size(57, 57);
            this.btLine.TabIndex = 4;
            this.toolTip.SetToolTip(this.btLine, "Pen width");
            this.btLine.UseVisualStyleBackColor = true;
            this.btLine.Click += new System.EventHandler(this.btLine_Click);
            // 
            // btInkVisible
            // 
            this.btInkVisible.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btInkVisible.FlatAppearance.BorderSize = 0;
            this.btInkVisible.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btInkVisible.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btInkVisible.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btInkVisible.Image = global::gInk.Properties.Resources.visible;
            this.btInkVisible.Location = new System.Drawing.Point(985, 3);
            this.btInkVisible.Margin = new System.Windows.Forms.Padding(2);
            this.btInkVisible.Name = "btInkVisible";
            this.btInkVisible.Size = new System.Drawing.Size(57, 57);
            this.btInkVisible.TabIndex = 3;
            this.toolTip.SetToolTip(this.btInkVisible, "Ink visible");
            this.btInkVisible.UseVisualStyleBackColor = true;
            this.btInkVisible.Click += new System.EventHandler(this.btInkVisible_Click);
            this.btInkVisible.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseDown);
            this.btInkVisible.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseMove);
            this.btInkVisible.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseUp);
            // 
            // btPan
            // 
            this.btPan.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btPan.FlatAppearance.BorderSize = 0;
            this.btPan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btPan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btPan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btPan.Image = global::gInk.Properties.Resources.pan;
            this.btPan.Location = new System.Drawing.Point(923, 3);
            this.btPan.Margin = new System.Windows.Forms.Padding(2);
            this.btPan.Name = "btPan";
            this.btPan.Size = new System.Drawing.Size(57, 57);
            this.btPan.TabIndex = 2;
            this.toolTip.SetToolTip(this.btPan, "Pan");
            this.btPan.UseVisualStyleBackColor = true;
            this.btPan.Click += new System.EventHandler(this.btPan_Click);
            this.btPan.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseDown);
            this.btPan.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseMove);
            this.btPan.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseUp);
            // 
            // btDock
            // 
            this.btDock.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btDock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btDock.FlatAppearance.BorderSize = 0;
            this.btDock.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btDock.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btDock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btDock.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btDock.Image = global::gInk.Properties.Resources.dock;
            this.btDock.Location = new System.Drawing.Point(0, 3);
            this.btDock.Margin = new System.Windows.Forms.Padding(2);
            this.btDock.Name = "btDock";
            this.btDock.Size = new System.Drawing.Size(42, 57);
            this.btDock.TabIndex = 0;
            this.toolTip.SetToolTip(this.btDock, "Dock / Undock");
            this.btDock.UseVisualStyleBackColor = false;
            this.btDock.Click += new System.EventHandler(this.btDock_Click);
            this.btDock.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseDown);
            this.btDock.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseMove);
            this.btDock.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseUp);
            // 
            // btPenWidth
            // 
            this.btPenWidth.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btPenWidth.FlatAppearance.BorderSize = 0;
            this.btPenWidth.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btPenWidth.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btPenWidth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btPenWidth.Image = global::gInk.Properties.Resources.penwidth;
            this.btPenWidth.Location = new System.Drawing.Point(440, 3);
            this.btPenWidth.Margin = new System.Windows.Forms.Padding(2);
            this.btPenWidth.Name = "btPenWidth";
            this.btPenWidth.Size = new System.Drawing.Size(57, 57);
            this.btPenWidth.TabIndex = 0;
            this.toolTip.SetToolTip(this.btPenWidth, "Pen width");
            this.btPenWidth.UseVisualStyleBackColor = true;
            this.btPenWidth.Click += new System.EventHandler(this.btPenWidth_Click);
            this.btPenWidth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseDown);
            this.btPenWidth.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseMove);
            this.btPenWidth.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseUp);
            // 
            // btEraser
            // 
            this.btEraser.FlatAppearance.BorderSize = 0;
            this.btEraser.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btEraser.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btEraser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btEraser.ForeColor = System.Drawing.Color.Transparent;
            this.btEraser.Image = global::gInk.Properties.Resources.eraser;
            this.btEraser.Location = new System.Drawing.Point(516, 3);
            this.btEraser.Margin = new System.Windows.Forms.Padding(2);
            this.btEraser.Name = "btEraser";
            this.btEraser.Size = new System.Drawing.Size(57, 57);
            this.btEraser.TabIndex = 0;
            this.toolTip.SetToolTip(this.btEraser, "Eraser");
            this.btEraser.UseVisualStyleBackColor = false;
            this.btEraser.Click += new System.EventHandler(this.btEraser_Click);
            this.btEraser.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseDown);
            this.btEraser.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseMove);
            this.btEraser.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseUp);
            // 
            // btSnap
            // 
            this.btSnap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btSnap.FlatAppearance.BorderSize = 0;
            this.btSnap.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btSnap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btSnap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSnap.Image = global::gInk.Properties.Resources.snap;
            this.btSnap.Location = new System.Drawing.Point(695, 3);
            this.btSnap.Margin = new System.Windows.Forms.Padding(2);
            this.btSnap.Name = "btSnap";
            this.btSnap.Size = new System.Drawing.Size(57, 57);
            this.btSnap.TabIndex = 0;
            this.toolTip.SetToolTip(this.btSnap, "Snapshot");
            this.btSnap.UseVisualStyleBackColor = true;
            this.btSnap.Click += new System.EventHandler(this.btSnap_Click);
            this.btSnap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseDown);
            this.btSnap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseMove);
            this.btSnap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseUp);
            // 
            // btPointer
            // 
            this.btPointer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btPointer.FlatAppearance.BorderSize = 0;
            this.btPointer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btPointer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btPointer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btPointer.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btPointer.Image = global::gInk.Properties.Resources.pointer;
            this.btPointer.Location = new System.Drawing.Point(586, 3);
            this.btPointer.Margin = new System.Windows.Forms.Padding(2);
            this.btPointer.Name = "btPointer";
            this.btPointer.Size = new System.Drawing.Size(57, 57);
            this.btPointer.TabIndex = 0;
            this.toolTip.SetToolTip(this.btPointer, "Mouse pointer");
            this.btPointer.UseVisualStyleBackColor = true;
            this.btPointer.Click += new System.EventHandler(this.btPointer_Click);
            this.btPointer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseDown);
            this.btPointer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseMove);
            this.btPointer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseUp);
            // 
            // btStop
            // 
            this.btStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btStop.FlatAppearance.BorderSize = 0;
            this.btStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btStop.Image = global::gInk.Properties.Resources.exit;
            this.btStop.Location = new System.Drawing.Point(1058, 3);
            this.btStop.Margin = new System.Windows.Forms.Padding(2);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(57, 57);
            this.btStop.TabIndex = 0;
            this.toolTip.SetToolTip(this.btStop, "Exit drawing");
            this.btStop.UseVisualStyleBackColor = true;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            this.btStop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseDown);
            this.btStop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseMove);
            this.btStop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseUp);
            // 
            // btClear
            // 
            this.btClear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btClear.FlatAppearance.BorderSize = 0;
            this.btClear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btClear.Image = global::gInk.Properties.Resources.garbage;
            this.btClear.Location = new System.Drawing.Point(836, 3);
            this.btClear.Margin = new System.Windows.Forms.Padding(2);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(57, 57);
            this.btClear.TabIndex = 1;
            this.toolTip.SetToolTip(this.btClear, "Clear");
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            this.btClear.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseDown);
            this.btClear.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseMove);
            this.btClear.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseUp);
            // 
            // btUndo
            // 
            this.btUndo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btUndo.FlatAppearance.BorderSize = 0;
            this.btUndo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.WhiteSmoke;
            this.btUndo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.btUndo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btUndo.Image = global::gInk.Properties.Resources.undo;
            this.btUndo.Location = new System.Drawing.Point(765, 3);
            this.btUndo.Margin = new System.Windows.Forms.Padding(2);
            this.btUndo.Name = "btUndo";
            this.btUndo.Size = new System.Drawing.Size(57, 57);
            this.btUndo.TabIndex = 1;
            this.toolTip.SetToolTip(this.btUndo, "Undo");
            this.btUndo.UseVisualStyleBackColor = true;
            this.btUndo.Click += new System.EventHandler(this.btUndo_Click);
            this.btUndo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseDown);
            this.btUndo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseMove);
            this.btUndo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gpButtons_MouseUp);
            // 
            // tiSlide
            // 
            this.tiSlide.Interval = 15;
            this.tiSlide.Tick += new System.EventHandler(this.tiSlide_Tick);
            // 
            // gpPenWidth
            // 
            this.gpPenWidth.BackColor = System.Drawing.Color.WhiteSmoke;
            this.gpPenWidth.BackgroundImage = global::gInk.Properties.Resources.penwidthpanel;
            this.gpPenWidth.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.gpPenWidth.Controls.Add(this.pboxPenWidthIndicator);
            this.gpPenWidth.Location = new System.Drawing.Point(145, 272);
            this.gpPenWidth.Name = "gpPenWidth";
            this.gpPenWidth.Size = new System.Drawing.Size(250, 67);
            this.gpPenWidth.TabIndex = 4;
            this.gpPenWidth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gpPenWidth_MouseDown);
            this.gpPenWidth.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gpPenWidth_MouseMove);
            this.gpPenWidth.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gpPenWidth_MouseUp);
            // 
            // pboxPenWidthIndicator
            // 
            this.pboxPenWidthIndicator.BackColor = System.Drawing.Color.Orange;
            this.pboxPenWidthIndicator.Location = new System.Drawing.Point(97, 0);
            this.pboxPenWidthIndicator.Name = "pboxPenWidthIndicator";
            this.pboxPenWidthIndicator.Size = new System.Drawing.Size(7, 67);
            this.pboxPenWidthIndicator.TabIndex = 5;
            this.pboxPenWidthIndicator.TabStop = false;
            this.pboxPenWidthIndicator.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pboxPenWidthIndicator_MouseDown);
            this.pboxPenWidthIndicator.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pboxPenWidthIndicator_MouseMove);
            this.pboxPenWidthIndicator.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pboxPenWidthIndicator_MouseUp);
            // 
            // textInputPanel
            // 
            this.textInputPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.textInputPanel.Controls.Add(this.numFontSizeDynamic);
            this.textInputPanel.Controls.Add(this.textInput);
            this.textInputPanel.Location = new System.Drawing.Point(559, 186);
            this.textInputPanel.Name = "textInputPanel";
            this.textInputPanel.Size = new System.Drawing.Size(400, 120);
            this.textInputPanel.TabIndex = 5;
            // 
            // numFontSizeDynamic
            // 
            this.numFontSizeDynamic.Location = new System.Drawing.Point(329, 4);
            this.numFontSizeDynamic.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numFontSizeDynamic.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numFontSizeDynamic.Name = "numFontSizeDynamic";
            this.numFontSizeDynamic.Size = new System.Drawing.Size(68, 22);
            this.numFontSizeDynamic.TabIndex = 1;
            this.numFontSizeDynamic.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numFontSizeDynamic.ValueChanged += new System.EventHandler(this.numFontSizeDynamic_ValueChanged);
            // 
            // textInput
            // 
            this.textInput.AcceptsTab = true;
            this.textInput.Location = new System.Drawing.Point(3, 3);
            this.textInput.Multiline = true;
            this.textInput.Name = "textInput";
            this.textInput.Size = new System.Drawing.Size(320, 114);
            this.textInput.TabIndex = 0;
            this.textInput.TextChanged += new System.EventHandler(this.textInput_TextChanged);
            // 
            // FormCollection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1272, 657);
            this.Controls.Add(this.textInputPanel);
            this.Controls.Add(this.gpPenWidth);
            this.Controls.Add(this.gpButtons);
            this.ForeColor = System.Drawing.Color.LawnGreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCollection";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCollection_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gpButtons.ResumeLayout(false);
            this.gpPenWidth.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pboxPenWidthIndicator)).EndInit();
            this.textInputPanel.ResumeLayout(false);
            this.textInputPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFontSizeDynamic)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.Button btStop;
		public System.Windows.Forms.Button btClear;
		public System.Windows.Forms.Button btUndo;
		public System.Windows.Forms.Panel gpButtons;
		public System.Windows.Forms.Button btEraser;
		private System.Windows.Forms.Timer tiSlide;
		public System.Windows.Forms.Button btDock;
		public System.Windows.Forms.Button btSnap;
		public System.Windows.Forms.Button btPointer;
		public System.Windows.Forms.Button btPenWidth;
		public System.Windows.Forms.ToolTip toolTip;
		public System.Windows.Forms.Panel gpPenWidth;
		private System.Windows.Forms.PictureBox pboxPenWidthIndicator;
		public System.Windows.Forms.Button btPan;
		public System.Windows.Forms.Button btInkVisible;
        public System.Windows.Forms.Button btRect;
        public System.Windows.Forms.Button btLine;
        public System.Windows.Forms.Button btDraw;
        public System.Windows.Forms.Button btEllipse;
        public System.Windows.Forms.Button btArrow;
        public System.Windows.Forms.Button btText;
        public System.Windows.Forms.Panel textInputPanel;
        public System.Windows.Forms.TextBox textInput;
        private System.Windows.Forms.NumericUpDown numFontSizeDynamic;
    }
}

