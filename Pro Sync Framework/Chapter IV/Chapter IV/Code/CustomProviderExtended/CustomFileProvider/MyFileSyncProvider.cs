using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseSyncProvider;
namespace CustomFileProvider
{
    public class MyFileSyncProvider : MyBaseSyncProvider<MyTextFile>
    {


        string _folderPath;
        public MyFileSyncProvider(string replicaName, string folderPath, string metadataFileName)
            : base(replicaName, metadataFileName)
        {
            _folderPath = folderPath;
            
        }


        // Abstract methods that need to be overridden in the derived classes.
        // These provide the functionality specific to each actual data store.
        public override void CreateItemData(MyTextFile itemData)
        {
            itemData.FolderPath = _folderPath;
            itemData.Create();
        }

        public override void UpdateItemData(MyTextFile itemData)
        {
            itemData.FolderPath = _folderPath;
            itemData.Update();
        }

        public override void DeleteItemData(MyTextFile itemData)
        {
            itemData.FolderPath = _folderPath;
            itemData.Delete();
        }

        public override void UpdateItemDataWithDestination(MyTextFile sourceItemData, MyTextFile destItemData)
        {           
           
            destItemData.Text = sourceItemData.Text;
            destItemData.FolderPath = _folderPath;
            destItemData.Update();
        }

        public override void MergeItemData(MyTextFile sourceItemData, MyTextFile destItemData)
        {
            destItemData.Text += sourceItemData.Text;
            destItemData.FolderPath = _folderPath;
            destItemData.Update();
        }
        public override MyTextFile GetItemData(string itemDataId, string replicaName)
        {
            return MyTextFile.GetFileByFileName(itemDataId, _folderPath);
        }

        public override Dictionary<string, DateTime> ItemDataIds
        {
            get
            {
                return MyTextFile.GetUpdatedFileNames(_folderPath);
            }
        }
    }
}
