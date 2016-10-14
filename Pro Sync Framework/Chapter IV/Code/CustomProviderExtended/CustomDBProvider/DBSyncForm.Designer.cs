namespace CustomProvider
{
    partial class DBSyncForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnSynchronize_AB = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCreateConflict = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.btnSyncConflicts = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSyncDeletes = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSyncFinish = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.replicaContentFinishSync = new CustomProvider.ReplicaContent();
            this.replicaContentHandleDeletes = new CustomProvider.ReplicaContent();
            this.replicaContentHandleConflicts = new CustomProvider.ReplicaContent();
            this.replicaContentConflict = new CustomProvider.ReplicaContent();
            this.replicaContentSyncAB = new CustomProvider.ReplicaContent();
            this.replicaContentIntial = new CustomProvider.ReplicaContent();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(394, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "This example shows synchronization between three replicas[SQL server database]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(282, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Click on the \"Create\" button to create new sample records";
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(305, 25);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 3;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnSynchronize_AB
            // 
            this.btnSynchronize_AB.Location = new System.Drawing.Point(305, 157);
            this.btnSynchronize_AB.Name = "btnSynchronize_AB";
            this.btnSynchronize_AB.Size = new System.Drawing.Size(75, 23);
            this.btnSynchronize_AB.TabIndex = 5;
            this.btnSynchronize_AB.Text = "Synchronize";
            this.btnSynchronize_AB.UseVisualStyleBackColor = true;
            this.btnSynchronize_AB.Click += new System.EventHandler(this.btnSynchronize_AB_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(231, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Click on “Synchronize “ to Synchronize A and B";
            // 
            // btnCreateConflict
            // 
            this.btnCreateConflict.Location = new System.Drawing.Point(305, 353);
            this.btnCreateConflict.Name = "btnCreateConflict";
            this.btnCreateConflict.Size = new System.Drawing.Size(104, 23);
            this.btnCreateConflict.TabIndex = 9;
            this.btnCreateConflict.Text = "Create Conflict";
            this.btnCreateConflict.UseVisualStyleBackColor = true;
            this.btnCreateConflict.Click += new System.EventHandler(this.btnCreateConflict_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 358);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(303, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Click on “Create conflict” to modify the same record in A and B.";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(481, 4);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 10;
            // 
            // btnSyncConflicts
            // 
            this.btnSyncConflicts.Location = new System.Drawing.Point(502, 540);
            this.btnSyncConflicts.Name = "btnSyncConflicts";
            this.btnSyncConflicts.Size = new System.Drawing.Size(187, 23);
            this.btnSyncConflicts.TabIndex = 13;
            this.btnSyncConflicts.Text = "Synchronize and handle Conflicts";
            this.btnSyncConflicts.UseVisualStyleBackColor = true;
            this.btnSyncConflicts.Click += new System.EventHandler(this.btnSyncConflicts_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 545);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(431, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Click on “Synchronize and handle Conflicts” to Synchronize A and B and handle con" +
                "flicts .";
            // 
            // btnSyncDeletes
            // 
            this.btnSyncDeletes.Location = new System.Drawing.Point(502, 728);
            this.btnSyncDeletes.Name = "btnSyncDeletes";
            this.btnSyncDeletes.Size = new System.Drawing.Size(170, 23);
            this.btnSyncDeletes.TabIndex = 16;
            this.btnSyncDeletes.Text = "Synchronize and handle Delete";
            this.btnSyncDeletes.UseVisualStyleBackColor = true;
            this.btnSyncDeletes.Click += new System.EventHandler(this.btnSyncDeletes_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 733);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(487, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Click on “Synchronize and handle deletes ” to Delete a record from B and then Syn" +
                "chronize  B and C .";
            // 
            // btnSyncFinish
            // 
            this.btnSyncFinish.Location = new System.Drawing.Point(502, 922);
            this.btnSyncFinish.Name = "btnSyncFinish";
            this.btnSyncFinish.Size = new System.Drawing.Size(170, 23);
            this.btnSyncFinish.TabIndex = 19;
            this.btnSyncFinish.Text = "Finish Synchronization";
            this.btnSyncFinish.UseVisualStyleBackColor = true;
            this.btnSyncFinish.Click += new System.EventHandler(this.btnSyncFinish_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 922);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(290, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Click on “Finish Synchronization ” to  Synchronize  A and C .";
            // 
            // replicaContentFinishSync
            // 
            this.replicaContentFinishSync.Location = new System.Drawing.Point(12, 943);
            this.replicaContentFinishSync.Name = "replicaContentFinishSync";
            this.replicaContentFinishSync.Size = new System.Drawing.Size(1407, 150);
            this.replicaContentFinishSync.TabIndex = 18;
            // 
            // replicaContentHandleDeletes
            // 
            this.replicaContentHandleDeletes.Location = new System.Drawing.Point(12, 757);
            this.replicaContentHandleDeletes.Name = "replicaContentHandleDeletes";
            this.replicaContentHandleDeletes.Size = new System.Drawing.Size(1407, 150);
            this.replicaContentHandleDeletes.TabIndex = 15;
            // 
            // replicaContentHandleConflicts
            // 
            this.replicaContentHandleConflicts.Location = new System.Drawing.Point(7, 569);
            this.replicaContentHandleConflicts.Name = "replicaContentHandleConflicts";
            this.replicaContentHandleConflicts.Size = new System.Drawing.Size(1407, 150);
            this.replicaContentHandleConflicts.TabIndex = 12;
            // 
            // replicaContentConflict
            // 
            this.replicaContentConflict.Location = new System.Drawing.Point(7, 382);
            this.replicaContentConflict.Name = "replicaContentConflict";
            this.replicaContentConflict.Size = new System.Drawing.Size(1407, 150);
            this.replicaContentConflict.TabIndex = 8;
            // 
            // replicaContentSyncAB
            // 
            this.replicaContentSyncAB.Location = new System.Drawing.Point(7, 186);
            this.replicaContentSyncAB.Name = "replicaContentSyncAB";
            this.replicaContentSyncAB.Size = new System.Drawing.Size(1407, 148);
            this.replicaContentSyncAB.TabIndex = 6;
            // 
            // replicaContentIntial
            // 
            this.replicaContentIntial.Location = new System.Drawing.Point(7, 46);
            this.replicaContentIntial.Name = "replicaContentIntial";
            this.replicaContentIntial.Size = new System.Drawing.Size(1407, 93);
            this.replicaContentIntial.TabIndex = 2;
            // 
            // SyncForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1444, 878);
            this.Controls.Add(this.btnSyncFinish);
            this.Controls.Add(this.replicaContentFinishSync);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnSyncDeletes);
            this.Controls.Add(this.replicaContentHandleDeletes);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnSyncConflicts);
            this.Controls.Add(this.replicaContentHandleConflicts);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnCreateConflict);
            this.Controls.Add(this.replicaContentConflict);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.replicaContentSyncAB);
            this.Controls.Add(this.btnSynchronize_AB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.replicaContentIntial);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "SyncForm";
            this.Text = "SyncForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.SyncForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SyncForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private ReplicaContent replicaContentIntial;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnSynchronize_AB;
        private System.Windows.Forms.Label label3;
        private ReplicaContent replicaContentSyncAB;
        private System.Windows.Forms.Button btnCreateConflict;
        private ReplicaContent replicaContentConflict;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnSyncConflicts;
        private ReplicaContent replicaContentHandleConflicts;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSyncDeletes;
        private ReplicaContent replicaContentHandleDeletes;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSyncFinish;
        private ReplicaContent replicaContentFinishSync;
        private System.Windows.Forms.Label label7;
    }
}