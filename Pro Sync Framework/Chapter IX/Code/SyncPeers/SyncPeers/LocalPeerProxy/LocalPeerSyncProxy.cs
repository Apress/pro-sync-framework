using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Synchronization;
using System.ServiceModel;
using SyncPeers.RemoteProviderProxy;

namespace SyncPeers.LocalPeerProxy
{
    public class LocalPeerSyncProxy:KnowledgeSyncProvider
    {
        string _remoteUri;
        SyncIdFormatGroup syncIdFormatGroup;
        RemoteProviderProxy.IRemotePeerSyncContract syncProxy;

        public LocalPeerSyncProxy(string remoteUri)
        {
            _remoteUri = remoteUri;
            CreateProxyForRemoteProvider();
        }

        private void CreateProxyForRemoteProvider()
        {
            try
            {                
                WSHttpBinding wSHttpBinding = new WSHttpBinding();
                wSHttpBinding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                wSHttpBinding.OpenTimeout = new TimeSpan(0, 1, 0);
                ChannelFactory<IRemotePeerSyncContract> factory = new ChannelFactory<IRemotePeerSyncContract>(wSHttpBinding,
                    new EndpointAddress(_remoteUri));
                syncProxy = factory.CreateChannel();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public override void BeginSession(SyncProviderPosition position, SyncSessionContext syncSessionContext)
        {
            syncProxy.BeginSession();
        }

        public override void EndSession(SyncSessionContext syncSessionContext)
        {
            syncProxy.EndSession();
        }

        public override void GetSyncBatchParameters(out uint batchSize, out SyncKnowledge knowledge)
        {
            batchSize = syncProxy.GetKnowledge(out knowledge);
            //syncProxy.GetKnowledge(out batchSize, out knowledge);
        }

        public override ChangeBatch GetChangeBatch(uint batchSize, SyncKnowledge destinationKnowledge,
            out object changeDataRetriever)
        {

            return syncProxy.GetChanges(out changeDataRetriever, batchSize, destinationKnowledge);
            //return syncProxy.GetChanges(batchSize, destinationKnowledge, out changeDataRetriever);
        }

        public override void ProcessChangeBatch(ConflictResolutionPolicy resolutionPolicy, ChangeBatch sourceChanges,
            object changeDataRetriever, SyncCallbacks syncCallback, SyncSessionStatistics sessionStatistics)
        {
            SyncSessionStatistics remoteSessionStatistics = new SyncSessionStatistics();
            syncProxy.ApplyChanges(resolutionPolicy, sourceChanges, changeDataRetriever, ref remoteSessionStatistics);
            sessionStatistics.ChangesApplied = remoteSessionStatistics.ChangesApplied;
            sessionStatistics.ChangesFailed = remoteSessionStatistics.ChangesFailed;
        }

        public override SyncIdFormatGroup IdFormats
        {
            get
            {
                if (syncIdFormatGroup == null)
                {
                    syncIdFormatGroup = new SyncIdFormatGroup();
                    syncIdFormatGroup.ChangeUnitIdFormat.IsVariableLength = false;
                    syncIdFormatGroup.ChangeUnitIdFormat.Length = 1;
                    syncIdFormatGroup.ReplicaIdFormat.IsVariableLength = false;
                    syncIdFormatGroup.ReplicaIdFormat.Length = 16;
                    syncIdFormatGroup.ItemIdFormat.IsVariableLength = true;
                    syncIdFormatGroup.ItemIdFormat.Length = 10 * 1024;
                }
                return syncIdFormatGroup;
            }
        }

        public override FullEnumerationChangeBatch GetFullEnumerationChangeBatch(uint batchSize, SyncId lowerEnumerationBound,
        SyncKnowledge knowledgeForDataRetrieval, out object changeDataRetriever)
        {
            throw new NotImplementedException();
        }

        public override void ProcessFullEnumerationChangeBatch(ConflictResolutionPolicy resolutionPolicy,
        FullEnumerationChangeBatch sourceChanges, object changeDataRetriever, SyncCallbacks syncCallback,
        SyncSessionStatistics sessionStatistics)
        {
            throw new NotImplementedException();
        }
    }
}
