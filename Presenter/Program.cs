using Application.Abstractions;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Application.Posts.Commands;
using Application.Posts.Queries;
using Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SocialDbcontext>(options =>
    options.UseSqlite(conn));
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddMediatR(typeof(CreatePost));

var app = builder.Build();

app.UseHttpsRedirection();

//Using method injection
app.MapGet("/api/post/{id}", async (IMediator mediator, int id) =>
{
    var getPost = new GetPostById { PostId = id };
    var post = mediator.Send(getPost);
    return Results.Ok(post); // Replace IActionResult with this.
}).WithName("GetPostById");

app.MapPost("/api/posts", async (IMediator mediator, Post post) => 
{
    var createPost = new CreatePost { PostContent = post.Comments };
    var cratedPost = await mediator.Send(createPost);
    return Results.CreatedAtRoute("GetPostById", new { createPost.Id }, createPost);
});

app.MapGet("/api/posts", async (IMediator mediator) => 
{
    var getCommand = new GetAllPosts();
    var posts = await mediator.Send(getCommand);
    return Results.Ok(posts);
});

app.MapPut("/api/posts/{id}", async (IMediator mediator, Post post, int id) => 
{
    var updatePost = new UpdatePost { PostId = id, UpdatedContent = post.Content };
    var updatedPost = await mediator.Send(updatePost);
    return Results.Ok(updatedPost);
});

app.MapDelete("/api/posts/{id}", async (IMediator mediator, int id) =>
{
    var deletePost = new DeletePost { PostId = id };
    await mediator.Send(deletePost);
    return Results.NoContent();
});

app.Run();

// We are using this for the Intergration Tests
public partial class Program { }