using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SyncPeers
{
    /// <summary>
    /// Displays the data from peer database in gridview
    /// </summary>
    public partial class PeersData : UserControl
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PeersData()
        {
            InitializeComponent();
            RefreshGrids();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Populates the data 
        /// </summary>
        public void RefreshGrids()
        {
            string querry = " SELECT * FROM Account";
            BindDataGridView(Properties.Settings.Default.Collaboration_Peer1ConnectionString, querry, grdViewA);
            BindDataGridView(Properties.Settings.Default.Collaboration_Peer2ConnectionString, querry, grdViewB);
            BindDataGridView(Properties.Settings.Default.Collaboration_Peer3ConnectionString, querry, grdViewC);
        }

        public Guid GetSelectedRecord(int peerId)
        {
            switch (peerId)
            {
                case 1:
                    return new Guid(grdViewA.SelectedCells[0].Value.ToString());                    
                case 2:
                    return new Guid(grdViewB.SelectedCells[0].Value.ToString());
                case 3:
                    return new Guid(grdViewC.SelectedCells[0].Value.ToString());
                default:
                    return Guid.Empty;
            }
        }

        #endregion

        #region Private helper methods
        private void BindDataGridView(string conString,string querry,DataGridView dgv )
        {
            using (SqlConnection con = new SqlConnection(conString))
            using (SqlDataAdapter adr = new SqlDataAdapter(querry, con))
            using (DataSet ds = new DataSet())
            {
                adr.Fill(ds);
                dgv.DataSource = ds.Tables[0];
            }
        }
        #endregion

    }
}
