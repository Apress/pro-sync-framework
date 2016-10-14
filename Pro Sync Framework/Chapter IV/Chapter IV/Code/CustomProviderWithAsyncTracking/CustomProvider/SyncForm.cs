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
    public partial class SyncForm : Form
    {

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public SyncForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Local variables

        string connection = "Server=localhost;Integrated security=SSPI;database={0}";
        string querry = "Select ID,Name,Age,Designation from Customer order by ID";
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
        MySyncProvider providerA;
        MySyncProvider providerB;
        MySyncProvider providerC;

        SyncOrchestrator agent;
        SyncOperationStatistics stats;



        //Metadata store location for the 3 providers we'll be using
        string providerAMetadata;
        string providerBMetadata;
        string providerCMetadata;

        #endregion

        #region Form events

        private void SyncForm_Load(object sender, EventArgs e)
        {
            //In our example we are storing metadata in a file 

            //Create provider and metadata for replica A
            providerA_ReplicaCon = string.Format(connection, replicaA);
            providerAMetadata = Environment.CurrentDirectory + "\\" + replicaA + " .Metadata";
            providerA = new MySyncProvider(replicaA, providerA_ReplicaCon, providerAMetadata);

            //Create provider and metadata for replica B
            providerB_ReplicaCon = string.Format(connection, replicaB);
            providerBMetadata = Environment.CurrentDirectory + "\\" + replicaB + " .Metadata";
            providerB = new MySyncProvider(replicaB, providerB_ReplicaCon, providerBMetadata);

            //Create provider and metadata for replica C
            providerC_ReplicaCon = string.Format(connection, replicaC);
            providerCMetadata = Environment.CurrentDirectory + "\\" + replicaC + " .Metadata";
            providerC = new MySyncProvider(replicaC, providerC_ReplicaCon, providerCMetadata);

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


            providerA.BeginUpdates();
            providerB.BeginUpdates();
            providerC.BeginUpdates();

            int NextId = GetNextId();
            c1 = new Customer(providerA_ReplicaCon, replicaA, NextId, "A_TestName1", "A_TestDesig1", 10, DateTime.Now, DateTime.Now);
            c1.Create();
            NextId += 1;
            c2 = new Customer(providerA_ReplicaCon, replicaA, NextId, "A_TestName2", "A_TestDesig2", 20, DateTime.Now, DateTime.Now);
            c2.Create();
            NextId += 1;
            c3 = new Customer(providerB_ReplicaCon, replicaB, NextId, "B_TestName1", "B_TestDesig1", 30, DateTime.Now, DateTime.Now);
            c3.Create();
            NextId += 1;
            c4 = new Customer(providerB_ReplicaCon, replicaB, NextId, "B_TestName2", "B_TestDesig2", 40, DateTime.Now, DateTime.Now);
            c4.Create();
            NextId += 1;
            c5 = new Customer(providerC_ReplicaCon, replicaC, NextId, "C_TestName1", "C_TestDesig1", 50, DateTime.Now, DateTime.Now);
            c5.Create();
            NextId += 1;
            c6 = new Customer(providerC_ReplicaCon, replicaC, NextId, "C_TestName2", "C_TestDesig2", 60, DateTime.Now, DateTime.Now);
            c6.Create();


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
            c1.DateModified = DateTime.Now;
            c1.Update();

            Customer c7 = Customer.GetCustomerById(c1.ID, providerB_ReplicaCon, replicaB);
            c7.Name = "UpdateB";
            c7.DateModified = DateTime.Now;
            c7.Update();


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
            c4.Delete();
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
            e.SetResolutionAction(ConflictResolutionAction.SourceWins);
        }

        private void SyncForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            providerA.Dispose();
            providerB.Dispose();
            providerC.Dispose();
        }

        #endregion

        #region Private helper methods

        private void Synchronize(MySyncProvider sourceProvider, MySyncProvider destProvider)
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
               //TODO : Eception Hnadling here
                throw ex;
            }
        }

        private int GetNextId()
        {

            int id1 = Customer.GetNextId(providerA_ReplicaCon);
            if (id1 == 1) return 100;//Known Issue in CTP2 http://forums.microsoft.com/sync/ShowPost.aspx?PostID=3547043&SiteID=75
            int id2 = Customer.GetNextId(providerB_ReplicaCon);
            if (id2 > id1)
            {
                id1 = id2;
            }
            int id3 = Customer.GetNextId(providerB_ReplicaCon);
            if (id3 > id1)
            {
                return id3;
            }
            else
            {
                return id1;
            }

        }

        #endregion

    }
}
