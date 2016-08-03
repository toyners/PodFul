namespace PodFul.Library

open System.Collections.Generic
open System.IO
open System.Linq
open Jabberwocky.Toolkit.String

type NativeFileStorage(directoryPath) = 
    inherit JSONFileStorage(directoryPath)

    override this.ReadFeedFromFile filePath = NativeFeedIO.ReadFeedFromFile filePath
    override this.WriteFeedToFile feed filePath = NativeFeedIO.WriteFeedToFile feed filePath