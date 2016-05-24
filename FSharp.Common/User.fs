namespace DevSpace.FSharp.Common

open System

type User =
  {
    Id:int;
    DisplayName:string;
    EmailAddress:string;
    Bio:string;
    Twitter:string;
    Permissions:byte;
    PasswordHash:string;
    Website:string;
    SessionToken:Guid;
    SessionExpires:DateTime
  }

  member this.WithId( value ) =
    { this with Id = value }

  member this.WithDisplayName( value ) =
    { this with DisplayName = value }

  member this.WithEmailAddress( value ) =
    { this with EmailAddress = value }

  member this.WithBio( value ) =
    { this with Bio = value }

  member this.WithTwitter( value ) =
    { this with Twitter = value }

  member this.WithPermissions( value ) =
    { this with Permissions = value }

  member this.WithPasswordHash( value ) =
    { this with PasswordHash = value }

  member this.WithWebsite( value ) =
    { this with Website = value }

  member this.WithSessionToken( value ) =
    { this with SessionToken = value }

  member this.WithSessionExpires( value ) =
    { this with SessionExpires = value }
