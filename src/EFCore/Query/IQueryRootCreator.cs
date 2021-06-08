// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     TODO: add comments
    /// </summary>
    public interface IQueryRootCreator
    {
        /// <summary>
        ///     TODO: add comments
        /// </summary>
        QueryRootExpression CreateQueryRoot(IEntityType entityType, QueryRootExpression? source);

        /// <summary>
        ///     TODO: add comments
        /// </summary>
        bool AreCompatible(QueryRootExpression? first, QueryRootExpression? second);
    }
}
