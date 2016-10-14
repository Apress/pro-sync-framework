using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Synchronization;

namespace CustomProvider
{
    /// <summary>
    /// Synchornizing application
    /// </summary>
    public partial class SyncFormWithCustomMetadataStore : Form
    {

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public SyncFormWithCustomMetadataStore()
        {
            InitializeComponent();
        }

        #endregion

        #region Local variables

        string connection = "Server=localhost;Integrated security=SSPI;database={0}";
        string querry = "Select * from Customer order by ID";
        string replicaA = "ReplicaA";
        string replicaB = "ReplicaB";
        string replicaC = "ReplicaC";

        Customer c1, c2, c3, c4, c5, c6;

        //Connection string for the DB
        string providerA_ReplicaCon;
        string providerB_ReplicaCon;
        string providerC_ReplicaCon;

        Random random;

        //Sync providers
        MySyncProviderWithCustomMetadataStore providerA;
        MySyncProviderWithCustomMetadataStore providerB;
        MySyncProviderWithCustomMetadataStore providerC;

        SyncOrchestrator agent;
        SyncOperationStatistics stats;
        #endregion

        #region Form events

        private void SyncFormWithCustomMetadataStore_Load(object sender, EventArgs e)
        {

            //In our example we are storing metadata in a file 
            //Create provider for replica B
            providerA_ReplicaCon = string.Format(connection, replicaA);
            providerA = new MySyncProviderWithCustomMetadataStore(replicaA, providerA_ReplicaCon);

            //Create provider for replica B
            providerB_ReplicaCon = string.Format(connection, replicaB);
            providerB = new MySyncProviderWithCustomMetadataStore(replicaB, providerB_ReplicaCon);

            //Create provider for replica C
            providerC_ReplicaCon = string.Format(connection, replicaC);
            providerC = new MySyncProviderWithCustomMetadataStore(replicaC, providerC_ReplicaCon);

            //Define the conflict resolution policy
            providerA.Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.ApplicationDefined;
            providerB.Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.ApplicationDefined;
            providerC.Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.ApplicationDefined;

            //Raise the event to handle conflicts
            providerA.DestinationCallbacks.ItemConflicting += new EventHandler<ItemConflictingEventArgs>(DestinationCallbacks_ItemConflicting);
            providerB.DestinationCallbacks.ItemConflicting += new EventHandler<ItemConflictingEventArgs>(DestinationCallbacks_ItemConflicting);
            providerC.DestinationCallbacks.ItemConflicting += new EventHandler<ItemConflictingEventArgs>(DestinationCallbacks_ItemConflicting);

            //create new instance of sync agent
            agent = new SyncOrchestrator();

            //Random class to generate Id for the records
            random = new Random(1);

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {

            //Begin transactions
            providerA.BeginUpdates();
            providerB.BeginUpdates();
            providerC.BeginUpdates();

            c1 = new Customer(providerA_ReplicaCon, replicaA, random.Next(1000), "A_TestName1", "A_TestDesig1", 10);
            providerA.CreateNewItem(c1);
            c2 = new Customer(providerA_ReplicaCon, replicaA, random.Next(1000), "A_TestName2", "A_TestDesig2", 20);
            providerA.CreateNewItem(c2);

            c3 = new Customer(providerB_ReplicaCon, replicaB, random.Next(1000), "B_TestName1", "B_TestDesig1", 30);
            providerB.CreateNewItem(c3);
            c4 = new Customer(providerB_ReplicaCon, replicaB, random.Next(1000), "B_TestName2", "B_TestDesig2", 40);
            providerB.CreateNewItem(c4);

            c5 = new Customer(providerC_ReplicaCon, replicaC, random.Next(1000), "C_TestName1", "C_TestDesig1", 50);
            providerC.CreateNewItem(c5);
            c6 = new Customer(providerC_ReplicaCon, replicaC, random.Next(1000), "C_TestName2", "C_TestDesig2", 60);
            providerC.CreateNewItem(c6);


            //End transactions
            providerA.EndUpdates();
            providerB.EndUpdates();
            providerC.EndUpdates();

            replicaContentIntial.Refresh(providerA_ReplicaCon, providerB_ReplicaCon, providerC_ReplicaCon, querry);
            btnCreate.Enabled = false;
        }

        private void btnSynchronize_AB_Click(object sender, EventArgs e)
        {
            Synchronize(providerA, providerB);
            replicaContentSyncAB.Refresh(providerA_ReplicaCon, providerB_ReplicaCon, providerC_ReplicaCon, querry);
            btnSynchronize_AB.Enabled = false;
        }

        private void btnCreateConflict_Click(object sender, EventArgs e)
        {

            providerA.BeginUpdates();
            providerB.BeginUpdates();

            c1.Name = "UpdateA";
            providerA.UpdateItem(c1);

            Customer c7 = Customer.GetCustomerById(c1.ID, providerB_ReplicaCon, replicaB);
            c7.Name = "UpdateB";
            providerB.UpdateItem(c7);


            providerA.EndUpdates();
            providerB.EndUpdates();

            replicaContentConflict.Refresh(providerA_ReplicaCon, providerB_ReplicaCon, providerC_ReplicaCon, querry);
            btnCreateConflict.Enabled = false;
        }

        private void btnSyncConflicts_Click(object sender, EventArgs e)
        {
            Synchronize(providerA, providerB);
            replicaContentHandleConflicts.Refresh(providerA_ReplicaCon, providerB_ReplicaCon, providerC_ReplicaCon, querry);
            btnSyncConflicts.Enabled = false;
        }

        private void btnSyncDeletes_Click(object sender, EventArgs e)
        {
            providerB.BeginUpdates();
            providerB.DeleteItem(c4);
            providerB.EndUpdates();


            Synchronize(providerB, providerC);

            replicaContentHandleDeletes.Refresh(providerA_ReplicaCon, providerB_ReplicaCon, providerC_ReplicaCon, querry);
            btnSyncDeletes.Enabled = false;
        }

        private void btnSyncFinish_Click(object sender, EventArgs e)
        {
            Synchronize(providerA, providerC);
            replicaContentFinishSync.Refresh(providerA_ReplicaCon, providerB_ReplicaCon, providerC_ReplicaCon, querry);
            btnSyncFinish.Enabled = false;
        }

        static void DestinationCallbacks_ItemConflicting(object sender, ItemConflictingEventArgs e)
        {
            e.SetResolutionAction(ConflictResolutionAction.Merge);
        }

        private void SyncFormWithCustomMetadataStore_FormClosed(object sender, FormClosedEventArgs e)
        {
            providerA.Dispose();
            providerB.Dispose();
            providerC.Dispose();
        }

        #endregion

        #region Private helper methods

        private void Synchronize(MySyncProviderWithCustomMetadataStore sourceProvider, MySyncProviderWithCustomMetadataStore destProvider)
        {
            try
            {
                agent.Direction = SyncDirectionOrder.DownloadAndUpload;
                agent.LocalProvider = sourceProvider;
                agent.RemoteProvider = destProvider;
                stats = agent.Synchronize();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
