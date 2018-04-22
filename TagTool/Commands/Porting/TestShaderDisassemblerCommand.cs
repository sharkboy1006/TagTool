﻿using System;
using System.Collections.Generic;
using System.IO;
using TagTool.Cache;
using TagTool.Commands;
using TagTool.Common;
using TagTool.Serialization;
using TagTool.ShaderDisassembler;
using TagTool.Tags;
using TagTool.Tags.Definitions;

namespace TagTool.Commands.Porting
{
	class TestShaderDisassemblerCommand : Command
	{
		private CacheFile BlamCache { get; }
		private GameCacheContext CacheContext { get; }

		public TestShaderDisassemblerCommand(GameCacheContext cacheContext, CacheFile blamCache) : base(
			CommandFlags.None,

			"DisassembleShader",
			"Test command for xbox360 shader disassembly.",

			"DisassembleShader <shaderIndex> <tagname>",

			"Test command for xbox360 shader disassembly.")
		{
			CacheContext = cacheContext;
			BlamCache = blamCache;
		}

		public override object Execute(List<string> args)
		{
			if (args.Count != 2)
				return false;

			int.TryParse(args[0], out int shaderIndex);
			var tagName = args[1];

			PixelShader pixl = new PixelShader();
			foreach (var tag in BlamCache.IndexItems)
			{
				if (tag.ClassCode != "pixl" || tag.Filename != tagName)
					continue;

				var blamContext = new CacheSerializationContext(BlamCache, tag);
				pixl = BlamCache.Deserializer.Deserialize<PixelShader>(blamContext);
				break;
			}

			if (pixl.Equals(new PixelShader()))
			{
				Console.WriteLine($"Unable to locate tag: {tagName}\n " +
					$"Please check your spelling and verify the tag exists.");
				return true;
			}

			var shaderData = pixl.Shaders[shaderIndex].XboxShaderReference.ShaderData;

			var instrs = Disassembler.Disassemble(shaderData);

			foreach (var instr in instrs)
			{
				if (instr.instructionType == InstructionType.CF)
					Console.WriteLine(instr.cfInstr.opcode);
				else if (instr.instructionType == InstructionType.ALU)
				{
					Console.WriteLine(instr.aluInstr.vector_opc);
					Console.WriteLine("+ " + instr.aluInstr.scalar_opc);
				}
				else if (instr.instructionType == InstructionType.FETCH)
					Console.WriteLine(instr.fetchInstr.opcode);
				else if (instr.instructionType == InstructionType.UNKNOWN)
					Console.WriteLine("<- UnKnown Instruction ->");
			}

			File.WriteAllBytes(tagName.Replace('\\', '_'), shaderData);

			return true;
		}
	}
}