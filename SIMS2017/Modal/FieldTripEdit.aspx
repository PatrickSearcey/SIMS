<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FieldTripEdit.aspx.cs" Inherits="SIMS2017.Modal.FieldTripEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Field Trips</title>
    <style type="text/css">
        body {
            background:white;
            font-family: Arial, Helvetica, sans-serif;
            min-height:500px;
            width:100%;
            margin:0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="rsm2" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadFormDecorator ID="rfd2" runat="server" DecoratedControls="all" Skin="Office2010Silver" RenderMode="Lightweight"></telerik:RadFormDecorator>
    <telerik:RadStyleSheetManager ID="rssm2" runat="server" CdnSettings-TelerikCdn="Enabled" />
    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function returnToParent() {
            //create the argument that will be returned to the parent page
            var oArg = new Object();
 
            //get the selection choices from the RadListBox
            var rlbFieldTripsEnd = document.getElementById("<%= rlbFieldTripsEnd.ClientID %>").value;
            oArg.FieldTrips = rlbFieldTripsEnd;

            //get a reference to the current RadWindow
            var oWnd = GetRadWindow();
 
            //Close the RadWindow and send the argument to the parent page
            oWnd.close(oArg);
        }
    </script>
    <div>
        <table>
            <tr>
                <td valign="top">
                    <center><p>Options</p></center>
                    <div style="padding-top:5px;">
                        <telerik:RadListBox Height="150px" runat="server" ID="rlbFieldTripsStart" EnableDragAndDrop="true" TransferToID="rlbFieldTripsEnd" AllowTransfer="true" Width="450px" Skin="Bootstrap" />
                    </div>
                </td>
                <td valign="top">
                    <center><p>Selected</p></center>
                    <div style="padding-top:5px;">
                        <telerik:RadListBox Height="150px" runat="server" ID="rlbFieldTripsEnd" EnableDragAndDrop="true" Width="450px" Skin="Bootstrap" />
                    </div>
                </td>
            </tr>
        </table>
        <telerik:RadButton ID="rbClose" runat="server" Text="Accept changes and close window" OnClientClicked="returnToParent(); return false;" />
    </div>
    </form>
</body>
</html>
