using Microsoft.Synchronization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MetadataStore
{
    [Serializable()]
    public class CustomItemMetadata
    {
        private SyncId itemId = null;
        private SyncVersion creationVersion = null;
        private SyncVersion changeVersion = null;
        private string uri = null;
        private bool isTombstone = false;

        public SyncId ItemId
        {
            get
            {
                if (itemId == null)
                {
                    throw new InvalidOperationException("ItemId not yet set.");
                }

                return itemId;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "ItemId cannot be null.");
                }

                itemId = value;
            }
        }

        public SyncVersion CreationVersion
        {
            get { return creationVersion; }
            set { creationVersion = value; }
        }

        public SyncVersion ChangeVersion
        {
            get { return changeVersion; }
            set { changeVersion = value; }
        }

        public string Uri
        {
            get
            {
                if (uri == null)
                {
                    throw new InvalidOperationException("Uri not yet set.");
                }

                return uri;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Uri cannot be null.");

                uri = value;
            }
        }

        public bool IsTombstone
        {
            get { return isTombstone; }
            set { isTombstone = value; }
        }

        /// <summary>
        /// Returns a clone of the ItemMetadata instance
        /// </summary>
        /// <returns></returns>
        public CustomItemMetadata Clone()
        {
            return (CustomItemMetadata)this.MemberwiseClone();
        }
    }
}
