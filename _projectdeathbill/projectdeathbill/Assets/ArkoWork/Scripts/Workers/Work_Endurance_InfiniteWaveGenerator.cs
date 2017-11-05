using UnityEngine;
using System.Collections;

public class Work_Endurance_InfiniteWaveGenerator : InfiniteWorker {

    int infinityWaveCount = 0;
    float targetDifficulty = 100;

	protected override int GetNextWorkerIndex ()
	{
        infinityWaveCount++;
        targetDifficulty += 9;
		

        int i=  base.GetNextWorkerIndex ();
		ManipulateWorker (workerList [i]);
        return i;
	}

	private void ManipulateWorker(BaseWorker bw)
	{
        
        bool M = false;
        float morFloat = Random.Range(-200.0f, 100.0f + infinityWaveCount*20.0f);
        if (morFloat> 0)
            M = true;

        int Nt = 0;
        float lerpVal = Mathf.Clamp(infinityWaveCount/10.0f, 0,1);
        Nt = Mathf.CeilToInt(Handy.Deviate(   Mathf.Lerp(10.0f,13.0f,lerpVal),    Mathf.Lerp(0.2f,0.35f,lerpVal)  ));
        float MaxGChance = Mathf.Lerp(0.85f, 0.45f, lerpVal);

        float GChance = Random.Range(0, MaxGChance);
        int Ng = Mathf.RoundToInt(Nt *GChance);

        int Nn = Nt - Ng;

        float A = 0.64f;
        float B = 4.0f;
        float C = 0.6f;
        float Mc = 20.0f;

        float D = (targetDifficulty- (M?Mc:0) - (Ng*B))/(A*(Nn + (C*Ng)));


		AIGeneratorModule agm = (AIGeneratorModule)bw;

        agm.dropCount = Nt;
        agm.nadeChance = GChance;
        agm.hasMortar = M;
        agm.useLocalInitData = true;
        agm.localAIInitData.accuracy = 0.90f;
        agm.localAIInitData.damage = D;
        agm.localAIInitData.intelligence = 0.5f;

        Debug.Log ( 
            "Difficulty: "+targetDifficulty.ToString() +
            "N: "+agm.dropCount.ToString() +
            "M: "+agm.hasMortar.ToString() +
            "G: "+agm.nadeChance.ToString() +
            "D: "+agm.localAIInitData.damage.ToString() 
        
        );
	}
}
