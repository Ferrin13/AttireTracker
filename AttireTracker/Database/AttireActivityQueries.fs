module AttireActivityQueries

open DatabaseModels
open Dapper
open System
open DatabaseActions

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
                activity_type_id,
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

let getLastActivityByUid(rfidUid: string) =
    async {
        use connection = getConnection()
        
        let sql = @"
            SELECT 
                attire_piece_activity_history_id,
                activity_type_id,
                description,
                activity_time
            FROM
                attire_piece_activity APA
                JOIN attire_piece AP
                	ON AP.attire_piece_id = APA.attire_piece_id
                JOIN attire_activity_type AAT
                	ON AAT.attire_activity_type_id = APA.activity_type_id	
            WHERE
                AP.rfid_uId = @rfidUid
            ORDER BY 
                attire_piece_activity_history_id DESC"
        
        let! results = connection.QueryAsync<CombinedActivityHistory>(sql, {|rfidUid = rfidUid|}) |> Async.AwaitTask
        return results |> List.ofSeq |> Seq.tryHead
    }
