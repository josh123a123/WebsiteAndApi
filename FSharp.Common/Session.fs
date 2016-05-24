namespace DevSpace.FSharp.Common

type Session =
  {
    Id:int;
    UserId:int;
    Title:string;
    Abstract:string;
    Notes:string;
    Accepted:bool;
    Tags:Tag[]
  }

  member this.WithId( value ) =
    { this with Id = value }

  member this.WithUserId( value ) =
    { this with UserId = value }

  member this.WithTitle( value ) =
    { this with Title = value }

  member this.WithAbstract( value ) =
    { this with Abstract = value }

  member this.WithNotes( value ) =
    { this with Notes = value }

  member this.WithAccepted( value ) =
    { this with Accepted = value }

  member this.WithTags( value ) =
    { this with Tags = value }
