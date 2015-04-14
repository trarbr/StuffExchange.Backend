module StuffExchange.Api.Program

open System
open Nancy.Hosting.Self

[<EntryPoint>]
let main args =
    let uri = Uri("http://localhost:3571")

    let configuration = HostConfiguration()
    configuration.RewriteLocalhost <- false

    use host = new NancyHost(configuration, uri)
    host.Start()

    Console.WriteLine("Your application is running on " + uri.AbsoluteUri)
    Console.WriteLine("Press any [Enter] to close the host.")
    Console.ReadLine() |> ignore
    0
