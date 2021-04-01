// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal
{
    /// <summary>
    ///     <para>
    ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///         any release. You should only use it directly in your code with extreme caution and knowing that
    ///         doing so can result in application failures when updating to a new Entity Framework Core release.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Singleton" />. This means a single instance
    ///         is used by many <see cref="DbContext" /> instances. The implementation must be thread-safe.
    ///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped" />.
    ///     </para>
    /// </summary>
    public class SqlServerMigrationsAnnotationProvider : MigrationsAnnotationProvider
    {
        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this service. </param>
#pragma warning disable EF1001 // Internal EF Core API usage.
        public SqlServerMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies)
#pragma warning restore EF1001 // Internal EF Core API usage.
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> ForRemove(IRelationalModel model)
            => model.GetAnnotations().Where(a => a.Name != SqlServerAnnotationNames.EditionOptions);

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> ForRemove(ITable table)
            => table.GetAnnotations();

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> ForRemove(IUniqueConstraint constraint)
        {
            if (constraint.Table[SqlServerAnnotationNames.IsTemporal] as bool? == true)
            {
                yield return new Annotation(SqlServerAnnotationNames.IsTemporal, true);

                yield return new Annotation(
                    SqlServerAnnotationNames.TemporalPeriodStartColumnName,
                    constraint.Table[SqlServerAnnotationNames.TemporalPeriodStartColumnName]);

                yield return new Annotation(
                    SqlServerAnnotationNames.TemporalPeriodEndColumnName,
                    constraint.Table[SqlServerAnnotationNames.TemporalPeriodEndColumnName]);

                yield return new Annotation(
                    SqlServerAnnotationNames.TemporalHistoryTableName,
                    constraint.Table[SqlServerAnnotationNames.TemporalHistoryTableName]);
            }
        }

        /// <inheritdoc />
        public override IEnumerable<IAnnotation> ForRemove(IColumn column)
        {
            if (column.Table[SqlServerAnnotationNames.IsTemporal] as bool? == true)
            {
                yield return new Annotation(SqlServerAnnotationNames.IsTemporal, true);

                if (column[SqlServerAnnotationNames.IsTemporalPeriodStartColumn] as bool? == true)
                {
                    yield return new Annotation(SqlServerAnnotationNames.IsTemporalPeriodStartColumn, true);
                }

                if (column[SqlServerAnnotationNames.IsTemporalPeriodEndColumn] as bool? == true)
                {
                    yield return new Annotation(SqlServerAnnotationNames.IsTemporalPeriodEndColumn, true);
                }

                yield return new Annotation(
                    SqlServerAnnotationNames.TemporalHistoryTableName,
                    column.Table[SqlServerAnnotationNames.TemporalHistoryTableName]);
            }
        }
    }
}
