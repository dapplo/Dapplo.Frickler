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
// 

using System.ComponentModel;
using Dapplo.Config.Language;

namespace Frickler.Configuration
{
    /// <summary>
    ///     The translations for the context menu
    /// </summary>
    [Language("ContextMenu")]
    public interface IContextMenuTranslations : ILanguage
    {
        /// <summary>
        ///     The translation of the exit entry in the context menu
        /// </summary>
        [DefaultValue("Exit")]
        string Exit { get; }

        /// <summary>
        ///     The translation of the title of the context menu
        /// </summary>
        [DefaultValue("Frickler")]
        string Title { get; }
    }
}