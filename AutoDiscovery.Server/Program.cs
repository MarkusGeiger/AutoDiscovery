var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoDiscoveryService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/descriptiondocument.xml", async () => {
  var xmlText = await File.ReadAllTextAsync("devicedescription.xml");
  //System.Console.WriteLine(xmlText);
  return Results.Text(xmlText, contentType: "application/xml");
});

app.Run();
