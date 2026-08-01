using Content.Shared.EntityTable.Conditions;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.EntityTable.EntitySelectors;

/// <summary>
/// Gets the spawns from the entity table prototype specified.
/// Can be used to reuse common tables.
/// </summary>
public sealed partial class NestedSelector : EntityTableSelector
{
    /// <summary>
    /// The prototype from which to draw random items.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<EntityTablePrototype> TableId;

    /// <summary>
    /// A list of conditions added to the <see cref="EntityTableSelector.TravelingConditions"/> of nested table.
    /// </summary>
    [DataField]
    public List<EntityTableCondition> ChildConditions = new();

    protected override IEnumerable<EntProtoId> GetSpawnsImplementation(IRobustRandom rand,
        IEntityManager entMan,
        IPrototypeManager proto,
        EntityTableContext ctx)
    {
        var table = proto.Index(TableId).Table;
        table.TravelingConditions.AddRange(ChildConditions);
        table.TravelingConditions.AddRange(TravelingConditions);

        return table.GetSpawns(rand, entMan, proto, ctx);
    }

    protected override IEnumerable<(EntProtoId spawn, double)> ListSpawnsImplementation(IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx)
    {
        return proto.Index(TableId).Table.ListSpawns(entMan, proto, ctx);
    }

    protected override IEnumerable<(EntProtoId spawn, double)> AverageSpawnsImplementation(IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx)
    {
        return proto.Index(TableId).Table.AverageSpawns(entMan, proto, ctx);
    }
}
