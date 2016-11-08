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
    <telerik:RadFormDecorator ID="rfd2" runat="server" DecoratedControls="all" Skin="Bootstrap" RenderMode="Lightweight" />
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

            //get the selected items from RadListBox
            var fieldTrips = $find("<%= rlbFieldTripsEnd.ClientID %>");
            var items = fieldTrips.get_items();
            if (items) {
                var trip_ids = "";
                items.forEach(function (item) {
                    trip_ids += item.get_value() + ",";
                })
                oArg.fieldTrips = trip_ids;
            }
            else {
                oArg.fieldTrips = "";
            }
            
            //get a reference to the current RadWindow
            var oWnd = GetRadWindow();

            //Close the RadWindow and send the argument to the parent page
            oWnd.close(oArg);
        }
        </script>
    <div>
        <table>
            <tr>
                <td>
                    <h4>Options</h4>
                    <div style="padding: 5px 0 10px 0;">
                        <telerik:RadListBox Height="170px" runat="server" ID="rlbFieldTripsStart" EnableDragAndDrop="true" TransferToID="rlbFieldTripsEnd" AllowTransfer="true" Width="400px" Skin="Bootstrap" DataTextField="TripName" DataValueField="trip_id" />
                    </div>
                </td>
                <td>
                    <h4>Selected</h4>
                    <div style="padding: 5px 0 10px 0;">
                        <telerik:RadListBox Height="170px" runat="server" ID="rlbFieldTripsEnd" EnableDragAndDrop="true" Width="400px" Skin="Bootstrap" DataTextField="TripName" DataValueField="trip_id" />
                    </div>
                </td>
            </tr>
        </table>
        <button title="Commit" id="close" onclick="returnToParent(); return false;">Commit changes and close</button>
    </div>
    </form>
</body>
</html>
