using UnityEngine;

public static class ScoreContainer {
    public static int score;

    //State 0 = Main menu -> Scoreboard
    //State 1 = Victory
    //State 2 = Game Over
    public static int state = 0;
}
