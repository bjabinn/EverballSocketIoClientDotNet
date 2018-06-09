namespace EverballDotNet
{
    public class ServerState
    {
        public Team[] Team_1 { get; set; }
        public Team[] Team_2 { get; set; }
        public Ball Ball { get; set; }
        public string Match_event { get; set; }
    }

    public class Ball
    {
        public float x { get; set; }
        public float y { get; set; }
        public float vel_x { get; set; }
        public float vel_y { get; set; }
        public float cooldown { get; set; }
        public int cap_num { get; set; }
        public int anim_state { get; set; }
    }

    public class Team
    {
        public float x { get; set; }
        public float y { get; set; }
        public float vel_x { get; set; }
        public float vel_y { get; set; }
        public float cooldown { get; set; }
        public int cap_num { get; set; }
        public int anim_state { get; set; }
    }
}
