<%@ Page Title="" Language="C#" MasterPageFile="~/SafetySingleMenu.Master" AutoEventWireup="true" CodeBehind="TCPView.aspx.cs" Inherits="Safety.TCPView" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/reports.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <asp:Panel ID="pnlImage" runat="server">
            <asp:Image ID="imgPlanImage" runat="server" CssClass="TCPPlanImage" />
        </asp:Panel>

        <asp:Panel ID="pnlSiteInfo" runat="server" CssClass="pnlSiteInfo">
            <h4>Site Specific Info</h4>
            <b>Site Name: </b><asp:Literal ID="ltlName" runat="server" /><br />
            <b>Station Number: </b><asp:Literal ID="ltlNumber" runat="server" /><br />
            <b>Highway/Road: </b><asp:Literal ID="ltlHighway" runat="server" /><br />
            <b>Freeway or Expressway: </b><asp:Literal ID="ltlFreeway" runat="server" /><br />
            <b>Bridge Width: </b><asp:Literal ID="ltlWidth" runat="server" /><br />
            <b>Bridge Length: </b><asp:Literal ID="ltlWorkZone" runat="server" /><br />
            <b>Lane Width: </b><asp:Literal ID="ltlLane" runat="server" /><br />
            <b>Shoulder Width: </b><asp:Literal ID="ltlShoulder" runat="server" /><br />
            <b>Speed Limit: </b><asp:Literal ID="ltlSpeed" runat="server" /><br /> 
            <b>Traffic Volume: </b><asp:Literal ID="ltlTraffic" runat="server" /><br />
            <b>Cell Service Available: </b><asp:Literal ID="ltlCell" runat="server" /><br />
            <b>Plan Updated: </b><asp:Literal ID="ltlUpdated" runat="server" /><br /> 
            <b>Plan Reviewed: </b><asp:Literal ID="ltlReviewed" runat="server" /><br />
            <b>Plan Approved: </b><asp:Literal ID="ltlApproved" runat="server" /><br />           
            <b>Site Specific Notes:</b><br />
            <asp:Literal ID="ltlNotes" runat="server" />
            <br />
            <b>Remarks:</b><br />
            <asp:Literal ID="ltlRemarks" runat="server" />
        </asp:Panel>

        <asp:Panel ID="pnlAllData" runat="server" CssClass="pnlCalcData">
            <h4>TCP Calculator Data</h4>
            <b style='color:red;'>WS* &mdash; </b><b>Advanced</b> <b style='color:red;'> Warning Sign </b><b>Spacing per sign:</b> <asp:Literal ID="ltlWS" runat="server" /> feet<br />
            <b style='color:grey;'>F &mdash; Flagger</b> <b>Distance from the shoulder taper:</b> <asp:Literal ID="ltlFlagger" runat="server" /><br />
            <b style='color:orange;'>WZ** &mdash; Work Zone </b><b>Length**:</b> <asp:Literal ID="ltlWZLength" runat="server" /><br />
            <b style='color:orange;'>WZ** &mdash; </b><b>Number of cones in the</b> <b style='color:orange;'>Work Zone:</b> <asp:Literal ID="ltlWZCones" runat="server" /><br />
            <b style='color:orange;'>WZ** &mdash; Work Zone </b><b>cone spacing:</b> <asp:Literal ID="ltlWZConeSpacing" runat="server" /><br />
            <b style='color:mediumpurple;'>T &mdash; </b><b>Minimum </b><b style='color:mediumpurple';>Transition Taper length:</b> <asp:Literal ID="ltlTTLength" runat="server" /><br />
            <b style='color:mediumpurple;'>T &mdash; </b><b>Number of cones in the </b><b style='color:mediumpurple';>Transition Taper:</b> <asp:Literal ID="ltlTTCones" runat="server" /><br />
            <b style='color:mediumpurple;'>T &mdash; </b><b>Maximum spacing for cones in the </b><b style='color:mediumpurple';>Transition Taper:</b> <asp:Literal ID="ltlTTConeSpacing" runat="server" /><br />
            <b style='color:cornflowerblue;'>S &mdash; </b><b>Minimum </b><b style='color:cornflowerblue';>Shoulder Taper:</b> <asp:Literal ID="ltlSTMin" runat="server" /><br />
            <b style='color:cornflowerblue;'>S &mdash; </b><b>Number of cones in </b><b style='color:cornflowerblue'>Shoulder Taper:</b> <asp:Literal ID="ltlSTCones" runat="server" /><br />
            <b> &mdash; TOTAL NUMBER OF CONES REQUIRED:</b> <asp:Literal ID="ltlConesReq" runat="server" /><br />
            <b style='color:deeppink;'>B &mdash; </b><b>Optional </b><b style='color:deeppink';>Buffer Zone </b><b>length:</b> <asp:Literal ID="ltlBZLength" runat="server" /><br />
            <b style='color:deeppink;'>B &mdash; </b><b>Optional </b><b style='color:deeppink';>Buffer Zone </b><b>cone spacing:</b> <asp:Literal ID="ltlBZConeSpacing" runat="server" /><br />
            <b style='color:deeppink;'>B &mdash; </b><b>Number of cones in the optional </b><b style='color:deeppink';>Buffer Zone</b><b>:</b> <asp:Literal ID="ltlBZCones" runat="server" /><br />
            <b style='color:limegreen;'>D &mdash; </b><b>Optional </b><b style='color:limegreen';>Downstream Taper </b><b>length:</b> _____________<br />
            <b style='color:limegreen;'>D &mdash; </b><b>Optional </b><b style='color:limegreen';>Downstream Taper </b><b>cone spacing:</b> _______________<br />
            <b style='color:limegreen;'>D &mdash; </b><b>Number of cones in the optional </b><b style='color:limegreen';>Downstream Taper</b><b>:</b> _____________<br />
        </asp:Panel>

        <asp:Panel ID="pnlIVbData" runat="server" CssClass="pnlCalcData">
            <h4>TCP Calculator Data</h4>
            <b style='color:red;'>WS* &mdash; </b><b>Advanced</b> <b style='color:red;'> Warning Sign </b><b>Spacing per sign:</b> <asp:Literal ID="ltlWS1" runat="server" /><br />
            <b style='color:orange;'>WZ** &mdash; Work Zone </b><b>Length**:</b> <asp:Literal ID="ltlWZLength1" runat="server" /><br />
            <b style='color:orange;'>WZ** &mdash; </b><b>Number of cones in the</b> <b style='color:orange;'>Work Zone:</b> <asp:Literal ID="ltlWZCones1" runat="server" /><br />
            <b style='color:orange;'>WZ** &mdash; Work Zone </b><b>cone spacing:</b> <asp:Literal ID="ltlWZConeSpacing1" runat="server" /><br />
            <b style='color:cornflowerblue;'>S &mdash; </b><b>Minimum </b><b style='color:cornflowerblue';>Shoulder Taper:</b> <asp:Literal ID="ltlSTMin1" runat="server" /><br />
            <b style='color:cornflowerblue;'>S &mdash; </b><b>Number of cones in </b><b style='color:cornflowerblue'>Shoulder Taper:</b> <asp:Literal ID="ltlSTCones1" runat="server" /><br />
            <b> &mdash; TOTAL NUMBER OF CONES REQUIRED:</b> <asp:Literal ID="ltlConesReq1" runat="server" /><br />
            <b style='color:deeppink;'>B &mdash; </b><b>Optional </b><b style='color:deeppink';>Buffer Zone </b><b>length:</b> <asp:Literal ID="ltlBZLength1" runat="server" /><br />
            <b style='color:deeppink;'>B &mdash; </b><b>Optional </b><b style='color:deeppink';>Buffer Zone </b><b>cone spacing:</b> <asp:Literal ID="ltlBZConeSpacing1" runat="server" /><br />
            <b style='color:deeppink;'>B &mdash; </b><b>Number of cones in the optional </b><b style='color:deeppink';>Buffer Zone</b><b>:</b> <asp:Literal ID="ltlBZCones1" runat="server" /><br />
            <b style='color:limegreen;'>D &mdash; </b><b>Optional </b><b style='color:limegreen';>Downstream Taper </b><b>length:</b> _____________<br />
            <b style='color:limegreen;'>D &mdash; </b><b>Optional </b><b style='color:limegreen';>Downstream Taper </b><b>cone spacing:</b> _______________<br />
            <b style='color:limegreen;'>D &mdash; </b><b>Number of cones in the optional </b><b style='color:limegreen';>Downstream Taper</b><b>:</b> _____________<br />
        </asp:Panel>

        <asp:Panel ID="pnlLessData" runat="server" CssClass="pnlCalcData">
            <h4>TCP Calculator Data</h4>
            <b style='color:red;'>WS* &mdash; </b><b>Optional Advanced</b> <b style='color:red;'>Warning Sign</b> <b>Spacing per sign:</b> <asp:Literal ID="ltlWS2" runat="server" /><br />
            <b style='color:orange;'>WZ** &mdash; Work Zone </b><b>Length**:</b> <asp:Literal ID="ltlWZLength2" runat="server" /><br />
        </asp:Panel>
         
        <asp:Panel ID="pnlInstructions" runat="server" CssClass="pnlInstructions">
            <h4>Instructions</h4>
            <asp:Literal ID="ltlInstructions" runat="server" />
        </asp:Panel>
    </div>
</asp:Content>
