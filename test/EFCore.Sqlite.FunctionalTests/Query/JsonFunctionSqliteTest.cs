// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Sqlite.Internal;
using Microsoft.EntityFrameworkCore.TestModels.JsonQuery;
using Microsoft.EntityFrameworkCore.TestModels.JsonTest;

namespace Microsoft.EntityFrameworkCore.Query;

public class JsonFunctionSqliteTest
{
    public JsonFunctionSqliteTest()
    {

    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public async Task JsonExtract_Query(bool async)
    {
        var ctx = new JsonTestSqliteContext();
        var created = async ? await ctx.Database.EnsureCreatedAsync() : ctx.Database.EnsureCreated();

        if (async ? !(await ctx.JsonEntitiesBasicString.AnyAsync()) : !ctx.JsonEntitiesBasicString.Any())
        {
            ctx.JsonEntitiesBasicString.Add(new() { Id = 1, Name = "Json_Extract sqlite test", OwnedReferenceRoot = "{\"Name\":\"e1_r\",\"Number\":10,\"OwnedCollectionBranch\":[{\"Date\":\"2101-01-01T00:00:00\",\"Enum\":\"Two\",\"Fraction\":10.1,\"NullableEnum\":\"One\",\"OwnedCollectionLeaf\":[{\"SomethingSomething\":\"e1_r_c1_c1\"},{\"SomethingSomething\":\"e1_r_c1_c2\"}],\"OwnedReferenceLeaf\":{\"SomethingSomething\":\"e1_r_c1_r\"}},{\"Date\":\"2102-01-01T00:00:00\",\"Enum\":\"Three\",\"Fraction\":10.2,\"NullableEnum\":\"Two\",\"OwnedCollectionLeaf\":[{\"SomethingSomething\":\"e1_r_c2_c1\"},{\"SomethingSomething\":\"e1_r_c2_c2\"}],\"OwnedReferenceLeaf\":{\"SomethingSomething\":\"e1_r_c2_r\"}}],\"OwnedReferenceBranch\":{\"Date\":\"2100-01-01T00:00:00\",\"Enum\":\"One\",\"Fraction\":10.0,\"NullableEnum\":null,\"OwnedCollectionLeaf\":[{\"SomethingSomething\":\"e1_r_r_c1\"},{\"SomethingSomething\":\"e1_r_r_c2\"}],\"OwnedReferenceLeaf\":{\"SomethingSomething\":\"e1_r_r_r\"}}}" });
            var _ = async ? await ctx.SaveChangesAsync() : ctx.SaveChanges();
        }

        var actual = ctx.JsonEntitiesBasicString.Select(c => new { Result = EF.Functions.JsonExtract(c.OwnedReferenceRoot, "$.Number", "$.Name") });

        var result = async ? await actual.ToListAsync() : actual.ToList();
        var queryString = actual.ToQueryString();

        Assert.Equal($@"[10,""e1_r""]", result.FirstOrDefault()?.Result);
    }

    public static IEnumerable<object[]> IsAsyncData = new[] { new object[] { false }, new object[] { true } };
}
