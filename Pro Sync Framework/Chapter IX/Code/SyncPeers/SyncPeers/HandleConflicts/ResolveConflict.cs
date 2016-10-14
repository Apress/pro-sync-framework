using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Synchronization.Data;

namespace SyncPeers.HandleConflicts
{
    public partial class ResolveConflict : Form
    {
        DbApplyChangeFailedEventArgs dbApplyChangeFailedEventArgs;
        public ResolveConflict(DbApplyChangeFailedEventArgs e,string conflictInfo)
        {
            InitializeComponent();
            dbApplyChangeFailedEventArgs = e;
            txtConflictInfo.Text = conflictInfo;
            grdViewLocal.DataSource = dbApplyChangeFailedEventArgs.Conflict.LocalChange;
            grdViewRemote.DataSource = dbApplyChangeFailedEventArgs.Conflict.RemoteChange;
        }

        private void btnResolveConflict_Click(object sender, EventArgs e)
        {
            if (rbContinue.Checked)
            {
                dbApplyChangeFailedEventArgs.Action = ApplyAction.Continue;
            }
            else
            {
                if (rbRetryWithForceWrite.Checked)
                {
                    dbApplyChangeFailedEventArgs.Action = ApplyAction.RetryWithForceWrite;
                }
                else
                {
                    if (rbRetryApplyingRow.Checked)
                    {
                        dbApplyChangeFailedEventArgs.Action = ApplyAction.RetryApplyingRow;
                    }
                    else
                    {
                        if (rbRetryNextSync.Checked)
                        {
                            dbApplyChangeFailedEventArgs.Action = ApplyAction.RetryNextSync;
                        }
                    }
                }
            }

            Close();

        }
    }
}
