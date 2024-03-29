﻿using FlameRestaurant.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlameRestaurant.Domain.Models.DbContexts.Configurations
{
    public class ProductRateEntityTypeConfiguration : IEntityTypeConfiguration<ProductRate>
    {
        public void Configure(EntityTypeBuilder<ProductRate> builder)
        {
            builder.HasKey(key =>
            new
            {
                key.ProductId,
                key.CreatedByUserId
            });

            builder.ToTable("ProductRates");
        }
    }
}
