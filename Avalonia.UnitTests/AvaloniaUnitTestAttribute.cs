using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Headless;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace Avalonia.UnitTests;

public class AvaloniaUnitTestAttribute: TestAttribute, ITestAction, IWrapTestMethod {
    private HeadlessUnitTestSession session;
    public void BeforeTest(ITest test)
    {
        session = HeadlessUnitTestSession.StartNew(typeof(Application));
    }

    public void AfterTest(ITest test)
    {
        session.Dispose();
    }
    
    public TestCommand Wrap(TestCommand command) {
        session = HeadlessUnitTestSession.StartNew(typeof(Application));
        return new DispatchCommand(command, session);
    }

    public ActionTargets Targets => ActionTargets.Test;
    
    private class DispatchCommand : TestCommand
    {
        private readonly TestCommand _innerCommand;
        private HeadlessUnitTestSession _session;

        public DispatchCommand(TestCommand innerCommand, HeadlessUnitTestSession session) 
            : base(innerCommand.Test)
        {
            _innerCommand = innerCommand;
            _session = session;
        }

        public override TestResult Execute(TestExecutionContext context) {
            TestResult result;
            try {
                result = _session.Dispatch(async () => await PerformAction(context)
                    , CancellationToken.None).GetAwaiter().GetResult();
            } catch (System.PlatformNotSupportedException) {
                // try again
                result = _session.Dispatch(async () => await PerformAction(context)
                    , CancellationToken.None).GetAwaiter().GetResult();
            }
            
            return result;
        }

        private async Task<TestResult> PerformAction(TestExecutionContext context) {
            //result = _innerCommand.Execute(context);
            var testMethod = _innerCommand.Test.Method;
            var methodInfo = testMethod!.MethodInfo;

            var result = methodInfo.Invoke(context.TestObject, _innerCommand.Test.Arguments);
            // Only Task, non generic ValueTask are supported in async context. No ValueTask<> nor F# tasks.
            if (result is Task task)
            {
                await task;
            }
            else if (result is ValueTask valueTask)
            {
                await valueTask;
            }
                    
            context.CurrentResult.SetResult(ResultState.Success);
            if (context.CurrentResult.AssertionResults.Count > 0) {
                context.CurrentResult.RecordTestCompletion();
            }

            return context.CurrentResult;
        }
    }
}
