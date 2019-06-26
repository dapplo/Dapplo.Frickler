// Dapplo - building blocks for desktop applications
// Copyright (C) 2017-2019  Dapplo
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

namespace Dapplo.Frickler.Extensions
{
    /// <summary>
    /// Extensions to help IDictionary 
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Get an IEnumerable with all changes between 2 IDictionary instances (old and new)
        /// </summary>
        /// <param name="before">IDictionary as it was before</param>
        /// <param name="after">IDictionary as it is</param>
        /// <typeparam name="TKey">Type for the key</typeparam>
        /// <typeparam name="TValue">Type for the value</typeparam>
        /// <returns>IEnumerable with the changes</returns>
        public static IEnumerable<DictionaryChangeInfo<TKey, TValue>> DetectChanges<TKey, TValue>(this IDictionary<TKey, TValue> before, IDictionary<TKey, TValue> after)
        {
            // Detect add & change
            foreach (var key in after.Keys)
            {
                if (before.ContainsKey(key))
                {
                    // Key was already there
                    if (!Equals(before[key], after[key]))
                    {
                        yield return DictionaryChangeInfo<TKey, TValue>.CreateChanged(key,before[key],after[key]);
                    }
                    continue;
                }

                yield return DictionaryChangeInfo<TKey, TValue>.CreateAdded(key, after[key]);
            }

            // Detect delete
            foreach (var key in before.Keys)
            {
                if (after.ContainsKey(key))
                {
                    continue;
                }

                yield return DictionaryChangeInfo<TKey, TValue>.CreateRemoved(key, before[key]);
            }
        }
    }
}
