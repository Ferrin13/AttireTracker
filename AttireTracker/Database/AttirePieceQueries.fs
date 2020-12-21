module AttirePieceQueries

open DatabaseModels
open Dapper
open DatabaseActions

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
