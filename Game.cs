class Move {
    public Int32 start_fort_id {get; set;}
    public Int32 end_fort_id {get; set;}
    public Int32 faction_id {get; set;}
};
class NetMove : Move {
    public Int32 game_id {get; set;}
};
class Game {
    static Int32 running_number = 0;
    public Int32 game_id {get; set;}
    public Int32[] state {get; set;}
    public Int32 host_id {get; set;}
    public List<Int32> guests {get; set;}
    public List<Move> queued_moves;

    public Game(Int32 host_id)
    {
        state = new Int32[0];
        guests = new List<Int32>();
        this.host_id = host_id;
        this.game_id = running_number++;
        this.queued_moves = new List<Move>();
    }
    public Int32 max_guests()
    {
        return 3;
    }
    public bool has_room()
    {
        return this.guests.Count() < this.max_guests();
    }
    public void unregister_guest(Int32 session_id)
    {
        //todo
    }
    public Int32[] add_guest(Int32 guest_session_id)
    {
        if(this.has_room())
        {
            guests.Add(guest_session_id);
            var arr = new Int32[1];
            arr[0] = guests.Count();
            return arr;
        }
        return new Int32[0];
    }
    public void register_move(Move move)
    {
        this.queued_moves.Add(move);
    }
};