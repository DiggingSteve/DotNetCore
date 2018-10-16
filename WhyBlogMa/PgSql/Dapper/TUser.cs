using Infrastructure.Dapper.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Dapper
{
    [TableName("tuser")]
   public class TUser
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
