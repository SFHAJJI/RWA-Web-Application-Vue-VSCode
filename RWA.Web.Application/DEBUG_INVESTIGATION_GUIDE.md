# Debug and Logging Investigation Setup

## What We Added

I've added comprehensive logging to track threading issues with your `AddRangeAsync` calls:

### 1. InventoryMapper Logging
- **Call tracking**: Each call gets a unique ID and timestamp
- **Thread information**: Shows which thread is executing the database operations
- **Stack traces**: Shows the call chain to identify who's calling the mapper
- **DbContext tracking**: Shows which DbContext instance is being used
- **Timing**: Shows duration of operations

### 2. EfWorkflowDbProvider Logging
- **Operation tracking**: Each database operation gets a unique ID
- **Thread safety monitoring**: Tracks concurrent access to DbContext
- **Error handling**: Specifically logs ObjectDisposedException cases

## How to View the Debug Output

### Option 1: Console Logging (Recommended for Investigation)
Add this to your `Program.cs` or `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "RWA.Web.Application.Services.Workflow.InventoryMapper": "Debug",
      "RWA.Web.Application.Services.Workflow.EfWorkflowDbProvider": "Debug"
    },
    "Console": {
      "IncludeScopes": true,
      "TimestampFormat": "yyyy-MM-dd HH:mm:ss.fff "
    }
  }
}
```

### Option 2: Run in Debug Mode
1. Set breakpoints in `InventoryMapper.MapAsync()` 
2. Use F5 in Visual Studio/VS Code
3. Watch the debug console output

### Option 3: File Logging (Best for Analysis)
Add file logging to capture all thread activity:

```csharp
// In Program.cs, add:
builder.Services.AddLogging(logging =>
{
    logging.AddFile("logs/debug-{Date}.txt", LogLevel.Debug);
});
```

## What the Logs Will Show You

### Expected Output Format:
```
📍 INVENTORY MAPPER START - Call #1 on Thread 15 at 2025-01-19 14:30:15.123
📊 Call #1: Processing 1250 rows from DataTable
💾 Call #1 BEFORE AddRangeAsync - Thread 15, 1250 rows, DbContext: 12345678
🔄 Call #1: Calling AddRangeAsync...
✅ Call #1 AddRangeAsync SUCCESS - Thread 15
🔄 Call #1: Calling SaveChangesAsync...
✅ Call #1 SaveChangesAsync SUCCESS - Thread 15, Saved 1250 entities
🏁 INVENTORY MAPPER END - Call #1 on Thread 15, Duration: 3450.2ms

💾 EF_DB_PROVIDER SaveChangesAsync START - Op #1, Thread 22, DbContext: 12345678
✅ EF_DB_PROVIDER SaveChangesAsync SUCCESS - Op #1, Thread 22
```

### What to Look For:
1. **Multiple simultaneous calls**: Look for overlapping call IDs
2. **Different threads**: Multiple thread IDs accessing same DbContext
3. **Context disposal**: Look for "DISPOSED" warnings
4. **Call frequency**: How often AddRangeAsync is being called

## Next Steps

1. **Start the application** with the enhanced logging
2. **Trigger the inventory import** functionality
3. **Examine the logs** to see:
   - How many times `AddRangeAsync` is called
   - Which threads are calling it
   - Whether there are concurrent calls
   - Stack traces showing the call origins

This will help us identify if the issue is:
- Multiple concurrent imports
- State machine triggering multiple calls
- Background tasks interfering
- Threading issues with singleton services

Let me know what you see in the logs and we can analyze the threading patterns together!
