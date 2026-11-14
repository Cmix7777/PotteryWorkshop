namespace PotteryWorkshop.Data;
public static class SqliteSetup
{
    private static bool _done;

    public static void Init()
    {
        if (_done)
        {
            return;
        }

        if (OperatingSystem.IsWindows())
        {
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_winsqlite3());
        }
        else
        {
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
        }

        _done = true;
    }
}
