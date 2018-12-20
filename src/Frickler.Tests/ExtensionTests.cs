// Dapplo - building blocks for desktop applications
// Copyright (C) 2017-2018  Dapplo
// 
// For more information see: http://dapplo.net/
// Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
// This file is part of Frickler
// 
// Frickler is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Frickler is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have a copy of the GNU Lesser General Public License
// along with Frickler. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

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
