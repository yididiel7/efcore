// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    /// <summary>
    ///     TODO: add comments
    /// </summary>
    public class SqlServerTemporalConvention : IModelFinalizingConvention
    {
        private const string PeriodStartDefaultName = "PeriodStart";
        private const string PeriodEndDefaultName = "PeriodEnd";

        /// <summary>
        ///     TODO: add comments
        /// </summary>
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            // if start/end names were provided check if we need to add them to shadow state
            // if start/end names were not provided, generate the names (make sure they are unique)

            // if we added the properties (either explicitly named or with default names)
            // also make sure that no properties map to columns with those names!

            foreach (var rootEntityType in modelBuilder.Metadata.GetEntityTypes().Select(et => et.GetRootType()).Distinct())
            {
                if (rootEntityType.IsTemporal())
                {
                    var storeObjectIdentifier = StoreObjectIdentifier.Table(rootEntityType.GetTableName()!, rootEntityType.GetSchema());

                    var periodStartPropertyName = rootEntityType.TemporalPeriodStartPropertyName();
                    if (periodStartPropertyName == null)
                    {
                        // property name was not provided - need to pick one
                        var propertyNames = rootEntityType.GetDerivedTypesInclusive()
                            .SelectMany(x => x.GetDeclaredProperties())
                            .Select(x => x.Name)
                            .Distinct()
                            .ToList();

                        periodStartPropertyName = GenerateUniqueName(PeriodStartDefaultName, propertyNames);
                        rootEntityType.SetTemporalPeriodStartPropertyName(periodStartPropertyName);
                    }

                    var periodStartProperty = rootEntityType.FindProperty(periodStartPropertyName);
                    if (periodStartProperty == null)
                    {
                        // need to create a new property and provide unique column mapping for it
                        var columnNames = rootEntityType.GetDerivedTypesInclusive()
                            .SelectMany(x => x.GetDeclaredProperties())
                            .Select(x => x.GetColumnName(in storeObjectIdentifier))
                            .Distinct()
                            .ToList();

                        // TODO: can builder ever be null here, or will new property be always created if necessary, xml comment is bit confusing
                        periodStartProperty = rootEntityType.Builder.Property(
                            typeof(DateTime),
                            periodStartPropertyName,
                            setTypeConfigurationSource: true, // TODO: true or false here?
                            fromDataAnnotation: false)!.Metadata;

                        periodStartProperty.SetValueGenerated(ValueGenerated.OnAddOrUpdate);

                        var generatedStartColumnName = GenerateUniqueName(periodStartPropertyName, columnNames!);
                        if (generatedStartColumnName != periodStartPropertyName)
                        {
                            periodStartProperty.SetAnnotation(
                                RelationalAnnotationNames.ColumnName,
                                generatedStartColumnName);
                        }

                        rootEntityType.SetTemporalPeriodStartColumnName(generatedStartColumnName);
                    }
                    else
                    {
                        rootEntityType.SetTemporalPeriodStartColumnName(periodStartProperty.GetColumnName(storeObjectIdentifier));
                    }

                    var periodEndPropertyName = rootEntityType.TemporalPeriodEndPropertyName();
                    if (periodEndPropertyName == null)
                    {
                        // property name was not provided - need to pick one
                        var propertyNames = rootEntityType.GetDerivedTypesInclusive()
                            .SelectMany(x => x.GetDeclaredProperties())
                            .Select(x => x.Name)
                            .Distinct()
                            .ToList();

                        periodEndPropertyName = GenerateUniqueName(PeriodEndDefaultName, propertyNames);
                        rootEntityType.SetTemporalPeriodEndPropertyName(periodEndPropertyName);
                    }

                    var periodEndProperty = rootEntityType.FindProperty(periodEndPropertyName);
                    if (periodEndProperty == null)
                    {
                        // need to create a new property and provide unique column mapping for it
                        var columnNames = rootEntityType.GetDerivedTypesInclusive()
                            .SelectMany(x => x.GetDeclaredProperties())
                            .Select(x => x.GetColumnName(in storeObjectIdentifier))
                            .Distinct()
                            .ToList();

                        // TODO: can builder ever be null here, or will new property be always created if necessary, xml comment is bit confusing
                        periodEndProperty = rootEntityType.Builder.Property(
                            typeof(DateTime),
                            periodEndPropertyName,
                            setTypeConfigurationSource: true, // TODO: true or false here?
                            fromDataAnnotation: false)!.Metadata;

                        periodEndProperty.SetValueGenerated(ValueGenerated.OnAddOrUpdate);

                        var generatedEndColumnName = GenerateUniqueName(periodEndPropertyName, columnNames!);
                        if (generatedEndColumnName != periodEndPropertyName)
                        {
                            periodEndProperty.SetAnnotation(
                                RelationalAnnotationNames.ColumnName,
                                generatedEndColumnName);
                        }

                        rootEntityType.SetTemporalPeriodEndColumnName(generatedEndColumnName);
                    }
                    else
                    {
                        rootEntityType.SetTemporalPeriodEndColumnName(periodEndProperty.GetColumnName(storeObjectIdentifier));
                    }
                }
            }

            static string GenerateUniqueName(string name, List<string> exisitingProperties, int index = 0)
            {
                var generated = index == 0 ? name : name + index;

                return !exisitingProperties.Contains(generated)
                    ? generated
                    : GenerateUniqueName(name, exisitingProperties, ++index);
            }
        }
    }
}
