using Application.Abstractions;
using Application.Posts.CommandHandlers;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Application.Posts.Commands;
using Application.Posts.Queries;
using Application.Posts.QueryHandlers;
using Domain.Entities;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using IGetPostByIdHandler = Application.Posts.CommandHandlers.IGetPostByIdHandler;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SocialDbcontext>(options =>
    options.UseSqlite(conn));
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddTransient<ICreatePostHandler, CreatePostHandler>();
builder.Services.AddTransient<IDeletePostHandler, DeletePostHandler>();
builder.Services.AddTransient<IUpdatePostHandler, UpdatePostHandler>();
builder.Services.AddTransient<IGetAllPostsHandler, GetAllPostsHandler>();


var app = builder.Build();

// --- START DIAGNOSTIC LOGGING ---
app.Use(async (context, next) =>
{
    Console.WriteLine($"---> Request Received: {context.Request.Method} {context.Request.Path}");
    await next(context); // Call the next middleware in the pipeline
    Console.WriteLine($"<--- Response Sent: {context.Response.StatusCode} for {context.Request.Path}");
});

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<IExceptionHandlerFeature>();
        if (exception != null)
        {
            await context.Response.WriteAsync($"{{\"error\": \"{exception.Error.Message}\"}}");
        }
    });
});



app.UseHttpsRedirection();

app.UseRouting();

// Endpoint 1: Plain Text
app.MapGet("/ping", () => "pong");

// Endpoint 2: Minimal JSON
app.MapGet("/minimal-json", () => Results.Ok(new { Message = "Minimal API works" }));

app.MapGet("/api/post/{id}", async (int id, [FromServices] IGetPostByIdHandler getPostHandler) =>
{
    var simpleData = new List<object>
    {
        new { Id = 998, Message = "Test Item 1" },
        new { Id = 999, Message = "Test Item 2" }
    };
    await getPostHandler.Handle(id);
    return Results.Ok(simpleData);
}).WithName("GetPostById");

app.MapPost("/api/posts", async (Post post, ICreatePostHandler createPostHandler) =>
{
    var createPost = new CreatePost { PostContent = post.Content, PostComments = post.Comments };
    var createdPost = await createPostHandler.Handle(createPost);  // ✅ Await the async method
    return Results.Created($"/api/posts/{createdPost.Id}", createdPost);
});


app.MapGet("/api/posts", async ([FromServices] IGetAllPostsHandler getAllPostsHandler) =>
{
    try
    {
        var posts = await getAllPostsHandler.Handle(new GetAllPosts());
        return Results.Ok(posts);
    }
    catch (Exception ex)
    {
        // Consider more specific exception handling and logging
        Console.WriteLine($"Error getting posts: {ex}");
        // Return a standard error response
        return Results.Problem("An error occurred while fetching posts.", statusCode: 500);
    }
});

app.MapPut("/api/posts/{id}", async (Post post, int id, IUpdatePostHandler updatePostHandler) =>
{
    var updatePost = new UpdatePost { PostId = id, UpdatedContent = post.Content };
    var updatedPost = await updatePostHandler.Handle(updatePost);
    return Results.Ok(updatedPost);
});

app.MapDelete("/api/posts/{id}", (int id, IDeletePostHandler deletePostHandler) =>
{
    deletePostHandler.Handle(new DeletePost { PostId = id });
    return Results.NoContent();
});

// ... your other app.MapGet/MapPost calls ...

// --- START DIAGNOSTIC: Log Endpoints ---
// Do this only once before app.Run()
if (app! is IEndpointRouteBuilder routeBuilder)
{
    Console.WriteLine("\n--- Registered Endpoints ---");
    var dataSources = routeBuilder.DataSources;
    foreach (var dataSource in dataSources)
    {
        foreach (var endpoint in dataSource.Endpoints)
        {
            if (endpoint is RouteEndpoint routeEndpoint)
            {
                Console.WriteLine($"- {routeEndpoint.RoutePattern.RawText} ({string.Join(", ", routeEndpoint.Metadata.OfType<HttpMethodMetadata>().SelectMany(m => m.HttpMethods))})");
                // Optionally log more metadata if needed
            }
            else
            {
                Console.WriteLine($"- (Non-Route Endpoint: {endpoint.DisplayName})");
            }
        }
    }
    Console.WriteLine("--- End Registered Endpoints ---\n");
}
// --- END DIAGNOSTIC: Log Endpoints ---


await app.RunAsync();

// We are using this for the Intergration Tests
public partial class Program { }