using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MultiverseServerTest.TestSetUp
{

    [CollectionDefinition("TestSetUp")]
    public class TestCollection : ICollectionFixture<TestFixture>
    {
    }
}
