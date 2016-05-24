namespace DevSpace.FSharp.Common

type Tag =
  {
    Id:int;
    Text:string
  }

  member this.WithId( value ) =
    { this with Id = value }

  member this.WithText( value ) =
    { this with Text = value }
