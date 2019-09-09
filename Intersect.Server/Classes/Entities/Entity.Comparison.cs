﻿using System;

using JetBrains.Annotations;

namespace Intersect.Server.Entities
{
    public partial class EntityInstance
    {

        #region Comparison

        /// <summary>
        /// Compares two player names, returning if they are equivalent.
        /// </summary>
        /// <param name="name">a name</param>
        /// <param name="nameOther">a name to compare with</param>
        /// <returns><code>false</code> if <code>null</code> or non-byte-equal ignoring case</returns>
        public static bool CompareName([CanBeNull] string name, [CanBeNull] string nameOther)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(nameOther))
            {
                return false;
            }

            return string.Equals(name, nameOther, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

    }
}
