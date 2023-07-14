﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Sqlite.Internal;
using Microsoft.EntityFrameworkCore.TestModels.JsonQuery;

namespace Microsoft.EntityFrameworkCore.Query;

public class JsonQuerySqliteTest : JsonQueryTestBase<JsonQuerySqliteFixture>
{
    public JsonQuerySqliteTest(JsonQuerySqliteFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);

    }

    public override async Task Project_json_entity_FirstOrDefault_subquery(bool async)
        => Assert.Equal(
            SqliteStrings.ApplyNotSupported,
            (await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Project_json_entity_FirstOrDefault_subquery(async)))
            .Message);

    public override async Task Project_json_entity_FirstOrDefault_subquery_deduplication(bool async)
        => Assert.Equal(
            SqliteStrings.ApplyNotSupported,
            (await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Project_json_entity_FirstOrDefault_subquery_deduplication(async)))
            .Message);

    public override async Task Project_json_entity_FirstOrDefault_subquery_deduplication_and_outer_reference(bool async)
        => Assert.Equal(
            SqliteStrings.ApplyNotSupported,
            (await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Project_json_entity_FirstOrDefault_subquery_deduplication_and_outer_reference(async)))
            .Message);

    public override async Task Project_json_entity_FirstOrDefault_subquery_deduplication_outer_reference_and_pruning(bool async)
        => Assert.Equal(
            SqliteStrings.ApplyNotSupported,
            (await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Project_json_entity_FirstOrDefault_subquery_deduplication_outer_reference_and_pruning(async)))
            .Message);

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task JsonExtract_Query_Single_Path(bool async)
    {
        var ctx = Fixture.CreateContext();

        var actual = ctx.JsonEntitiesBasicString.Select(c => new { Result = EF.Functions.JsonExtract(c.OwnedReferenceRoot, "$.Number") });

        var result = async ? await actual.ToListAsync() : actual.ToList();
        var queryString = actual.ToQueryString();

        Assert.Equal("10", result.FirstOrDefault()?.Result);
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task JsonExtract_Query_Multiple_Paths(bool async)
    {
        var ctx = Fixture.CreateContext();

        var actual = ctx.JsonEntitiesBasicString.Select(c => new { Result = EF.Functions.JsonExtract(c.OwnedReferenceRoot, "$.Number", "$.Name") });

        var result = async ? await actual.ToListAsync() : actual.ToList();
        var queryString = actual.ToQueryString();

        Assert.Equal($@"[10,""e1_r""]", result.FirstOrDefault()?.Result);
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task FromSqlInterpolated_on_entity_with_json_with_predicate(bool async)
    {
        var parameter = new SqliteParameter { ParameterName = "prm", Value = 1 };
        await AssertQuery(
            async,
            ss => ((DbSet<JsonEntityBasic>)ss.Set<JsonEntityBasic>()).FromSql(
                Fixture.TestStore.NormalizeDelimitersInInterpolatedString($"SELECT * FROM [JsonEntitiesBasic] AS j WHERE [j].[Id] = {parameter}")),
            ss => ss.Set<JsonEntityBasic>(),
            entryCount: 40);

        AssertSql(
"""
prm='1' (DbType = String)

SELECT "m"."Id", "m"."EntityBasicId", "m"."Name", "m"."OwnedCollectionRoot", "m"."OwnedReferenceRoot"
FROM (
    SELECT * FROM "JsonEntitiesBasic" AS j WHERE "j"."Id" = @prm
) AS "m"
""");
    }

    [ConditionalTheory(Skip = "issue #30326")]
    public override async Task Json_predicate_on_bool_converted_to_int_zero_one(bool async)
    {
        await base.Json_predicate_on_bool_converted_to_int_zero_one(async);

        AssertSql();
    }

    [ConditionalTheory(Skip = "issue #30326")]
    public override async Task Json_predicate_on_bool_converted_to_string_True_False(bool async)
    {
        await base.Json_predicate_on_bool_converted_to_string_True_False(async);

        AssertSql();
    }

    [ConditionalTheory(Skip = "issue #30326")]
    public override async Task Json_predicate_on_bool_converted_to_string_Y_N(bool async)
    {
        await base.Json_predicate_on_bool_converted_to_string_Y_N(async);

        AssertSql();
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
