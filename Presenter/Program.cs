using Application.Abstractions;
using Application.Posts.CommandHandlers;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Application.Posts.Commands;
using Application.Posts.Queries;
using Domain.Entities;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SocialDbcontext>(options =>
    options.UseSqlite(conn));
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddTransient<ICreatePostHandler, CreatePostHandler>();
builder.Services.AddTransient<IDeletePostHandler, DeletePostHandler>();
builder.Services.AddTransient<IUpdatePostHandler, UpdatePostHandler>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

var app = builder.Build();

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

// app.MapGet("/api/post/{id}", async (int id, IGetPostByIdHandler getPostHandler) =>
// {
//     var post = await getPostHandler.Handle(new GetPostById { PostId = id });
//     return Results.Ok(post);
// }).WithName("GetPostById");

app.MapPost("/api/posts", async (Post post, ICreatePostHandler createPostHandler) =>
{
    var createPost = new CreatePost { PostContent = post.Comments };
    var createdPost = await createPostHandler.Handle(createPost);  // ✅ Await the async method
    return  Results.Created($"/api/posts/{createdPost.Id}", createdPost);//Results.Ok();
});


// app.MapGet("/api/posts", async (IGetAllPostsHandler getAllPostsHandler) =>
// {
//     try
//     {
//         var posts = await getAllPostsHandler.Handle(new GetAllPosts());
//         return Results.Ok(posts);
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine(ex);
//         return Results.Problem("An error occurred while fetching posts.");
//     }
// });

app.MapPut("/api/posts/{id}",  (Post post, int id, IUpdatePostHandler updatePostHandler) =>
{
    var updatePost = new UpdatePost { PostId = id, UpdatedContent = post.Content };
    var updatedPost =  updatePostHandler.Handle(updatePost);
    return Results.Ok(updatedPost);
});

app.MapDelete("/api/posts/{id}",  (int id, IDeletePostHandler deletePostHandler) =>
{
     deletePostHandler.Handle(new DeletePost { PostId = id });
    return Results.NoContent();
});


app.Run();

// We are using this for the Intergration Tests
public partial class Program { }