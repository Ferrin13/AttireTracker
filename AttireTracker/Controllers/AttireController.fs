namespace AttireTracker.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open FSharp.Json
open AttirePieceService
open AttireActivityService
open InternalModels

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

    // We prefer toggles here rather than the more "correct" approach of parameterizing the
    // state because it offloads computational work from the client to the API.
    // See nextActivityType comment for more information on valid state changes.
    [<HttpPost("/attirePieces/{rfidUid}/activity/wardrobe")>]
    member __.ToggleWardrobe(rfidUid: string) : string =
        let lastActivity = getLastActivityByUid(rfidUid) |> Async.RunSynchronously
        let newActivityType = nextActivityType(lastActivity, ToggleType.Wardrobe)

        addToActivityHistory(rfidUid, newActivityType) |> Async.RunSynchronously |> ignore

        getLastActivityByUid(rfidUid) 
        |> Async.RunSynchronously 
        |> Json.serialize

    // We prefer toggles here rather than the more "correct" approach of parameterizing the
    // state because it offloads computational work from the client to the API.
    // See nextActivityType comment for more information on valid state changes.
    [<HttpPost("/attirePieces/{rfidUid}/activity/laundry")>]
    member __.ToggleLaundry(rfidUid: string) : string =
        let lastActivity = getLastActivityByUid(rfidUid) |> Async.RunSynchronously
        let newActivityType = nextActivityType(lastActivity, ToggleType.Laundry)

        addToActivityHistory(rfidUid, newActivityType) |> Async.RunSynchronously |> ignore

        getLastActivityByUid(rfidUid) 
        |> Async.RunSynchronously 
        |> Json.serialize
