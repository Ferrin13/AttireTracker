module DatabaseModels

open System

type ActivityTypeId = CheckIn = 1 | CheckOut = 2

[<CLIMutable>]
type AttirePiece = 
    { id: int
      name: string
      photoUrl: string
      rfidUid: string }

type NewAttirePiece = 
    { name: string
      photoUrl: string 
      rfidUid: string }

type NewAttirePieceActivityHistory = 
    { attire_piece_id: int
      activity_type_id: int
      activity_time: DateTimeOffset }

[<CLIMutable>]
type CombinedActivityHistory = 
    { attirePieceActivityHistoryId: int
      activityTypeId: ActivityTypeId
      description: string
      activityTime: DateTimeOffset }

