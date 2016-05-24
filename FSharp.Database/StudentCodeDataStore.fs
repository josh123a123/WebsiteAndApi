namespace DevSpace.FSharp.Database

open Microsoft.FSharp.Control
open DevSpace.FSharp.Common
open System.Data
open System.Data.SqlClient

type StudentCodeDataStore() =
  inherit DataStore<StudentCode>()

  let connectionString = "Server=localhost;Database=DevSpace;Trusted_Connection=True;"

  let getStudentCodeFromDataReader (dataReader:SqlDataReader) : StudentCode = {
      Id    = dataReader.GetInt32 0;
      Email = dataReader.GetString 1;
      Code  = dataReader.GetString 2
    }

  let rec getSeqFromDataReader (dataReader:SqlDataReader) : StudentCode list =
    match dataReader.Read() with
    | true  -> getStudentCodeFromDataReader( dataReader ) :: getSeqFromDataReader(dataReader)
    | false -> []

  override this.Get id =    
    Async.StartAsTask( async {
      use connection = new SqlConnection( connectionString )
      connection.Open()

      use command = new SqlCommand( "SELECT Id, Email, Code FROM StudentCodes WHERE Id = @Id", connection )
      command.Parameters.Add( "Id", SqlDbType.Int ).Value <- id

      use! dataReader = command.ExecuteReaderAsync() |> Async.AwaitTask
      match dataReader.Read() with
      | true  -> return Some {
        Id    = dataReader.GetInt32 0;
        Email = dataReader.GetString 1;
        Code  = dataReader.GetString 2 }
      | false -> return None
    } )

  override this.Get( field, value ) =
    Async.StartAsTask( async {
      use connection = new SqlConnection( connectionString )
      connection.Open()

      use command = new SqlCommand( "SELECT Id, Email, Code FROM StudentCodes WHERE " + field + " = @value", connection )
      command.Parameters.AddWithValue( "value", value ) |> ignore

      use! dataReader = command.ExecuteReaderAsync() |> Async.AwaitTask
      return getSeqFromDataReader dataReader
    } )

  override this.Add itemToAdd =
    Async.StartAsTask( async {
      use connection = new SqlConnection( connectionString )
      connection.Open()

      use command = new SqlCommand( "INSERT StudentCodes ( Email, Code ) VALUES ( @Email, @Code ); SELECT SCOPE_IDENTITY();", connection )
      command.Parameters.Add( "Email", SqlDbType.VarChar ).Value <- itemToAdd.Email
      command.Parameters.Add( "Code", SqlDbType.VarChar ).Value <- itemToAdd.Code

      let! id = command.ExecuteScalarAsync() |> Async.AwaitTask
      return { itemToAdd with Id = System.Convert.ToInt32( id ) }
    } )
