﻿using TagTool.Cache;
using TagTool.Tags.Definitions;
using TagTool.Commands.Geometry;

namespace TagTool.Commands.RenderModels
{
    static class ParticleModelContextFactory
    {
        public static CommandContext Create(CommandContext parent, HaloOnlineCacheContext info, CachedTagInstance tag, ParticleModel particleModel)
        {
            var groupName = info.GetString(tag.Group.Name);

            var context = new CommandContext(parent,
                string.Format("{0:X8}.{1}", tag.Index, groupName));

            Populate(context, info, tag, particleModel);

            return context;
        }

        public static void Populate(CommandContext context, HaloOnlineCacheContext cacheContext, CachedTagInstance tag, ParticleModel particleModel)
        {
            context.AddCommand(new DumpRenderGeometryCommand(cacheContext, particleModel.Geometry, "particle"));
        }
    }
}
