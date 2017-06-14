using UnityEngine;
using UnityEngine.Rendering;

public class ShadowSlicer : MonoBehaviour
{
    [SerializeField] Camera _baseCamera;
    [SerializeField] RenderTexture _targetTexture;

    [SerializeField, HideInInspector] Mesh _quadMesh;
    [SerializeField, HideInInspector] Shader _slicerShader;

    Material _slicerMaterial;
    RenderTexture _backupBuffer;
    CommandBuffer _backupCommand;
    CommandBuffer _restoreCommand;

    void OnEnable()
    {
        var width = _baseCamera.pixelWidth;
        var height = _baseCamera.pixelHeight;

        _backupBuffer = RenderTexture.GetTemporary(width, height, 0);

        if (_backupCommand == null)
        {
            _backupCommand = new CommandBuffer();
            _backupCommand.name = "Shadow Slicer (Backup)";
            _backupCommand.Blit(BuiltinRenderTextureType.CurrentActive, _backupBuffer);
        }

        if (_restoreCommand == null)
        {
            _restoreCommand = new CommandBuffer();
            _restoreCommand.name = "Shadow Slicer (Restore)";
            _restoreCommand.Blit(BuiltinRenderTextureType.CurrentActive, _targetTexture);
            _restoreCommand.Blit(_backupBuffer, BuiltinRenderTextureType.CameraTarget);
        }

        _baseCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _backupCommand);
        _baseCamera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _restoreCommand);
    }

    void OnDisable()
    {
        if (_baseCamera != null)
        {
            _baseCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, _backupCommand);
            _baseCamera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, _restoreCommand);
        }
    }

    void OnDestroy()
    {
        if (_slicerMaterial != null) Destroy(_slicerMaterial);
        if (_backupBuffer != null) RenderTexture.ReleaseTemporary(_backupBuffer);
        if (_backupCommand != null) _backupCommand.Dispose();
        if (_restoreCommand != null) _restoreCommand.Dispose();
    }

    void Update()
    {
        if (_slicerMaterial == null)
            _slicerMaterial = new Material(_slicerShader);

        Graphics.DrawMesh(
            _quadMesh, transform.localToWorldMatrix, _slicerMaterial,
            gameObject.layer, _baseCamera
        );
    }
}
