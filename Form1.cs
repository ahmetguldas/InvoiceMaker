using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace InvoiceMaker
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	/// 
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TextBox AddressTextBox;
		private System.Windows.Forms.TextBox BillToTextBox;
		private System.Windows.Forms.TextBox ShipToTextBox;
		private System.Windows.Forms.DateTimePicker TodayPicker;
		private System.Windows.Forms.TextBox InvoiceNumberTextBox;
		private System.Windows.Forms.DateTimePicker DueDatePicker;
		private System.Windows.Forms.TextBox PONumberTextBox;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader Item;
		private System.Windows.Forms.ColumnHeader Description;
		private System.Windows.Forms.ColumnHeader Quantity;
		private System.Windows.Forms.ColumnHeader Rate;
		private System.Windows.Forms.ColumnHeader Amount;
		private System.Windows.Forms.TextBox PercentTextBox;
		private System.Windows.Forms.TextBox TotalTextBox;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.Label label1;
        private IContainer components;
        public int CurrentRow = 0;
		private System.Windows.Forms.TextBox SubtotalTextBox;
		private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem PreviewMenu;
		private System.Windows.Forms.MenuItem PrintMenu;
		private System.Drawing.Printing.PrintDocument printDocument1;
		private System.Windows.Forms.Button PreviewButton;
		private System.Windows.Forms.Button PrintButton;
		private System.Windows.Forms.PrintDialog printDialog1;
		private System.Windows.Forms.Button LogoButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Button SaveButton;
		private System.Windows.Forms.Button OpenButton;
		private TextBox[] RowItems;
		private InvoiceData TheInvoiceData = new InvoiceData();
		private System.Windows.Forms.OpenFileDialog openFileDialog2;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private string ThePictureFileName = "";
		private System.Windows.Forms.Button CalculateButton;
		private Image LargerImage = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			RowItems = new TextBox[]{textBox1, textBox2, textBox3, textBox4, textBox5};
			InitializeRowEditing(CurrentRow);
//			LargerImage = Image.FromFile("InvoiceTemplate.jpg");
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
		
		/// <summary>
		/// Line up all the edit boxes along the correct listview vertical borders
		/// </summary>
		void InitializeRowEditing(int rowNum)
		{
			int sum = 0;
			for (int i = 0; i < listView1.Columns.Count; i++)
			{
				int theWidth = listView1.Columns[i].Width;
				int theHeight = listView1.Items[0].Bounds.Height;
				RowItems[i].SetBounds(sum + listView1.Bounds.X, listView1.Bounds.Y + theHeight*rowNum,
										theWidth, theHeight);		
				sum += theWidth;
			}
		}

		/// <summary>
		/// Dumps the row of edit boxes into the list view they are 
		/// Position at
		/// </summary>
		/// <param name="nRow"></param>
		void DumpRowToListView(int nRow)
		{
			for (int i = 0; i < listView1.Columns.Count; i++)
			{
				if (listView1.Items[nRow].SubItems.Count - 1  < i)
				{
					listView1.Items[nRow].SubItems.Add(RowItems[i].Text);
				}
				else
				{
					listView1.Items[nRow].SubItems[i].Text = RowItems[i].Text;
				}
			}
			
		}

		/// <summary>
		/// Dumps the row of the listview into the edit boxes
		/// Position at
		/// </summary>
		/// <param name="nRow"></param>
		void DumpListViewToRow(int nRow)
		{
			for (int i = 0; i < listView1.Columns.Count; i++)
			{
				if (listView1.Items[nRow].SubItems.Count - 1  < i)
				{
					RowItems[i].Text = "";
				}
				else
				{
					RowItems[i].Text = listView1.Items[nRow].SubItems[i].Text;
				}
			}
			
		}


		void FillInvoiceDataWithForm()
		{
			TheInvoiceData.BillerAddress = this.AddressTextBox.Text;
			TheInvoiceData.BillToAddress = this.BillToTextBox.Text;
			TheInvoiceData.DueDate = this.DueDatePicker.Text;
			TheInvoiceData.InvoiceDate = this.TodayPicker.Text;
			TheInvoiceData.InvoiceNumber = this.InvoiceNumberTextBox.Text;
			TheInvoiceData.LogoFile = this.ThePictureFileName;
			TheInvoiceData.PercentTax = this.PercentTextBox.Text;
			TheInvoiceData.PONumber = this.PONumberTextBox.Text;
			TheInvoiceData.ShipToAddress = this.ShipToTextBox.Text;
			TheInvoiceData.Subtotal = this.SubtotalTextBox.Text;
			TheInvoiceData.Total = this.TotalTextBox.Text;

			TheInvoiceData.RowCount = listView1.Items.Count;
			for (int i = 0; i < listView1.Items.Count; i++)
			{
				if (listView1.Items[i].SubItems.Count > 0)
					TheInvoiceData.DataRows[i].Item  = listView1.Items[i].SubItems[0].Text;
				if (listView1.Items[i].SubItems.Count > 1)
					TheInvoiceData.DataRows[i].Description   = listView1.Items[i].SubItems[1].Text;
				if (listView1.Items[i].SubItems.Count > 2)
					TheInvoiceData.DataRows[i].Quantity   = listView1.Items[i].SubItems[2].Text;
				if (listView1.Items[i].SubItems.Count > 3)
					TheInvoiceData.DataRows[i].Rate   = listView1.Items[i].SubItems[3].Text;
				if (listView1.Items[i].SubItems.Count > 4)
					TheInvoiceData.DataRows[i].Amount  = listView1.Items[i].SubItems[4].Text;
			}
		}

		void FillFormWithInvoiceData()
		{
			this.AddressTextBox.Text = TheInvoiceData.BillerAddress;
			this.BillToTextBox.Text = TheInvoiceData.BillToAddress;
			this.DueDatePicker.Text = TheInvoiceData.DueDate;
			this.TodayPicker.Text = TheInvoiceData.InvoiceDate;
			this.InvoiceNumberTextBox.Text = TheInvoiceData.InvoiceNumber;
			this.ThePictureFileName = TheInvoiceData.LogoFile;
			this.PercentTextBox.Text = TheInvoiceData.PercentTax;
			this.PONumberTextBox.Text = TheInvoiceData.PONumber;
			this.ShipToTextBox.Text = TheInvoiceData.ShipToAddress;
			this.SubtotalTextBox.Text = TheInvoiceData.Subtotal;
			this.TotalTextBox.Text = TheInvoiceData.Total;

			for (int i = 0; i < TheInvoiceData.RowCount; i++)
			{
				listView1.Items[i].SubItems.Clear();
				listView1.Items[i].Text = TheInvoiceData.DataRows[i].Item;
				listView1.Items[i].SubItems.Add(TheInvoiceData.DataRows[i].Description);
				listView1.Items[i].SubItems.Add(TheInvoiceData.DataRows[i].Quantity);
				listView1.Items[i].SubItems.Add(TheInvoiceData.DataRows[i].Rate);
				listView1.Items[i].SubItems.Add(TheInvoiceData.DataRows[i].Amount);
			}

						// get picture if it exists
			if (TheInvoiceData.LogoFile != null && TheInvoiceData.LogoFile.Length > 0)
			{
			  pictureBox1.Image = Image.FromFile(TheInvoiceData.LogoFile);
			}


		}

		void SaveForm()
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				FillInvoiceDataWithForm();
				IFormatter formatter = new BinaryFormatter();
				Stream stream = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
				formatter.Serialize(stream, TheInvoiceData);
				stream.Close();
			}
		}


		void OpenForm()
		{
			if (openFileDialog2.ShowDialog() == DialogResult.OK)
			{
				IFormatter formatter = new BinaryFormatter();
				Stream stream = new FileStream(openFileDialog2.FileName, FileMode.Open, FileAccess.Read, FileShare.None);
				TheInvoiceData = (InvoiceData)formatter.Deserialize(stream);
				stream.Close();
				FillFormWithInvoiceData();
				InitializeRowEditing(0);					 
				DumpListViewToRow(0);
			}
		}

		


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem24 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem25 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem26 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem27 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem28 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem29 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem30 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem31 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem32 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem33 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem34 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem35 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem36 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem37 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem38 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem39 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem40 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem41 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem42 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem43 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem44 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem45 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem46 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.AddressTextBox = new System.Windows.Forms.TextBox();
            this.BillToTextBox = new System.Windows.Forms.TextBox();
            this.ShipToTextBox = new System.Windows.Forms.TextBox();
            this.TodayPicker = new System.Windows.Forms.DateTimePicker();
            this.InvoiceNumberTextBox = new System.Windows.Forms.TextBox();
            this.DueDatePicker = new System.Windows.Forms.DateTimePicker();
            this.PONumberTextBox = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Item = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Quantity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Rate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Amount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SubtotalTextBox = new System.Windows.Forms.TextBox();
            this.PercentTextBox = new System.Windows.Forms.TextBox();
            this.TotalTextBox = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.PreviewMenu = new System.Windows.Forms.MenuItem();
            this.PrintMenu = new System.Windows.Forms.MenuItem();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.PreviewButton = new System.Windows.Forms.Button();
            this.PrintButton = new System.Windows.Forms.Button();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.LogoButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SaveButton = new System.Windows.Forms.Button();
            this.OpenButton = new System.Windows.Forms.Button();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.CalculateButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(19, 37);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(120, 80);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // AddressTextBox
            // 
            this.AddressTextBox.AcceptsReturn = true;
            this.AddressTextBox.Location = new System.Drawing.Point(202, 37);
            this.AddressTextBox.Multiline = true;
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.Size = new System.Drawing.Size(182, 65);
            this.AddressTextBox.TabIndex = 1;
            // 
            // BillToTextBox
            // 
            this.BillToTextBox.AcceptsReturn = true;
            this.BillToTextBox.Location = new System.Drawing.Point(67, 175);
            this.BillToTextBox.Multiline = true;
            this.BillToTextBox.Name = "BillToTextBox";
            this.BillToTextBox.Size = new System.Drawing.Size(279, 74);
            this.BillToTextBox.TabIndex = 2;
            // 
            // ShipToTextBox
            // 
            this.ShipToTextBox.AcceptsReturn = true;
            this.ShipToTextBox.Location = new System.Drawing.Point(365, 175);
            this.ShipToTextBox.Multiline = true;
            this.ShipToTextBox.Name = "ShipToTextBox";
            this.ShipToTextBox.Size = new System.Drawing.Size(288, 74);
            this.ShipToTextBox.TabIndex = 3;
            this.ShipToTextBox.TextChanged += new System.EventHandler(this.ShipToTextBox_TextChanged);
            // 
            // TodayPicker
            // 
            this.TodayPicker.CustomFormat = "MM/dd/yy";
            this.TodayPicker.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TodayPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.TodayPicker.Location = new System.Drawing.Point(480, 102);
            this.TodayPicker.Name = "TodayPicker";
            this.TodayPicker.Size = new System.Drawing.Size(67, 23);
            this.TodayPicker.TabIndex = 4;
            this.TodayPicker.ValueChanged += new System.EventHandler(this.TodayPicker_ValueChanged);
            // 
            // InvoiceNumberTextBox
            // 
            this.InvoiceNumberTextBox.Location = new System.Drawing.Point(547, 102);
            this.InvoiceNumberTextBox.Name = "InvoiceNumberTextBox";
            this.InvoiceNumberTextBox.Size = new System.Drawing.Size(125, 22);
            this.InvoiceNumberTextBox.TabIndex = 5;
            this.InvoiceNumberTextBox.TextChanged += new System.EventHandler(this.InvoiceNumberTextBox_TextChanged);
            // 
            // DueDatePicker
            // 
            this.DueDatePicker.CustomFormat = "MM/dd/yy";
            this.DueDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DueDatePicker.Location = new System.Drawing.Point(461, 323);
            this.DueDatePicker.Name = "DueDatePicker";
            this.DueDatePicker.Size = new System.Drawing.Size(86, 22);
            this.DueDatePicker.TabIndex = 6;
            // 
            // PONumberTextBox
            // 
            this.PONumberTextBox.Location = new System.Drawing.Point(547, 323);
            this.PONumberTextBox.Name = "PONumberTextBox";
            this.PONumberTextBox.Size = new System.Drawing.Size(125, 22);
            this.PONumberTextBox.TabIndex = 7;
            // 
            // listView1
            // 
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Item,
            this.Description,
            this.Quantity,
            this.Rate,
            this.Amount});
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem24,
            listViewItem25,
            listViewItem26,
            listViewItem27,
            listViewItem28,
            listViewItem29,
            listViewItem30,
            listViewItem31,
            listViewItem32,
            listViewItem33,
            listViewItem34,
            listViewItem35,
            listViewItem36,
            listViewItem37,
            listViewItem38,
            listViewItem39,
            listViewItem40,
            listViewItem41,
            listViewItem42,
            listViewItem43,
            listViewItem44,
            listViewItem45,
            listViewItem46});
            this.listView1.Location = new System.Drawing.Point(38, 369);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(634, 379);
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // Item
            // 
            this.Item.Width = 68;
            // 
            // Description
            // 
            this.Description.Width = 245;
            // 
            // Quantity
            // 
            this.Quantity.Width = 55;
            // 
            // Rate
            // 
            this.Rate.Width = 70;
            // 
            // Amount
            // 
            this.Amount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Amount.Width = 86;
            // 
            // SubtotalTextBox
            // 
            this.SubtotalTextBox.Location = new System.Drawing.Point(566, 748);
            this.SubtotalTextBox.Name = "SubtotalTextBox";
            this.SubtotalTextBox.Size = new System.Drawing.Size(106, 22);
            this.SubtotalTextBox.TabIndex = 9;
            // 
            // PercentTextBox
            // 
            this.PercentTextBox.Location = new System.Drawing.Point(394, 766);
            this.PercentTextBox.Name = "PercentTextBox";
            this.PercentTextBox.Size = new System.Drawing.Size(28, 22);
            this.PercentTextBox.TabIndex = 10;
            this.PercentTextBox.TextChanged += new System.EventHandler(this.PercentTextBox_TextChanged);
            // 
            // TotalTextBox
            // 
            this.TotalTextBox.Location = new System.Drawing.Point(557, 794);
            this.TotalTextBox.Name = "TotalTextBox";
            this.TotalTextBox.Size = new System.Drawing.Size(115, 22);
            this.TotalTextBox.TabIndex = 12;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(38, 369);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(87, 22);
            this.textBox1.TabIndex = 13;
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Location = new System.Drawing.Point(125, 369);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(288, 22);
            this.textBox2.TabIndex = 14;
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.Location = new System.Drawing.Point(413, 369);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(67, 22);
            this.textBox3.TabIndex = 15;
            // 
            // textBox4
            // 
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox4.Location = new System.Drawing.Point(480, 369);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(86, 22);
            this.textBox4.TabIndex = 16;
            // 
            // textBox5
            // 
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox5.Location = new System.Drawing.Point(566, 369);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(106, 22);
            this.textBox5.TabIndex = 17;
            this.textBox5.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            this.textBox5.Leave += new System.EventHandler(this.textBox5_Leave);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(202, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 19);
            this.label1.TabIndex = 18;
            this.label1.Text = "Biller";
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            this.printPreviewDialog1.Load += new System.EventHandler(this.printPreviewDialog1_Load);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.PreviewMenu,
            this.PrintMenu});
            this.menuItem1.Text = "File";
            // 
            // PreviewMenu
            // 
            this.PreviewMenu.Index = 0;
            this.PreviewMenu.Text = "Print Preview";
            this.PreviewMenu.Click += new System.EventHandler(this.PreviewMenu_Click);
            // 
            // PrintMenu
            // 
            this.PrintMenu.Index = 1;
            this.PrintMenu.Text = "Print...";
            this.PrintMenu.Click += new System.EventHandler(this.PrintMenu_Click);
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // PreviewButton
            // 
            this.PreviewButton.Location = new System.Drawing.Point(182, 757);
            this.PreviewButton.Name = "PreviewButton";
            this.PreviewButton.Size = new System.Drawing.Size(68, 28);
            this.PreviewButton.TabIndex = 19;
            this.PreviewButton.Text = "Preview";
            this.PreviewButton.Click += new System.EventHandler(this.PreviewButton_Click);
            // 
            // PrintButton
            // 
            this.PrintButton.Location = new System.Drawing.Point(259, 757);
            this.PrintButton.Name = "PrintButton";
            this.PrintButton.Size = new System.Drawing.Size(58, 28);
            this.PrintButton.TabIndex = 20;
            this.PrintButton.Text = "Print";
            // 
            // LogoButton
            // 
            this.LogoButton.Location = new System.Drawing.Point(48, 9);
            this.LogoButton.Name = "LogoButton";
            this.LogoButton.Size = new System.Drawing.Size(67, 28);
            this.LogoButton.TabIndex = 21;
            this.LogoButton.Text = "Logo";
            this.LogoButton.Click += new System.EventHandler(this.LogoButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(115, 757);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(58, 28);
            this.SaveButton.TabIndex = 23;
            this.SaveButton.Text = "Save";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // OpenButton
            // 
            this.OpenButton.Location = new System.Drawing.Point(38, 757);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(68, 28);
            this.OpenButton.TabIndex = 22;
            this.OpenButton.Text = "Open";
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.DefaultExt = "bin";
            this.openFileDialog2.FileName = "Invoice.bin";
            this.openFileDialog2.Title = "Open an Invoice File";
            this.openFileDialog2.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog2_FileOk);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "bin";
            this.saveFileDialog1.FileName = "Invoice.bin";
            this.saveFileDialog1.Title = "Choose a file to save your Invoice";
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // CalculateButton
            // 
            this.CalculateButton.Location = new System.Drawing.Point(326, 757);
            this.CalculateButton.Name = "CalculateButton";
            this.CalculateButton.Size = new System.Drawing.Size(58, 28);
            this.CalculateButton.TabIndex = 24;
            this.CalculateButton.Text = "Calc";
            this.CalculateButton.Click += new System.EventHandler(this.CalculateButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.AutoScroll = true;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1103, 847);
            this.Controls.Add(this.CalculateButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.OpenButton);
            this.Controls.Add(this.LogoButton);
            this.Controls.Add(this.PrintButton);
            this.Controls.Add(this.PreviewButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.TotalTextBox);
            this.Controls.Add(this.PercentTextBox);
            this.Controls.Add(this.SubtotalTextBox);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.PONumberTextBox);
            this.Controls.Add(this.DueDatePicker);
            this.Controls.Add(this.InvoiceNumberTextBox);
            this.Controls.Add(this.TodayPicker);
            this.Controls.Add(this.ShipToTextBox);
            this.Controls.Add(this.BillToTextBox);
            this.Controls.Add(this.AddressTextBox);
            this.Controls.Add(this.pictureBox1);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "InvoiceMaker.NET";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void PercentTextBox_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		private void Form1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			switch(e.KeyChar)
			{
				case 'a':

					break;
			}
		}

		void HandleMoveDown()
		{
			DumpRowToListView(CurrentRow);
			CurrentRow++;
			if (CurrentRow > 22)
				CurrentRow = 22;
			DumpListViewToRow(CurrentRow);
			InitializeRowEditing(CurrentRow);					 
			AddUpListViewAmounts();
			textBox1.Focus();
		}

		void HandleMoveUp()
		{
			DumpRowToListView(CurrentRow);
			CurrentRow--;
			if (CurrentRow < 0)
				CurrentRow = 0;
			DumpListViewToRow(CurrentRow);
			InitializeRowEditing(CurrentRow);					 
			AddUpListViewAmounts();
			textBox1.Focus();
		}

		private bool RowTextBoxesHaveFocus()
		{
			for (int i = 0; i < RowItems.Length; i++)
			{
			  if (RowItems[i].Focused)
				  return true;
			}

			return false;
		}

		private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyData.ToString())
			{
				case "Tab":
					if (textBox5.Focused)
						goto case "Up";
					break;
				case "Enter":
					if (RowTextBoxesHaveFocus())					
						goto case "Down";
					break;
				case "Down":
					HandleMoveDown();
					break;
				case "Up":
					HandleMoveUp();
					break;
			}
		
		}

		private void textBox5_Leave(object sender, System.EventArgs e)
		{
			HandleMoveDown();
		}

		float AddUpListViewAmounts()
		{
			double sum = 0;

			try
			{
				for (int i = 0; i < listView1.Items.Count; i++)
				{
					if (listView1.Items[i].SubItems.Count == 5) // full row
					{
						try
						{
							double partialsum = Convert.ToDouble(listView1.Items[i].SubItems[4].Text);
							sum += partialsum;
							listView1.Items[i].SubItems[4].Text = partialsum.ToString("#,###.00");

						}
						catch (FormatException ex)
						{
						  sum += 0;
						}
					}
				}

			SubtotalTextBox.Text = sum.ToString("#,###.00");

			if (this.PercentTextBox.Text.Length == 0)
				TotalTextBox.Text = sum.ToString("#,###.00");
			else
				TotalTextBox.Text = (sum + sum * Convert.ToDouble(this.PercentTextBox.Text)/100).ToString("#,###.00");

			}
			catch (Exception ex)
			{
			  MessageBox.Show(ex.Message.ToString());
			}

			return (float)sum;
		}

		//scaling function
		RectangleF GetScaledRectangle(float x, float y, RectangleF r)
		{
			RectangleF result = new RectangleF(r.X * x, r.Y * y, r.Width * x, r.Height * y);
			return result;
		}

		/// <summary>
		/// Test ListViewEdit controls
		/// </summary>
		/// <param name="g"></param>
		/// 
		private bool ControlIsListViewEdit(int index)
		{
			try
			{
				for (int i = 0; i < RowItems.Length; i++)
				{
					if (RowItems[i].Name == Controls[index].Name)
						return true;
				}
			}
			catch(Exception ex)
			{
				 MessageBox.Show(ex.Message.ToString());
			}

			return false;
		}

		// Printing functions

		private void DrawAll(Graphics g)
		{
//			RectangleF srcRect = new Rectangle(0, 0, this.LargerImage.Width, 
//				LargerImage.Height);

				RectangleF srcRect = new Rectangle(0, 0, this.BackgroundImage.Width, 
					BackgroundImage.Height);


			int nWidth = printDocument1.PrinterSettings.DefaultPageSettings.PaperSize.Width;
			int nHeight = printDocument1.PrinterSettings.DefaultPageSettings.PaperSize.Height;
			RectangleF destRect = new Rectangle(0, 0, nWidth, nHeight);
		  
			g.DrawImage(this.BackgroundImage, destRect, srcRect, GraphicsUnit.Pixel);
			//			g.DrawImage(this.LargerImage, destRect, srcRect, GraphicsUnit.Pixel);

			float scalex = destRect.Width/this.BackgroundImage.Width;
			float scaley = destRect.Height/this.BackgroundImage.Height;

			Pen aPen = new Pen(Brushes.Black, 1);

			for (int i = 0; i < this.Controls.Count; i++)
			{
				// draw logo
				if (Controls[i].GetType() == this.pictureBox1.GetType())
				{
					if (pictureBox1.Image != null)
					{
						GraphicsUnit gu = GraphicsUnit.Pixel;
						RectangleF scaledRectangle = GetScaledRectangle(scalex, scaley, pictureBox1.Bounds);
						Image myImage = (Image)pictureBox1.Image.Clone();
						g.DrawImage(myImage, scaledRectangle, pictureBox1.Image.GetBounds(ref gu), GraphicsUnit.Pixel);
					}
				}

				// print edit box control contents
				if (Controls[i].GetType() == this.textBox1.GetType())
				{
					if (!ControlIsListViewEdit(i)) // skip these
					{
						TextBox theText = (TextBox)Controls[i];
						g.DrawString(theText.Text, theText.Font, Brushes.Black, theText.Bounds.Left*scalex, theText.Bounds.Top * scaley, new StringFormat());
					}
				}

			// handle date controls
				if (Controls[i].GetType() == this.TodayPicker.GetType())
				{
					DateTimePicker aPicker = (DateTimePicker)Controls[i];
					g.DrawString(aPicker.Text, aPicker.Font, Brushes.Black, aPicker.Bounds.Left*scalex, aPicker.Bounds.Top * scaley, new StringFormat());
				}


				// handle List View Control
				if (Controls[i].GetType() == this.listView1.GetType())
				{
					for (int row = 0; row < listView1.Items.Count; row++)
					{
						int nextColumnPosition = listView1.Bounds.X;
						for (int col = 0; col < listView1.Items[row].SubItems.Count; col++)
						{
								g.DrawString(listView1.Items[row].SubItems[col].Text, listView1.Items[row].Font, Brushes.Black, (nextColumnPosition + 3)*scalex, (listView1.Items[row].Bounds.Y + listView1.Bounds.Y)* scaley, new StringFormat());
								nextColumnPosition += listView1.Columns[col].Width;

						}
					}
				}




//				if (Controls[i].GetType() == this.RetirementPlanCheck.GetType())
//				{
//					CheckBox theCheck = (CheckBox)Controls[i];
//					Rectangle aRect = theCheck.Bounds;
//					g.DrawRectangle(aPen, aRect.Left*scalex, aRect.Top*scaley, aRect.Width*scalex, aRect.Height*scaley);
//					if (theCheck.Checked)
//					{
//						g.DrawString("x", theCheck.Font, Brushes.Black, 
//							theCheck.Left*scalex + 1, theCheck.Top*scaley + 1, new StringFormat());
//					}
//				}
			}

		}


		private void textBox5_TextChanged(object sender, System.EventArgs e)
		{
		}

		private void PrintPreviewInvoice()
		{
			try
			{
				PrintPreviewDialog printPreviewDialog1 = new PrintPreviewDialog();
				printPreviewDialog1.Document = this.printDocument1 ;
				printPreviewDialog1.FormBorderStyle = FormBorderStyle.Fixed3D ;
				printPreviewDialog1.SetBounds(20, 20, this.Width, this.Height);
				printPreviewDialog1.ShowDialog();
			}
			catch(Exception exp)
			{
				System.Console.WriteLine(exp.Message.ToString());
			}

		}

		private void PrintInvoice()
		{
			printDialog1.Document = this.printDocument1;
			if (printDialog1.ShowDialog() == DialogResult.OK)
			{
				this.printDocument1.Print();
			}

		}


		private void PreviewMenu_Click(object sender, System.EventArgs e)
		{
			PrintPreviewInvoice();
		}



		private void PrintMenu_Click(object sender, System.EventArgs e)
		{
			PrintInvoice();
		}

		private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			this.Focus();
			DrawAll(e.Graphics);
		}

		private void PreviewButton_Click(object sender, System.EventArgs e)
		{
			PrintPreviewInvoice();
		}

		private void LogoButton_Click(object sender, System.EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
				ThePictureFileName = openFileDialog1.FileName;
			}
		}

		private void OpenButton_Click(object sender, System.EventArgs e)
		{
			this.OpenForm();
		}

		private void SaveButton_Click(object sender, System.EventArgs e)
		{
			this.SaveForm();
		}

		private void CalculateButton_Click(object sender, System.EventArgs e)
		{
			DumpRowToListView(CurrentRow);
			AddUpListViewAmounts();
		}

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ShipToTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void InvoiceNumberTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        private void TodayPicker_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
