using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Synchronization;

namespace CustomFileProvider
{
    public partial class FileSyncForm : Form
    {
        public FileSyncForm()
        {
            InitializeComponent();
        }


        #region Local variables

        MyFileSyncProvider sourceProvider;
        MyFileSyncProvider destinationProvider;

        SyncOrchestrator agent;
        SyncOperationStatistics stats;

        string sourceReplicaName = "ReplicaA";
        string destinationReplicaName = "ReplicaB";

        string sourceProviderMetadataPath;
        string destinationMetadataPath;

        #endregion

        private void FileSyncForm_Load(object sender, EventArgs e)
        {
            sourceProviderMetadataPath = Path.Combine(Environment.CurrentDirectory, sourceReplicaName + ".metadata");
            destinationMetadataPath = Path.Combine(Environment.CurrentDirectory, destinationReplicaName + ".metadata");

            //create new instance of sync agent
            agent = new SyncOrchestrator();
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            btnSync.Enabled = false;
            string sourceFolderPath = txtSource.Text;
            string destFolderPath = txtDestination.Text;

            if (string.IsNullOrEmpty(sourceFolderPath))
            {
                MessageBox.Show("Please Enter a value for Source folder");
                return;
            }

            if (string.IsNullOrEmpty(destFolderPath))
            {
                MessageBox.Show("Please Enter a value for Destination folder");
                return;
            }

            sourceProvider = new MyFileSyncProvider(sourceReplicaName, sourceFolderPath, sourceProviderMetadataPath);
            destinationProvider = new MyFileSyncProvider(destinationReplicaName, destFolderPath, destinationMetadataPath);
            Synchronize();
            btnSync.Enabled = true;
        }

        #region Private helper methods

        private void Synchronize()
        {
            try
            {
                agent.Direction = SyncDirectionOrder.DownloadAndUpload;
                agent.LocalProvider = sourceProvider;
                agent.RemoteProvider = destinationProvider;
                stats = agent.Synchronize();

                MessageBox.Show("Synchronization Finished");
            }
            catch (Exception ex)
            {
                throw ex;
                //TODO : Exception handling
            }
            finally
            {
                sourceProvider.Dispose();
                destinationProvider.Dispose();
            }
        }

        #endregion
    }
}
