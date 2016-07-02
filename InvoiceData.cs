using System;
using System.Collections;

namespace InvoiceMaker
{
	/// <summary>
	/// Summary description for InvoiceData.
	/// </summary>
	/// 
	[Serializable]
	public class InvoiceData
	{
		public InvoiceDataRow[]   DataRows = new InvoiceDataRow[23];
		public string LogoFile = "";
		public string BillerAddress = "";
		public string BillToAddress = "";
		public string ShipToAddress = "";
		public string InvoiceDate = "";
		public string InvoiceNumber = "";
		public string DueDate = "";
		public string PONumber = "";
		public string PercentTax = "";
		public string Subtotal = "";
		public string Total = "";
		public int RowCount = 0;



		public InvoiceData()
		{
			DataRows.Initialize();
			for (int i = 0; i < DataRows.Length; i++)
			{
			   DataRows[i] = new InvoiceDataRow();
			}
			//
			// TODO: Add constructor logic here
			//
		}

	}
}
