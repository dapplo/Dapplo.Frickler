using System.Collections.Generic;
using System.Linq;
using Dapplo.Frickler.Extensions;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Frickler.Tests
{
    public class ExtensionTests
    {
        private static readonly LogSource Log = new LogSource();
        
        /// <summary>
        /// </summary>
        /// <param name="testOutputHelper"></param>
        public ExtensionTests(ITestOutputHelper testOutputHelper)
        {
            LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
        }

        [Fact]
        public void Test_DictionaryChanges()
        {
            var before = new Dictionary<string, string>();
            var after = new Dictionary<string, string>();

            before.Add("one", "1");
            after.Add("one", "1");

            // Make it look like two was removed, by not adding it to after
            before.Add("two", "2");

            // Make it look like three was added, by not adding it to before
            after.Add("three", "3");

            // Change four=4 tp four=IV
            before.Add("four", "4");
            after.Add("four", "IV");

            var changeList = before.DetectChanges(after).ToList();
            Assert.Contains(DictionaryChangeInfo<string, string>.CreateAdded("three", "3"), changeList);

            Assert.Contains(DictionaryChangeInfo<string, string>.CreateRemoved("two", "2"), changeList);

            Assert.Contains(DictionaryChangeInfo<string, string>.CreateChanged("four", "4", "IV"), changeList);

            Log.Info().WriteLine("\r\n" + string.Join("\r\n", before.DetectChanges(after)));
        }
    }
}
