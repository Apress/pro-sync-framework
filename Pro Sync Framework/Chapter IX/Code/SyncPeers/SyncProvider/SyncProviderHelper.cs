using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Synchronization.Data;
using System.Data.SqlClient;

namespace SyncPeers
{
    /// <summary>
    /// Contains Static Method to Create Sync Adapter
    /// </summary>
    public class SyncProviderHelper
    {
        /// <summary>
        ///    1.	Create a new Sync provider
        ///    2.	Creating ADO.NET commands for manipulating knowledge of the peer
        ///    3.	Associating these commands with the sync provider
        ///    4.	Assigning a scope to the sync provider
        ///    5.	Assigning the SQL connection for Provider.
        /// </summary>
        /// <param name="peerID"></param>
        /// <returns></returns>
        public static DbSyncProvider CreateSyncProvider(int peerID)
        {
            DbSyncProvider syncProvider = new DbSyncProvider();
            syncProvider.ScopeName = "AccountScope";
            syncProvider.Connection = new SqlConnection(DBHelper.GetPeerConncetionStringByPeerID(peerID));

            syncProvider.SyncAdapters.Add(SyncAdapterHelper.CreateSyncAdpters());

            syncProvider.SelectNewTimestampCommand = SyncProviderCommandHelper.GetSelectNewTimestampCommand();
            syncProvider.SelectScopeInfoCommand = SyncProviderCommandHelper.GetSelectScopeInfoCommand();
            syncProvider.UpdateScopeInfoCommand = SyncProviderCommandHelper.GetUpdateScopeInfoCommand();

            return syncProvider;
        }
    }
}
