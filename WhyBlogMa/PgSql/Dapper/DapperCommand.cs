using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Npgsql;

namespace Infrastructure.Dapper
{
    public class DapperCommand<T>
    {
        public static List<T> GetList(string sql)
        {
            using (var con = new NpgsqlConnection(PgSql.PgConfig.ConnectionStr))
            {
                var list = con.Query<T>(sql).ToList();
                return list;
            }
        }

        public static bool Update(string sql)
        {
            using (var con = new NpgsqlConnection(PgSql.PgConfig.ConnectionStr))
            {
                var updateSQL = string.Format(@"UPDATE tuser  SET name='{0}' where id={1};", "aaa", 1);
                var res = con.Execute(updateSQL);
                return res > 0;
            }
        }
    }
}
