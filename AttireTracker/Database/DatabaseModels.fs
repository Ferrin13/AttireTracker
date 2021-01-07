module DatabaseModels

open System

type ActivityTypeId = Wardrobe = 1 | InUse = 2 | Laundry = 3
type AttireCategoryId = Top = 1 | Bottom = 2 | TopOverwear = 3

[<CLIMutable>]
type AttirePiece = 
    { id: int
      name: string
      photoUrl: string
      rfidUid: string
      categoryId: AttireCategoryId }

type NewAttirePiece = 
    { name: string
      photoUrl: string 
      rfidUid: string
      categoryId: AttireCategoryId }

type NewAttirePieceActivityHistory = 
    { attire_piece_id: int
      activity_type_id: int
      activity_time: DateTimeOffset }

[<CLIMutable>]
type CombinedActivityHistory = 
    { attirePieceActivityHistoryId: int
      activityTypeId: ActivityTypeId
      pieceName: string
      description: string
      activityTime: DateTimeOffset }

