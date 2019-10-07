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

using System;
using System.Collections.Generic;

namespace Frickler.Extensions
{
    /// <summary>
    /// Describes a change which was made to a dictionary
    /// </summary>
    /// <typeparam name="TKey">Type for the Key</typeparam>
    /// <typeparam name="TValue">Type for the value</typeparam>
    public class DictionaryChangeInfo<TKey, TValue> : IEquatable<DictionaryChangeInfo<TKey, TValue>>
    {
        /// <summary>
        /// Constructor for the DictionaryChangeInfo
        /// </summary>
        /// <param name="changeKind">DictionaryChangeKinds</param>
        /// <param name="key">TKey</param>
        /// <param name="before">TValue</param>
        /// <param name="after">TValue</param>
        public DictionaryChangeInfo(DictionaryChangeKinds changeKind, TKey key, TValue before, TValue after)
        {
            ChangeKind = changeKind;
            Key = key;
            Before = before;
            After = after;
        }

        /// <summary>
        /// Creates an CreateAdded DictionaryChangeInfo
        /// </summary>
        /// <param name="key">TKey</param>
        /// <param name="addedValue">TValue</param>
        public static DictionaryChangeInfo<TKey, TValue> CreateAdded(TKey key, TValue addedValue)
        {
            return new DictionaryChangeInfo<TKey, TValue>(DictionaryChangeKinds.Added, key, addedValue, default);
        }

        /// <summary>
        /// Creates an Removed DictionaryChangeInfo
        /// </summary>
        /// <param name="key">TKey</param>
        /// <param name="removedValue">TValue</param>
        public static DictionaryChangeInfo<TKey, TValue> CreateRemoved(TKey key, TValue removedValue)
        {
            return new DictionaryChangeInfo<TKey, TValue>(DictionaryChangeKinds.Removed, key, default, removedValue);
        }

        /// <summary>
        /// Creates an Changed DictionaryChangeInfo
        /// </summary>
        /// <param name="key">TKey</param>
        /// <param name="oldValue">TValue</param>
        /// <param name="newValue">TValue</param>
        public static DictionaryChangeInfo<TKey, TValue> CreateChanged(TKey key, TValue oldValue, TValue newValue)
        {
            return new DictionaryChangeInfo<TKey, TValue>(DictionaryChangeKinds.Changed, key, oldValue, newValue);
        }

        /// <summary>
        /// Type of the change
        /// </summary>
        public DictionaryChangeKinds ChangeKind { get; }
        /// <summary>
        /// Key which is added, removed or changed
        /// </summary>
        public TKey Key { get; }
        /// <summary>
        /// Value before, filled when deleted or changed
        /// </summary>
        public TValue Before { get; }
        /// <summary>
        /// Value after, filled when added or changed
        /// </summary>
        public TValue After { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as DictionaryChangeInfo<TKey, TValue>);
        }

        /// <inheritdoc />
        public bool Equals(DictionaryChangeInfo<TKey, TValue> other)
        {
            return other != null &&
                   ChangeKind == other.ChangeKind &&
                   EqualityComparer<TKey>.Default.Equals(Key, other.Key) &&
                   EqualityComparer<TValue>.Default.Equals(Before, other.Before) &&
                   EqualityComparer<TValue>.Default.Equals(After, other.After);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 463101270;
            hashCode = hashCode * -1521134295 + ChangeKind.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TKey>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<TValue>.Default.GetHashCode(Before);
            hashCode = hashCode * -1521134295 + EqualityComparer<TValue>.Default.GetHashCode(After);
            return hashCode;
        }

        /// <inheritdoc />
        public override string ToString() =>
            ChangeKind switch
            {
                DictionaryChangeKinds.Added => $"{ChangeKind} {Key} = {After}",
                DictionaryChangeKinds.Removed => $"{ChangeKind} {Key} = {Before}",
                DictionaryChangeKinds.Changed => $"{ChangeKind} {Key} = {Before} -> {After}",
                _ => string.Empty,
            };
    }
}
