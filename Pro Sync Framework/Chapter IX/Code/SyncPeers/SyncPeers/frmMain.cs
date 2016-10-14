using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using System.Data.SqlClient;

namespace SyncPeers
{
	public partial class frmMain : Form
    {
        #region Constructor
        public frmMain()
        {
            InitializeComponent();
        }
        #endregion

        #region Local Variables
        DbSyncProvider localSyncProvider;
        DbSyncProvider remoteSyncProvider;
        SyncOrchestrator collaborationSyncAgent;
        SyncOperationStatistics syncStatistics;
        #endregion

        #region Private Methods

        private void Synchronize(int localPeerID, int remotePeerID)
        {
            try
            {
                localSyncProvider = SyncProviderHelper.CreateSyncProvider(localPeerID);
                localSyncProvider.SyncProviderPosition = SyncProviderPosition.Local;
                localSyncProvider.ApplyChangeFailed += new EventHandler<DbApplyChangeFailedEventArgs>(HandleConflicts);

                remoteSyncProvider = SyncProviderHelper.CreateSyncProvider(remotePeerID);
                remoteSyncProvider.SyncProviderPosition = SyncProviderPosition.Remote;
                remoteSyncProvider.ApplyChangeFailed += new EventHandler<DbApplyChangeFailedEventArgs>(HandleConflicts);

                collaborationSyncAgent = new SyncOrchestrator();
                collaborationSyncAgent.LocalProvider = localSyncProvider;
                collaborationSyncAgent.RemoteProvider = remoteSyncProvider;
                collaborationSyncAgent.Direction = SyncDirectionOrder.UploadAndDownload;
                syncStatistics = collaborationSyncAgent.Synchronize();

                DisplaySyncStatus(syncStatistics);
            }
            catch (DbOutdatedSyncException ex)
            {
                throw new Exception("Peer Database is outdated.Please run the  CleanData.sql script in SQL or execute Clean Data Command from UI" + ex.ToString());
            }
            catch (Exception ex)
            {
                txtStatus.Text += ex.ToString();
                //TODO Exception handling
            }

        }     

        private void DisplaySyncStatus(SyncOperationStatistics syncOperationStatistics)
        {
            txtStatus.Text += "/********************Sync Status****************************/" + Environment.NewLine;
            txtStatus.Text += "Sync Start Time : " + syncOperationStatistics.SyncStartTime.ToString() + Environment.NewLine;
            txtStatus.Text += "Upload Changes Applied : " + syncOperationStatistics.UploadChangesApplied.ToString() + Environment.NewLine;
            txtStatus.Text += "Upload Changes Failed : " + syncOperationStatistics.UploadChangesFailed.ToString() + Environment.NewLine;
            txtStatus.Text += "Upload Changes Total: " + syncOperationStatistics.UploadChangesTotal.ToString() + Environment.NewLine;
            txtStatus.Text += Environment.NewLine;
            txtStatus.Text += "Download Changes Applied : " + syncOperationStatistics.DownloadChangesApplied.ToString() + Environment.NewLine;
            txtStatus.Text += "Download Changes Failed : " + syncOperationStatistics.DownloadChangesFailed.ToString() + Environment.NewLine;
            txtStatus.Text += "Download Changes Total: " + syncOperationStatistics.DownloadChangesTotal.ToString() + Environment.NewLine;
            txtStatus.Text += Environment.NewLine;
            txtStatus.Text += "Sync Finish Time : " + syncOperationStatistics.SyncEndTime.ToString();
        }

        private void CreateConflicts(int ConflictType, int localPeerID, int remotePeerID)
        {
            switch (ConflictType)
            {
                case 1:
                    DBHelper.ExecuteSqlQuery("INSERT INTO Account(Accountid,Name) VALUES ('00000000-0000-0000-0000-000000000010','InsertConflictAccount')", localPeerID);
                    DBHelper.ExecuteSqlQuery("INSERT INTO Account(Accountid,Name) VALUES ('00000000-0000-0000-0000-000000000010','InsertConflictAccount')", remotePeerID);
                    break;
                case 2:
                    DBHelper.ExecuteSqlQuery("UPDATE Account SET Name = 'UpdateConflictLocalPeerAccount' WHERE Name = 'Account1'", localPeerID);
                    DBHelper.ExecuteSqlQuery("UPDATE Account SET Name = 'UpdateConflictRemotePeerAccount' WHERE Name = 'Account1'", remotePeerID);
                    break;
                case 3:
                    DBHelper.ExecuteSqlQuery("UPDATE Account SET Name = 'UpdateConflictLocalPeerAccount' WHERE Name = 'Account2'", localPeerID);
                    DBHelper.ExecuteSqlQuery("DELETE Account WHERE Name = 'Account2'", remotePeerID);
                    break;
                default:
                    break;
            }
            ctlPeersData.RefreshGrids();
        }

        private void HandleConflicts(object sender, DbApplyChangeFailedEventArgs e)
        {

            string conflictingPeerConnection;
            if (e.Connection.Database.ToString().Equals(localSyncProvider.Connection.Database))
            {
                conflictingPeerConnection = localSyncProvider.Connection.Database;
            }
            else
            {
                conflictingPeerConnection = remoteSyncProvider.Connection.Database;
            }

            string conflictInfo = "/***********Conflicts Found***********/ " + Environment.NewLine +
                e.Conflict.Type + " was detected at follwoing Peer " + Environment.NewLine +
                conflictingPeerConnection + Environment.NewLine;

            txtStatus.Text += conflictInfo;
            SyncPeers.HandleConflicts.ResolveConflict rc = new SyncPeers.HandleConflicts.ResolveConflict(e, conflictInfo);

            switch (e.Conflict.Type)
            {
                case DbConflictType.LocalInsertRemoteInsert:
                    rc.ShowDialog();
                    break;
                case DbConflictType.LocalUpdateRemoteUpdate:
                    rc.ShowDialog();
                    break;
                case DbConflictType.LocalUpdateRemoteDelete:
                    rc.ShowDialog();
                    break;
                case DbConflictType.LocalDeleteRemoteUpdate:
                    rc.ShowDialog();
                    break;
                case DbConflictType.ErrorsOccurred:
                    rc.ShowDialog();
                    break;
                case DbConflictType.LocalDeleteRemoteDelete:
                    //TODO
                    break;
                default:
                    break;
            }
            ctlPeersData.RefreshGrids();
        }

        private void IntializeControls()
        {
            cboPeer.DataSource = DBHelper.GetPeers();
            cboPeer.ValueMember = "PeerID";
            cboPeer.DisplayMember = "PeerName";
            cboPeer.SelectedIndex = 0;

            cboConflictType.DataSource = DBHelper.GetConflictType();
            cboConflictType.ValueMember = "ConflictTypeID";
            cboConflictType.DisplayMember = "ConflictTypeName";
            cboConflictType.SelectedIndex = 0;

            cboOperation.DataSource = DBHelper.GetOperations();
            cboOperation.ValueMember = "OpCode";
            cboOperation.DisplayMember = "OpName";
            cboOperation.SelectedIndex = 0;

            cboLocalPeer.DataSource = DBHelper.GetPeers();
            cboLocalPeer.ValueMember = "PeerID";
            cboLocalPeer.DisplayMember = "PeerName";
            cboLocalPeer.SelectedIndex = 0;

        }
        
        #endregion

        #region Form Events
        private void frmMain_Load(object sender, EventArgs e)
        {
            IntializeControls();
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            txtStatus.Text = string.Empty;
            Synchronize(int.Parse(cboLocalPeer.SelectedValue.ToString()), int.Parse(cboRemotePeer.SelectedValue.ToString()));
            ctlPeersData.RefreshGrids();
        }

        private void btnConflict_Click(object sender, EventArgs e)
        {
            CreateConflicts(int.Parse(cboConflictType.SelectedValue.ToString()), 
                int.Parse(cboLocalPeer.SelectedValue.ToString()), int.Parse(cboRemotePeer.SelectedValue.ToString()));
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            int peerId =  int.Parse(cboPeer.SelectedValue.ToString());
            int operationCode = int.Parse(cboOperation.SelectedValue.ToString());

            if (operationCode != 5)
            {
                Guid slectedGuid =  ctlPeersData.GetSelectedRecord(peerId);
                DBHelper.ExecuteOperation(operationCode ,peerId,slectedGuid);
                ctlPeersData.RefreshGrids();
            }
            else
            {
                CleanMetadata(peerId);
            }
        }

        private void CleanMetadata(int peerId)
        {
         
            DbSyncProvider dbSyncProvider =SyncProviderHelper.CreateSyncProvider(peerId);        

            if (!dbSyncProvider.CleanupMetadata())
            {
                txtStatus.Text = "Metadata for Peer " + peerId.ToString() + "Can not be deleted, Please try Again";
            }

            txtStatus.Text = "Metadata for Peer " + peerId.ToString() + " is Deleted";
        }

        private void cboLocalPeer_SelectedIndexChanged(object sender, EventArgs e)
        {

            DataTable dt = DBHelper.GetPeers();
            dt.Rows.RemoveAt(cboLocalPeer.SelectedIndex);
            cboRemotePeer.DataSource = dt;
            cboRemotePeer.ValueMember = "PeerID";
            cboRemotePeer.DisplayMember = "PeerName";
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DBHelper.CleanPeers(1);
            CleanMetadata(1);
            DBHelper.CleanPeers(2);
            CleanMetadata(2);
            DBHelper.CleanPeers(3);
            CleanMetadata(3);
        }
        #endregion

        private void btnN_TierSync_Click(object sender, EventArgs e)
        {
            try
            {
                KnowledgeSyncProvider localKnowledgeSyncProvider;
                KnowledgeSyncProvider remoteKnowledgeSyncProvider;
                string remoteUri =@"http://localhost:8731/RemoteProvider/RemotePeerSyncService/";

                localKnowledgeSyncProvider =SyncProviderHelper.CreateSyncProvider(1);//1 : Peer1
                remoteKnowledgeSyncProvider = new LocalPeerProxy.LocalPeerSyncProxy(remoteUri);
                 

                collaborationSyncAgent = new SyncOrchestrator();
                collaborationSyncAgent.LocalProvider = localKnowledgeSyncProvider;
                collaborationSyncAgent.RemoteProvider = remoteKnowledgeSyncProvider;
                collaborationSyncAgent.Direction = SyncDirectionOrder.UploadAndDownload;
                syncStatistics = collaborationSyncAgent.Synchronize();
                txtStatus.Text = string.Empty;
                DisplaySyncStatus(syncStatistics);
                ctlPeersData.RefreshGrids();
            }
            catch (DbOutdatedSyncException ex)
            {
                throw new Exception("Peer Database is outdated.Please run the  CleanData.sql script in SQL or execute Clean Data Command from UI" + ex.ToString());
            }
            catch (Exception ex)
            {
                txtStatus.Text = ex.ToString();
                //TODO Exception handling
            }
        }


	}
}