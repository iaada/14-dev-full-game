using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Destructible.Thresholds;
using Content.Shared.EntityTable;
using Content.Shared.EntityTable.Conditions;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server.Commands;

[AdminCommand(AdminFlags.Fun)]
internal sealed partial class TestRangeConditionSpawn : IConsoleCommand
{
    [Dependency] private IEntityManager _entMan = default!;
    [Dependency] private IPrototypeManager _protoMan = default!;

    public string Command => "test:RangeConditionSpawn";
    public string Description => "Prints the id of an entity gotten from an entity table using the supplied context range.";
    public string Help => "Usage: test:RangeConditionSpawn <EntityTableSelector> <min> <max>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 3)
        {
            shell.WriteLine("Incorrect number of arguments.");
            return;
        }

        var tableSys = _entMan.EntitySysManager.GetEntitySystem<EntityTableSystem>();

        var table = _protoMan.Index<EntityTablePrototype>(args[0]);
        var range = new MinMax(int.Parse(args[1]), int.Parse(args[2]));

        // Create a context
        var ctx = new EntityTableContext(new Dictionary<string, object>
        {
            { BudgetRangeCondition.BudgetRangeContextKey, range },
        });

        var spawns = tableSys.GetSpawns(table, ctx: ctx);
        foreach (var spawn in spawns)
        {
            shell.WriteLine(spawn);
        }
    }
}
