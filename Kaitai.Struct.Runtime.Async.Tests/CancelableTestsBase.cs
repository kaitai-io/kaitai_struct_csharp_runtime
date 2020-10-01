using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
  public abstract class CancelableTestsBase
  {
    protected readonly CancellationToken CancellationToken;

    protected CancelableTestsBase(bool isTestingCancellation)
    {
      CancellationToken = new CancellationToken(isTestingCancellation);
    }

    protected async Task Evaluate(Func<Task> assertFunc)
    {
      if (CancellationToken.IsCancellationRequested)
      {
        await Assert.ThrowsAsync<TaskCanceledException>(assertFunc);
      }
      else
      {
        await assertFunc();
      }
    }

    protected async Task EvaluateMaybeCancelled(Func<Task> assertFunc)
    {
      try
      {
        await assertFunc();
      }
      catch (TaskCanceledException)
      {
      }
    }

    protected async Task Evaluate<TExpectedException>(Func<Task> assertFunc) where TExpectedException : Exception
    {
      try
      {
        await assertFunc();
      }
      catch (TaskCanceledException)
      {
      }
      catch (TExpectedException)
      {
      }
    }
  }
}