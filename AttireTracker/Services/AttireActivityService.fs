module AttireActivityService

open DatabaseModels
open InternalModels

let private nextWardrobeActivityType(typeId: ActivityTypeId) =
    match typeId with
    | ActivityTypeId.Wardrobe -> ActivityTypeId.InUse
    | _ -> ActivityTypeId.Wardrobe

let private nextLaundryActivityType(typeId: ActivityTypeId) =
    match typeId with
    | ActivityTypeId.Laundry -> ActivityTypeId.Wardrobe
    | _ -> ActivityTypeId.Laundry

let private nextActivityTypeFromUnions(typeId: ActivityTypeId, toggleType: ToggleType): ActivityTypeId =
    match toggleType with
    | ToggleType.Wardrobe -> nextWardrobeActivityType(typeId)
    | ToggleType.Laundry -> nextLaundryActivityType(typeId)
    | _ -> invalidArg "toggleType" "Toggle type did not have  valid value" 

(*
    The legal state changes are:
    InUse -> Wardobe, InUse -> Laundry, Wardrobe -> InUse, Wardrobe -> Laundry, Laundry -> Wardrobe
    The wardrobe toggle type handles InUse -> Wardrobe and Wardrobe -> InUse
    The laundry toggle type handles InUse -> Laundry, Wardrobe -> Laundry, and Laundry -> Wardrobe
    Note that because Laundry cannot move to InUse, we don't need any additional information to move
    the piece to its correct state.
*)
let nextActivityType(previousActivity: Option<CombinedActivityHistory>, toggleType: ToggleType) = 
    match previousActivity with
    | Some(activity) -> nextActivityTypeFromUnions(activity.activityTypeId, toggleType)
    | _ -> ActivityTypeId.Wardrobe //If this is the first activity for this piece, it goes in the wardrove

let addToActivityHistory = AttireActivityQueries.addToActivityHistory
let getLastActivityByUid= AttireActivityQueries.getLastActivityByUid
let getActivityHistoryByUid(rfidUid: string) =
    async {
        let! results = AttireActivityQueries.getActivityHistoryByUid(rfidUid)
        return results |> Seq.take(10) |> Seq.toList
    }
