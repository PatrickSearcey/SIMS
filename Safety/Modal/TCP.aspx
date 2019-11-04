<%@ Page Title="" Language="C#" MasterPageFile="~/Modal.Master" AutoEventWireup="true" CodeBehind="TCP.aspx.cs" Inherits="Safety.TCP" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function CloseModal() {
            //create the argument that will be returned to the parent page
            var oArg = new Object();
            oArg.type = "tcp";
            //get a reference to the current RadWindow
            var oWnd = GetRadWindow();
            //Close the RadWindow and send the argument to the parent page
            oWnd.close(oArg);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <h3><asp:Literal ID="ltlSiteNoName" runat="server" /></h3>
    <asp:Literal ID="ltlError" runat="server" />
    <asp:Panel ID="pnlTCP" runat="server" CssClass="pnlTCP">
        <h3><asp:Literal ID="ltlPlanTitle" runat="server" /></h3><h4><asp:Literal ID="ltlPlanSubTitle" runat="server" /></h4>
        <div>
            <table width="100%">
                <tr>
                    <td>
                        <b>Last Reviewed:</b> <asp:Literal ID="ltlReviewed" runat="server" />
                    </td>
                    <td>
                        <b>Last Approved:</b> <asp:Literal ID="ltlApproved1" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <b>Remote Site:</b> <asp:Literal ID="ltlRemoteSite" runat="server" /><br />
        <b>Highway or Road:</b> <asp:Literal ID="ltlRoadName" runat="server" /><br />
        <asp:Panel ID="pnlPlanInfo" runat="server">
            <b>Expressway:</b> <asp:Literal ID="ltlExpressway" runat="server" /><br />
            <b>Bridge Width:</b> <asp:Literal ID="ltlBridgeWidth" runat="server" /><br />
            <b>Work Zone (Bridge Length):</b> <asp:Literal ID="ltlWorkZone" runat="server" /><br />
            <b>Lane Width:</b> <asp:Literal ID="ltlLaneWidth" runat="server" /><br />
            <b>Shoulder Width:</b> <asp:Literal ID="ltlShoulderWidth" runat="server" /><br />
            <b>Speed Limit:</b> <asp:Literal ID="ltlSpeedLimit" runat="server" /><br />
            <b>Lane Number:</b> <asp:Literal ID="ltlLaneNumber" runat="server" /><br />
            <b>Traffic Flow Two-Way:</b> <asp:Literal ID="ltlFlow2Way" runat="server" /><br />
            <b>Divided Highway:</b> <asp:Literal ID="ltlDividedHighway" runat="server" /><br />
            <b>Median:</b> <asp:Literal ID="ltlMedian" runat="server" /><br />
            <b>Flaggers:</b> <asp:Literal ID="ltlFlaggers" runat="server" /><br />
        </asp:Panel>
        <b>Traffic Volume:</b> <asp:Literal ID="ltlTrafficVolume" runat="server" /><br />
        <b>Site Specific Notes:</b><br />
        <asp:Literal ID="ltlSiteSpecificNotes" runat="server" /><br />
        <b>Plan Specific Notes:</b><br />
        <asp:Literal ID="ltlPlanSpecificNotes" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlReview" runat="server">
        <h3>Review TCP and Submit for Approval</h3>
        <p>If the Traffic Control Plan for this site has been updated as needed, please confirm that you have reviewed the information above before sending it for approval.</p>
        
        <p>Indicate below if no changes have been made since the last approval, and/or enter comments for the approver.  The check box is optional,
            but you must <b>enter comments for the approver</b> and click the <b>Send for Approval</b> button.
        </p>
        <p><b>Last Approved:</b> <asp:Literal ID="ltlApproved2" runat="server" /></p>
        <p style="font-weight:bold;">Check box if no changes have been made since last approval: <asp:CheckBox ID="cbNoChanges" runat="server" /></p>
        <p><b>Reviewer Comments:</b><br />
            <asp:TextBox ID="tbReviewerComments" runat="server" Width="600px" Height="100px" TextMode="MultiLine" />
        </p>
        <telerik:RadButton ID="rbSubmit1" runat="server" Text="Send for Approval" OnClick="ReviewClicked" />
        <telerik:RadButton ID="rbCancel1" runat="server" Text="Cancel" OnClick="Cancel" />
    </asp:Panel>
    <asp:Panel ID="pnlApprove" runat="server">
        <h3>Approve the TCP</h3>
        <asp:Literal ID="ltlNoChanges" runat="server" />
        <p><b>Reviewer Comments:</b><br />
            <asp:Literal ID="ltlReviewerComments" runat="server" />
        </p>
        <p><b>After approving, please submit the following:</b><br />
            Approved time: <asp:Literal id="ltlApprovedDt" runat="server" />
            Approved by: <asp:Literal ID="ltlApprovedBy" runat="server" /> &nbsp;&nbsp;&nbsp;
            <span style="color:red;font-weight:bold;">
                <asp:Literal ID="ltlNotApprover" runat="server" Text="You are not a safety approver and therefore cannot approve the TCP." />
            </span>
            <telerik:RadButton ID="rbSubmit2" runat="server" Text="Submit Approval" OnClick="ApproveClicked" />
            <telerik:RadButton ID="rbCancel2" runat="server" Text="Cancel" OnClick="Cancel" />
        </p>
    </asp:Panel>
</asp:Content>
