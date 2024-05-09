using System.Net.Http.Headers;
using System.Text.Json;

namespace Templum.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient("Zotero", config =>
            {
                config.BaseAddress = new Uri("https://api.zotero.org/");
                config.Timeout = new TimeSpan(0, 0, 30);
                config.DefaultRequestHeaders.Clear();
                config.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("ZOTERO_BEARER"));
            });

            builder.Services.AddHttpClient("Strapi", config =>
            {
                config.BaseAddress = new Uri("http://host.docker.internal:1337/");
                config.Timeout = new TimeSpan(0, 0, 30);
                config.DefaultRequestHeaders.Clear();
            });

            builder.Services.AddSingleton<IZoteroClient, ZoteroClient>();
            builder.Services.AddSingleton<IStrapiClient, StrapiClient>();
            builder.Services.AddGithubJsonDb(options => 
            {
                options.Branch = "gh-pages";
                options.FilePath = "data";
                options.RepoName = "Templum-Data";
                options.RepoOwner = "8BitSensei";
                options.PersonalAccessToken = Environment.GetEnvironmentVariable("GITHUB_PAT");
                options.JsonSerializerOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
