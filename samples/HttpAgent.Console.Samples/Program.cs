var content = await HttpRemoteClient.Service.GetAsStringAsync("https://furion.net", builder => builder.Profiler());
Console.WriteLine(content);