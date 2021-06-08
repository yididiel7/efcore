// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <inheritdoc/>
    public class QueryRootCreator : IQueryRootCreator
    {
        /// <inheritdoc/>
        public virtual QueryRootExpression CreateQueryRoot(IEntityType entityType, QueryRootExpression? source)
            => source?.QueryProvider != null
                ? new QueryRootExpression(source.QueryProvider, entityType)
                : new QueryRootExpression(entityType);

        /// <inheritdoc/>
        public virtual bool AreCompatible(QueryRootExpression? first, QueryRootExpression? second)
        {
            if (first is null && second is null)
            {
                return true;
            }

            if (first is not null && second is not null)
            {
                return first.EntityType.GetRootType() == second.EntityType.GetRootType();
            }

            return false;
        }
    }
}
