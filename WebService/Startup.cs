using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StackOverflowData;
using AutoMapper;
using StackOverflowData.Functions;
using WebService.Models;

namespace WebService {
    public class Startup {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc();
            services.AddSingleton<IDataService, DataService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            MapperConfig();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            //app.Run(async (context) => { await context.Response.WriteAsync("Hello World!"); });
        }

        private void MapperConfig()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<GetHistoryResult, HistoryModel>();
                cfg.CreateMap<GetPostOrCommentResult, AnswerModel>();
                cfg.CreateMap<GetPostOrCommentResult, CommentModel>();
                cfg.CreateMap<GetPostOrCommentResult, PostModel>();
                cfg.CreateMap<GetMarkedResult, MarkModel>();
                cfg.CreateMap<SearchResult, SearchModel>();
                //.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name));
            });
        }
    }
}