using Common.Caching;
using Common.Messaging;
using Posts.API.SDK;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddRabbitMQ(builder.Configuration, withProducer: true);
builder.Services.AddPostsApiClient(builder.Configuration["PostsApiUrl"]!);
builder.Services.AddRedis(builder.Configuration.GetConnectionString("Redis")!);
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
