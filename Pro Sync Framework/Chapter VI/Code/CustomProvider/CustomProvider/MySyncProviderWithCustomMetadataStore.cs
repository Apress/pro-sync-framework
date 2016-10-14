using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Synchronization;
using Microsoft.Synchronization.MetadataStorage;
using MetadataStore;


namespace CustomProvider
{
    /// <summary>
    /// Custom Synchronization Provider with custom metadata store
    /// </summary>
    public class MySyncProviderWithCustomMetadataStore : KnowledgeSyncProvider, IChangeDataRetriever, INotifyingChangeApplierTarget, IDisposable 
    {

        #region Local variables
        CustomMetadataStore _customMetadataStore = null; 
        SyncSessionContext _currentSessionContext = null;
        string _replicaName;
        //Connection string for the replica which is used by the provider to talk to the replica
        string _replicaCon;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates or opens the existing metadata store.
        /// Initializes the Replica Id and replica connection
        /// </summary>
        /// <param name="replicaName"></param>
        /// <param name="replicaCon"></param>
        /// <param name="fileName"></param>
        public MySyncProviderWithCustomMetadataStore(string replicaName, string replicaCon)
        {          
            _replicaName = replicaName;
            _replicaCon = replicaCon;

            SyncIdFormatGroup idFormats = new SyncIdFormatGroup();
            idFormats.ItemIdFormat.IsVariableLength = true;
            idFormats.ItemIdFormat.Length = 500;
            idFormats.ReplicaIdFormat.IsVariableLength = true;
            idFormats.ReplicaIdFormat.Length = 500;

            _customMetadataStore = new CustomMetadataStore(_replicaName, new SyncId(replicaName),idFormats);         
        }

        #endregion

        #region Replica Operation methods

        /// <summary>
        /// Creates a new cutomer and updates the metadata
        /// </summary>
        /// <param name="customer"></param>
        public void CreateNewItem(Customer customer)
        {
            //Creates the Customer
            customer.CreateCustomer();

            ulong currentLocalTickCount = GetNextTickCount();
            CustomItemMetadata item = new CustomItemMetadata();
            item.Uri = customer.ID.ToString();
            item.IsTombstone = false; // Not a tombstone, since alive
            item.ItemId = new SyncId(customer.ID.ToString());
            // Record the create version.
            // Local replica key is always 0.
            item.CreationVersion = new SyncVersion(0, currentLocalTickCount);
            // For a newly created version, ChangeVersion = CreationVersion.
            item.ChangeVersion = item.CreationVersion;
            // Flush the updated metadata to disk.
            _customMetadataStore.SaveItemMetadata(item);
        }

        /// <summary>
        /// BeginUpdates() begins a transaction for the replica.Using BeginUpdates and EndUpdates the operations on the replica can be made transactional.
        /// </summary>
        public void BeginUpdates()
        {
            //
        }

        /// <summary>
        /// EndUpdates ends a transaction for the replica
        /// </summary>
        public void EndUpdates()
        {
            //
        }

        /// <summary>
        /// Updates a customer and the metadata
        /// </summary>
        /// <param name="customer"></param>
        public void UpdateItem(Customer customer)
        {
            customer.Update();
            // Lookup the item in the metadata store.
            CustomItemMetadata existingItem;
            _customMetadataStore.TryGetItem(customer.ID.ToString(), out existingItem);
            if (null == existingItem)
            {
                throw new System.Exception("Item to be updated is not present in metadata store");
            }
            // Update the change version.
            existingItem.ChangeVersion = new SyncVersion(0, GetNextTickCount());          
            // Flush the updated metadata to disk.
            _customMetadataStore.SaveItemMetadata(existingItem);
        }

        /// <summary>
        /// Deletes a customer and updates the metadata
        /// </summary>
        /// <param name="customer"></param>
        public void DeleteItem(Customer customer)
        {
            customer.Delete();
            // Lookup the item in the metadata store.
            CustomItemMetadata existingItem;
            _customMetadataStore.TryGetItem(customer.ID.ToString(), out existingItem);
            if (null == existingItem)
            {
                throw new System.Exception("Item to be Deleted is not present in metadata store");
            }

            // Check is already recorded as a tombstone.
            if (!existingItem.IsTombstone)
            {
                // Not recorded.
                // This is a new delete.
                existingItem.IsTombstone = true;
                // Record the version.
                existingItem.ChangeVersion = new SyncVersion(0, GetNextTickCount());               
            }
            // Flush the updated metadata to disk.
            _customMetadataStore.SaveItemMetadata(existingItem);
        }       

        #endregion

        #region Overriden Provider Methods

        public override void BeginSession(SyncProviderPosition position, SyncSessionContext syncSessionContext)
        {          
            _currentSessionContext = syncSessionContext;          
        }

        public override void EndSession(SyncSessionContext syncSessionContext)
        {
            //throw new NotImplementedException();        
        }

        public ulong GetNextTickCount()
        {
            return _customMetadataStore.GetNextTickCount();
        }
        public override FullEnumerationChangeBatch GetFullEnumerationChangeBatch(uint batchSize, SyncId lowerEnumerationBound, SyncKnowledge knowledgeForDataRetrieval, out object changeDataRetriever)
        {
            throw new NotImplementedException();
        }

        public override SyncIdFormatGroup IdFormats
        {
            get { return _customMetadataStore.IdFormats; }
        }

        public override void ProcessFullEnumerationChangeBatch(ConflictResolutionPolicy resolutionPolicy, FullEnumerationChangeBatch sourceChanges, object changeDataRetriever, SyncCallbacks syncCallback, SyncSessionStatistics sessionStatistics)
        {
            throw new NotImplementedException();
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
           if (_customMetadataStore.MyKnowledge == null)
            {
                throw new InvalidOperationException("Knowledge not yet loaded.");
            }
           _customMetadataStore.MyKnowledge.SetLocalTickCount(_customMetadataStore.TickCount);
           batchSize = _customMetadataStore.RequestedBatchSize;
            knowledge = _customMetadataStore.MyKnowledge.Clone();
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
            // Get local changes.
            List<ItemChange> changes = DetectChanges(destinationKnowledge, batchSize);

            // Update the knowledge with an updated local tick count.
            _customMetadataStore.MyKnowledge.SetLocalTickCount(_customMetadataStore.TickCount);

            ChangeBatch changeBatch = new ChangeBatch(IdFormats, destinationKnowledge, _customMetadataStore.MyForgottenKnowledge);
            // Add the changes to the ChangeBatch with our made with knowledge
            // (Made width knowledge is the knowledge the other side will "learn" if they apply these
            // changes successfully)
            changeBatch.BeginUnorderedGroup();
            changeBatch.AddChanges(changes);

            // If last change batch, mark accordingly
            // (We always enumerate full batches, so if our batch is less than the batch size we
            // must be at the last batch. The second condition is spurious.)

            if ((changes.Count < batchSize) || (changes.Count == 0))
            {
                changeBatch.SetLastBatch();
            }
            changeBatch.EndUnorderedGroup(_customMetadataStore.MyKnowledge, changeBatch.IsLastBatch);


            changeDataRetriever = this;
            // Construct the ChangeBatch and return it
            return changeBatch;
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

            // Increment the tick count.
            ulong tickCount = GetNextTickCount();
            // Increase local knowledge tick count.
            _customMetadataStore.MyKnowledge.SetLocalTickCount(tickCount);

            // We will build a parallel change batch to the one the source sent us
            // We populate our change batch with the corresponding local versions 
            // for the changes we received. The engine will use this information 
            // to detect conflicts when applying changes.

            // Create a collection to hold the changes we'll put into our batch

            List<ItemChange> localVersions = new List<ItemChange>();
            // Iterate over changes in the source ChangeBatch
            foreach (ItemChange ic in sourceChanges)
            {
                CustomItemMetadata item;
                ItemChange change;

                // Iterate through each item to get the corresponding version in the local store

                if (_customMetadataStore.TryGetItem(ic.ItemId, out item))
                {
                    // Found the corresponding item in the local metadata

                    // Get the local creation version and change (update) version from the metadata 

                    //change = new ItemChange(IdFormats, _customMetadataStore.ReplicaId, item.ItemId, item.CreationVersion, item.ChangeVersion);

                    //// If local item is a tombstone, mark it accordingly

                    //if (item.IsTombstone)
                    //    change.ChangeKind = ChangeKind.Deleted;



                    if (item.IsTombstone)
                    {
                        change = new ItemChange(IdFormats, _customMetadataStore.ReplicaId, item.ItemId, ChangeKind.Deleted, item.CreationVersion, item.ChangeVersion);
                    }
                    else
                    {
                        change = new ItemChange(IdFormats, _customMetadataStore.ReplicaId, item.ItemId, ChangeKind.Update, item.CreationVersion, item.ChangeVersion);
                    }

                }
                else
                {
                    // Remote item has no local counterpart
                    // This item is unknown to us

                    //change = new ItemChange(IdFormats, _customMetadataStore.ReplicaId, ic.ItemId, SyncVersion.UnknownVersion, SyncVersion.UnknownVersion);
                    change = new ItemChange(IdFormats, _customMetadataStore.ReplicaId, ic.ItemId,ChangeKind.UnknownItem, SyncVersion.UnknownVersion, SyncVersion.UnknownVersion);

                    // Mark the change as unknown

                   // change.ChangeKind = ChangeKind.UnknownItem;
                }

                // Add our change to the change list

                localVersions.Add(change);
            }
            // Construct our change batch

            // Now we call the change applier
            // The change applier will compare the local and remote versions, apply 
            // non-conflicting changes, and will also detect conflicts and react as specified

            ForgottenKnowledge destinationForgottenKnowledge = new ForgottenKnowledge(IdFormats, _customMetadataStore.MyKnowledge);

            NotifyingChangeApplier changeApplier = new NotifyingChangeApplier(IdFormats);

            changeApplier.ApplyChanges(resolutionPolicy, sourceChanges,
                changeDataRetriever as IChangeDataRetriever, localVersions,
                _customMetadataStore.MyKnowledge.Clone(),
                destinationForgottenKnowledge, this, _currentSessionContext, syncCallback);

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
            CustomItemMetadata item;
            // Retrieve metadata for the changed item.
            _customMetadataStore.TryGetItem(loadChangeContext.ItemChange.ItemId, out item);

            // Make sure this isn't a delete.
            if (item.IsTombstone)
            {
                throw new InvalidOperationException("Cannot load change data for a deleted item.");
            }

            return Customer.GetCustomerById(int.Parse(ID), _replicaCon, _replicaName);
        }

        /// <summary>
        /// Saves the item and metadata in destination replica
        /// </summary>
        /// <param name="saveChangeAction">specifies what operation to perform on destination replica like create/update or delete</param>
        /// <param name="change">contains the information about the change to an item</param>
        /// <param name="context">represents information about a change to be saved to the item store</param>
        public void SaveItemChange(SaveChangeAction saveChangeAction, ItemChange change, SaveChangeContext context)
        {
            
            // Extract the item change.
            // Note: Since our provider doesn't use CU, it is guaranteed that only one
            // change type will be present on the SaveChangeContext.
            // Extract the data from the change.
            Customer customer = context.ChangeData as Customer;
            // Item metadata.
           CustomItemMetadata item;

            switch (saveChangeAction)
            {
                case SaveChangeAction.Create:
                    //Save the Change
                    customer.ReplicaCon = _replicaCon;
                    customer.ReplicaName = _replicaName;
                    customer.CreateCustomer();
                    //Save the metadata
                    item = new CustomItemMetadata();
                    item.Uri = customer.ID.ToString();
                    item.ItemId = change.ItemId;
                    item.CreationVersion = change.CreationVersion;
                    item.ChangeVersion = change.ChangeVersion;
                    _customMetadataStore.SaveItemMetadata(item);
                    break;

                case SaveChangeAction.UpdateVersionAndData:
                    {
                       // item = _metadata.FindItemMetadataById(change.ItemId);
                        if (!_customMetadataStore.TryGetItem(change.ItemId, out item))
                        {
                            throw new Exception("Record is not present in replica");
                        }
                        item.ChangeVersion = change.ChangeVersion;

                        Customer destCustomer = Customer.GetCustomerById(Int32.Parse(item.Uri), _replicaCon, _replicaName);                       
                        destCustomer.Name = customer.Name;
                        destCustomer.Designation = customer.Designation;
                        destCustomer.Age = customer.Age;
                        destCustomer.ReplicaCon = _replicaCon;
                        destCustomer.ReplicaName = _replicaName;
                        destCustomer.Update();

                        _customMetadataStore.SaveItemMetadata(item);
                    }
                    break;
                case SaveChangeAction.UpdateVersionOnly:
                    {
                        if (!_customMetadataStore.TryGetItem(change.ItemId, out item))
                        {
                            //throw new Exception("Record is not present in replica");
                            // Not found, must mean we've never seen this item locally.
                            // Can safely use an empty URI.
                            item = new CustomItemMetadata();
                            item.Uri = String.Empty;
                        }
                        item.ChangeVersion = change.ChangeVersion; 
                        _customMetadataStore.SaveItemMetadata(item);
                    }
                    break;

                case SaveChangeAction.DeleteAndStoreTombstone:
                    if (!_customMetadataStore.TryGetItem(change.ItemId, out item))
                    {
                        //throw new Exception("Record is not present in replica");
                        item = new MetadataStore.CustomItemMetadata();
                        item.ItemId = change.ItemId;
                        item.Uri = string.Empty;
                        item.CreationVersion = change.CreationVersion;
                    }
                    // If deletion change, mark it as a tombstone.
                    if ((change.ChangeKind & ChangeKind.Deleted) == ChangeKind.Deleted)
                    {
                        item.IsTombstone = true;
                    }
                    else
                    {
                        throw new Exception("Invalid changeType");
                    }
                    item.ChangeVersion = change.ChangeVersion;
                    if (item.Uri != string.Empty)
                    {

                        Customer customerDelete = Customer.GetCustomerById(Int32.Parse(item.Uri), _replicaCon, _replicaName);

                        if (null != customerDelete) customerDelete.Delete();
                    }

                    _customMetadataStore.SaveItemMetadata(item);
                    break;

                case SaveChangeAction.UpdateVersionAndMergeData:
                    //item = _metadata.FindItemMetadataById(change.ItemId);
                    //item.ChangeVersion = new SyncVersion(0, _metadata.GetNextTickCount());//********

                    if (!_customMetadataStore.TryGetItem(change.ItemId, out item))
                    {
                        throw new Exception("Record is not present in replica");
                    }
                    item.ChangeVersion = change.ChangeVersion;//********
                    //int ID = Int32.Parse(item.GlobalId.GetStringId());
                    int ID = Int32.Parse(item.Uri);
                    Customer destCustomerMerge = Customer.GetCustomerById(ID, _replicaCon, _replicaName);                    
                    destCustomerMerge.Name += customer.Name;
                    destCustomerMerge.Designation += customer.Designation;
                    destCustomerMerge.Age += customer.Age;
                    destCustomerMerge.ReplicaCon = _replicaCon;
                    destCustomerMerge.ReplicaName = _replicaName;
                    destCustomerMerge.Update();

                    _customMetadataStore.SaveItemMetadata(item);
                    break;
            }


            SyncKnowledge Knowledge = _customMetadataStore.MyKnowledge;
            ForgottenKnowledge MyForgottenKnowledge = _customMetadataStore.MyForgottenKnowledge;

            // If we made it here, the change was successfully applied locally
            // (or it is a version only change), so we can update our knowledge with the 
            // learned knowledge from the change.
            context.GetUpdatedDestinationKnowledge(out Knowledge, out MyForgottenKnowledge);
            _customMetadataStore.MyKnowledge = Knowledge;
            _customMetadataStore.MyForgottenKnowledge = MyForgottenKnowledge;
        }

        /// <summary>
        /// We also need to save the knowledge after each sync.
        /// We just save the knowledge and forgotten Knowledge in the Replica metadata store.
        /// </summary>
        /// <param name="knowledge"></param>
        /// <param name="forgottenKnowledge"></param>
        public void StoreKnowledgeForScope(SyncKnowledge knowledge, ForgottenKnowledge forgottenKnowledge)
        {
            _customMetadataStore.MyKnowledge = knowledge;
            _customMetadataStore.MyForgottenKnowledge = forgottenKnowledge;
            // Persist knowledge to disk.
            _customMetadataStore.SaveKnowledgeFile();
        }
        #endregion

        #region Destructor

        ~MySyncProviderWithCustomMetadataStore()
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
                    _customMetadataStore.ReleaseMetadataStore();
                    _customMetadataStore.Dispose();
                  
                }
                // Clean up unmanaged resources here.
            }
            disposed = true;
        }
        #endregion

        #region Private Helper methods    

        private List<ItemChange> DetectChanges(SyncKnowledge destinationKnowledge, uint batchSize)
        {
            List<ItemChange> changeBatch = new List<ItemChange>();
            if (destinationKnowledge == null)
            {
                throw new ArgumentNullException("destinationKnowledge");
            }
            if (batchSize < 0)
            {
                throw new ArgumentOutOfRangeException("batchSize");
            }
            ulong currentLocalTickCount = _customMetadataStore.TickCount;

            // Update local knowledge with the current local tick count.
            _customMetadataStore.MyKnowledge.SetLocalTickCount(currentLocalTickCount);

            // Map the destination knowledge.
            // This maps the knowledge from the remote replica key map (where the destination is replicaKey 0)
            // to the local replica key map (where the source is replicaKey).
            //
            // We do this because our metadata is relative to the local store (and local key map)
            // (This is typical of most sync providers).
            SyncKnowledge mappedKnowledge = _customMetadataStore.MyKnowledge.MapRemoteKnowledgeToLocal(destinationKnowledge);

            foreach (CustomItemMetadata item in _customMetadataStore)
            {
                // Check if the current version of the item is known to the destination
                // We simply check if the update version is contained in his knowledge.

                // If the metadata is for a tombstone, the change is a delete.
                if (!mappedKnowledge.Contains(_customMetadataStore.ReplicaId, item.ItemId, item.ChangeVersion))
                {
                    ItemChange itemChange = null;
                    // ItemChange itemChange = new ItemChange(IdFormats, _customMetadataStore.ReplicaId, item.ItemId, item.CreationVersion, item.ChangeVersion);

                    if (item.IsTombstone)
                    {
                        //itemChange.ChangeKind = ChangeKind.Deleted;

                        itemChange = new ItemChange(IdFormats, _customMetadataStore.ReplicaId, item.ItemId, ChangeKind.Deleted, item.CreationVersion, item.ChangeVersion);
                    }
                    else
                    {
                        itemChange = new ItemChange(IdFormats, _customMetadataStore.ReplicaId, item.ItemId, ChangeKind.Update, item.CreationVersion, item.ChangeVersion);
                    }
                    // Change isn't known to the remote store, so add it to the batch.
                    changeBatch.Add(itemChange);
                }

                // If the batch is full, break
                //
                // N.B. Rest of changes will be detected in next batch. Current batch will not be
                // re-enumerated (except in case destination doesn't successfully apply them) as
                // when destination applies the changes in this batch, they will add them to their
                // knowledge.
                if (changeBatch.Count == batchSize)
                {
                    break;
                }
            }
            return changeBatch;
        }    
       
        #endregion     

    }
}
