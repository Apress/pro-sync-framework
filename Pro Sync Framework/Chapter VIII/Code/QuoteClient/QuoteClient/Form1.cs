using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuoteClient
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void quoteBindingNavigatorSaveItem_Click(object sender, EventArgs e)
		{
			this.Validate();
			this.quoteBindingSource.EndEdit();
			this.tableAdapterManager.UpdateAll(this.quoteDataSet);

		}

		private void Form1_Load(object sender, EventArgs e)
		{
			// TODO: This line of code loads data into the 'quoteDataSet.Quote' table. You can move, or remove it, as needed.
			this.quoteTableAdapter.Fill(this.quoteDataSet.Quote);

		}

		private void button1_Click(object sender, EventArgs e)
		{
			MySyncSyncAgent quoteSyncAgent = new MySyncSyncAgent();
			Microsoft.Synchronization.Data.SyncStatistics syncStat = quoteSyncAgent.Synchronize();
			if (MessageBox.Show(
				string.Format(@"Start Time: {0}{8}Completed Time: {1}{8}No of Changes Applied (Downloaded / Failed): {2}/{3}{8}No of Changes Applied (Uploaded / Failed): {4}/{5}{8}Total (Downloaded / Uploaded): {6}/{7}", 
				syncStat.SyncStartTime, syncStat.SyncCompleteTime,
				syncStat.DownloadChangesApplied, syncStat.DownloadChangesFailed, syncStat.UploadChangesApplied, syncStat.UploadChangesFailed,
				syncStat.TotalChangesDownloaded, syncStat.TotalChangesUploaded, Environment.NewLine)) == DialogResult.OK)
			{
				Form1_Load(null, null);
			}
		}
	}
}
