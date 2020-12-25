module AttireActivityService

open DatabaseModels

let oppositeActivityType(typeId: ActivityTypeId): ActivityTypeId =
    match typeId with
    | ActivityTypeId.CheckIn -> ActivityTypeId.CheckOut
    | ActivityTypeId.CheckOut -> ActivityTypeId.CheckIn
    | _ -> ActivityTypeId.CheckIn

let nextActivityType(previousActivity: Option<CombinedActivityHistory>) = 
    match previousActivity with
    | Some(activity) -> oppositeActivityType(activity.activityTypeId)
    | _ -> ActivityTypeId.CheckIn

let addToActivityHistory = AttireActivityQueries.addToActivityHistory
let getLastActivityByUid = AttireActivityQueries.getLastActivityByUid
let getActivityHistoryByUid = AttireActivityQueries.getActivityHistoryByUid

