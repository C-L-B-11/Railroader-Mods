using System.Collections;
using System.Reflection;
using Network;
using Railloader;
using Serilog;
using TransformHandles;
using UnityEngine;

namespace MapEditor
{
  class Editor : Singleton<Editor>
  {
    private Serilog.ILogger logger = Log.ForContext<Editor>();
    private Settings Settings
    {
      get => SingletonPluginBase<EditorMod>.Shared?.Settings ?? new Settings();
    }

    public Editor()
    {
    }

    public void Start()
    {
      StartCoroutine(LoadFromMemoryAsync());
    }


    private IEnumerator LoadFromMemoryAsync()
    {
      var rth = Assembly.GetExecutingAssembly().GetManifestResourceStream("MapEditor.Resources.rth.runtime");
      var ms = new System.IO.MemoryStream();
      rth.CopyTo(ms);
      byte[] bytes = ms.ToArray();
      AssetBundleCreateRequest createRequest = AssetBundle.LoadFromMemoryAsync(bytes);
      yield return createRequest;
      var bundle = createRequest.assetBundle;
      var transformHandlePrefab = bundle.LoadAsset<GameObject>("Assets/RTH.Runtime/Prefabs/NativeTransformHandle.prefab");
      var ghostPrefab = bundle.LoadAsset<GameObject>("Assets/RTH.Runtime/Prefabs/Ghost.prefab");
      TransformHandleManager.Instance.transformHandlePrefab = transformHandlePrefab;
      TransformHandleManager.Instance.ghostPrefab = ghostPrefab;
      TransformHandleManager.Instance.mainCamera = CameraSelector.shared.strategyCamera.CameraContainer.GetComponent<Camera>();
    }

    public void OnEnable()
    {
      logger.Debug("Editor OnEnable()");
    }

    public void OnDisable()
    {
      logger.Debug("Editor OnDisable()");
    }

    public void Update()
    {
      // TransformHandleManager.Instance.mainCamera = Camera.main;
    }
  }
}
