namespace DevSpace.FSharp.Common

type Sponsor =
  {
    Id:int;
    Level:SponsorLevel;
    DisplayName:string;
    LogoLarge:string;
    LogoSmall:string;
    Website:string
  }

  member this.WithId( value ) =
    { this with Id = value }

  member this.WithLevel( value ) =
    { this with Level = value }

  member this.WithDisplayName( value ) =
    { this with DisplayName = value }

  member this.WithLogoLarge( value ) =
    { this with LogoLarge = value }

  member this.WithLogoSmall( value ) =
    { this with LogoSmall = value }

  member this.WithWebsite( value ) =
    { this with Website = value }
