using UnityEngine;
using Cysharp.Threading.Tasks;
using LacelSDK;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BrainrotPoolService", menuName = "Services/BrainrotPoolService")]
public class BrainrotPoolService : LacelService, IService
{
    [ReadOnly]
    public Transform PoolTransform;

    [SerializeField] private int NumberOfBrainrotToPreload = 100;
    [ReadOnly][SerializeField] private List<BrainrotManager> BrainrotPoolList = new();
    [SerializeField] private List<BrainrotConfig> BrainrotConfigs = new();
    public string PrefabAdress;
    [HideInInspector]
    public BrainrotManager BrainrotPrefab;
    public async UniTaskVoid InitAsync()
    {
        LoadPrefab();
        InstantiateBrainrot(NumberOfBrainrotToPreload);
    }

    private void LoadPrefab()
    {
        BrainrotPrefab = Addressables.LoadAssetAsync<BrainrotManager>(PrefabAdress).WaitForCompletion();
    }

    private void InstantiateBrainrot(int numberOfBrainrotToPreload)
    {
        for (int i = 0; i < numberOfBrainrotToPreload; i++)
        {
            var brainrot = Instantiate(BrainrotPrefab, PoolTransform);
            brainrot.transform.position = Vector3.zero;
            brainrot.transform.rotation = Quaternion.identity;
            brainrot.transform.localScale = Vector3.one;
            brainrot.gameObject.SetActive(false);
            BrainrotPoolList.Add(brainrot);
        }
    }

    public void InjectPoolTransform(Transform poolTransform)
    {
        PoolTransform = poolTransform;
    }

    public void ReturnToPool(BrainrotManager brainrot)
    {
        brainrot.transform.SetParent(PoolTransform);
        brainrot.transform.position = Vector3.zero;
        brainrot.gameObject.SetActive(false);
        BrainrotPoolList.Add(brainrot);
    }

    public BrainrotManager GetBrainrot()
    {
        if (BrainrotPoolList.Count == 0)
        {
            InstantiateBrainrot(1);
        }
        return BrainrotPoolList[0];
    }

    public BrainrotConfig GetRandomBrainrotConfig(bool spawnWithRarity, Rarity rarity = Rarity.None)
    {
        List<BrainrotConfig> filteredConfigs = BrainrotConfigs;
        if (spawnWithRarity)
        {
            filteredConfigs = filteredConfigs.FindAll(config => config.Rarity == rarity);
        }
        return filteredConfigs[Random.Range(0, filteredConfigs.Count)];
    }
}
