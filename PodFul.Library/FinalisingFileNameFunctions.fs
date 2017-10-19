namespace PodFul.Library

open Jabberwocky.Toolkit.String
open System.Collections.Generic

module public FinalisingFileNameFunctions = 

    let illegalCharacterSubstitutes = Dictionary<string, string>(dict
                                        [
                                            ( "\\", "-bs-" );
                                            ( "/", "-fs-" );
                                            ( ":", "-c-" );
                                            ( "*", "-a-" );
                                            ( "?", "-q-" );
                                            ( "\"", "-qu-" );
                                            ( "<", "-lt-" );
                                            ( ">", "-gt-" );
                                            ( "|", "-b-" )
                                        ])

    let finaliseUsingStandardAlgorithm (podcasts : list<Podcast>) : bool * list<Podcast> = 
        raise (new System.NotImplementedException())

    let finaliseUsingAlternateAlgorithm (podcasts : list<Podcast>) : bool * list<Podcast> = 
        raise (new System.NotImplementedException())

    let substituteBadFileNameCharacters (fileName : string) : string = 
        fileName.Substitute(illegalCharacterSubstitutes)