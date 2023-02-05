using Web;
using StackExchange.Profiling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddMongoClientWithProfiling(builder.Configuration.GetConnectionString("mongoDB")) // Setup MongoDriver in the DI using MiniProfiler to profiling queries.
                .AddMyServices()
                .AddMiniProfiler(options =>
                {
                    options.RouteBasePath = "/profiler";

                    options.IgnoredPaths.Add("/lib");
                    options.IgnoredPaths.Add("/css");
                    options.IgnoredPaths.Add("/js");
                    options.IgnoredPaths.Add("favicon.ico");
                });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseMiniProfiler();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
