//https://github.com/whateep/unity-simple-URP-pixelation/blob/main/Shaders/Pixelize.shader
Shader "Hidden/Pixelize"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Palette ("Palette", 2D) = "white" {}
		_Fade("Fade", Range( 0 , 5)) = 1
		_EnablePalette("EnablePalette", Integer) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        TEXTURE2D(_MainTex);
        float4 _MainTex_TexelSize;
        float4 _MainTex_ST;
        TEXTURE2D(_Palette);
		uniform float _Fade;
		int _EnablePalette;

        //SAMPLER(sampler_MainTex);
        //Texture2D _MainTex;
        //SamplerState sampler_MainTex;

        SamplerState SmpClampPoint;
        
        uniform float2 _BlockCount;
        uniform float2 _BlockSize;
        uniform float2 _HalfBlockSize;


        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
            //OUT.uv = IN.uv;
            return OUT;
        }

        ENDHLSL

        Pass
        {
            Name "Pixelation"

            HLSLPROGRAM
            half4 frag(Varyings IN) : SV_TARGET
            {
                float2 blockPos = floor(IN.uv * _BlockCount);
                float2 blockCenter = blockPos * _BlockSize + _HalfBlockSize;

                float4 lowResTex = SAMPLE_TEXTURE2D(_MainTex, SmpClampPoint, blockCenter);

            if(_EnablePalette == 0)
            {
                return lowResTex;
            }
            

                float colorMagnitude = (length(lowResTex.rgb)/3);
                float samplePointForPalette = lerp(colorMagnitude, 0, (1 - _Fade));
                
                return SAMPLE_TEXTURE2D(_Palette, SmpClampPoint,samplePointForPalette);

            }
            ENDHLSL
        }

        
    }
}