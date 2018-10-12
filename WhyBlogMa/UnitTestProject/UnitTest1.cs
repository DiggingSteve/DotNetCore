
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Npgsql;
using Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Infrastructure.PgSql;
using System.Data;
using Infrastructure.Dapper;
using System.Linq.Expressions;
using System.Linq;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Conn()
        {
            //string sql = "insert into tuser values (@a,@b)";

            //PgSqlHelper.ExecuteNonQuery(sql, new NpgsqlParameter("@a",1), new NpgsqlParameter("@b",1 ));

            var list = DapperCommand<TUser>.GetList("select id,name from tuser");
            DapperCommand<TUser>.Update("");
        }

        [TestMethod]
        public void TestConfig()
        {
            //Parallel.For(0, 10, i =>
            //{
            //    //string a = ConfigurationManager.Config.GetSection<String>(new string[] { "PgsqlConfig", "ConnectionStr" });
            //    //Console.WriteLine(a);


            //});
            string key = "aaa";
            CacheHelper.CachePool.Set(key, 1111, TimeSpan.FromSeconds(3));
            Thread.Sleep(1000);
            CacheHelper.CachePool.Set(key, 1111, TimeSpan.FromSeconds(100));

        }

        public class JsonConfig<T> where T : class
        {
            public object GetAppSettings(string[] keys)
            {
                var config = new ConfigurationBuilder().AddJsonFile("config.json").AddJsonFile("config2.json").Build();
                IConfigurationSection section = null;
                section = GetSections(config.GetSection("default"), keys, 0);

                return section.Value;
            }

            private IConfigurationSection GetSections(IConfigurationSection section, string[] keys, int i)
            {

                while (i < keys.Length)
                {
                    section = section.GetSection(keys[i]);
                    i++;
                    return GetSections(section, keys, i);

                }
                return section;

            }

        }

        [TestMethod]
        public void TestCache()
        {

            CacheHelper.CachePool.Set("a", 1, TimeSpan.FromSeconds(100), true);

        }

        [TestMethod]
        public void TestExpression()
        {
            using (var con = new NpgsqlConnection(Infrastructure.PgSql.PgConfig.ConnectionStr))
            {
                var a = new DapperExtension<TUser>();
                a.Where(p => p.Id > 0 && p.Name == "ddd");
                
            }

        }




    }
}
