<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FootnotesElems.aspx.vb" Inherits="SIMS.FootnotesElems" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>FOOTNOTES</title>
    <link rel="stylesheet" type="text/css" href="../css/styles.css" />
    <link rel="stylesheet" type="text/css" href="../css/common.css" />
    <link rel="stylesheet" type="text/css" href="../css/custom.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="padding:5px;">
        <p style="text-align: center;font-weight:bold;">
            FOOTNOTES:
        </p>
        <p>FOOTNOTES have been deprecated, and are no longer shown to the public.  All footnotes must be migrated to a more suitable element, or deleted.  
        These tips may help you decide how to handle any footnotes:</p>
        <ul class="fancyList">
            <li>Footnotes can be deleted if they simply re-state things easily observed in tables or summaries (e.g., daily maximum discharge for a water year).</li>
            <li>If a footnote contains data that can be entered in the peak flow file (PFF), then do so.</li>
            <li>Descriptions of values measured outside of the period of record (average, minimum, etc.) can be migrated to elements like EXTREMES OUTSIDE PERIOD OF 
                RECORD or EXTREMES FOR PERIOD PRIOR TO REGULATION.</li>
            <li>Methodological details regarding method of computation of a peak flow can be migrated to EXTREMES FOR PERIOD OF RECORD.</li>
            <li>Remarks indicating that zero flow is common can be migrated to EXTREMES FOR PERIOD OF RECORD.</li>
            <li>Footnotes about significant factors affecting flow at the gage (for example, an upstream diversion) can be migrated to REMARKS or GAGE.</li>
            <li>If no other suitable location can be determined for a footnote, and it needs to be maintained and delivered to the public, it might be suited for REMARKS.  
                If the public does not need to read it, one or more Station Analysis elements might be appropriate.</li>
        </ul>
    </div>
    </form>
</body>
</html>
