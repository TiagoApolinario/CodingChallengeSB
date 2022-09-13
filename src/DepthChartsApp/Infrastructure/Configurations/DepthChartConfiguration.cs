using DepthChartsApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DepthChartsApp.Infrastructure.Configurations;

public sealed class DepthChartConfiguration : IEntityTypeConfiguration<DepthChart>
{
    public void Configure(EntityTypeBuilder<DepthChart> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.TeamId);

        builder.HasMany(x => x.Positions)
            .WithOne(x => x.DepthChart)
            .IsRequired();
    }
}