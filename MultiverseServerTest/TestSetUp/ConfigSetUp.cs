using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiverseServerTest.TestSetUp
{
    class ConfigSetUp
    {
        private ConfigSetUp()
        {

        }

        public static IConfiguration SetUp()
        {

            string workingDir = Environment.CurrentDirectory;
            string projectdir = Directory.GetParent(workingDir).Parent.Parent.FullName;

            IConfiguration Config = new ConfigurationBuilder()
                        .SetBasePath(projectdir)
                        .AddJsonFile("apptestsettings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
            return Config;
        }
    }
}
