module DatabaseModels

open System

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
    { attire_piece_activity_history_id: int
      description: string
      activity_time: DateTimeOffset }

type ActivityTypeId = CheckIn = 1 | CheckOut = 2