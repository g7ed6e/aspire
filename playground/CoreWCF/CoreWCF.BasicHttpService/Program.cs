using Contracts;
using CoreWCF;
using CoreWCF.BasicHttpService;
using CoreWCF.Configuration;
using CoreWCF.Description;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddServiceModelServices();
builder.Services.AddTransient<EchoService>();
builder.Services.AddSingleton<IServiceBehavior, LoggingServiceBehavior>();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<EchoService>();
    serviceBuilder.AddServiceEndpoint<EchoService, IEchoService>(new BasicHttpBinding(), "/EchoService.svc");
});

app.MapGet("/", () => "Hello World!");

app.Run();
