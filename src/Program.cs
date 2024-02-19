using Hangfire;
using Hangfire.Storage.SQLite;
using src.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<IServiceManagement, ServiceManagement>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();
app.MapHangfireDashboard();

app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();

app.Run();
