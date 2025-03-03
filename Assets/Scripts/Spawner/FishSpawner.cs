using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private List<FishBehaviour> fishList;
    
    public void SpawnFish(Transform hook)
    {
        FishBehaviour currentFish = Instantiate(GetRandomFish());
        
        currentFish.Catch(hook);
    }
    
    private FishBehaviour GetRandomFish()
    {
        return fishList[Random.Range(0, fishList.Count)];
    }
}