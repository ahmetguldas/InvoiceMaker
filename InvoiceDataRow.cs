using System;

namespace InvoiceMaker
{
	/// <summary>
	/// Summary description for InvoiceDataRow.
	/// </summary>
	/// 
	[Serializable]
	public class InvoiceDataRow
	{
		public string Item;
		public string Description;
		public string Quantity;
		public string Rate;
		public string Amount;

		public InvoiceDataRow()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
