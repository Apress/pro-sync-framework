using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;
using System.IO;

namespace SyncApp_BuiltInProviders
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Guid sourceReplicaId;
        Guid destReplicaId;
       
        private void Form1_Load(object sender, EventArgs e)
        {
            //Assign Unique Guid's to the Replicas
            sourceReplicaId = GetReplicaGuid(@"C:\TestSync1\ReplicaID");
            destReplicaId = GetReplicaGuid(@"C:\TestSync2\ReplicaID");
        }

        /// <summary>
        /// Checks whether the file contaning replica id exists.If the file does not exists ,creates a new replica id,stores it in the path provided by GuidPath
        /// parameter and returns the replica id.
        /// If the file exists ,then returns the replica id stored in the file.
        /// </summary>
        /// <param name="GuidPath">string containing the path to store the replica Id's</param>
        /// <returns>Guid reresenting the Replica Id</returns>
        private Guid GetReplicaGuid(string GuidPath)
        {
            if (!File.Exists(GuidPath))
            {
                Guid replicaId = Guid.NewGuid();
                using (FileStream fs = File.Open(GuidPath, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(replicaId.ToString());
                }
                return replicaId;
            }

            else
            {
                using (FileStream fs = File.Open(GuidPath, FileMode.Open))
                using (StreamReader sr = new StreamReader(fs))
                {
                    return new Guid(sr.ReadLine());
                }
            }
        }

        private void btnSynchronize_Click(object sender, EventArgs e)
        {
            btnSynchronize.Enabled = false;

            //Create the Source and destination Sync provider.
            //Attach the source sync provider to C:\TestSync1 folder and assign it a a unique replica guid.
            FileSyncProvider sourceProvider = new FileSyncProvider(sourceReplicaId, @"C:\TestSync1");
            //Attach the destination sync provider to C:\TestSync2 folder and assign it a a unique replica guid.            
            FileSyncProvider destProvider = new FileSyncProvider(destReplicaId, @"C:\TestSync2");

            //syncAgent is the Sync Controller and it co-ordinates the sync session
            SyncOrchestrator syncAgent = new SyncOrchestrator();
            syncAgent.LocalProvider = sourceProvider;
            syncAgent.RemoteProvider = destProvider;
            syncAgent.Synchronize();

            label1.Text = "Synchronizing Finished...";
            btnSynchronize.Enabled = true;


        }     

        
    }
}
