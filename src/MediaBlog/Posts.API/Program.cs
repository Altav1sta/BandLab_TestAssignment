using Common.Messaging;
using Microsoft.EntityFrameworkCore;
using Posts.API.Messaging.Consumers;
using Posts.API.Services;
using Posts.API.Services.Interfaces;
using Posts.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddRabbitMQ(builder.Configuration);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddDbContext<PostsDbContext>(x =>
    x.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ICommentsService, CommentsService>();
builder.Services.AddScoped<IPostsService, PostsService>();

builder.Services.AddHostedService<CreateCommentRequestedEventConsumer>();
builder.Services.AddHostedService<CreatePostRequestedEventConsumer>();
builder.Services.AddHostedService<DeleteCommentRequestedEventConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
using var context = scope.ServiceProvider.GetRequiredService<PostsDbContext>();

context.Database.Migrate();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
