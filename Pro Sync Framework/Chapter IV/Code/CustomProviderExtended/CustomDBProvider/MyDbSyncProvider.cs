using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseSyncProvider;

namespace CustomProvider
{
    public class MyDbSyncProvider : MyBaseSyncProvider<Customer>
    {


        string _replicaCon;

        public MyDbSyncProvider(string replicaName, string replicaCon, string metadataFileName)
            : base(replicaName, metadataFileName)
        {
            _replicaCon = replicaCon;
        }       


        // Abstract methods that need to be overridden in the derived classes.
        // These provide the functionality specific to each actual data store.
        public  override void CreateItemData(Customer itemData)
        {
            itemData.ReplicaCon = _replicaCon;
            itemData.Create();
        }
        public override void UpdateItemData(Customer itemData)
        {
            itemData.ReplicaCon = _replicaCon;
            itemData.Update();
        }

        public override void DeleteItemData(Customer itemData)
        {
            itemData.ReplicaCon = _replicaCon;
            itemData.Delete();
        }
        public override void UpdateItemDataWithDestination(Customer sourceItemData, Customer destItemData)
        {
            destItemData.Name = sourceItemData.Name;
            destItemData.Designation = sourceItemData.Designation;
            destItemData.Age = sourceItemData.Age;
            destItemData.ReplicaCon = _replicaCon;
            destItemData.Update();
        }

        public override void MergeItemData(Customer sourceItemData, Customer destItemData)
        {
            destItemData.Name += sourceItemData.Name;
            destItemData.Designation += sourceItemData.Designation;
            destItemData.Age += sourceItemData.Age;
            destItemData.ReplicaCon = _replicaCon;
            destItemData.Update();
        }
        public override Customer GetItemData(string itemDataId, string replicaName)
        {
           return Customer.GetCustomerById(int.Parse(itemDataId), _replicaCon, replicaName);
        }

        public override Dictionary<string, DateTime> ItemDataIds
        {
            get
            {
                return Customer.GetUpdatedCustomersId(_replicaCon);
            }
        }
    }
}
