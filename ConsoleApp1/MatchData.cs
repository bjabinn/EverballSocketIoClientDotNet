namespace EverballDotNet
{
    public class MatchData
    {
        public Playground_Info playground_info { get; set; }
        public int role { get; set; }
    }

    public class Playground_Info
    {
        public Field_Corners field_corners { get; set; }
        public Right_Goal_Posts right_goal_posts { get; set; }
        public Left_Goal_Posts left_goal_posts { get; set; }
        public float cap_radius { get; set; }
        public float ball_radius { get; set; }
    }

    public class Field_Corners
    {
        public float bottom_left_x { get; set; }
        public float bottom_left_y { get; set; }
        public float top_left_x { get; set; }
        public float top_left_y { get; set; }
        public float bottom_right_x { get; set; }
        public float bottom_right_y { get; set; }
        public float top_right_x { get; set; }
        public float top_right_y { get; set; }
    }

    public class Right_Goal_Posts
    {
        public float bottom_x { get; set; }
        public float bottom_y { get; set; }
        public float top_x { get; set; }
        public float top_y { get; set; }
    }

    public class Left_Goal_Posts
    {
        public float bottom_x { get; set; }
        public float bottom_y { get; set; }
        public float top_x { get; set; }
        public float top_y { get; set; }
    }

}
