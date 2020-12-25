module DatabaseActions

open Npgsql
open System

let username = Environment.GetEnvironmentVariable("ATTIRE_TRACKER_DB_USERNAME")
let password = Environment.GetEnvironmentVariable("ATTIRE_TRACKER_DB_PASSWORD")
let database = Environment.GetEnvironmentVariable("ATTIRE_TRACKER_DB_DATABASE")
let host = Environment.GetEnvironmentVariable("ATTIRE_TRACKER_DB_HOST")
let port = Environment.GetEnvironmentVariable("ATTIRE_TRACKER_DB_PORT")

let connString = "Host=" + host + ";Port=" + port + ";Username=" + username + ";Password=" + password + ";Database=" + database;

Dapper.DefaultTypeMap.MatchNamesWithUnderscores <- true

printfn "Host: %s Port: %s" host port

let getConnection() =
    let connection = new NpgsqlConnection(connString)
    connection.Open() |> ignore
    connection
