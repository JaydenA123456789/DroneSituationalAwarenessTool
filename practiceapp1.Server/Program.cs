using Microsoft.Extensions.DependencyInjection;

using DroneSituationalAwarenessTool.Server.EntityStateFunctionality;
using DroneSituationalAwarenessTool.Server.SignalRHubs;
using SharedLibraries.EntityFunctionality;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientWithCredentials", policy =>
    {
        policy
            .WithOrigins("https://localhost:50550") // Client address
            .WithOrigins("https://localhost:50551") // Client address backup
            /*   I think CORS isnt an issue because its not cross origin to the other .net projects
            .WithOrigins("https://localhost:7225") // Air Data Micro Service
            .WithOrigins("https://localhost:7103") // Maritime Data Micro Service
            .WithOrigins("https://localhost:7272") // Mavlink Micro Service
            */
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddTransient<IMapEntity, Debug_GenericEntity>();
builder.Services.AddTransient<IMapEntity, AirEntity>();
builder.Services.AddTransient<IMapEntity, DroneEntity>();
builder.Services.AddTransient<IMapEntity, MaritimeEntity>();

builder.Services.AddSingleton<EntityState>();

var app = builder.Build();





app.UseDefaultFiles();
app.UseStaticFiles();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

//signalR hubs
app.UseCors("AllowClientWithCredentials");
app.MapHub<ClientHub>("/ClientHub");
app.MapHub<InterfaceHub>("/InterfaceHub");

app.Services.GetRequiredService<EntityState>();

app.Run();
