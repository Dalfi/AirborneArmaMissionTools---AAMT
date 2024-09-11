// See https://aka.ms/new-console-template for more information

using AirborneArmaMissionTools.SQM;


Console.WriteLine("Hello, World!");

//SQMToJSONParser.Parse("J:\\InstallFolder\\mission.sqm", "J:\\InstallFolder\\mission.json");

JSONToSQMParser.Parse("J:\\InstallFolder\\mission.json", "J:\\InstallFolder\\mission_parsed.sqm");

Console.ReadLine();