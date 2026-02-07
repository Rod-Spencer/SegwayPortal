using Segway.EF.SegwayCntxt;

using var db = new SegwayContext();

// SELECT
var entities = db.Manufacturing_Component_Assemblies
    .OrderBy(x => x.Date_Time_Created)
    .Skip(0)
    .Take(10)
    .ToList();

foreach (var entity in entities)
{
    Console.WriteLine($"Parent: {entity.Parent_Serial}, Child: {entity.Child_Serial}, P: {entity.Part_Type}, Create: {entity.Date_Time_Created}, By: {entity.Created_By}");
}

Console.WriteLine("--- Portal Service Tools ---");
var pst = db.PortalServiceTools.OrderBy(x => x.DisplayName).ToList();
pst.ForEach(x => Console.WriteLine($"ID: {x.Id}, Name: {x.DisplayName}"));

Console.WriteLine("--- Portal Tool Installer Types ---");
var ptit = db.PortalToolInstallerTypes.OrderBy(x => x.Description).ToList();
ptit.ForEach(x => Console.WriteLine($"ID: {x.Id}, Name: {x.Description}"));

Console.WriteLine("--- Portal Tool States ---");
var pts = db.PortalToolStates.OrderBy(x => x.Description).ToList();
pts.ForEach(x => Console.WriteLine($"ID: {x.Id}, Name: {x.Description}"));

Console.WriteLine("--- Portal Tool Types ---");
var ptt = db.PortalToolTypes.OrderBy(x => x.Description).ToList();
ptt.ForEach(x => Console.WriteLine($"ID: {x.Id}, Name: {x.Description}"));

Console.WriteLine("--- Portal Users ---");
var pu = db.PortalUsers.OrderBy(x => x.User_Name).ToList();
pu.ForEach(x => Console.WriteLine($"ID: {x.ID}, Name: {x.User_Name}"));

Console.WriteLine("--- Portal Users Access ---");
var pua = db.PortalUsersAccess.OrderBy(x => x.Description).ToList();
pua.ForEach(x => Console.WriteLine($"ID: {x.ID}, Name: {x.Description}"));

Console.WriteLine("--- {Portal Users - Find me ---");
var me = db.PortalUsers.FirstOrDefault(x => x.User_Name == "rod.spencer");
if (me is not null) Console.WriteLine($"ID: {me.ID}, Name: {me.User_Name}");

