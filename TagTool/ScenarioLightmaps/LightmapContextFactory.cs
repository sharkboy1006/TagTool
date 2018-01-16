﻿using BlamCore.Cache;
using BlamCore.Commands;
using BlamCore.TagDefinitions;
using TagTool.Geometry;

namespace TagTool.ScenarioLightmaps
{
    static class LightmapContextFactory
    {
        public static CommandContext Create(CommandContext parent, GameCacheContext cacheContext, CachedTagInstance tag, ScenarioLightmapBspData Lbsp)
        {
            var groupName = cacheContext.GetString(tag.Group.Name);

            var context = new CommandContext(parent,
                string.Format("{0:X8}.{1}", tag.Index, groupName));

            Populate(context, cacheContext, tag, Lbsp);

            return context;
        }

        public static void Populate(CommandContext commandContext, GameCacheContext cacheContext, CachedTagInstance tag, ScenarioLightmapBspData Lbsp)
        {
            commandContext.AddCommand(new DumpRenderGeometryCommand(cacheContext, Lbsp.Geometry, "Lightmap"));
        }
    }
}