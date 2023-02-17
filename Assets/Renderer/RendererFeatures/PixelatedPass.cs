using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelatedPass : ScriptableRendererFeature
{
    [System.Serializable]
    public class CustomPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        [Range(10, 288)]
        public int screenHeight = 144;
        public Texture2D Palette;
        [Range(0, 10)]
        public float Fade = 1;
        [Range(0, 1)]

        public int EnablePalette = 1;
    }

    [SerializeField] private CustomPassSettings settings;
    private PixelizePass pixelizePass;

    class PixelizePass : ScriptableRenderPass
    {
        private CustomPassSettings settings;
        private RenderTargetIdentifier colorBuffer, pixelBuffer;
        private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");
        private Material material;
        private int pixelScreenHeight, pixelScreenWidth;
        private Texture2D Palette;
        private float Fade;
        private int EnablePalette;


        public PixelizePass(CustomPassSettings settings) //Constructor
        {
            this.settings = settings;
            this.renderPassEvent = renderPassEvent;
            if (material == null) material = CoreUtils.CreateEngineMaterial("Hidden/Pixelize");
            Palette = settings.Palette;
            Fade = settings.Fade;
            EnablePalette = settings.EnablePalette;
        }
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

            pixelScreenHeight = settings.screenHeight;
            pixelScreenWidth = (int)(pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f);

            material.SetVector("_BlockCount", new Vector2(pixelScreenWidth, pixelScreenHeight));
            material.SetVector("_BlockSize", new Vector2(1.0f / pixelScreenWidth, 1.0f / pixelScreenHeight));
            material.SetVector("_HalfBlockSize", new Vector2(0.5f / pixelScreenWidth, 0.5f / pixelScreenHeight));
            material.SetTexture("_Palette", Palette);
            material.SetFloat("_Fade", Fade);
            material.SetInteger("_EnablePalette", EnablePalette);

            descriptor.height = pixelScreenHeight;
            descriptor.width = pixelScreenWidth;

            cmd.GetTemporaryRT(pixelBufferID, descriptor, FilterMode.Point);
            pixelBuffer = new RenderTargetIdentifier(pixelBufferID);

        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("Pixelize Pass")))
            {
                Blit(cmd, colorBuffer, pixelBuffer, material);
                Blit(cmd, pixelBuffer, colorBuffer);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (cmd == null) throw new System.ArgumentNullException("cmd");
            cmd.ReleaseTemporaryRT(pixelBufferID);
        }
    }


    /// <inheritdoc/>
    public override void Create()
    {
        pixelizePass = new PixelizePass(settings);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        /*
        #if UNITY_EDITOR
                if (renderingData.cameraData.isSceneViewCamera) return;
        #endif
        */
                renderer.EnqueuePass(pixelizePass);
    }
}


