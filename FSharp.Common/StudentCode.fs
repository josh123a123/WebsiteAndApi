namespace DevSpace.FSharp.Common

open System.Runtime.Serialization

[<DataContract>]
type StudentCode =
  {
    [<field: DataMember>]Id:int;
    [<field: DataMember>]Email:string;
    [<field: DataMember>]Code:string
  }

  member this.WithId( value ) =
    { this with Id = value }

  member this.WithEmail( value ) =
    { this with Email = value }

  member this.WithCode( value ) =
    { this with Code = value }
