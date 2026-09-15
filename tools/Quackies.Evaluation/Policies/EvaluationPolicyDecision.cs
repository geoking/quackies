using Quackies.Core.Match;

namespace Quackies.Evaluation.Policies;

public sealed record EvaluationPolicyDecision(GameAction Action, string Reason);
