namespace DevSpace.FSharp.Database

open System.Threading.Tasks

type DataStore<'t>() = 
    abstract member Get : int -> Task<'t option>
    abstract member Get : unit -> Task<'t list>
    abstract member Get : string * obj -> Task<'t list>
    abstract member Add : 't -> Task<'t>
    abstract member Update : 't -> Task<'t>
    abstract member Delete : int -> Task<bool>

    default this.Get id =
      raise ( System.NotImplementedException() )

    default this.Get () =
      raise ( System.NotImplementedException() )

    default this.Get (field,value) =
      raise ( System.NotImplementedException() )

    default this.Add itemToAdd =
      raise ( System.NotImplementedException() )

    default this.Update itemToDelete =
      raise ( System.NotImplementedException() )

    default this.Delete id =
      raise ( System.NotImplementedException() )