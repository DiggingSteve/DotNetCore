using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.PgSql
{
  public  class PgConfig
    {
        public static readonly string ConnectionStr = 
            ConfigurationManager.Config.GetSection<string>(new string[] { "PgsqlConfig", "ConnectionStr" });
    }
}
