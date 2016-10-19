<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RemarksElems.aspx.vb" Inherits="SIMS.RemarksElems" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>REMARKS</title>
    <link rel="stylesheet" type="text/css" href="../css/styles.css" />
    <link rel="stylesheet" type="text/css" href="../css/common.css" />
    <link rel="stylesheet" type="text/css" href="../css/custom.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="padding:5px;">
        <p style="text-align: center;font-weight:bold;">
            REMARKS:
        </p>
        <p>
            Because the NWISWeb product is generated on demand by the NWISWeb user, date sensitive entries must be associated with an actual date. For example, an old entry of 
            "Records good except estimated daily discharges which are poor." can be updated to "10/01/2013-09/30/2014: Records good except estimated daily discharges which are poor." 
            Another option is to make the rating generalized for the gage "Records good except for estimated daily discharges which are poor and other periods as noted. 06/01/2014- 
            08/01/2014: Records fair due to reservoir releases at Strawberry Scone Dam." Multiple time period information may be noted using a ";" and a new date period. For more 
            information regarding this element, 
            <a href="https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/PADRE/FAQ.html" target="_blank">check the FAQ page</a>.
        </p>
    </div>
    </form>
</body>
</html>
