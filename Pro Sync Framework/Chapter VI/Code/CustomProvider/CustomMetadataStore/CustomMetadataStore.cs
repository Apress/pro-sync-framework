using Microsoft.Synchronization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MetadataStore
{
    public class CustomMetadataStore : IEnumerable<CustomItemMetadata>, IDisposable
    {

        #region Local variables


        private List<CustomItemMetadata> _data;
        private string _metadataDir = @"C:\CustomMetadata";

        private FileStream _itemMetadataFileStream = null;
        private const string _itemMetadataFileName = "ItemMetadata.msf";
        private string _itemMetadataFilePath;

        private FileStream _replicaIdFileStream = null;
        private const string _replicaIdFileName = "ReplicaId.msf";
        private string _replicaIdFilePath;
        private SyncId _replicaId = null;

        private const string _knowledgeFileName = "Knowledge.msf";
        private string _knowledgeFilePath;
        private FileStream _knowledgeFileStream = null;
        private SyncKnowledge _myKnowledge;
        private ForgottenKnowledge _myForgottenKnowledge;

        private const string _tickCountFileName = "TickCount.msf";
        private string _tickCountFilePath;
        private ulong _tickCount = 1;
        private FileStream _tickCountFileStream = null;


        //Default Batch Size
        private uint _requestedBatchSize = 100;
        private SyncIdFormatGroup _idFormats = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Directory where metadata is stored
        /// </summary>
        public virtual string MetadataDir
        {
            get { return _metadataDir; }
        }

        /// <summary>
        /// Gets or sets the file path of metadata
        /// </summary>
        public string ItemMetadataFilePath
        {
            get { return _itemMetadataFilePath; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "ItemMetadataFilePath cannot be null.");
                }

                if (value == String.Empty)
                {
                    throw new ArgumentOutOfRangeException("value", "ItemMetadataFilePath cannot be the empty string.");
                }
                _itemMetadataFilePath = value;
            }
        }

        /// <summary>
        /// Gets or sets the file path which is used to store the replica ID
        /// </summary>
        public string ReplicaIdFilePath
        {
            get { return _replicaIdFilePath; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "ReplicaIdFilePath cannot be null.");
                }

                if (value == String.Empty)
                {
                    throw new ArgumentOutOfRangeException("value", "ReplicaIdFilePath cannot be the empty string.");
                }

                _replicaIdFilePath = value;
            }
        }

        /// <summary>
        /// Gets or sets the replica ID
        /// </summary>
        public SyncId ReplicaId
        {
            get { return _replicaId; }
            set { _replicaId = value; }
        }

        /// <summary>
        /// Gets or sets the file path which is used to store the Tick Count
        /// </summary>
        public string TickCountFilePath
        {
            get { return _tickCountFilePath; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "TickCountFilePath cannot be null.");
                }

                if (value == String.Empty)
                {
                    throw new ArgumentOutOfRangeException("value", "TickCountFilePath cannot be the empty string.");
                }

                _tickCountFilePath = value;
            }
        }

        /// <summary>
        /// Gets the Tick Count
        /// </summary>
        public ulong TickCount
        {
            get { return _tickCount; }
        }

        /// <summary>
        /// Gets or sets the file path which is used to store the Knowledge
        /// </summary>
        public string KnowledgeFilePath
        {
            get { return _knowledgeFilePath; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "KnowledgeFilePath cannot be null.");
                }
                if (value == String.Empty)
                {
                    throw new ArgumentOutOfRangeException("value", "KnowledgeFilePath cannot be the empty string.");
                }
                _knowledgeFilePath = value;
            }
        }

        /// <summary>
        /// Current Knowledge
        /// </summary>
        public SyncKnowledge MyKnowledge
        {
            get
            {
                return _myKnowledge;
            }
            set
            {
                _myKnowledge = value;
            }
        }

        /// <summary>
        /// Forgotten Knowledge
        /// </summary>
        public ForgottenKnowledge MyForgottenKnowledge
        {
            get
            {
                return _myForgottenKnowledge;
            }
            set
            {
                _myForgottenKnowledge = value;
            }
        }

        /// <summary>
        /// Gets or sets the Batch size
        /// </summary>
        public uint RequestedBatchSize
        {
            get { return _requestedBatchSize; }
            set { _requestedBatchSize = value; }
        }

        /// <summary>
        /// ID format for the metadata store
        /// </summary>
        public SyncIdFormatGroup IdFormats
        {
            get
            {
                return _idFormats;
            }
            set
            {
                _idFormats = value;
            }
        }


        #endregion

        #region Constructor
        /// <summary>
        /// Creates and intializes the metadata store
        /// </summary>
        /// <param name="filename">Location of the Metadata</param>
        /// <param name="replicaID">Id of the replica</param>
        /// <param name="idFormats">ID formats for replica na ditem</param>
        public CustomMetadataStore(string filename, SyncId replicaID, SyncIdFormatGroup idFormats)
        {
            _replicaId = replicaID;
            _idFormats = idFormats;
            CreateMetadataStore(filename);
            IntializeSyncMetadata();
        }
        #endregion

        #region Item Metadata Management
        /// <summary>
        /// Creates the Item metadata store
        /// </summary>
        private void CreateItemMetadataInStore()
        {
            // Create data list
            _data = new List<CustomItemMetadata>();
            // Write out the new metadata store instance 
            // Clear out any existing contents.
            _itemMetadataFileStream.SetLength(0);

            // Serialize the data into the file
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(_itemMetadataFileStream, _data);
            _itemMetadataFileStream.Flush();
        }

        /// <summary>
        /// Loads a metadata store from disk
        /// </summary>
        private void LoadItemMetadataFromStore()
        {
            // Reset stream to the beginning
            _itemMetadataFileStream.Seek(0, SeekOrigin.Begin);  
            // Deserialize the data from the file
            BinaryFormatter bf = new BinaryFormatter();
            _data = bf.Deserialize(_itemMetadataFileStream) as List<CustomItemMetadata>;
        }

        /// <summary>
        /// Saves the item metadata to disk.
        /// </summary>
        /// <param name="itemMetadata">Item to save</param>
        public void SaveItemMetadata(CustomItemMetadata itemMetadata)
        {
            int index = _data.FindIndex(delegate(CustomItemMetadata compareItem) { return (compareItem.ItemId == itemMetadata.ItemId); });
            if (index >= 0)
            {
                _data[index] = itemMetadata.Clone();
            }
            else
            {
                _data.Add(itemMetadata.Clone());
            }
            // Clear out any existing contents.
            _itemMetadataFileStream.SetLength(0);
            // Serialize the data into the file
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(_itemMetadataFileStream, _data);
            _itemMetadataFileStream.Flush();
        }

        /// <summary>
        /// Gets the Item Metadata by URI of the item
        /// </summary>
        /// <param name="uri">URI of the Item </param>
        /// <param name="item">Item Metadata</param>
        /// <returns>True if found, else false</returns>
        public bool TryGetItem(string uri, out CustomItemMetadata item)
        {
            CustomItemMetadata im = _data.Find(delegate(CustomItemMetadata compareItem) { return (compareItem.Uri == uri && compareItem.IsTombstone == false); });
            if (im == null)
            {
                item = null;
                return false;
            }
            item = im.Clone();
            return true;
        }

        /// <summary>
        /// Gets the Item Metadata by ID of the item
        /// </summary>
        /// <param name="id">ID of the Item </param>
        /// <param name="item">Item Metadata</param>
        /// <returns>True if found, else false</returns>
        public bool TryGetItem(SyncId itemId, out CustomItemMetadata item)
        {
            CustomItemMetadata im = _data.Find(delegate(CustomItemMetadata compareItem) { return (compareItem.ItemId == itemId); });
            if (im == null)
            {
                item = null;
                return false;
            }
            item = im.Clone();
            return true;
        }

        #endregion

        #region Replica ID  Management
        /// <summary>
        /// Loads the Replica ID
        /// </summary>
        /// <param name="stream"></param>
        private void LoadReplicaIDFile()
        {
            // Deserialize the replicaId from the file.
            _replicaIdFileStream.Seek(0, SeekOrigin.Begin);
            BinaryFormatter bf = new BinaryFormatter();
            _replicaId = new SyncId((bf.Deserialize(_replicaIdFileStream).ToString()));
        }

        /// <summary>
        /// Saves the Replica ID
        /// </summary>
        /// <param name="stream"></param>
        private void SaveReplicaIDFile()
        {
            // Serialize replica Id to the file.
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(_replicaIdFileStream, _replicaId.ToString());
            _replicaIdFileStream.Flush();
        }

        #endregion

        #region Tick Count Management
        /// <summary>
        /// Loads the tick count
        /// </summary>
        private void LoadTickCountFile()
        {
            // Read the tick count from the file.
            _tickCountFileStream.Seek(0, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(_tickCountFileStream);
            _tickCount = br.ReadUInt64();
        }

        /// <summary>
        /// Saves the Tick count
        /// </summary>
        private void SaveTickCountFile()
        {
            // Write the tick count to the file.
            _tickCountFileStream.SetLength(0);
            BinaryWriter bw = new BinaryWriter(_tickCountFileStream);
            bw.Write(_tickCount);
        }

        /// <summary>
        /// Gets the next tick count and store the tick count
        /// </summary>
        /// <returns></returns>
        public ulong GetNextTickCount()
        {
            _tickCount++;
            SaveTickCountFile();
            return _tickCount;
        }

        #endregion

        #region knowledge Management
        /// <summary>
        /// Loads the knowledge
        /// </summary>
        private void LoadKnowledgeFile()
        {
            // Deserialize the knowledge from the file.
            _knowledgeFileStream.Seek(0, SeekOrigin.Begin);
            BinaryFormatter bf = new BinaryFormatter();
            _myKnowledge = (SyncKnowledge)bf.Deserialize(_knowledgeFileStream);

            //Check that knowledge should be for this replica.
            if (_myKnowledge.ReplicaId != _replicaId)
            {
                throw new Exception("Replica id of loaded knowledge doesn't match replica id provided in constructor.");
            }


            // Load the forgotten knowledge.
            _myForgottenKnowledge = (ForgottenKnowledge)bf.Deserialize(_knowledgeFileStream);

            // Check that knowledge should be for this replica.
            if (_myForgottenKnowledge.ReplicaId != _replicaId)
            {
                throw new Exception("Replica id of loaded forgotten knowledge doesn't match replica id provided in constructor.");
            }
        }

        /// <summary>
        /// Creates the knowledge
        /// </summary>
        private void CreateKnowledgeBlob()
        {
            _myKnowledge = new SyncKnowledge(_idFormats, _replicaId, _tickCount);
            _myForgottenKnowledge = new ForgottenKnowledge(IdFormats, _myKnowledge);
            SaveKnowledgeFile();
        }

        /// <summary>
        /// Saves the knowledge
        /// </summary>
        public void SaveKnowledgeFile()
        {
            // Serialize knowledge to the file.
            _knowledgeFileStream.SetLength(0);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(_knowledgeFileStream, _myKnowledge);
            bf.Serialize(_knowledgeFileStream, _myForgottenKnowledge);
        }

        #endregion

        #region Release Metadata Store 

        /// <summary>
        /// Releases the metadata store
        /// </summary>
        public void ReleaseMetadataStore()
        {
            if (_replicaIdFileStream != null)
            {
                _replicaIdFileStream.Close();
                _replicaIdFileStream = null;
            }
            if (_itemMetadataFileStream != null)
            {
                _itemMetadataFileStream.Close();
                _itemMetadataFileStream = null;
            }
            if (_tickCountFileStream != null)
            {
                _tickCountFileStream.Close();
                _tickCountFileStream = null;
            }

            if (_knowledgeFileStream != null)
            {
                _knowledgeFileStream.Close();
                _knowledgeFileStream = null;
            }
        }

        #endregion

        #region IEnumerable<CustomItemMetadata> Members

        public IEnumerator<CustomItemMetadata> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_data).GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //throw new NotImplementedException();           
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Intializes the metadata store.
        /// </summary>
        /// <param name="filename">Location for the metadata</param>
        private void CreateMetadataStore(string filename)
        {

            //Create parent directory          
            Directory.CreateDirectory(_metadataDir);
            //Create the directory for each replica
            _metadataDir = Path.Combine(_metadataDir, filename);
            //Clean up the Metadata Store
            if (Directory.Exists(_metadataDir))
            {
                Directory.Delete(_metadataDir, true);
            }
            Directory.CreateDirectory(_metadataDir);
            KnowledgeFilePath = Path.Combine(_metadataDir, _knowledgeFileName);
            ItemMetadataFilePath = Path.Combine(_metadataDir, _itemMetadataFileName);
            ReplicaIdFilePath = Path.Combine(_metadataDir, _replicaIdFileName);
            TickCountFilePath = Path.Combine(_metadataDir, _tickCountFileName);
        }

        /// <summary>
        /// Intializes the Sync Metadata
        /// </summary>
        private void IntializeSyncMetadata()
        {
            try
            {

                bool replicaIdFileAlreadyExists = File.Exists(ReplicaIdFilePath);
                _replicaIdFileStream = File.Open(ReplicaIdFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

                if (replicaIdFileAlreadyExists)
                {
                    LoadReplicaIDFile();
                }
                else
                {
                    SaveReplicaIDFile();
                }

                bool itemMetadataFileAlreadyExists = File.Exists(ItemMetadataFilePath);
                _itemMetadataFileStream = File.Open(ItemMetadataFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

                if (itemMetadataFileAlreadyExists)
                {
                    
                    LoadItemMetadataFromStore();
                }
                else
                {
                    CreateItemMetadataInStore();
                }

                bool tickCountFileAlreadyExists = File.Exists(TickCountFilePath);
                _tickCountFileStream = File.Open(TickCountFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

              
                if (tickCountFileAlreadyExists)
                {                    
                    LoadTickCountFile();
                }
                else
                {
                  SaveTickCountFile();
                }

                bool knowledgeFileAlreadyExists = File.Exists(KnowledgeFilePath);
                _knowledgeFileStream = File.Open(KnowledgeFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

                if (knowledgeFileAlreadyExists)
                {
                    LoadKnowledgeFile();
                }
                else
                {
                    CreateKnowledgeBlob();
                }

            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
