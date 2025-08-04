using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiUseSqlite.Helper
{
    public class SqlSugarHelper
    {
        public static SqlSugarScope Db = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = $"DataSource={GetDatabasePath()};",
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = true
        },
      db =>
      {

          db.Aop.OnLogExecuting = (sql, pars) =>
          {
              Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));

          };
      });

        private static string GetDatabasePath()
        {
            string databasePath = string.Empty;

#if ANDROID
        databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "maui_app.db");
#elif WINDOWS
            databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "maui_app.db");
#elif IOS || MACCATALYST
        databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", "maui_app.db");
#endif

            return databasePath;
        }
    }
}
