using System;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace NamedPipe
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                    throw new ArgumentException("usage: namedpipe PIPENAME");

                var pipeName = args[0];
                if (pipeName.Replace('/', '\\').ToLower().StartsWith(@"\\.\pipe\"))
                    pipeName = pipeName.Substring(9);
                using (var stream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                {
                    stream.WaitForConnection();
                    Task.Run(async () =>
                    {
                        await Console.OpenStandardInput().CopyToAsync(stream);
                        stream.Close();
                    });
                    stream.CopyToAsync(Console.OpenStandardOutput()).Wait();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Environment.Exit(2);
            }
        }
    }
}
