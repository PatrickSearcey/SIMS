using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ionic.Zip;
using System.IO;
using System.Data.Linq;

namespace SIMS2017.Handler
{
    /// <summary>
    /// Outputs a document from a blob field in the database, and also contains method for batch loading documents into the database.
    /// </summary>
    public class DocHandler : IHttpHandler
    {
        Data.SIMSDataContext db = new Data.SIMSDataContext();

        public void ProcessRequest(HttpContext context)
        {
            string task = context.Request.QueryString["task"];
            int docID = Convert.ToInt32(context.Request.QueryString["ID"]);

            if (task == "get") GetDoc(docID, context);
            else if (task == "getzip") ZipDocs(docID, context);
        }

        /// <summary>
        /// Used to create a zip file of all the supporting documents for an audit
        /// </summary>
        public void ZipDocs(int rms_audit_id, HttpContext context)
        {
            context.Response.ContentType = "application/zip";

            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    var docs = db.AuditDocuments.Where(p => p.rms_audit_id == rms_audit_id).ToList();
                    int x = 1;
                    foreach (var d in docs)
                    {
                        Byte[] file = db.AuditDocumentFiles.Where(p => p.rms_audit_document_id == d.rms_audit_document_id).Select(p => p.document_file.ToArray()).FirstOrDefault();
                        using (MemoryStream stream = new MemoryStream(file))
                        {
                            zip.AddEntry(x.ToString() + "_" + d.document_nm.Replace(" ", "_"), file);
                        }
                        x += 1;
                    }
                    string FileName = "SupportingDocs" + rms_audit_id.ToString() + ".zip";
                    context.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + FileName + "\"");
                    zip.Save(context.Response.OutputStream);
                }
            }
            catch (Exception)
            {
                throw;
            }
            context.Response.End();
        }

        public static byte[] GetDoc(string filePath)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);

            byte[] doc = reader.ReadBytes((int)stream.Length);

            reader.Close();
            stream.Close();

            return doc;
        }

        public void GetDoc(int docID, HttpContext context)
        {
            try
            {
                var doc = db.AuditDocuments.FirstOrDefault(p => p.rms_audit_document_id == docID);
                var docFile = db.AuditDocumentFiles.FirstOrDefault(p => p.rms_audit_document_id == docID);

                switch (doc.document_content_tp.ToLower())
                {
                    case "pdf":
                        context.Response.ContentType = "application/pdf";
                        break;
                    case "zip":
                        context.Response.ContentType = "application/zip";
                        break;
                    case "xlsx":
                        context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    case "docx":
                        context.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    case "txt":
                        context.Response.ContentType = "text/plain";
                        break;
                    case "gif":
                        context.Response.ContentType = "image/GIF";
                        break;
                    case "jpeg":
                    case "jpg":
                        context.Response.ContentType = "image/JPEG";
                        break;
                    case "png":
                        context.Response.ContentType = "image/PNG";
                        break;
                    default:
                        context.Response.ContentType = "application/pdf";
                        break;
                }

                Binary file = docFile.document_file;
                byte[] buffer = file.ToArray();

                context.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + doc.document_nm.Replace(" ", "_") + "." + doc.document_content_tp + "\"");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
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