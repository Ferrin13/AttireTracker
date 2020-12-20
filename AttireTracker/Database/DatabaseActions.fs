module DatabaseActions

open Npgsql
open DatabaseModels
open Dapper
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

let getAllAttirePieces() =
    async {
        use connection = getConnection()

        let sql = @"
            SELECT
                attire_piece_id as id,
                name,
                rfid_uid,
                photo_url
            FROM
                attire_piece"

        let! results = connection.QueryAsync<AttirePiece>(sql) |> Async.AwaitTask
        return results 
    }

let getAttirePieceByUid(rfidUid: string) =
    async {
        use connection = getConnection()

        let sql = @"
            SELECT
                attire_piece_id as id,
                name,
                rfid_uid,
                photo_url
            FROM
                attire_piece
            WHERE
                rfid_uid = @rfidUid"

        let! results = connection.QueryAsync<AttirePiece>(sql, {|rfidUid = rfidUid|}) |> Async.AwaitTask
        let result = results |> List.ofSeq |> Seq.tryHead
        return result
    }

let addToActivityHistory(rfidUid: string, activityTypeId: ActivityTypeId) =
    async {
        let connection = getConnection()

        let time = DateTimeOffset.Now;
        let sql = @"
            INSERT INTO attire_piece_activity_history
            (attire_piece_id, activity_type_id, activity_time)
            SELECT attire_piece_id, @activityTypeId, @time
            FROM attire_piece
            WHERE rfid_uid = @rfidUid
            LIMIT 1"

        let queryParams = {|rfidUid = rfidUid; time = time; activityTypeId = activityTypeId |}
        let! res = connection.ExecuteAsync(sql, queryParams) |> Async.AwaitTask
        return res
    }

let getActivityHistoryByUid(rfidUid: string) =
    async {
        use connection = getConnection()
    
        let sql = @"
            SELECT 
            	attire_piece_activity_history_id,
            	description,
            	activity_time
            FROM
            	attire_piece_activity_history APAH
            	JOIN attire_piece AP
            		ON AP.attire_piece_id = APAH.attire_piece_id
            	JOIN attire_activity_type AAT
            		ON AAT.attire_activity_type_id = APAH.activity_type_id	
            WHERE
            	AP.rfid_uId = @rfidUid
            ORDER BY 
                attire_piece_activity_history_id DESC"
    
        let! results = connection.QueryAsync<CombinedActivityHistory>(sql, {|rfidUid = rfidUid|}) |> Async.AwaitTask
        return results |> List.ofSeq
    }

let insertAttirePiece(attirePiece: NewAttirePiece) =
    async {
        let connection = getConnection()

        let sql = @"
            INSERT INTO attire_piece
            (name, rfid_uid, photo_url)
            VALUES (@name, @rfidUid, @photoUrl)"

        let! res = connection.ExecuteAsync(sql, attirePiece) |> Async.AwaitTask
        return res
    }
    