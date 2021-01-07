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
                AP.name,
                rfid_uid,
                photo_url,
                category_id
            FROM
                attire_piece AP
                JOIN attire_piece_category APC
                    ON APC.attire_piece_category_id = AP.category_id"

        let! results = connection.QueryAsync<AttirePiece>(sql) |> Async.AwaitTask
        return results 
    }

let getAttirePieceByUid(rfidUid: string) =
    async {
        use connection = getConnection()

        let sql = @"
            SELECT
                attire_piece_id as id,
                AP.name,
                rfid_uid,
                photo_url,
                category_id
            FROM
                attire_piece AP
                JOIN attire_piece_category APC
                    ON APC.attire_piece_category_id = AP.category_id
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
            (name, rfid_uid, photo_url, attire_piece_type_id, type_id)
            VALUES (@name, @rfidUid, @photoUrl, @typeId)"

        let! res = connection.ExecuteAsync(sql, attirePiece) |> Async.AwaitTask
        return res
    }
