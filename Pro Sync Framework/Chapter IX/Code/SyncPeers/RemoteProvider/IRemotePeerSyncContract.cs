using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
namespace RemoteProvider
{

    [ServiceContract(SessionMode = SessionMode.Required)]
    [ServiceKnownType(typeof(SyncIdFormatGroup))]
    [ServiceKnownType(typeof(DbSyncContext))]
    public interface IRemotePeerSyncContract
    {
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        void BeginSession();

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void GetKnowledge(out uint batchSize, out SyncKnowledge knowledge);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        ChangeBatch GetChanges(uint batchSize, SyncKnowledge destinationKnowledge, out object changeData);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void ApplyChanges(ConflictResolutionPolicy resolutionPolicy, ChangeBatch sourceChanges, object changeData,
            ref SyncSessionStatistics sessionStatistics);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void EndSession();
    }
}

