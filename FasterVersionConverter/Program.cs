using System;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;

namespace FasterVersionConverter
{
	public class Program
	{
		static void Main(string[] args) => BenchmarkRunner.Run<Benchmark>();
	}

	[MemoryDiagnoser]
	public class Benchmark
	{
		[Params(TestCases.UnparsedTestCase1, TestCases.UnparsedTestCase2, TestCases.UnparsedTestCase3, TestCases.UnparsedTestCase4,
			TestCases.UnparsedTestCase5, TestCases.UnparsedTestCase6)]
		public string unparsedVersion;

		private static readonly JsonSerializerOptions optionsWithProposedConverter = new JsonSerializerOptions()
		{
			Converters =
			{
				new ProposedJsonVersionConverter()
			}
		};

		private static readonly Consumer _consumer = new Consumer();

		private Version _version;

		private VersionWrapper _versionWrapper;

		private string _json;

		[GlobalSetup]
		public void Setup()
		{
			this._version = Version.Parse(this.unparsedVersion);
			this._versionWrapper = new VersionWrapper()
			{
				Version = this._version
			};
			this._json = JsonSerializer.Serialize(this._versionWrapper);
		}

		[Benchmark]
		[BenchmarkCategory("Serialize", "Current")]
		public void CurrentConverterSerialize() => _consumer.Consume(JsonSerializer.Serialize(this._versionWrapper));

		[Benchmark]
		[BenchmarkCategory("Serialize", "Proposed")]
		public void ProposedConverterSerialize() => _consumer.Consume(JsonSerializer.Serialize(this._versionWrapper, optionsWithProposedConverter));

		[Benchmark]
		[BenchmarkCategory("Deserialize", "Current")]
		public void CurrentConverterDeserialize() => _consumer.Consume(JsonSerializer.Deserialize<VersionWrapper>(this._json));

		[Benchmark]
		[BenchmarkCategory("Deserialize", "Proposed")]
		public void ProposedConverterDeserialize() =>
			_consumer.Consume(JsonSerializer.Deserialize<VersionWrapper>(this._json, optionsWithProposedConverter));
	}

	public sealed class ProposedJsonVersionConverter : JsonConverter<Version>
	{
		private const int MaxStringRepresentationOfPositiveInt32 = 10; // int.MaxValue.ToString().Length

		private const int
			MaxStringLengthOfVersion = (MaxStringRepresentationOfPositiveInt32 * VersionComponentsCount) + 1 + 1 + 1; // 43, 1 is length of '.'

		private const int VersionComponentsCount = 4; // Major, Minor, Build, Revision

		public override Version Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			ReadOnlySpan<byte> rawVersion = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
			Span<int?> versionComponents = stackalloc int?[VersionComponentsCount] {null, null, null, null}; // 86 bytes
			int indexOfDot = GetIndexOfDot(rawVersion);
			// we won't need to calculate IndexOf of backslash since Utf8JsonReader has internal field indicating if value has backslash
			if (rawVersion.IndexOf((byte) '\\') != -1 || indexOfDot == -1)
			{
				ThrowHelper.ThrowJsonException();
			}

			for (int i = 0; i < VersionComponentsCount; i++)
			{
				bool lastComponent = indexOfDot == -1;
				var readOnlySpan = lastComponent ? rawVersion : rawVersion.Slice(0, indexOfDot);
				if (TryGetVersionComponent(readOnlySpan, out int value))
				{
					versionComponents[i] = value;
					rawVersion = rawVersion.Slice(indexOfDot + 1);
					indexOfDot = GetIndexOfDot(rawVersion);
					if (lastComponent)
						break;
				}
				else
				{
					ThrowHelper.ThrowJsonException();
				}
			}

			var major = versionComponents[0];
			var minor = versionComponents[1];
			var build = versionComponents[2];
			var revision = versionComponents[3];
			if (major.HasValue && minor.HasValue && build.HasValue && revision.HasValue)
			{
				return new Version(major.Value, minor.Value, build.Value, revision.Value);
			}
			else if (major.HasValue && minor.HasValue && build.HasValue)
			{
				return new Version(major.Value, minor.Value, build.Value);
			}
			else if (major.HasValue && minor.HasValue)
			{
				return new Version(major.Value, minor.Value);
			}

			ThrowHelper.ThrowJsonException();
			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryGetVersionComponent(ReadOnlySpan<byte> source, out int value) => Utf8Parser.TryParse(source, out value, out _);

		public static int GetIndexOfDot(ReadOnlySpan<byte> source) => source.IndexOf((byte) '.');

		public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
		{
			/*
			 stackalloc of 43 chars will alloc 86 bytes since sizeof(char) == 2
			 
			maybe we can calculate length based on version value, like checking if
			optional Build and Revision property are present but i'm not sure,
			what will be better, to substract integers or
			stackalloc buffer that will be bigger than needed
			*/
			Span<char> span = stackalloc char[MaxStringLengthOfVersion];
			value.TryFormat(span, out int charsWritten);
			writer.WriteStringValue(span.Slice(0, charsWritten));
		}

		public static class ThrowHelper
		{
			[DoesNotReturn]
			public static void ThrowJsonException() => throw new JsonException();
		}
	}

	public class VersionWrapper
	{
		public Version Version { get; init; }
	}

	public static class TestCases
	{
		public const string UnparsedTestCase1 = "1.0";
		public const string UnparsedTestCase2 = UnparsedTestCase1 + ".0";
		public const string UnparsedTestCase3 = UnparsedTestCase2 + ".0";
		public const string UnparsedTestCase4 = "2147483647.2147483647";
		public const string UnparsedTestCase5 = UnparsedTestCase4 + ".2147483647";
		public const string UnparsedTestCase6 = UnparsedTestCase5 + ".2147483647";
	}
}