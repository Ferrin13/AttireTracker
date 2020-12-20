namespace AttireTracker.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open DatabaseActions
open DatabaseModels
open FSharp.Json

[<ApiController>]
[<Route("/attirePieces")>]
type AttirePieceController (logger : ILogger<AttirePieceController>) =
    inherit ControllerBase()

    [<HttpGet>]
    member __.GetAll(rfidUid: string) : string =
        let gePieceByUid(rfidUid: string) =
            let optionPiece = getAttirePieceByUid(rfidUid) |> Async.RunSynchronously
            match optionPiece with 
            | Some piece -> Json.serialize(piece)
            | None -> "No piece found"

        let getAllPieces() = 
            getAllAttirePieces()
            |> Async.RunSynchronously
            |> List.ofSeq
            |> Json.serialize

        match rfidUid with 
        | null -> getAllPieces()
        | _ -> gePieceByUid(rfidUid)

    [<HttpGet("/attirePieces/{rfidUid}/activity")>]
    member __.GetActivity(rfidUid: string) : string =
        getActivityHistoryByUid(rfidUid) 
        |> Async.RunSynchronously 
        |> Json.serialize

    [<HttpPost("/attirePieces/{rfidUid}/activity")>]
    member __.Post(rfidUid: string) : string =
        addToActivityHistory(rfidUid, ActivityTypeId.CheckOut) |> Async.RunSynchronously |> ignore

        getActivityHistoryByUid(rfidUid) 
        |> Async.RunSynchronously 
        |> Json.serialize
