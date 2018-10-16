using System;
using System.Collections.Generic;
using System.Text;



namespace Infrastructure.Dapper.Attributes
{
  public  class TableNameAttribute: Attribute
    {
        public TableNameAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
    }
}
