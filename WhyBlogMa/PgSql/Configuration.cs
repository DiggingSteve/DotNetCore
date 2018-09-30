using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{

    public class ConfigurationManager
    {
        public readonly static Configuration Config = new Configuration();

    }
  public  class Configuration
    {
        private static readonly IConfigurationRoot _config = new ConfigurationBuilder().AddJsonFile("config.json").Build();

        public  T GetSection<T>(string[]keys)
        {
            IConfigurationSection section = null;
            section = GetSections(_config.GetSection("default"), keys, 0);
            return section.Get<T>();
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
}
