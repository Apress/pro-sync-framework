using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CustomProvider
{
    /// <summary>
    /// Displays the data from replica in gridview
    /// </summary>
    public partial class ReplicaContent : UserControl
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ReplicaContent()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Populates the data from the replicas
        /// </summary>
        /// <param name="providerA_ReplicaCon"></param>
        /// <param name="providerB_ReplicaCon"></param>
        /// <param name="providerC_ReplicaCon"></param>
        /// <param name="querry"></param>
        public void Refresh(string providerA_ReplicaCon, string providerB_ReplicaCon, string providerC_ReplicaCon, string querry)
        {

            BindDataGridView(providerA_ReplicaCon, querry, grdViewA);
            BindDataGridView(providerB_ReplicaCon, querry, grdViewB);
            BindDataGridView(providerC_ReplicaCon, querry, grdViewC);
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
