namespace CustomProvider
{
    partial class ReplicaContent
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grdViewA = new System.Windows.Forms.DataGridView();
            this.grdViewB = new System.Windows.Forms.DataGridView();
            this.grdViewC = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewC)).BeginInit();
            this.SuspendLayout();
            // 
            // grdViewA
            // 
            this.grdViewA.AllowUserToAddRows = false;
            this.grdViewA.AllowUserToDeleteRows = false;
            this.grdViewA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdViewA.Location = new System.Drawing.Point(3, 25);
            this.grdViewA.Name = "grdViewA";
            this.grdViewA.ReadOnly = true;
            this.grdViewA.Size = new System.Drawing.Size(491, 153);
            this.grdViewA.TabIndex = 0;
            // 
            // grdViewB
            // 
            this.grdViewB.AllowUserToAddRows = false;
            this.grdViewB.AllowUserToDeleteRows = false;
            this.grdViewB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdViewB.Location = new System.Drawing.Point(500, 25);
            this.grdViewB.Name = "grdViewB";
            this.grdViewB.ReadOnly = true;
            this.grdViewB.Size = new System.Drawing.Size(485, 153);
            this.grdViewB.TabIndex = 1;
            // 
            // grdViewC
            // 
            this.grdViewC.AllowUserToAddRows = false;
            this.grdViewC.AllowUserToDeleteRows = false;
            this.grdViewC.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdViewC.Location = new System.Drawing.Point(991, 25);
            this.grdViewC.Name = "grdViewC";
            this.grdViewC.ReadOnly = true;
            this.grdViewC.Size = new System.Drawing.Size(470, 153);
            this.grdViewC.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Replica A";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(497, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Replica B";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(988, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Replica C";
            // 
            // ReplicaContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grdViewC);
            this.Controls.Add(this.grdViewB);
            this.Controls.Add(this.grdViewA);
            this.Name = "ReplicaContent";
            this.Size = new System.Drawing.Size(1464, 181);
            ((System.ComponentModel.ISupportInitialize)(this.grdViewA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewC)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView grdViewA;
        private System.Windows.Forms.DataGridView grdViewB;
        private System.Windows.Forms.DataGridView grdViewC;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
