namespace QuoteClient {
    
    
    public partial class MySyncSyncAgent {
        
        partial void OnInitialized(){
			this.Quote.SyncDirection = Microsoft.Synchronization.Data.SyncDirection.Bidirectional;
        }
    }
}
