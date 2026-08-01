using Content.Shared.EntityTable.Conditions;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.EntityTable.EntitySelectors;

/// <summary>
/// Gets spawns from all of the child selectors
/// </summary>
public sealed partial class AllSelector : EntityTableSelector
{
    /// <summary>
    /// All children selectors to pick from.
    /// </summary>
    [DataField(required: true)]
    public List<EntityTableSelector> Children;

    /// <summary>
    /// A list of conditions added to the <see cref="EntityTableSelector.TravelingConditions"/> of children.
    /// </summary>
    [DataField]
    public List<EntityTableCondition> ChildConditions = new();

    protected override IEnumerable<EntProtoId> GetSpawnsImplementation(IRobustRandom rand,
        IEntityManager entMan,
        IPrototypeManager proto,
        EntityTableContext ctx)
    {
        foreach (var child in Children)
        {
            child.TravelingConditions.AddRange(ChildConditions);
            child.TravelingConditions.AddRange(TravelingConditions);

            foreach (var spawn in child.GetSpawns(rand, entMan, proto, ctx))
            {
                yield return spawn;
            }
        }
    }

    protected override IEnumerable<(EntProtoId spawn, double)> ListSpawnsImplementation(IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx)
    {
        foreach (var child in Children)
        {
            foreach (var (spawn, prob) in child.ListSpawns(entMan, proto, ctx))
            {
                yield return (spawn, prob);
            }
        }
    }

    protected override IEnumerable<(EntProtoId spawn, double)> AverageSpawnsImplementation(IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx)
    {
        foreach (var child in Children)
        {
            foreach (var (spawn, prob) in child.AverageSpawns(entMan, proto, ctx))
            {
                yield return (spawn, prob);
            }
        }
    }
}
