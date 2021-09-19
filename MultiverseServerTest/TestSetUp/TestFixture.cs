using Microsoft.Extensions.Configuration;
using MultiverseServer.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiverseServerTest.TestSetUp
{
    public class TestFixture : IDisposable
    {
        public IConfiguration Config;
        public MultiverseDbContext DbContext;

        public TestFixture()
        {
            Config = ConfigSetUp.SetUp();
            DbContext = DatabaseSetUp.SetUp();
        }

        public void Dispose()
        {
            
        }
    }
}
