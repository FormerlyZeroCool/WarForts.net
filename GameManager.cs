

class GameManager {
    SessionManager sessions;
    Dictionary<Int32, Game> games {get;}
    Dictionary<Int32, Int32> session_to_game_id;
    List<Game> open_games;

    public GameManager(SessionManager sessions)
    {
        this.sessions = sessions;
        games = new Dictionary<Int32, Game>();
        session_to_game_id = new Dictionary<int, int>();
        this.open_games = new List<Game>();
    }
    public Game register_game(Int32 session_id) //returns registered game id
    {
        var game = new Game(session_id);
        this.session_to_game_id[session_id] = game.game_id;
        this.games[game.game_id] = game;
        return game;
    }
    public void unregister_session(Int32 session_id)
    {
        this.sessions.unregister_session(session_id);
        if(this.session_to_game_id.ContainsKey(session_id))
        {
            Int32 game_id = this.session_to_game_id[session_id];
            Game game = this.get_game(game_id);
            this.remove_game(game_id);
        }
    }
    public Game? attempt_join_game(Int32 session_id, Int32 game_id)
    {
        if(open_games.Count() > 0)
        {
            var game = open_games[0];
            if(game.host_id != session_id)
            {
                game.add_guest(session_id);
                this.session_to_game_id[session_id] = game.game_id;
                this.remove_game(game_id);
                if(!game.has_room())
                {
                    this.open_games.Remove(game);
                }
                return game;
            }
            else
            {
                return game;
            }
        }
        else if(this.games.ContainsKey(game_id))
        {
            var game = this.get_game(game_id);
            if(!this.open_games.Contains(game))
            {
                open_games.Add(game);
            }
        }
        return null;
    }
    public Game register_game(Int32 session_id, Int32 old_game_id) //returns registered game id
    {
        this.remove_game(old_game_id);
        var game = new Game(session_id);
        this.session_to_game_id[session_id] = game.game_id;
        this.games[game.game_id] = game;
        return game;
    }
    public void remove_game(Int32 game_id)
    {
        Console.WriteLine("Unregistering session: ");
        Console.WriteLine(game_id);
        Game game = this.get_game(game_id);
        this.open_games.Remove(this.get_game(game_id));
        this.games.Remove(game_id);
        for(Int32 i = 0; i < game.guests.Count(); i++)
        {
            Int32 session_id = game.guests[i];
            this.sessions.unregister_session(session_id);
        }
    }
    public Game get_game(Int32 id)
    {
        if(this.games.ContainsKey(id))
        {
            return this.games[id];
        }
        throw new ArgumentException(id.ToString() + " is not in dictionary games");
    }
};