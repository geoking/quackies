# M4 evaluation baseline

`Policies/M4BaselinePolicy.cs` preserves the `DuckNormalPolicy` decision algorithm
from Quackies commit `327f75169453de8a57c752abb547991a54b2e3b1`.

The original file is:

`src/Quackies.Core/Ducks/AI/DuckNormalPolicy.cs`

Its SHA-256 is:

`f8d0e7e26cf2152740c25b4666d0d94bdc1cf5fe8cdab55584bee15088a2f6ea`

Only the namespace, class name, and decision result type were adapted so the
historical algorithm can coexist with the current Core candidate. The evaluation
result type carries the same issued `GameAction` and reason because the Core
`DuckPolicyDecision` constructor is internal. Provenance constants and this note
are metadata; they do not participate in decisions.
