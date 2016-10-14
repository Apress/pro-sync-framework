namespace SyncPeers
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.pnlManipulateData = new System.Windows.Forms.Panel();
            this.cboOperation = new System.Windows.Forms.ComboBox();
            this.cboPeer = new System.Windows.Forms.ComboBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblHeading = new System.Windows.Forms.Label();
            this.pnlData = new System.Windows.Forms.Panel();
            this.pnlSync = new System.Windows.Forms.Panel();
            this.btnConflict = new System.Windows.Forms.Button();
            this.btnSync = new System.Windows.Forms.Button();
            this.cboConflictType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboRemotePeer = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboLocalPeer = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlSyncStatus = new System.Windows.Forms.Panel();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ctlPeersData = new SyncPeers.PeersData();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.btnN_TierSync = new System.Windows.Forms.Button();
            this.pnlManipulateData.SuspendLayout();
            this.pnlData.SuspendLayout();
            this.pnlSync.SuspendLayout();
            this.pnlSyncStatus.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlManipulateData
            // 
            this.pnlManipulateData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlManipulateData.Controls.Add(this.cboOperation);
            this.pnlManipulateData.Controls.Add(this.cboPeer);
            this.pnlManipulateData.Controls.Add(this.btnExecute);
            this.pnlManipulateData.Controls.Add(this.label2);
            this.pnlManipulateData.Controls.Add(this.label1);
            this.pnlManipulateData.Location = new System.Drawing.Point(8, 37);
            this.pnlManipulateData.Name = "pnlManipulateData";
            this.pnlManipulateData.Size = new System.Drawing.Size(408, 61);
            this.pnlManipulateData.TabIndex = 0;
            // 
            // cboOperation
            // 
            this.cboOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOperation.FormattingEnabled = true;
            this.cboOperation.Location = new System.Drawing.Point(165, 34);
            this.cboOperation.Name = "cboOperation";
            this.cboOperation.Size = new System.Drawing.Size(121, 21);
            this.cboOperation.TabIndex = 4;
            // 
            // cboPeer
            // 
            this.cboPeer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPeer.FormattingEnabled = true;
            this.cboPeer.Location = new System.Drawing.Point(6, 34);
            this.cboPeer.Name = "cboPeer";
            this.cboPeer.Size = new System.Drawing.Size(121, 21);
            this.cboPeer.TabIndex = 3;
            // 
            // btnExecute
            // 
            this.btnExecute.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExecute.Location = new System.Drawing.Point(322, 12);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 43);
            this.btnExecute.TabIndex = 2;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(162, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Select Operation";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select Peer";
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.Location = new System.Drawing.Point(3, 9);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(651, 25);
            this.lblHeading.TabIndex = 13;
            this.lblHeading.Text = "Collaboration Scenarios : Peer to Peer Synchronization";
            // 
            // pnlData
            // 
            this.pnlData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlData.Controls.Add(this.ctlPeersData);
            this.pnlData.Location = new System.Drawing.Point(8, 104);
            this.pnlData.Name = "pnlData";
            this.pnlData.Size = new System.Drawing.Size(783, 250);
            this.pnlData.TabIndex = 14;
            // 
            // pnlSync
            // 
            this.pnlSync.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSync.Controls.Add(this.panel1);
            this.pnlSync.Controls.Add(this.btnSync);
            this.pnlSync.Controls.Add(this.cboRemotePeer);
            this.pnlSync.Controls.Add(this.label4);
            this.pnlSync.Controls.Add(this.cboLocalPeer);
            this.pnlSync.Controls.Add(this.label3);
            this.pnlSync.Location = new System.Drawing.Point(8, 363);
            this.pnlSync.Name = "pnlSync";
            this.pnlSync.Padding = new System.Windows.Forms.Padding(2);
            this.pnlSync.Size = new System.Drawing.Size(696, 81);
            this.pnlSync.TabIndex = 15;
            // 
            // btnConflict
            // 
            this.btnConflict.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnConflict.Location = new System.Drawing.Point(170, 4);
            this.btnConflict.Name = "btnConflict";
            this.btnConflict.Size = new System.Drawing.Size(144, 67);
            this.btnConflict.TabIndex = 11;
            this.btnConflict.Text = "Create Conflict";
            this.btnConflict.UseVisualStyleBackColor = true;
            this.btnConflict.Click += new System.EventHandler(this.btnConflict_Click);
            // 
            // btnSync
            // 
            this.btnSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSync.Location = new System.Drawing.Point(547, 5);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(137, 67);
            this.btnSync.TabIndex = 5;
            this.btnSync.Text = "Synchronize";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // cboConflictType
            // 
            this.cboConflictType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConflictType.FormattingEnabled = true;
            this.cboConflictType.Location = new System.Drawing.Point(15, 28);
            this.cboConflictType.Name = "cboConflictType";
            this.cboConflictType.Size = new System.Drawing.Size(144, 21);
            this.cboConflictType.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Conflict Type";
            // 
            // cboRemotePeer
            // 
            this.cboRemotePeer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRemotePeer.FormattingEnabled = true;
            this.cboRemotePeer.Location = new System.Drawing.Point(106, 29);
            this.cboRemotePeer.Name = "cboRemotePeer";
            this.cboRemotePeer.Size = new System.Drawing.Size(82, 21);
            this.cboRemotePeer.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(105, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Select Remote Peer";
            // 
            // cboLocalPeer
            // 
            this.cboLocalPeer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLocalPeer.FormattingEnabled = true;
            this.cboLocalPeer.Location = new System.Drawing.Point(6, 29);
            this.cboLocalPeer.Name = "cboLocalPeer";
            this.cboLocalPeer.Size = new System.Drawing.Size(82, 21);
            this.cboLocalPeer.TabIndex = 6;
            this.cboLocalPeer.SelectedIndexChanged += new System.EventHandler(this.cboLocalPeer_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Select Local Peer";
            // 
            // pnlSyncStatus
            // 
            this.pnlSyncStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSyncStatus.Controls.Add(this.txtStatus);
            this.pnlSyncStatus.Controls.Add(this.label6);
            this.pnlSyncStatus.Location = new System.Drawing.Point(797, 104);
            this.pnlSyncStatus.Name = "pnlSyncStatus";
            this.pnlSyncStatus.Size = new System.Drawing.Size(355, 248);
            this.pnlSyncStatus.TabIndex = 16;
            // 
            // txtStatus
            // 
            this.txtStatus.ForeColor = System.Drawing.Color.Red;
            this.txtStatus.Location = new System.Drawing.Point(6, 24);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(338, 213);
            this.txtStatus.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 16);
            this.label6.TabIndex = 5;
            this.label6.Text = "Sync Status";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnConflict);
            this.panel1.Controls.Add(this.cboConflictType);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(215, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(326, 79);
            this.panel1.TabIndex = 12;
            // 
            // ctlPeersData
            // 
            this.ctlPeersData.Location = new System.Drawing.Point(3, 4);
            this.ctlPeersData.Name = "ctlPeersData";
            this.ctlPeersData.Size = new System.Drawing.Size(775, 243);
            this.ctlPeersData.TabIndex = 15;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnN_TierSync);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Location = new System.Drawing.Point(710, 365);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(442, 79);
            this.panel2.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(4, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(433, 16);
            this.label7.TabIndex = 0;
            this.label7.Text = "Sync Peer1 and Peer2 using N-tier Peer-Peer Synchronization";
            // 
            // btnN_TierSync
            // 
            this.btnN_TierSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnN_TierSync.Location = new System.Drawing.Point(96, 27);
            this.btnN_TierSync.Name = "btnN_TierSync";
            this.btnN_TierSync.Size = new System.Drawing.Size(212, 41);
            this.btnN_TierSync.TabIndex = 13;
            this.btnN_TierSync.Text = "N-Tier Synchronize";
            this.btnN_TierSync.UseVisualStyleBackColor = true;
            this.btnN_TierSync.Click += new System.EventHandler(this.btnN_TierSync_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1161, 448);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnlSyncStatus);
            this.Controls.Add(this.pnlSync);
            this.Controls.Add(this.pnlData);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.pnlManipulateData);
            this.Name = "frmMain";
            this.Text = "Peer to Peer Synchronization";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.pnlManipulateData.ResumeLayout(false);
            this.pnlManipulateData.PerformLayout();
            this.pnlData.ResumeLayout(false);
            this.pnlSync.ResumeLayout(false);
            this.pnlSync.PerformLayout();
            this.pnlSyncStatus.ResumeLayout(false);
            this.pnlSyncStatus.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Panel pnlManipulateData;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.ComboBox cboPeer;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboOperation;
        private System.Windows.Forms.Panel pnlData;
        private PeersData ctlPeersData;
        private System.Windows.Forms.Panel pnlSync;
        private System.Windows.Forms.Button btnConflict;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.ComboBox cboConflictType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboRemotePeer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboLocalPeer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlSyncStatus;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnN_TierSync;
	}
}

