using UnityEngine;

[CreateAssetMenu(fileName = "Fish Collection", menuName = "Fishing/Fish Collection")]
public class FishCollectionSO : ScriptableObject
{
    [SerializeField] private GameObject[] _fishPrefabs;

    public GameObject GetRandomFish()
    {
        if (_fishPrefabs == null || _fishPrefabs.Length == 0)
        {
            Debug.LogWarning("[FishCollection] Коллекция рыб пуста!");
            return null;
        }

        int index = Random.Range(0, _fishPrefabs.Length);
        return _fishPrefabs[index];
    }
}