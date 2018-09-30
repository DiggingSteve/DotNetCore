
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

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Conn()
        {


            var connString = "Host=localhost;Port=5432;Username=root;Password=Dlive185;Database=User";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Retrieve all rows
                using (var cmd = new NpgsqlCommand("SELECT * FROM tuser", conn))

                {

                    var a = cmd.ExecuteScalar();
                    Console.WriteLine(a.ToString());
                }
            }
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




    }
}
