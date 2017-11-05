using UnityEngine;
using System.Collections;

public class Work_OptionalPlayerSetup : BaseWorker {

    protected override void OnStart()
    {
        FighterName chosenOne = LevelManager.OptionalPlayer;

        ThirdPersonController trump = PlayerInputController.instance.GetPlayerByID(FighterName.Trump);
        ThirdPersonController hillary = PlayerInputController.instance.GetPlayerByID(FighterName.Hillary);

        if (chosenOne == FighterName.Trump)
        {
            trump.fighterRole = FighterRole.Heavy;
            hillary.fighterRole = FighterRole.NotActive;

            hillary.gameObject.SetActive(false);
        }
        else
        {
            hillary.fighterRole = FighterRole.Heavy;
            trump.fighterRole = FighterRole.NotActive;

            trump.gameObject.SetActive(false);
        }

        WorkFinished();
    }
}
