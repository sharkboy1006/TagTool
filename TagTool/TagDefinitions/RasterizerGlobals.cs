using TagTool.Cache;
using TagTool.Serialization;
using System.Collections.Generic;

namespace TagTool.TagDefinitions
{
    [TagStructure(Name = "rasterizer_globals", Size = 0xA4, Tag = "rasg", MaxVersion = CacheVersion.Halo3Retail)]
    [TagStructure(Name = "rasterizer_globals", Size = 0xAC, Tag = "rasg", MinVersion = CacheVersion.Halo3ODST, MaxVersion = CacheVersion.Halo3ODST)]
    [TagStructure(Name = "rasterizer_globals", Size = 0xBC, Tag = "rasg", MinVersion = CacheVersion.HaloOnline106708)]
    public class RasterizerGlobals
    {
        public List<DefaultBitmap> DefaultBitmaps;
        public List<DefaultRasterizerBitmap> DefaultRasterizerBitmaps;
        public CachedTagInstance VertexShaderSimple;
        public CachedTagInstance PixelShaderSimple;
        public List<DefaultShader> DefaultShaders;
        public uint Unknown;
        public uint Unknown2;
        public uint Unknown3;
        public int Unknown4;
        public int Unknown5;
        public CachedTagInstance ActiveCamoDistortion;
        public CachedTagInstance DefaultPerformanceTemplate;
        public CachedTagInstance DefaultShieldImpact;

        [TagField(MinVersion = CacheVersion.Halo3ODST)]
        public CachedTagInstance DefaultVisionMode;

        [TagField(MinVersion = CacheVersion.Halo3Retail ,MaxVersion = CacheVersion.Halo3Retail)]
        public int Unknown6;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public int Unknown6HO; //6 

        public float Unknown7;
        public float Unknown8;
        public float Unknown9;
        public float Unknown10;

        public float Unknown11;
        public float Unknown12;

        [TagField(Padding = true, Length = 8, MinVersion = CacheVersion.HaloOnline106708)]
        public byte[] Unused;

        [TagStructure(Size = 0x14)]
        public class DefaultBitmap
        {
            public int Unknown;
            public CachedTagInstance Bitmap;
        }

        [TagStructure(Size = 0x10)]
        public class DefaultRasterizerBitmap
        {
            public CachedTagInstance Bitmap;
        }

        [TagStructure(Size = 0x20)]
        public class DefaultShader
        {
            public CachedTagInstance VertexShader;
            public CachedTagInstance PixelShader;
        }
    }
}