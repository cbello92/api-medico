using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.IO;

namespace AgendaApiMedico.Core.Extensions
{
    public class Logger
    {
        private static readonly string Directorio = "C:\\Logger";
        private static string NombreArchivo = ".log-{Date}.txt";
        private static string Ruta = @"C:\\Logger\\";
        private static readonly string Format = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} | [{Level}] [{SourceContext}] | {Message}{NewLine}{Exception} \n";

        public static ILogger BuildLogger(string nombreAPI)
        {

            NombreArchivo = nombreAPI + NombreArchivo;

            Ruta += NombreArchivo;

            if (!Directory.Exists(Directorio))
            {
                Directory.CreateDirectory(Path.Combine(Directorio));
            }

            var log = new LoggerConfiguration()
                .WriteTo.Console(theme: SystemConsoleTheme.Colored)
                .MinimumLevel.Error()
                .WriteTo.RollingFile(Ruta, outputTemplate: Format)
                .CreateLogger();

            return log;
        }
    }
}
