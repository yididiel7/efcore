// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.SqlServer.Internal;

namespace Microsoft.EntityFrameworkCore.SqlServer.Query.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class SqlServerQueryRootCreator : QueryRootCreator
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override QueryRootExpression CreateQueryRoot(IEntityType entityType, QueryRootExpression? source)
        {
            if (source is TemporalQueryRootExpression tqre)
            {
                if (!entityType.GetRootType().IsTemporal())
                {
                    throw new InvalidOperationException(SqlServerStrings.TemporalNavigationExpansionBetweenTemporalAndNonTemporal(entityType.DisplayName()));
                }

                if (tqre is TemporalAsOfQueryRootExpression asOf)
                {
                    return source.QueryProvider != null
                        ? new TemporalAsOfQueryRootExpression(source.QueryProvider, entityType, asOf.PointInTime)
                        : new TemporalAsOfQueryRootExpression(entityType, asOf.PointInTime);
                }

                throw new InvalidOperationException(SqlServerStrings.TemporalNavigationExpansionOnlySupportedForAsOf(nameof(TemporalOperationType.AsOf)));

            }

            if (entityType.GetRootType().IsTemporal()
                && source is null)
            {
                throw new InvalidOperationException(SqlServerStrings.TemporalFailedToCreateQueryRoot(entityType.DisplayName()));
            }

            return base.CreateQueryRoot(entityType, source);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override bool AreCompatible(QueryRootExpression? first, QueryRootExpression? second)
        {
            if (!base.AreCompatible(first, second))
            {
                return false;
            }

            var firstTemporal = first as TemporalQueryRootExpression;
            var secondTemporal = second as TemporalQueryRootExpression;
            if (firstTemporal != null && secondTemporal != null)
            {
                if (firstTemporal is TemporalAsOfQueryRootExpression firstAsOf
                    && secondTemporal is TemporalAsOfQueryRootExpression secondAsOf
                    && firstAsOf.PointInTime == secondAsOf.PointInTime)
                {
                    return true;
                }

                if (firstTemporal is TemporalAllQueryRootExpression
                    && secondTemporal is TemporalAllQueryRootExpression)
                {
                    return true;
                }

                if (firstTemporal is TemporalRangeQueryRootExpression firstRange
                    && secondTemporal is TemporalRangeQueryRootExpression secondRange
                    && firstRange.From == secondRange.From
                    && firstRange.To == secondRange.To)
                {
                    return true;
                }
            }

            if (firstTemporal != null || secondTemporal != null)
            {
                var entityType = first?.EntityType ?? second?.EntityType;

                throw new InvalidOperationException(SqlServerStrings.TemporalSetOperationOnMismatchedSources(entityType));
            }
            
            return true;
        }
    }
}
