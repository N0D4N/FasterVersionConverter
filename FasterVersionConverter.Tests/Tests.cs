using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;
using static FasterVersionConverter.TestCases;

namespace FasterVersionConverter.Tests
{
	public class Tests
	{
		private static readonly JsonSerializerOptions _options = new JsonSerializerOptions()
		{
			Converters =
			{
				new ProposedJsonVersionConverter()
			}
		};

		private static readonly Dictionary<string, Version> _dictionary = new Dictionary<string, Version>()
		{
			[UnparsedTestCase1] = Version.Parse(UnparsedTestCase1),
			[UnparsedTestCase2] = Version.Parse(UnparsedTestCase2),
			[UnparsedTestCase3] = Version.Parse(UnparsedTestCase3),
			[UnparsedTestCase4] = Version.Parse(UnparsedTestCase4),
			[UnparsedTestCase5] = Version.Parse(UnparsedTestCase5),
			[UnparsedTestCase6] = Version.Parse(UnparsedTestCase6)
		};

		[Fact]
		public void JsonTests()
		{
			foreach ((var _, var value) in _dictionary)
			{
				var vw = new VersionWrapper
				{
					Version = value
				};
				byte[] currentUtf8Json = JsonSerializer.SerializeToUtf8Bytes(vw);
				byte[] proposedUtf8Json = JsonSerializer.SerializeToUtf8Bytes(vw, _options);
				Assert.Equal(currentUtf8Json, proposedUtf8Json);
				string currentStringJson = JsonSerializer.Serialize(vw);
				string proposedStringJson = JsonSerializer.Serialize(vw, _options);
				Assert.Equal(currentStringJson, proposedStringJson);

				var current = JsonSerializer.Deserialize<VersionWrapper>(currentUtf8Json);
				var proposed = JsonSerializer.Deserialize<VersionWrapper>(currentUtf8Json, _options);
				Assert.Equal(current.Version, proposed.Version);
				Assert.Equal(proposed.Version, value);
			}
		}
	}
}