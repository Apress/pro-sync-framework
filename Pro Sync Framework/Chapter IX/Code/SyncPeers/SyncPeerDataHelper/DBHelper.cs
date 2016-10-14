using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SyncPeers
{
    /// <summary>
    /// Contains Static method to Populate controls and Handle Data operations for base table[Account]
    /// </summary>
    public class DBHelper
    {
        /// <summary>
        /// Gets the connection string for peer
        /// </summary>
        /// <param name="peerID"></param>
        /// <returns></returns>
        public static string GetPeerConncetionStringByPeerID(int peerID)
        {
            switch (peerID)
            {
                case 1:
                    return Properties.Settings.Default.Collaboration_Peer1ConnectionString;                   
                case 2:
                    return Properties.Settings.Default.Collaboration_Peer2ConnectionString;                 
                case 3:
                    return Properties.Settings.Default.Collaboration_Peer3ConnectionString;                    
                default:
                    return string.Empty;                  
            }
        }

        /// <summary>
        /// Returns a datatable containing Peers
        /// </summary>
        /// <returns></returns>
        public static DataTable GetPeers()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PeerID", typeof(int));
            dt.Columns.Add("PeerName", typeof(string));
            DataRow dr = dt.NewRow();
            dr["PeerID"] = 1;
            dr["PeerName"] = "Peer1";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["PeerID"] = 2;
            dr["PeerName"] = "Peer2";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["PeerID"] = 3;
            dr["PeerName"] = "Peer3";
            dt.Rows.Add(dr);
            return dt;
        }

        /// <summary>
        /// Returns a datatable containing operations
        /// </summary>
        /// <returns></returns>
        public static DataTable GetOperations()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("OpCode", typeof(int));
            dt.Columns.Add("OpName", typeof(string));
            DataRow dr = dt.NewRow();
            dr["OpCode"] = 1;
            dr["OpName"] = "Create Data";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["OpCode"] = 2;
            dr["OpName"] = "Updtate Data";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["OpCode"] = 3;
            dr["OpName"] = "Delete Data";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["OpCode"] = 4;
            dr["OpName"] = "Clean Data";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["OpCode"] = 5;
            dr["OpName"] = "Clean MetaData";
            dt.Rows.Add(dr);
            return dt;
        }

        /// <summary>
        /// Returns datatable containing  Conflict types
        /// </summary>
        /// <returns></returns>
        public static DataTable GetConflictType()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ConflictTypeID", typeof(int));
            dt.Columns.Add("ConflictTypeName", typeof(string));
            DataRow dr = dt.NewRow();
            dr["ConflictTypeID"] = 1;
            dr["ConflictTypeName"] = "Insert-Insert";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["ConflictTypeID"] = 2;
            dr["ConflictTypeName"] = "Update-Update";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["ConflictTypeID"] = 3;
            dr["ConflictTypeName"] = "Update-Delete";
            dt.Rows.Add(dr);
            return dt;
        }

        /// <summary>
        /// Performs data operations
        /// </summary>
        /// <param name="operationCode"></param>
        /// <param name="peerID"></param>
        public static void ExecuteOperation(int operationCode, int peerID, Guid slectedGuid)
        {
            switch (operationCode)
            {
                case 1:
                    string sql = "INSERT INTO Account(Name) VALUES ('TestAccount" + DateTime.Now.ToLongTimeString() + "')";
                    ExecuteSqlQuery(sql, peerID);
                    break;
                case 2:
                    sql = "UPDATE Account SET Name = 'TestAccount" + DateTime.Now.ToLongTimeString() + "' WHERE AccountId = '" + slectedGuid.ToString() + "'";
                    ExecuteSqlQuery(sql, peerID);
                    break;
                case 3:
                    sql = "DELETE FROM Account WHERE AccountId = '" + slectedGuid.ToString() + "'";
                    ExecuteSqlQuery(sql, peerID);
                    break;
                case 4:
                    CleanPeers(peerID);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Resets the Peer Db's back to orginal state
        /// </summary>
        /// <param name="peerID"></param>
        public static void CleanPeers(int peerID)
        {
            ExecuteSqlQuery("EXEC ('sp_ResetData')", peerID);
        }

        /// <summary>
        /// Eexutes specified sql queery on the specfied peer db
        /// </summary>
        /// <param name="query"></param>
        /// <param name="peerID"></param>
        public  static void ExecuteSqlQuery(string query, int peerID)
        {           
            using (SqlConnection con = new SqlConnection(GetPeerConncetionStringByPeerID(peerID)))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }        
    }
}
