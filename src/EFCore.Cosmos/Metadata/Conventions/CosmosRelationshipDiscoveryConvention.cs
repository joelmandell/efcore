// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures relationships between entity types based on the navigation properties
    ///     as long as there is no ambiguity as to which is the corresponding inverse navigation.
    ///     All navigations are assumed to be targeting owned entity types for Cosmos.
    /// </summary>
    public class CosmosRelationshipDiscoveryConvention : RelationshipDiscoveryConvention
    {
        /// <summary>
        ///     Creates a new instance of <see cref="RelationshipDiscoveryConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public CosmosRelationshipDiscoveryConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <summary>
        ///     Finds or tries to create an entity type target for the given navigation member.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the referencing entity type. </param>
        /// <param name="targetClrType"> The CLR type of the target entity type. </param>
        /// <param name="navigationMemberInfo"> The navigation member. </param>
        /// <param name="shouldCreate"> Whether an entity type should be created if one doesn't currently exist. </param>
        /// <returns> The builder for the target entity type or <see langword="null"/> if it can't be created. </returns>
        protected override IConventionEntityTypeBuilder? TryGetTargetEntityTypeBuilder(
            IConventionEntityTypeBuilder entityTypeBuilder,
            Type targetClrType,
            MemberInfo navigationMemberInfo,
            bool shouldCreate = true)
            => ((InternalEntityTypeBuilder)entityTypeBuilder)
#pragma warning disable EF1001 // Internal EF Core API usage.
                .GetTargetEntityTypeBuilder(
                    targetClrType,
                    navigationMemberInfo,
                    shouldCreate ? ConfigurationSource.Convention : null,
                    targetShouldBeOwned: true);
#pragma warning restore EF1001 // Internal EF Core API usage.

        /// <summary>
        ///     Returns a value indicating whether the given entity type should be owned.
        /// </summary>
        /// <param name="targetEntityType"> Target entity type. </param>
        /// <returns> <see langword="true"/> if the given entity type should be owned. </returns>
        protected override bool ShouldBeOwned(IConventionEntityType targetEntityType)
            => targetEntityType.GetConfigurationSource() == ConfigurationSource.Convention
                || base.ShouldBeOwned(targetEntityType);
    }
}
