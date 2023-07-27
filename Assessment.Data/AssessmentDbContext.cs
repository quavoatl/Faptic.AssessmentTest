using Microsoft.EntityFrameworkCore;

namespace Assessment.Data
{
    public class AssessmentDbContext : DbContext
    {
        public AssessmentDbContext()
        {
        }

        public AssessmentDbContext(DbContextOptions<AssessmentDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UriConfiguration> UriConfigurations { get; set; }
        public virtual DbSet<CloseAggregate> CloseAggregates { get; set; }
        public virtual DbSet<CloseApiResponse> CloseApiResponses { get; set; }
        public virtual DbSet<ApiSource> ApiSources { get; set; }

        protected override void OnConfiguring
            (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "AssessmentDb");
        }

        //entity config
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var urlConfigurationSeed = new List<UriConfiguration>
            {
                new UriConfiguration(1,1,"Bitstamp1hCloseEndpoint","https://www.bitstamp.net/api/v2/ohlc/btcusd/?step=3600&limit=1&start=startPointPlaceholder"),
                new UriConfiguration(2,2,"Bitfinex1hCloseEndpoint","https://api-pub.bitfinex.com/v2/candles/trade:1h:tBTCUSD/hist?start=startPointPlaceholder&end=endPointPlaceholder&limit=1"),
                new UriConfiguration(3,3,"Coinbase1hCloseEndpoint","https://api.exchange.coinbase.com/products/BTC-USD/candles?granularity=60&start=startPointPlaceholder&end=endPointPlaceholder")
            };

            var apiSourcesSeed = new List<ApiSource>
            {
                new ApiSource { Id = 1, ApiName = "Bitstamp" },
                new ApiSource { Id = 2, ApiName = "Bitfinex" },
                new ApiSource { Id = 3, ApiName = "Coinbase" },
            };

            modelBuilder.Entity<UriConfiguration>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.ApiSource)
                    .WithMany(y => y.UrlConfigurations)
                    .HasForeignKey(x => x.ApiSourceId);
                entity.HasData(urlConfigurationSeed);
            });

            modelBuilder.Entity<CloseAggregate>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<CloseApiResponse>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.ApiSource)
                    .WithMany(y => y.CloseApiResponses)
                    .HasForeignKey(x => x.ApiSourceId);
            });

            modelBuilder.Entity<ApiSource>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasData(apiSourcesSeed);
            });
        }

    }
}