// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using SharedLibrary;

string baseURL = "http://localhost:8080/";

Console.WriteLine("DOBRODOŠAO U AIBG V5.0 IGRU \n\n\n");


Console.WriteLine("Ukucaj Id svog lika - mora biti broj");
int id= Convert.ToInt32(Console.ReadLine());
Player player=new Player();
Console.WriteLine("Ukucaj sifru svoje igre");
string gameId = Console.ReadLine();

HttpClient client = new HttpClient();
string s = $"{baseURL}game/connect/{gameId}/{id}";
var result = await client.PostAsync(s,null);

if (result.IsSuccessStatusCode)
{
    string jsonString = await result.Content.ReadAsStringAsync();
    
    Map map = JsonConvert.DeserializeObject<Map>(jsonString);
    
    Console.WriteLine($"Mapa uspešno učitana!");
    Console.WriteLine(map);
    var field=map.Grid.Find(info => info.Entity != null && info.Entity.Id == id && info.Entity is Player);
    player = (Player)field.Entity;
    Console.WriteLine(player.Name);
    Console.WriteLine(player.Id);
    Console.WriteLine(player.Position.X +" "+ player.Position.Y);
}
else
{
    Console.WriteLine($"Greška na serveru: {result.StatusCode}");
}

string move1 = $"{baseURL}player/move/gameId/{gameId}";
MoveRequest mr = new MoveRequest(){Direction = Direction.UP,newPosition = new Position(0,1),playerId=player.Id,Steps=1};


if (player.Position.X > 0)
{
    mr.Direction = Direction.DOWN;
    var newPos = new Position(player.Position.X, player.Position.Y-1);
    mr.newPosition = newPos;
}

if (!player.First)
{
    Console.WriteLine("Waiting for the first player to play his move");
    await Task.Delay(3000);
}

string jsonBody = JsonConvert.SerializeObject(mr);
var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

result = await client.PutAsync(move1,httpContent);
if (result.IsSuccessStatusCode)
{
    string jsonString = await result.Content.ReadAsStringAsync();
    
    Map map = JsonConvert.DeserializeObject<Map>(jsonString);
    
    Console.WriteLine($"Pomereno uspesno");
    var field=map.Grid.Find(info => info.Entity != null && info.Entity.Id == id && info.Entity is Player);
    player = (Player)field.Entity;
    Console.WriteLine($"filed je {field.Position.X},{field.Position.Y}");
    Console.WriteLine($"Trenutno stanje igraca je sledece \n ({player.Position.X},{player.Position.Y})");
}
else
{
    Console.WriteLine($"Greška na serveru: {result.StatusCode}");
}

Console.WriteLine("\nPritisni Enter za izlaz...");
Console.ReadLine();