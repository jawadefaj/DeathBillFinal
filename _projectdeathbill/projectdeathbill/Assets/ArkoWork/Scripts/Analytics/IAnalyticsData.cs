

public interface IAnalyticsData  {

    //track when a level is being loaded
    void TrackLevelLoadEvent(int level, int Stage);

    //track when a level is being restarted with the previous time of play
    void TrackLevelRestartEvent(int level, int stage, long tryTime);

    //when he finally finished the game track the time he took to finish the play
    void TrackLevelFinishTimeEvent(int level, int stage, long time);

    //track the particular level he left from playing with the number of tries
    void TrackGameLeaveEvent(int level, int Stage, long no_of_tries);

    //what rating did he put
    void TrackGameRating(int rate);

    //the useres personal info
    void TrackUserSpecifiqData(string countryName, string deviceModel, string graphicsDeviceName, string graphicsDeviceVendor, string systemMemorySize);

    void TrackSceneEntryEvent(string sceneName);


}
