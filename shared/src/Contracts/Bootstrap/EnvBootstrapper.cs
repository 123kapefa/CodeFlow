using DotNetEnv;

namespace Contracts.Bootstrap;

public static class EnvBootstrapper {

    public static void Load() {
        // читаем имя среды (Development/Production/…)
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var root = Directory.GetCurrentDirectory();
        var low = env.ToLowerInvariant();

        // приоритет: .env.{Environment}.local -> .env.{Environment} -> .env.local -> .env
        var candidates = new[]
        {
            Path.Combine(root, $".env.{low}.local"),
            Path.Combine(root, $".env.{low}"),
            Path.Combine(root, ".env.local"),
            Path.Combine(root, ".env"),
        };

        foreach(var f in candidates)
            if(File.Exists(f)) LoadEnvNoOverwrite(f);
    }


    static void LoadEnvNoOverwrite( string path ) {
        foreach(var raw in File.ReadAllLines(path)) {
            var line = raw.Trim();
            if(line.Length == 0 || line.StartsWith("#")) continue;

            var i = line.IndexOf('=');
            if(i <= 0) continue;

            var key = line.Substring(0, i).Trim();
            var val = line.Substring(i + 1).Trim();

            if((val.StartsWith("\"") && val.EndsWith("\"")) ||
                (val.StartsWith("'") && val.EndsWith("'")))
                val = val.Substring(1, val.Length - 2);

            if(Environment.GetEnvironmentVariable(key) is null)
                Environment.SetEnvironmentVariable(key, val);
        }
    }
}
