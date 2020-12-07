module DatabaseModels

[<CLIMutable>]
type AttirePiece = 
    { id: int
      name: string
      photoUrl: string }

type NewAttirePiece = 
    { name: string
      photoUrl: string }