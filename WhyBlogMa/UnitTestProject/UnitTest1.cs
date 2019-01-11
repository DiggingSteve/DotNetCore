
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
            var builder = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile("config2.json", true, reloadOnChange: true);

            var config = builder.Build();

            //读取配置
            Console.WriteLine(config["default:code"]);


            Console.WriteLine("更改文件之后，按下任意键");


            Console.WriteLine(config["default:code"]);




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
                var list =
                    a.Select("max(id),name")
                    .Where(p => (p.Id >= 0 || p.Name == "name") && p.Name != null)
                    .GroupBy(p => new { p.Name })
                    .Having("having count(1)>0")
                   .ToList(con);


            }

        }
        [TestMethod]
        public void TestInsert()
        {
            using (var con = new NpgsqlConnection(Infrastructure.PgSql.PgConfig.ConnectionStr))
            {
                var a = new DapperExtension<TUser>();
                int time = TimeSpanCalculator.Run(() =>
                {
                    for (int i = 0; i < 1000000; i++)
                    {
                        var user = new TUser() { Id = 4, Name = "dadasd" };
                        a.Insert(user, con);
                    }
                });
                Console.WriteLine(time.ToString());



            }

        }

        private int Get0(int i)
        {
            return i;
        }

        public static int 库存 = 999;
        public static object _lock = new object();

        [TestMethod]
        public void TestAsync()
        {

            Parallel.For(0, 10, (i) =>
            {
                lock (_lock)
                {
                    处理请求(库存);
                }
            });
        }

        public void 处理请求(int a)
        {

        }
    }
}
