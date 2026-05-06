using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BrainrotSpawner : MonoBehaviour
{
    public int MaxBrainrotInArea = 8;
    public float randomSpawnTime = 50f;
    public int MinBrainrotInArea = 4;
    public float MinBrainrotDistance = 10f;
    public Transform BrainrotParent;
    public Range AreaRangeX;
    public Range AreaRangeZ;

    public bool SpawnWithRarity = false;
    [ShowIf("SpawnWithRarity")]
    public Rarity Rarity;

    [ReadOnly]
    public List<BrainrotManager> BrainrotInArea = new();

    private void Awake()
    {
        BrainrotManager.OnBrainrotCollected += OnBrainrotCollected;
    }

    private void Start()
    {
        SpawnRandomNumberBrainrot();
    }

    private void SpawnRandomNumberBrainrot()
    {
        int random = Random.Range(MinBrainrotInArea, MaxBrainrotInArea + 1);
        SpawnBrainrot(random);
    }

    private void SpawnBrainrot(int random)
    {
        for (int i = 0; i < random; i++)
        {
            BrainrotConfig brainrotConfig = Services.BrainrotPoolService.GetRandomBrainrotConfig(SpawnWithRarity, Rarity);
            BrainrotManager brainrot = Services.BrainrotPoolService.GetBrainrot();
            Vector3 randomPosition = GetRandomPositionInArea();
            brainrot.transform.SetParent(BrainrotParent);
            randomPosition.y = -7f;
            brainrot.transform.localPosition = randomPosition;
            brainrot.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            brainrot.transform.localScale = Vector3.one;
            brainrot.InitializeFromSpawner(brainrotConfig, GetRandomModifier());
        }
    }

    private Modifier GetRandomModifier()
    {
        return Modifier.None;
    }

    private Vector3 GetRandomPositionInArea()
    {
        Vector3 randomPosition = Vector3.zero;
        int maxCount = 1000;
        int count = 0;
        while (true && count < maxCount)
        {
            bool skipLoop = false;
            count++;

            randomPosition = new Vector3(Random.Range((float)AreaRangeX.Min, (float)AreaRangeX.Max), 0, Random.Range((float)AreaRangeZ.Min, (float)AreaRangeZ.Max));
            if (count >= maxCount)
            {
                Debug.LogError("Max count reached, returning zero vector");
                return randomPosition;
            }

            foreach (var brainrot in BrainrotInArea)
            {
                if (Vector3.Distance(brainrot.transform.position, randomPosition) < MinBrainrotDistance)
                {
                    skipLoop = true;
                    break;
                }
            }

            if (skipLoop) continue;

            break;
        }
        return randomPosition;
    }

    private void OnBrainrotCollected(BrainrotManager brainrot)
    {
        BrainrotInArea.Remove(brainrot);
    }

}
