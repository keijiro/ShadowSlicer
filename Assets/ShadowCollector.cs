using UnityEngine;
using UnityEngine.Rendering;

public class ShadowCollector : MonoBehaviour
{
    [SerializeField] Camera _targetCamera;
    [SerializeField] RenderTexture _targetTexture;

    [SerializeField, HideInInspector] Mesh _quadMesh;
    [SerializeField, HideInInspector] Shader _collectorShader;

    Material _collectorMaterial;
    RenderTexture _backupBuffer;
    CommandBuffer _backupCommand;
    CommandBuffer _restoreCommand;

    void OnEnable()
    {
        var width = _targetCamera.pixelWidth;
        var height = _targetCamera.pixelHeight;

        _backupBuffer = RenderTexture.GetTemporary(width, height, 0);

        if (_backupCommand == null)
        {
            _backupCommand = new CommandBuffer();
            _backupCommand.name = "Shadow Collector (Backup)";
            _backupCommand.Blit(BuiltinRenderTextureType.CurrentActive, _backupBuffer);
        }

        if (_restoreCommand == null)
        {
            _restoreCommand = new CommandBuffer();
            _restoreCommand.name = "Shadow Collector (Restore)";
            _restoreCommand.Blit(BuiltinRenderTextureType.CurrentActive, _targetTexture);
            _restoreCommand.Blit(_backupBuffer, BuiltinRenderTextureType.CameraTarget);
        }

        _targetCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _backupCommand);
        _targetCamera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _restoreCommand);
    }

    void OnDisable()
    {
        if (_targetCamera != null)
        {
            _targetCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, _backupCommand);
            _targetCamera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, _restoreCommand);
        }
    }

    void OnDestroy()
    {
        if (_collectorMaterial != null) Destroy(_collectorMaterial);
        if (_backupBuffer != null) RenderTexture.ReleaseTemporary(_backupBuffer);
        if (_backupCommand != null) _backupCommand.Dispose();
        if (_restoreCommand != null) _restoreCommand.Dispose();
    }

    void Update()
    {
        if (_collectorMaterial == null)
            _collectorMaterial = new Material(_collectorShader);

        Graphics.DrawMesh(
            _quadMesh, transform.localToWorldMatrix, _collectorMaterial,
            gameObject.layer, _targetCamera
        );
    }
}
