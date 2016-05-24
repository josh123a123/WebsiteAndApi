namespace DevSpace.FSharp.Common

type SponsorLevel =
  {
    Id:int;
    DisplayOrder:int;
    DisplayName:string;
    DisplayInSidebar:bool;
    DisplayInEmails:bool
  }

  member this.WithId( value ) =
    { this with Id = value }

  member this.WithDisplayOrder( value ) =
    { this with DisplayOrder = value }

  member this.WithDisplayName( value ) =
    {this with DisplayName = value }

  member this.WithDisplayInSidebar( value ) =
    { this with DisplayInSidebar = value }

  member this.WithDisplayInEmails( value ) =
    { this with DisplayInEmails = value }
