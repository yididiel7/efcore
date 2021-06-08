// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query
{
    [SqlServerCondition(SqlServerCondition.SupportsTemporalTablesCascadeDelete)]
    public class TemporalManyToManyQuerySqlServerTest : ManyToManyQueryRelationalTestBase<TemporalManyToManyQuerySqlServerFixture>
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public TemporalManyToManyQuerySqlServerTest(TemporalManyToManyQuerySqlServerFixture fixture, ITestOutputHelper testOutputHelper)
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
                typeof(TestModels.ManyToManyModel.EntityOne),
                typeof(TestModels.ManyToManyModel.EntityTwo),
                typeof(TestModels.ManyToManyModel.EntityThree),
                typeof(TestModels.ManyToManyModel.EntityCompositeKey),
                typeof(TestModels.ManyToManyModel.EntityRoot),
                typeof(TestModels.ManyToManyModel.EntityBranch),
                typeof(TestModels.ManyToManyModel.EntityLeaf),
            };

            var rewriter = new PointInTimeQueryRewriter(Fixture.ChangesDate, temporalEntityTypes);

            return rewriter.Visit(serverQueryExpression);
        }

        public override async Task Skip_navigation_all(bool async)
        {
            await base.Skip_navigation_all(async);

            AssertSql(
                string.Format(@"SELECT [e].[Id], [e].[Name], [e].[PeriodEnd], [e].[PeriodStart]
FROM [EntityOnes] FOR SYSTEM_TIME AS OF '{0}' AS [e]
WHERE NOT EXISTS (
    SELECT 1
    FROM [JoinOneToTwo] FOR SYSTEM_TIME AS OF '{0}' AS [j]
    INNER JOIN [EntityTwos] FOR SYSTEM_TIME AS OF '{0}' AS [e0] ON [j].[TwoId] = [e0].[Id]
    WHERE ([e].[Id] = [j].[OneId]) AND NOT ([e0].[Name] LIKE N'%B%'))", Fixture.ChangeDateLiteral));
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
