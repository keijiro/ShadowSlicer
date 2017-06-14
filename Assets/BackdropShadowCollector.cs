using UnityEngine;
using UnityEngine.Rendering;

public class BackdropShadowCollector : MonoBehaviour
{
    [SerializeField] BackdropRenderer _renderer;

    [SerializeField, HideInInspector] Shader _shader;

    CommandBuffer _command;
    Material _material;

    void OnEnable()
    {
        var light = GetComponent<Light>();

        if (_material == null)
        {
            _material = new Material(_shader);
        }

        if (_command == null)
        {
            _command = new CommandBuffer();
            _command.name = "Backdrop Shadow Collector";

            _command.SetGlobalTexture(
                "_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive
            );

            _command.SetGlobalColor("_LightColor0", light.color);

            _command.Blit(
                BuiltinRenderTextureType.CurrentActive,
                _renderer.targetTexture, _material, (int)light.type
            );
        }

        light.AddCommandBuffer(LightEvent.AfterShadowMap, _command);
    }

    void OnDisable()
    {
        var light = GetComponent<Light>();
        light.RemoveCommandBuffer(LightEvent.AfterShadowMap, _command);
    }

    void OnDestroy()
    {
        Destroy(_material);
        _command.Dispose();
    }
}
