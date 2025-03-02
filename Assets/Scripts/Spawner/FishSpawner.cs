using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private List<FishBehavior> fishList;
    
    public void SpawnFish(Transform hook)
    {
        FishBehavior currentFish = Instantiate(GetRandomFish());
        
        currentFish.Catch(hook);
    }
    
    private FishBehavior GetRandomFish()
    {
        return fishList[Random.Range(0, fishList.Count)];
    }
}