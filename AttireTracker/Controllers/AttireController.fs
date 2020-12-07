namespace AttireTracker.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open DatabaseActions
open DatabaseModels

[<ApiController>]
[<Route("/attirePieces")>]
type AttireController (logger : ILogger<AttireController>) =
    inherit ControllerBase()

    [<HttpGet>]
    member __.Get() : string =
        let newPiece = { name = "Live Test"; photoUrl = "PHOTO_URL_EXAMPLE" }

        insertAttirePiece(newPiece) |> Async.RunSynchronously |> ignore

        getAllAttirePieces() 
        |> Async.RunSynchronously
        |> Seq.map(fun a -> a.id.ToString() + ", " + a.name + ", " + a.photoUrl)
        |> String.concat "\n"
