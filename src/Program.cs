using Hangfire;
using Hangfire.Storage.SQLite;
using HangfireBasicAuthenticationFilter;
using src.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHangfireServer();
builder.Services.AddTransient<IServiceManagement, ServiceManagement>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    DashboardTitle = "Drivers Dashboard",
    Authorization = new [] {
        new HangfireCustomBasicAuthenticationFilter()
        {
            User = "Jed",
            Pass = "Passw0rd"
        }
    }
});
app.MapHangfireDashboard();

RecurringJob.AddOrUpdate<IServiceManagement>(x => x.SyncData(), "0 * * ? * *");


app.Run();
