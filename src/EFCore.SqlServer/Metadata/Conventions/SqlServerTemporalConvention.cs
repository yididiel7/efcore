// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.SqlServer.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    /// <summary>
    ///     A convention that manipulates temporal settings for an entity mapped to a temporal table.
    /// </summary>
    public class SqlServerTemporalConvention : IEntityTypeAnnotationChangedConvention, ISkipNavigationForeignKeyChangedConvention
        //IModelFinalizingConvention
    {
        private const string PeriodStartDefaultName = "PeriodStart";
        private const string PeriodEndDefaultName = "PeriodEnd";

        /// <inheritdoc />
        public virtual void ProcessEntityTypeAnnotationChanged(
            IConventionEntityTypeBuilder entityTypeBuilder,
            string name,
            IConventionAnnotation? annotation,
            IConventionAnnotation? oldAnnotation,
            IConventionContext<IConventionAnnotation> context)
        {
            if (name == SqlServerAnnotationNames.IsTemporal)
            {
                if (annotation?.Value as bool? == true)
                {
                    if (entityTypeBuilder.Metadata.GetTemporalPeriodStartPropertyName() == null)
                    {
                        entityTypeBuilder.HasPeriodStart(PeriodStartDefaultName);
                    }

                    if (entityTypeBuilder.Metadata.GetTemporalPeriodEndPropertyName() == null)
                    {
                        entityTypeBuilder.HasPeriodEnd(PeriodEndDefaultName);
                    }
                }
                else
                {
                    entityTypeBuilder.HasPeriodStart(null);
                    entityTypeBuilder.HasPeriodEnd(null);
                }
            }

            if (name == SqlServerAnnotationNames.TemporalPeriodStartPropertyName
                || name == SqlServerAnnotationNames.TemporalPeriodEndPropertyName)
            {
                if (oldAnnotation?.Value is string oldPeriodPropertyName)
                {
                    var oldPeriodProperty = entityTypeBuilder.Metadata.GetProperty(oldPeriodPropertyName);
                    entityTypeBuilder.RemoveUnusedImplicitProperties(new[] { oldPeriodProperty });

                    if (oldPeriodProperty.GetTypeConfigurationSource() == ConfigurationSource.Explicit)
                    {
                        if ((name == SqlServerAnnotationNames.TemporalPeriodStartPropertyName
                                && oldPeriodProperty.GetDefaultValue() is DateTime start
                                && start == DateTime.MinValue)
                            || (name == SqlServerAnnotationNames.TemporalPeriodEndPropertyName
                                && oldPeriodProperty.GetDefaultValue() is DateTime end
                                && end == DateTime.MaxValue))
                        {
                            oldPeriodProperty.Builder.HasDefaultValue(null);
                        }
                    }
                }

                if (annotation?.Value is string periodPropertyName)
                {
                    var periodPropertyBuilder = entityTypeBuilder.Property(
                        typeof(DateTime),
                        periodPropertyName);

                    if (periodPropertyBuilder != null)
                    {
                        // set column name explicitly so that we don't try to uniquefy it to some other column
                        // in case another property is defined that maps to the same column
                        periodPropertyBuilder.HasColumnName(periodPropertyName);
                    }
                }
            }
        }

        ///// <inheritdoc />
        //public virtual void ProcessModelFinalizing(
        //    IConventionModelBuilder modelBuilder,
        //    IConventionContext<IConventionModelBuilder> context)
        //{
        //    // TODO: what is the proper way to recognize M2M join entity?
        //    var model = modelBuilder.Metadata;
        //    var manyToManyJoinEntityCandidates = model.GetEntityTypes().Where(t => !t.IsTemporal()
        //        && t.HasSharedClrType
        //        && t.GetConfigurationSource() == ConfigurationSource.Convention
        //        && t.GetForeignKeys().Count() == 2);

        //    foreach (var manyToManyJoinEntityCandidate in manyToManyJoinEntityCandidates)
        //    {
        //        var foreignKeys = manyToManyJoinEntityCandidate.GetForeignKeys().ToList();
        //        if (foreignKeys.Count == 2
        //            && foreignKeys[0].DeclaringEntityType == manyToManyJoinEntityCandidate
        //            && foreignKeys[0].PrincipalEntityType.IsTemporal()
        //            && foreignKeys[1].DeclaringEntityType == manyToManyJoinEntityCandidate
        //            && foreignKeys[1].PrincipalEntityType.IsTemporal())
        //        {
        //            manyToManyJoinEntityCandidate.SetIsTemporal(true);
        //        }
        //    }
        //}

        /// <inheritdoc />
        public void ProcessSkipNavigationForeignKeyChanged(
            IConventionSkipNavigationBuilder skipNavigationBuilder,
            IConventionForeignKey? foreignKey,
            IConventionForeignKey? oldForeignKey,
            IConventionContext<IConventionForeignKey> context)
        {
             if (skipNavigationBuilder.Metadata.JoinEntityType is IConventionEntityType joinEntityType
                && joinEntityType.HasSharedClrType
                && !joinEntityType.IsTemporal()
                && joinEntityType.GetConfigurationSource() == ConfigurationSource.Convention
                && skipNavigationBuilder.Metadata.DeclaringEntityType.IsTemporal()
                && skipNavigationBuilder.Metadata.Inverse is IConventionSkipNavigation inverse
                && inverse.DeclaringEntityType.IsTemporal())
            {
                joinEntityType.SetIsTemporal(true);
            }
        }
    }
}
