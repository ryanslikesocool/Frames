namespace ifelse.Frames
{
    public enum StackDirection
    {
        Horizontal,  //Starts at left and goes right
        Vertical     //Starts at top and goes down
    }

    public enum StackDistribution
    {
        Start,          //Snaps stack at beginning with spacing between content
        Center,         //Centers stack with spacing between content
        End,            //Snaps stack at end with spacing between content
        SpaceBetween,   //Even spaces only between content, not bounds
        SpaceAround,    //Cramped content with even spaces around bounds
        SpaceEvenly,    //Even spacing between content and bounds
    }

    //Kind of like paragraph alignment in a text editor
    public enum StackAlignment
    {
        Left,   //Bottom
        Center, //Center
        Right   //Top
    }
}