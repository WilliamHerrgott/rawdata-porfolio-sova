using System;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackOverflowData;
using StackOverflowData.Functions;
using WebService.Models;

namespace WebService {
    public class Startup {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddCors();
            services.AddMvc();
            services.AddSingleton<IDataService, DataService>();

            services.AddCors();

            var key = Encoding.UTF8.GetBytes(Configuration["security:key"]);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option => {
                    option.TokenValidationParameters = new TokenValidationParameters {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            MapperConfig();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseAuthentication();
            app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }

        private void MapperConfig() {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<GetHistoryResult, HistoryModel>();
                cfg.CreateMap<GetPostOrCommentResult, AnswerModel>();
                cfg.CreateMap<GetPostOrCommentResult, CommentModel>();
                cfg.CreateMap<GetPostOrCommentResult, PostModel>();
                cfg.CreateMap<GetMarkedResult, MarkModel>();
                cfg.CreateMap<SearchResult, SearchModel>();
                cfg.CreateMap<GetUserResult, GetUserModel>();
                cfg.CreateMap<SearchResultWords, SearchWordsModel>();
                
            });
        }
    }
}