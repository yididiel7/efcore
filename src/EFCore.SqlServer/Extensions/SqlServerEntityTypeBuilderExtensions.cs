// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.SqlServer.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     SQL Server specific extension methods for <see cref="EntityTypeBuilder" />.
    /// </summary>
    public static class SqlServerEntityTypeBuilderExtensions
    {
        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as memory-optimized.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder IsMemoryOptimized(
            this EntityTypeBuilder entityTypeBuilder,
            bool memoryOptimized = true)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.SetIsMemoryOptimized(memoryOptimized);

            return entityTypeBuilder;
        }

        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as memory-optimized.
        /// </summary>
        /// <typeparam name="TEntity"> The entity type being configured. </typeparam>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder<TEntity> IsMemoryOptimized<TEntity>(
            this EntityTypeBuilder<TEntity> entityTypeBuilder,
            bool memoryOptimized = true)
            where TEntity : class
            => (EntityTypeBuilder<TEntity>)IsMemoryOptimized((EntityTypeBuilder)entityTypeBuilder, memoryOptimized);

        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as memory-optimized.
        /// </summary>
        /// <param name="collectionOwnershipBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static OwnedNavigationBuilder IsMemoryOptimized(
            this OwnedNavigationBuilder collectionOwnershipBuilder,
            bool memoryOptimized = true)
        {
            Check.NotNull(collectionOwnershipBuilder, nameof(collectionOwnershipBuilder));

            collectionOwnershipBuilder.OwnedEntityType.SetIsMemoryOptimized(memoryOptimized);

            return collectionOwnershipBuilder;
        }

        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as memory-optimized.
        /// </summary>
        /// <typeparam name="TEntity"> The entity type being configured. </typeparam>
        /// <typeparam name="TRelatedEntity"> The entity type that this relationship targets. </typeparam>
        /// <param name="collectionOwnershipBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static OwnedNavigationBuilder<TEntity, TRelatedEntity> IsMemoryOptimized<TEntity, TRelatedEntity>(
            this OwnedNavigationBuilder<TEntity, TRelatedEntity> collectionOwnershipBuilder,
            bool memoryOptimized = true)
            where TEntity : class
            where TRelatedEntity : class
            => (OwnedNavigationBuilder<TEntity, TRelatedEntity>)IsMemoryOptimized(
                (OwnedNavigationBuilder)collectionOwnershipBuilder, memoryOptimized);

        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as memory-optimized.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionEntityTypeBuilder? IsMemoryOptimized(
            this IConventionEntityTypeBuilder entityTypeBuilder,
            bool? memoryOptimized,
            bool fromDataAnnotation = false)
        {
            if (entityTypeBuilder.CanSetIsMemoryOptimized(memoryOptimized, fromDataAnnotation))
            {
                entityTypeBuilder.Metadata.SetIsMemoryOptimized(memoryOptimized, fromDataAnnotation);
                return entityTypeBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the mapped table can be configured as memory-optimized.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="memoryOptimized"> A value indicating whether the table is memory-optimized. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <see langword="true" /> if the mapped table can be configured as memory-optimized. </returns>
        public static bool CanSetIsMemoryOptimized(
            this IConventionEntityTypeBuilder entityTypeBuilder,
            bool? memoryOptimized,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            return entityTypeBuilder.CanSetAnnotation(SqlServerAnnotationNames.MemoryOptimized, memoryOptimized, fromDataAnnotation);
        }

        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as temporal.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="periodStartPropertyName"> A value specifying the property name representing start of the period.</param>
        /// <param name="periodEndPropertyName"> A value specifying the property name representing end of the period.</param>
        /// <param name="historyTableName"> A value specifying the history table name for this entity. Default name will be used if none is specified. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder IsTemporal(
            this EntityTypeBuilder entityTypeBuilder,
            string? periodStartPropertyName = null,
            string? periodEndPropertyName = null,
            string? historyTableName = null)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.Metadata.SetIsTemporal(true);
            entityTypeBuilder.Metadata.SetTemporalPeriodStartPropertyName(periodStartPropertyName);
            entityTypeBuilder.Metadata.SetTemporalPeriodEndPropertyName(periodEndPropertyName);
            entityTypeBuilder.Metadata.SetTemporalHistoryTableName(historyTableName);

            return entityTypeBuilder;
        }

        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as temporal.
        /// </summary>
        /// <typeparam name="TEntity"> The entity type being configured. </typeparam>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="periodStartPropertyExpression"> A value specifying the property representing start of the period.</param>
        /// <param name="periodEndPropertyExpression"> A value specifying the property representing end of the period.</param>
        /// <param name="historyTableName"> A value specifying the history table name for this entity. Default name will be used if none is specified. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static EntityTypeBuilder<TEntity> IsTemporal<TEntity>(
            this EntityTypeBuilder<TEntity> entityTypeBuilder,
            Expression<Func<TEntity, DateTime>> periodStartPropertyExpression,
            Expression<Func<TEntity, DateTime>> periodEndPropertyExpression,
            string? historyTableName = null)
            where TEntity : class
            => (EntityTypeBuilder<TEntity>)IsTemporal(
                entityTypeBuilder,
                periodStartPropertyExpression.GetMemberAccess().Name,
                periodEndPropertyExpression.GetMemberAccess().Name,
                historyTableName);

        /// <summary>
        ///     Configures the table that the entity maps to when targeting SQL Server as temporal.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="periodStartPropertyName"> A value specifying the property name representing start of the period.</param>
        /// <param name="periodEndPropertyName"> A value specifying the property name representing end of the period.</param>
        /// <param name="historyTableName"> A value specifying the history table name for this entity. Default name will be used if none is specified. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionEntityTypeBuilder? IsTemporal(
            this IConventionEntityTypeBuilder entityTypeBuilder,
            string? periodStartPropertyName = null,
            string? periodEndPropertyName = null,
            string? historyTableName = null,
            bool fromDataAnnotation = false)
        {
            if (entityTypeBuilder.CanSetIsTemporal(true, fromDataAnnotation)
                && entityTypeBuilder.CanSetTemporalPeriodStartPropertyName(periodStartPropertyName, fromDataAnnotation)
                && entityTypeBuilder.CanSetTemporalPeriodEndPropertyName(periodEndPropertyName, fromDataAnnotation)
                && entityTypeBuilder.CanSetTemporalHistoryTableName(historyTableName))
            {
                entityTypeBuilder.Metadata.SetIsTemporal(true, fromDataAnnotation);
                entityTypeBuilder.Metadata.SetTemporalPeriodStartPropertyName(periodStartPropertyName, fromDataAnnotation);
                entityTypeBuilder.Metadata.SetTemporalPeriodEndPropertyName(periodEndPropertyName, fromDataAnnotation);
                entityTypeBuilder.Metadata.SetTemporalHistoryTableName(historyTableName, fromDataAnnotation);

                return entityTypeBuilder;
            }

            return null;
        }

        /// <summary>
        ///     Returns a value indicating whether the mapped table can be configured as temporal.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="temporal"> A value indicating whether the table is temporal. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> <see langword="true" /> if the mapped table can be configured as temporal. </returns>
        public static bool CanSetIsTemporal(
            this IConventionEntityTypeBuilder entityTypeBuilder,
            bool? temporal,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            return entityTypeBuilder.CanSetAnnotation(SqlServerAnnotationNames.IsTemporal, temporal, fromDataAnnotation);
        }

        /// <summary>
        /// TODO: add comment
        /// </summary>
        public static bool CanSetTemporalPeriodStartPropertyName(
            this IConventionEntityTypeBuilder entityTypeBuilder,
            string? periodStartPropertyName,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            return entityTypeBuilder.CanSetAnnotation(SqlServerAnnotationNames.TemporalPeriodStartPropertyName, periodStartPropertyName, fromDataAnnotation);
        }

        /// <summary>
        /// TODO: add comment
        /// </summary>
        public static bool CanSetTemporalPeriodEndPropertyName(
            this IConventionEntityTypeBuilder entityTypeBuilder,
            string? periodEndPropertyName,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            return entityTypeBuilder.CanSetAnnotation(SqlServerAnnotationNames.TemporalPeriodEndPropertyName, periodEndPropertyName, fromDataAnnotation);
        }

        /// <summary>
        /// TODO: add comment
        /// </summary>
        public static bool CanSetTemporalHistoryTableName(
            this IConventionEntityTypeBuilder entityTypeBuilder,
            string? historyTableName,
            bool fromDataAnnotation = false)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            return entityTypeBuilder.CanSetAnnotation(SqlServerAnnotationNames.TemporalHistoryTableName, historyTableName, fromDataAnnotation);
        }
    }
}
