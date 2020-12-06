namespace AttireTracker.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

[<ApiController>]
[<Route("/attirePieces")>]
type AttireController (logger : ILogger<AttireController>) =
    inherit ControllerBase()

    [<HttpGet>]
    member __.Get() : string =
        "Eventually some attire"
