using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMS.Handler
{
    /// <summary>
    /// Summary description for ClearLock
    /// </summary>
    public class ClearLock : IHttpHandler
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();

        public void ProcessRequest(HttpContext context)
        {
            string user_id = context.Request.QueryString["user_id"];
            string rms_record_id = context.Request.QueryString["rms_record_id"];
            string all = context.Request.QueryString["all"];

            if (!string.IsNullOrEmpty(rms_record_id))
            {
                var currRecord = db.Records.FirstOrDefault(p => p.rms_record_id == Convert.ToInt32(rms_record_id));

                if (currRecord != null)
                {
                    //Clear the lock, but only the "lock" type, not the "save" type
                    if (currRecord.RecordLock.lock_type != "Analyzing" || currRecord.RecordLock.lock_type != "Approving")
                    {
                        db.RecordLocks.DeleteOnSubmit(currRecord.RecordLock);
                        db.SubmitChanges();
                    }
                    else if (all == "true")
                    {
                        db.RecordLocks.DeleteOnSubmit(currRecord.RecordLock);
                        db.SubmitChanges();
                    }
                }
            }

            if (!string.IsNullOrEmpty(user_id))
            {
                if (all == "true") //Delete all locks, no matter what
                    db.RecordLocks.DeleteAllOnSubmit(db.RecordLocks.Where(p => p.lock_uid == user_id));
                else //Delete only "locks", not "saves"
                    db.RecordLocks.DeleteAllOnSubmit(db.RecordLocks.Where(p => p.lock_uid == user_id && p.lock_type == "Analyze" || p.lock_uid == user_id && p.lock_type == "Approve" || p.lock_uid == user_id && p.lock_type == "Reanalyze"));
                db.SubmitChanges();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}