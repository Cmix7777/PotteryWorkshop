namespace PotteryWorkshop.Data;

/// <summary>
/// Подключает библиотеку SQLite, которая уже есть в системе.
/// На Windows это встроенный winsqlite3.dll, на Linux — libsqlite3.
/// Вызывается один раз до первого обращения к базе.
/// </summary>
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
