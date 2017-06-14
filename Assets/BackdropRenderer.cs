using UnityEngine;
using UnityEngine.Rendering;

public class BackdropRenderer : MonoBehaviour
{
    [SerializeField] RenderTexture _targetTexture;

    public RenderTexture targetTexture
    {
        get { return _targetTexture; }
        set { _targetTexture = value; }
    }

    CommandBuffer _command;

    void OnEnable()
    {
        var camera = GetComponent<Camera>();

        if (_command == null)
        {
            _command = new CommandBuffer();
            _command.name = "Backdrop Renderer";
            _command.SetRenderTarget(_targetTexture);
            _command.ClearRenderTarget(true, true, Color.black);
        }

        camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, _command);
    }

    void OnDisable()
    {
        var camera = GetComponent<Camera>();
        camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, _command);
    }

    void OnDestroy()
    {
        _command.Dispose();
    }
}
