using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;
using Microsoft.Synchronization.MetadataStorage;

namespace BaseSyncProvider
{

    /// <summary>
    /// Custom Synchronization Provider
    /// </summary>
    public abstract class MyBaseSyncProvider<TransferClass> : KnowledgeSyncProvider, IChangeDataRetriever, INotifyingChangeApplierTarget, IDisposable
    {

        #region Local variables

        //Built in SQL CE metadatatore provided by MSF.In this sample we are going to use the metadata store provided by MSF
        SqlMetadataStore _metadataStore = null;
        ReplicaMetadata _metadata = null;
        SyncId _replicaID = null;
        SyncIdFormatGroup _idFormats = null;
        SyncSessionContext _currentSessionContext = null;
        string _replicaName;
        Dictionary<string, DateTime> _itemDataIds;

        #endregion

        #region Constructor

       
        /// <summary>
        /// Creates or opens the existing metadata store.
        /// </summary>
        /// <param name="replicaName"></param>
        /// <param name="replicaCon"></param>
        /// <param name="fileName"></param>
        public MyBaseSyncProvider(string replicaName, string fileName)
        {
            _replicaName = replicaName;
            _replicaID = new SyncId(replicaName);
            _idFormats = new SyncIdFormatGroup();
            _idFormats.ItemIdFormat.IsVariableLength = true;
            _idFormats.ItemIdFormat.Length = 500;
            _idFormats.ReplicaIdFormat.IsVariableLength = true;
            _idFormats.ReplicaIdFormat.Length = 500;

            //Create or open the metadata store, initializing it with the flexbile id formats we'll use to reference our items and endpoints
            if (!File.Exists(fileName))
            {
                _metadataStore = SqlMetadataStore.CreateStore(fileName);
                _metadata = _metadataStore.InitializeReplicaMetadata(_idFormats, _replicaID, null, null);
            }
            else
            {
                _metadataStore = SqlMetadataStore.OpenStore(fileName);
                _metadata = _metadataStore.GetReplicaMetadata(_idFormats, _replicaID);
            }
            _metadata.SetForgottenKnowledge(new ForgottenKnowledge(_idFormats, _metadata.GetKnowledge()));
        }

        #endregion

        #region Replica Operation methods

        // Abstract methods that need to be overridden in the derived classes.
        // These provide the functionality specific to each actual data store.
        public abstract void CreateItemData(TransferClass itemData);
        public abstract void UpdateItemData(TransferClass itemData);
        public abstract void DeleteItemData(TransferClass itemData);
        public abstract void UpdateItemDataWithDestination(TransferClass sourceItemData, TransferClass destItemData);
        public abstract void MergeItemData(TransferClass sourceItemData, TransferClass destItemData);
        public abstract TransferClass GetItemData(string itemDataId, string replicaName);
        public abstract Dictionary<string, DateTime> ItemDataIds { get; }

        /// <summary>
        /// BeginUpdates() begins a transaction for the replica.Using BeginUpdates and EndUpdates the operations on the replica can be made transactional.
        /// </summary>
        public void BeginUpdates()
        {
            _metadataStore.BeginTransaction();
        }

        /// <summary>
        /// EndUpdates ends a transaction for the replica
        /// </summary>
        public void EndUpdates()
        {
            _metadataStore.CommitTransaction();
        }

        #endregion

        #region Overriden Provider Methods

        public override void BeginSession(SyncProviderPosition position, SyncSessionContext syncSessionContext)
        {
            _currentSessionContext = syncSessionContext;
        }

        public override void EndSession(SyncSessionContext syncSessionContext)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, _replicaName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            SaveLastSyncTime(DateTime.Now);
        }

        public override FullEnumerationChangeBatch GetFullEnumerationChangeBatch(uint batchSize, SyncId lowerEnumerationBound, SyncKnowledge knowledgeForDataRetrieval, out object changeDataRetriever)
        {
            throw new NotImplementedException();
        }

        public override SyncIdFormatGroup IdFormats
        {
            get { return _idFormats; }
        }

        public override void ProcessFullEnumerationChangeBatch(ConflictResolutionPolicy resolutionPolicy, FullEnumerationChangeBatch sourceChanges, object changeDataRetriever, SyncCallbacks syncCallback, SyncSessionStatistics sessionStatistics)
        {
            throw new NotImplementedException();
        }

        public ulong GetNextTickCount()
        {
            return _metadata.GetNextTickCount();
        }

        public void SaveChangeWithChangeUnits(ItemChange change, SaveChangeWithChangeUnitsContext context)
        {
            throw new NotImplementedException();
        }

        public void SaveConflict(ItemChange conflictingChange, object conflictingChangeData, SyncKnowledge conflictingChangeKnowledge)
        {
            throw new NotImplementedException();
        }

        public IChangeDataRetriever GetDataRetriever()
        {
            return this;
        }

        public bool TryGetDestinationVersion(ItemChange sourceChange, out ItemChange destinationVersion)
        {
            destinationVersion = null;
            return false;
        }

        /// <summary>
        /// Sync framework calls GetSyncBatchParameters method on the destination provider
        /// </summary>
        /// <param name="batchSize">an integer which returns the batch size </param>
        /// <param name="knowledge">returns the knowledge of the destination</param>
        public override void GetSyncBatchParameters(out uint batchSize, out SyncKnowledge knowledge)
        {
            batchSize = 10000;
            knowledge = _metadata.GetKnowledge();
        }

        /// <summary>
        /// Sync framework calls the GetChangeBatch method on the source provider.
        /// This method receives the batch size and the knowledge of the destination as two input parameters. 
        /// After comparing this with local item versions, source sync provider sends the summary of changed versions 
        /// and knowledge to the destination provider in form of the changeDataRetriever object.
        /// </summary>
        /// <param name="batchSize"></param>
        /// <param name="destinationKnowledge"></param>
        /// <param name="changeDataRetriever"></param>
        /// <returns></returns>
        public override ChangeBatch GetChangeBatch(uint batchSize, SyncKnowledge destinationKnowledge, out object changeDataRetriever)
        {
            //Make sure that Metadata store is updated with local changes in the replica.
            UpdateMetadataStoreWithChanges();
            // Construct the ChangeBatch and return it
            ChangeBatch batch = _metadata.GetChangeBatch(batchSize, destinationKnowledge); ;
            changeDataRetriever = this;
            return batch;
        }

        /// <summary>
        /// Sync framework calls the ProcessChangeBatch method on the destination provider 
        /// Destination provider receives the change versions and source knowledge in the form of two input parameters sourceChanges and changeDataRetriever.
        /// </summary>
        /// <param name="resolutionPolicy">Defines the way conflicts are handled</param>
        /// <param name="sourceChanges">Chnage batch from the source provider</param>
        /// <param name="changeDataRetriever">IChangeDataRetriever passed from the source</param>
        /// <param name="syncCallback">Sync call abck for raising events to sync agent</param>
        /// <param name="sessionStatistics">statistics about the sync session.</param>
        public override void ProcessChangeBatch(ConflictResolutionPolicy resolutionPolicy, ChangeBatch sourceChanges,
            object changeDataRetriever, SyncCallbacks syncCallback, SyncSessionStatistics sessionStatistics)
        {
            BeginUpdates(); 
            IEnumerable<ItemChange> localChanges = _metadata.GetLocalVersions(sourceChanges);
            NotifyingChangeApplier changeApplier = new NotifyingChangeApplier(_idFormats);
            changeApplier.ApplyChanges(resolutionPolicy, sourceChanges, changeDataRetriever as IChangeDataRetriever, localChanges, _metadata.GetKnowledge(),
                _metadata.GetForgottenKnowledge(), this, _currentSessionContext, syncCallback);
            EndUpdates();
        }

        /// <summary>
        /// Sync framework calls the LoadChangeData method on the source provider.
        /// source provider retrieves the items requested by destination provider from its replica and sends them to the destination provider
        /// </summary>
        /// <param name="loadChangeContext"></param>
        /// <returns></returns>
        public object LoadChangeData(LoadChangeContext loadChangeContext)
        {

            string ID = loadChangeContext.ItemChange.ItemId.GetStringId();
            return GetItemData(ID, _replicaName);
        }


        /// <summary>
        /// Saves the item and metadata in destination replica
        /// </summary>
        /// <param name="saveChangeAction">specifies what operation to perform on destination replica like create/update or delete</param>
        /// <param name="change">contains the information about the change to an item</param>
        /// <param name="context">represents information about a change to be saved to the item store</param>
        public void SaveItemChange(SaveChangeAction saveChangeAction, ItemChange change, SaveChangeContext context)
        {
            ItemMetadata item = null;

            switch (saveChangeAction)
            {
                case SaveChangeAction.Create:
                    //Save the Change
                    TransferClass itemData = (TransferClass)context.ChangeData;
                    CreateItemData(itemData);

                    //Save the metadata
                    item = _metadata.CreateItemMetadata(change.ItemId, change.CreationVersion);
                    item.ChangeVersion = change.ChangeVersion;
                    _metadata.SaveItemMetadata(item);
                    break;

                case SaveChangeAction.UpdateVersionAndData:
                    {
                        item = _metadata.FindItemMetadataById(change.ItemId);
                        if (null == item)
                        {
                            throw new Exception("Record is not present in replica");
                        }

                        item.ChangeVersion = change.ChangeVersion;

                        TransferClass destItemData = GetItemData(item.GlobalId.GetStringId(), _replicaName);                        
                        TransferClass sourceItemData = (TransferClass)context.ChangeData;

                        UpdateItemDataWithDestination(sourceItemData, destItemData);
                        _metadata.SaveItemMetadata(item);
                    }
                    break;
                case SaveChangeAction.UpdateVersionOnly:
                    {
                        item = _metadata.FindItemMetadataById(change.ItemId);
                        if (null == item)
                        {
                            throw new Exception("Record is not present in replica");
                        }

                        item.ChangeVersion = change.ChangeVersion;
                        _metadata.SaveItemMetadata(item);
                    }
                    break;

                case SaveChangeAction.DeleteAndStoreTombstone:
                    item = _metadata.FindItemMetadataById(change.ItemId);
                    if (null == item)
                    {
                        item = _metadata.CreateItemMetadata(change.ItemId, change.CreationVersion);
                    }

                    if (change.ChangeKind == ChangeKind.Deleted)
                    {
                        item.MarkAsDeleted(change.ChangeVersion);
                    }
                    else
                    {
                        throw new Exception("Invalid changeType");
                    }

                    item.ChangeVersion = change.ChangeVersion;
                    TransferClass itemDataToBeDeleted = GetItemData(item.GlobalId.GetStringId(), _replicaName);
                    if (null != itemDataToBeDeleted) DeleteItemData(itemDataToBeDeleted);
                    _metadata.SaveItemMetadata(item);
                    break;

                case SaveChangeAction.UpdateVersionAndMergeData:
                    item = _metadata.FindItemMetadataById(change.ItemId);
                    item.ChangeVersion = new SyncVersion(0, _metadata.GetNextTickCount());

                    TransferClass destItemDataMerge = GetItemData(item.GlobalId.GetStringId(), _replicaName);
                    TransferClass sourceItemDataMerge = (TransferClass)context.ChangeData;

                    MergeItemData(sourceItemDataMerge, destItemDataMerge);
                    _metadata.SaveItemMetadata(item);
                    break;
            }
        }

        /// <summary>
        /// We also need to save the knowledge after each sync.
        /// We just save the knowledge and forgotten Knowledge in the Replica metadata store.
        /// </summary>
        /// <param name="knowledge"></param>
        /// <param name="forgottenKnowledge"></param>
        public void StoreKnowledgeForScope(SyncKnowledge knowledge, ForgottenKnowledge forgottenKnowledge)
        {
            _metadata.SetKnowledge(knowledge);
            _metadata.SetForgottenKnowledge(forgottenKnowledge);
            _metadata.SaveReplicaMetadata();
        }

        #endregion

        #region Asynchronus Change Tracking Methods

        /// <summary>
        /// This Methods Performs Aynchronous Change Tracking
        /// </summary>
        protected void UpdateMetadataStoreWithChanges()
        {
            try
            {
                _metadataStore.BeginTransaction();
                _itemDataIds = ItemDataIds;
                _metadata.DeleteDetector.MarkAllItemsUnreported();
                foreach (KeyValuePair<string, DateTime> itemDataId in _itemDataIds)
                {

                    SyncId syncId = new SyncId(itemDataId.Key);
                    ItemMetadata existingItem = _metadata.FindItemMetadataById(syncId);
                    if (existingItem == null)
                    {
                        //Creates new item metadata by using the 
                        //1. sync id [Created from ID field of Customer]
                        //2. sync version which has replica Id [0] and the tick count which a logical sync clock
                        existingItem = _metadata.CreateItemMetadata(syncId, new SyncVersion(0, _metadata.GetNextTickCount()));
                        //sets teh change version to created version
                        existingItem.ChangeVersion = existingItem.CreationVersion;
                        //saves the item in metadata store
                        _metadata.SaveItemMetadata(existingItem);
                    }
                    else
                    {
                        if (itemDataId.Value.CompareTo(GetLastSyncTime()) > 0)
                        {
                            //sets the change version by incrementing the tick count
                            existingItem.ChangeVersion = new SyncVersion(0, _metadata.GetNextTickCount());
                            _metadata.SaveItemMetadata(existingItem);
                        }
                        else
                        {
                            _metadata.DeleteDetector.ReportLiveItemById(syncId);
                        }
                    }
                }
                foreach (ItemMetadata item in _metadata.DeleteDetector.FindUnreportedItems())
                {
                    item.ChangeVersion = new SyncVersion(0, _metadata.GetNextTickCount());
                    item.MarkAsDeleted(item.ChangeVersion);
                    _metadata.SaveItemMetadata(item);
                }
                _metadataStore.CommitTransaction();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DateTime GetLastSyncTime()
        {
            string LastSyncTimeFilePath = Path.Combine(Environment.CurrentDirectory, _replicaName + @".LastSyncTime");
            if (!File.Exists(LastSyncTimeFilePath))
            {
                DateTime dt = DateTime.MinValue;
                SaveLastSyncTime(dt);
                return dt;
            }
            else
            {
                using (FileStream fs = File.Open(LastSyncTimeFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // Read the LastSyncTime from the file.
                    fs.Seek(0, SeekOrigin.Begin);
                    BinaryReader br = new BinaryReader(fs);
                    return new DateTime(br.ReadInt64());
                }
            }
        }

        private void SaveLastSyncTime(DateTime dt)
        {
            using (FileStream fs = File.Open(Path.Combine(Environment.CurrentDirectory, _replicaName + @".LastSyncTime"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                // Write the LastSyncTime to the file.
                fs.SetLength(0);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(dt.Ticks);
            }
        }

        #endregion

        #region Destructor

        ~MyBaseSyncProvider()
        {
            CleanUp(false);
        }

        private bool disposed = false;
        public void Dispose()
        {
            CleanUp(true);
            GC.SuppressFinalize(this);
        }

        private void CleanUp(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    _metadataStore.Dispose();
                }
                // Clean up unmanaged resources here.
            }
            disposed = true;
        }




        #endregion
    }
}
