using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AECPulse.Infrastructure.Data;
using AEC.AI.Core.Extensions;
using AEC.AI.Core.Tools;
using AEC.AI.Core.Tools.Core;
using AEC.AI.Core.Services;
using AEC.AI.Core.Tools.Implementations;
using AEC.AI.Core.Agents;
using AECPulse.Infrastructure.AI;


var builder = WebApplication.CreateBuilder(args);

// Add this to register the Controller system
builder.Services.AddControllers();

// 1. Setup SQL Server (Existing)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Setup the New AI Engine (Replacing Semantic Kernel)
// This uses your custom extension method from AEC.AI.Core
builder.Services.AddAiProviders(builder.Configuration);

builder.Services.AddDistributedMemoryCache();
// 2.5 Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") // our Vite/React URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 3. Register the Tool Registry and AEC-specific tools
// Chaining .AddTool<T> allows you to register tools from any project layer
builder.Services.AddTools(builder.Configuration)
                //.AddTool<ProjectDataTool>() // our data bridge
                .AddTool<CalculatorTool>()     // Utility
                .AddTool<DateTimeTool>();      // Utility

// 3b. Scoped tool provider — infrastructure implementation
builder.Services.AddScoped<IScopedToolProvider, AecScopedToolProvider>();
 
// 3c. Agent service — scoped, builds agents per request
builder.Services.AddScoped<IAgentService, AgentService>();                

// 4. Standard API services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// 5. CRITICAL: Bootstrap the Tool Registry
// We populate the registry once the DI container is built
var registry = app.Services.GetRequiredService<ToolRegistry>();
registry.PopulateFromDI(app.Services);

// Enable middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors();


// // 6. The Modern Agent Endpoint
// // Using the AgentBuilder from your AEC.AI.Core library
// app.MapPost("/analyze", async (string prompt, IChatClient client, ToolRegistry registry) =>
// {
//     var agent = new AgentBuilder("ProjectAnalyst", client)
//         .WithSystemPrompt("You are an AEC Analyst. Use database tools to provide accurate project insights.")
//         .WithTools(registry) // Injects all registered tools
//         .WithTemperature(0.2f)
//         .Build();

//     var result = await agent.RunAsync(prompt);
    
//     return Results.Ok(new { 
//         Analysis = result.Output,
//         TokensUsed = result.InputTokens + result.OutputTokens,
//         Duration = $"{result.Duration.TotalSeconds:F2}s"
//     });
// });

app.MapGet("/", () => new { 
    Status = "AEC-Pulse AI API is Online (Lightweight Core)", 
    Version = "1.1",
    Framework = ".NET 10",
    AgentsLoaded = true,
    AvailableTools = app.Services.GetRequiredService<ToolRegistry>().GetRegisteredNames()
});

app.UseAuthorization(); // Recommended if  use Controllers
app.MapControllers();   // This "plugs in" ChatController

app.Run();

