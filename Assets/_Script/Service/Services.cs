using LacelSDK;
using UnityEngine;

public static class Services 
{
   public static InputHandlerService InputService => LacelSystem.GetService<InputHandlerService>();
   public static InventoryService InventoryService => LacelSystem.GetService<InventoryService>();
   public static BrainrotPoolService BrainrotPoolService => LacelSystem.GetService<BrainrotPoolService>();
   public static AsyncSceneManager SceneManager { get; private set; }

   [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
   private static void InitializeServices()
   {
      // Create AsyncSceneManager if it doesn't exist
      if (SceneManager == null)
      {
         GameObject sceneManagerObj = new GameObject("AsyncSceneManager");
         SceneManager = sceneManagerObj.AddComponent<AsyncSceneManager>();
         Object.DontDestroyOnLoad(sceneManagerObj);
      }
   }
}
