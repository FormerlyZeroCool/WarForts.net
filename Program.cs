using Microsoft.Extensions.FileProviders;
using System.Collections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
var session_manager = new SessionManager();
var games_manager = new GameManager(session_manager);
app.UseRouting();
app.UseStaticFiles();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    app.MapGet("/register_session", (HttpContext ctx) => {
        var session_id = session_manager.register_session(ctx.Connection.RemoteIpAddress);
        var game = games_manager.register_game(session_id);
        return game;
    });
    app.MapPost("/unregister_session", (Int32[] data, HttpContext ctx) => {
        Int32 session_id = data[0];
        games_manager.unregister_session(session_id);
        return data;
    });
    app.MapPost("/new_game", (Int32[] data) => {
        return games_manager.register_game(data[0], data[1]);
    });
    app.MapPost("/save_game_state", (Game game) => {
        var old_game = games_manager.get_game(game.game_id);
        old_game.state = game.state;
        return game;
    });
    app.MapPost("/request_join_random_game", (Int32[] data) => {
        var session_id = data[0];
        var game_id = data[1];
        var maybe_game = games_manager.attempt_join_game(session_id, game_id);
        var arr = new Int32[2];
        arr[0] = maybe_game != null ? maybe_game.game_id : -1;
        
        arr[1] = maybe_game != null ? maybe_game.guests.Count() + 1 : 1;

        if(maybe_game != null && maybe_game.host_id == session_id)
        {
            arr[1] = 1;
            arr[0] = game_id;
        }

        return arr;
    });
    app.MapPost("/register_guest", (Int32[] data) => {
        var game = games_manager.get_game(data[0]);
        if(data[1] != -1)
            games_manager.remove_game(data[1]);
        game.add_guest(data[2]);
        var arr = new Int32[1];
        arr[0] = game.guests.Count() + 1;
        return arr;
    });
    app.MapPost("/register_moves", (NetMove[] moves) => {
        var game = games_manager.get_game(moves[0].game_id);
        for(var i = 0; i < moves.Length; i++)
        {
            game.register_move(moves[i]);
        }
        var arr = new bool[1];
        arr[0] = true;
        return arr;
    });
    app.MapPost("/get_game_state", (Int32[] game_id) => {
        return games_manager.get_game(game_id[0]);
    });
    app.MapPost("/get_moves", (Int32[] game_id) => {
        var game = games_manager.get_game(game_id[0]);
        var queued_moves = game.queued_moves;
        if(queued_moves.Count() != 0)
            game.queued_moves = new List<Move>();
        return queued_moves;
    });
    app.MapPost("/get_count_guests", (Int32[] game_id) => {
        var game = games_manager.get_game(game_id[0]);
        var arr = new Int32[1];
        arr[0] = game.guests.Count();
        return arr;
    });
});

app.MapRazorPages();

app.Run();
