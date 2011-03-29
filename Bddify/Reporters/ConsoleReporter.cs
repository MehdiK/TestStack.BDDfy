﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bddify.Core;

namespace Bddify.Reporters
{
    public class ConsoleReporter : IProcessor
    {
        readonly List<Exception> _exceptions = new List<Exception>();
        private int _longestStepSentence;

        public ProcessType ProcessType
        {
            get { return ProcessType.Report; }
        }

        public void Process(Scenario scenario)
        {
            var reporterRegistry
                = new Dictionary<StepExecutionResult, Action<ExecutionStep>>
                          {
                              {StepExecutionResult.Passed, s => ReportOnStep(s)},
                              {StepExecutionResult.Failed, s => ReportOnStep(s, true)},
                              {StepExecutionResult.Inconclusive, s => ReportOnStep(s)},
                              {StepExecutionResult.NotImplemented, s => ReportOnStep(s, true)},
                              {StepExecutionResult.NotExecuted, s => ReportOnStep(s)}
                          };

            Report(scenario);

            if (scenario.Steps.Any())
            {
                _longestStepSentence = scenario.Steps.Max(s => s.ReadableMethodName.Length);
                foreach (var step in scenario.Steps.Where(s => s.ShouldReport))
                    reporterRegistry[step.Result](step);
            }

            ReportExceptions();

            Console.WriteLine("================================================================================");
        }

        void ReportOnStep(ExecutionStep step, bool reportOnException = false)
        {
            var message =
                string.Format
                    ("{0}  [{1}]",
                    step.ReadableMethodName.PadRight(_longestStepSentence + 5),
                    NetToString.FromCamelName(step.Result.ToString()));

            if(reportOnException)
            {
                _exceptions.Add(step.Exception);
                message += string.Format(" :: Exception Stack Trace below on number [{0}]", _exceptions.Count);
            }

            if (step.Result == StepExecutionResult.Inconclusive || step.Result == StepExecutionResult.NotImplemented)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (step.Result == StepExecutionResult.Failed)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (step.Result == StepExecutionResult.NotExecuted)
                Console.ForegroundColor = ConsoleColor.Gray;
            
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        void ReportExceptions()
        {
            if (_exceptions.Count == 0)
                return;

            Console.WriteLine(string.Empty);
            Console.WriteLine("Exceptions' Details: ");
            Console.WriteLine(string.Empty);

            for (int index = 0; index < _exceptions.Count; index++)
            {
                var exception = _exceptions[index];
                Console.WriteLine(string.Format("[{0}]:", index + 1));
                
                if (string.IsNullOrEmpty(exception.Message))
                    Console.WriteLine(exception.Message);
         
                Console.WriteLine(exception.StackTrace);
            }
        }

        static void Report(Scenario scenario)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Scenario: " +  scenario.ScenarioText + Environment.NewLine);
        }
    }
}