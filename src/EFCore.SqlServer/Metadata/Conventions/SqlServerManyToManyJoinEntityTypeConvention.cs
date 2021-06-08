// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Microsoft.EntityFrameworkCore.SqlServer.Metadata.Conventions
{
    ///// <inheritdoc />
    //public class SqlServerManyToManyJoinEntityTypeConvention : ManyToManyJoinEntityTypeConvention
    //{
    //    /// <summary>
    //    ///     Creates a new instance of <see cref="SqlServerManyToManyJoinEntityTypeConvention" />.
    //    /// </summary>
    //    /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
    //    public SqlServerManyToManyJoinEntityTypeConvention(ProviderConventionSetBuilderDependencies dependencies)
    //        : base(dependencies)
    //    {
    //    }

    //    /// <inheritdoc />
    //    public override void ProcessSkipNavigationAdded(
    //        IConventionSkipNavigationBuilder skipNavigationBuilder,
    //        IConventionContext<IConventionSkipNavigationBuilder> context)
    //    {
    //        base.ProcessSkipNavigationAdded(skipNavigationBuilder, context);
    //        AddTemporalInformation(skipNavigationBuilder);
    //    }

    //    /// <inheritdoc />
    //    public override void ProcessSkipNavigationInverseChanged(
    //        IConventionSkipNavigationBuilder skipNavigationBuilder,
    //        IConventionSkipNavigation? inverse,
    //        IConventionSkipNavigation? oldInverse,
    //        IConventionContext<IConventionSkipNavigation> context)
    //    {
    //        base.ProcessSkipNavigationInverseChanged(skipNavigationBuilder, inverse, oldInverse, context);
    //        AddTemporalInformation(skipNavigationBuilder);
    //    }

    //    private void AddTemporalInformation(IConventionSkipNavigationBuilder skipNavigationBuilder)
    //    {
    //        var skipNavigation = skipNavigationBuilder.Metadata;

    //        if (!skipNavigation.IsCollection)
    //        {
    //            return;
    //        }

    //        var inverseSkipNavigation = skipNavigation.Inverse;
    //        if (inverseSkipNavigation == null
    //            || !inverseSkipNavigation.IsCollection)
    //        {
    //            return;
    //        }

    //        var declaringEntityType = skipNavigation.DeclaringEntityType;
    //        var inverseEntityType = inverseSkipNavigation.DeclaringEntityType;

    //        if (declaringEntityType.IsTemporal()
    //            && inverseEntityType.IsTemporal())
    //        {
    //            var model = declaringEntityType.Model;

    //            var joinEntityTypeName = declaringEntityType.ShortName();
    //            var inverseName = inverseEntityType.ShortName();

    //            joinEntityTypeName = StringComparer.Ordinal.Compare(joinEntityTypeName, inverseName) < 0
    //                ? joinEntityTypeName + inverseName
    //                : inverseName + joinEntityTypeName;

    //            var joinEntityType = model.FindEntityType(joinEntityTypeName);
    //            if (joinEntityType != null
    //                && !joinEntityType.IsTemporal())
    //            {
    //                joinEntityType.SetIsTemporal(true);
    //            }
    //        }
    //    }
    //}
}
