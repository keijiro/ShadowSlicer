using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ShadowCollector : MonoBehaviour
{
    [SerializeField] Camera _targetCamera;
    [SerializeField] RenderTexture _targetTexture;

    CommandBuffer _command1;
    CommandBuffer _command2;

    void OnEnable()
    {
        var width = _targetCamera.pixelWidth;
        var height = _targetCamera.pixelHeight;

        var rt = RenderTexture.GetTemporary(width, height, 0);

        if (_command1 == null)
        {
            _command1 = new CommandBuffer();
            _command1.name = "Shadow Collector";
            _command1.Blit(BuiltinRenderTextureType.CurrentActive, rt);
        }

        if (_command2 == null)
        {
            _command2 = new CommandBuffer();
            _command2.name = "Shadow Collector";
            _command2.Blit(BuiltinRenderTextureType.CurrentActive, _targetTexture);
            _command2.Blit(rt, BuiltinRenderTextureType.CameraTarget);
        }

        _targetCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _command1);
        _targetCamera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _command2);
    }

    void OnDisable()
    {
        if (_targetCamera != null)
        {
            _targetCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, _command1);
            _targetCamera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, _command2);
        }
    }

    void OnDestroy()
    {
        _command1.Dispose();
        _command2.Dispose();
    }
}
