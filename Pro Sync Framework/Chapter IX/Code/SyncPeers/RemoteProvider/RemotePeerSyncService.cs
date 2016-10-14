using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using SyncPeers;

namespace RemoteProvider
{

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RemotePeerSyncService : IRemotePeerSyncContract  
    {


        private DbSyncProvider remoteSyncProvider = null;

        public void BeginSession()
        {
            remoteSyncProvider = SyncProviderHelper.CreateSyncProvider(2);//@:Peer2
        }

        public void GetKnowledge(
            out uint batchSize,
            out SyncKnowledge knowledge)
        {
            remoteSyncProvider.GetSyncBatchParameters(out batchSize, out knowledge);
        }

        public ChangeBatch GetChanges(
            uint batchSize,
            SyncKnowledge destinationKnowledge,
            out object changeData)
        {
            return remoteSyncProvider.GetChangeBatch(batchSize, destinationKnowledge, out changeData);
        }

        public void ApplyChanges(
            ConflictResolutionPolicy resolutionPolicy,
            ChangeBatch sourceChanges,
            object changeData,
            ref SyncSessionStatistics sessionStatistics)
        {
            SyncCallbacks syncCallback = new SyncCallbacks();
            remoteSyncProvider.ProcessChangeBatch(resolutionPolicy, sourceChanges, changeData, syncCallback,
                sessionStatistics);
        }

        public void EndSession()
        {
            remoteSyncProvider = null;
        }

    }
}
