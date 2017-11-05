using UnityEngine;
using System.Collections;

public class Work_OptionalSneakyPlayerSetup : BaseWorker {

    protected override void OnStart()
    {
        FighterName chosenOne = LevelManager.OptionalPlayer;

        SneakyPlayer trump = SneakyPlayerManager.instance.GetSneakyPlayerByName(FighterName.Trump);
        SneakyPlayer hillary = SneakyPlayerManager.instance.GetSneakyPlayerByName(FighterName.Hillary);

        if (chosenOne == FighterName.Trump)
        {
            trump.fighterRole = FighterRole.Leader;
            hillary.fighterRole = FighterRole.NotActive;

            hillary.gameObject.SetActive(false);
        }
        else
        {
            hillary.fighterRole = FighterRole.Leader;
            trump.fighterRole = FighterRole.NotActive;

            trump.gameObject.SetActive(false);
        }

        WorkFinished();
    }
}
