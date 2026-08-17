using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Web.Poc.Infrastructure.UnitTests;

[TestClass]
public sealed class MyServiceTests
{
	[TestMethod]
	public async Task MyMethodTestAsync()
	{
		Channel<int> channel = Channel.CreateBounded<int>(new BoundedChannelOptions(3)
		{
			FullMode = BoundedChannelFullMode.Wait
		});

		for (int i = 0; i < 4; i++)
		{
			var result = channel.Writer.TryWrite(i);
			System.Diagnostics.Debug.WriteLine($"TryWrite {i}: {result}");
			
			System.Diagnostics.Debug.WriteLine($"Count: {channel.Reader.Count} - " +
											   $"CanCount: {channel.Reader.CanCount} - " +
											   $"CanPeek: {channel.Reader.CanPeek} - ");
		}

		for (int i = 0; i < 4; i++)
		{
			var result = channel.Reader.TryRead(out var item);
			System.Diagnostics.Debug.WriteLine($"TryRead {i}: {item} - {result}");
		}
	}
}
