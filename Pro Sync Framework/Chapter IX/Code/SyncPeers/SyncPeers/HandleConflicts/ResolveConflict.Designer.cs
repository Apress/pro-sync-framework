namespace SyncPeers.HandleConflicts
{
    partial class ResolveConflict
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.grdViewRemote = new System.Windows.Forms.DataGridView();
            this.grdViewLocal = new System.Windows.Forms.DataGridView();
            this.pnlConflictInfo = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnResolveConflict = new System.Windows.Forms.Button();
            this.rbRetryNextSync = new System.Windows.Forms.RadioButton();
            this.rbRetryApplyingRow = new System.Windows.Forms.RadioButton();
            this.rbRetryWithForceWrite = new System.Windows.Forms.RadioButton();
            this.rbContinue = new System.Windows.Forms.RadioButton();
            this.txtConflictInfo = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewRemote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewLocal)).BeginInit();
            this.pnlConflictInfo.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.grdViewRemote);
            this.panel1.Controls.Add(this.grdViewLocal);
            this.panel1.Location = new System.Drawing.Point(12, 93);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(843, 192);
            this.panel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Remote Changes";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(-1, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Local Changes";
            // 
            // grdViewRemote
            // 
            this.grdViewRemote.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdViewRemote.Location = new System.Drawing.Point(3, 118);
            this.grdViewRemote.Name = "grdViewRemote";
            this.grdViewRemote.Size = new System.Drawing.Size(835, 65);
            this.grdViewRemote.TabIndex = 1;
            // 
            // grdViewLocal
            // 
            this.grdViewLocal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdViewLocal.Location = new System.Drawing.Point(3, 25);
            this.grdViewLocal.Name = "grdViewLocal";
            this.grdViewLocal.Size = new System.Drawing.Size(835, 65);
            this.grdViewLocal.TabIndex = 0;
            // 
            // pnlConflictInfo
            // 
            this.pnlConflictInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlConflictInfo.Controls.Add(this.txtConflictInfo);
            this.pnlConflictInfo.Location = new System.Drawing.Point(12, 25);
            this.pnlConflictInfo.Name = "pnlConflictInfo";
            this.pnlConflictInfo.Size = new System.Drawing.Size(843, 62);
            this.pnlConflictInfo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Conflict Information";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnResolveConflict);
            this.panel2.Controls.Add(this.rbRetryNextSync);
            this.panel2.Controls.Add(this.rbRetryApplyingRow);
            this.panel2.Controls.Add(this.rbRetryWithForceWrite);
            this.panel2.Controls.Add(this.rbContinue);
            this.panel2.Location = new System.Drawing.Point(12, 295);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(312, 102);
            this.panel2.TabIndex = 3;
            // 
            // btnResolveConflict
            // 
            this.btnResolveConflict.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResolveConflict.Location = new System.Drawing.Point(150, 3);
            this.btnResolveConflict.Name = "btnResolveConflict";
            this.btnResolveConflict.Size = new System.Drawing.Size(147, 94);
            this.btnResolveConflict.TabIndex = 4;
            this.btnResolveConflict.Text = "Resolve Conflict";
            this.btnResolveConflict.UseVisualStyleBackColor = true;
            this.btnResolveConflict.Click += new System.EventHandler(this.btnResolveConflict_Click);
            // 
            // rbRetryNextSync
            // 
            this.rbRetryNextSync.AutoSize = true;
            this.rbRetryNextSync.Location = new System.Drawing.Point(3, 69);
            this.rbRetryNextSync.Name = "rbRetryNextSync";
            this.rbRetryNextSync.Size = new System.Drawing.Size(102, 17);
            this.rbRetryNextSync.TabIndex = 3;
            this.rbRetryNextSync.Text = "Retry Nex tSync";
            this.rbRetryNextSync.UseVisualStyleBackColor = true;
            // 
            // rbRetryApplyingRow
            // 
            this.rbRetryApplyingRow.AutoSize = true;
            this.rbRetryApplyingRow.Location = new System.Drawing.Point(3, 46);
            this.rbRetryApplyingRow.Name = "rbRetryApplyingRow";
            this.rbRetryApplyingRow.Size = new System.Drawing.Size(118, 17);
            this.rbRetryApplyingRow.TabIndex = 2;
            this.rbRetryApplyingRow.Text = "Retry Applying Row";
            this.rbRetryApplyingRow.UseVisualStyleBackColor = true;
            // 
            // rbRetryWithForceWrite
            // 
            this.rbRetryWithForceWrite.AutoSize = true;
            this.rbRetryWithForceWrite.Location = new System.Drawing.Point(3, 26);
            this.rbRetryWithForceWrite.Name = "rbRetryWithForceWrite";
            this.rbRetryWithForceWrite.Size = new System.Drawing.Size(130, 17);
            this.rbRetryWithForceWrite.TabIndex = 1;
            this.rbRetryWithForceWrite.Text = "Retry With ForceWrite";
            this.rbRetryWithForceWrite.UseVisualStyleBackColor = true;
            // 
            // rbContinue
            // 
            this.rbContinue.AutoSize = true;
            this.rbContinue.Checked = true;
            this.rbContinue.Location = new System.Drawing.Point(3, 3);
            this.rbContinue.Name = "rbContinue";
            this.rbContinue.Size = new System.Drawing.Size(67, 17);
            this.rbContinue.TabIndex = 0;
            this.rbContinue.TabStop = true;
            this.rbContinue.Text = "Continue";
            this.rbContinue.UseVisualStyleBackColor = true;
            // 
            // txtConflictInfo
            // 
            this.txtConflictInfo.Location = new System.Drawing.Point(3, 3);
            this.txtConflictInfo.Multiline = true;
            this.txtConflictInfo.Name = "txtConflictInfo";
            this.txtConflictInfo.ReadOnly = true;
            this.txtConflictInfo.Size = new System.Drawing.Size(833, 54);
            this.txtConflictInfo.TabIndex = 0;
            // 
            // ResolveConflict
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 403);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlConflictInfo);
            this.Controls.Add(this.panel1);
            this.Name = "ResolveConflict";
            this.Text = "ResolveConflict";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewRemote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewLocal)).EndInit();
            this.pnlConflictInfo.ResumeLayout(false);
            this.pnlConflictInfo.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView grdViewLocal;
        private System.Windows.Forms.DataGridView grdViewRemote;
        private System.Windows.Forms.Panel pnlConflictInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbRetryApplyingRow;
        private System.Windows.Forms.RadioButton rbRetryWithForceWrite;
        private System.Windows.Forms.RadioButton rbContinue;
        private System.Windows.Forms.RadioButton rbRetryNextSync;
        private System.Windows.Forms.Button btnResolveConflict;
        private System.Windows.Forms.TextBox txtConflictInfo;
    }
}