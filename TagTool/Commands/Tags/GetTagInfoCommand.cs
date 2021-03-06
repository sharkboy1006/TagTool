﻿using System;
using System.Collections.Generic;
using TagTool.Cache;

namespace TagTool.Commands.Tags
{
    class GetTagInfoCommand : Command
    {
        private HaloOnlineCacheContext CacheContext { get; }

        public GetTagInfoCommand(HaloOnlineCacheContext cacheContext)
            : base(true,

            "GetTagInfo",
            "Displays detailed information about a tag.",

            "GetTagInfo <tag>",

            "Displays detailed information about a tag.")
        {
            CacheContext = cacheContext;
        }

        public override object Execute(List<string> args)
        {
            if (args.Count != 1)
                return false;

            if (!CacheContext.TryGetTag(args[0], out var tag))
                return false;

            Console.WriteLine("Information for tag {0:X8}:", tag.Index);
            Console.Write("- Groups:        {0}", tag.Group.Tag);
            if (tag.Group.ParentTag.Value != -1)
                Console.Write(" -> {0}", tag.Group.ParentTag);
            if (tag.Group.GrandparentTag.Value != -1)
                Console.Write(" -> {0}", tag.Group.GrandparentTag);
            Console.WriteLine();
            Console.WriteLine("- Header offset: 0x{0:X}", tag.HeaderOffset);
            Console.WriteLine("- Total size:    0x{0:X}", tag.TotalSize);
            Console.WriteLine("- Definition offset (relative to header offset): 0x{0:X}", tag.DefinitionOffset);
            Console.WriteLine();
            Console.WriteLine("Use \"dep list {0:X}\" to list this tag's dependencies.", tag.Index);
            return true;
        }
    }
}
