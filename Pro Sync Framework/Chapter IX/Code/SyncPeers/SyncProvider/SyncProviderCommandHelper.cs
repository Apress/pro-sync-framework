using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Synchronization.Data;
using System.Data;

namespace SyncPeers
{
    /// <summary>
    /// Creates Commands related to sync provider
    /// </summary>
    public class SyncProviderCommandHelper
    {
        /// <summary>
        /// Returns a command for selecting the new high watermark for 
        ///   the current synchronization session.
        /// </summary>
        /// <returns></returns>
        public static SqlCommand GetSelectNewTimestampCommand()
        {
            SqlCommand cmdSelectNewTimestamp = new SqlCommand();
            string newTimestamp = "@" + DbSyncSession.SyncNewTimestamp;
            cmdSelectNewTimestamp.CommandText = "SELECT " + newTimestamp + " = @@DBTS "; // for SQL Server 2005 SP2, use "min_active_rowversion() - 1" or " = @@DBTS";
            cmdSelectNewTimestamp.Parameters.Add(newTimestamp, SqlDbType.Timestamp);
            cmdSelectNewTimestamp.Parameters[newTimestamp].Direction = ParameterDirection.Output;
            return cmdSelectNewTimestamp;
        }

        /// <summary>
        /// Returns a command for Selecting sync knowledge, cleanup knowledge, 
        //   and a scope version (timestamp)
        /// </summary>
        /// <returns></returns>
        public static SqlCommand GetSelectScopeInfoCommand()
        {
            SqlCommand cmdSelectScopeInfo = new SqlCommand();
            cmdSelectScopeInfo.CommandType = CommandType.Text;
            cmdSelectScopeInfo.CommandText = "SELECT " +
                                            "@" + DbSyncSession.SyncScopeId + " = scope_id, " +
                                            "@" + DbSyncSession.SyncScopeKnowledge + " = scope_sync_knowledge, " +
                                            "@" + DbSyncSession.SyncScopeCleanupKnowledge + " = scope_tombstone_cleanup_knowledge, " +
                                            "@" + DbSyncSession.SyncScopeTimestamp + " = scope_timestamp " +
                                            "FROM AccountScope " +
                                            "WHERE scope_name = @" + DbSyncSession.SyncScopeName;
            cmdSelectScopeInfo.Parameters.Add("@" + DbSyncSession.SyncScopeName, SqlDbType.NVarChar, 100);
            cmdSelectScopeInfo.Parameters.Add("@" + DbSyncSession.SyncScopeId, SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;
            cmdSelectScopeInfo.Parameters.Add("@" + DbSyncSession.SyncScopeKnowledge, SqlDbType.VarBinary, 10000).Direction = ParameterDirection.Output;
            cmdSelectScopeInfo.Parameters.Add("@" + DbSyncSession.SyncScopeCleanupKnowledge, SqlDbType.VarBinary, 10000).Direction = ParameterDirection.Output;
            cmdSelectScopeInfo.Parameters.Add("@" + DbSyncSession.SyncScopeTimestamp, SqlDbType.BigInt).Direction = ParameterDirection.Output;

            return cmdSelectScopeInfo;
        }

        /// <summary>
        /// Returns a command for updating sync knowledge and cleanup knowledge
        /// </summary>
        /// <returns></returns>
        public static SqlCommand GetUpdateScopeInfoCommand()
        {
            SqlCommand cmdUpdateScopeInfo = new SqlCommand();
            cmdUpdateScopeInfo.CommandType = CommandType.Text;
            cmdUpdateScopeInfo.CommandText = "UPDATE  AccountScope SET " +
                                            "scope_sync_knowledge = @" + DbSyncSession.SyncScopeKnowledge + ", " +
                                            "scope_tombstone_cleanup_knowledge = @" + DbSyncSession.SyncScopeCleanupKnowledge + " " +
                                            "WHERE scope_name = @" + DbSyncSession.SyncScopeName + " AND " +
                                            " ( @" + DbSyncSession.SyncCheckConcurrency + " = 0 or scope_timestamp = @" + DbSyncSession.SyncScopeTimestamp + "); " +
                                            "SET @" + DbSyncSession.SyncRowCount + " = @@rowcount";
            cmdUpdateScopeInfo.Parameters.Add("@" + DbSyncSession.SyncScopeKnowledge, SqlDbType.VarBinary, 10000);
            cmdUpdateScopeInfo.Parameters.Add("@" + DbSyncSession.SyncScopeCleanupKnowledge, SqlDbType.VarBinary, 10000);
            cmdUpdateScopeInfo.Parameters.Add("@" + DbSyncSession.SyncScopeName, SqlDbType.NVarChar, 100);
            cmdUpdateScopeInfo.Parameters.Add("@" + DbSyncSession.SyncCheckConcurrency, SqlDbType.Int);
            cmdUpdateScopeInfo.Parameters.Add("@" + DbSyncSession.SyncScopeTimestamp, SqlDbType.BigInt);
            cmdUpdateScopeInfo.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            return cmdUpdateScopeInfo;
        }
    }
}
