namespace AttireTracker.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open FSharp.Json
open AttirePieceService
open AttireActivityService

[<ApiController>]
type AttirePieceController (logger : ILogger<AttirePieceController>) =
    inherit ControllerBase()

    [<HttpGet("/attirePieces")>]
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

    [<HttpGet("/attirePieces/{rfidUid}/activity/last")>]
    member __.GetLastActivity(rfidUid: string) : string =
        getLastActivityByUid(rfidUid) 
        |> Async.RunSynchronously 
        |> Json.serialize

    [<HttpPost("/attirePieces/{rfidUid}/activity")>]
    member __.ToggleActivity(rfidUid: string) : string =
        let lastActivity = getLastActivityByUid(rfidUid) |> Async.RunSynchronously
        let newActivityType = nextActivityType(lastActivity)

        addToActivityHistory(rfidUid, newActivityType) |> Async.RunSynchronously |> ignore

        getLastActivityByUid(rfidUid) 
        |> Async.RunSynchronously 
        |> Json.serialize
