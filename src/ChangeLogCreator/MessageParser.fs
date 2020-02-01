﻿namespace ChangeLogCreator

open System
open System.Text

module MessageParser =
               
    //
    // Tokenizer 
    //

    type Token =
        | StringToken of string     // any string value
        | OpenParenthesisToken      // '('
        | CloseParenthesisToken     // ')'
        | ColonAndSpaceToken        // ': '
        | LineBreakToken of string  // '\r\n' or '\n'
        | ExclamationMarkToken      // '!'
        | SpaceAndHashToken         // ' #'
        | EofToken                  // end of input / last token
    
    let tokenize (input: string) =
        seq {

            let getStringTokenAndReset (stringBuilder: StringBuilder) =
                let value = stringBuilder.ToString()
                stringBuilder.Clear() |> ignore
                (StringToken value)

            let currentValueBuilder = StringBuilder()

            let getSingleCharToken char =
                match char with 
                    | '(' -> Some OpenParenthesisToken
                    | ')' -> Some CloseParenthesisToken
                    | '!' -> Some ExclamationMarkToken
                    | _ -> None
           
            let mutable i = 0
            while i < input.Length  do
                let nextChar =  if i + 1 < input.Length then Some input.[i+1] else None
                let currentCharToken = getSingleCharToken input.[i]                

                match input.[i], nextChar, currentCharToken with                    
                    | _, _, Some matchedCharToken ->
                        if currentValueBuilder.Length > 0 then
                            yield getStringTokenAndReset currentValueBuilder
                        yield matchedCharToken
                    | '\r', Some '\n', _ ->
                        if currentValueBuilder.Length > 0 then
                            yield getStringTokenAndReset currentValueBuilder
                        yield LineBreakToken "\r\n"
                        // next already matched => skip next iteration
                        i <- i + 1
                    | '\n', _, _ -> 
                        if currentValueBuilder.Length > 0 then
                            yield getStringTokenAndReset currentValueBuilder
                        yield LineBreakToken "\n"
                    | ':', (Some ' '), _ ->
                        if currentValueBuilder.Length > 0 then
                            yield getStringTokenAndReset currentValueBuilder
                        yield ColonAndSpaceToken   
                        // next already matched => skip next iteration
                        i <- i + 1
                    | ' ', (Some '#'), _ ->
                        if currentValueBuilder.Length > 0 then
                            yield getStringTokenAndReset currentValueBuilder
                        yield SpaceAndHashToken   
                        // next already matched => skip next iteration
                        i <- i + 1
                    | _ ->  currentValueBuilder.Append(input.[i]) |> ignore

                i <- i + 1

            // if any input is left in currentValueBuilder, return it as StringToken
            if currentValueBuilder.Length > 0 then
                yield getStringTokenAndReset currentValueBuilder

            yield EofToken
        }

    let tokenToString (token : Token) : string =
        match token with 
            | StringToken str -> str
            | OpenParenthesisToken -> "("
            | CloseParenthesisToken -> ")"
            | ColonAndSpaceToken -> ": "
            | LineBreakToken lb -> lb
            | ExclamationMarkToken -> "!"
            | SpaceAndHashToken -> " #"
            | EofToken -> ""

    // 
    // Parser
    //
    type ParseError =
        | EmptyInput
        | UnspecifiedError
        | EmptyText
        | UnexpectedToken of Token * Token

    type ConventionalCommit = {
        Type : string
        Scope : string option
        Description : string
    }
    
    type ParseResult =
        | Parsed of ConventionalCommit
        | Failed of ParseError

    let parse (input: string) : ParseResult =                 

        //
        // parsing helper functions
        //
        let checkForError parseLogic (state:ParseResult) (tokens: Token list) : ParseResult * (Token list)  = 
            match state with 
                | Failed parseError -> Failed parseError,tokens
                | _ -> parseLogic state tokens
           
        // matching a single token
        let matchToken expectedToken state tokens : ParseResult * (Token list) =
            match tokens with
                | head::tail ->
                    if head = expectedToken then
                        state,tail
                    else
                        Failed (UnexpectedToken (head,expectedToken)),tokens
                | [] -> Failed EmptyInput,tokens
        
        let updateParsed dataUpdater state value =
            let currentData = 
                match state with 
                    | Parsed d -> d
                    | _ -> raise (InvalidOperationException "Data from invalid ParseResult requested. 'matchString' should not be called without error checking")
            dataUpdater currentData value

        // matches a string token and updates the parsed data using the specified function
        let matchString dataUpdater state tokens = 
            match tokens with
                | head::tail -> 
                    match head with 
                        | StringToken strValue -> Parsed (updateParsed dataUpdater state strValue),tail
                        | t -> Failed (UnexpectedToken (t, StringToken "")),tokens
                | [] -> Failed EmptyInput,tokens

        let testToken (tokens: Token list) (expectedToken:Token) =
            match tokens with
                | head::_ -> head = expectedToken                    
                | [] -> false

        let matchTextLine dataUpdater state tokens =
            let processableTokens = 
                tokens 
                    |> List.takeWhile (fun token ->
                        match token with
                            | StringToken _  -> true
                            | OpenParenthesisToken -> true
                            | CloseParenthesisToken -> true
                            | ColonAndSpaceToken -> true
                            | ExclamationMarkToken -> true
                            | SpaceAndHashToken -> true
                            | EofToken -> false
                            | LineBreakToken _ -> false)

            if (List.isEmpty processableTokens) then
                Failed EmptyText, tokens
            else
                let strValue = 
                    processableTokens  
                    |> Seq.map tokenToString
                    |> Seq.reduce( fun a b -> a + b)                        
                Parsed (updateParsed dataUpdater state strValue), tokens |> List.skip(Seq.length processableTokens)                        
        
        //
        // high-level parsing functions
        //
        let ensureNotEmpty state tokens =
                match tokens with
                    | head::_ ->
                        match head with
                            | EofToken -> Failed EmptyInput,tokens
                            | _ -> state,tokens
                    | [] -> Failed EmptyInput,tokens

        let parseType  = checkForError (matchString (fun data value -> { data with Type = value }))

        let parseDescription = checkForError (matchTextLine (fun data value -> { data with Description = value }))

        let parseToken token = checkForError (matchToken token)

        let parseScope state tokens : ParseResult * (Token list) =
            let doParseScope (state:ParseResult) (tokens: Token list) : ParseResult * (Token list) = 
                if (testToken tokens OpenParenthesisToken) then                                    
                        (state,tokens)
                            ||> matchToken OpenParenthesisToken
                            ||> matchString (fun data value -> { data with Scope = Some value })
                            ||> matchToken CloseParenthesisToken                
                else                
                    state,tokens
            checkForError doParseScope state tokens
           
        let ignoreTrailingLineBreaks state tokens =
            let rec doIgnoreTrailingLineBreaks state tokens =
                match tokens with
                    | head::tail ->
                        match head with 
                            | LineBreakToken _ -> doIgnoreTrailingLineBreaks state tail
                            | _ -> state,tokens
                    | [] -> state, tokens
            checkForError doIgnoreTrailingLineBreaks state tokens

        let tokens = tokenize input |> List.ofSeq

        let result,_ = 
            (Parsed { Type = ""; Scope = None; Description = "" }, tokens)
                ||> ensureNotEmpty
                ||> parseType
                ||> parseScope
                ||> parseToken ColonAndSpaceToken
                ||> parseDescription
                ||> ignoreTrailingLineBreaks
                ||> parseToken EofToken                           
        result

        