﻿using System;
using System.Collections.Generic;
using TagTool.Cache;
using TagTool.Tags.Definitions;
using System.IO;
using System.Linq;
using TagTool.Common;

namespace TagTool.Commands.ModelAnimationGraphs
{
    class ResourceDataCommand : Command
    {
        private HaloOnlineCacheContext CacheContext { get; }
        private CachedTagInstance Tag { get; }
        private ModelAnimationGraph Definition { get; }

        public ResourceDataCommand(HaloOnlineCacheContext cacheContext, CachedTagInstance tag, ModelAnimationGraph definition)
            : base(false,

                  "ResourceData",
                  "ResourceData Extract/Import [Cache Type] <Resource group Index (decimal)> <Filename>",

                  "ResourceData Extract 0 warthog.raw\n" +
                  "ResourceData Import resources_b 0 ones_0.raw",

                  "ResourceData Extract/Import an animation raw resource and update the tag resource index.")
        {
            CacheContext = cacheContext;
            Tag = tag;
            Definition = definition;
        }

        public override object Execute(List<string> args)
        {
            if (args.Count < 1)
                return false;

            var command = args[0];
            args.RemoveAt(0);

            switch (command.ToLower())
            {
                case "extract":
                    if (args.Count != 2)
                        return false;

                    return ExtractResource(Definition, CacheContext, args);

                case "import":
                    if (args.Count != 3)
                        return false;

                    return ImportResource(Definition, CacheContext, args);

                default:
                    return false;
            }
        }

        private static bool ExtractResource(ModelAnimationGraph Definition, HaloOnlineCacheContext CacheContext, IReadOnlyList<string> args)
        {
            int groupIndex = -1;
            if (!Int32.TryParse(args[0], out groupIndex))
                return false;

            var filePath = args[1]; // warthog.raw

            if (groupIndex == -1)
            {
                Console.WriteLine("Resource is null.");
                return false;
            }

            try
            {
                Definition.ResourceGroups[groupIndex].Resource.GetLocation(out var location);
                using (var stream = File.OpenRead(CacheContext.TagCacheFile.DirectoryName + "\\" + CacheContext.ResourceCacheNames[location]))
                {
                    var cache = new ResourceCache(stream);
                    using (var outStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
                    {
                        cache.Decompress(stream, Definition.ResourceGroups[groupIndex].Resource.Page.Index, 0x7FFFFFFF, outStream);
                        Console.WriteLine("Wrote 0x{0:X} bytes to {1}.", outStream.Position, filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to extract resource: {0}", ex.Message);
            }

            return true;
        }

        private static bool ImportResource(ModelAnimationGraph Definition, HaloOnlineCacheContext CacheContext, IReadOnlyList<string> args)
        {
            var locationName = args[0].Split(".".ToCharArray()).First().ToLower();

            int index = -1;
            if (!Int32.TryParse(args[1], out index))
            {
                Console.WriteLine($"ERROR: resource index could not be parsed.");
                return false;
            }

            var filePath = args[2];

            if (index > Definition.ResourceGroups.Count && index != -1)
            {
                Console.WriteLine($"ERROR: bitmap index higher than the bitmaps count or == -1.");
                return false;
            }

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var location = ResourceLocation.None;

                switch (locationName)
                {
                    case "textures":
                        location = ResourceLocation.Textures;
                        break;

                    case "textures_b":
                        location = ResourceLocation.TexturesB;
                        break;

                    case "resources":
                        location = ResourceLocation.Resources;
                        break;

                    case "resources_b":
                        location = ResourceLocation.ResourcesB;
                        break;

                    case "audio":
                        location = ResourceLocation.Audio;
                        break;

                    case "lightmaps":
                        location = ResourceLocation.Lightmaps;
                        break;

                    case "render_models":
                        location = ResourceLocation.RenderModels;
                        break;

                    default:
                        Console.WriteLine($"ERROR: invalid resource location: {locationName}");
                        break;
                }

                Definition.ResourceGroups[index].Resource.ChangeLocation(location);
                CacheContext.AddResource(Definition.ResourceGroups[index].Resource, stream);

                Console.WriteLine($"New resource info: " +
                    $"Index 0x{Definition.ResourceGroups[index].Resource.Page.Index:X4}, " +
                    $"Index {Definition.ResourceGroups[index].Resource.Page.Index:D8}, " +
                    $"Compressed size: 0x{Definition.ResourceGroups[index].Resource.Page.CompressedBlockSize:X8}, " +
                    $"Decompressed size: 0x{Definition.ResourceGroups[index].Resource.Page.UncompressedBlockSize:X8}, " +
                    $"");
            }

            return true;
        }
    }
}
