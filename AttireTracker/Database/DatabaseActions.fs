module DatabaseActions

open Npgsql
open DatabaseModels
open Dapper
open System

let username = Environment.GetEnvironmentVariable("ATTIRE_TRACKER_DB_USERNAME")
let password = Environment.GetEnvironmentVariable("ATTIRE_TRACKER_DB_PASSWORD")
let host = Environment.GetEnvironmentVariable("ATTIRE_TRACKER_DB_HOST")
let database = Environment.GetEnvironmentVariable("ATTIRE_TRACKER_DB_DATABASE")

let connString = "Host=" + host + ";Username=" + username + ";Password=" + password + ";Database=" + database;

Dapper.DefaultTypeMap.MatchNamesWithUnderscores <- true

let getConnection() =
    let connection = new NpgsqlConnection(connString)
    connection.Open() |> ignore
    connection

let getAllAttirePieces() =
    async {
        use connection = getConnection()

        let sql = @"
            SELECT
                attire_piece_id as id,
                name,
                photo_url
            FROM
                attire_piece"

        let! res = connection.QueryAsync<AttirePiece>(sql) |> Async.AwaitTask
        return res
    }

let insertAttirePiece(attirePiece: NewAttirePiece) =
    async {
        let connection = getConnection()

        let sql = @"
            INSERT INTO attire_piece
            (name, photo_url)
            VALUES (@name, @photoUrl)"

        let! res = connection.ExecuteAsync(sql, attirePiece) |> Async.AwaitTask
        return res
    }
    