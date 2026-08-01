using Content.Shared.Destructible.Thresholds;
using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityTable.Conditions;

/// <summary>
/// Condition that succeeds when the selector has a cost between a supplied range.
/// Always fails if no context was passed to it.
/// </summary>
public sealed partial class BudgetRangeCondition : EntityTableCondition
{
    /// <summary>
    /// The context key this condition expects.
    /// Condition fails if the value isn't <see cref="MinMax"/>.
    /// </summary>
    public static readonly string BudgetRangeContextKey = "BudgetRange";

    /// <summary>
    /// Used for determining the cost for the budget.
    /// If null, attempts to fetch the cost from the attached selector.
    /// </summary>
    [DataField]
    public int? CostOverride;

    /// <inheritdoc/>
    protected override bool EvaluateImplementation(EntityTableSelector root,
                                                    IEntityManager entMan,
                                                    IPrototypeManager proto,
                                                    EntityTableContext ctx)
    {
        if (!ctx.TryGetData<MinMax>(BudgetRangeContextKey, out var budgetRange))
            return false;

        int cost;
        if (CostOverride != null)
        {
            // If the condition has an override on it, use that instead of what's on the entity
            cost = CostOverride.Value;
        }
        else
        {
            // If there's no override, get it from the entity on the selector
            if (root is not EntSelector entSelector)
            {
                var log = Logger.GetSawmill("BudgetRangeCondition");
                log.Error("CostOverride is required for selectors other than EntSelector.");
                return false;
            }

            if (!proto.Index(entSelector.Id).TryComp(out TableBudgetCostComponent? costComponent, entMan.ComponentFactory))
            {
                var log = Logger.GetSawmill("BudgetRangeCondition");
                log.Error($"Selected object {entSelector.Id} does not have a TableBudgetCostComponent.");
                return false;
            }

            cost = costComponent.Cost;
        }

        return budgetRange.Min <= cost && cost <= budgetRange.Max;
    }
}

/// <summary>
/// Tracks how much this entity "costs" when being selected through Entity Tables and <see cref="BudgetRangeCondition"/>.
/// </summary>
[RegisterComponent]
public sealed partial class TableBudgetCostComponent : Component
{
    /// <summary>
    /// The value of this entity to the selector.
    /// </summary>
    [DataField(required: true)]
    public int Cost;
}
