using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoPersistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;
using MongoPersistence.Domains;
using Microsoft.OpenApi.Models;
using Common.Repositories;
using Common.Domains;
using CryptoNotifier.Services;
using CryptoNotifier.Models;
using MongoPersistence.MongoPersistence;

namespace CryptoNotifier
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }
        public static IMapper Mapper { get; internal set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
            .AddApiExplorer();


            services.AddSingleton<MongoDbManager>();
            services.AddSingleton<ICryptoDataRepository, CryptoDataRepository>();
            services.AddSingleton<IMailService, MailService>();
            services.AddSingleton<ICryptoAnalyzer, CryptoAnalyzer>();

            services.AddControllers();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Crypto Data v1",
                    Description = "",
                    TermsOfService = new Uri("http://www.test.com/"),
                    Contact = new OpenApiContact()
                    {
                        Name = "Luis Felipe",
                        Email = "luisfmazzu@hotmail.com",
                        Url = new Uri("http://www.test.com/")
                    }
                });
            });

            CryptoAnalyzer cryptoAnalyzer = new CryptoAnalyzer();
            MongoDbManager mongoDbManager = new MongoDbManager(Configuration);
            CryptoDataRepository cryptoDataRepository = new CryptoDataRepository(mongoDbManager);
            WalletAlertsRepository walletAlertsRepository = new WalletAlertsRepository(mongoDbManager);
            TokenHistoryRepository tokenHistoryRepository = new TokenHistoryRepository(mongoDbManager);
            TwilioPhoneCallService phoneCallService = new TwilioPhoneCallService(Configuration);
            IMailService mailService = new MailService(Configuration);
            cryptoAnalyzer.InitializeClient(cryptoDataRepository, mailService, phoneCallService, walletAlertsRepository, tokenHistoryRepository);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Crypto data V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();

            var config = new MapperConfiguration(configuration =>
            {
                configuration.AllowNullCollections = true;
                configuration.CreateMap<ICryptoDataDomain, CryptoDataDto>();
                configuration.CreateMap<CryptoDataForCreationDto, ICryptoDataDomain>();
                configuration.CreateMap<CryptoDataForUpdateDto, ICryptoDataDomain>();
            });

            Mapper = config.CreateMapper();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
