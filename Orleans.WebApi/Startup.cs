using Orleans.Configuration;
using Orleans.Hosting;

namespace Orleans.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOrleansClient(opt =>
            {
                opt.UseLocalhostClustering();
                opt.Configure<ClusterOptions>((options =>
                {
                    options.ClusterId = "cluster";
                    options.ServiceId = "MyOrleansService";
                }));
                opt.AddMemoryStreams("SMSProvider");
            });
            services.AddControllers();
            services.AddSwaggerGen();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
