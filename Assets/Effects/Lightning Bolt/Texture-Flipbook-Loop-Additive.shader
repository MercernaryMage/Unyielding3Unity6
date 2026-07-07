// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Texture/FlipBook-Additive"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ColumnsX("Columns", int) = 1
        _RowsY("Rows", int) = 1
        _AnimationSpeed("Frames Per Seconds", float) = 10
    }

    SubShader
    {
        Tags
        {
            "Queue"             = "Transparent"
            "IgnoreProjector"   = "True"
            "RenderType"        = "Opaque"
            "PreviewType"       = "Plane"
            "CanUseSpriteAtlas" = "False" // Flipbook should not be packed in an atlas
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One One

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVertFlipBook
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #include "UnitySprites.cginc"

            uint _ColumnsX;
            uint _RowsY;
            float _AnimationSpeed;

            // Modified from SpriteVert
            v2f SpriteVertFlipBook(appdata_t IN)
            {
                v2f OUT;

                OUT.vertex = UnityObjectToClipPos(IN.vertex); 
                OUT.color = IN.color * _Color;

                // FlipBook
                float2 size = float2(1.0f / _ColumnsX, 1.0f / _RowsY);
                uint totalFrames = _ColumnsX * _RowsY;                      // get single sprite size
                uint index = _Time.y * _AnimationSpeed;                     // use timer to increment index
                uint indexX = index % _ColumnsX;                            // wrap x index
                uint indexY = floor((index % totalFrames) / _ColumnsX);     // wrap y index
                float2 offset = float2(size.x * indexX, -size.y * indexY);  // get offsets to our sprite index
                float2 newUV = IN.texcoord * size;                          // get single sprite UV
                newUV.y = newUV.y + size.y * (_RowsY - 1);                  // flip Y (to start 0 from top)
                OUT.texcoord = newUV + offset;

                return OUT;
            }
        ENDCG
        }
    }
}
