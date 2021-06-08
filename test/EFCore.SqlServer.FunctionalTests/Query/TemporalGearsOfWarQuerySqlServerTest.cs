// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query
{
    [SqlServerCondition(SqlServerCondition.SupportsTemporalTablesCascadeDelete)]
    public class TemporalGearsOfWarQuerySqlServerTest : GearsOfWarQueryRelationalTestBase<TemporalGearsOfWarQuerySqlServerFixture>
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public TemporalGearsOfWarQuerySqlServerTest(TemporalGearsOfWarQuerySqlServerFixture fixture, ITestOutputHelper testOutputHelper)
#pragma warning restore IDE0060 // Remove unused parameter
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override Expression RewriteServerQueryExpression(Expression serverQueryExpression)
        {
            var temporalEntityTypes = new List<Type>
            {
                typeof(TestModels.GearsOfWarModel.City),
                typeof(TestModels.GearsOfWarModel.CogTag),
                typeof(TestModels.GearsOfWarModel.Faction),
                typeof(TestModels.GearsOfWarModel.LocustHorde),
                typeof(TestModels.GearsOfWarModel.Gear),
                typeof(TestModels.GearsOfWarModel.Officer),
                typeof(TestModels.GearsOfWarModel.LocustLeader),
                typeof(TestModels.GearsOfWarModel.LocustCommander),
                typeof(TestModels.GearsOfWarModel.LocustHighCommand),
                typeof(TestModels.GearsOfWarModel.Mission),
                typeof(TestModels.GearsOfWarModel.Squad),
                typeof(TestModels.GearsOfWarModel.SquadMission),
                typeof(TestModels.GearsOfWarModel.Weapon),
            };

            var rewriter = new PointInTimeQueryRewriter(Fixture.ChangesDate, temporalEntityTypes);

            return rewriter.Visit(serverQueryExpression);
        }

        public override Task Include_where_list_contains_navigation(bool async)
            => Task.CompletedTask;

        public override Task Include_where_list_contains_navigation2(bool async)
            => Task.CompletedTask;

        public override Task Navigation_accessed_twice_outside_and_inside_subquery(bool async)
            => Task.CompletedTask;

        public override Task Select_correlated_filtered_collection_returning_queryable_throws(bool async)
            => Task.CompletedTask;

        // test infra issue
        public override Task Query_reusing_parameter_with_inner_query_doesnt_declare_duplicate_parameter(bool async)
            => Task.CompletedTask;

        public override Task Multiple_includes_with_client_method_around_entity_and_also_projecting_included_collection()
            => Task.CompletedTask;

        public override void Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result1()
        {
        }

        public override void Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result2()
        {
        }

        public override void Byte_array_filter_by_length_parameter_compiled()
        {
        }

        public override async Task Basic_query_gears(bool async)
        {
            await base.Basic_query_gears(async);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOfBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[PeriodEnd], [g].[PeriodStart], [g].[Rank]
FROM [Gears] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [g]");
        }

        public override async Task Accessing_derived_property_using_hard_and_soft_cast(bool async)
        {
            await base.Accessing_derived_property_using_hard_and_soft_cast(async);

            AssertSql(
                @"SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[PeriodEnd], [l].[PeriodStart], [l].[ThreatLevel], [l].[ThreatLevelByte], [l].[ThreatLevelNullableByte], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId]
FROM [LocustLeaders] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [l]
WHERE ([l].[Discriminator] = N'LocustCommander') AND (([l].[HighCommandId] <> 0) OR [l].[HighCommandId] IS NULL)",
                //
                @"SELECT [l].[Name], [l].[Discriminator], [l].[LocustHordeId], [l].[PeriodEnd], [l].[PeriodStart], [l].[ThreatLevel], [l].[ThreatLevelByte], [l].[ThreatLevelNullableByte], [l].[DefeatedByNickname], [l].[DefeatedBySquadId], [l].[HighCommandId]
FROM [LocustLeaders] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [l]
WHERE ([l].[Discriminator] = N'LocustCommander') AND (([l].[HighCommandId] <> 0) OR [l].[HighCommandId] IS NULL)");
        }

        public override async Task Accessing_property_of_optional_navigation_in_child_projection_works(bool async)
        {
            await base.Accessing_property_of_optional_navigation_in_child_projection_works(async);

            AssertSql(
                @"SELECT CASE
    WHEN [g].[Nickname] IS NOT NULL AND [g].[SquadId] IS NOT NULL THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END, [t].[Id], [g].[Nickname], [g].[SquadId], [t0].[Nickname], [t0].[Id], [t0].[SquadId]
FROM [Tags] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [t]
LEFT JOIN [Gears] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [g] ON ([t].[GearNickName] = [g].[Nickname]) AND ([t].[GearSquadId] = [g].[SquadId])
LEFT JOIN (
    SELECT [g0].[Nickname], [w].[Id], [g0].[SquadId], [w].[OwnerFullName]
    FROM [Weapons] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [w]
    LEFT JOIN [Gears] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [g0] ON [w].[OwnerFullName] = [g0].[FullName]
) AS [t0] ON [g].[FullName] = [t0].[OwnerFullName]
ORDER BY [t].[Note], [t].[Id], [g].[Nickname], [g].[SquadId], [t0].[Id], [t0].[Nickname], [t0].[SquadId]");
        }

        public override async Task Acessing_reference_navigation_collection_composition_generates_single_query(bool async)
        {
            await base.Acessing_reference_navigation_collection_composition_generates_single_query(async);

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [t].[Id], [t].[IsAutomatic], [t].[Name], [t].[Id0]
FROM [Gears] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [g]
LEFT JOIN (
    SELECT [w].[Id], [w].[IsAutomatic], [w0].[Name], [w0].[Id] AS [Id0], [w].[OwnerFullName]
    FROM [Weapons] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [w]
    LEFT JOIN [Weapons] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [w0] ON [w].[SynergyWithId] = [w0].[Id]
) AS [t] ON [g].[FullName] = [t].[OwnerFullName]
ORDER BY [g].[Nickname], [g].[SquadId], [t].[Id], [t].[Id0]");
        }

        public override async Task All_with_optional_navigation_is_translated_to_sql(bool async)
        {
            await base.All_with_optional_navigation_is_translated_to_sql(async);

            AssertSql(
                @"SELECT CASE
    WHEN NOT EXISTS (
        SELECT 1
        FROM [Gears] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [g]
        LEFT JOIN [Tags] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [t] ON ([g].[Nickname] = [t].[GearNickName]) AND ([g].[SquadId] = [t].[GearSquadId])
        WHERE ([t].[Note] = N'Foo') AND [t].[Note] IS NOT NULL) THEN CAST(1 AS bit)
    ELSE CAST(0 AS bit)
END");
        }

        public override async Task Anonymous_projection_take_followed_by_projecting_single_element_from_collection_navigation(bool async)
        {
            await base.Anonymous_projection_take_followed_by_projecting_single_element_from_collection_navigation(async);

            AssertSql(
                @"");
        }

        public override async Task Any_with_optional_navigation_as_subquery_predicate_is_translated_to_sql(bool async)
        {
            await base.Any_with_optional_navigation_as_subquery_predicate_is_translated_to_sql(async);

            AssertSql(
                @"SELECT [s].[Name]
FROM [Squads] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [s]
WHERE NOT (EXISTS (
    SELECT 1
    FROM [Gears] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [g]
    LEFT JOIN [Tags] FOR SYSTEM_TIME AS OF '2010-01-01T00:00:00.0000000' AS [t] ON ([g].[Nickname] = [t].[GearNickName]) AND ([g].[SquadId] = [t].[GearSquadId])
    WHERE ([s].[Id] = [g].[SquadId]) AND ([t].[Note] = N'Dom''s Tag')))");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Set_operation_on_temporal_same_ops(bool async)
        {
            using var ctx = CreateContext();
            var date = new DateTime(2015, 1, 1);
            var query = ctx.Set<Gear>().TemporalAsOf(date).Where(g => g.HasSoulPatch).Concat(ctx.Set<Gear>().TemporalAsOf(date));
            var expected = async
                ? await query.ToListAsync()
                : query.ToList();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOfBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[PeriodEnd], [g].[PeriodStart], [g].[Rank]
FROM [Gears] FOR SYSTEM_TIME AS OF '2015-01-01T00:00:00.0000000' AS [g]
WHERE [g].[HasSoulPatch] = CAST(1 AS bit)
UNION ALL
SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOfBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[PeriodEnd], [g0].[PeriodStart], [g0].[Rank]
FROM [Gears] FOR SYSTEM_TIME AS OF '2015-01-01T00:00:00.0000000' AS [g0]");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Set_operation_with_inheritance_on_temporal_same_ops(bool async)
        {
            using var ctx = CreateContext();
            var date = new DateTime(2015, 1, 1);
            var query = ctx.Set<Officer>().TemporalAsOf(date).Concat(ctx.Set<Officer>().TemporalAsOf(date));
            var expected = async
                ? await query.ToListAsync()
                : query.ToList();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOfBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[PeriodEnd], [g].[PeriodStart], [g].[Rank]
FROM [Gears] FOR SYSTEM_TIME AS OF '2015-01-01T00:00:00.0000000' AS [g]
WHERE [g].[Discriminator] = N'Officer'
UNION ALL
SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOfBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[PeriodEnd], [g0].[PeriodStart], [g0].[Rank]
FROM [Gears] FOR SYSTEM_TIME AS OF '2015-01-01T00:00:00.0000000' AS [g0]
WHERE [g0].[Discriminator] = N'Officer'");
        }

        [ConditionalTheory]
        [MemberData(nameof(IsAsyncData))]
        public virtual async Task Set_operation_on_temporal_different_dates(bool async)
        {
            using var ctx = CreateContext();
            var date1 = new DateTime(2015, 1, 1);
            var date2 = new DateTime(2018, 1, 1);
            var query = ctx.Set<Gear>().TemporalAsOf(date1).Where(g => g.HasSoulPatch).Concat(ctx.Set<Gear>().TemporalAsOf(date2));



            var expected = async
                ? await query.ToListAsync()
                : query.ToList();

            AssertSql(
                @"SELECT [g].[Nickname], [g].[SquadId], [g].[AssignedCityName], [g].[CityOfBirthName], [g].[Discriminator], [g].[FullName], [g].[HasSoulPatch], [g].[LeaderNickname], [g].[LeaderSquadId], [g].[PeriodEnd], [g].[PeriodStart], [g].[Rank]
FROM [Gears] FOR SYSTEM_TIME AS OF '2015-01-01T00:00:00.0000000' AS [g]
WHERE [g].[HasSoulPatch] = CAST(1 AS bit)
UNION ALL
SELECT [g0].[Nickname], [g0].[SquadId], [g0].[AssignedCityName], [g0].[CityOfBirthName], [g0].[Discriminator], [g0].[FullName], [g0].[HasSoulPatch], [g0].[LeaderNickname], [g0].[LeaderSquadId], [g0].[PeriodEnd], [g0].[PeriodStart], [g0].[Rank]
FROM [Gears] FOR SYSTEM_TIME AS OF '2015-01-01T00:00:00.0000000' AS [g0]");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }

    public class PointInTimeQueryRewriter : ExpressionVisitor
    {
        private static readonly MethodInfo _setMethodInfo
            = typeof(ISetSource).GetMethod(nameof(ISetSource.Set));

        private static readonly MethodInfo _asOfMethodInfo
            = typeof(SqlServerQueryableExtensions).GetMethod(nameof(SqlServerQueryableExtensions.TemporalAsOf));

        private readonly DateTime _pointInTime;

        // TODO: need model instead
        private readonly List<Type> _temporalEntityTypes;

        public PointInTimeQueryRewriter(DateTime pointInTime, List<Type> temporalEntityTypes)
        {
            _pointInTime = pointInTime;
            _temporalEntityTypes = temporalEntityTypes;
        }

        protected override Expression VisitExtension(Expression extensionExpression)
        {
            if (extensionExpression is QueryRootExpression queryRootExpression
                && queryRootExpression.EntityType.GetRootType().IsTemporal())
            {
                return new TemporalAsOfQueryRootExpression(
                    queryRootExpression.QueryProvider,
                    queryRootExpression.EntityType,
                    _pointInTime);
            }

            return base.VisitExtension(extensionExpression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Method.IsGenericMethod
                && methodCallExpression.Method.GetGenericMethodDefinition() == _setMethodInfo)
            {
                var entityType = methodCallExpression.Method.GetGenericArguments()[0];
                if (_temporalEntityTypes.Contains(entityType))
                {
                    var method = _asOfMethodInfo.MakeGenericMethod(methodCallExpression.Method.GetGenericArguments()[0]);

                    // temporal methods are defined on DBSet so we need to hard cast here.
                    // This rewrite is only done for actual queries (and not expected), so the cast is safe to do
                    var dbSetType = typeof(DbSet<>).MakeGenericType(entityType);

                    return Expression.Call(
                        method,
                        Expression.Convert(
                            methodCallExpression,
                            dbSetType),
                        Expression.Constant(_pointInTime));
                }
            }

            return base.VisitMethodCall(methodCallExpression);
        }
    }
}
